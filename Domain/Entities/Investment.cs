using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Investment
    {
        [Required]
        [Key]
        public string InvestmentID { get; set; }
        public string ClientId { get; set; }
        public string AdvisorId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string InvestmentType { get; set; }
        public string TimePeriod { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
