using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IInvestment
    {
        Investment CreateInvestment(Investment investment);
        Investment UpdateInvestment(Investment investment);
        void DeleteInvestment(int id);
        Investment GetInvestmentById(int id);
        IEnumerable<Investment> GetAllInvestments();
        IEnumerable<Investment> GetInvestmentsByClientId(int clientId);
        IEnumerable<Investment> GetInvestmentsByAdvisorId(int advisorId);

    }
}
