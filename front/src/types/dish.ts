import { z } from 'zod';

export interface DishPhotoResponse {
  id: string;
  relativePath: string;
  sortOrder: number;
}

export interface DishResponse {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishPhotoResponse[];
  sortOrder: number;
  isVisible: boolean;
  createdUtc: string;
  updatedUtc: string;
}

export interface DishPhotoForm {
  tempId: string;
  relativePath: string | null;
  sortOrder: number;
  uploading: boolean;
}

export interface DishFormData {
  categoryId: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishPhotoForm[];
  sortOrder: number;
  isVisible: boolean;
}

export interface DishWritePhoto {
  relativePath: string;
  sortOrder: number;
}

export interface DishWritePayload {
  categoryId: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishWritePhoto[];
  sortOrder: number;
  isVisible: boolean;
}

const dishPhotoFormSchema = z.object({
  tempId: z.string().min(1),
  relativePath: z.string().nullable(),
  sortOrder: z.number().int().min(0),
  uploading: z.boolean(),
});

export const dishFormSchema = z.object({
  categoryId: z.string().min(1, 'Категория обязательна'),
  name: z.string().min(1, 'Название обязательно').max(200),
  description: z.string().max(2000).nullable(),
  price: z.number().positive('Цена должна быть больше 0'),
  photos: z.array(dishPhotoFormSchema).max(50, 'Не больше 50 фото'),
  sortOrder: z.number().int().min(0),
  isVisible: z.boolean(),
});
