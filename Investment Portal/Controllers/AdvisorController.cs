using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Application.Services;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Microsoft.AspNetCore.Cors;

namespace InvestmentPortal.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisorController : ControllerBase
    {
        private readonly AdvisorService _advisorService;

        public AdvisorController(AdvisorService advisorService)
        {
            _advisorService = advisorService;
        }

        [HttpGet]
        public async Task<ActionResult<Advisor>> GetAdvisorWithLowestClients()
        {
            var advisor = await _advisorService.GetAdvisorWithLowestClientsAsync();

            if (advisor == null)
            {
                return NotFound();
            }

            return Ok(advisor);
        }
    }
}
