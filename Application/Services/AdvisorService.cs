using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AdvisorService
    {
        private readonly IAdvisor _advisorRepository;

        public AdvisorService(IAdvisor advisorRepository)
        {
            _advisorRepository = advisorRepository;
        }

        public async Task<Advisor> GetAdvisorWithLowestClientsAsync()
        {
            return await _advisorRepository.GetAdvisorWithLowestClientsAsync();
        }
    }
}
