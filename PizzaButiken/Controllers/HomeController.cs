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
            if (dishCategoryId == 0)
            {
                return View(await _context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync());
            }
            return View(await _context.Dishes.Where(x => x.DishCategoryId == dishCategoryId).Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync());
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

        public IActionResult AddAndCustomizeCartItem(int dishId)
        {
            var dish = _context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).FirstOrDefault(d => d.DishId == dishId);
            foreach (var ingredient in dish.DishIngredients)
            {
                ingredient.Enabled = true;
            }
            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAndCustomizeCartItem([Bind("DishId")]Dish dish, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                _cartService.CustomizeAndAddItemForCurrentSession(HttpContext.Session, dish.DishId, form);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CustomizeCartItem(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var cartItem = await _context.CartItems
                .Include(c => c.CartItmeIngredients)
                .ThenInclude(i => i.Ingredient)
                .Include(d => d.Dish)
                .ThenInclude(x => x.DishIngredients)
                .SingleOrDefaultAsync(x => x.CartItemId == id);

            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CustomizeCartItem([Bind("CartItemId")]CartItem cartItem, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                _cartService.CustomizeItemForCurrentsession(HttpContext.Session, cartItem, form);
            }
            return RedirectToAction("Index");
        }

        public IActionResult IncreaseCartItemQuantity(int cartItemId)
        {
            _cartService.IncreaseCartItemQuantity(cartItemId);
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseCartItemQuantity(int cartItemId)
        {
            _cartService.DecreaseCartItemQuantity(cartItemId);
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
