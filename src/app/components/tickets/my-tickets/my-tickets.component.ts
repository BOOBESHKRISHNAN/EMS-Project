import { Component, OnInit } from '@angular/core';
import { TicketService } from '../../../services/ticket.service';
import { TicketResponseViewModel } from '../../../models/ticket.model';

@Component({
  selector: 'app-my-tickets',
  templateUrl: './my-tickets.component.html',
  styleUrls: ['./my-tickets.component.css']
})
export class MyTicketsComponent implements OnInit {
  tickets: TicketResponseViewModel[] = [];
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.isLoading = true;
    this.ticketService.getMyTickets().subscribe({
      next: (tickets) => {
        this.tickets = tickets;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load your tickets. Please try again.';
        this.isLoading = false;
        console.error('Error loading tickets:', error);
      }
    });
  }

  cancelTicket(ticketId: number): void {
    if (confirm('Are you sure you want to cancel this ticket? This action cannot be undone.')) {
      this.ticketService.cancelTicket(ticketId).subscribe({
        next: (response) => {
          this.successMessage = 'Ticket cancelled successfully.';
          this.loadTickets(); // Reload the tickets
          
          // Clear success message after 3 seconds
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to cancel ticket. Please try again.';
          console.error('Error cancelling ticket:', error);
          
          // Clear error message after 5 seconds
          setTimeout(() => {
            this.errorMessage = '';
          }, 5000);
        }
      });
    }
  }

  canCancelTicket(ticket: TicketResponseViewModel): boolean {
    // Can only cancel confirmed tickets
    return ticket.status === 'Confirmed';
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'status-confirmed';
      case 'cancelled':
        return 'status-cancelled';
      case 'payment_pending':
        return 'status-pending';
      default:
        return 'status-default';
    }
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

  getActiveTickets(): TicketResponseViewModel[] {
    return this.tickets.filter(ticket => ticket.status !== 'Cancelled');
  }

  getCancelledTickets(): TicketResponseViewModel[] {
    return this.tickets.filter(ticket => ticket.status === 'Cancelled');
  }
}