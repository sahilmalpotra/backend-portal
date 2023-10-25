using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IClient
    {
        Task<Client> GetClientByIdAsync(string clientId);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<string> CreateClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(string clientId);
    }
}
