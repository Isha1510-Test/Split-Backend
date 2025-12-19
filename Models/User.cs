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

    public class UserBalanceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalOwed { get; set; }
        public decimal TotalOwing { get; set; }
        public decimal NetBalance { get; set; }
        public List<GroupBalanceDto> GroupBalances { get; set; } = new();
    }

    public class GroupBalanceDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public decimal NetBalance { get; set; }
    }
}