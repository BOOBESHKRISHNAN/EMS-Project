import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventResponseDTO, EventCreateDTO } from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'https://localhost:7297/api/Event'; // Update with your backend URL

  constructor(private http: HttpClient) {}

  getEvents(): Observable<EventResponseDTO[]> {
    return this.http.get<EventResponseDTO[]>(this.apiUrl);
  }

  getEventById(id: number): Observable<EventResponseDTO> {
    return this.http.get<EventResponseDTO>(`${this.apiUrl}/${id}`);
  }

  createEvent(event: EventCreateDTO): Observable<EventResponseDTO> {
    return this.http.post<EventResponseDTO>(this.apiUrl, event);
  }

  updateEvent(id: number, event: EventCreateDTO): Observable<EventResponseDTO> {
    return this.http.put<EventResponseDTO>(`${this.apiUrl}/${id}`, event);
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}