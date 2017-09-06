using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models
{
    public class ShippingAddress
    {
        public int ShippingAddressId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string Street { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string City { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
