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
            return _context.Ingredients.ToList();
        }

        public List<Ingredient> GetAllIngredientsForEditingDish(int dishId)
        {
            var dish = GetDish(dishId).Result;
            var dishIngredients = dish.DishIngredients;
            var ingredients = _context.Ingredients.ToList();

            foreach (var ingredient in ingredients)
            {
                if (dishIngredients.Any(x => x.IngredientId == ingredient.IngredientId))
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
    }
}
