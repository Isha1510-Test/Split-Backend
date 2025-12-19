using System.ComponentModel.DataAnnotations;

namespace ExpenseSharing.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public List<Group> Groups { get; set; } = new();
        public List<Expense> PaidExpenses { get; set; } = new();
        public List<ExpenseSplit> ExpenseSplits { get; set; } = new();
    }
}