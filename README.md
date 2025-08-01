# Event Management System - Angular Frontend

This is the Angular frontend for the Event Management System that works with the C# backend API.

## Features

- **Authentication**: Login/Register with JWT token management
- **Role-based Access**: SuperAdmin, Admin, Organizer, RegisteredUser roles
- **Event Management**: Create, view, edit, delete events (for Organizers/Admins)
- **Ticket Booking**: Book and manage tickets (for RegisteredUsers)
- **Location Management**: Manage venues (for SuperAdmins)
- **Responsive Design**: Modern, clean UI that works on all devices

## Setup Instructions

1. **Install Dependencies**
   ```bash
   npm install
   ```

2. **Update API URLs**
   Update the backend API URLs in the service files:
   - `src/app/services/auth.service.ts`
   - `src/app/services/event.service.ts`
   - `src/app/services/ticket.service.ts`
   - `src/app/services/location.service.ts`
   - `src/app/services/feedback.service.ts`

3. **Run the Application**
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:4200`

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── login/
│   │   ├── register/
│   │   ├── dashboard/
│   │   ├── navbar/
│   │   ├── events/
│   │   │   ├── event-list/
│   │   │   ├── event-form/
│   │   │   └── event-detail/
│   │   ├── tickets/
│   │   │   ├── ticket-booking/
│   │   │   └── my-tickets/
│   │   ├── locations/
│   │   │   └── location-management/
│   │   └── unauthorized/
│   ├── services/
│   ├── guards/
│   ├── interceptors/
│   ├── models/
│   └── ...
```

## Remaining Components to Create

### 1. Navbar Component
**File: `src/app/components/navbar/navbar.component.ts`**
```typescript
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User, UserRole } from '../../models/user.model';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  currentUser: User | null = null;
  UserRole = UserRole;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  hasRole(role: UserRole): boolean {
    return this.authService.hasRole(role);
  }

  hasAnyRole(roles: UserRole[]): boolean {
    return this.authService.hasAnyRole(roles);
  }
}
```

**File: `src/app/components/navbar/navbar.component.html`**
```html
<nav class="navbar">
  <div class="navbar-container">
    <div class="navbar-brand">
      <a routerLink="/dashboard">Event Management</a>
    </div>

    <div class="navbar-menu">
      <a routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
      <a routerLink="/events" routerLinkActive="active">Events</a>
      
      <a *ngIf="hasRole(UserRole.RegisteredUser)" 
         routerLink="/my-tickets" 
         routerLinkActive="active">My Tickets</a>
      
      <a *ngIf="hasRole(UserRole.SuperAdmin)" 
         routerLink="/locations" 
         routerLinkActive="active">Locations</a>
    </div>

    <div class="navbar-user">
      <span class="user-info">
        Welcome, {{ currentUser?.firstName || currentUser?.email }}
        <span class="user-role">({{ currentUser?.role }})</span>
      </span>
      <button class="btn-logout" (click)="logout()">Logout</button>
    </div>
  </div>
</nav>
```

**File: `src/app/components/navbar/navbar.component.css`**
```css
.navbar {
  background: white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  height: 60px;
}

.navbar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  height: 100%;
}

.navbar-brand a {
  font-size: 20px;
  font-weight: 600;
  color: #667eea;
  text-decoration: none;
}

.navbar-menu {
  display: flex;
  gap: 30px;
}

.navbar-menu a {
  color: #666;
  text-decoration: none;
  font-weight: 500;
  padding: 8px 16px;
  border-radius: 6px;
  transition: all 0.2s ease;
}

.navbar-menu a:hover,
.navbar-menu a.active {
  color: #667eea;
  background-color: #f8f9ff;
}

.navbar-user {
  display: flex;
  align-items: center;
  gap: 15px;
}

.user-info {
  color: #333;
  font-size: 14px;
}

.user-role {
  color: #666;
  font-size: 12px;
}

.btn-logout {
  background: #dc3545;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.2s ease;
}

.btn-logout:hover {
  background: #c82333;
}

@media (max-width: 768px) {
  .navbar-container {
    flex-direction: column;
    height: auto;
    padding: 10px 20px;
  }
  
  .navbar-menu {
    order: 3;
    margin-top: 10px;
  }
  
  .user-info {
    display: none;
  }
}
```

### 2. Dashboard Component
**File: `src/app/components/dashboard/dashboard.component.ts`**
```typescript
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { EventService } from '../../services/event.service';
import { TicketService } from '../../services/ticket.service';
import { User, UserRole } from '../../models/user.model';
import { EventResponseDTO } from '../../models/event.model';
import { TicketResponseViewModel } from '../../models/ticket.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  UserRole = UserRole;
  recentEvents: EventResponseDTO[] = [];
  userTickets: TicketResponseViewModel[] = [];
  isLoading = true;

  constructor(
    private authService: AuthService,
    private eventService: EventService,
    private ticketService: TicketService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    
    // Load recent events
    this.eventService.getEvents().subscribe({
      next: (events) => {
        this.recentEvents = events.slice(0, 5); // Show only 5 recent events
      },
      error: (error) => console.error('Error loading events:', error)
    });

    // Load user tickets if user is RegisteredUser
    if (this.currentUser?.role === UserRole.RegisteredUser) {
      this.ticketService.getMyTickets().subscribe({
        next: (tickets) => {
          this.userTickets = tickets.slice(0, 5); // Show only 5 recent tickets
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading tickets:', error);
          this.isLoading = false;
        }
      });
    } else {
      this.isLoading = false;
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
```

