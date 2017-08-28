using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        [Display (Name = "Ingredients")]
        public List<DishIngredient> DishIngredients { get; set; }
        [Display(Name = "Dish Category")]
        public int DishCategoryId { get; set; }
        [Display(Name = "Dish Category")]
        public DishCategory DishCategory { get; set; }
    }
}
