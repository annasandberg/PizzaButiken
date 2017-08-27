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
            return _context.DishCategories.ToList();
        }

        public DishCategory GetDishCategory(int id)
        {
            return _context.DishCategories.Find(id);
        }

    }
}
