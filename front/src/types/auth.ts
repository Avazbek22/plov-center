export interface AdminUser {
  id: string;
  username: string;
  isActive: boolean;
}

export interface AuthSession {
  isAuthenticated: boolean;
  token: string | null;
  expiresAtUtc: string | null;
  admin: AdminUser | null;
}

export interface LoginCredentials {
  username: string;
  password: string;
}
