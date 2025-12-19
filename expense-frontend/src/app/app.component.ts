import { Component, OnInit } from '@angular/core';
import { User, UserBalance } from './models/user.model';
import { Group } from './models/group.model';
import { UserService } from './services/user.service';
import { GroupService } from './services/group.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Expense Sharing App';
  currentUser: User | null = null;
  selectedGroup: Group | null = null;
  activeTab = 'dashboard';
  isLoggedIn = false;

  constructor(
    private userService: UserService,
    private groupService: GroupService
  ) {}

  ngOnInit() {
    // Check if user is already logged in
    this.currentUser = this.userService.getCurrentUser();
    this.isLoggedIn = !!this.currentUser;
  }

  onUserSelected(user: User) {
    this.currentUser = user;
  }

  onGroupSelected(group: Group) {
    this.selectedGroup = group;
    this.activeTab = 'expenses';
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  logout() {
    this.userService.logout();
    this.currentUser = null;
    this.isLoggedIn = false;
    this.selectedGroup = null;
    this.activeTab = 'dashboard';
  }
}