**File: `src/app/components/dashboard/dashboard.component.html`**
```html
<div class="dashboard-container">
  <div class="dashboard-header">
    <h1>Dashboard</h1>
    <p>Welcome back, {{ currentUser?.firstName || currentUser?.email }}!</p>
  </div>

  <div class="dashboard-content">
    <!-- Quick Actions -->
    <div class="card">
      <h2>Quick Actions</h2>
      <div class="actions-grid">
        <a routerLink="/events" class="action-card">
          <h3>View Events</h3>
          <p>Browse all available events</p>
        </a>
        
        <a *ngIf="currentUser?.role === UserRole.RegisteredUser" 
           routerLink="/my-tickets" 
           class="action-card">
          <h3>My Tickets</h3>
          <p>View your booked tickets</p>
        </a>
        
        <a *ngIf="currentUser?.role === UserRole.Organizer || currentUser?.role === UserRole.Admin || currentUser?.role === UserRole.SuperAdmin" 
           routerLink="/events/create" 
           class="action-card">
          <h3>Create Event</h3>
          <p>Organize a new event</p>
        </a>
        
        <a *ngIf="currentUser?.role === UserRole.SuperAdmin" 
           routerLink="/locations" 
           class="action-card">
          <h3>Manage Locations</h3>
          <p>Add and manage venues</p>
        </a>
      </div>
    </div>

    <!-- Recent Events -->
    <div class="card">
      <div class="card-header">
        <h2>Recent Events</h2>
        <a routerLink="/events" class="view-all">View All</a>
      </div>
      
      <div *ngIf="isLoading" class="loading">Loading...</div>
      
      <div *ngIf="!isLoading && recentEvents.length === 0" class="no-data">
        No events available
      </div>
      
      <div *ngIf="!isLoading && recentEvents.length > 0" class="events-list">
        <div *ngFor="let event of recentEvents" class="event-item">
          <div class="event-info">
            <h4>{{ event.title }}</h4>
            <p>{{ formatDate(event.startDate) }} - {{ event.location.name }}</p>
          </div>
          <a [routerLink]="['/events', event.id]" class="btn-secondary">View</a>
        </div>
      </div>
    </div>

    <!-- My Tickets (for RegisteredUser) -->
    <div *ngIf="currentUser?.role === UserRole.RegisteredUser" class="card">
      <div class="card-header">
        <h2>My Recent Tickets</h2>
        <a routerLink="/my-tickets" class="view-all">View All</a>
      </div>
      
      <div *ngIf="userTickets.length === 0" class="no-data">
        No tickets booked yet
      </div>
      
      <div *ngIf="userTickets.length > 0" class="tickets-list">
        <div *ngFor="let ticket of userTickets" class="ticket-item">
          <div class="ticket-info">
            <h4>{{ ticket.eventName }}</h4>
            <p>Booked: {{ formatDate(ticket.bookingDate.toString()) }}</p>
            <span class="status" [class]="ticket.status.toLowerCase()">{{ ticket.status }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
```

**File: `src/app/components/dashboard/dashboard.component.css`**
```css
.dashboard-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.dashboard-header {
  margin-bottom: 30px;
}

.dashboard-header h1 {
  color: #333;
  margin-bottom: 8px;
}

.dashboard-header p {
  color: #666;
  font-size: 16px;
}

.dashboard-content {
  display: grid;
  gap: 24px;
}

.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  padding: 24px;
}

.card h2 {
  color: #333;
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.view-all {
  color: #667eea;
  text-decoration: none;
  font-weight: 500;
}

.view-all:hover {
  text-decoration: underline;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.action-card {
  background: #f8f9ff;
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  padding: 20px;
  text-decoration: none;
  color: #333;
  transition: all 0.2s ease;
}

.action-card:hover {
  border-color: #667eea;
  transform: translateY(-2px);
}

.action-card h3 {
  color: #667eea;
  margin-bottom: 8px;
}

.action-card p {
  color: #666;
  font-size: 14px;
}

.events-list, .tickets-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.event-item, .ticket-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.event-info h4, .ticket-info h4 {
  color: #333;
  margin-bottom: 4px;
}

.event-info p, .ticket-info p {
  color: #666;
  font-size: 14px;
}

.status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.status.confirmed {
  background: #d5f4e6;
  color: #27ae60;
}

.status.cancelled {
  background: #fdf2f2;
  color: #e74c3c;
}

.status.payment_pending {
  background: #fff3cd;
  color: #856404;
}

.loading, .no-data {
  text-align: center;
  color: #666;
  padding: 20px;
}

@media (max-width: 768px) {
  .actions-grid {
    grid-template-columns: 1fr;
  }
  
  .event-item, .ticket-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
```

### 3. Additional Required Components

You'll also need to create these components (I can provide the code for any of these if needed):

1. **Event Detail Component** (`src/app/components/events/event-detail/`)
2. **Event Form Component HTML/CSS** (complete the form component)
3. **Ticket Booking Component** (`src/app/components/tickets/ticket-booking/`)
4. **My Tickets Component** (`src/app/components/tickets/my-tickets/`)
5. **Location Management Component** (`src/app/components/locations/location-management/`)
6. **Unauthorized Component** (`src/app/components/unauthorized/`)

## API Configuration

Make sure to update the API base URLs in all service files to match your backend:

```typescript
private apiUrl = 'https://localhost:7297/api/[Controller]';
```

## User Roles & Permissions

- **RegisteredUser**: Can view events, book tickets, manage their tickets
- **Organizer**: Can create, edit, delete events + RegisteredUser permissions
- **Admin**: Can manage events, users + Organizer permissions  
- **SuperAdmin**: Can manage locations, create admins/organizers + Admin permissions

## Technologies Used

- Angular 17
- TypeScript
- RxJS
- Angular Router
- Angular Reactive Forms
- CSS Grid & Flexbox

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request