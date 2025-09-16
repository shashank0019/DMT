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

        [HttpGet("masterdata/{choice}/{rid}")]
        public async Task<IActionResult> GetMasterData(int choice, int rid)
        {
            if (choice <= 0 || rid <= 0)
                return BadRequest("Invalid choice or RID");

            var data = await _biz.GetMasterDataAsync(choice, rid);

            if (data == null || data.Count == 0)
                return NotFound();

            return Ok(data);
        }

    }
}
