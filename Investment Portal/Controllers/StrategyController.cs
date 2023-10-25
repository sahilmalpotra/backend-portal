using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using Infrastructure.Data;





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
                    .Where(s => s.InvestmentId == strategy.InvestmentId)
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

                if (strategy.InvestmentAmount > investment.InvestmentAmount)
                {
                    return BadRequest(new
                    {
                        message = "Investment amount should be less than the total amount.",
                        code = 400
                    });
                }


                decimal remainingAmount = investment.InvestmentAmount - sumOfPreviousStrategies;

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
    }
}