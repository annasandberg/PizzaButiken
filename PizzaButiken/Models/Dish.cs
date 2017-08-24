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
        [Display (Name = "Ingredienser")]
        public List<DishIngredient> DishIngredients { get; set; }
        public int DishCategoryId { get; set; }
        public DishCategory DishCategory { get; set; }
    }
}
