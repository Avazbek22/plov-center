import type { LoginCredentials, AdminUser } from '@/types/auth';
import type { LoginResponse } from '@/types/api';
import { apiFetch } from './client';

export function login(credentials: LoginCredentials): Promise<LoginResponse> {
  return apiFetch<LoginResponse>('/api/admin/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials),
  });
}

export function getMe(): Promise<AdminUser> {
  return apiFetch<AdminUser>('/api/admin/auth/me');
}
