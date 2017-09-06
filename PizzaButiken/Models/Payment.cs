using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class Payment
    {
        [Required]
        [StringLength(16, MinimumLength = 16)]
        [RegularExpression("[0-9]")]
        public string CardNumber { get; set; }

        [Required]
        public string ValidTo { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CVV { get; set; }
    }
}
