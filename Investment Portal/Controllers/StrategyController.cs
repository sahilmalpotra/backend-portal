using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using Infrastructure.Data;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;





namespace InvestmentPortal.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/strategies")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private readonly IStrategy _strategyService;
        private readonly AppDbContext _context;


        public StrategyController(IStrategy strategyService, AppDbContext context)
        {
            _strategyService = strategyService;
            _context = context;

        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddStrategy([FromBody] Strategy strategy)
        {
            if (strategy == null)
            {
                return BadRequest(new
                {
                    message = "Invalid strategy data.",
                    code = 400
                });
            }



            try
            {
                // Calculate the remaining amount
                decimal sumOfPreviousStrategies = _context.Strategy
                    .Where(s => s.InvestmentId == strategy.InvestmentId && s.Status != "Rejected")
                    .Sum(s => s.InvestmentAmount);

                Investment investment = _context.Investments.FirstOrDefault(i => i.InvestmentID == strategy.InvestmentId);

                if (investment == null)
                {
                    return NotFound(new
                    {
                        message = "Investment not found.",
                        code = 404
                    });
                }

                strategy.ClientId = investment.ClientId;
                strategy.AdvisorId = investment.AdvisorId;

                if (strategy.InvestmentAmount > investment.InvestmentAmount)
                {
                    return BadRequest(new
                    {
                        message = "Investment amount should be less than the total amount.",
                        code = 400
                    });
                }


                decimal remainingAmount = investment.InvestmentAmount - sumOfPreviousStrategies;
                investment.RemainingAmount = investment.RemainingAmount - strategy.InvestmentAmount;
                investment.Status = "In Progess";
                _context.SaveChanges();

                if (strategy.InvestmentAmount > remainingAmount)
                {
                    return BadRequest(new
                    {
                        message = "Strategy amount exceeds the remaining investment amount.",
                        code = 400
                    });
                }
                string customId = GenerateCustomStrategyId();
                strategy.StrategyId = customId;
                var addedStrategyId = await _strategyService.AddStrategyAsync(strategy);


                var clientEmail = _context.Client
                .Where(a => a.ClientId == strategy.ClientId)
                .Select(a => a.Email)
                .FirstOrDefault();

                string clientSubject = "New Strategy Proposed for Your Investment";
                string clientmsg = $@"
                <p>Dear,</p>
                <p>Exciting news! One of our expert advisors has just proposed a new strategy for your investment on INCvest.</p>
                <p>You can now log in to your account to review the details of this strategy and make an informed decision about your investment. Your financial journey is one step closer to success.</p>
                <p>If you have any questions or need further guidance, please don't hesitate to reach out to our support team. We're here to assist you every step of the way.</p>
                <p>Thank you for choosing INCvest for your investments. We look forward to your continued success.</p>
                <p>Best regards,</p>
                <p>INCvest</p>
                ";
                SendEmail(clientEmail, clientmsg, clientSubject);

                return Ok(new
                {
                    message = "Strategy added successfully.",
                    strategyId = addedStrategyId,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while adding the strategy.",
                    details = ex.Message,
                    code = 500
                });
            }
        }

        private string GenerateCustomStrategyId()
        {
            string customStrategyId;
            bool isUnique = false;
            int uniqueNumber = 1;
            do
            {
                customStrategyId = "STR" + uniqueNumber.ToString("D4");
                bool isIdUnique = !_context.Strategy.Any(s => s.StrategyId == customStrategyId);
                if (isIdUnique)
                {
                    isUnique = true;
                }
                else
                {
                    uniqueNumber++;
                }
            } while (!isUnique);
            return customStrategyId;
        }





        [HttpGet]
        public async Task<IActionResult> GetStrategies()
        {
            try
            {
                var strategies = await _strategyService.GetStrategiesAsync();



                if (strategies != null && strategies.Any())
                {
                    return Ok(new
                    {
                        message = "Strategies retrieved successfully.",
                        strategies,
                        code = 200
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "No strategies found.",
                        code = 404
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving strategies.",
                    details = ex.Message,
                    code = 500
                });
            }
        }



        [HttpGet("{investmentId}/By-InvestmentId")]
        public async Task<IActionResult> GetStrategiesByInvestment(string investmentId)
        {
            try
            {
                var strategies = await _strategyService.GetStrategiesByInvestmentIdAsync(investmentId);



                if (strategies == null || !strategies.Any())
                {
                    return NotFound(new
                    {
                        message = "No strategies found for the specified investment ID.",
                        code = 404
                    });
                }



                return Ok(new
                {
                    message = "Strategies retrieved successfully.",
                    strategies,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving strategies by investment ID.",
                    details = ex.Message,
                    code = 500
                });
            }
        }



        [HttpGet("{StrategyId}/By-StrategyId")]
        public async Task<IActionResult> GetStrategy(string StrategyId)
        {
            try
            {
                var strategy = await _strategyService.GetStrategyByStrategyIdAsync(StrategyId);



                if (strategy == null)
                {
                    return NotFound(new
                    {
                        message = "Strategy not found.",
                        code = 404
                    });
                }



                return Ok(new
                {
                    message = "Strategy retrieved successfully.",
                    strategy,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the strategy.",
                    details = ex.Message,
                    code = 500
                });
            }
        }



        [HttpGet("{clientId}/By-ClientId")]
        public async Task<IActionResult> GetStrategiesByClientId(string clientId)
        {
            try
            {
                var strategies = await _strategyService.GetStrategiesByClientIdAsync(clientId);



                if (strategies == null || !strategies.Any())
                {
                    return NotFound(new
                    {
                        message = "No strategies found for the specified client ID.",
                        code = 404
                    });
                }



                return Ok(new
                {
                    message = "Strategies retrieved successfully.",
                    strategies,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving strategies by client ID.",
                    details = ex.Message,
                    code = 500
                });
            }
        }





        [HttpGet("{advisorId}/By-AdvisorId")]
        public async Task<IActionResult> GetStrategiesByAdvisorId(string advisorId)
        {
            try
            {
                var strategies = await _strategyService.GetStrategiesByAdvisorIdAsync(advisorId);



                if (strategies == null || !strategies.Any())
                {
                    return NotFound(new
                    {
                        message = "No strategies found for the specified advisor ID.",
                        code = 404
                    });
                }



                return Ok(new
                {
                    message = "Strategies retrieved successfully.",
                    strategies,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving strategies by advisor ID.",
                    details = ex.Message,
                    code = 500
                });
            }
        }



        [HttpPut("{strategyId}/Update-by-Advisor")]
        public async Task<IActionResult> UpdateStrategy(string strategyId, [FromBody] Strategy strategy)
        {
            if (strategy == null || strategyId != strategy.StrategyId)
            {
                return BadRequest(new
                {
                    message = "Invalid strategy data.",
                    code = 400
                });
            }



            try
            {
                await _strategyService.UpdateStrategyAsync(strategy);
                return Ok(new
                {
                    message = "Strategy updated successfully.",
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the strategy.",
                    details = ex.Message,
                    code = 500
                });
            }
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrategy(string id)
        {
            try
            {
                var strategy = await _strategyService.GetStrategyByStrategyIdAsync(id);



                if (strategy == null)
                {
                    return NotFound(new
                    {
                        message = "Strategy not found.",
                        code = 404
                    });
                }



                // Check if the strategy's status is 'rejected' or 'completed' and the amount is credited
                if (strategy.Status == "rejected" || strategy.Completed)
                {
                    await _strategyService.DeleteStrategyAsync(id);



                    return Ok(new
                    {
                        message = "Strategy deleted successfully.",
                        code = 200
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Strategy cannot be deleted. It must be 'rejected' or  amount credited to user account.",
                        code = 400
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the strategy.",
                    details = ex.Message,
                    code = 500
                });
            }
        }





        [HttpPut("{strategyId}/Update-by-Client")]
        public async Task<IActionResult> UpdatebyClient(string strategyId, [FromBody] StatusUpdateRequest statusUpdate)
        {
            if (statusUpdate == null || !ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid status update request data.",
                    code = 400
                });
            }



            var strategy = await _strategyService.GetStrategyByStrategyIdAsync(strategyId);
            Investment investment = _context.Investments.FirstOrDefault(i => i.InvestmentID == strategy.InvestmentId);



            if (strategy == null)
            {
                return NotFound(new
                {
                    message = "Strategy not found.",
                    code = 404
                });
            }



            try
            {
                strategy.Status = statusUpdate.Status;
                strategy.Remarks = statusUpdate.Remarks;



                await _strategyService.UpdateStrategyAsync(strategy);

                var advisorEmail = _context.Advisor
             .Where(a => a.AdvisorId == strategy.AdvisorId)
             .Select(a => a.Email)
             .FirstOrDefault();

                if (strategy.Status == "Approved")
                {
                    if (investment.RemainingAmount == 0)
                    {
                        investment.Status = "Investment Approved";
                    }
                    _context.SaveChanges();
                    string subject = "Strategy Approved by Investor";
                    string advisormsg = $@"
                    <p>Dear,</p>
                    <p>Great news! We're pleased to inform you that your strategy has been approved by investor with ID {strategy.ClientId} on INCvest. Your expertise and guidance have made a real impact.</p>
                    <p>This successful approval is a testament to your valuable insights and the trust our clients have in your expertise. Keep up the excellent work!</p>
                    <p>If you have any questions or need further assistance, feel free to reach out to our support team.</p>
                    <p>Thank you for your contributions to INCvest. We look forward to your continued success.</p>
                    <p>Best regards,</p>
                    <p>INCvest</p>
                    ";
                    SendEmail(advisorEmail, advisormsg, subject);
                }

                if (strategy.Status == "Rejected")
                { investment.RemainingAmount = investment.RemainingAmount + strategy.InvestmentAmount;
                  _context.SaveChanges();
                    string subject = " Strategy Rejected by Investor";
                    string advisormsg = $@"
                    <p>Dear,</p>
                    <p>We regret to inform you that the strategy you proposed for an investment on INCvest has been rejected by the investor with ID {strategy.ClientId}. While this particular strategy may not have been accepted, we encourage you to view this as an opportunity to refine and create a new strategy.</p>
                    <p>Your expertise is highly valued, and we believe that your next proposal could be the key to success for our clients. Keep up the great work, and we look forward to seeing your new strategy soon.</p>
                    <p>If you have any questions or need further assistance, please don't hesitate to reach out to our support team.</p>
                    <p>Thank you for your dedication to INCvest. Your contributions are invaluable, and we're here to support you in assisting our clients.</p>
                    <p>Best regards,</p>
                    <p>INCvest</p>
                    ";

                    SendEmail(advisorEmail, advisormsg, subject);
                }

                return Ok(new
                {
                    message = "Strategy status and remarks updated successfully.",
                    strategy = strategy,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the strategy status and remarks.",
                    details = ex.Message,
                    code = 500
                });
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