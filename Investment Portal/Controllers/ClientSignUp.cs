using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Net.Mail;
using System.Net;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace Investment_Portal.Controllers
{
    [EnableCors("AllowAll")]

    [Route("api/[controller]")]
    [ApiController]
    public class ClientSignUp : ControllerBase
    {
        private readonly AppDbContext _context;

        public object JsonRequestBehavior { get; private set; }

        public ClientSignUp(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest(new
                {
                    message = "Invalid user data.",
                    client = client,
                    code = 400
                });
            }

            try
            {
                if (await _context.Client.AnyAsync(u => u.Email == client.Email))
                {
                    return StatusCode(409, new
                    {
                        message = "Email address is already in use.",
                        client = client,
                        code = 409
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing the request.",
                    details = ex.Message,
                    code = 500
                });
            }


            client.Password = HashPassword(client.Password);

            client.City = client.City;
            client.Address = client.Address;
            client.State = client.State;
            client.PinCode = client.PinCode;
            client.PhoneNumber = client.PhoneNumber;
            client.AdvisorId = 0;
            client.AccountNumber = ""; // Set to empty initially
            client.BankName = "";      // Set to empty initially
            client.IfscCode = "";      // Set to empty initially
            client.PanNumber = "";     // Set to empty initially
                                       // client.InvestmentAmount = 0;// Set to 0 initially
            client.IsProfileComplete = false; // Profile is not complete initially

            _context.Client.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully!",
                client = client,
                code = 200
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] ClientLogin model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    message = "Invalid login data.",
                    code = 400
                });
            }

            var client = _context.Client.FirstOrDefault(u => u.Email == model.Email);

            if (client == null || !VerifyPassword(model.Password, client.Password))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password.",
                    client = client,
                    code = 401
                });
            }

            if (!client.IsProfileComplete)
            {
                return Ok(new
                {
                    message = "Profile is not complete. Please provide the missing information.",
                    client = client,
                    code = 202
                });
            }

            return Ok(new
            {
                message = "Login successful!",
                client = client,
                code = 200
            });
        }

        [HttpPost("complete-profile")]
        public IActionResult CompleteProfile([FromBody] ClientProfileUpdateModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    message = "Invalid profile update data.",
                    code = 400
                });
            }

            var client = _context.Client.FirstOrDefault(u => u.ClientId == model.ClientId);

            if (client == null)
            {
                return NotFound(new
                {
                    message = "Client not found.",
                    code = 404
                });
            }

            if (client.IsProfileComplete)
            {
                return BadRequest(new
                {
                    message = "Client profile is already complete.",
                    code = 400
                });
            }

            if (client.AccountNumber == model.AccountNumber ||
                 client.BankName == model.BankName ||
                 client.IfscCode == model.IfscCode ||
                 client.PanNumber == model.PanNumber)
            {
                return BadRequest(new
                {
                    message = "No changes detected in profile update.",
                    code = 400
                });
            }
            client.AccountNumber = model.AccountNumber;
            client.BankName = model.BankName;
            client.IfscCode = model.IfscCode;
            client.PanNumber = model.PanNumber;
            // client.InvestmentAmount = model.InvestmentAmount;
            client.IsProfileComplete = true;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Client profile has been updated and marked as complete.",
                client = client,
                code = 200
            });
        }

        private string HashPassword(string password)
        {

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;

        }


        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var client = await _context.Client.FindAsync(id);

                if (client == null)
                {
                    return NotFound(new
                    {
                        message = "Client not found.",
                        code = 404
                    });
                }

                _context.Client.Remove(client);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Client deleted successfully.",
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing the request.",
                    details = ex.Message,
                    code = 500
                });
            }
        }

    }
}
