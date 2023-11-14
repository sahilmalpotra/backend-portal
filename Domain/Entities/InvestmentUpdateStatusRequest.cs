using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvestmentUpdateStatusRequest
    {
        public List<InvestmentUpdate> InvestmentUpdates { get; set; }

    }
}
