using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvestmentBundleDTO
    {
        public string InvestmentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Strategy> Strategies { get; set; }

    }
}