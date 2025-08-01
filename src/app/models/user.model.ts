export enum UserRole {
  SuperAdmin = 'SuperAdmin',
  Admin = 'Admin',
  Organizer = 'Organizer',
  RegisteredUser = 'RegisteredUser'
}

export interface User {
  id: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  contactNumber?: string;
  location?: string;
  role: UserRole;
  createdAt: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface RegisterUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  contactNumber: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterAdminRequest {
  firstName: string;
  lastName: string;
  email: string;
  contactNumber: string;
  password: string;
  confirmPassword: string;
  location: string;
}

export interface RegisterOrganizerRequest {
  firstName: string;
  lastName: string;
  email: string;
  contactNumber: string;
  password: string;
  confirmPassword: string;
}