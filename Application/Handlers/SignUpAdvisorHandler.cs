using Application.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;

namespace Application.Handlers
{
    public class SignUpAdvisorHandler
    {
        private readonly AppDbContext _dbContext;
        public SignUpAdvisorHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext; // Initialize the DbContext via constructor injection
        }
        public async Task<bool> Handle(SignUpAdvisorCommand request, CancellationToken cancellationToken)
        {

            if (request.Password != request.ConfirmPassword)
            {
                return false;
            }

            //create an advisor entity
            var newAdvisor = new Advisor
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                State = request.State,
                PinCode = request.PinCode,
            };

            try
            {
                _dbContext.Advisor.Add(newAdvisor);
                await _dbContext.SaveChangesAsync();

                return true; // Sign-up was successful

            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

