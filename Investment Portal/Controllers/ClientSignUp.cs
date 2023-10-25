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

namespace InvestmentPortal.Controllers

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

            client.AdvisorId = "";

            client.AccountNumber = client.AccountNumber;

            client.BankName = client.BankName;

            client.IfscCode = client.IfscCode;

            client.PanNumber = client.PanNumber;

            client.IsProfileComplete = true;


            string customId = GenerateCustomClientId();

            client.ClientId = customId;

            _context.Client.Add(client);

            await _context.SaveChangesAsync();

            return Ok(new

            {

                message = "User registered successfully!",

                client = client,

                code = 200

            });

        }

        private string GenerateCustomClientId()

        {

            string customCltId;

            bool isUnique = false;

            int uniqueNumber = 1;

            do

            {

                customCltId = "CLT" + uniqueNumber.ToString("D4");

                bool isIdUnique = !_context.Client.Any(s => s.ClientId == customCltId);

                if (isIdUnique)

                {

                    isUnique = true;

                }

                else

                {

                    uniqueNumber++;

                }

            } while (!isUnique);

            return customCltId;

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
            model.FirstName = client.FirstName;
            return Ok(new
            {
                message = "Login successful!",
                client = model,
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
       

        [HttpPut("update/{id}")]

        public IActionResult UpdateClient(string id, [FromBody] ClientUpdateModel updateModel)

        {

            if (updateModel == null)

            {

                return BadRequest(new
                {
                    message = "Invalid update data.",
                    code = 400
                });

            }



            var client = _context.Client.FirstOrDefault(c => c.ClientId == id);



            if (client == null)

            {

                return NotFound(new

                {

                    message = "Client not found.",

                    code = 404

                });

            }



            if (!string.IsNullOrEmpty(updateModel.FirstName))

            {

                client.FirstName = updateModel.FirstName;

            }



            if (!string.IsNullOrEmpty(updateModel.LastName))

            {

                client.LastName = updateModel.LastName;

            }



            if (!string.IsNullOrEmpty(updateModel.Email))

            {

                client.Email = updateModel.Email;

            }



            if (!string.IsNullOrEmpty(updateModel.Password))

            {

                client.Password = updateModel.Password;
                client.ConfirmPassword = client.Password;
                client.Password = HashPassword(client.Password);

            }



            if (!string.IsNullOrEmpty(updateModel.PhoneNumber))

            {

                client.PhoneNumber = updateModel.PhoneNumber;

            }



            if (!string.IsNullOrEmpty(updateModel.Address))

            {

                client.Address = updateModel.Address;

            }



            if (!string.IsNullOrEmpty(updateModel.City))

            {

                client.City = updateModel.City;

            }



            if (!string.IsNullOrEmpty(updateModel.State))

            {

                client.State = updateModel.State;

            }



            if (!string.IsNullOrEmpty(updateModel.PinCode))

            {

                client.PinCode = updateModel.PinCode;

            }



            if (!string.IsNullOrEmpty(updateModel.AccountNumber))

            {

                client.AccountNumber = updateModel.AccountNumber;

            }



            if (!string.IsNullOrEmpty(updateModel.BankName))

            {

                client.BankName = updateModel.BankName;

            }



            if (!string.IsNullOrEmpty(updateModel.IfscCode))

            {

                client.IfscCode = updateModel.IfscCode;

            }



            if (!string.IsNullOrEmpty(updateModel.PanNumber))

            {

                client.PanNumber = updateModel.PanNumber;

            }



            _context.SaveChanges();



            return Ok(new

            {

                message = "Client information updated successfully.",

                code = 200

            });

        }


        [HttpGet("{id}")]
        public IActionResult GetClient(string id)
        {
            var client = _context.Client.FirstOrDefault(c => c.ClientId == id);



            if (client == null)
            {
                return NotFound(new
                {
                    message = "Client not found.",
                    code = 404
                });
            }



            return Ok(new
            {
                message = "Client data retrieved successfully.",
                client = client,
                code = 200
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(string id)
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
