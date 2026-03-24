import type { AdminUser } from './auth';

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  admin: AdminUser;
}

export interface ApiErrorResponse {
  code: string;
  message: string;
  traceId: string;
  errors: Record<string, string[]> | null;
}
