using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Strategies
    {
        [Required]
        [Key]
        public string StartegyName { get; set; }

        [Required]
        public string StrategyType { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Return percentage should be between 0 and 100.")]
        public decimal ReturnPercentageAfter6months { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Return percentage should be between 0 and 100.")]
        public decimal ReturnPercentageAfter1year { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Return percentage should be between 0 and 100.")]
        public decimal ReturnPercentageAfter3year { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Return percentage should be between 0 and 100.")]
        public decimal ReturnPercentageAfter5year { get; set; }
    }
}
