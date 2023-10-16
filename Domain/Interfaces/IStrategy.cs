using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStrategy
    {
        Task<IEnumerable<Strategy>> GetStrategiesAsync();
        Task<IEnumerable<Strategy>> GetStrategiesByInvestmentIdAsync(int investmentId);
        Task<Strategy> GetStrategyByStrategyIdAsync(int strategyId);
        Task<IEnumerable<Strategy>> GetStrategiesByClientIdAsync(int clientId);
        Task<IEnumerable<Strategy>> GetStrategiesByAdvisorIdAsync(int advisorId);
        Task<int> AddStrategyAsync(Strategy strategy);
        Task UpdateStrategyAsync(Strategy strategy);
        Task DeleteStrategyAsync(int strategyId);
        Task UpdatebyClientAsync(int strategyId, string status, string remarks);

    }
}
