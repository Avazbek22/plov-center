import { createContext, useContext, useReducer, useEffect, useCallback, type ReactNode } from 'react';
import type { AuthSession, AdminUser, LoginCredentials } from '@/types/auth';
import type { LoginResponse } from '@/types/api';
import { getStoredAuth, setStoredAuth, clearStoredAuth } from '@/api/client';
import { login as apiLogin, getMe } from '@/api/auth';

type AuthAction =
  | { type: 'LOGIN'; payload: { token: string; expiresAtUtc: string; admin: AdminUser } }
  | { type: 'LOGOUT' }
  | { type: 'SET_LOADING'; payload: boolean };

interface AuthState extends AuthSession {
  isLoading: boolean;
}

const initialState: AuthState = {
  isAuthenticated: false,
  token: null,
  expiresAtUtc: null,
  admin: null,
  isLoading: true,
};

function authReducer(state: AuthState, action: AuthAction): AuthState {
  switch (action.type) {
    case 'LOGIN':
      return {
        isAuthenticated: true,
        token: action.payload.token,
        expiresAtUtc: action.payload.expiresAtUtc,
        admin: action.payload.admin,
        isLoading: false,
      };
    case 'LOGOUT':
      return { ...initialState, isLoading: false };
    case 'SET_LOADING':
      return { ...state, isLoading: action.payload };
  }
}

interface AuthContextValue {
  session: AuthState;
  login: (credentials: LoginCredentials) => Promise<LoginResponse>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, dispatch] = useReducer(authReducer, initialState);

  useEffect(() => {
    const stored = getStoredAuth();
    if (!stored?.token) {
      dispatch({ type: 'SET_LOADING', payload: false });
      return;
    }

    getMe()
      .then((admin) => {
        dispatch({
          type: 'LOGIN',
          payload: { token: stored.token, expiresAtUtc: stored.expiresAtUtc, admin },
        });
      })
      .catch(() => {
        clearStoredAuth();
        dispatch({ type: 'LOGOUT' });
      });
  }, []);

  const login = useCallback(async (credentials: LoginCredentials): Promise<LoginResponse> => {
    const response = await apiLogin(credentials);
    setStoredAuth(response.token, response.expiresAtUtc, response.admin);
    dispatch({
      type: 'LOGIN',
      payload: { token: response.token, expiresAtUtc: response.expiresAtUtc, admin: response.admin },
    });
    return response;
  }, []);

  const logout = useCallback(() => {
    clearStoredAuth();
    dispatch({ type: 'LOGOUT' });
  }, []);

  return (
    <AuthContext.Provider value={{ session, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
