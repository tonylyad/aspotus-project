import { useState } from 'react'
import { Box, Button, IconButton, Typography } from '@mui/material'
import { AddPhotoAlternate, Delete, DragIndicator } from '@mui/icons-material'

const MAX_IMAGES = 5

export default function CatalogImagesEditor({ items, setItems, onRemoveStored }) {
  const [dropActive, setDropActive] = useState(false)
  const [draggingIndex, setDraggingIndex] = useState(null)
  const [dropPosition, setDropPosition] = useState(null)

  const addFiles = (files) => {
    const freeSlots = MAX_IMAGES - items.length
    if (freeSlots <= 0) return

    const selected = Array.from(files || [])
      .filter((file) => file.type.startsWith('image/'))
      .slice(0, freeSlots)
      .map((file) => ({
        clientId: crypto.randomUUID(),
        file,
        preview: URL.createObjectURL(file),
        pending: true,
      }))

    setItems([...items, ...selected])
  }

  const chooseFiles = (event) => {
    addFiles(event.target.files)
    event.target.value = ''
  }

  const dropFiles = (event) => {
    event.preventDefault()
    setDropActive(false)
    addFiles(event.dataTransfer.files)
  }

  const moveImage = (fromIndex, insertionIndex) => {
    if (fromIndex < 0 || insertionIndex < 0) return
    const reordered = [...items]
    const [moved] = reordered.splice(fromIndex, 1)
    const adjustedIndex = fromIndex < insertionIndex ? insertionIndex - 1 : insertionIndex
    reordered.splice(adjustedIndex, 0, moved)
    setItems(reordered)
  }

  const getDropSide = (event) => {
    const bounds = event.currentTarget.getBoundingClientRect()
    return event.clientX < bounds.left + bounds.width / 2 ? 'before' : 'after'
  }

  const dropImage = (event, targetIndex) => {
    event.preventDefault()
    const rawIndex = event.dataTransfer.getData('application/x-catalog-image-index')
    const fromIndex = Number(rawIndex)
    const side = getDropSide(event)
    if (rawIndex !== '' && Number.isInteger(fromIndex)) {
      moveImage(fromIndex, targetIndex + (side === 'after' ? 1 : 0))
    }
    setDraggingIndex(null)
    setDropPosition(null)
  }

  const removeImage = (image) => {
    if (image.pending) URL.revokeObjectURL(image.preview)
    else onRemoveStored(image.fileKey)
    setItems(items.filter((item) => item !== image))
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'baseline', justifyContent: 'space-between', gap: 2, mb: 1.5 }}>
        <Typography fontWeight={600}>Изображения</Typography>
        <Typography color="text.secondary" variant="body2">{items.length} из {MAX_IMAGES}</Typography>
      </Box>

      <Box
        className={`catalog-dropzone${dropActive ? ' catalog-dropzone--active' : ''}${items.length >= MAX_IMAGES ? ' catalog-dropzone--disabled' : ''}`}
        onDragEnter={(event) => { event.preventDefault(); setDropActive(true) }}
        onDragOver={(event) => event.preventDefault()}
        onDragLeave={(event) => { if (!event.currentTarget.contains(event.relatedTarget)) setDropActive(false) }}
        onDrop={dropFiles}
      >
        <AddPhotoAlternate className="catalog-dropzone__icon" />
        <Typography fontWeight={600} sx={{ textAlign: 'center' }}>
          {items.length >= MAX_IMAGES ? 'Добавлено максимальное количество' : 'Перетащите фотографии сюда'}
        </Typography>
        <Typography color="text.secondary" variant="body2">или</Typography>
        <Button component="label" variant="contained" disabled={items.length >= MAX_IMAGES}>
          Загрузить фотографии
          <input hidden type="file" accept="image/*" multiple onChange={chooseFiles} />
        </Button>
      </Box>

      <Typography color="text.secondary" variant="caption" sx={{ display: 'block', mt: 1.25, mb: 1.25 }}>
        Перетаскивайте карточки, чтобы изменить порядок. Первая фотография будет главной.
      </Typography>

      <Box className="catalog-images-grid">
        {items.map((image, index) => (
          <Box
            className={[
              'catalog-image',
              image.pending && 'catalog-image--pending',
              draggingIndex === index && 'catalog-image--dragging',
              dropPosition?.index === index && draggingIndex !== index && `catalog-image--drop-${dropPosition.side}`,
            ].filter(Boolean).join(' ')}
            key={image.fileKey || image.clientId}
            draggable
            onDragStart={(event) => {
              event.dataTransfer.effectAllowed = 'move'
              event.dataTransfer.setData('application/x-catalog-image-index', String(index))
              setDraggingIndex(index)
            }}
            onDragEnter={(event) => {
              event.preventDefault()
              if (draggingIndex !== index) setDropPosition({ index, side: getDropSide(event) })
            }}
            onDragOver={(event) => {
              event.preventDefault()
              event.dataTransfer.dropEffect = 'move'
              if (draggingIndex !== index) setDropPosition({ index, side: getDropSide(event) })
            }}
            onDrop={(event) => dropImage(event, index)}
            onDragEnd={() => {
              setDraggingIndex(null)
              setDropPosition(null)
            }}
          >
            <img src={image.url || image.preview} alt={`Фотография ${index + 1}`} draggable={false} />
            <DragIndicator className="catalog-image__drag" />
            <span className="catalog-image__number">{index + 1}</span>
            <Box className="catalog-image__actions">
              <IconButton size="small" color="error" title="Удалить" onClick={() => removeImage(image)}>
                <Delete />
              </IconButton>
            </Box>
          </Box>
        ))}
        {Array.from({ length: MAX_IMAGES - items.length }, (_, index) => (
          <Box className="catalog-image-placeholder" key={`placeholder-${index}`}>
            <AddPhotoAlternate />
            <Typography variant="caption">Свободное место</Typography>
          </Box>
        ))}
      </Box>
    </Box>
  )
}
