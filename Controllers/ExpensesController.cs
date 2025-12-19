using Microsoft.AspNetCore.Mvc;
using ExpenseSharing.Models;
using ExpenseSharing.Services;
using System.Text.Json;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly ExpenseService _expenseService;

        public ExpensesController(ExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<ExpenseDto>>> GetExpensesByGroupId(int groupId)
        {
            var expenses = await _expenseService.GetExpensesByGroupIdAsync(groupId);
            return Ok(expenses);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] JsonElement request)
        {
            try
            {
                var expenseDto = new ExpenseDto
                {
                    Description = request.GetProperty("description").GetString() ?? "",
                    Amount = decimal.Parse(request.GetProperty("amount").ToString()),
                    PaidById = int.Parse(request.GetProperty("paidById").ToString()),
                    GroupId = int.Parse(request.GetProperty("groupId").ToString()),
                    SplitType = Enum.Parse<SplitType>(request.GetProperty("splitType").GetString() ?? "EQUAL"),
                    Splits = new List<ExpenseSplitDto>()
                };
                
                var splitsArray = request.GetProperty("splits");
                foreach (var split in splitsArray.EnumerateArray())
                {
                    expenseDto.Splits.Add(new ExpenseSplitDto
                    {
                        UserId = int.Parse(split.GetProperty("userId").ToString()),
                        Amount = decimal.Parse(split.GetProperty("amount").ToString()),
                        Percentage = split.TryGetProperty("percentage", out var pct) && pct.ValueKind != JsonValueKind.Null ? decimal.Parse(pct.ToString()) : null
                    });
                }
                
                var createdExpense = await _expenseService.CreateExpenseAsync(expenseDto);
                return Ok(createdExpense);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating expense: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var result = await _expenseService.DeleteExpenseAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}