import { Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

export default function Landing() {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: 'background.default',
      }}
    >
      <Container maxWidth="sm">
        <Stack spacing={3} alignItems="center" textAlign="center">
          <Typography variant="h2" component="h1" fontWeight={700} color="primary">
            Плов Центр
          </Typography>
          <Typography variant="h6" color="text.secondary">
            Настоящий узбекский плов и блюда восточной кухни.
            Готовим с душой, подаём с теплом.
          </Typography>
          <Button
            component={RouterLink}
            to="/admin/login"
            variant="outlined"
            size="large"
          >
            Админ-панель
          </Button>
        </Stack>
      </Container>
    </Box>
  );
}
