import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { FeedbackService } from '../../../services/feedback.service';
import { AuthService } from '../../../services/auth.service';
import { EventResponseDTO } from '../../../models/event.model';
import { FeedbackSummaryDTO, FeedbackCreateDTO } from '../../../models/feedback.model';
import { UserRole } from '../../../models/user.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event: EventResponseDTO | null = null;
  feedbackSummary: FeedbackSummaryDTO | null = null;
  feedbackForm: FormGroup;
  isLoading = true;
  errorMessage = '';
  successMessage = '';
  showFeedbackForm = false;
  isSubmittingFeedback = false;
  currentUserRole: UserRole | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private feedbackService: FeedbackService,
    private authService: AuthService,
    private formBuilder: FormBuilder
  ) {
    this.feedbackForm = this.formBuilder.group({
      rating: ['', [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.currentUserRole = this.authService.getCurrentUser()?.role || null;
    this.route.params.subscribe(params => {
      const eventId = +params['id'];
      if (eventId) {
        this.loadEvent(eventId);
        this.loadFeedback(eventId);
      }
    });
  }

  loadEvent(eventId: number): void {
    this.eventService.getEventById(eventId).subscribe({
      next: (event) => {
        this.event = event;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load event details. Please try again.';
        this.isLoading = false;
        console.error('Error loading event:', error);
      }
    });
  }

  loadFeedback(eventId: number): void {
    this.feedbackService.getEventFeedback(eventId).subscribe({
      next: (summary) => {
        this.feedbackSummary = summary;
      },
      error: (error) => {
        console.error('Error loading feedback:', error);
      }
    });
  }

  bookTicket(): void {
    if (this.event) {
      this.router.navigate(['/events', this.event.id, 'book']);
    }
  }

  editEvent(): void {
    if (this.event) {
      this.router.navigate(['/events', this.event.id, 'edit']);
    }
  }

  deleteEvent(): void {
    if (this.event && confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(this.event.id).subscribe({
        next: () => {
          this.router.navigate(['/events']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete event. Please try again.';
          console.error('Error deleting event:', error);
        }
      });
    }
  }

  toggleFeedbackForm(): void {
    this.showFeedbackForm = !this.showFeedbackForm;
    if (!this.showFeedbackForm) {
      this.feedbackForm.reset();
      this.successMessage = '';
      this.errorMessage = '';
    }
  }

  submitFeedback(): void {
    if (this.feedbackForm.valid && this.event) {
      this.isSubmittingFeedback = true;
      this.errorMessage = '';

      const feedbackData: FeedbackCreateDTO = {
        eventId: this.event.id,
        ticketId: 1, // You might need to get this from user's tickets
        rating: +this.feedbackForm.value.rating,
        comment: this.feedbackForm.value.comment
      };

      this.feedbackService.createFeedback(feedbackData).subscribe({
        next: (response) => {
          this.isSubmittingFeedback = false;
          this.successMessage = 'Feedback submitted successfully!';
          this.feedbackForm.reset();
          this.showFeedbackForm = false;
          this.loadFeedback(this.event!.id); // Reload feedback
        },
        error: (error) => {
          this.isSubmittingFeedback = false;
          this.errorMessage = error.error?.message || 'Failed to submit feedback. Please try again.';
          console.error('Error submitting feedback:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.feedbackForm.controls).forEach(key => {
      const control = this.feedbackForm.get(key);
      control?.markAsTouched();
    });
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

  canLeaveFeedback(): boolean {
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

  getStarArray(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i + 1);
  }

  get rating() { return this.feedbackForm.get('rating'); }
  get comment() { return this.feedbackForm.get('comment'); }
}