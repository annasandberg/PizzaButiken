using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaButiken.Data;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Models;

namespace PizzaButiken.Controllers
{
    public class DishCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DishCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            return View(await _context.DishCategories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishCategoryId,Name")] DishCategory dishCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dishCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dishCategory);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dishCategory = await _context.DishCategories.SingleOrDefaultAsync(m => m.DishCategoryId == id);
            if (dishCategory == null)
            {
                return NotFound();
            }
            return View(dishCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishCategoryId,Name")] DishCategory dishCategory)
        {
            if (id != dishCategory.DishCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dishCategory);
                    await _context.SaveChangesAsync();
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dishCategory = await _context.DishCategories
                .SingleOrDefaultAsync(m => m.DishCategoryId == id);
            if (dishCategory == null)
            {
                return NotFound();
            }

            return View(dishCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dishCategory = await _context.DishCategories.SingleOrDefaultAsync(m => m.DishCategoryId == id);
            _context.DishCategories.Remove(dishCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishCategoryExists(int id)
        {
            return _context.DishCategories.Any(e => e.DishCategoryId == id);
        }
    }
}