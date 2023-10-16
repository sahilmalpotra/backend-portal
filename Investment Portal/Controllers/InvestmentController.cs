using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
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

                if (client.AdvisorId == 0)
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


                var createdInvestment = _context.Investments.Add(newInvestment);
                _context.SaveChanges();

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

        // GET api/investments/{id}
        [HttpGet("{id}")]
        public IActionResult GetInvestmentById(int id)
        {
            try
            {
                var investment = _context.Investments.FirstOrDefault(i => i.InvestmentID == id);

                if (investment == null)
                {
                    return NotFound();
                }

                return Ok(investment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/investments/client/{clientId}
        [HttpGet("client/{clientId}")]
        public IActionResult GetInvestmentsByClientId(int clientId)
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
        public IActionResult GetInvestmentsByAdvisorId(int advisorId)
        {
            try
            {
                var investments = _context.Investments.Where(i => i.AdvisorId == advisorId).ToList();

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
        public IActionResult UpdateInvestment(int id, [FromBody] Investment model)
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
        public IActionResult DeleteInvestment(int id)
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


    }
}
