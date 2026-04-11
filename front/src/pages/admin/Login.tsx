import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Alert from '@mui/material/Alert';
import Stack from '@mui/material/Stack';
import { motion } from 'motion/react';
import { useAuth } from '@/auth/auth-context';
import { ApiError } from '@/api/client';

const loginSchema = z.object({
  username: z.string().min(1, 'Введите логин'),
  password: z.string().min(1, 'Введите пароль'),
});

type LoginForm = z.infer<typeof loginSchema>;

const MotionCard = motion.create(Card);

export default function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginForm) => {
    setServerError(null);
    try {
      await login(data);
      navigate('/admin', { replace: true });
    } catch (error) {
      if (error instanceof ApiError) {
        setServerError(error.response.message);
      } else {
        setServerError('Не удалось подключиться к серверу.');
      }
    }
  };

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: 'secondary.main',
        background: (theme) => `radial-gradient(circle at 50% 40%, ${theme.palette.primary.main}0F 0%, transparent 60%), ${theme.palette.secondary.main}`,
      }}
    >
      <MotionCard
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, ease: 'easeOut' }}
        sx={{ width: '100%', maxWidth: 400, borderRadius: '16px' }}
      >
        <CardContent sx={{ p: 3 }}>
          <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
            <Box>
              <Typography
                variant="h5"
                component="h1"
                textAlign="center"
                fontWeight={600}
                sx={{ fontFamily: '"Young Serif", Georgia, serif', mb: 1.5 }}
              >
                Вход
              </Typography>
              <Box
                sx={{
                  width: 40,
                  height: 2,
                  bgcolor: 'primary.main',
                  mx: 'auto',
                  borderRadius: 1,
                }}
              />
            </Box>

            {serverError && <Alert severity="error">{serverError}</Alert>}

            <TextField
              label="Логин"
              autoComplete="username"
              autoFocus
              error={!!errors.username}
              helperText={errors.username?.message}
              {...register('username')}
            />

            <TextField
              label="Пароль"
              type="password"
              autoComplete="current-password"
              error={!!errors.password}
              helperText={errors.password?.message}
              {...register('password')}
            />

            <Button
              type="submit"
              variant="contained"
              size="large"
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Вход...' : 'Войти'}
            </Button>
          </Stack>
        </CardContent>
      </MotionCard>
    </Box>
  );
}
