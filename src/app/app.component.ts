import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Event Management System';
  isAuthenticated = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
      
      // Redirect to dashboard if user is authenticated and on login/register page
      if (user && (this.router.url === '/login' || this.router.url === '/register' || this.router.url === '/')) {
        this.router.navigate(['/dashboard']);
      }
    });
  }
}