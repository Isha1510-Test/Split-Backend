using ExpenseSharing.Models;

namespace ExpenseSharing.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public List<UserDto> Members { get; set; } = new();
        public List<int> MemberIds { get; set; } = new();
    }

    public class ExpenseDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int PaidById { get; set; }
        public string PaidByName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public SplitType SplitType { get; set; }
        public List<ExpenseSplitDto> Splits { get; set; } = new();
    }

    public class ExpenseSplitDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
    }

    public class BalanceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalOwed { get; set; }
        public decimal TotalOwing { get; set; }
        public decimal NetBalance { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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

    public class SimplifiedDebt
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}