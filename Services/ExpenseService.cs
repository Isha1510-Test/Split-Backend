using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;
using ExpenseSharing.Models;

namespace ExpenseSharing.Services
{
    public class ExpenseService
    {
        private readonly ExpenseSharingContext _context;

        public ExpenseService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseDto>> GetExpensesByGroupIdAsync(int groupId)
        {
            return await _context.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Group)
                .Include(e => e.Splits)
                .ThenInclude(s => s.User)
                .Where(e => e.GroupId == groupId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    PaidById = e.PaidById,
                    PaidByName = e.PaidBy.Name,
                    GroupId = e.GroupId,
                    GroupName = e.Group.Name,
                    SplitType = e.SplitType,
                    Splits = e.Splits.Select(s => new ExpenseSplitDto
                    {
                        Id = s.Id,
                        UserId = s.UserId,
                        UserName = s.User.Name,
                        Amount = s.Amount,
                        Percentage = s.Percentage
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<ExpenseDto> CreateExpenseAsync(ExpenseDto expenseDto)
        {
            try
            {
                Console.WriteLine($"Service: Creating expense with {expenseDto.Splits?.Count} splits");
                
                var expense = new Expense
                {
                    Description = expenseDto.Description,
                    Amount = expenseDto.Amount,
                    PaidById = expenseDto.PaidById,
                    GroupId = expenseDto.GroupId,
                    SplitType = expenseDto.SplitType
                };

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                // Calculate and create splits
                if (expenseDto.Splits != null && expenseDto.Splits.Any())
                {
                    var splits = CalculateSplits(expense, expenseDto.Splits);
                    _context.ExpenseSplits.AddRange(splits);
                    await _context.SaveChangesAsync();
                }

                // Return the created expense with splits
                return await _context.Expenses
                    .Include(e => e.PaidBy)
                    .Include(e => e.Group)
                    .Include(e => e.Splits)
                    .ThenInclude(s => s.User)
                    .Where(e => e.Id == expense.Id)
                    .Select(e => new ExpenseDto
                    {
                        Id = e.Id,
                        Description = e.Description,
                        Amount = e.Amount,
                        PaidById = e.PaidById,
                        PaidByName = e.PaidBy.Name,
                        GroupId = e.GroupId,
                        GroupName = e.Group.Name,
                        SplitType = e.SplitType,
                        Splits = e.Splits.Select(s => new ExpenseSplitDto
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            UserName = s.User.Name,
                            Amount = s.Amount,
                            Percentage = s.Percentage
                        }).ToList()
                    })
                    .FirstAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service error: {ex.Message}");
                throw;
            }
        }

        private List<ExpenseSplit> CalculateSplits(Expense expense, List<ExpenseSplitDto> splitDtos)
        {
            var splits = new List<ExpenseSplit>();

            switch (expense.SplitType)
            {
                case SplitType.EQUAL:
                    var splitAmount = Math.Round(expense.Amount / splitDtos.Count, 2);
                    foreach (var splitDto in splitDtos)
                    {
                        splits.Add(new ExpenseSplit
                        {
                            ExpenseId = expense.Id,
                            UserId = splitDto.UserId,
                            Amount = splitAmount,
                            Percentage = Math.Round(100m / splitDtos.Count, 2)
                        });
                    }
                    break;

                case SplitType.EXACT:
                    foreach (var splitDto in splitDtos)
                    {
                        splits.Add(new ExpenseSplit
                        {
                            ExpenseId = expense.Id,
                            UserId = splitDto.UserId,
                            Amount = splitDto.Amount
                        });
                    }
                    break;

                case SplitType.PERCENTAGE:
                    foreach (var splitDto in splitDtos)
                    {
                        var amount = Math.Round(expense.Amount * (splitDto.Percentage ?? 0) / 100, 2);
                        splits.Add(new ExpenseSplit
                        {
                            ExpenseId = expense.Id,
                            UserId = splitDto.UserId,
                            Amount = amount,
                            Percentage = splitDto.Percentage
                        });
                    }
                    break;
            }

            return splits;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}