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

        public StrategyRepo(AppDbContext context)
        {
            _context = context;
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
            return await _context.Strategy
                .Where(s => s.InvestmentId == investmentId)
                .ToListAsync();
        }
        public async Task<Strategy> GetStrategyByStrategyIdAsync(string strategyId)
        {
            return await _context.Strategy
                .FirstOrDefaultAsync(s => s.StrategyId == strategyId);
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByClientIdAsync(string clientId)
        {
            return await _context.Strategy
                .Where(s => s.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByAdvisorIdAsync(string advisorId)
        {
            return await _context.Strategy
                            .Where(s => s.AdvisorId == advisorId)
                            .ToListAsync();
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