import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { User } from '../../models/user.model';
import { Group } from '../../models/group.model';
import { Balance, SimplifiedDebt } from '../../models/balance.model';
import { BalanceService } from '../../services/balance.service';

@Component({
  selector: 'app-balance-view',
  templateUrl: './balance-view.component.html',
  styleUrls: ['./balance-view.component.scss']
})
export class BalanceViewComponent implements OnInit, OnChanges {
  @Input() selectedGroup: Group | null = null;
  @Input() currentUser: User | null = null;

  balances: Balance[] = [];
  simplifiedDebts: SimplifiedDebt[] = [];
  loading = false;
  activeTab = 'balances';

  constructor(private balanceService: BalanceService) {}

  ngOnInit() {
    this.loadBalances();
  }

  ngOnChanges() {
    if (this.selectedGroup) {
      this.loadBalances();
    }
  }

  loadBalances() {
    if (!this.selectedGroup) return;
    
    this.loading = true;
    
    // Load group balances
    this.balanceService.getGroupBalances(this.selectedGroup.id!).subscribe({
      next: (balances) => {
        this.balances = balances;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading balances:', error);
        this.loading = false;
      }
    });

    // Load simplified debts
    this.balanceService.getSimplifiedDebts(this.selectedGroup.id!).subscribe({
      next: (debts) => {
        this.simplifiedDebts = debts;
      },
      error: (error) => {
        console.error('Error loading simplified debts:', error);
      }
    });
  }

  settleDebt(debt: SimplifiedDebt) {
    if (!this.selectedGroup) return;

    // Find user IDs from names
    const payer = this.selectedGroup.members?.find(m => m.name === debt.from);
    const payee = this.selectedGroup.members?.find(m => m.name === debt.to);

    if (!payer || !payee) {
      alert('Error: Could not find users for settlement');
      return;
    }

    if (confirm(`Settle $${debt.amount.toFixed(2)} from ${debt.from} to ${debt.to}?`)) {
      this.balanceService.settleBalance(payer.id!, payee.id!, this.selectedGroup.id!, debt.amount).subscribe({
        next: () => {
          alert('Settlement recorded successfully!');
          this.loadBalances();
        },
        error: (error) => {
          console.error('Error settling balance:', error);
          alert('Error recording settlement. Please try again.');
        }
      });
    }
  }

  getBalanceClass(balance: number): string {
    if (balance > 0) return 'positive';
    if (balance < 0) return 'negative';
    return 'neutral';
  }

  getBalanceStatus(balance: number): string {
    if (balance > 0) return 'You are owed';
    if (balance < 0) return 'You owe';
    return 'Settled up';
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  getBalanceWidth(netBalance: number): number {
    return Math.min(Math.abs(netBalance) / 100 * 100, 100);
  }

  getTotalDebtAmount(): number {
    return this.simplifiedDebts.reduce((sum, debt) => sum + debt.amount, 0);
  }
}