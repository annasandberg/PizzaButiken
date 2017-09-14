using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaButiken.Data;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Models;
using PizzaButiken.Services;

namespace PizzaButiken.Controllers
{
    public class DishCategoriesController : Controller
    {
        private readonly DishCategoryService _dishCategoryService;

        public DishCategoriesController(DishCategoryService dishCategoryService)
        {
            _dishCategoryService = dishCategoryService;
        }

        public IActionResult Index()
        {
            return View(_dishCategoryService.GetAllDishCategories());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DishCategoryId,Name")] DishCategory dishCategory)
        {
            if (ModelState.IsValid)
            {
                _dishCategoryService.CreateDishCategory(dishCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(dishCategory);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dishCategory = _dishCategoryService.GetDishCategory(id);

            if (dishCategory == null)
            {
                return NotFound();
            }
            return View(dishCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("DishCategoryId,Name")] DishCategory dishCategory)
        {
            if (id != dishCategory.DishCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dishCategoryService.Edit(dishCategory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishCategoryExists(dishCategory.DishCategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dishCategory);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dishCategory = _dishCategoryService.GetDishCategory(id);
            if (dishCategory == null)
            {
                return NotFound();
            }

            return View(dishCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dishCategoryService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool DishCategoryExists(int id)
        {
            if (_dishCategoryService.GetDishCategory(id) != null)
            {
                return true;
            }
            return  false;
        }
    }
}