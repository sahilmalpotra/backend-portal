using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Commands
{
    public class SignUpAdvisorCommand
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get; set;
        }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get; set;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get; set;
        }

        [Required]
        [Display(Name = "Password")]
        public string Password
        {
            get; set;
        }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword
        {
            get; set;
        }

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string PhoneNumber
        {
            get; set;
        }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address")]
        public string Address
        {
            get; set;
        }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public string City
        {
            get; set;
        }

        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        public string State
        {
            get; set;
        }

        [Required]
        [Display(Name = "Pin Code")]
        public string PinCode
        {
            get; set;
        }

        // Other properties for advisor information like phone number, address, etc.
    }
}


/*

namespace Application.Commands
{
    public class SignUpAdvisorCommand
    {

    }
}
*/