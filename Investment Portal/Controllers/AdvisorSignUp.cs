
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
using Microsoft.AspNetCore.Cors;

namespace InvestmentPortal.Controllers
{
    [EnableCors("AllowAll")]


    [Route("api/[controller]")]
    [ApiController]
    public class AdvisorSignUp : ControllerBase
    {

        private readonly AppDbContext _context;

        public object JsonRequestBehavior { get; private set; }

        public AdvisorSignUp(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] Advisor advisor)
        {
            if (advisor == null)
            {
                return BadRequest(new
                {
                    message = "Invalid user data.",
                    advisor = advisor,
                    code = 400
                });
            }

            try
            {
                if (await _context.Advisor.AnyAsync(u => u.Email == advisor.Email))
                {
                    return StatusCode(409, new
                    {
                        message = "Email address is already in use.",
                        user = advisor,
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


            advisor.Password = HashPassword(advisor.Password);

            advisor.City = advisor.City;
            advisor.Address = advisor.Address;
            advisor.State = advisor.State;
            advisor.PinCode = advisor.PinCode;
            advisor.PhoneNumber = advisor.PhoneNumber;
            advisor.NumberOfClients = 0;

            _context.Advisor.Add(advisor);
            await _context.SaveChangesAsync();

            // return Ok("User registered successfully!", user);
            return Ok(new
            {
                message = "User registered successfully!",
                advisor = advisor,
                code = 200
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AdvisorLogin model)
        {
            if (model == null)
            {
                // return BadRequest("Invalid login data.");
                return BadRequest(new
                {
                    message = "Invalid login data.",
                    code = 400
                });
            }

            var advisor = _context.Advisor.FirstOrDefault(u => u.Email == model.Email);

            if (advisor == null || !VerifyPassword(model.Password, advisor.Password))
            {
                // return Unauthorized("Invalid email or password.");
                return Unauthorized(new
                {
                    message = "Invalid email or password.",
                    advisor = advisor,
                    code = 401
                });
            }


            return Ok(new
            {
                message = "Login successful!",
                advisor = advisor,
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
        public async Task<IActionResult> DeleteAdvisor(int id)
        {
            try
            {
                var advisor = await _context.Advisor.FindAsync(id);

                if (advisor == null)
                {
                    return NotFound(new
                    {
                        message = "Advisor not found.",
                        code = 404
                    });
                }

                _context.Advisor.Remove(advisor);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Advisor deleted successfully.",
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

        [HttpGet("clients-by-advisor/{advisorId}")]
        public async Task<IActionResult> GetClientsByAdvisorId(int advisorId)
        {
            try
            {
                var clients = await _context.Client.Where(c => c.AdvisorId == advisorId).ToListAsync();
                return Ok(clients);
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
