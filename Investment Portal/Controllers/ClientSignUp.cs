using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Net;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cors;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

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

                string otp = GenerateOTP();

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

                SaveOTPInDatabase(client.Email, otp);

                SendOTPEmail(client.Email, otp);


                return Ok(new
                {
                    message = "OTP sent to your email for verification.",
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

        private void SaveOTPInDatabase(string email, string otp)
        {

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new User { Email = email, OTP = otp };
                _context.Users.Add(user);
            }
            else
            {
                user.OTP = otp;
            }
            _context.SaveChanges();
            Console.WriteLine("OTP saved to database.");
        }

        [HttpGet("Generate-OTP")]
        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        [HttpPost("send-otp-email")]
        public void SendOTPEmail(string email, string otp)
        {
            using var smtp = new SmtpClient();

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse("priyaagg29@gmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = "Email Verification OTP";
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<h1>Your OTP is: {otp}</h1>"
            };

            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("priyaagg29@gmail.com", "damz zbnq cvrf iydn");

            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
            Console.WriteLine("OTP email sent.");
        }


        [HttpPost("verify-otp")]
        public IActionResult VerifyOTP([FromBody] EmailVerificationModel model)
        {

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP))
            {
                return BadRequest(new
                {
                    message = "Invalid verification data.",
                    code = 400
                });
            }

            string storedOTP = GetStoredOTPFromDatabase(model.Email);
            if (model.OTP == storedOTP)
            {
                MarkEmailAsVerifiedInDatabase(model.Email);
                Console.WriteLine("OTP matched and modified.");
                return Ok(new
                {
                    message = "Email verified successfully.",
                    code = 200
                });
            }
            Console.WriteLine("Invalid OTP.");
            return BadRequest(new
            {
                message = "Invalid OTP.",
                code = 400
            });
        }


        private string GetStoredOTPFromDatabase(string email)
        {
            try
            {

                var user = _context.Users.SingleOrDefault(u => u.Email == email);

                if (user != null && !string.IsNullOrEmpty(user.OTP))
                {
                    return user.OTP;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while retrieving OTP: " + ex.ToString());

                return "An error occurred while retrieving OTP. Please try again later.";
            }

            return null;
        }

        [HttpPut("verify-otp")]
        public void MarkEmailAsVerifiedInDatabase(string email)
        {
            try
            {

                var user = _context.Users.SingleOrDefault(u => u.Email == email);

                if (user != null)
                {
                    user.IsVerified = true;
                    user.OTP = null;
                    _context.SaveChanges();
                    Console.WriteLine("Email marked as verified.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while marking email as verified: " + ex.Message);
            }
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
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = _context.Client.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found.",
                    code = 404
                });
            }

            string otp = GenerateOTP();
            SaveOTPInDatabase(user.Email, otp);
            SendOTPEmail(user.Email, otp);

            return Ok(new
            {
                message = "Password reset OTP sent to your email.",
                code = 200
            });
        }


        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return BadRequest(new
                {
                    message = "Invalid reset password data.",
                    code = 400
                });
            }

            string storedOTP = GetStoredOTPFromDatabase(model.Email);
            if (model.OTP == storedOTP)
            {
                if (model.NewPassword == model.ConfirmPassword)
                {
                    var client = _context.Client.FirstOrDefault(a => a.Email == model.Email);
                    if (client != null)
                    {
                        client.Password = HashPassword(model.NewPassword);
                        // advisor.ConfirmPassword = advisor.Password;
                        client.ConfirmPassword = model.NewPassword;
                        _context.SaveChanges();
                        return Ok(new
                        {
                            message = "Password reset successfully.",
                            code = 200
                        });
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "New password and confirmation password do not match.",
                        code = 400
                    });
                }
            }

            return BadRequest(new
            {
                message = "Invalid OTP.",
                code = 400
            });
        }

    }
}