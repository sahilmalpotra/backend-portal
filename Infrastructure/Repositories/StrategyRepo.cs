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


        public async Task<int> AddStrategyAsync(Strategy strategy)
        {
            strategy.Status = "Pending";
            _context.Strategy.Add(strategy);
            await _context.SaveChangesAsync();
            return strategy.StrategyId;
        }


        public async Task<IEnumerable<Strategy>> GetStrategiesAsync()
        {
            return await _context.Strategy.ToListAsync();
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByInvestmentIdAsync(int investmentId)
        {
            return await _context.Strategy
                .Where(s => s.InvestmentId == investmentId)
                .ToListAsync();
        }
        public async Task<Strategy> GetStrategyByStrategyIdAsync(int strategyId)
        {
            return await _context.Strategy
                .FirstOrDefaultAsync(s => s.StrategyId == strategyId);
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByClientIdAsync(int clientId)
        {
            return await _context.Strategy
                .Where(s => s.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesByAdvisorIdAsync(int advisorId)
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
                existingStrategy.Amount = strategy.Amount;
                existingStrategy.ClientId = strategy.ClientId;
                existingStrategy.AdvisorId = strategy.AdvisorId;
                existingStrategy.ReturnPercentage = strategy.ReturnPercentage;
                existingStrategy.InvestmentAmount = strategy.InvestmentAmount;
                existingStrategy.ExpectedAmount = strategy.ExpectedAmount;
                existingStrategy.Status = strategy.Status;
                existingStrategy.TimePeriod = strategy.TimePeriod;
                existingStrategy.Remarks = strategy.Remarks;

                // Mark entity as modified and save changes
                _context.Entry(existingStrategy).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteStrategyAsync(int strategyId)
        {
            var strategy = await _context.Strategy.FindAsync(strategyId);
            if (strategy != null)
            {
                _context.Strategy.Remove(strategy);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatebyClientAsync(int strategyId, string status, string remarks)
        {
            var strategy = await _context.Strategy.FindAsync(strategyId);

            if (strategy != null)
            {
                strategy.Status = status;
                strategy.Remarks = remarks;

                // Update the strategy
                _context.Entry(strategy).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
