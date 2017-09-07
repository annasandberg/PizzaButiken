using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Card number must be numeric")]
        public string CardNumber { get; set; }

        [Required]
        public string ValidTo { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "CVV code must be numeric")]
        public string CVV { get; set; }
    }
}
