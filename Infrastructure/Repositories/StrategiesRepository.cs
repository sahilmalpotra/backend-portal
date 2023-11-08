using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StrategiesRepository : IStrategies
    {
        private readonly AppDbContext _context;

        public StrategiesRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Strategies> GetStrategiesByName(string strategyName)
        {
            return _context.Strategies.Where(s => s.StartegyName == strategyName).ToList();
        }

        public IEnumerable<Strategies> GetStrategiesByType(string strategyType)
        {
            return _context.Strategies.Where(s => s.StrategyType == strategyType).ToList();
        }

        public IEnumerable<Strategies> GetAllStrategies()
        {
            return _context.Strategies.ToList();
        }

        public void AddStrategy(Strategies strategy)
        {
            _context.Strategies.Add(strategy);
            _context.SaveChanges();
        }
    }
}
