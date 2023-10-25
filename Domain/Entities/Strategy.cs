using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Strategy
    {
        [Key]
        public string StrategyId { get; set; }

        [Required]
        public string InvestmentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal InvestmentAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ExpectedAmount { get; set; }

        [Required]
        [MaxLength(100)]
        public string InvestmentName { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string AdvisorId { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Return percentage should be between 0 and 100.")]
        public decimal ReturnPercentage { get; set; }

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

        public string Status { get; set; }

        [Required]
        public string TimePeriod { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; }

        [Required]
        public bool Completed { get; set; }
    }
}