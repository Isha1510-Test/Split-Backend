using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSharing.Models
{
    public class ExpenseSplit
    {
        public int Id { get; set; }
        
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Percentage { get; set; }
    }
}