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

        public IActionResult Index(int cartId)
        {
            var carts = _context.Carts.Include(ci => ci.Items).ThenInclude(x => x.CartItmeIngredients).ThenInclude(i => i.Ingredient).Include(u=> u.ApplicationUser);
            var cart = carts.FirstOrDefault(id => id.CartId == cartId);
            var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
            var appUser = new ApplicationUser
            {
                CustomerName = user?.CustomerName,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber,
                Street = user?.Street,
                PostalCode = user?.PostalCode,
                City = user?.City
            };
            cart.ApplicationUser = appUser;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buy([Bind("CartId")]Cart cart, [Bind("CustomerName, Email, PhoneNumber, Street, PostalCode, City")]ApplicationUser user)
        {
            _orderService.CreateOrder(cart.CartId, user);

            var order = _orderService.GetOrder(cart.CartId);

            string message = String.Format("Orderdatum: {0} Summa: {1} SEK Fraktavgift: {2} SEK", 
                order.OrderDate.ToString(), order.TotalPrice.ToString(), order.ShippingFee.ToString());
            _emailSender.SendEmailAsync(user.Email, "Tack för din beställning", message);

            HttpContext.Session.Clear();

            return View("ThankYouPage");
        }
    }
}