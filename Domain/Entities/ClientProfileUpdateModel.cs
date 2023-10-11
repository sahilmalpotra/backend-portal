using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ClientProfileUpdateModel
    {
        public int ClientId { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required]
        [Display(Name = "IFSC Code")]
        public string IfscCode { get; set; }

        [Required]
        [Display(Name = "PAN Number")]
        public string PanNumber { get; set; }

        [Required]
        [Display(Name = "Investment Amount")]
        // public long InvestmentAmount { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}
