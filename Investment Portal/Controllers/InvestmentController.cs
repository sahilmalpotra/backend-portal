using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
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

namespace Investment_Portal.Controllers
{
    [EnableCors("AllowAll")]


    [Route("api/investments")]
    [ApiController]
    public class InvestmentController : ControllerBase
    {

        private readonly AppDbContext _context;

        public object JsonRequestBehavior { get; private set; }

        public InvestmentController(AppDbContext context)
        {
            _context = context;
        }


        // POST api/investments
        [HttpPost("New Investment")]
        public async Task<IActionResult> CreateInvestment([FromBody] Investment model)
        {
            try
            {

                if (model == null)
                {
                    return BadRequest("Invalid input data.");
                }

                var client = _context.Client.FirstOrDefault(c => c.ClientId == model.ClientId);

                if (client == null)
                {
                    return BadRequest("Client not found.");
                }

                if (client.AdvisorId == "")
                {
                    var lowestClientAdvisor = _context.Advisor
                        .OrderBy(a => a.NumberOfClients)
                        .FirstOrDefault();

                    if (lowestClientAdvisor != null)
                    {
                        lowestClientAdvisor.NumberOfClients++;
                        _context.Advisor.Update(lowestClientAdvisor);

                        client.AdvisorId = lowestClientAdvisor.AdvisorId;
                        _context.Client.Update(client);


                        _context.SaveChanges();

                    }
                }


                var newInvestment = new Investment
                {
                    InvestmentAmount = model.InvestmentAmount,
                    InvestmentType = model.InvestmentType,
                    TimePeriod = model.TimePeriod,
                    CreatedDate = DateTime.UtcNow,
                    ClientId = model.ClientId,
                    AdvisorId = client.AdvisorId
                };

                string customId = GenerateCustomInvestmentId();
                newInvestment.InvestmentID = customId;
                newInvestment.RemainingAmount = model.InvestmentAmount;
                newInvestment.Status = "Pending";

                var createdInvestment = _context.Investments.Add(newInvestment);
                _context.SaveChanges();

                var clientEmail = _context.Client
              .Where(a => a.ClientId == newInvestment.ClientId)
              .Select(a => a.Email)
              .FirstOrDefault();

                var clientName = _context.Client
               .Where(a => a.ClientId == newInvestment.ClientId)
               .Select(a => a.FirstName)
               .FirstOrDefault();

                string clientSubject = "Your Investment Has Been Made";
                string clientmsg = $@"
                <p>Dear,</p>
                <p>Your investment is in! Now, sit back and relax while our advisors create tailored strategies to optimize your portfolio.</p>
                <p>For any questions or assistance, our support team is here.</p>
                <p>Best regards,</p>
                <p>INCvest</p>
                <h1><a><span class='logo-text'>INCvest</span><span class='dot'>.</span></a></h1>
                    <style>
                        .logo-text {{
                            color: black;
                        }}
                        .dot {{
                            color: #4b49ac;
                        }}
                    </style>
                ";
                SendEmail(clientEmail, clientmsg, clientSubject);

                var advisorEmail = _context.Advisor
                .Where(a => a.AdvisorId == newInvestment.AdvisorId)
                .Select(a => a.Email)
                .FirstOrDefault();

                string advisorSubject = "New Investment Available";
                string advisormsg = $@"
                <p>Dear,</p>
                <p>A new investment opportunity has been added by {clientName} with ID {newInvestment.ClientId}. Please log in to review and provide your insights and strategy for this opportunity.</p>
                <p>Your expertise is highly valued, and your advice can help our investors make informed decisions.</p>
                <p>Best regards,</p>
                <p>INCvest</p>
                <h1><a><span class='logo-text'>INCvest</span><span class='dot'>.</span></a></h1>
                    <style>
                        .logo-text {{
                            color: black;
                        }}
                        .dot {{
                            color: #4b49ac;
                        }}
                    </style>
                ";
                SendEmail(advisorEmail, advisormsg, advisorSubject);

                return Ok(new
                {
                    message = "Investment Successfully Generated",
                    Investment = newInvestment,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateCustomInvestmentId()
        {
            string customInvId;
            bool isUnique = false;
            int uniqueNumber = 1;
            do
            {
                customInvId = "INV" + uniqueNumber.ToString("D4");
                bool isIdUnique = !_context.Investments.Any(s => s.InvestmentID == customInvId);

                if (isIdUnique)
                {
                    isUnique = true;
                }
                else
                {
                    uniqueNumber++;
                }
            } while (!isUnique);
            return customInvId;
        }

        [HttpPut("Status-update-ByAdvisor")]
        public IActionResult InvestmentStatusUpdateByAdvisor(string InvestmentId)
        {
            try
            {
                var investment = _context.Investments.Find(InvestmentId);

                if (investment == null)
                {
                    return NotFound("Investment not found.");
                }

                investment.Status = "Funded";

                _context.SaveChanges();

                return Ok(new
                {
                    message = "Investment status updated to funded.",
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/investments/client/{clientId}
        [HttpGet("client/{clientId}")]
        public IActionResult GetInvestmentsByClientId(string clientId)
        {
            try
            {
                var investments = _context.Investments.Where(i => i.ClientId == clientId).ToList();

                if (investments == null || investments.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(investments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/investments/advisor/{advisorId}
        [HttpGet("advisor/{advisorId}")]
        public IActionResult GetInvestmentsByAdvisorId(string advisorId)
        {
            try
            {
                var investments = _context.Investments.Where(i => i.AdvisorId == advisorId && i.RemainingAmount > 0).ToList();

                if (investments == null || investments.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(investments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT api/investments/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateInvestment(string id, [FromBody] Investment model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid input data.");
                }
                var existingInvestment = _context.Investments.FirstOrDefault(i => i.InvestmentID == id);

                if (existingInvestment == null)
                {
                    return NotFound();
                }

                existingInvestment.InvestmentAmount = model.InvestmentAmount;
                existingInvestment.InvestmentType = model.InvestmentType;
                existingInvestment.TimePeriod = model.TimePeriod;

                _context.SaveChanges();

                return Ok(existingInvestment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/investments/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteInvestment(string id)
        {
            try
            {
                var existingInvestment = _context.Investments.FirstOrDefault(i => i.InvestmentID == id);

                if (existingInvestment == null)
                {
                    return NotFound();
                }

                _context.Investments.Remove(existingInvestment); // Remove the investment
                _context.SaveChanges(); // Save the changes

                return Ok(existingInvestment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("send-email")]
        public void SendEmail(string email, string smtpBody, string subject)
        {
            using var smtp = new SmtpClient();

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse("hello.incvest@gmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = "<html><body>" + smtpBody + "</body></html>"

            };

            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("hello.incvest@gmail.com", "lowl auye dojt fjwk");

            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
            
        }
    }
}
