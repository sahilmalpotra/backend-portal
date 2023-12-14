using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StrategyRepo : IStrategy
    {
        private readonly AppDbContext _context;
        private List<PastData> companies;

        public StrategyRepo(AppDbContext context)
        {
            _context = context;
            this.companies = companies;
        }


        public async Task<string> AddStrategyAsync(Strategy strategy)
        {
            strategy.Status = "Pending";
            strategy.Completed = false;
            _context.Strategy.Add(strategy);
            await _context.SaveChangesAsync();
            return strategy.StrategyId;
        }


        public async Task<IEnumerable<Strategy>> GetStrategiesAsync()
        {
            return await _context.Strategy.ToListAsync();
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByInvestmentIdAsync(string investmentId)
        {
            var strategies = await _context.Strategy
            .Where(s => s.InvestmentId == investmentId)
            .ToListAsync();

            return strategies.OrderBy(s => strategies.IndexOf(s));
        }
        public async Task<Strategy> GetStrategyByStrategyIdAsync(string strategyId)
        {
            return await _context.Strategy
                .FirstOrDefaultAsync(s => s.StrategyId == strategyId);
        }

        public async Task<IEnumerable<StockData>> GetStockData(string companyName)
        {
            var company = companies.Find(c => c.name.Equals(companyName, StringComparison.OrdinalIgnoreCase));

            if (company == null)
            {
                // You may want to return an empty list or handle this case differently
                return new List<StockData>();
            }

            return company.StockHistory;
        }
        public async Task<bool> AddCompany(PastData pastData)
        {


            var newCompany = new PastData();
            {
                newCompany.name = pastData.name;

            };


            companies.Add(newCompany);

            return true;
        }

        public PastData GetCompany(string name)
        {
            return companies.Find(c => c.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<bool> PostStockData(string companyName, StockData stockData)
        {
            var company = companies.Find(c => c.name.Equals(companyName, StringComparison.OrdinalIgnoreCase));

            if (company == null)
            {

                return false;
            }


            company.StockHistory.Add(stockData);

            return true;
        }

        public async Task<IEnumerable<Strategy>> GetAllStrategiesByClientIdAsync(string clientId)
        {
            var strategies = await _context.Strategy
               .Where(s => s.ClientId == clientId)
                .ToListAsync();
            return strategies.OrderBy(s => strategies.IndexOf(s));
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByClientIdAsync(string clientId)
        {
            var investmentIds = await _context.Investments
                .Where(i => i.ClientId == clientId)
                .Select(i => i.InvestmentID)
                .ToListAsync();

            var strategies = await _context.Strategy
           .Where(s => investmentIds.Contains(s.InvestmentId))
            .ToListAsync();
            return strategies.OrderBy(s => strategies.IndexOf(s));
        }

        public async Task UpdateInvestmentAmountAsync(string strategyId, decimal newInvestmentAmount)
        {
            var strategy = await _context.Strategy.FindAsync(strategyId);

            if (strategy != null)
            {
                strategy.InvestmentAmount = newInvestmentAmount;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByAdvisorIdAsync(string advisorId)
        {
            var strategies = await _context.Strategy
             .Where(s => s.AdvisorId == advisorId)
             .ToListAsync();

            return strategies.OrderBy(s => strategies.IndexOf(s));
        }
        public async Task<IEnumerable<Strategy>> GetApprovedStrategiesByAdvisorIdAsync(string advisorId)
        {
            var approvedStrategies = await _context.Strategy
                .Where(s => s.AdvisorId == advisorId && s.Status == "Approved")
                .ToListAsync();

            return approvedStrategies.OrderBy(s => approvedStrategies.IndexOf(s)); 
        }

        public async Task UpdateStrategyAsync(Strategy strategy)
        {
            var existingStrategy = await _context.Strategy
                .FirstOrDefaultAsync(s => s.StrategyId == strategy.StrategyId);

            if (existingStrategy != null)
            {
                existingStrategy.InvestmentId = strategy.InvestmentId;
                existingStrategy.InvestmentName = strategy.InvestmentName;
                existingStrategy.ClientId = strategy.ClientId;
                existingStrategy.AdvisorId = strategy.AdvisorId;
                existingStrategy.ReturnPercentage = strategy.ReturnPercentage;
                existingStrategy.InvestmentAmount = strategy.InvestmentAmount;
                existingStrategy.ExpectedAmount = strategy.ExpectedAmount;
                existingStrategy.Status = strategy.Status;
                existingStrategy.TimePeriod = strategy.TimePeriod;
                existingStrategy.Remarks = strategy.Remarks;


                _context.Entry(existingStrategy).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteStrategyAsync(string strategyId)
        {
            var strategy = await _context.Strategy.FindAsync(strategyId);
            if (strategy != null)
            {
                _context.Strategy.Remove(strategy);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatebyClientAsync(string strategyId, string status, string remarks)
        {
            var strategy = await _context.Strategy.FindAsync(strategyId);

            if (strategy != null)
            {
                strategy.Status = status;
                strategy.Remarks = remarks;
                _context.Entry(strategy).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}