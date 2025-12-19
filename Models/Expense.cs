using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSharing.Models
{
    public class Expense
    {
        public int Id { get; set; }
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        public int PaidById { get; set; }
        public User PaidBy { get; set; } = null!;
        
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
        
        public SplitType SplitType { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public List<ExpenseSplit> Splits { get; set; } = new();
    }
}