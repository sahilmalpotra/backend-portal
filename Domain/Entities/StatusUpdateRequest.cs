using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StatusUpdateRequest
    {

        public string StrategyId { get; set; }

        [Required]
        [RegularExpression("^(Approved|Rejected|Funded)$", ErrorMessage = "Status must be either 'Approved' or 'Rejected' or 'Funded'.")]
        public string Status { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; }
    }
}
