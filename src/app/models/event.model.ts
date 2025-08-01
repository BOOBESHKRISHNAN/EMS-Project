export interface Event {
  id: number;
  title: string;
  description?: string;
  startDate: Date;
  endDate: Date;
  locationId: number;
  organizerId: number;
  ticketPrice: number;
  location?: Location;
  organizer?: string;
}

export interface EventResponseDTO {
  id: number;
  title: string;
  description?: string;
  startDate: string;
  endDate: string;
  locationId: number;
  organizerName: string;
  location: EventWithLocationsDTO;
}

export interface EventCreateDTO {
  title: string;
  description?: string;
  startDate: Date;
  endDate: Date;
  locationId: number;
  ticketPrice: number;
}

export interface EventWithLocationsDTO {
  name: string;
  address: string;
}

export interface EventPreviewDTO {
  id: number;
  title: string;
  startDate: string;
  endDate: string;
  locationName: string;
}

export interface Location {
  id: number;
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode?: string;
  venueFee: number;
}

export interface LocationCreateDto {
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode?: string;
  venueFee: number;
}

export interface LocationUpdateDto {
  name: string;
  venueFee: number;
}

export interface LocationWithEventsDto {
  id: number;
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode?: string;
  venueFee: number;
  events: EventPreviewDTO[];
}