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
        void DeleteInvestment(string id);
        Investment GetInvestmentById(string id);
        IEnumerable<Investment> GetAllInvestments();
        IEnumerable<Investment> GetInvestmentsByClientId(string clientId);
        IEnumerable<Investment> GetInvestmentsByAdvisorId(string advisorId);
        void InvestmentStatusUpdateByAdvisor(string InvestmentId, string newStatus);
    }
}
