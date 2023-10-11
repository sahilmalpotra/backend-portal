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
        private readonly IInvestment _investmentService;

        public InvestmentController(IInvestment investmentService)
        {
            _investmentService = investmentService;
        }
        
            // POST api/investments
            [HttpPost]
        public IActionResult CreateInvestment([FromBody] Investment model)
        {
            try
            {
                
                if (model == null)
                {
                    return BadRequest("Invalid input data.");
                }

                var newInvestment = new Investment
                {
                    InvestmentAmount = model.InvestmentAmount,
                    InvestmentType = model.InvestmentType,
                    TimePeriod = model.TimePeriod,
                    CreatedDate = DateTime.UtcNow,
                    ClientId = model.ClientId,
                    AdvisorId = model.AdvisorId
                };

                
                var createdInvestment = _investmentService.CreateInvestment(newInvestment);

                return CreatedAtAction("GetInvestment", new { id = createdInvestment.InvestmentID }, createdInvestment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/investments/{id}
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetInvestment(int Id)
        {
            var investment = _investmentService.GetInvestmentById(Id);


            if (investment == null)
            {
                return NotFound(new
                {
                    message = "Investment not found.",
                    code = 404
                });
            }

            return Ok(new
            {
                message = "Investment retrieved successfully.",
                investment,
                code = 200
            });
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

                var updatedInvestment = _investmentService.UpdateInvestment(model);

                if (updatedInvestment == null)
                {
                    return NotFound();
                }

                return Ok(updatedInvestment);
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
                var deletedInvestment = _investmentService.GetInvestmentById(id);

                if (deletedInvestment == null)
                {
                    return NotFound();
                }

                return Ok(deletedInvestment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
