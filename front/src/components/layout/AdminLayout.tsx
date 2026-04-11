import { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useTheme } from '@mui/material/styles';
import LogoutIcon from '@mui/icons-material/Logout';
import MenuIcon from '@mui/icons-material/Menu';
import { useAuth } from '@/auth/auth-context';
import AdminSidebar from './AdminSidebar';

const DRAWER_WIDTH = 260;

function DrawerContent() {
  return (
    <>
      <Box sx={{ px: 2.5, py: 2.5 }}>
        <Typography
          variant="h6"
          noWrap
          sx={{
            fontFamily: '"Young Serif", Georgia, serif',
            fontWeight: 700,
            fontSize: '1.3rem',
            color: '#F9F5EE',
            letterSpacing: '-0.01em',
          }}
        >
          Плов Центр
        </Typography>
        <Typography
          variant="caption"
          sx={{ color: 'rgba(249,245,238,0.4)', fontSize: '0.7rem', display: 'block', mt: 0.25 }}
        >
          Админ-панель
        </Typography>
      </Box>
      <AdminSidebar />
    </>
  );
}

export default function AdminLayout() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/admin/login', { replace: true });
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {isMobile ? (
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' },
          }}
        >
          <DrawerContent />
        </Drawer>
      ) : (
        <Drawer
          variant="permanent"
          sx={{
            width: DRAWER_WIDTH,
            flexShrink: 0,
            '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' },
          }}
        >
          <DrawerContent />
        </Drawer>
      )}

      <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column', width: isMobile ? '100%' : `calc(100% - ${DRAWER_WIDTH}px)` }}>
        <AppBar position="static" elevation={0}>
          <Toolbar sx={{ gap: 1 }}>
            {isMobile && (
              <IconButton
                edge="start"
                onClick={() => setMobileOpen(true)}
                sx={{ color: 'text.primary' }}
              >
                <MenuIcon />
              </IconButton>
            )}
            <Box sx={{ flexGrow: 1 }} />
            {session.admin && (
              <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                {session.admin.username}
              </Typography>
            )}
            <Button
              size="small"
              onClick={handleLogout}
              startIcon={<LogoutIcon sx={{ fontSize: 18 }} />}
              sx={{ color: 'text.secondary', '&:hover': { color: 'text.primary' } }}
            >
              Выйти
            </Button>
          </Toolbar>
        </AppBar>

        <Box component="main" sx={{ flexGrow: 1, p: { xs: 1.5, sm: 3 }, bgcolor: 'background.default' }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
