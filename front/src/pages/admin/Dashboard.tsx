import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { useAuth } from '@/auth/auth-context';

export default function Dashboard() {
  const { session } = useAuth();

  return (
    <Container maxWidth="md" sx={{ mt: 2 }}>
      <Typography variant="h4" gutterBottom>
        {session.admin
          ? `Добро пожаловать, ${session.admin.username}!`
          : 'Панель управления'}
      </Typography>
      <Card>
        <CardContent>
          <Typography color="text.secondary">
            Панель управления. Функциональность будет добавлена позже.
          </Typography>
        </CardContent>
      </Card>
    </Container>
  );
}
