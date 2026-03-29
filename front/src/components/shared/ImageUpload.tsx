import { useRef, useState, useEffect } from 'react'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import CloudUploadIcon from '@mui/icons-material/CloudUpload'
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline'
import { uploadImage } from '@/api/uploads'
import type { UploadImageResponse } from '@/types/content'

interface ImageUploadProps {
  value: string | null
  onChange: (relativePath: string | null) => void
  area: 'dish' | 'about'
}

function ImageUpload({ value, onChange, area }: ImageUploadProps) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [uploading, setUploading] = useState(false)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)

  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl)
    }
  }, [previewUrl])

  const displaySrc = previewUrl ?? value

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    e.target.value = ''

    const objectUrl = URL.createObjectURL(file)
    setPreviewUrl(objectUrl)
    setUploading(true)

    try {
      const response: UploadImageResponse = await uploadImage(file, area)
      onChange(response.relativePath)
      URL.revokeObjectURL(objectUrl)
      setPreviewUrl(null)
    } catch (err) {
      console.error('Image upload failed', err)
    } finally {
      setUploading(false)
    }
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, alignItems: 'flex-start' }}>
      {displaySrc && (
        <Box sx={{ position: 'relative', width: 200, height: 200 }}>
          <Box
            component="img"
            src={displaySrc}
            sx={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: 1 }}
          />
          {uploading && (
            <Box sx={{ position: 'absolute', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'rgba(0,0,0,0.4)', borderRadius: 1 }}>
              <CircularProgress size={36} sx={{ color: '#fff' }} />
            </Box>
          )}
        </Box>
      )}

      <input ref={inputRef} type="file" accept="image/jpeg,image/png,image/webp" hidden onChange={handleFileChange} />

      <Box sx={{ display: 'flex', gap: 1 }}>
        <Button variant="outlined" startIcon={<CloudUploadIcon />} onClick={() => inputRef.current?.click()} disabled={uploading}>
          Загрузить фото
        </Button>
        {value && (
          <Button size="small" color="error" startIcon={<DeleteOutlineIcon />} onClick={() => onChange(null)} disabled={uploading}>
            Удалить фото
          </Button>
        )}
      </Box>
    </Box>
  )
}

export default ImageUpload
