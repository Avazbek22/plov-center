import { z } from 'zod';

export interface DishResponse {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  description: string | null;
  price: number;
  photoPath: string | null;
  sortOrder: number;
  isVisible: boolean;
  createdUtc: string;
  updatedUtc: string;
}

export interface DishFormData {
  categoryId: string;
  name: string;
  description: string | null;
  price: number;
  photoPath: string | null;
  sortOrder: number;
  isVisible: boolean;
}

export const dishFormSchema = z.object({
  categoryId: z.string().min(1, 'Категория обязательна'),
  name: z.string().min(1, 'Название обязательно').max(200),
  description: z.string().max(2000).nullable(),
  price: z.number().positive('Цена должна быть больше 0'),
  photoPath: z.string().nullable(),
  sortOrder: z.number().int().min(0),
  isVisible: z.boolean(),
});
