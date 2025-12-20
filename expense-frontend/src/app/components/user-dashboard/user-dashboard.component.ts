import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { User, UserBalance } from '../../models/user.model';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss']
})
export class UserDashboardComponent implements OnInit {
  @Input() currentUser: User | null = null;
  @Output() tabChanged = new EventEmitter<string>();
  
  userBalance: UserBalance | null = null;
  loading = false;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.loadUserBalance();
  }

  loadUserBalance() {
    if (!this.currentUser) return;
    
    this.loading = true;
    this.userService.getUserBalance(this.currentUser.id!).subscribe({
      next: (balance) => {
        this.userBalance = balance;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading balance:', error);
        // Set default values on error
        this.userBalance = {
          userId: this.currentUser!.id!,
          userName: this.currentUser!.name,
          totalOwed: 0,
          totalOwing: 0,
          netBalance: 0,
          groupBalances: []
        };
        this.loading = false;
      }
    });
  }

  getBalanceClass(balance: number): string {
    if (balance > 0) return 'positive';
    if (balance < 0) return 'negative';
    return 'neutral';
  }

  getAbsoluteValue(value: number): number {
    return Math.abs(value);
  }

  navigateToTab(tab: string) {
    this.tabChanged.emit(tab);
  }
}