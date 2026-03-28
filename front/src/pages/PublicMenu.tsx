import { useCallback, useEffect, useRef, useState } from 'react';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import CircularProgress from '@mui/material/CircularProgress';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import PhoneIcon from '@mui/icons-material/Phone';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import AccessTimeIcon from '@mui/icons-material/AccessTime';

import { usePublicMenu, usePublicContent } from '@/hooks/use-public-menu';
import type { PublicMenuCategory } from '@/types/public';

function formatPrice(price: number): string {
  return price.toLocaleString('ru-RU');
}

export default function PublicMenu() {
  const { data: menu, isLoading: menuLoading, error: menuError } = usePublicMenu();
  const { data: content, isLoading: contentLoading } = usePublicContent();

  const [activeTab, setActiveTab] = useState(0);
  const sectionRefs = useRef<Map<number, HTMLDivElement>>(new Map());
  const tabsRef = useRef<HTMLDivElement>(null);
  const isScrollingByClick = useRef(false);

  const categories = menu?.categories ?? [];

  const handleTabClick = useCallback((_: React.SyntheticEvent, index: number) => {
    setActiveTab(index);
    const section = sectionRefs.current.get(index);
    if (section && tabsRef.current) {
      isScrollingByClick.current = true;
      const tabsHeight = tabsRef.current.getBoundingClientRect().height;
      const top = section.getBoundingClientRect().top + window.scrollY - tabsHeight;
      window.scrollTo({ top, behavior: 'smooth' });
      setTimeout(() => { isScrollingByClick.current = false; }, 800);
    }
  }, []);

  useEffect(() => {
    if (categories.length === 0) return;

    function onScroll() {
      if (isScrollingByClick.current) return;
      const tabsHeight = tabsRef.current?.getBoundingClientRect().height ?? 0;

      let closestIndex = 0;
      let closestDistance = Infinity;

      sectionRefs.current.forEach((el, index) => {
        const distance = Math.abs(el.getBoundingClientRect().top - tabsHeight - 16);
        if (distance < closestDistance) {
          closestDistance = distance;
          closestIndex = index;
        }
      });

      setActiveTab(closestIndex);
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, [categories.length]);

  if (menuLoading || contentLoading) {
    return (
      <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'background.default' }}>
        <CircularProgress color="primary" />
      </Box>
    );
  }

  if (menuError) {
    return (
      <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'background.default' }}>
        <Typography color="error">Не удалось загрузить меню</Typography>
      </Box>
    );
  }

  const about = content?.about;
  const contacts = content?.contacts;

  return (
    <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', pb: 4 }}>
      {/* Hero */}
      <Box
        sx={{
          background: 'linear-gradient(135deg, #E65100 0%, #AC1900 100%)',
          color: '#fff',
          pt: 5,
          pb: about?.photoPath ? 0 : 5,
          textAlign: 'center',
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        <Container maxWidth="sm">
          <Typography variant="h3" component="h1" fontWeight={800} sx={{ letterSpacing: '-0.5px' }}>
            Плов Центр
          </Typography>
          {about?.text && (
            <Typography variant="body1" sx={{ mt: 1.5, opacity: 0.9, lineHeight: 1.6 }}>
              {about.text}
            </Typography>
          )}
        </Container>
        {about?.photoPath && (
          <Box
            component="img"
            src={about.photoPath}
            alt="Плов Центр"
            sx={{
              width: '100%',
              maxHeight: 220,
              objectFit: 'cover',
              mt: 3,
              display: 'block',
            }}
          />
        )}
      </Box>

      {/* Sticky category tabs */}
      {categories.length > 0 && (
        <Box
          ref={tabsRef}
          sx={{
            position: 'sticky',
            top: 0,
            zIndex: 10,
            bgcolor: 'background.paper',
            boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
          }}
        >
          <Container maxWidth="sm" disableGutters>
            <Tabs
              value={activeTab}
              onChange={handleTabClick}
              variant="scrollable"
              scrollButtons="auto"
              allowScrollButtonsMobile
              textColor="primary"
              indicatorColor="primary"
              sx={{
                '& .MuiTab-root': {
                  textTransform: 'none',
                  fontWeight: 600,
                  fontSize: '0.9rem',
                  minWidth: 'auto',
                  px: 2,
                },
              }}
            >
              {categories.map((cat) => (
                <Tab key={cat.id} label={cat.name} />
              ))}
            </Tabs>
          </Container>
        </Box>
      )}

      {/* Menu sections */}
      <Container maxWidth="sm" sx={{ mt: 2 }}>
        {categories.map((category, index) => (
          <CategorySection
            key={category.id}
            category={category}
            ref={(el) => {
              if (el) sectionRefs.current.set(index, el);
              else sectionRefs.current.delete(index);
            }}
          />
        ))}

        {categories.length === 0 && (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <Typography variant="h6" color="text.secondary">
              Меню скоро появится
            </Typography>
          </Box>
        )}
      </Container>

      {/* Contacts footer */}
      {contacts && (contacts.address || contacts.phone || contacts.hours) && (
        <Container maxWidth="sm" sx={{ mt: 4 }}>
          <Divider sx={{ mb: 3 }} />
          <Typography variant="h6" fontWeight={700} sx={{ mb: 2, color: 'secondary.main' }}>
            Контакты
          </Typography>

          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
            {contacts.address && (
              <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1 }}>
                <LocationOnIcon sx={{ color: 'primary.main', fontSize: 20, mt: 0.3 }} />
                <Typography variant="body2" color="text.secondary">{contacts.address}</Typography>
              </Box>
            )}
            {contacts.phone && (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <PhoneIcon sx={{ color: 'primary.main', fontSize: 20 }} />
                <Typography
                  component="a"
                  href={`tel:${contacts.phone.replace(/\s/g, '')}`}
                  variant="body2"
                  sx={{ color: 'primary.main', textDecoration: 'none', fontWeight: 600 }}
                >
                  {contacts.phone}
                </Typography>
              </Box>
            )}
            {contacts.hours && (
              <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1 }}>
                <AccessTimeIcon sx={{ color: 'primary.main', fontSize: 20, mt: 0.3 }} />
                <Typography variant="body2" color="text.secondary">{contacts.hours}</Typography>
              </Box>
            )}
          </Box>

          {contacts.mapEmbed && (
            <Box
              sx={{ mt: 3, borderRadius: 2, overflow: 'hidden', lineHeight: 0 }}
              dangerouslySetInnerHTML={{ __html: contacts.mapEmbed }}
            />
          )}
        </Container>
      )}
    </Box>
  );
}

