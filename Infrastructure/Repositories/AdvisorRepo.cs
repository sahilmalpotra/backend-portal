using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace Infrastructure.Repositories
{
    public class AdvisorRepo : IAdvisor
    {
        private readonly AppDbContext _context;


        public AdvisorRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Advisor> GetAdvisorByIdAsync(string advisorId)
        {
            return await _context.Advisor.FindAsync(advisorId);
        }


        public async Task<IEnumerable<Advisor>> GetAllAdvisorsAsync()
        {
            return await _context.Advisor.ToListAsync();
        }


        public async Task<string> CreateAdvisorAsync(Advisor advisor)
        {
            _context.Advisor.Add(advisor);
            await _context.SaveChangesAsync();
            return advisor.AdvisorId;
        }


        public async Task UpdateAdvisorAsync(Advisor advisor)
        {
            _context.Entry(advisor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAdvisorAsync(string advisorId)
        {
            var advisor = await _context.Advisor.FindAsync(advisorId);
            if (advisor != null)
            {
                _context.Advisor.Remove(advisor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Advisor> GetAdvisorWithLowestClientsAsync()
        {
            return await _context.Advisor
           .OrderBy(a => a.NumberOfClients)
           .FirstOrDefaultAsync();
        }

    }
}