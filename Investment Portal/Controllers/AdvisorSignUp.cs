using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Net;
using Microsoft.AspNetCore.Cors;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

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
                //return BadRequest("Invalid user data.");
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
                    //return StatusCode(409, "Email address is already in use.");
                    return StatusCode(409, new
                    {
                        message = "Email address is already in use.",
                        user = advisor,
                        code = 409
                    });
                }

                string otp = GenerateOTP();

                advisor.Password = HashPassword(advisor.Password);

                advisor.City = advisor.City;
                advisor.Address = advisor.Address;
                advisor.State = advisor.State;
                advisor.PinCode = advisor.PinCode;
                advisor.PhoneNumber = advisor.PhoneNumber;
                advisor.NumberOfClients = 0;

                string customId = GenerateCustomAdvisorId();
                advisor.AdvisorId = customId;

                _context.Advisor.Add(advisor);
                await _context.SaveChangesAsync();


                SaveOTPInDatabase(advisor.Email, otp);
                SendOTPEmail(advisor.Email, otp);


                return Ok(new
                {
                    message = "OTP sent to your email for verification.",
                    code = 200
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during Signup: " + ex.ToString());

                return StatusCode(500, new
                {
                    message = "An error occurred while processing the request.",
                    details = ex.Message,
                    code = 500
                });
            }

        }

        private string GenerateCustomAdvisorId()
        {
            string customAdvId;
            bool isUnique = false;
            int uniqueNumber = 1;
            do
            {
                customAdvId = "ADV" + uniqueNumber.ToString("D4");
                bool isIdUnique = !_context.Advisor.Any(s => s.AdvisorId == customAdvId);
                if (isIdUnique)
                {
                    isUnique = true;
                }
                else
                {
                    uniqueNumber++;
                }
            } while (!isUnique);
            return customAdvId;
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
        public IActionResult Login([FromBody] AdvisorLogin model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    message = "Invalid login data.",
                    code = 400
                });
            }

            var advisor = _context.Advisor.FirstOrDefault(u => u.Email == model.Email);

            if (advisor == null || !VerifyPassword(model.Password, advisor.Password))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password.",
                    code = 401
                });
            }
            model.FirstName = advisor.FirstName;
            return Ok(new
            {
                message = "Login successful!",
                advisor = model,
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

        public IActionResult UpdateAdvisor(string id, [FromBody] AdvisorUpdateModel updateModel)

        {

            if (updateModel == null)

            {

                return BadRequest(new

                {

                    message = "Invalid update data.",

                    code = 400

                });

            }


            var advisor = _context.Advisor.FirstOrDefault(a => a.AdvisorId == id);


            if (advisor == null)
            {
                return NotFound(new
                {
                    message = "Advisor not found.",
                    code = 404
                });
            }


            if (!string.IsNullOrEmpty(updateModel.FirstName))
            {
                advisor.FirstName = updateModel.FirstName;
            }


            if (!string.IsNullOrEmpty(updateModel.LastName))
            {
                advisor.LastName = updateModel.LastName;
            }


            if (!string.IsNullOrEmpty(updateModel.Email))
            {
                advisor.Email = updateModel.Email;
            }


            if (!string.IsNullOrEmpty(updateModel.Password))
            {
                advisor.Password = updateModel.Password;
                advisor.ConfirmPassword = advisor.Password;
                advisor.Password = HashPassword(advisor.Password);
            }


            if (!string.IsNullOrEmpty(updateModel.PhoneNumber))
            {
                advisor.PhoneNumber = updateModel.PhoneNumber;
            }


            if (!string.IsNullOrEmpty(updateModel.Address))
            {
                advisor.Address = updateModel.Address;
            }


            if (!string.IsNullOrEmpty(updateModel.City))
            {
                advisor.City = updateModel.City;
            }


            if (!string.IsNullOrEmpty(updateModel.State))
            {
                advisor.State = updateModel.State;
            }


            if (!string.IsNullOrEmpty(updateModel.PinCode))
            {
                advisor.PinCode = updateModel.PinCode;
            }

            _context.SaveChanges();

            return Ok(new
            {
                message = "Advisor information updated successfully.",
                code = 200

            });

        }

        [HttpGet("{id}")]

        public IActionResult GetAdvisor(string id)

        {

            var advisor = _context.Advisor.FirstOrDefault(a => a.AdvisorId == id);
            if (advisor == null)
            {
                return NotFound(new
                {
                    message = "Advisor not found.",
                    code = 404
                });
            }


            return Ok(new
            {
                message = "Advisor data retrieved successfully.",
                advisor = advisor,
                code = 200

            });

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvisor(string id)
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
        public async Task<IActionResult> GetClientsByAdvisorId(string advisorId)
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