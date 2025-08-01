import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User, UserRole } from '../../models/user.model';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.css']
})
export class UnauthorizedComponent implements OnInit {
  currentUser: User | null = null;
  attemptedRoute: string | null = null;
  requiredRoles: UserRole[] = [];
  
  // Role enum for template access
  UserRole = UserRole;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadUserData();
    this.getRouteInfo();
  }

  private loadUserData(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  private getRouteInfo(): void {
    // Get the attempted route from query parameters
    this.route.queryParams.subscribe(params => {
      this.attemptedRoute = params['returnUrl'] || null;
      
      // Parse required roles if provided
      if (params['requiredRoles']) {
        try {
          this.requiredRoles = JSON.parse(params['requiredRoles']);
        } catch (error) {
          console.error('Error parsing required roles:', error);
        }
      }
    });
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  goToLogin(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goBack(): void {
    window.history.back();
  }

  goToHome(): void {
    this.router.navigate(['/']);
  }

  // Helper methods for template
  getUserRoleDisplayName(role: UserRole): string {
    switch (role) {
      case UserRole.SuperAdmin:
        return 'Super Administrator';
      case UserRole.Admin:
        return 'Administrator';
      case UserRole.Organizer:
        return 'Event Organizer';
      case UserRole.RegisteredUser:
        return 'Registered User';
      default:
        return 'User';
    }
  }

  getCurrentUserRoleDisplay(): string {
    if (!this.currentUser) return 'Guest';
    return this.getUserRoleDisplayName(this.currentUser.role);
  }

  getRequiredRolesDisplay(): string {
    if (this.requiredRoles.length === 0) return 'Special permissions';
    
    const roleNames = this.requiredRoles.map(role => this.getUserRoleDisplayName(role));
    
    if (roleNames.length === 1) {
      return roleNames[0];
    } else if (roleNames.length === 2) {
      return roleNames.join(' or ');
    } else {
      const lastRole = roleNames.pop();
      return roleNames.join(', ') + ', or ' + lastRole;
    }
  }

  getRouteDisplayName(): string {
    if (!this.attemptedRoute) return 'the requested page';
    
    const routeNames: { [key: string]: string } = {
      '/dashboard': 'Dashboard',
      '/events': 'Events',
      '/events/create': 'Create Event',
      '/locations': 'Location Management',
      '/my-tickets': 'My Tickets',
      '/admin': 'Admin Panel'
    };
    
    // Check for dynamic routes
    if (this.attemptedRoute.includes('/events/') && this.attemptedRoute.includes('/edit')) {
      return 'Edit Event';
    }
    if (this.attemptedRoute.includes('/events/') && this.attemptedRoute.includes('/book')) {
      return 'Book Ticket';
    }
    if (this.attemptedRoute.includes('/events/') && !this.attemptedRoute.includes('/create')) {
      return 'Event Details';
    }
    
    return routeNames[this.attemptedRoute] || 'the requested page';
  }

  canAccessDashboard(): boolean {
    return this.authService.isAuthenticated();
  }

  getErrorMessage(): string {
    if (!this.currentUser) {
      return 'You need to be logged in to access this page.';
    }
    
    if (this.requiredRoles.length > 0) {
      return `You need ${this.getRequiredRolesDisplay()} permissions to access ${this.getRouteDisplayName()}.`;
    }
    
    return 'You do not have permission to access this page.';
  }

  getErrorIcon(): string {
    if (!this.currentUser) {
      return 'fas fa-sign-in-alt';
    }
    return 'fas fa-shield-alt';
  }

  getSuggestionMessage(): string {
    if (!this.currentUser) {
      return 'Please log in with an account that has the necessary permissions.';
    }
    
    return `Your current role is ${this.getCurrentUserRoleDisplay()}. Contact your administrator if you believe you should have access to this resource.`;
  }
}