using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Domain.Entities
{
    public class AdvisorUpdateModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }



        [Display(Name = "Last Name")]
        public string LastName { get; set; }



        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }



        [Display(Name = "Password")]
        public string Password { get; set; }



        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }



        [Display(Name = "Address")]
        public string Address { get; set; }



        [Display(Name = "City")]
        public string City { get; set; }



        [Display(Name = "State")]
        public string State { get; set; }



        [Display(Name = "Pin Code")]
        public string PinCode { get; set; }
    }
}
