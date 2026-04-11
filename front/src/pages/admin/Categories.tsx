import { useEffect, useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import CircularProgress from '@mui/material/CircularProgress';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import FormControlLabel from '@mui/material/FormControlLabel';
import IconButton from '@mui/material/IconButton';
import Stack from '@mui/material/Stack';
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
import Container from '@mui/material/Container';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';

import ConfirmDialog from '@/components/shared/ConfirmDialog';
import {
  useCategoriesQuery,
  useCreateCategory,
  useDeleteCategory,
  useSetCategoryVisibility,
  useUpdateCategory,
} from '@/hooks/use-categories';
import type { CategoryFormData, CategoryResponse } from '@/types/category';
import { categoryFormSchema } from '@/types/category';

function CategoryFormDialog({
  open,
  category,
  onClose,
}: {
  open: boolean;
  category: CategoryResponse | null;
  onClose: () => void;
}) {
  const isEdit = category !== null;

  const createMutation = useCreateCategory();
  const updateMutation = useUpdateCategory();

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors },
  } = useForm<CategoryFormData>({
    resolver: zodResolver(categoryFormSchema),
    defaultValues: { name: '', sortOrder: 0, isVisible: true },
  });

  useEffect(() => {
    if (!open) return;
    if (category) {
      reset({
        name: category.name,
        sortOrder: category.sortOrder,
        isVisible: category.isVisible,
      });
    } else {
      reset({ name: '', sortOrder: 0, isVisible: true });
    }
  }, [open, category, reset]);

  const isPending = createMutation.isPending || updateMutation.isPending;

  const onSubmit = (data: CategoryFormData) => {
    if (isEdit) {
      updateMutation.mutate(
        { id: category.id, data },
        { onSuccess: onClose },
      );
    } else {
      createMutation.mutate(data, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>
        {isEdit ? 'Редактировать категорию' : 'Создать категорию'}
      </DialogTitle>
      <DialogContent>
        <Stack
          spacing={2}
          component="form"
          id="category-form"
          onSubmit={handleSubmit(onSubmit)}
          noValidate
          sx={{ mt: 1 }}
        >
          <TextField
            label="Название"
            autoFocus
            error={!!errors.name}
            helperText={errors.name?.message}
            {...register('name')}
          />
          <TextField
            label="Порядок"
            type="number"
            error={!!errors.sortOrder}
            helperText={errors.sortOrder?.message}
            {...register('sortOrder', { valueAsNumber: true })}
          />
          <Controller
            name="isVisible"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={
                  <Checkbox
                    checked={field.value}
                    onChange={field.onChange}
                  />
                }
                label="Видимая"
              />
            )}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isPending}>
          Отмена
        </Button>
        <Button
          type="submit"
          form="category-form"
          variant="contained"
          disabled={isPending}
          startIcon={isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
        >
          {isEdit ? 'Сохранить' : 'Создать'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export default function Categories() {
  const { data: categories, isLoading } = useCategoriesQuery();
  const visibilityMutation = useSetCategoryVisibility();
  const deleteMutation = useDeleteCategory();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryResponse | null>(null);
  const [deletingCategory, setDeletingCategory] = useState<CategoryResponse | null>(null);

  const handleCreate = () => {
    setEditingCategory(null);
    setDialogOpen(true);
  };

  const handleEdit = (cat: CategoryResponse) => {
    setEditingCategory(cat);
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
  };

  const handleConfirmDelete = () => {
    if (!deletingCategory) return;
    deleteMutation.mutate(deletingCategory.id, {
      onSuccess: () => setDeletingCategory(null),
    });
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 2 }}>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h5" fontWeight={600}>
            Категории
          </Typography>
          <Box sx={{ width: 40, height: 2, bgcolor: 'primary.main', borderRadius: 1, mt: 1 }} />
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={handleCreate}>
          Добавить категорию
        </Button>
      </Stack>

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : (
        <TableContainer component={Paper} variant="outlined">
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Название</TableCell>
                <TableCell>Порядок</TableCell>
                <TableCell>Видимость</TableCell>
                <TableCell>Блюд</TableCell>
                <TableCell align="right">Действия</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {categories?.map((cat) => (
                <TableRow key={cat.id}>
                  <TableCell>{cat.name}</TableCell>
                  <TableCell>{cat.sortOrder}</TableCell>
                  <TableCell>
                    <Switch
                      checked={cat.isVisible}
                      onChange={(_, checked) =>
                        visibilityMutation.mutate({ id: cat.id, isVisible: checked })
                      }
                    />
                  </TableCell>
                  <TableCell>{cat.dishCount}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => handleEdit(cat)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" onClick={() => setDeletingCategory(cat)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <CategoryFormDialog
        open={dialogOpen}
        category={editingCategory}
        onClose={handleCloseDialog}
      />

      <ConfirmDialog
        open={deletingCategory !== null}
        title="Удалить категорию"
        message={`Вы уверены, что хотите удалить категорию «${deletingCategory?.name ?? ''}»?`}
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeletingCategory(null)}
        loading={deleteMutation.isPending}
      />
    </Container>
  );
}
