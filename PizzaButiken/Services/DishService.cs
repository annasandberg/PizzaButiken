using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Data;
using PizzaButiken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Services
{
    public class DishService
    {
        private readonly ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Dish> GetAllDishes()
        {
            return _context.Dishes.OrderBy(d => d.Name).ToList();
        }

        public Dish GetDish(int? id)
        {
            return _context.Dishes
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .SingleOrDefault(m => m.DishId == id);
        }

        public void Create(Dish dish, IFormCollection form)
        {
            var filename = form.Files[0].FileName;
            dish.ImageUrl = filename;

            dish.DishIngredients = GetCheckedIngredients(dish, form);

            _context.Add(dish);
            _context.SaveChanges();
        }

        public void Edit(Dish dish, IFormCollection form)
        {
            var filename = form.Files[0].FileName;
            if (!string.IsNullOrEmpty(filename))
            {
                dish.ImageUrl = filename;
            }

            var dishIngredients = _context.DishIngredients.Where(x => x.DishId == dish.DishId);
            _context.DishIngredients.RemoveRange(dishIngredients);
            _context.Update(dish);
            _context.SaveChanges();

            dish.DishIngredients = GetCheckedIngredients(dish, form);
            _context.Update(dish);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var dish = GetDish(id);
            _context.Dishes.Remove(dish);
            _context.SaveChanges();
        }

        private List<DishIngredient> GetCheckedIngredients(Dish dish, IFormCollection form)
        {
            var key = form.Keys.FirstOrDefault(k => k.Contains("ingredient-"));
            var dashPos = key.IndexOf("-");
            var checkedIngredients = form.Keys.Where(k => k.Contains("ingredient-"));

            var dishIngredients = new List<DishIngredient>();

            foreach (var ingredient in checkedIngredients)
            {
                var id = int.Parse(ingredient.Substring(dashPos + 1));
                dishIngredients.Add(new DishIngredient { IngredientId = id, Enabled = true, DishId = dish.DishId });
            }

            return dishIngredients;
        }
    }
            
}
