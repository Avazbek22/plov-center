import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#E65100',
      light: '#FF833A',
      dark: '#AC1900',
    },
    secondary: {
      main: '#5D4037',
      light: '#8B6B61',
      dark: '#321911',
    },
    background: {
      default: '#FFF8F0',
      paper: '#FFFFFF',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Arial", sans-serif',
  },
  shape: {
    borderRadius: 8,
  },
});

export default theme;
