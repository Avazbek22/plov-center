import { useRef } from 'react'
import {
  DndContext,
  PointerSensor,
  KeyboardSensor,
  closestCenter,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import {
  SortableContext,
  arrayMove,
  rectSortingStrategy,
  sortableKeyboardCoordinates,
  useSortable,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

import Box from '@mui/material/Box'
import CircularProgress from '@mui/material/CircularProgress'
import IconButton from '@mui/material/IconButton'
import CloseIcon from '@mui/icons-material/Close'
import StarIcon from '@mui/icons-material/Star'
import DragIndicatorIcon from '@mui/icons-material/DragIndicator'
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate'

import { uploadImage } from '@/api/uploads'
import { imageUrl } from '@/utils/image-url'
import type { DishPhotoForm } from '@/types/dish'

interface DishGalleryEditorProps {
  value: DishPhotoForm[]
  onChange: (next: DishPhotoForm[]) => void
}

function generateTempId(): string {
  return `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 10)}`
}

function reindex(photos: DishPhotoForm[]): DishPhotoForm[] {
  return photos.map((p, index) => ({ ...p, sortOrder: index }))
}

export default function DishGalleryEditor({ value, onChange }: DishGalleryEditorProps) {
  const inputRef = useRef<HTMLInputElement>(null)

  // Keep a ref-mirror of the latest value so async upload callbacks
  // close over the freshest array (avoids stale-closure overwrites
  // when multiple parallel uploads finish).
  const currentValueRef = useRef(value)
  // eslint-disable-next-line react-hooks/refs
  currentValueRef.current = value

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 6 } }),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates }),
  )

  function handleFiles(files: FileList | null) {
    if (!files || files.length === 0) return

    const fileArray = Array.from(files)
    const startIndex = value.length

    const newPhotos: DishPhotoForm[] = fileArray.map((_, i) => ({
      tempId: generateTempId(),
      relativePath: null,
      sortOrder: startIndex + i,
      uploading: true,
    }))

    onChange([...value, ...newPhotos])

    // Batch all upload results into a single onChange so concurrent resolves
    // can't race: each individual onChange call would read currentValueRef
    // before the previous call's update flushed back, and React 19's automatic
    // batching collapses the calls into one — the last one wins.
    Promise.allSettled(
      fileArray.map((file, i) =>
        uploadImage(file, 'dish').then((response) => ({
          tempId: newPhotos[i].tempId,
          relativePath: response.relativePath,
        })),
      ),
    ).then((results) => {
      const succeeded = new Map<string, string>()
      const failed = new Set<string>()
      results.forEach((result, i) => {
        const tempId = newPhotos[i].tempId
        if (result.status === 'fulfilled') {
          succeeded.set(tempId, result.value.relativePath)
        } else {
          failed.add(tempId)
        }
      })
      onChange(
        (currentValueRef.current ?? [])
          .filter((p) => !failed.has(p.tempId))
          .map((p) =>
            succeeded.has(p.tempId)
              ? { ...p, relativePath: succeeded.get(p.tempId)!, uploading: false }
              : p,
          ),
      )
    })

    if (inputRef.current) inputRef.current.value = ''
  }

  function handleRemove(tempId: string) {
    onChange(reindex(value.filter((p) => p.tempId !== tempId)))
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over || active.id === over.id) return

    const oldIndex = value.findIndex((p) => p.tempId === active.id)
    const newIndex = value.findIndex((p) => p.tempId === over.id)
    if (oldIndex < 0 || newIndex < 0) return

    onChange(reindex(arrayMove(value, oldIndex, newIndex)))
  }

  return (
    <Box>
      <Box sx={{ fontSize: 12, fontWeight: 600, color: 'text.secondary', mb: 1 }}>
        Фото блюда
      </Box>

      <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
        <SortableContext items={value.map((p) => p.tempId)} strategy={rectSortingStrategy}>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {value.map((photo, index) => (
              <SortableTile
                key={photo.tempId}
                photo={photo}
                isCover={index === 0}
                onRemove={() => handleRemove(photo.tempId)}
              />
            ))}
            <AddTile onClick={() => inputRef.current?.click()} />
          </Box>
        </SortableContext>
      </DndContext>

      <input
        ref={inputRef}
        type="file"
        accept="image/jpeg,image/png"
        hidden
        multiple
        onChange={(e) => handleFiles(e.target.files)}
      />

      <Box sx={{ fontSize: 11, color: 'text.disabled', mt: 1 }}>
        Перетащи, чтобы изменить порядок · первое фото = обложка · JPG/PNG, до 5 МБ
      </Box>
    </Box>
  )
}