interface CategorySectionProps {
  category: PublicMenuCategory;
}

const CategorySection = ({ category, ref }: CategorySectionProps & { ref?: React.Ref<HTMLDivElement> }) => (
  <Box ref={ref} sx={{ mb: 4 }}>
    <Typography variant="h6" fontWeight={700} sx={{ mb: 1.5, color: 'secondary.main' }}>
      {category.name}
    </Typography>

    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
      {category.dishes.map((dish) =>
        dish.photoPath ? (
          <Card key={dish.id} elevation={0} sx={{ display: 'flex', bgcolor: 'background.paper', borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
            <CardMedia
              component="img"
              image={dish.photoPath}
              alt={dish.name}
              sx={{ width: 110, height: 110, objectFit: 'cover', flexShrink: 0 }}
            />
            <CardContent sx={{ flex: 1, py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
              <Typography variant="subtitle2" fontWeight={700} sx={{ lineHeight: 1.3 }}>
                {dish.name}
              </Typography>
              {dish.description && (
                <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.5, lineHeight: 1.4 }}>
                  {dish.description}
                </Typography>
              )}
              <Typography variant="body2" fontWeight={700} color="primary" sx={{ mt: 1 }}>
                {formatPrice(dish.price)} сум
              </Typography>
            </CardContent>
          </Card>
        ) : (
          <Box
            key={dish.id}
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'baseline',
              py: 1,
              px: 2,
              bgcolor: 'background.paper',
              borderRadius: 2,
              border: '1px solid',
              borderColor: 'divider',
            }}
          >
            <Box sx={{ flex: 1, mr: 2 }}>
              <Typography variant="subtitle2" fontWeight={600}>
                {dish.name}
              </Typography>
              {dish.description && (
                <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.3 }}>
                  {dish.description}
                </Typography>
              )}
            </Box>
            <Typography variant="body2" fontWeight={700} color="primary" sx={{ whiteSpace: 'nowrap' }}>
              {formatPrice(dish.price)} сум
            </Typography>
          </Box>
        ),
      )}
    </Box>
  </Box>
);
