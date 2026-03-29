import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { enqueueSnackbar } from 'notistack';
import { getSiteContent, updateAboutContent, updateContactsContent } from '@/api/content';
import { ApiError } from '@/api/client';
import type { AboutFormData, ContactsFormData } from '@/types/content';

const CONTENT_KEY = ['content'] as const;

function handleError(error: Error): void {
  const message = error instanceof ApiError ? error.response.message : 'Произошла ошибка';
  enqueueSnackbar(message, { variant: 'error' });
}

export function useContentQuery() {
  return useQuery({
    queryKey: CONTENT_KEY,
    queryFn: getSiteContent,
  });
}

export function useUpdateAbout() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: AboutFormData) => updateAboutContent(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTENT_KEY });
      enqueueSnackbar('Раздел "О нас" обновлён', { variant: 'success' });
    },
    onError: handleError,
  });
}

export function useUpdateContacts() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: ContactsFormData) => updateContactsContent(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTENT_KEY });
      enqueueSnackbar('Контакты обновлены', { variant: 'success' });
    },
    onError: handleError,
  });
}
