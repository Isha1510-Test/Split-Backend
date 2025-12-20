using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;
using ExpenseSharing.Models;

namespace ExpenseSharing.Services
{
    public class BalanceService
    {
        private readonly ExpenseSharingContext _context;

        public BalanceService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public async Task<List<BalanceDto>> GetGroupBalancesAsync(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null) return new List<BalanceDto>();

            var balances = new List<BalanceDto>();

            foreach (var member in group.Members)
            {
                var balance = await CalculateUserBalanceAsync(member.Id, groupId);
                balances.Add(balance);
            }

            return balances;
        }

        public async Task<BalanceDto> GetUserBalanceAsync(int userId, int groupId)
        {
            return await CalculateUserBalanceAsync(userId, groupId);
        }

        private async Task<BalanceDto> CalculateUserBalanceAsync(int userId, int groupId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            // Calculate what user paid
            var expenses = await _context.Expenses
                .Where(e => e.PaidById == userId && e.GroupId == groupId)
                .ToListAsync();
            var totalPaid = expenses.Sum(e => e.Amount);

            // Calculate what user owes
            var splits = await _context.ExpenseSplits
                .Include(es => es.Expense)
                .Where(es => es.UserId == userId && es.Expense.GroupId == groupId)
                .ToListAsync();
            var totalOwes = splits.Sum(es => es.Amount);

            // Calculate settlements - this is the key fix
            // When user is payer (pays someone), it reduces what they owe
            // When user is payee (receives payment), it reduces what others owe them
            var settlementsAsPayer = await _context.Settlements
                .Where(s => s.PayerId == userId && s.GroupId == groupId)
                .ToListAsync();
            var settledAsPayer = settlementsAsPayer.Sum(s => s.Amount);

            var settlementsAsPayee = await _context.Settlements
                .Where(s => s.PayeeId == userId && s.GroupId == groupId)
                .ToListAsync();
            var settledAsPayee = settlementsAsPayee.Sum(s => s.Amount);

            // Correct settlement logic:
            // Base balance = what I paid - what I owe
            // If I pay someone (settledAsPayer): reduces my negative balance (less debt)
            // If someone pays me (settledAsPayee): reduces my positive balance (less owed to me)
            var baseBalance = totalPaid - totalOwes;
            var netBalance = baseBalance + settledAsPayer - settledAsPayee;
            
            Console.WriteLine($"User {user.Name}: BaseBalance={baseBalance}, PaidOut={settledAsPayer}, Received={settledAsPayee}, NetBalance={netBalance}");

            return new BalanceDto
            {
                UserId = userId,
                UserName = user.Name,
                TotalOwed = netBalance > 0 ? netBalance : 0,
                TotalOwing = netBalance < 0 ? Math.Abs(netBalance) : 0,
                NetBalance = netBalance
            };
        }

        public async Task<List<SimplifiedDebt>> GetSimplifiedDebtsAsync(int groupId)
        {
            var balances = await GetGroupBalancesAsync(groupId);
            return SimplifyDebts(balances);
        }

        private List<SimplifiedDebt> SimplifyDebts(List<BalanceDto> balances)
        {
            var debts = new List<SimplifiedDebt>();

            var creditors = balances
                .Where(b => b.NetBalance > 0)
                .OrderByDescending(b => b.NetBalance)
                .ToList();

            var debtors = balances
                .Where(b => b.NetBalance < 0)
                .OrderBy(b => b.NetBalance)
                .ToList();

            int i = 0, j = 0;
            while (i < creditors.Count && j < debtors.Count)
            {
                var creditor = creditors[i];
                var debtor = debtors[j];

                var credit = creditor.NetBalance;
                var debt = Math.Abs(debtor.NetBalance);

                var settleAmount = Math.Min(credit, debt);

                debts.Add(new SimplifiedDebt
                {
                    From = debtor.UserName,
                    To = creditor.UserName,
                    Amount = settleAmount
                });

                creditor.NetBalance -= settleAmount;
                debtor.NetBalance += settleAmount;

                if (creditor.NetBalance == 0) i++;
                if (debtor.NetBalance == 0) j++;
            }

            return debts;
        }

        public async Task SettleBalanceAsync(int payerId, int payeeId, int groupId, decimal amount)
        {
            var settlement = new Settlement
            {
                PayerId = payerId,
                PayeeId = payeeId,
                GroupId = groupId,
                Amount = amount
            };

            _context.Settlements.Add(settlement);
            await _context.SaveChangesAsync();
        }
    }
}