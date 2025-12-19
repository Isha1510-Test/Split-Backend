import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { User, CreateUserRequest } from '../../models/user.model';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  @Output() userSelected = new EventEmitter<User>();

  users: User[] = [];
  showCreateForm = false;
  loading = false;
  
  newUser: CreateUserRequest = {
    username: '',
    email: '',
    name: ''
  };

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.loading = false;
      }
    });
  }

  createUser() {
    if (!this.newUser.username || !this.newUser.email || !this.newUser.name) {
      alert('Please fill in all fields');
      return;
    }

    this.userService.createUser(this.newUser).subscribe({
      next: (user) => {
        this.users.push(user);
        this.resetForm();
        alert('User created successfully!');
      },
      error: (error) => {
        console.error('Error creating user:', error);
        alert('Error creating user. Please try again.');
      }
    });
  }

  selectUser(user: User) {
    this.userSelected.emit(user);
    alert(`Selected user: ${user.name}`);
  }

  deleteUser(user: User) {
    if (confirm(`Are you sure you want to delete ${user.name}?`)) {
      this.userService.deleteUser(user.id!).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.id !== user.id);
          alert('User deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          alert('Error deleting user. Please try again.');
        }
      });
    }
  }

  resetForm() {
    this.newUser = { username: '', email: '', name: '' };
    this.showCreateForm = false;
  }
}