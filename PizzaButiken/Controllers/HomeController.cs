using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaButiken.Models;
using PizzaButiken.Data;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Services;
using Microsoft.AspNetCore.Http;

namespace PizzaButiken.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public HomeController(ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(int dishCategoryId)
        {
            return View(await _context.Dishes.Where(x => x.DishCategoryId == dishCategoryId).ToListAsync());
        }

        [HttpPost]
        public IActionResult ShoppingCartAction(IFormCollection form)
        {
            var key = form.Keys.FirstOrDefault(k => k.Contains("-"));
            var dashPos = key.IndexOf("-");
            var action = key.Substring(0, dashPos);
            var id = int.Parse(key.Substring(dashPos + 1));
            switch (action)
            {
                case "add": _cartService.AddItemForCurrentSession(HttpContext.Session, id); break;
                case "remove": _cartService.DeleteItemForCurrentSession(HttpContext.Session, id); break;
            }

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
