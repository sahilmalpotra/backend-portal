using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    internal interface IUserRepository
    {
        Task<Client> GetClientByIdAsync(int clientId);
        Task<Client> GetClientByEmailAsync(string email);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<int> AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int clientId);
    }
}
