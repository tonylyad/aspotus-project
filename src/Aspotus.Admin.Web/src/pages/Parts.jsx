import { useState, useEffect, useCallback } from 'react'
import {
  Box,
  Button,
  TextField,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Alert,
  Snackbar,
  TablePagination,
  Switch,
  FormControlLabel,
  FormHelperText,
} from '@mui/material'
import { Edit as EditIcon, Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material'
import CatalogImagesEditor from '../components/CatalogImagesEditor'
import {
  deleteCatalogImages,
  toImageRequests,
  uploadCatalogImages,
} from '../utils/catalogImages'

const API = '/catalog/api'

async function apiGet(url) {
  const token = localStorage.getItem('token')
  const headers = { Authorization: `Bearer ${token}` }
  const res = await fetch(url, { headers })
  if (!res.ok) throw new Error('Ошибка загрузки')
  return res.json()
}

async function apiSave(url, method, body) {
  const token = localStorage.getItem('token')
  const headers = { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }
  const res = await fetch(url, { method, headers, body: JSON.stringify(body) })
  if (!res.ok) {
    const data = await res.json().catch(() => null)
    throw new Error(data?.message || data?.title || 'Ошибка сохранения')
  }
  return res.status === 204 ? null : res.json()
}

const conditionLabels = { 1: 'New', 2: 'Used' }

function emptyForm() {
  return {
    name: '',
    article: '',
    description: '',
    price: '',
    stockQuantity: '',
    isOriginal: false,
    conditionType: 1,
    conditionPercent: '',
    conditionDescription: '',
    mileageAtRemoval: '',
    replacementArticles: '',
    categoryId: '',
    manufacturerId: '',
  }
}

export default function Parts() {
  const [parts, setParts] = useState([])
  const [categories, setCategories] = useState([])
  const [manufacturers, setManufacturers] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)

  const [dialog, setDialog] = useState(false)
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selectedPart, setSelectedPart] = useState(null)
  const [form, setForm] = useState(emptyForm())
  const [saving, setSaving] = useState(false)
  const [images, setImages] = useState([])
  const [removedImageKeys, setRemovedImageKeys] = useState([])

  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchParts = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/parts`)
      setParts(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setLoading(false)
    }
  }, [])

  const fetchLookups = useCallback(async () => {
    try {
      const [cats, mans] = await Promise.all([
        apiGet(`${API}/categories`),
        apiGet(`${API}/manufacturers`),
      ])
      setCategories(cats)
      setManufacturers(mans)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }, [])

  useEffect(() => { fetchLookups() }, [fetchLookups])
  useEffect(() => { fetchParts() }, [fetchParts])

  function set(field) {
    return (e) => setForm((prev) => ({ ...prev, [field]: e.target.value }))
  }

  function setBool(field) {
    return (e) => setForm((prev) => ({ ...prev, [field]: e.target.checked }))
  }

  const openCreate = () => {
    setSelectedPart(null)
    setForm(emptyForm())
    setImages([])
    setRemovedImageKeys([])
    setDialog(true)
  }

  const openEdit = (part) => {
    setSelectedPart(part)
    setForm({
      name: part.name,
      article: part.article,
      description: part.description || '',
      price: String(part.price),
      stockQuantity: String(part.stockQuantity),
      isOriginal: part.isOriginal,
      conditionType: part.conditionType,
      conditionPercent: part.conditionPercent != null ? String(part.conditionPercent) : '',
      conditionDescription: part.conditionDescription || '',
      mileageAtRemoval: part.mileageAtRemoval != null ? String(part.mileageAtRemoval) : '',
      replacementArticles: (part.replacementArticles || []).join(', '),
      categoryId: part.categoryId,
      manufacturerId: part.manufacturerId,
    })
    setImages(part.images || [])
    setRemovedImageKeys([])
    setDialog(true)
  }

  const openDelete = (part) => {
    setSelectedPart(part)
    setDeleteDialog(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      const body = {
        name: form.name,
        article: form.article,
        description: form.description || null,
        price: parseFloat(form.price),
        stockQuantity: parseInt(form.stockQuantity, 10),
        isOriginal: form.isOriginal,
        conditionType: parseInt(form.conditionType, 10),
        conditionPercent: form.conditionPercent ? parseInt(form.conditionPercent, 10) : null,
        conditionDescription: form.conditionDescription || null,
        mileageAtRemoval: form.mileageAtRemoval ? parseInt(form.mileageAtRemoval, 10) : null,
        replacementArticles: form.replacementArticles
          ? form.replacementArticles.split(',').map((s) => s.trim()).filter(Boolean)
          : [],
        categoryId: form.categoryId,
        manufacturerId: form.manufacturerId,
        images: toImageRequests(images.filter((image) => !image.pending)),
      }

      if (selectedPart) {
        const resolvedImages = await uploadCatalogImages('parts', selectedPart.id, images)
        body.images = toImageRequests(resolvedImages)
        await apiSave(`${API}/parts/${selectedPart.id}`, 'PUT', body)
        setSnackbar({ open: true, message: 'Запчасть обновлена', severity: 'success' })
      } else {
        const saved = await apiSave(`${API}/parts`, 'POST', body)
        const resolvedImages = await uploadCatalogImages('parts', saved.id, images)
        if (resolvedImages.length > 0) {
          body.images = toImageRequests(resolvedImages)
          await apiSave(`${API}/parts/${saved.id}`, 'PUT', body)
        }
        setSnackbar({ open: true, message: 'Запчасть создана', severity: 'success' })
      }

      await deleteCatalogImages(removedImageKeys)

      setDialog(false)
      fetchParts()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    try {
      await apiSave(`${API}/parts/${selectedPart.id}`, 'DELETE')
      await deleteCatalogImages((selectedPart.images || []).map((image) => image.fileKey))
      setDeleteDialog(false)
      setSnackbar({ open: true, message: 'Запчасть удалена', severity: 'success' })
      fetchParts()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const visibleParts = parts.filter((p) => {
    if (!search) return true
    const q = search.toLowerCase()
    return p.name.toLowerCase().includes(q) || p.article.toLowerCase().includes(q)
  })

  const isNew = form.conditionType == 1

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Запчасти</Typography>

      <Box className="page-toolbar">
        <TextField
          size="small"
          placeholder="Поиск по названию или артикулу"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(0) }}
          sx={{ minWidth: 280 }}
        />
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          Создать
        </Button>
      </Box>

      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Фото</TableCell>
              <TableCell>Название</TableCell>
              <TableCell>Артикул</TableCell>
              <TableCell>Категория</TableCell>
              <TableCell>Производитель</TableCell>
              <TableCell>Цена</TableCell>
              <TableCell>Кол-во</TableCell>
              <TableCell>Состояние</TableCell>
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={9} align="center">Загрузка…</TableCell>
              </TableRow>
            ) : visibleParts.length === 0 ? (
              <TableRow>
                <TableCell colSpan={9} align="center">Запчасти не найдены</TableCell>
              </TableRow>
            ) : (
              visibleParts.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((p) => (
                <TableRow
                  key={p.id}
                  hover
                  onClick={() => openEdit(p)}
                  sx={{ cursor: 'pointer' }}
                >
                  <TableCell>
                    {p.images?.[0]?.url ? <img className="catalog-thumbnail" src={p.images[0].url} alt="" /> : '—'}
                  </TableCell>
                  <TableCell>{p.name}</TableCell>
                  <TableCell>{p.article}</TableCell>
                  <TableCell>{p.categoryName}</TableCell>
                  <TableCell>{p.manufacturerName}</TableCell>
                  <TableCell>{p.price.toLocaleString('ru')} ₽</TableCell>
                  <TableCell>{p.stockQuantity}</TableCell>
                  <TableCell>{conditionLabels[p.conditionType] || p.conditionType}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={(event) => { event.stopPropagation(); openEdit(p) }}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={(event) => { event.stopPropagation(); openDelete(p) }} color="error"><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={visibleParts.length}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </TableContainer>

      <Dialog open={dialog} onClose={() => setDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selectedPart ? 'Редактировать запчасть' : 'Создать запчасть'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <TextField
              label="Название"
              value={form.name}
              onChange={set('name')}
              required
              fullWidth
            />

            <TextField
              label="Артикул"
              value={form.article}
              onChange={set('article')}
              required
              fullWidth
            />

            <FormControl fullWidth>
              <InputLabel>Категория</InputLabel>
              <Select value={form.categoryId} label="Категория" onChange={set('categoryId')} required>
                {categories.map((c) => (
                  <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControl fullWidth>
              <InputLabel>Производитель</InputLabel>
              <Select value={form.manufacturerId} label="Производитель" onChange={set('manufacturerId')} required>
                {manufacturers.map((m) => (
                  <MenuItem key={m.id} value={m.id}>{m.name}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <Box className="form-fields__row">
              <TextField label="Цена" type="number" value={form.price} onChange={set('price')} required fullWidth slotProps={{ htmlInput: { min: 0, step: 0.01 } }} />
              <TextField label="Количество" type="number" value={form.stockQuantity} onChange={set('stockQuantity')} required fullWidth slotProps={{ htmlInput: { min: 0 } }} />
            </Box>

            <FormControlLabel
              control={<Switch checked={form.isOriginal} onChange={setBool('isOriginal')} />}
              label="Оригинальная запчасть"
            />

            <FormControl fullWidth>
              <InputLabel>Состояние</InputLabel>
              <Select value={form.conditionType} label="Состояние" onChange={set('conditionType')}>
                <MenuItem value={1}>Новая</MenuItem>
                <MenuItem value={2}>Б/У</MenuItem>
              </Select>
            </FormControl>

            {!isNew && (
              <>
                <TextField label="Износ %" type="number" value={form.conditionPercent} onChange={set('conditionPercent')} fullWidth slotProps={{ htmlInput: { min: 0, max: 100 } }} />
                <TextField label="Описание состояния" value={form.conditionDescription} onChange={set('conditionDescription')} multiline rows={2} fullWidth />
                <TextField label="Пробег при снятии (км)" type="number" value={form.mileageAtRemoval} onChange={set('mileageAtRemoval')} fullWidth slotProps={{ htmlInput: { min: 0 } }} />
              </>
            )}

            <TextField label="Описание" value={form.description} onChange={set('description')} multiline rows={2} fullWidth />

            <TextField
              label="Артикулы замен"
              value={form.replacementArticles}
              onChange={set('replacementArticles')}
              fullWidth
              helperText="Через запятую: ABC-123, XYZ-789"
            />

            <CatalogImagesEditor
              items={images}
              setItems={setImages}
              onRemoveStored={(key) => setRemovedImageKeys((current) => [...current, key])}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving}>
            {saving ? 'Сохранение…' : 'Сохранить'}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить запчасть</DialogTitle>
        <DialogContent>
          <Typography>
            Вы уверены, что хотите удалить <strong>{selectedPart?.name}</strong>?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog(false)}>Отмена</Button>
          <Button variant="contained" color="error" onClick={handleDelete}>Удалить</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  )
}
