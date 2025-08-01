import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { AuthService } from '../../../services/auth.service';
import { EventResponseDTO } from '../../../models/event.model';
import { UserRole } from '../../../models/user.model';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent implements OnInit {
  events: EventResponseDTO[] = [];
  isLoading = true;
  errorMessage = '';
  currentUserRole: UserRole | null = null;

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUserRole = this.authService.getCurrentUser()?.role || null;
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.eventService.getEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load events. Please try again.';
        this.isLoading = false;
        console.error('Error loading events:', error);
      }
    });
  }

  viewEvent(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  editEvent(eventId: number): void {
    this.router.navigate(['/events', eventId, 'edit']);
  }

  deleteEvent(eventId: number): void {
    if (confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(eventId).subscribe({
        next: () => {
          this.loadEvents(); // Reload the list
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete event. Please try again.';
          console.error('Error deleting event:', error);
        }
      });
    }
  }

  bookTicket(eventId: number): void {
    this.router.navigate(['/events', eventId, 'book']);
  }

  canEditEvent(): boolean {
    return this.authService.hasAnyRole([UserRole.SuperAdmin, UserRole.Admin, UserRole.Organizer]);
  }

  canDeleteEvent(): boolean {
    return this.authService.hasAnyRole([UserRole.SuperAdmin, UserRole.Admin, UserRole.Organizer]);
  }

  canBookTicket(): boolean {
    return this.authService.hasRole(UserRole.RegisteredUser);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}