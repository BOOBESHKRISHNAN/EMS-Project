import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { UserRole } from './models/user.model';

import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { EventListComponent } from './components/events/event-list/event-list.component';
import { EventFormComponent } from './components/events/event-form/event-form.component';
import { EventDetailComponent } from './components/events/event-detail/event-detail.component';
import { TicketBookingComponent } from './components/tickets/ticket-booking/ticket-booking.component';
import { MyTicketsComponent } from './components/tickets/my-tickets/my-tickets.component';
import { LocationManagementComponent } from './components/locations/location-management/location-management.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  
  // Protected routes
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  
  // Event routes
  {
    path: 'events',
    component: EventListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'events/create',
    component: EventFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Organizer, UserRole.Admin, UserRole.SuperAdmin] }
  },
  {
    path: 'events/:id',
    component: EventDetailComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'events/:id/edit',
    component: EventFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Organizer, UserRole.Admin, UserRole.SuperAdmin] }
  },
  {
    path: 'events/:id/book',
    component: TicketBookingComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.RegisteredUser] }
  },
  
  // Ticket routes
  {
    path: 'my-tickets',
    component: MyTicketsComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.RegisteredUser] }
  },
  
  // Location management (SuperAdmin only)
  {
    path: 'locations',
    component: LocationManagementComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.SuperAdmin] }
  },
  
  // Catch-all route
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }