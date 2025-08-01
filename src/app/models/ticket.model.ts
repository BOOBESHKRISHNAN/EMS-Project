export enum TicketStatus {
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Payment_pending = 'Payment_pending'
}

export interface Ticket {
  ticketID: number;
  eventID: number;
  userID: number;
  bookingDate: Date;
  status: TicketStatus;
  numberOfTickets: number;
  event?: Event;
  user?: User;
}

export interface BookTicketViewModel {
  eventID: number;
  numberOfTickets: number;
}

export interface TicketResponseViewModel {
  ticketID: number;
  eventName: string;
  bookingDate: Date;
  status: string;
}

export interface Payment {
  id: number;
  ticketId: number;
  amount: number;
  paymentDate: Date;
  paymentMethod: string;
  status: string;
}

export interface PaymentResponseDTO {
  id: number;
  ticketId: number;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  status: string;
}