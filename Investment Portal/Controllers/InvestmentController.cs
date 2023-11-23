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
                var investments = _context.Investments.Where(i => i.ClientId == clientId)
                 .OrderByDescending(i => i.CreatedDate)
                .ToList();

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
                var investments = _context.Investments.Where(i => i.AdvisorId == advisorId && i.RemainingAmount > 0)
                .OrderByDescending(i => i.CreatedDate)
                .ToList();

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

        // GET api/investments/approved/{advisorId}
        [HttpGet("approved/{advisorId}")]
        public IActionResult GetApprovedInvestments(string advisorId)
        {
            try
            {
                var approvedInvestments = _context.Investments
                    .Where(i => i.Status == "Approved" && i.AdvisorId == advisorId)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToList();

                if (approvedInvestments == null || approvedInvestments.Count() == 0)
                {
                    return NotFound($"No approved investments found for advisor with ID {advisorId}.");
                }

                return Ok(approvedInvestments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //// PUT api/investments/update-status
        //[HttpPut("update-status")]
        //public IActionResult UpdateInvestmentsStatus([FromBody] List<InvestmentUpdate> updateRequests)
        //{
        //    try
        //    {
        //        if (updateRequests == null || !ModelState.IsValid)
        //        {
        //            return BadRequest(new
        //            {
        //                message = "Invalid update status request data.",
        //                code = 400
        //            });
        //        }

        //        foreach (var updateRequest in updateRequests)
        //        {
        //            var investment = _context.Investments.Find(updateRequest.InvestmentId);

        //            if (investment == null)
        //            {
        //                return NotFound(new
        //                {
        //                    message = $"Investment with ID {updateRequest.InvestmentId} not found.",
        //                    code = 404
        //                });
        //            }

        //            investment.Status = updateRequest.Status;

        //            if (updateRequest.Status == "Funded")
        //            {

        //                var strategies = _context.Strategy
        //                    .Where(s => s.InvestmentId == updateRequest.InvestmentId && s.Status == "Approved")
        //                    .ToList();

        //                foreach (var strategy in strategies)
        //                {
        //                    strategy.Status = "Funded";
        //                    _context.Entry(strategy).State = EntityState.Modified;
        //                }

        //                var clientEmail = _context.Client
        //          .Where(a => a.ClientId == investment.ClientId)
        //          .Select(a => a.Email)
        //          .FirstOrDefault();

        //                string clientSubject = "Investment Funded";
        //                string clientmsg = $@"
        //            <p>Dear ,</p>
        //            <p>We're delighted to inform you that your investment with ID {investment.InvestmentID} has been successfully funded. Congratulations on taking this significant step towards your financial goals!</p>
        //            <p>Our team is here to assist you throughout this investment journey. If you have any questions or need further assistance, please don't hesitate to reach out to our support team.</p>
        //            <p>Thank you for choosing INCvest. We wish you continued success in your investment.</p>
        //            <p>Best regards,</p>
        //            <p>INCvest</p>
        //            <h1><a><span class='logo-text'>INCvest</span><span class='dot'>.</span></a></h1>
        //            <style>
        //                .logo-text {{
        //                    color: black;
        //                }}
        //                .dot {{
        //                    color: #4b49ac;
        //                }}
        //            </style>
        //        ";
        //                SendEmail(clientEmail, clientmsg, clientSubject);

        //                var advisorEmail = _context.Advisor
        //                    .Where(a => a.AdvisorId == investment.AdvisorId)
        //                    .Select(a => a.Email)
        //                    .FirstOrDefault();

        //                string advisorSubject = "Investment Funded";
        //                string advisormsg = $@"
        //            <p>Dear ,</p>
        //            <p>The investment with ID {investment.InvestmentID} has been successfully funded. Your guidance and support have contributed to the success of this investment.</p>
        //            <p>If you have any additional insights or recommendations, feel free to share them with the client. Thank you for your valuable contributions to INCvest.</p>
        //            <p>Best regards,</p>
        //            <p>INCvest</p>
        //            <h1><a><span class='logo-text'>INCvest</span><span class='dot'>.</span></a></h1>
        //            <style>
        //                .logo-text {{
        //                    color: black;
        //                }}
        //                .dot {{
        //                    color: #4b49ac;
        //                }}
        //            </style>
        //        ";
        //                SendEmail(advisorEmail, advisormsg, advisorSubject);
        //            }
        //        }

        //        _context.SaveChanges();

        //        return Ok(new
        //        {
        //            message = "Investment statuses updated successfully.",
        //            code = 200
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "An error occurred while updating investment statuses.",
        //            details = ex.Message,
        //            code = 500
        //        });
        //    }
        //}


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
