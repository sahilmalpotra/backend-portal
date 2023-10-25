using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public readonly IConfiguration _configuration;
        public DbSet<Client> Client { get; set; }
        public DbSet<Advisor> Advisor { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Strategy> Strategy { get; set; }
        public DbSet<User> Users { get; set; }

        // public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
         : base(options)
        {
            _configuration = configuration;
        }

        public void UpdateClientAdvisorId(string clientId, string advisorId)
        {
            var client = Client.Find(clientId);
            if (client != null)
            {
                client.AdvisorId = advisorId;
                SaveChanges();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                    });
            }
        }
    }
}

