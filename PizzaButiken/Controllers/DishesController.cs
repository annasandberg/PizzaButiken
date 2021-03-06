﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Data;
using PizzaButiken.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using PizzaButiken.Services;

namespace PizzaButiken.Controllers
{
    public class DishesController : Controller
    {
        private readonly DishService _dishService;

        public DishesController(DishService dishService)
        {
            _dishService = dishService;
        }

        // GET: Dishes
        public IActionResult Index()
        {
            return View(_dishService.GetAllDishes());
        }

        // GET: Dishes/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = _dishService.GetDish(id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DishId,Name,Price,DishCategoryId,Image,ImageUrl")] Dish dish, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                _dishService.Create(dish, form);
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = _dishService.GetDish(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("DishId,Name,Price,DishCategoryId,ImageUrl")] Dish dish, IFormCollection form)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dishService.Edit(dish, form);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
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
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = _dishService.GetDish(id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dishService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            if (_dishService.GetDish(id) != null)
            {
                return true;
            }
            return false;
        }
    }
}
