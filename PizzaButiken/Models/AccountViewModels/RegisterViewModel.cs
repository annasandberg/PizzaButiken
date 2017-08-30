using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string Street { get; set; }

        [StringLength(6, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        [Display(Name ="Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string City { get; set; }
    }
}

