using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace InvestmentPortal.Controllers
{
    [EnableCors("AllowAll")]
    //[EnableCors()""]

    // StrategyController.cs
    [Route("api/strategies")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private readonly IStrategy _strategyService;

        public StrategyController(IStrategy strategyService)
        {
            _strategyService = strategyService;
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

            var addedStrategyId = await _strategyService.AddStrategyAsync(strategy);

            return Ok(new
            {
                message = "Strategy added successfully.",
                strategyId = addedStrategyId,
                code = 200
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetStrategies()
        {
            var strategies = await _strategyService.GetStrategiesAsync();

            return Ok(new
            {
                message = "Strategies retrieved successfully.",
                strategies,
                code = 200
            });
        }


        [HttpGet("{investmentId}/By-InvestmentId")]
        public async Task<IActionResult> GetStrategiesByInvestment(int investmentId)
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

        [HttpGet("{StrategyId}/By-StrategyId")]
        public async Task<IActionResult> GetStrategy(int StrategyId)
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

        [HttpGet("{clientId}/By-ClientId")]
        public async Task<IActionResult> GetStrategiesByClientId(int clientId)
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


        [HttpPut("{strategyId}/Update-by-Advisor")]
        public async Task<IActionResult> UpdateStrategy(int strategyId, [FromBody] Strategy strategy)
        {
            if (strategy == null || strategyId != strategy.StrategyId)
            {
                return BadRequest(new
                {
                    message = "Invalid strategy data.",
                    code = 400
                });
            }

            await _strategyService.UpdateStrategyAsync(strategy);

            return Ok(new
            {
                message = "Strategy updated successfully.",
                code = 200
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrategy(int id)
        {
            await _strategyService.DeleteStrategyAsync(id);

            return Ok(new
            {
                message = "Strategy deleted successfully.",
                code = 200
            });
        }

        [HttpPut("{strategyId}/Update-by-Client")]
        public async Task<IActionResult> UpdatebyClient(int strategyId, [FromBody] StatusUpdateRequest statusUpdate)
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

            if (strategy == null)
            {
                return NotFound(new
                {
                    message = "Strategy not found.",
                    code = 404
                });
            }

            strategy.Status = statusUpdate.Status;
            strategy.Remarks = statusUpdate.Remarks;

            await _strategyService.UpdateStrategyAsync(strategy);

            return Ok(new
            {
                message = "Strategy status and remarks updated successfully.",
                strategy = strategy,
                code = 200
            });
        }

    }


}


