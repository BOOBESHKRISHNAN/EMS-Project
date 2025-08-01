import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookTicketViewModel, TicketResponseViewModel } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = 'https://localhost:7297/api/Ticket'; // Update with your backend URL

  constructor(private http: HttpClient) {}

  bookTicket(ticketData: BookTicketViewModel): Observable<{ ticketID: number }> {
    return this.http.post<{ ticketID: number }>(`${this.apiUrl}/book`, ticketData);
  }

  getMyTickets(): Observable<TicketResponseViewModel[]> {
    return this.http.get<TicketResponseViewModel[]>(`${this.apiUrl}/my-tickets`);
  }

  cancelTicket(ticketId: number): Observable<string> {
    return this.http.put<string>(`${this.apiUrl}/cancel/${ticketId}`, {});
  }
}