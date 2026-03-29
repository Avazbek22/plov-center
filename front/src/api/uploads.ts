import { ApiError, getStoredToken, clearStoredAuth } from '@/api/client';
import type { ApiErrorResponse } from '@/types/api';
import type { UploadImageResponse } from '@/types/content';

const AREA_MAP: Record<'dish' | 'about', string> = {
  dish: '1',
  about: '2',
};

export async function uploadImage(file: File, area: 'dish' | 'about'): Promise<UploadImageResponse> {
  const token = getStoredToken();

  const formData = new FormData();
  formData.append('Area', AREA_MAP[area]);
  formData.append('File', file);

  const headers: Record<string, string> = {};
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch('/api/admin/uploads/image', {
    method: 'POST',
    headers,
    body: formData,
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

  return (await response.json()) as UploadImageResponse;
}
