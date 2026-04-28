import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { enqueueSnackbar } from 'notistack';

import type { DishWritePayload } from '@/types/dish';
import { ApiError } from '@/api/client';
import {
  getDishes,
  createDish,
  updateDish,
  setDishVisibility,
  deleteDish,
} from '@/api/dishes';

function handleError(error: Error) {
  const message =
    error instanceof ApiError ? error.response.message : 'Произошла ошибка';
  enqueueSnackbar(message, { variant: 'error' });
}

export function useDishesQuery(categoryId?: string) {
  return useQuery({
    queryKey: ['dishes', { categoryId }],
    queryFn: () => getDishes(categoryId),
  });
}

export function useCreateDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createDish,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо создано', { variant: 'success' });
    },
    onError: handleError,
  });
}

export function useUpdateDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; data: DishWritePayload }) =>
      updateDish(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо обновлено', { variant: 'success' });
    },
    onError: handleError,
  });
}

export function useSetDishVisibility() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; isVisible: boolean }) =>
      setDishVisibility(params.id, params.isVisible),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
    },
    onError: handleError,
  });
}

export function useDeleteDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteDish,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо удалено', { variant: 'success' });
    },
    onError: handleError,
  });
}
