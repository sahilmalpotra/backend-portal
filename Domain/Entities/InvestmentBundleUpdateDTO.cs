using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvestmentBundleUpdateDTO
    {
        public string InvestmentId { get; set; }
        public List<StrategyUpdateDTO> Strategies { get; set; }
    }

    public class StrategyUpdateDTO
    {
        public string StrategyId { get; set; }
        public decimal InvestmentAmount { get; set; }
    }

}
