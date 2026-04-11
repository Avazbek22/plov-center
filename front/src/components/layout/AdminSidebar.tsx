import { Link, useLocation } from 'react-router-dom';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import DashboardIcon from '@mui/icons-material/Dashboard';
import CategoryIcon from '@mui/icons-material/Category';
import RestaurantMenuIcon from '@mui/icons-material/RestaurantMenu';
import ArticleIcon from '@mui/icons-material/Article';

const navItems = [
  { label: 'Дашборд', icon: <DashboardIcon />, path: '/admin', exact: true },
  { label: 'Категории', icon: <CategoryIcon />, path: '/admin/categories', exact: false },
  { label: 'Блюда', icon: <RestaurantMenuIcon />, path: '/admin/dishes', exact: false },
  { label: 'Контент', icon: <ArticleIcon />, path: '/admin/content', exact: false },
] as const;

export default function AdminSidebar() {
  const { pathname } = useLocation();

  return (
    <List component="nav" sx={{ px: 1, pt: 1.5 }}>
      {navItems.map(({ label, icon, path, exact }) => (
        <ListItemButton
          key={path}
          component={Link}
          to={path}
          selected={exact ? pathname === path : pathname.startsWith(path)}
          sx={{ mb: 0.5, py: 1 }}
        >
          <ListItemIcon sx={{ fontSize: 20 }}>{icon}</ListItemIcon>
          <ListItemText
            primary={label}
            primaryTypographyProps={{ fontSize: '0.9rem', fontWeight: 500 }}
          />
        </ListItemButton>
      ))}
    </List>
  );
}
