import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EventService } from '../../../services/event.service';
import { TicketService } from '../../../services/ticket.service';
import { EventResponseDTO } from '../../../models/event.model';
import { BookTicketViewModel } from '../../../models/ticket.model';

@Component({
  selector: 'app-ticket-booking',
  templateUrl: './ticket-booking.component.html',
  styleUrls: ['./ticket-booking.component.css']
})
export class TicketBookingComponent implements OnInit {
  event: EventResponseDTO | null = null;
  bookingForm: FormGroup;
  isLoading = false;
  isLoadingEvent = true;
  errorMessage = '';
  successMessage = '';
  eventId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private eventService: EventService,
    private ticketService: TicketService
  ) {
    this.bookingForm = this.formBuilder.group({
      numberOfTickets: ['1', [Validators.required, Validators.min(1), Validators.max(10)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.eventId = +params['id'];
      if (this.eventId) {
        this.loadEvent();
      }
    });
  }

  loadEvent(): void {
    this.isLoadingEvent = true;
    this.eventService.getEventById(this.eventId).subscribe({
      next: (event) => {
        this.event = event;
        this.isLoadingEvent = false;
        
        // Check if event has already started
        const eventStartDate = new Date(event.startDate);
        const now = new Date();
        
        if (eventStartDate <= now) {
          this.errorMessage = 'This event has already started. Ticket booking is no longer available.';
        }
      },
      error: (error) => {
        this.errorMessage = 'Failed to load event details. Please try again.';
        this.isLoadingEvent = false;
        console.error('Error loading event:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.bookingForm.valid && this.event) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const bookingData: BookTicketViewModel = {
        eventID: this.event.id,
        numberOfTickets: +this.bookingForm.value.numberOfTickets
      };

      this.ticketService.bookTicket(bookingData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage = `Successfully booked ${bookingData.numberOfTickets} ticket(s)! Ticket ID: ${response.ticketID}`;
          
          // Redirect to my tickets after a delay
          setTimeout(() => {
            this.router.navigate(['/my-tickets']);
          }, 3000);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to book ticket. Please try again.';
          console.error('Error booking ticket:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.bookingForm.controls).forEach(key => {
      const control = this.bookingForm.get(key);
      control?.markAsTouched();
    });
  }

  calculateTotal(): number {
    if (!this.event) return 0;
    const numberOfTickets = +this.bookingForm.value.numberOfTickets || 0;
    return numberOfTickets * 50; // Assuming a base price, you can get this from event.ticketPrice if available
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

  get numberOfTickets() { return this.bookingForm.get('numberOfTickets'); }
}