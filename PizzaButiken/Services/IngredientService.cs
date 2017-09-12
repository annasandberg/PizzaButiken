using Microsoft.EntityFrameworkCore;
using PizzaButiken.Data;
using PizzaButiken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Ingredient> GetAllIngredients()
        {
            return _context.Ingredients.OrderBy(i => i.Name).ToList();
        }

        public List<Ingredient> GetAllIngredientsForEditingDish(int dishId)
        {
            var dish = GetDish(dishId).Result;
            var dishIngredients = dish.DishIngredients;
            var ingredients = _context.Ingredients.OrderBy(i => i.Name).ToList();

            foreach (var ingredient in ingredients)
            {
                if (dishIngredients.Any(x => x.IngredientId == ingredient.IngredientId))
                {
                    ingredient.Enabled = true;
                }
            }

            return ingredients;
        }

        public List<Ingredient> GetAllIngredientsForCustomizingCartItem(int cartItemId)
        {
            var cartItem = GetCartItem(cartItemId).Result;
            var ingredients = _context.Ingredients.OrderBy(i => i.Name).ToList();

            foreach (var ingredient in ingredients)
            {
                if (cartItem.CartItmeIngredients.Any(x => x.IngredientId == ingredient.IngredientId))
                {
                    ingredient.Enabled = true;
                }
            }

            return ingredients;
        }

        public List<Ingredient> GetAllIngredientsForCustomizingDish(int dishId)
        {
            var dish = GetDish(dishId).Result;
            var ingredients = _context.Ingredients.OrderBy(i => i.Name).ToList();

            foreach (var ingredient in ingredients)
            {
                if (dish.DishIngredients.Any(x => x.IngredientId == ingredient.IngredientId))
                {
                    ingredient.Enabled = true;
                }
            }

            return ingredients;
        }

        private async Task<Dish> GetDish(int dishId)
        {
            var dish = await _context.Dishes
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .SingleOrDefaultAsync(m => m.DishId == dishId);
            return dish;
        }

        private async Task<CartItem> GetCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.CartItmeIngredients)
                .ThenInclude(i => i.Ingredient)
                .SingleOrDefaultAsync(x => x.CartItemId == cartItemId);

            return cartItem;
        }
    }
}
