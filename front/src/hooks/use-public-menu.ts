import { useQuery } from '@tanstack/react-query';

import { getPublicMenu, getPublicContent } from '@/api/public';

export function usePublicMenu() {
  return useQuery({
    queryKey: ['public-menu'] as const,
    queryFn: getPublicMenu,
  });
}

export function usePublicContent() {
  return useQuery({
    queryKey: ['public-content'] as const,
    queryFn: getPublicContent,
  });
}
