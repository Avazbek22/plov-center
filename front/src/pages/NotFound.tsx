import { Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

export default function NotFound() {
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
        <Stack spacing={2} alignItems="center" textAlign="center">
          <Typography variant="h1" fontWeight={700} color="primary">
            404
          </Typography>
          <Typography variant="h5" color="text.secondary">
            Страница не найдена
          </Typography>
          <Button component={RouterLink} to="/" variant="contained">
            На главную
          </Button>
        </Stack>
      </Container>
    </Box>
  );
}
