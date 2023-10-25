using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace Infrastructure.Repositories
{
    public class ClientRepo : IClient
    {
        private readonly AppDbContext _context;


        public ClientRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Client> GetClientByIdAsync(string clientId)
        {
            return await _context.Client.FindAsync(clientId);
        }


        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Client.ToListAsync();
        }


        public async Task<string> CreateClientAsync(Client client)
        {
            _context.Client.Add(client);
            await _context.SaveChangesAsync();
            return client.ClientId;
        }


        public async Task UpdateClientAsync(Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteClientAsync(string clientId)
        {
            var client = await _context.Client.FindAsync(clientId);
            if (client != null)
            {
                _context.Client.Remove(client);
                await _context.SaveChangesAsync();
            }
        }
    }
}