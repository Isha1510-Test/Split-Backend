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

            // Simple implementation - just return empty balance for now
            return new UserBalanceDto
            {
                UserId = userId,
                UserName = user.Name,
                TotalOwed = 0,
                TotalOwing = 0,
                NetBalance = 0,
                GroupBalances = new List<GroupBalanceDto>()
            };
        }


    }
}