import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import { Group } from '../../models/group.model';
import { Balance } from '../../models/balance.model';
import { GroupService } from '../../services/group.service';
import { BalanceService } from '../../services/balance.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  @Input() currentUser: User | null = null;
  @Output() groupSelected = new EventEmitter<Group>();

  userGroups: Group[] = [];
  groupBalances: { [key: number]: Balance } = {};
  loading = false;

  constructor(
    private groupService: GroupService,
    private balanceService: BalanceService
  ) {}

  ngOnInit() {
    this.loadUserGroups();
  }

  ngOnChanges() {
    if (this.currentUser) {
      this.loadUserGroups();
    }
  }

  loadUserGroups() {
    if (!this.currentUser) return;
    
    this.loading = true;
    this.groupService.getGroupsByUserId(this.currentUser.id!).subscribe({
      next: (groups) => {
        this.userGroups = groups;
        this.loadGroupBalances();
      },
      error: (error) => {
        console.error('Error loading groups:', error);
        this.loading = false;
      }
    });
  }

  loadGroupBalances() {
    if (!this.currentUser) return;

    this.userGroups.forEach(group => {
      this.balanceService.getUserBalance(this.currentUser!.id!, group.id!).subscribe({
        next: (balance) => {
          this.groupBalances[group.id!] = balance;
        },
        error: (error) => {
          console.error('Error loading balance for group:', group.id, error);
        }
      });
    });
    this.loading = false;
  }

  selectGroup(group: Group) {
    this.groupSelected.emit(group);
  }

  getBalanceClass(balance: number): string {
    if (balance > 0) return 'positive';
    if (balance < 0) return 'negative';
    return 'neutral';
  }

  getTotalBalance(): number {
    return Object.values(this.groupBalances)
      .reduce((total, balance) => total + balance.netBalance, 0);
  }
}