using Microsoft.AspNetCore.Mvc;
using ExpenseSharing.Models;
using ExpenseSharing.Services;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalancesController : ControllerBase
    {
        private readonly BalanceService _balanceService;

        public BalancesController(BalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet("user/{userId}/group/{groupId}")]
        public async Task<ActionResult<BalanceDto>> GetUserBalance(int userId, int groupId)
        {
            try
            {
                var balance = await _balanceService.GetUserBalanceAsync(userId, groupId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<BalanceDto>>> GetGroupBalances(int groupId)
        {
            var balances = await _balanceService.GetGroupBalancesAsync(groupId);
            return Ok(balances);
        }

        [HttpGet("group/{groupId}/simplified")]
        public async Task<ActionResult<List<SimplifiedDebt>>> GetSimplifiedDebts(int groupId)
        {
            var debts = await _balanceService.GetSimplifiedDebtsAsync(groupId);
            return Ok(debts);
        }

        [HttpPost("settle")]
        public async Task<IActionResult> SettleBalance([FromQuery] int payerId, [FromQuery] int payeeId, [FromQuery] int groupId, [FromQuery] decimal amount)
        {
            try
            {
                await _balanceService.SettleBalanceAsync(payerId, payeeId, groupId, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}