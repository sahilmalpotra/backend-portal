using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAdvisor
    {
        Task<Advisor> GetAdvisorByIdAsync(int advisorId);
        Task<IEnumerable<Advisor>> GetAllAdvisorsAsync();
        Task<int> CreateAdvisorAsync(Advisor advisor);
        Task UpdateAdvisorAsync(Advisor advisor);
        Task DeleteAdvisorAsync(int advisorId);

        Task<Advisor> GetAdvisorWithLowestClientsAsync();
    }
}
