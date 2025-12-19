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
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            var userGroups = await _context.Groups
                .Where(g => g.Members.Any(m => m.Id == userId))
                .ToListAsync();

            var groupBalances = new List<GroupBalanceDto>();
            decimal totalOwed = 0, totalOwing = 0;

            foreach (var group in userGroups)
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

        private async Task<decimal> CalculateUserBalanceInGroup(int userId, int groupId)
        {
            var totalPaid = await _context.Expenses
                .Where(e => e.PaidById == userId && e.GroupId == groupId)
                .SumAsync(e => e.Amount);

            var totalOwes = await _context.ExpenseSplits
                .Include(es => es.Expense)
                .Where(es => es.UserId == userId && es.Expense.GroupId == groupId)
                .SumAsync(es => es.Amount);

            var settledAsPayer = await _context.Settlements
                .Where(s => s.PayerId == userId && s.GroupId == groupId)
                .SumAsync(s => s.Amount);

            var settledAsPayee = await _context.Settlements
                .Where(s => s.PayeeId == userId && s.GroupId == groupId)
                .SumAsync(s => s.Amount);

            return totalPaid - totalOwes + settledAsPayee - settledAsPayer;
        }
    }
}