interface SortableTileProps {
  photo: DishPhotoForm
  isCover: boolean
  onRemove: () => void
}

function SortableTile({ photo, isCover, onRemove }: SortableTileProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: photo.tempId,
  })

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.4 : 1,
  }

  const src = photo.relativePath ? imageUrl(photo.relativePath) : null

  return (
    <Box
      ref={setNodeRef}
      style={style}
      sx={{
        position: 'relative',
        width: 96,
        height: 96,
        border: isCover ? '2px solid' : '1px solid',
        borderColor: isCover ? 'primary.main' : 'divider',
        borderRadius: 1,
        overflow: 'hidden',
        bgcolor: 'grey.100',
      }}
    >
      {src && (
        <Box
          component="img"
          src={src}
          alt={`Фото блюда ${photo.sortOrder + 1}`}
          sx={{ width: '100%', height: '100%', objectFit: 'cover' }}
        />
      )}

      {photo.uploading && (
        <Box sx={{
          position: 'absolute', inset: 0,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          bgcolor: 'rgba(255,255,255,0.7)',
        }}>
          <CircularProgress size={28} />
        </Box>
      )}

      {isCover && !photo.uploading && (
        <Box sx={{
          position: 'absolute', top: 4, left: 4,
          bgcolor: 'primary.main', color: 'primary.contrastText',
          fontSize: 10, fontWeight: 600,
          px: 0.75, py: 0.25, borderRadius: 8,
          display: 'flex', alignItems: 'center', gap: 0.25,
        }}>
          <StarIcon sx={{ fontSize: 12 }} /> Обложка
        </Box>
      )}

      <IconButton
        size="small"
        onClick={onRemove}
        disabled={photo.uploading}
        sx={{
          position: 'absolute', top: 2, right: 2,
          bgcolor: 'rgba(0,0,0,0.55)', color: 'white',
          width: 22, height: 22,
          '&:hover': { bgcolor: 'rgba(0,0,0,0.75)' },
        }}
      >
        <CloseIcon sx={{ fontSize: 14 }} />
      </IconButton>

      <Box
        {...attributes}
        {...listeners}
        sx={{
          position: 'absolute', bottom: 0, left: 0, right: 0,
          height: 18,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: 'rgba(255,255,255,0.85)',
          bgcolor: 'rgba(0,0,0,0.35)',
          cursor: 'grab',
          '&:active': { cursor: 'grabbing' },
        }}
      >
        <DragIndicatorIcon sx={{ fontSize: 14 }} />
      </Box>
    </Box>
  )
}

function AddTile({ onClick }: { onClick: () => void }) {
  return (
    <Box
      onClick={onClick}
      sx={{
        width: 96, height: 96,
        border: '2px dashed',
        borderColor: 'primary.light',
        borderRadius: 1,
        display: 'flex', flexDirection: 'column',
        alignItems: 'center', justifyContent: 'center',
        gap: 0.5, color: 'primary.main',
        cursor: 'pointer',
        bgcolor: 'background.paper',
        '&:hover': { bgcolor: 'action.hover' },
      }}
    >
      <AddPhotoAlternateIcon />
      <Box sx={{ fontSize: 11 }}>Загрузить</Box>
    </Box>
  )
}
