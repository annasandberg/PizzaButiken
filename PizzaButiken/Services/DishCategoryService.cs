using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaButiken.Data;
using PizzaButiken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Services
{
    public class DishCategoryService
    {
        private readonly ApplicationDbContext _context;

        public DishCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DishCategory> GetAllDishCategories()
        {
            return _context.DishCategories.OrderBy(dc => dc.Name).ToList();
        }

        public DishCategory GetDishCategory(int? id)
        {
            return _context.DishCategories.Find(id);
        }

        public void CreateDishCategory(DishCategory dishCategory)
        {
            _context.Add(dishCategory);
            _context.SaveChanges();
        }

        public void Edit(DishCategory dishCategory)
        {
            _context.Update(dishCategory);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var dishCategory = GetDishCategory(id);
            _context.DishCategories.Remove(dishCategory);
            _context.SaveChanges();
        }

    }
}
