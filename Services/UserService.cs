using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;
using ExpenseSharing.Models;

namespace ExpenseSharing.Services
{
    public class UserService
    {
        private readonly ExpenseSharingContext _context;

        public UserService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Name = u.Name
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Name = userDto.Name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.Id = user.Id;
            return userDto;
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Name = userDto.Name;
            user.Email = userDto.Email;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            
            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }




        public async Task<UserBalanceDto> GetUserOverallBalanceAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Groups)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                    
                if (user == null)
                {
                    // Return empty balance for non-existent user instead of throwing
                    return new UserBalanceDto
                    {
                        UserId = userId,
                        UserName = "Unknown User",
                        TotalOwed = 0,
                        TotalOwing = 0,
                        NetBalance = 0,
                        GroupBalances = new List<GroupBalanceDto>()
                    };
                }

                // Get user's groups
                var userGroups = user.Groups?.ToList() ?? new List<Group>();

                var groupBalances = new List<GroupBalanceDto>();
                decimal totalOwed = 0, totalOwing = 0;

                foreach (var group in userGroups)
                {
                    try
                    {
                        var balance = await CalculateUserBalanceInGroup(userId, group.Id);
                        groupBalances.Add(new GroupBalanceDto
                        {
                            GroupId = group.Id,
                            GroupName = group.Name,
                            NetBalance = balance
                        });

                        if (balance > 0) totalOwed += balance;
                        else totalOwing += Math.Abs(balance);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other groups
                        Console.WriteLine($"Error calculating balance for group {group.Id}: {ex.Message}");
                    }
                }

                return new UserBalanceDto
                {
                    UserId = userId,
                    UserName = user.Name,
                    TotalOwed = totalOwed,
                    TotalOwing = totalOwing,
                    NetBalance = totalOwed - totalOwing,
                    GroupBalances = groupBalances
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserOverallBalanceAsync: {ex.Message}");
                // Return empty balance instead of throwing
                return new UserBalanceDto
                {
                    UserId = userId,
                    UserName = "Error Loading User",
                    TotalOwed = 0,
                    TotalOwing = 0,
                    NetBalance = 0,
                    GroupBalances = new List<GroupBalanceDto>()
                };
            }
        }

        private async Task<decimal> CalculateUserBalanceInGroup(int userId, int groupId)
        {
            // Amount user paid for expenses
            var expenses = await _context.Expenses
                .Where(e => e.PaidById == userId && e.GroupId == groupId)
                .ToListAsync();
            var totalPaid = expenses.Sum(e => e.Amount);

            // Amount user owes from expense splits
            var splits = await _context.ExpenseSplits
                .Include(es => es.Expense)
                .Where(es => es.UserId == userId && es.Expense.GroupId == groupId)
                .ToListAsync();
            var totalOwes = splits.Sum(es => es.Amount);

            // Calculate settlements - same logic as BalanceService
            var settlementsAsPayer = await _context.Settlements
                .Where(s => s.PayerId == userId && s.GroupId == groupId)
                .ToListAsync();
            var settledAsPayer = settlementsAsPayer.Sum(s => s.Amount);

            var settlementsAsPayee = await _context.Settlements
                .Where(s => s.PayeeId == userId && s.GroupId == groupId)
                .ToListAsync();
            var settledAsPayee = settlementsAsPayee.Sum(s => s.Amount);

            // Same settlement logic as BalanceService
            var baseBalance = totalPaid - totalOwes;
            var netBalance = baseBalance + settledAsPayer - settledAsPayee;

            return netBalance;
        }
    }
}