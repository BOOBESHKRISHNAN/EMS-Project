import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { LocationService } from '../../../services/location.service';
import { EventCreateDTO, EventResponseDTO, LocationWithEventsDto } from '../../../models/event.model';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit {
  eventForm: FormGroup;
  locations: LocationWithEventsDto[] = [];
  isEditMode = false;
  eventId: number | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private eventService: EventService,
    private locationService: LocationService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.eventForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      locationId: ['', Validators.required],
      ticketPrice: ['', [Validators.required, Validators.min(0)]]
    }, { validators: this.dateValidator });
  }

  ngOnInit(): void {
    this.loadLocations();
    
    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.eventId = +params['id'];
        this.loadEvent();
      }
    });
  }

  dateValidator(form: FormGroup) {
    const startDate = form.get('startDate')?.value;
    const endDate = form.get('endDate')?.value;
    
    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      const now = new Date();
      
      if (start < now) {
        return { startDatePast: true };
      }
      
      if (end <= start) {
        return { endDateBeforeStart: true };
      }
    }
    
    return null;
  }

  loadLocations(): void {
    this.locationService.getLocations().subscribe({
      next: (locations) => {
        this.locations = locations;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load locations. Please try again.';
        console.error('Error loading locations:', error);
      }
    });
  }

  loadEvent(): void {
    if (this.eventId) {
      this.eventService.getEventById(this.eventId).subscribe({
        next: (event) => {
          this.populateForm(event);
        },
        error: (error) => {
          this.errorMessage = 'Failed to load event. Please try again.';
          console.error('Error loading event:', error);
        }
      });
    }
  }

  populateForm(event: EventResponseDTO): void {
    // Convert date strings to the format required by datetime-local input
    const startDate = new Date(event.startDate).toISOString().slice(0, 16);
    const endDate = new Date(event.endDate).toISOString().slice(0, 16);
    
    this.eventForm.patchValue({
      title: event.title,
      description: event.description,
      startDate: startDate,
      endDate: endDate,
      locationId: event.locationId
    });
  }

  onSubmit(): void {
    if (this.eventForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const eventData: EventCreateDTO = {
        title: this.eventForm.value.title,
        description: this.eventForm.value.description,
        startDate: new Date(this.eventForm.value.startDate),
        endDate: new Date(this.eventForm.value.endDate),
        locationId: +this.eventForm.value.locationId,
        ticketPrice: +this.eventForm.value.ticketPrice
      };

      const operation = this.isEditMode && this.eventId
        ? this.eventService.updateEvent(this.eventId, eventData)
        : this.eventService.createEvent(eventData);

      operation.subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage = this.isEditMode 
            ? 'Event updated successfully!' 
            : 'Event created successfully!';
          
          setTimeout(() => {
            this.router.navigate(['/events']);
          }, 2000);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 
            (this.isEditMode ? 'Failed to update event.' : 'Failed to create event.') + 
            ' Please try again.';
          console.error('Error saving event:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.eventForm.controls).forEach(key => {
      const control = this.eventForm.get(key);
      control?.markAsTouched();
    });
  }

  get title() { return this.eventForm.get('title'); }
  get description() { return this.eventForm.get('description'); }
  get startDate() { return this.eventForm.get('startDate'); }
  get endDate() { return this.eventForm.get('endDate'); }
  get locationId() { return this.eventForm.get('locationId'); }
  get ticketPrice() { return this.eventForm.get('ticketPrice'); }
}