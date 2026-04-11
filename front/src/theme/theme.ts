import { createTheme } from '@mui/material/styles';

const palette = {
  primary: '#C4841D',
  primaryDark: '#A86E14',
  primaryLight: '#D4A04A',
  bg: '#FAF7F2',
  paper: '#FFFFFF',
  dark: '#2C2416',
  text: '#2C2416',
  textSecondary: '#6B5D4D',
  textMuted: '#A09585',
  border: '#E8E0D4',
  tableHeader: '#F5ECD7',
  hoverBg: 'rgba(196, 132, 29, 0.04)',
  activeBg: 'rgba(196, 132, 29, 0.10)',
};

const theme = createTheme({
  palette: {
    primary: {
      main: palette.primary,
      dark: palette.primaryDark,
      light: palette.primaryLight,
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: palette.dark,
      light: '#3D2E24',
      dark: '#0D0A07',
    },
    background: {
      default: palette.bg,
      paper: palette.paper,
    },
    text: {
      primary: palette.text,
      secondary: palette.textSecondary,
      disabled: palette.textMuted,
    },
    divider: palette.border,
  },
  typography: {
    fontFamily: '"DM Sans", system-ui, -apple-system, sans-serif',
    h4: { fontWeight: 700, letterSpacing: '-0.01em' },
    h5: { fontWeight: 700, letterSpacing: '-0.01em' },
    h6: { fontWeight: 600 },
    subtitle1: { fontWeight: 500 },
    button: { fontWeight: 600, textTransform: 'none' as const },
  },
  shape: {
    borderRadius: 10,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          boxShadow: 'none',
          '&:hover': { boxShadow: 'none' },
        },
        containedPrimary: {
          '&:hover': { backgroundColor: palette.primaryDark },
        },
      },
      defaultProps: {
        disableElevation: true,
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          border: `1px solid ${palette.border}`,
          boxShadow: 'none',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        outlined: {
          borderColor: palette.border,
          borderRadius: 12,
        },
      },
    },
    MuiTableHead: {
      styleOverrides: {
        root: {
          '& .MuiTableCell-head': {
            backgroundColor: palette.tableHeader,
            fontWeight: 600,
            fontSize: '0.82rem',
            textTransform: 'uppercase' as const,
            letterSpacing: '0.04em',
            color: palette.textSecondary,
            borderBottom: `1px solid ${palette.border}`,
          },
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          borderBottom: `1px solid ${palette.border}`,
          fontSize: '0.9rem',
        },
      },
    },
    MuiTableRow: {
      styleOverrides: {
        root: {
          '&:hover': { backgroundColor: palette.hoverBg },
          '&:last-child td': { borderBottom: 0 },
        },
      },
    },
    MuiDialog: {
      styleOverrides: {
        paper: {
          borderRadius: 16,
        },
      },
    },
    MuiDialogTitle: {
      styleOverrides: {
        root: {
          fontWeight: 600,
          fontSize: '1.15rem',
        },
      },
    },
    MuiDialogActions: {
      styleOverrides: {
        root: {
          padding: '12px 24px 16px',
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
              borderColor: palette.primary,
            },
          },
        },
      },
    },
    MuiSwitch: {
      styleOverrides: {
        switchBase: {
          '&.Mui-checked': {
            color: palette.primary,
            '& + .MuiSwitch-track': {
              backgroundColor: palette.primary,
            },
          },
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          backgroundColor: palette.dark,
          color: palette.bg,
          borderRight: 'none',
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: palette.paper,
          color: palette.text,
          boxShadow: 'none',
          borderBottom: `1px solid ${palette.border}`,
        },
      },
    },
    MuiCircularProgress: {
      styleOverrides: {
        colorPrimary: {
          color: palette.primary,
        },
      },
    },
    MuiIconButton: {
      styleOverrides: {
        root: {
          '&:hover': { backgroundColor: palette.hoverBg },
        },
      },
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          marginLeft: 8,
          marginRight: 8,
          '&.Mui-selected': {
            backgroundColor: palette.activeBg,
            color: palette.primary,
            '& .MuiListItemIcon-root': { color: palette.primary },
          },
          '&.Mui-selected:hover': {
            backgroundColor: palette.activeBg,
          },
          '&:hover': {
            backgroundColor: 'rgba(249, 245, 238, 0.08)',
          },
        },
      },
    },
    MuiListItemIcon: {
      styleOverrides: {
        root: {
          color: palette.textMuted,
          minWidth: 40,
        },
      },
    },
  },
});

export default theme;
