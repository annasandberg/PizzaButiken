using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public bool Paid { get; set; }
        public bool Baked { get; set; }
        public bool Shipped { get; set; }
    }
}
