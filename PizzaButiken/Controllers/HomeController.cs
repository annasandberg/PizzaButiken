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
        private readonly CartService _cartService;
        private readonly DishService _dishService;

        public HomeController(CartService cartService, DishService dishService)
        {
            _cartService = cartService;
            _dishService = dishService;
        }

        public IActionResult Index(int dishCategoryId)
        {
            if (dishCategoryId == 0)
            {
                return View(_dishService.GetAllDishesAndTheirIngredients());
            }
            return View(_dishService.GetAllDishesInCategory(dishCategoryId));
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
                case "increaseQuantity": _cartService.IncreaseCartItemQuantity(id); break;
                case "decreaseQuantity": _cartService.DecreaseCartItemQuantity(id); break;
            }

            return RedirectToAction("Index");
        }

        public IActionResult AddAndCustomizeCartItem(int dishId)
        {
            var dish = _dishService.GetDish(dishId);
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

        public IActionResult CustomizeCartItem(int id, string returnUrl = null)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var cartItem = _cartService.GetCartItem(id); 

            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(cartItem);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CustomizeCartItem([Bind("CartItemId")]CartItem cartItem, IFormCollection form, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                _cartService.CustomizeItemForCurrentsession(HttpContext.Session, cartItem, form);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return RedirectToLocal(returnUrl);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
