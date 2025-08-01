import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LocationService } from '../../../services/location.service';
import { LocationWithEventsDto, LocationCreateDto, LocationUpdateDto } from '../../../models/event.model';

@Component({
  selector: 'app-location-management',
  templateUrl: './location-management.component.html',
  styleUrls: ['./location-management.component.css']
})
export class LocationManagementComponent implements OnInit {
  locations: LocationWithEventsDto[] = [];
  filteredLocations: LocationWithEventsDto[] = [];
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  
  // Form states
  showCreateForm = false;
  showEditForm = false;
  editingLocationId: number | null = null;
  
  // Forms
  createForm: FormGroup;
  editForm: FormGroup;
  
  // Search and filter
  searchTerm = '';
  sortBy: 'name' | 'city' | 'venueFee' | 'events' = 'name';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(
    private locationService: LocationService,
    private formBuilder: FormBuilder
  ) {
    this.createForm = this.initializeCreateForm();
    this.editForm = this.initializeEditForm();
  }

  ngOnInit(): void {
    this.loadLocations();
  }

  private initializeCreateForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      address: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      state: ['', [Validators.required, Validators.minLength(2)]],
      zipCode: [''],
      venueFee: [0, [Validators.required, Validators.min(0)]]
    });
  }

  private initializeEditForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      venueFee: [0, [Validators.required, Validators.min(0)]]
    });
  }

  loadLocations(): void {
    this.loading = true;
    this.error = null;
    
    this.locationService.getLocations().subscribe({
      next: (locations) => {
        this.locations = locations;
        this.applyFilters();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading locations:', error);
        this.error = 'Failed to load locations. Please try again.';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.locations];

    // Apply search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(location =>
        location.name.toLowerCase().includes(term) ||
        location.city.toLowerCase().includes(term) ||
        location.state.toLowerCase().includes(term) ||
        location.address.toLowerCase().includes(term)
      );
    }

    // Apply sorting
    filtered.sort((a, b) => {
      let aValue: any, bValue: any;
      
      switch (this.sortBy) {
        case 'name':
          aValue = a.name.toLowerCase();
          bValue = b.name.toLowerCase();
          break;
        case 'city':
          aValue = a.city.toLowerCase();
          bValue = b.city.toLowerCase();
          break;
        case 'venueFee':
          aValue = a.venueFee;
          bValue = b.venueFee;
          break;
        case 'events':
          aValue = a.events.length;
          bValue = b.events.length;
          break;
        default:
          return 0;
      }

      if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    this.filteredLocations = filtered;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onSortChange(sortBy: 'name' | 'city' | 'venueFee' | 'events'): void {
    if (this.sortBy === sortBy) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = sortBy;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }

  showCreateLocationForm(): void {
    this.showCreateForm = true;
    this.showEditForm = false;
    this.createForm.reset();
    this.clearMessages();
  }

  hideCreateForm(): void {
    this.showCreateForm = false;
    this.createForm.reset();
  }

  showEditLocationForm(location: LocationWithEventsDto): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.editingLocationId = location.id;
    
    this.editForm.patchValue({
      name: location.name,
      venueFee: location.venueFee
    });
    this.clearMessages();
  }

  hideEditForm(): void {
    this.showEditForm = false;
    this.editingLocationId = null;
    this.editForm.reset();
  }

  createLocation(): void {
    if (this.createForm.valid) {
      this.loading = true;
      this.error = null;
      
      const locationData: LocationCreateDto = this.createForm.value;
      
      this.locationService.createLocation(locationData).subscribe({
        next: (newLocation) => {
          this.successMessage = 'Location created successfully!';
          this.hideCreateForm();
          this.loadLocations();
          this.loading = false;
          this.clearMessagesAfterDelay();
        },
        error: (error) => {
          console.error('Error creating location:', error);
          this.error = 'Failed to create location. Please try again.';
          this.loading = false;
        }
      });
    } else {
      this.markFormGroupTouched(this.createForm);
    }
  }

  updateLocation(): void {
    if (this.editForm.valid && this.editingLocationId) {
      this.loading = true;
      this.error = null;
      
      const locationData: LocationUpdateDto = this.editForm.value;
      
      this.locationService.updateLocation(this.editingLocationId, locationData).subscribe({
        next: (updatedLocation) => {
          this.successMessage = 'Location updated successfully!';
          this.hideEditForm();
          this.loadLocations();
          this.loading = false;
          this.clearMessagesAfterDelay();
        },
        error: (error) => {
          console.error('Error updating location:', error);
          this.error = 'Failed to update location. Please try again.';
          this.loading = false;
        }
      });
    } else {
      this.markFormGroupTouched(this.editForm);
    }
  }

  deleteLocation(location: LocationWithEventsDto): void {
    if (location.events.length > 0) {
      this.error = `Cannot delete location "${location.name}" because it has ${location.events.length} associated event(s).`;
      this.clearMessagesAfterDelay();
      return;
    }

    if (confirm(`Are you sure you want to delete the location "${location.name}"? This action cannot be undone.`)) {
      this.loading = true;
      this.error = null;
      
      this.locationService.deleteLocation(location.id).subscribe({
        next: () => {
          this.successMessage = `Location "${location.name}" deleted successfully!`;
          this.loadLocations();
          this.loading = false;
          this.clearMessagesAfterDelay();
        },
        error: (error) => {
          console.error('Error deleting location:', error);
          this.error = 'Failed to delete location. Please try again.';
          this.loading = false;
        }
      });
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  private clearMessages(): void {
    this.error = null;
    this.successMessage = null;
  }

  private clearMessagesAfterDelay(): void {
    setTimeout(() => {
      this.clearMessages();
    }, 5000);
  }

  // Helper methods for template
  getFieldError(form: FormGroup, fieldName: string): string {
    const field = form.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required.`;
      }
      if (field.errors['minlength']) {
        return `${this.getFieldDisplayName(fieldName)} must be at least ${field.errors['minlength'].requiredLength} characters.`;
      }
      if (field.errors['min']) {
        return `${this.getFieldDisplayName(fieldName)} must be greater than or equal to ${field.errors['min'].min}.`;
      }
    }
    return '';
  }

  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      name: 'Location Name',
      address: 'Address',
      city: 'City',
      state: 'State',
      zipCode: 'ZIP Code',
      venueFee: 'Venue Fee'
    };
    return displayNames[fieldName] || fieldName;
  }

  isFieldInvalid(form: FormGroup, fieldName: string): boolean {
    const field = form.get(fieldName);
    return !!(field && field.errors && field.touched);
  }

  getSortIcon(column: string): string {
    if (this.sortBy !== column) return 'fas fa-sort';
    return this.sortDirection === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
  }

  canDeleteLocation(location: LocationWithEventsDto): boolean {
    return location.events.length === 0;
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }
}