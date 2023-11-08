using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStrategies
    {
        IEnumerable<Strategies> GetStrategiesByName(string strategyName);
        IEnumerable<Strategies> GetStrategiesByType(string strategyType);
        void AddStrategy(Strategies strategy);
        IEnumerable<Strategies> GetAllStrategies();

    }
}
