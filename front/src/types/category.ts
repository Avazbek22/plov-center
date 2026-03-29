import { z } from 'zod';

export interface CategoryResponse {
  id: string;
  name: string;
  sortOrder: number;
  isVisible: boolean;
  dishCount: number;
  createdUtc: string;
  updatedUtc: string;
}

export interface CategoryFormData {
  name: string;
  sortOrder: number;
  isVisible: boolean;
}

export interface ReorderCategoryItem {
  categoryId: string;
  sortOrder: number;
}

export const categoryFormSchema = z.object({
  name: z.string().min(1, 'Название обязательно').max(200),
  sortOrder: z.number().int().min(0),
  isVisible: z.boolean(),
});
