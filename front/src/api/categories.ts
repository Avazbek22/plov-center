import type { CategoryResponse, CategoryFormData, ReorderCategoryItem } from '@/types/category';
import { apiFetch } from './client';

export function getCategories(): Promise<CategoryResponse[]> {
  return apiFetch<CategoryResponse[]>('/api/admin/categories');
}

export function getCategory(id: string): Promise<CategoryResponse> {
  return apiFetch<CategoryResponse>(`/api/admin/categories/${id}`);
}

export function createCategory(data: CategoryFormData): Promise<CategoryResponse> {
  return apiFetch<CategoryResponse>('/api/admin/categories', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function updateCategory(id: string, data: CategoryFormData): Promise<CategoryResponse> {
  return apiFetch<CategoryResponse>(`/api/admin/categories/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export function setCategoryVisibility(id: string, isVisible: boolean): Promise<CategoryResponse> {
  return apiFetch<CategoryResponse>(`/api/admin/categories/${id}/visibility`, {
    method: 'PATCH',
    body: JSON.stringify({ isVisible }),
  });
}

export function reorderCategories(items: ReorderCategoryItem[]): Promise<void> {
  return apiFetch<void>('/api/admin/categories/reorder', {
    method: 'PUT',
    body: JSON.stringify({ items }),
  });
}

export function deleteCategory(id: string): Promise<void> {
  return apiFetch<void>(`/api/admin/categories/${id}`, {
    method: 'DELETE',
  });
}
