import type { DishResponse, DishFormData } from '@/types/dish';
import { apiFetch } from './client';

export function getDishes(categoryId?: string): Promise<DishResponse[]> {
  const url = categoryId
    ? `/api/admin/dishes?categoryId=${encodeURIComponent(categoryId)}`
    : '/api/admin/dishes';
  return apiFetch<DishResponse[]>(url);
}

export function getDish(id: string): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}`);
}

export function createDish(data: DishFormData): Promise<DishResponse> {
  return apiFetch<DishResponse>('/api/admin/dishes', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function updateDish(id: string, data: DishFormData): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export function setDishVisibility(id: string, isVisible: boolean): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}/visibility`, {
    method: 'PATCH',
    body: JSON.stringify({ isVisible }),
  });
}

export function deleteDish(id: string): Promise<void> {
  return apiFetch<void>(`/api/admin/dishes/${id}`, {
    method: 'DELETE',
  });
}
