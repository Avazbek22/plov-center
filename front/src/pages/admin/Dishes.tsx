import { useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { imageUrl } from '@/utils/image-url';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import CircularProgress from '@mui/material/CircularProgress';
import Container from '@mui/material/Container';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';
import IconButton from '@mui/material/IconButton';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import Switch from '@mui/material/Switch';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import ImageIcon from '@mui/icons-material/Image';

import {
  useDishesQuery,
  useCreateDish,
  useUpdateDish,
  useSetDishVisibility,
  useDeleteDish,
} from '@/hooks/use-dishes';
import { useCategoriesQuery } from '@/hooks/use-categories';
import { dishFormSchema } from '@/types/dish';
import type { DishResponse, DishFormData, DishWritePayload } from '@/types/dish';
import ConfirmDialog from '@/components/shared/ConfirmDialog';
import DishGalleryEditor from '@/components/shared/DishGalleryEditor';

const DEFAULT_VALUES: DishFormData = {
  categoryId: '',
  name: '',
  description: null,
  price: 0,
  photos: [],
  sortOrder: 0,
  isVisible: true,
};

export default function Dishes() {
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingDish, setEditingDish] = useState<DishResponse | null>(null);
  const [deletingDish, setDeletingDish] = useState<DishResponse | null>(null);

  const dishesQuery = useDishesQuery(selectedCategoryId || undefined);
  const categoriesQuery = useCategoriesQuery();
  const createDish = useCreateDish();
  const updateDish = useUpdateDish();
  const setVisibility = useSetDishVisibility();
  const deleteDish = useDeleteDish();

  const categories = categoriesQuery.data ?? [];
  const dishes = dishesQuery.data ?? [];

  const form = useForm<DishFormData>({
    resolver: zodResolver(dishFormSchema),
    defaultValues: DEFAULT_VALUES,
  });

  function openCreateDialog() {
    setEditingDish(null);
    form.reset(DEFAULT_VALUES);
    setDialogOpen(true);
  }

  function openEditDialog(dish: DishResponse) {
    setEditingDish(dish);
    form.reset({
      categoryId: dish.categoryId,
      name: dish.name,
      description: dish.description,
      price: dish.price,
      photos: dish.photos.map((p) => ({
        tempId: `srv-${p.id}`,
        relativePath: p.relativePath,
        sortOrder: p.sortOrder,
        uploading: false,
      })),
      sortOrder: dish.sortOrder,
      isVisible: dish.isVisible,
    });
    setDialogOpen(true);
  }

  function closeDialog() {
    setDialogOpen(false);
    setEditingDish(null);
  }

  async function onSubmit(data: DishFormData) {
    if (data.photos.some((p) => p.uploading || !p.relativePath)) {
      return;
    }
    const payload: DishWritePayload = {
      categoryId: data.categoryId,
      name: data.name,
      description: data.description,
      price: data.price,
      photos: data.photos.map((p, index) => ({
        relativePath: p.relativePath as string,
        sortOrder: index,
      })),
      sortOrder: data.sortOrder,
      isVisible: data.isVisible,
    };
    if (editingDish) {
      await updateDish.mutateAsync({ id: editingDish.id, data: payload });
    } else {
      await createDish.mutateAsync(payload);
    }
    closeDialog();
  }

  async function handleDelete() {
    if (!deletingDish) return;
    await deleteDish.mutateAsync(deletingDish.id);
    setDeletingDish(null);
  }

  const photosUploading = form.watch('photos')?.some((p) => p.uploading) ?? false;
  const submitting = createDish.isPending || updateDish.isPending;

  return (
    <Container maxWidth="lg" sx={{ mt: 2 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <Box sx={{ flexGrow: 1 }}>
          <Typography variant="h4">
            Блюда
          </Typography>
          <Box sx={{ width: 40, height: 2, bgcolor: 'primary.main', borderRadius: 1, mt: 1 }} />
        </Box>

        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel id="category-filter-label">Категория</InputLabel>
          <Select
            labelId="category-filter-label"
            label="Категория"
            value={selectedCategoryId}
            onChange={(e) => setSelectedCategoryId(e.target.value)}
          >
            <MenuItem value="">Все категории</MenuItem>
            {categories.map((cat) => (
              <MenuItem key={cat.id} value={cat.id}>
                {cat.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreateDialog}>
          Добавить блюдо
        </Button>
      </Box>

      {dishesQuery.isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
          <CircularProgress />
        </Box>
      ) : (
        <TableContainer component={Paper} variant="outlined">
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Фото</TableCell>
                <TableCell>Название</TableCell>
                <TableCell>Категория</TableCell>
                <TableCell>Цена</TableCell>
                <TableCell>Видимость</TableCell>
                <TableCell align="right">Действия</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {dishes.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                    <Typography color="text.secondary">Блюд пока нет</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                dishes.map((dish) => (
                  <TableRow key={dish.id}>
                    <TableCell>
                      {dish.photos.length > 0 ? (
                        <Box sx={{ position: 'relative', width: 40, height: 40 }}>
                          <Box
                            component="img"
                            src={imageUrl(dish.photos[0].relativePath)!}
                            alt={dish.name}
                            sx={{
                              width: 40,
                              height: 40,
                              objectFit: 'cover',
                              borderRadius: 0.5,
                            }}
                          />
                          {dish.photos.length > 1 && (
                            <Box sx={{
                              position: 'absolute',
                              top: -4, right: -4,
                              bgcolor: 'rgba(0,0,0,0.7)',
                              color: 'white',
                              fontSize: 10,
                              fontWeight: 600,
                              borderRadius: 8,
                              minWidth: 18, height: 18,
                              px: 0.5,
                              display: 'flex', alignItems: 'center', justifyContent: 'center',
                            }}>
                              +{dish.photos.length - 1}
                            </Box>
                          )}
                        </Box>
                      ) : (
                        <Box
                          sx={{
                            width: 40,
                            height: 40,
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            bgcolor: '#F5ECD7',
                            borderRadius: 0.5,
                          }}
                        >
                          <ImageIcon color="disabled" fontSize="small" />
                        </Box>
                      )}
                    </TableCell>
                    <TableCell>{dish.name}</TableCell>
                    <TableCell>{dish.categoryName}</TableCell>
                    <TableCell>{dish.price.toLocaleString('ru-RU')} сум</TableCell>
                    <TableCell>
                      <Switch
                        checked={dish.isVisible}
                        onChange={(_, checked) =>
                          setVisibility.mutate({ id: dish.id, isVisible: checked })
                        }
                      />
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => openEditDialog(dish)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => setDeletingDish(dish)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Create / Edit Dialog */}
      <Dialog open={dialogOpen} onClose={closeDialog} fullWidth maxWidth="sm">
        <form onSubmit={form.handleSubmit(onSubmit)}>
          <DialogTitle>{editingDish ? 'Редактировать блюдо' : 'Новое блюдо'}</DialogTitle>
          <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
            <Controller
              name="categoryId"
              control={form.control}
              render={({ field, fieldState }) => (
                <FormControl fullWidth error={!!fieldState.error}>
                  <InputLabel id="dish-category-label">Категория</InputLabel>
                  <Select
                    {...field}
                    labelId="dish-category-label"
                    label="Категория"
                  >
                    {categories.map((cat) => (
                      <MenuItem key={cat.id} value={cat.id}>
                        {cat.name}
                      </MenuItem>
                    ))}
                  </Select>
                  {fieldState.error && (
                    <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.5 }}>
                      {fieldState.error.message}
                    </Typography>
                  )}
                </FormControl>
              )}
            />

            <TextField
              label="Название"
              fullWidth
              {...form.register('name')}
              error={!!form.formState.errors.name}
              helperText={form.formState.errors.name?.message}
            />

            <TextField
              label="Описание"
              fullWidth
              multiline
              rows={3}
              {...form.register('description')}
              error={!!form.formState.errors.description}
              helperText={form.formState.errors.description?.message}
            />

            <TextField
              label="Цена"
              type="number"
              fullWidth
              {...form.register('price', { valueAsNumber: true })}
              error={!!form.formState.errors.price}
              helperText={form.formState.errors.price?.message}
            />

            <TextField
              label="Порядок сортировки"
              type="number"
              fullWidth
              {...form.register('sortOrder', { valueAsNumber: true })}
              error={!!form.formState.errors.sortOrder}
              helperText={form.formState.errors.sortOrder?.message}
            />

            <Controller
              name="isVisible"
              control={form.control}
              render={({ field }) => (
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={field.value}
                      onChange={(_, checked) => field.onChange(checked)}
                    />
                  }
                  label="Видимость"
                />
              )}
            />

            <Controller
              name="photos"
              control={form.control}
              render={({ field }) => (
                <DishGalleryEditor
                  value={field.value}
                  onChange={(next) => field.onChange(next)}
                />
              )}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={closeDialog} disabled={submitting}>
              Отмена
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={submitting || photosUploading}
              startIcon={submitting ? <CircularProgress size={18} color="inherit" /> : undefined}
            >
              {editingDish ? 'Сохранить' : 'Создать'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* Delete Confirmation */}
      <ConfirmDialog
        open={!!deletingDish}
        title="Удалить блюдо"
        message={deletingDish ? `Вы уверены, что хотите удалить "${deletingDish.name}"?` : ''}
        onConfirm={handleDelete}
        onCancel={() => setDeletingDish(null)}
        loading={deleteDish.isPending}
      />
    </Container>
  );
}
