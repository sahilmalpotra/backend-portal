using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class InvestmentRepo : IInvestment
    {
        private readonly List<Investment> _investments = new List<Investment>();
        // private int _nextInvestmentId = 1;

        public Investment CreateInvestment(Investment investment)
        {
            if (investment == null)
            {
                throw new ArgumentNullException(nameof(investment));
            }

            //  investment.InvestmentID = _nextInvestmentId++;
            _investments.Add(investment);

            return investment;
        }

        public Investment UpdateInvestment(Investment investment)
        {
            if (investment == null)
            {
                throw new ArgumentNullException(nameof(investment));
            }

            var existingInvestment = _investments.FirstOrDefault(i => i.InvestmentID == investment.InvestmentID);
            if (existingInvestment != null)
            {
                existingInvestment.InvestmentAmount = investment.InvestmentAmount;
                existingInvestment.InvestmentType = investment.InvestmentType;
                existingInvestment.TimePeriod = investment.TimePeriod;
                existingInvestment.CreatedDate = investment.CreatedDate;
            }

            return investment;

        }

        public void DeleteInvestment(string id)
        {
            var existingInvestment = _investments.FirstOrDefault(i => i.InvestmentID == id);
            if (existingInvestment != null)
            {
                _investments.Remove(existingInvestment);
            }
        }

        public Investment GetInvestmentById(string id)
        {
            var investment = _investments.FirstOrDefault(i => i.InvestmentID == id);
            return investment;
        }

        public IEnumerable<Investment> GetInvestmentsByClientId(string clientId)
        {
            return _investments.Where(i => i.ClientId == clientId);
        }

        public IEnumerable<Investment> GetInvestmentsByAdvisorId(string advisorId)
        {
            return _investments.Where(i => i.AdvisorId == advisorId  && i.RemainingAmount > 0);
        }

        public IEnumerable<Investment> GetAllInvestments()
        {
            return _investments;
        }

    }
}