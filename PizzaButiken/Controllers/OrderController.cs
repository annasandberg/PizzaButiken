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

        public IActionResult OrderDetails()
        {
            return View();
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
        public IActionResult Checkout([Bind("CustomerName, Email, PhoneNumber, Street, PostalCode, City, CardNumber, CVV")]ShippingAddress user)
        {
            var cartId = _cartService.GetTempCartId(HttpContext.Session);
            if (ModelState.IsValid)
            {
                _orderService.CreateOrder(cartId, user);

                var order = _orderService.GetOrder(cartId);

                string message = String.Format("Orderdatum: {0} Summa: {1} SEK Fraktavgift: {2} SEK",
                    order.OrderDate.ToString(), order.TotalPrice.ToString(), order.ShippingFee.ToString());
                _emailSender.SendEmailAsync(user.Email, "Tack för din beställning", message);

                HttpContext.Session.Clear();

                return View("ThankYouPage");
            }

            return View(user);
        }
    }
}