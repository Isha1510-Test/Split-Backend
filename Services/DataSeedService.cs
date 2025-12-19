using ExpenseSharing.Data;
using ExpenseSharing.Models;

namespace ExpenseSharing.Services
{
    public class DataSeedService
    {
        private readonly ExpenseSharingContext _context;

        public DataSeedService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            if (_context.Users.Any()) return; // Data already exists

            // Create users with passwords
            var users = new List<User>
            {
                new User { Username = "john_doe", Email = "john123@gmail.com", Name = "John Doe", Password = "john123" },
                new User { Username = "jane_smith", Email = "jane123@gmail.com", Name = "Jane Smith", Password = "jane123" },
                new User { Username = "bob_wilson", Email = "bob123@gmail.com", Name = "Bob Wilson", Password = "bob123" },
                new User { Username = "alice_brown", Email = "alice123@gmail.com", Name = "Alice Brown", Password = "alice123" }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Create groups
            var groups = new List<Group>
            {
                new Group 
                { 
                    Name = "Roommates", 
                    Description = "Shared apartment expenses", 
                    CreatedById = users[0].Id,
                    Members = new List<User> { users[0], users[1], users[2] }
                },
                new Group 
                { 
                    Name = "Trip to Paris", 
                    Description = "Vacation expenses", 
                    CreatedById = users[1].Id,
                    Members = new List<User> { users[1], users[3] }
                }
            };

            _context.Groups.AddRange(groups);
            await _context.SaveChangesAsync();
        }
    }
}