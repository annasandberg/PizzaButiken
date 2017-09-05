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

namespace PizzaButiken.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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
    }
}