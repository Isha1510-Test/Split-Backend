using System.ComponentModel.DataAnnotations;

namespace ExpenseSharing.Models
{
    public class Group
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public List<User> Members { get; set; } = new();
        public List<Expense> Expenses { get; set; } = new();
    }
}