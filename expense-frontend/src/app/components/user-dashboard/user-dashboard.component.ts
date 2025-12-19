import { Component, Input, OnInit } from '@angular/core';
import { User, UserBalance } from '../../models/user.model';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss']
})
export class UserDashboardComponent implements OnInit {
  @Input() currentUser: User | null = null;
  
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
}