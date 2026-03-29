import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { enqueueSnackbar } from 'notistack';

import {
  createCategory,
  deleteCategory,
  getCategories,
  reorderCategories,
  setCategoryVisibility,
  updateCategory,
} from '@/api/categories';
import { ApiError } from '@/api/client';
import type { CategoryFormData, ReorderCategoryItem } from '@/types/category';

const CATEGORIES_KEY = ['categories'] as const;

function handleMutationError(error: Error) {
  const message =
    error instanceof ApiError
      ? error.response.message
      : 'Произошла ошибка';
  enqueueSnackbar(message, { variant: 'error' });
}

export function useCategoriesQuery() {
  return useQuery({
    queryKey: CATEGORIES_KEY,
    queryFn: getCategories,
  });
}

export function useCreateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIES_KEY });
      enqueueSnackbar('Категория создана', { variant: 'success' });
    },
    onError: handleMutationError,
  });
}

export function useUpdateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; data: CategoryFormData }) =>
      updateCategory(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIES_KEY });
      enqueueSnackbar('Категория обновлена', { variant: 'success' });
    },
    onError: handleMutationError,
  });
}

export function useSetCategoryVisibility() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; isVisible: boolean }) =>
      setCategoryVisibility(params.id, params.isVisible),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIES_KEY });
    },
    onError: handleMutationError,
  });
}

export function useReorderCategories() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (items: ReorderCategoryItem[]) => reorderCategories(items),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIES_KEY });
    },
    onError: handleMutationError,
  });
}

export function useDeleteCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIES_KEY });
      enqueueSnackbar('Категория удалена', { variant: 'success' });
    },
    onError: handleMutationError,
  });
}
