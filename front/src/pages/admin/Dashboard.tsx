import { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { animate } from 'motion';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardActionArea from '@mui/material/CardActionArea';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import AddIcon from '@mui/icons-material/Add';
import CategoryIcon from '@mui/icons-material/Category';
import RestaurantMenuIcon from '@mui/icons-material/RestaurantMenu';
import { useAuth } from '@/auth/auth-context';
import { useCategoriesQuery } from '@/hooks/use-categories';
import { useDishesQuery } from '@/hooks/use-dishes';

function AnimatedNumber({ value }: { value: number }) {
  const ref = useRef<HTMLSpanElement>(null);
  const prevValue = useRef(0);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    const controls = animate(prevValue.current, value, {
      duration: 0.8,
      ease: [0.22, 1, 0.36, 1],
      onUpdate: (v) => {
        el.textContent = Math.round(v).toString();
      },
    });

    prevValue.current = value;
    return () => controls.stop();
  }, [value]);

  return <span ref={ref}>0</span>;
}

export default function Dashboard() {
  const { session } = useAuth();
  const navigate = useNavigate();
  const { data: categories } = useCategoriesQuery();
  const { data: dishes } = useDishesQuery();

  const stats = [
    { label: 'Категории', value: categories?.length ?? 0, path: '/admin/categories' },
    { label: 'Блюда', value: dishes?.length ?? 0, path: '/admin/dishes' },
  ];

  return (
    <Container maxWidth="md" sx={{ mt: 2 }}>
      <Box>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          Дашборд
        </Typography>
        <Box sx={{ width: 40, height: 2, bgcolor: 'primary.main', borderRadius: 1, mt: 1, mb: 3 }} />
      </Box>

      <Typography sx={{ color: 'text.secondary', mb: 3 }}>
        {session.admin
          ? `Добро пожаловать, ${session.admin.username}`
          : 'Панель управления'}
      </Typography>

      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
        {stats.map((s) => (
          <Card key={s.label} sx={{ flex: '1 1 140px', minWidth: 140 }}>
            <CardActionArea onClick={() => navigate(s.path)} sx={{ height: 120 }}>
              <CardContent
                sx={{
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  justifyContent: 'center',
                  py: 2,
                }}
              >
                <Typography
                  sx={{
                    fontSize: '2.5rem',
                    fontWeight: 700,
                    color: 'primary.main',
                    lineHeight: 1,
                  }}
                >
                  <AnimatedNumber value={s.value} />
                </Typography>
                <Typography
                  sx={{
                    fontSize: '0.85rem',
                    color: 'text.secondary',
                    textTransform: 'uppercase',
                    letterSpacing: '0.08em',
                    mt: 0.5,
                  }}
                >
                  {s.label}
                </Typography>
              </CardContent>
            </CardActionArea>
          </Card>
        ))}

        <Card sx={{ flex: '1 1 140px', minWidth: 140 }}>
          <CardActionArea
            component="a"
            href="/"
            target="_blank"
            rel="noopener noreferrer"
            sx={{ height: 120 }}
          >
            <CardContent
              sx={{
                height: '100%',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                py: 2,
              }}
            >
              <IconButton
                component="span"
                sx={{ color: 'primary.main', pointerEvents: 'none' }}
              >
                <OpenInNewIcon sx={{ fontSize: '2.5rem' }} />
              </IconButton>
              <Typography
                sx={{
                  fontSize: '0.85rem',
                  color: 'text.secondary',
                  textTransform: 'uppercase',
                  letterSpacing: '0.08em',
                  mt: 0.5,
                }}
              >
                Меню
              </Typography>
            </CardContent>
          </CardActionArea>
        </Card>
      </Box>

      <Typography variant="h6" sx={{ mt: 4, mb: 2, fontWeight: 600 }}>
        Быстрые действия
      </Typography>
      <Box sx={{ display: 'flex', gap: 1.5, flexWrap: 'wrap' }}>
        <Button
          variant="outlined"
          startIcon={<CategoryIcon />}
          endIcon={<AddIcon />}
          onClick={() => navigate('/admin/categories')}
        >
          Категория
        </Button>
        <Button
          variant="outlined"
          startIcon={<RestaurantMenuIcon />}
          endIcon={<AddIcon />}
          onClick={() => navigate('/admin/dishes')}
        >
          Блюдо
        </Button>
      </Box>
    </Container>
  );
}
