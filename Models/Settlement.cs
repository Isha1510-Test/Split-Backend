using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSharing.Models
{
    public class Settlement
    {
        public int Id { get; set; }
        
        public int PayerId { get; set; }
        public User Payer { get; set; } = null!;
        
        public int PayeeId { get; set; }
        public User Payee { get; set; } = null!;
        
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        public DateTime SettledAt { get; set; } = DateTime.Now;
    }
}