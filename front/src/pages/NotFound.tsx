import { Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { motion } from 'motion/react';

const MotionStack = motion.create(Stack);

export default function NotFound() {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: 'secondary.main',
      }}
    >
      <MotionStack
        spacing={2}
        alignItems="center"
        textAlign="center"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, ease: 'easeOut' }}
      >
        <Typography
          variant="h1"
          fontWeight={700}
          sx={{
            fontFamily: '"Young Serif", Georgia, serif',
            fontSize: '6rem',
            color: 'primary.main',
          }}
        >
          404
        </Typography>
        <Typography sx={{ color: 'background.default' }}>
          Страница не найдена
        </Typography>
        <Button component={RouterLink} to="/" variant="contained">
          На главную
        </Button>
      </MotionStack>
    </Box>
  );
}
