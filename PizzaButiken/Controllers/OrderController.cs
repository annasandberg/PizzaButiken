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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, OrderService orderService,
            CartService cartService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("CustomerName, Email, PhoneNumber, Street, PostalCode, City")]ShippingAddress user)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);
            if (ModelState.IsValid)
            {
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
            var order = _orderService.GetOrder(cartId);
            order.Paid = true;
            _context.Update(order);
            _context.SaveChanges();

            string message = String.Format("Orderdatum: {0} Summa: {1} SEK Fraktavgift: {2} SEK",
            order.OrderDate.ToString(), order.TotalPrice.ToString(), order.ShippingFee.ToString());
            _emailSender.SendEmailAsync(userEmail, "Tack för din beställning", message);

            HttpContext.Session.Clear();

            return View("ThankYouPage");
        }

        public IActionResult Bake()
        {
            var orders = _context.Orders.Include(x => x.Cart).ThenInclude(i => i.Items).Where(o => o.Paid == true && o.Baked == false).OrderBy(d => d.OrderDate).ToList();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetToBaked(Order order)
        {
            var orderBaked = _orderService.GetOrder(order.CartId);
            orderBaked.Baked = true;
            _context.Update(orderBaked);
            _context.SaveChanges();

            return RedirectToAction("Bake");
        }

        public IActionResult Ship()
        {
            var orders = _context.Orders.Include(x => x.Cart).ThenInclude(i => i.Items)
                .Where(o => o.Paid == true && o.Baked == true && o.Shipped == false).OrderBy(d => d.OrderDate).ToList();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetToShipped(Order order)
        {
            var orderToShip = _orderService.GetOrder(order.CartId);
            orderToShip.Shipped = true;
            _context.Update(orderToShip);
            _context.SaveChanges();

            return RedirectToAction("Ship");
        }
    }
}