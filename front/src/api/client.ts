import type { ApiErrorResponse } from '@/types/api';

const AUTH_STORAGE_KEY = 'plov-center-auth';

export class ApiError extends Error {
  readonly status: number;
  readonly response: ApiErrorResponse;

  constructor(status: number, response: ApiErrorResponse) {
    super(response.message);
    this.name = 'ApiError';
    this.status = status;
    this.response = response;
  }
}

export function getStoredToken(): string | null {
  try {
    const raw = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as { token?: string };
    return parsed.token ?? null;
  } catch {
    return null;
  }
}

export function setStoredAuth(token: string, expiresAtUtc: string, admin: unknown): void {
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify({ token, expiresAtUtc, admin }));
}

export function clearStoredAuth(): void {
  localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getStoredAuth(): { token: string; expiresAtUtc: string; admin: unknown } | null {
  try {
    const raw = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) return null;
    return JSON.parse(raw) as { token: string; expiresAtUtc: string; admin: unknown };
  } catch {
    return null;
  }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = getStoredToken();

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((options?.headers as Record<string, string>) ?? {}),
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(path, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorBody: ApiErrorResponse;
    try {
      errorBody = (await response.json()) as ApiErrorResponse;
    } catch {
      errorBody = {
        code: 'network_error',
        message: 'Failed to connect to server.',
        traceId: '',
        errors: null,
      };
    }

    if (response.status === 401 && token) {
      clearStoredAuth();
      window.location.href = '/admin/login';
    }

    throw new ApiError(response.status, errorBody);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
