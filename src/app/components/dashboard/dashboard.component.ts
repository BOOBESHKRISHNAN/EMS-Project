import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { EventService } from '../../services/event.service';
import { TicketService } from '../../services/ticket.service';
import { LocationService } from '../../services/location.service';
import { User, UserRole } from '../../models/user.model';
import { EventResponseDTO } from '../../models/event.model';
import { TicketResponseViewModel } from '../../models/ticket.model';
import { LocationWithEventsDto } from '../../models/event.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  events: EventResponseDTO[] = [];
  myTickets: TicketResponseViewModel[] = [];
  locations: LocationWithEventsDto[] = [];
  loading = true;
  error: string | null = null;
  currentDate = new Date();

  // Role enum for template access
  UserRole = UserRole;

  constructor(
    private authService: AuthService,
    private eventService: EventService,
    private ticketService: TicketService,
    private locationService: LocationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserData();
    this.loadDashboardData();
  }

  private loadUserData(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  private loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    // Load events for all users
    this.eventService.getEvents().subscribe({
      next: (events) => {
        this.events = events.slice(0, 5); // Show only latest 5 events
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.error = 'Failed to load events';
      }
    });

    // Load user-specific data based on role
    if (this.currentUser) {
      switch (this.currentUser.role) {
        case UserRole.RegisteredUser:
          this.loadUserTickets();
          break;
        case UserRole.SuperAdmin:
          this.loadLocations();
          break;
        case UserRole.Admin:
        case UserRole.Organizer:
          // Additional data for admins and organizers if needed
          break;
      }
    }

    this.loading = false;
  }

  private loadUserTickets(): void {
    this.ticketService.getMyTickets().subscribe({
      next: (tickets) => {
        this.myTickets = tickets.slice(0, 5); // Show only latest 5 tickets
      },
      error: (error) => {
        console.error('Error loading tickets:', error);
      }
    });
  }

  private loadLocations(): void {
    this.locationService.getLocations().subscribe({
      next: (locations) => {
        this.locations = locations.slice(0, 5); // Show only 5 locations
      },
      error: (error) => {
        console.error('Error loading locations:', error);
      }
    });
  }

  // Navigation methods
  navigateToEvents(): void {
    this.router.navigate(['/events']);
  }

  navigateToMyTickets(): void {
    this.router.navigate(['/my-tickets']);
  }

  navigateToLocations(): void {
    this.router.navigate(['/locations']);
  }

  navigateToCreateEvent(): void {
    this.router.navigate(['/events/create']);
  }

  viewEventDetails(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Helper methods for template
  canCreateEvents(): boolean {
    return this.authService.hasAnyRole([UserRole.Organizer, UserRole.Admin, UserRole.SuperAdmin]);
  }

  canManageLocations(): boolean {
    return this.authService.hasRole(UserRole.SuperAdmin);
  }

  canViewTickets(): boolean {
    return this.authService.hasRole(UserRole.RegisteredUser);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  getWelcomeMessage(): string {
    if (!this.currentUser) return 'Welcome to Event Management';
    
    const firstName = this.currentUser.firstName || 'User';
    const roleDisplay = this.getRoleDisplayName(this.currentUser.role);
    
    return `Welcome back, ${firstName}! (${roleDisplay})`;
  }

  private getRoleDisplayName(role: UserRole): string {
    switch (role) {
      case UserRole.SuperAdmin:
        return 'Super Admin';
      case UserRole.Admin:
        return 'Admin';
      case UserRole.Organizer:
        return 'Organizer';
      case UserRole.RegisteredUser:
        return 'User';
      default:
        return 'User';
    }
  }
}