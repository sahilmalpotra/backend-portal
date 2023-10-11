using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IInvestmentService
    {
        Investment CreateInvestment(Investment investment);
       
        Investment GetInvestmentById(int id);

        Investment UpdateInvestment(Investment investment);

        void DeleteInvestment(int id);

        IEnumerable<Investment> GetAllInvestments();
    }
}
