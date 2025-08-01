import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LocationWithEventsDto, LocationCreateDto, LocationUpdateDto, Location } from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = 'https://localhost:7297/api/Location'; // Update with your backend URL

  constructor(private http: HttpClient) {}

  getLocations(): Observable<LocationWithEventsDto[]> {
    return this.http.get<LocationWithEventsDto[]>(this.apiUrl);
  }

  getLocationById(id: number): Observable<LocationWithEventsDto> {
    return this.http.get<LocationWithEventsDto>(`${this.apiUrl}/${id}`);
  }

  createLocation(location: LocationCreateDto): Observable<Location> {
    return this.http.post<Location>(this.apiUrl, location);
  }

  updateLocation(id: number, location: LocationUpdateDto): Observable<Location> {
    return this.http.put<Location>(`${this.apiUrl}/${id}`, location);
  }

  deleteLocation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}