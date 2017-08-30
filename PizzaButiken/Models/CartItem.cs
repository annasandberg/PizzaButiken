using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public string Name { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public Dish Dish { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        [Display(Name="Ingredients")]
        public List<CartItemIngredient> CartItmeIngredients { get; set; }
    }
}
