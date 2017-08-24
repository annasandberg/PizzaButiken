using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class DishCategory
    {
        public int DishCategoryId { get; set; }
        public string Name { get; set; }
        public List<Dish> Dishes { get; set; }
    }
}
