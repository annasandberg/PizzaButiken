using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string Street { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string City { get; set; }

        [StringLength(6, ErrorMessage = "The {0} cannot be longer than {1} characters.")]
        public string PostalCode { get; set; }

        public string StatusMessage { get; set; }
    }
}