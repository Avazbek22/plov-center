import type { AdminSiteContentResponse, AboutFormData, ContactsFormData } from '@/types/content';
import { apiFetch } from './client';

export function getSiteContent(): Promise<AdminSiteContentResponse> {
  return apiFetch<AdminSiteContentResponse>('/api/admin/content');
}

export function updateAboutContent(data: AboutFormData): Promise<AdminSiteContentResponse> {
  return apiFetch<AdminSiteContentResponse>('/api/admin/content/about', {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export function updateContactsContent(data: ContactsFormData): Promise<AdminSiteContentResponse> {
  return apiFetch<AdminSiteContentResponse>('/api/admin/content/contacts', {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}
