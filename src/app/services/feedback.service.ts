import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FeedbackCreateDTO, FeedbackResponseDTO, FeedbackSummaryDTO } from '../models/feedback.model';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  private apiUrl = 'https://localhost:7297/api/Feedback'; // Update with your backend URL

  constructor(private http: HttpClient) {}

  createFeedback(feedback: FeedbackCreateDTO): Observable<FeedbackResponseDTO> {
    return this.http.post<FeedbackResponseDTO>(this.apiUrl, feedback);
  }

  getEventFeedback(eventId: number): Observable<FeedbackSummaryDTO> {
    return this.http.get<FeedbackSummaryDTO>(`${this.apiUrl}/event/${eventId}`);
  }

  getFeedbackById(id: number): Observable<FeedbackResponseDTO> {
    return this.http.get<FeedbackResponseDTO>(`${this.apiUrl}/${id}`);
  }

  updateFeedback(id: number, feedback: FeedbackCreateDTO): Observable<FeedbackResponseDTO> {
    return this.http.put<FeedbackResponseDTO>(`${this.apiUrl}/${id}`, feedback);
  }

  deleteFeedback(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}