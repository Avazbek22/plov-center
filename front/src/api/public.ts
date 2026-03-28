import type { PublicMenu, PublicSiteContent } from '@/types/public';
import { apiFetch } from './client';

export function getPublicMenu(): Promise<PublicMenu> {
  return apiFetch<PublicMenu>('/api/public/menu');
}

export function getPublicContent(): Promise<PublicSiteContent> {
  return apiFetch<PublicSiteContent>('/api/public/content');
}
