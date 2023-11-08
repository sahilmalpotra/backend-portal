using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Investment_Portal.Controllers
{
    [Route("api/strategies")]
    [ApiController]
    public class StrategiesController : ControllerBase
    {
        private readonly IStrategies _strategyRepository;

        public StrategiesController(IStrategies strategyRepository)
        {
            _strategyRepository = strategyRepository;
        }

        [HttpGet("byname/{strategyName}")]
        public IActionResult GetStrategiesByName(string strategyName)
        {
            var strategies = _strategyRepository.GetStrategiesByName(strategyName);
            if (strategies == null)
            {
                return NotFound();
            }

            return Ok(strategies);
        }

        [HttpGet("bytype/{strategyType}")]
        public IActionResult GetStrategiesByType(string strategyType)
        {
            if (strategyType.Equals("Need Consultation", StringComparison.OrdinalIgnoreCase))
            {
                var strategies = _strategyRepository.GetAllStrategies();
                return Ok(strategies);
            }
            else
            {
                var strategies = _strategyRepository.GetStrategiesByType(strategyType);
                if (strategies == null)
                {
                    return NotFound();
                }

                return Ok(strategies);
            }
        }

        [HttpPost]
        public IActionResult AddStrategy([FromBody] Strategies strategy)
        {
            if (strategy == null)
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                _strategyRepository.AddStrategy(strategy);
                return Ok("Strategy added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
