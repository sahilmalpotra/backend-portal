using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Application.Handlers;



namespace Service.Services
{
    public class InvestmentService :IInvestmentService
    {
        private readonly IInvestment _investmentRepository;

        public InvestmentService(IInvestment investmentRepository)
        {
            _investmentRepository = investmentRepository;
        }

        public Investment CreateInvestment(Investment investment)
        {
            if (string.IsNullOrEmpty(investment.InvestmentType))
            {
                throw new ArgumentException("InvestmentType is required.");
            }

            if (investment.InvestmentType == "Equity" && !IsValidEquityTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for Equity.");
            }
            else if ((investment.InvestmentType == "SIP" || investment.InvestmentType == "Mutual Funds") &&
                     !IsValidSipOrMutualFundsTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for SIP or Mutual Funds.");
            }
            else if (investment.InvestmentType == "F&O" && !string.IsNullOrEmpty(investment.TimePeriod))
            {
                throw new ArgumentException("TimePeriod should not be specified for F&O.");
            }
            else if (investment.InvestmentType == "On Advisor" && !IsValidOnAdvisorTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for On Advisor.");
            }

            investment.CreatedDate = DateTime.UtcNow;

         
            return _investmentRepository.CreateInvestment(investment);
        }

        public Investment UpdateInvestment(Investment investment)
        {
            if (string.IsNullOrEmpty(investment.InvestmentType))
            {
                throw new ArgumentException("InvestmentType is required.");
            }

            if (investment.InvestmentType == "Equity" && !IsValidEquityTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for Equity.");
            }
            else if ((investment.InvestmentType == "SIP" || investment.InvestmentType == "Mutual Funds") &&
                     !IsValidSipOrMutualFundsTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for SIP or Mutual Funds.");
            }
            else if (investment.InvestmentType == "F&O" && !string.IsNullOrEmpty(investment.TimePeriod))
            {
                throw new ArgumentException("TimePeriod should not be specified for F&O.");
            }
            else if (investment.InvestmentType == "On Advisor" && !IsValidOnAdvisorTimePeriod(investment.TimePeriod))
            {
                throw new ArgumentException("Invalid TimePeriod for On Advisor.");
            }

           
            _investmentRepository.UpdateInvestment(investment);

         
            return investment;
        }

        private bool IsValidEquityTimePeriod(string timePeriod)
        {
            return timePeriod == "Long Term" || timePeriod == "Short Term" || timePeriod == "Swing";
        }

        private bool IsValidSipOrMutualFundsTimePeriod(string timePeriod)
        {
            return timePeriod == "Long Term" || timePeriod == "Short Term";
        }

        private bool IsValidOnAdvisorTimePeriod(string timePeriod)
        {
            return timePeriod == "Long Term" || timePeriod == "Short Term" || timePeriod == "Swing";
        }
        public IEnumerable<Investment> GetAllInvestments()
        {
            return _investmentRepository.GetAllInvestments();
        }

        public Investment GetInvestmentById(string id)
        {

            var investment = _investmentRepository.GetInvestmentById(id);

            if (investment == null)
            {
                throw new NotFoundException("Investment not found");
            }

            return investment;
        }
        public void DeleteInvestment(string id)
        {
            
            var existingInvestment = _investmentRepository.GetInvestmentById(id);

            if (existingInvestment == null)
            {
                throw new NotFoundException("Investment not found"); 
            }

           
            _investmentRepository.DeleteInvestment(id);
        }

        

    }
}