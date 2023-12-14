using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StockData
    {
        public int year { get; set; }
        public decimal price { get; set; }
    }
    public class PastData
    {
        public string name { get; set; }

        public List<StockData> StockHistory { get; set; }
    }
}

