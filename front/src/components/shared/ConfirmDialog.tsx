import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'

interface ConfirmDialogProps {
  open: boolean
  title: string
  message: string
  onConfirm: () => void
  onCancel: () => void
  loading?: boolean
  confirmLabel?: string
  confirmColor?: 'error' | 'primary' | 'warning'
}

function ConfirmDialog({
  open,
  title,
  message,
  onConfirm,
  onCancel,
  loading = false,
  confirmLabel = 'Удалить',
  confirmColor = 'error',
}: ConfirmDialogProps) {
  return (
    <Dialog open={open} onClose={onCancel}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <WarningAmberIcon sx={{ fontSize: 40, color: 'warning.main', display: 'block', mx: 'auto', mb: 1.5 }} />
        <DialogContentText sx={{ textAlign: 'center' }}>{message}</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} disabled={loading}>
          Отмена
        </Button>
        <Button
          onClick={onConfirm}
          disabled={loading}
          color={confirmColor}
          variant="contained"
          startIcon={loading ? <CircularProgress size={18} color="inherit" /> : undefined}
        >
          {confirmLabel}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default ConfirmDialog
