using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaButiken.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PizzaButiken.Models;
using Microsoft.AspNetCore.Http;
using PizzaButiken.Services;

namespace PizzaButiken.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrderController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, 
            OrderService orderService, CartService cartService)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _cartService = cartService;
        }

        public IActionResult OrderDetails(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCartItem(int cartItemId)
        {
            _cartService.DeleteItemForCurrentSession(HttpContext.Session, cartItemId);
            return RedirectToAction("OrderDetails");
        }

        [HttpPost]
        public IActionResult OrderDetailsAction(IFormCollection form)
        {
            var key = form.Keys.FirstOrDefault(k => k.Contains("-"));
            var dashPos = key.IndexOf("-");
            var action = key.Substring(0, dashPos);
            var id = int.Parse(key.Substring(dashPos + 1));
            switch (action)
            {
                case "remove": _cartService.DeleteItemForCurrentSession(HttpContext.Session, id); break;
                case "increaseQuantity": _cartService.IncreaseCartItemQuantity(id); break;
                case "decreaseQuantity": _cartService.DecreaseCartItemQuantity(id); break;
            }

            return RedirectToAction("OrderDetails");
        }

        public IActionResult Checkout(string returnUrl = null)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);
            var order = _orderService.GetOrder(cartId);

            if (order == null)
            {
                var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
                var appUser = new ShippingAddress
                {
                    CustomerName = user?.CustomerName,
                    Email = user?.Email,
                    PhoneNumber = user?.PhoneNumber,
                    Street = user?.Street,
                    PostalCode = user?.PostalCode,
                    City = user?.City
                };
                ViewData["ReturnUrl"] = returnUrl;
                return View(appUser);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(order.ShippingAddress);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("CustomerName, Email, PhoneNumber, Street, PostalCode, City")]ShippingAddress user)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);
            var order = _orderService.GetOrder(cartId);
            if (ModelState.IsValid)
            {
                if (order != null)
                {
                    _orderService.UpdateAddress(cartId, user);
                }
                _orderService.CreateOrder(cartId, user);

                return View("OrderPayment", user);
            }

            return View(user);
        }

        public IActionResult OrderPayment(ShippingAddress user)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);
            var order = _orderService.GetOrder(cartId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderPayment(string userEmail)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);

            _orderService.SetOrderToPaid(cartId);

            _orderService.SendConfirmationEmail(cartId, userEmail);

            HttpContext.Session.Clear();

            return View("ThankYouPage");
        }

        public IActionResult Bake()
        {
            var orders = _orderService.GetOrdersToBake(); 
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetToBaked(Order order)
        {
            _orderService.SetOrderToBaked(order);

            return RedirectToAction("Bake");
        }

        public IActionResult Ship()
        {
            var orders = _orderService.GetOrdersToShip();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetToShipped(Order order)
        {
            _orderService.SetOrderToShipped(order);

            return RedirectToAction("Ship");
        }
    }
}