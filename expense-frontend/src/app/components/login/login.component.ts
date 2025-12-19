import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;
  error = '';

  constructor(private userService: UserService) {}

  login() {
    if (!this.email || !this.password) {
      this.error = 'Please enter email and password';
      return;
    }

    this.loading = true;
    this.error = '';

    this.userService.login(this.email, this.password).subscribe({
      next: (user) => {
        localStorage.setItem('currentUser', JSON.stringify(user));
        window.location.reload();
      },
      error: (error) => {
        this.error = 'Invalid credentials';
        this.loading = false;
      }
    });
  }

  // Quick login buttons for demo
  quickLogin(email: string, password: string) {
    this.email = email;
    this.password = password;
    this.login();
  }
}