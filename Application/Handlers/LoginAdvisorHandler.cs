using Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace Application.Handlers
{
   public class LoginAdvisorHandler
    {
        private readonly AppDbContext dbContext; // Replace YourDbContext with your actual database context

        public LoginAdvisorHandler(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> Handle(LoginAdvisorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an advisor with the provided email exists in the database
                var advisor = await dbContext.Advisor
                    .FirstOrDefaultAsync(a => a.Email == request.Email);

                if (advisor != null)
                {
                    // Verify the provided password against the stored password
                    // In a real application, you should use a secure password hashing and verification library
                    if (advisor.Password == request.Password)
                    {
                        // Authentication successful
                        return true;
                    }
                }

                // Authentication failed
                return false;
            }
            catch (Exception ex)
            {
                // Handle and log any exceptions that may occur during the authentication process
                return false; // Authentication failed
            }
        }
    }
}

