using DMTDatapointAllocation.BUSINESSLOGIC;
using DMTDatapointAllocation.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DMTDatapointAllocation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DMTDatapointAllocationController : ControllerBase
    {
        private readonly DMTDatapointAllocationBusinessLogic _biz;

        public DMTDatapointAllocationController(DMTDatapointAllocationBusinessLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _biz.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("quarters")]
        public async Task<IActionResult> GetQuarters()
        {
            var quarters = await _biz.GetQuartersAsync();
            return Ok(quarters);
        }

        [HttpGet("budget")]
        public async Task<IActionResult> GetBudget()
        {
            var budgets = await _biz.GetBudgetAsync();
            return Ok(budgets);
        }


        [HttpPost("allocate")]
        public async Task<IActionResult> Allocate([FromBody] AllocationRequest request)
        {
            if (request == null) return BadRequest("Request is null");

            try
            {
                var resp = await _biz.InsertOrUpdateAsync(request);
                if (resp.Success)
                    return Ok(resp);
                return BadRequest(resp);
            }
            catch (Exception ex)
            {
                // log ex
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("{rid}")]
        public async Task<IActionResult> Get(int rid)
        {
            var data = await _biz.GetMasterDataAsync(1, rid); // Choice = 1
            if (data == null || !data.Any())
                return NotFound(new { Message = "No active data found." });

            return Ok(data);
        }

    }
}
