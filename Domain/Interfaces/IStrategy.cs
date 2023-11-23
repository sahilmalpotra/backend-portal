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
        Task<IEnumerable<Strategy>> GetStrategiesByInvestmentIdAsync(string investmentId);
        Task<Strategy> GetStrategyByStrategyIdAsync(string strategyId);
        Task<IEnumerable<Strategy>> GetStrategiesByClientIdAsync(string clientId);

        Task<IEnumerable<Strategy>> GetStrategiesByAdvisorIdAsync(string advisorId);
        Task<IEnumerable<Strategy>> GetApprovedStrategiesByAdvisorIdAsync(string advisorId);
        Task<string> AddStrategyAsync(Strategy strategy);
        Task UpdateStrategyAsync(Strategy strategy);
        Task DeleteStrategyAsync(string strategyId);
        Task UpdatebyClientAsync(string strategyId, string status, string remarks);

    }
}
