using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAdvisor
    {
        Task<Advisor> GetAdvisorByIdAsync(string advisorId);
        Task<IEnumerable<Advisor>> GetAllAdvisorsAsync();
        Task<string> CreateAdvisorAsync(Advisor advisor);
        Task UpdateAdvisorAsync(Advisor advisor);
        Task DeleteAdvisorAsync(string advisorId);

        Task<Advisor> GetAdvisorWithLowestClientsAsync();
    }
}
