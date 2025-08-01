import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { 
  LoginRequest, 
  LoginResponse, 
  RegisterUserRequest, 
  RegisterAdminRequest, 
  RegisterOrganizerRequest, 
  User, 
  UserRole 
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7297/api/User'; // Update with your backend URL
  private tokenKey = 'auth_token';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.loadUserFromToken();
        })
      );
  }

  registerUser(userData: RegisterUserRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register-User`, userData);
  }

  registerAdmin(userData: RegisterAdminRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register-Admin-by-SuperAdmin`, userData);
  }

  registerOrganizer(userData: RegisterOrganizerRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register-Organizer-by-SuperAdmin`, userData);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp > Date.now() / 1000;
    } catch {
      return false;
    }
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: UserRole): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  hasAnyRole(roles: UserRole[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.role) : false;
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (!token || !this.isAuthenticated()) {
      this.currentUserSubject.next(null);
      return;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const user: User = {
        id: parseInt(payload.user_id || payload.nameid),
        email: payload.email,
        firstName: payload.given_name,
        lastName: payload.family_name,
        role: payload.role as UserRole,
        createdAt: new Date()
      };
      this.currentUserSubject.next(user);
    } catch (error) {
      console.error('Error parsing token:', error);
      this.logout();
    }
  }
}