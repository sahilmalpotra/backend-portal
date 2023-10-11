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
        public int InvestmentID { get; set; }
        public int ClientId { get; set; }
        public int AdvisorId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public string InvestmentType { get; set; }
        public string TimePeriod { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
