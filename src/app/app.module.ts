import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Interceptors
import { AuthInterceptor } from './interceptors/auth.interceptor';

// Components
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { EventListComponent } from './components/events/event-list/event-list.component';
import { EventFormComponent } from './components/events/event-form/event-form.component';
import { EventDetailComponent } from './components/events/event-detail/event-detail.component';
import { TicketBookingComponent } from './components/tickets/ticket-booking/ticket-booking.component';
import { MyTicketsComponent } from './components/tickets/my-tickets/my-tickets.component';
import { LocationManagementComponent } from './components/locations/location-management/location-management.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    DashboardComponent,
    NavbarComponent,
    EventListComponent,
    EventFormComponent,
    EventDetailComponent,
    TicketBookingComponent,
    MyTicketsComponent,
    LocationManagementComponent,
    UnauthorizedComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }