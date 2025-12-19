import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { User } from '../../models/user.model';
import { Group } from '../../models/group.model';
import { Expense, CreateExpenseRequest, SplitType, ExpenseSplit } from '../../models/expense.model';
import { ExpenseService } from '../../services/expense.service';

@Component({
  selector: 'app-expense-management',
  templateUrl: './expense-management.component.html',
  styleUrls: ['./expense-management.component.scss']
})
export class ExpenseManagementComponent implements OnInit, OnChanges {
  @Input() selectedGroup: Group | null = null;
  @Input() currentUser: User | null = null;

  expenses: Expense[] = [];
  showCreateForm = false;
  loading = false;
  
  splitTypes = Object.values(SplitType);
  
  newExpense: CreateExpenseRequest = {
    description: '',
    amount: 0,
    paidById: 0,
    groupId: 0,
    splitType: SplitType.EQUAL,
    splits: []
  };

  constructor(private expenseService: ExpenseService) {}

  ngOnInit() {
    this.loadExpenses();
  }

  ngOnChanges() {
    if (this.selectedGroup) {
      this.newExpense.groupId = this.selectedGroup.id!;
      this.loadExpenses();
      this.initializeSplits();
    }
    if (this.currentUser) {
      this.newExpense.paidById = this.currentUser.id!;
    }
  }

  loadExpenses() {
    if (!this.selectedGroup) return;
    
    this.loading = true;
    this.expenseService.getExpensesByGroupId(this.selectedGroup.id!).subscribe({
      next: (expenses) => {
        this.expenses = expenses;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading expenses:', error);
        this.loading = false;
      }
    });
  }

  initializeSplits() {
    if (!this.selectedGroup?.members) return;
    
    this.newExpense.splits = this.selectedGroup.members.map(member => ({
      userId: member.id!,
      userName: member.name,
      amount: 0,
      percentage: 0
    }));
    
    // Auto-calculate for equal split
    if (this.newExpense.splitType === SplitType.EQUAL && this.newExpense.amount > 0) {
      this.calculateEqualSplits();
    }
  }

  onSplitTypeChange() {
    this.calculateSplits();
  }

  onAmountChange() {
    this.calculateSplits();
  }

  calculateSplits() {
    if (!this.newExpense.splits.length || !this.newExpense.amount) return;

    switch (this.newExpense.splitType) {
      case SplitType.EQUAL:
        this.calculateEqualSplits();
        break;
      case SplitType.PERCENTAGE:
        this.calculatePercentageSplits();
        break;
      // For EXACT, user enters amounts manually
    }
  }

  calculateEqualSplits() {
    const splitAmount = this.newExpense.amount / this.newExpense.splits.length;
    this.newExpense.splits.forEach(split => {
      split.amount = Math.round(splitAmount * 100) / 100;
      split.percentage = Math.round((100 / this.newExpense.splits.length) * 100) / 100;
    });
  }

  calculatePercentageSplits() {
    this.newExpense.splits.forEach(split => {
      if (split.percentage) {
        split.amount = Math.round((this.newExpense.amount * split.percentage / 100) * 100) / 100;
      }
    });
  }

  validateSplits(): boolean {
    if (!this.newExpense.splits.length) return false;
    
    switch (this.newExpense.splitType) {
      case SplitType.EXACT:
        const totalAmount = this.newExpense.splits.reduce((sum, split) => sum + (split.amount || 0), 0);
        return Math.abs(totalAmount - this.newExpense.amount) < 0.01;
      
      case SplitType.PERCENTAGE:
        const totalPercentage = this.newExpense.splits.reduce((sum, split) => sum + (split.percentage || 0), 0);
        return Math.abs(totalPercentage - 100) < 0.01;
      
      default:
        return true;
    }
  }

  createExpense() {
    if (!this.newExpense.description || !this.newExpense.amount || !this.selectedGroup) {
      alert('Please fill in all required fields');
      return;
    }

    // Ensure splits are initialized
    if (!this.newExpense.splits || this.newExpense.splits.length === 0) {
      this.initializeSplits();
    }

    // For equal split, recalculate to ensure amounts are set
    if (this.newExpense.splitType === SplitType.EQUAL) {
      this.calculateEqualSplits();
    }

    // Clean the expense object - remove any undefined/null values
    const cleanExpense = {
      description: this.newExpense.description,
      amount: this.newExpense.amount,
      paidById: this.newExpense.paidById,
      groupId: this.newExpense.groupId,
      splitType: this.newExpense.splitType,
      splits: this.newExpense.splits.map(split => ({
        userId: split.userId,
        amount: split.amount || 0,
        percentage: split.percentage || undefined
      }))
    };

    console.log('Creating expense:', cleanExpense);

    this.expenseService.createExpense(cleanExpense).subscribe({
      next: (expense) => {
        this.expenses.unshift(expense);
        this.resetForm();
        alert('Expense created successfully!');
      },
      error: (error) => {
        console.error('Error creating expense:', error);
        alert('Error creating expense: ' + (error.error?.message || error.message || 'Unknown error'));
      }
    });
  }

  deleteExpense(expense: Expense) {
    if (confirm(`Are you sure you want to delete "${expense.description}"?`)) {
      this.expenseService.deleteExpense(expense.id!).subscribe({
        next: () => {
          this.expenses = this.expenses.filter(e => e.id !== expense.id);
          alert('Expense deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting expense:', error);
          alert('Error deleting expense. Please try again.');
        }
      });
    }
  }

  resetForm() {
    this.newExpense = {
      description: '',
      amount: 0,
      paidById: this.currentUser?.id || 0,
      groupId: this.selectedGroup?.id || 0,
      splitType: SplitType.EQUAL,
      splits: []
    };
    this.initializeSplits();
    this.showCreateForm = false;
  }

  getSplitTypeLabel(splitType: SplitType): string {
    switch (splitType) {
      case SplitType.EQUAL: return 'Equal Split';
      case SplitType.EXACT: return 'Exact Amount';
      case SplitType.PERCENTAGE: return 'Percentage';
      default: return splitType;
    }
  }

  getTotalSplitAmount(): number {
    return this.newExpense.splits.reduce((sum, split) => sum + (split.amount || 0), 0);
  }

  getTotalSplitPercentage(): number {
    return this.newExpense.splits.reduce((sum, split) => sum + (split.percentage || 0), 0);
  }
}