import { useState, useEffect, useCallback } from 'react'
import {
  Box, Button, TextField, Typography, Table, TableBody, TableCell,
  TableContainer, TableHead, TableRow, Paper, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions,
  Select, MenuItem, FormControl, InputLabel,
  Alert, Snackbar, TablePagination,
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
  const res = await fetch(url, { headers: { Authorization: `Bearer ${token}` } })
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

export default function Cars() {
  const [items, setItems] = useState([])
  const [brands, setBrands] = useState([])
  const [models, setModels] = useState([])
  const [generations, setGenerations] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [dialog, setDialog] = useState(false)
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selected, setSelected] = useState(null)
  const [form, setForm] = useState({
    brandId: '', modelId: '', generationId: '', year: '', mileage: '', price: '',
    bodyType: '', trimLevelName: '', trimLevelDescription: '',
    engineVolume: '', fuelType: '', transmissionType: '', driveType: '',
  })
  const [saving, setSaving] = useState(false)
  const [images, setImages] = useState([])
  const [removedImageKeys, setRemovedImageKeys] = useState([])
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchItems = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/cars`)
      setItems(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setLoading(false)
    }
  }, [])

  const fetchBrands = useCallback(async () => {
    try { setBrands(await apiGet(`${API}/brands`)) } catch { /* ignore */ }
  }, [])

  useEffect(() => { fetchBrands() }, [fetchBrands])
  useEffect(() => { fetchItems() }, [fetchItems])

  async function fetchModelsByBrand(brandId) {
    if (!brandId) { setModels([]); return }
    try { setModels(await apiGet(`${API}/models/by-brand/${brandId}`)) } catch { setModels([]) }
  }

  async function fetchGenerationsByModel(modelId) {
    if (!modelId) { setGenerations([]); return }
    try { setGenerations(await apiGet(`${API}/generations/by-model/${modelId}`)) } catch { setGenerations([]) }
  }

  function setField(field) {
    return async (e) => {
      const value = e.target.value
      setForm((prev) => {
        const next = { ...prev, [field]: value }
        if (field === 'brandId') { next.modelId = ''; next.generationId = '' }
        if (field === 'modelId') { next.generationId = '' }
        return next
      })
      if (field === 'brandId') { setGenerations([]); await fetchModelsByBrand(value) }
      if (field === 'modelId') await fetchGenerationsByModel(value)
    }
  }

  const openCreate = () => {
    setSelected(null)
    setForm({
      brandId: '', modelId: '', generationId: '', year: '', mileage: '', price: '',
      bodyType: '', trimLevelName: '', trimLevelDescription: '',
      engineVolume: '', fuelType: '', transmissionType: '', driveType: '',
    })
    setModels([])
    setGenerations([])
    setImages([])
    setRemovedImageKeys([])
    setDialog(true)
  }

  const openEdit = async (item) => {
    setSelected(item)
    setForm({
      brandId: item.brandId,
      modelId: item.modelId,
      generationId: item.generationId,
      year: String(item.year),
      mileage: String(item.mileage),
      price: String(item.price),
      bodyType: item.bodyType,
      trimLevelName: item.trimLevelName || '',
      trimLevelDescription: item.trimLevelDescription || '',
      engineVolume: String(item.engineVolume),
      fuelType: item.fuelType,
      transmissionType: item.transmissionType,
      driveType: item.driveType,
    })
    setImages(item.images || [])
    setRemovedImageKeys([])
    await fetchModelsByBrand(item.brandId)
    await fetchGenerationsByModel(item.modelId)
    setDialog(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      const body = {
        brandId: form.brandId,
        modelId: form.modelId,
        generationId: form.generationId,
        year: parseInt(form.year, 10),
        mileage: parseInt(form.mileage, 10),
        price: parseFloat(form.price),
        bodyType: form.bodyType,
        trimLevelName: form.trimLevelName || null,
        trimLevelDescription: form.trimLevelDescription || null,
        engineVolume: parseFloat(form.engineVolume),
        fuelType: form.fuelType,
        transmissionType: form.transmissionType,
        driveType: form.driveType,
        images: toImageRequests(images.filter((image) => !image.pending)),
      }
      let saved
      if (selected) {
        const resolvedImages = await uploadCatalogImages('cars', selected.id, images)
        body.images = toImageRequests(resolvedImages)
        saved = await apiSave(`${API}/cars/${selected.id}`, 'PUT', body)
        setSnackbar({ open: true, message: 'Автомобиль обновлён', severity: 'success' })
      } else {
        saved = await apiSave(`${API}/cars`, 'POST', body)
        const resolvedImages = await uploadCatalogImages('cars', saved.id, images)
        if (resolvedImages.length > 0) {
          body.images = toImageRequests(resolvedImages)
          saved = await apiSave(`${API}/cars/${saved.id}`, 'PUT', body)
        }
        setSnackbar({ open: true, message: 'Автомобиль создан', severity: 'success' })
      }
      await deleteCatalogImages(removedImageKeys)
      setDialog(false)
      fetchItems()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    try {
      await apiSave(`${API}/cars/${selected.id}`, 'DELETE')
      await deleteCatalogImages((selected.images || []).map((image) => image.fileKey))
      setDeleteDialog(false)
      setSnackbar({ open: true, message: 'Автомобиль удалён', severity: 'success' })
      fetchItems()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const filtered = items.filter((i) => {
    if (!search) return true
    const q = search.toLowerCase()
    return i.brandName?.toLowerCase().includes(q) ||
      i.modelName?.toLowerCase().includes(q) ||
      i.bodyType?.toLowerCase().includes(q)
  })

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Автомобили</Typography>
      <Box className="page-toolbar">
        <TextField size="small" placeholder="Поиск по бренду, модели, кузову…" value={search} onChange={(e) => { setSearch(e.target.value); setPage(0) }} sx={{ minWidth: 280 }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>Создать</Button>
      </Box>
      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Фото</TableCell>
              <TableCell>Бренд</TableCell>
              <TableCell>Модель</TableCell>
              <TableCell>Поколение</TableCell>
              <TableCell>Год</TableCell>
              <TableCell>Цена</TableCell>
              <TableCell>Кузов</TableCell>
              <TableCell>Двигатель</TableCell>
              <TableCell>Привод</TableCell>
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={10} align="center">Загрузка…</TableCell></TableRow>
            ) : filtered.length === 0 ? (
              <TableRow><TableCell colSpan={10} align="center">Автомобили не найдены</TableCell></TableRow>
            ) : (
              filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((i) => (
                <TableRow
                  key={i.id}
                  hover
                  onClick={() => openEdit(i)}
                  sx={{ cursor: 'pointer' }}
                >
                  <TableCell>
                    {i.images?.[0]?.url ? <img className="catalog-thumbnail" src={i.images[0].url} alt="" /> : '—'}
                  </TableCell>
                  <TableCell>{i.brandName}</TableCell>
                  <TableCell>{i.modelName}</TableCell>
                  <TableCell>{i.generationName}</TableCell>
                  <TableCell>{i.year}</TableCell>
                  <TableCell>{i.price?.toLocaleString('ru-RU')} ₸</TableCell>
                  <TableCell>{i.bodyType}</TableCell>
                  <TableCell>{i.engineVolume}L {i.fuelType}</TableCell>
                  <TableCell>{i.driveType}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={(event) => { event.stopPropagation(); openEdit(i) }}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={(event) => { event.stopPropagation(); setSelected(i); setDeleteDialog(true) }} color="error"><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination component="div" count={filtered.length} page={page} onPageChange={(_, p) => setPage(p)} rowsPerPage={rowsPerPage} onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }} rowsPerPageOptions={[5, 10, 25]} />
      </TableContainer>

      <Dialog open={dialog} onClose={() => setDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>{selected ? 'Редактировать автомобиль' : 'Создать автомобиль'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <FormControl fullWidth>
              <InputLabel>Бренд</InputLabel>
              <Select value={form.brandId} label="Бренд" onChange={setField('brandId')} required>
                {brands.map((b) => <MenuItem key={b.id} value={b.id}>{b.name}</MenuItem>)}
              </Select>
            </FormControl>

            <FormControl fullWidth>
              <InputLabel>Модель</InputLabel>
              <Select value={form.modelId} label="Модель" onChange={setField('modelId')} required disabled={!form.brandId}>
                {models.map((m) => <MenuItem key={m.id} value={m.id}>{m.name}</MenuItem>)}
              </Select>
            </FormControl>

            <FormControl fullWidth>
              <InputLabel>Поколение</InputLabel>
              <Select value={form.generationId} label="Поколение" onChange={setField('generationId')} required disabled={!form.modelId}>
                {generations.map((g) => <MenuItem key={g.id} value={g.id}>{g.name} ({g.yearFrom}{g.yearTo ? `-${g.yearTo}` : '-н.в.'})</MenuItem>)}
              </Select>
            </FormControl>

            <Box className="form-fields__row">
              <TextField label="Год" type="number" value={form.year} onChange={setField('year')} required fullWidth slotProps={{ htmlInput: { min: 1900, max: 3000 } }} />
              <TextField label="Пробег (км)" type="number" value={form.mileage} onChange={setField('mileage')} required fullWidth slotProps={{ htmlInput: { min: 0 } }} />
            </Box>

            <TextField label="Цена (₸)" type="number" value={form.price} onChange={setField('price')} required fullWidth slotProps={{ htmlInput: { min: 0.01, step: 0.01 } }} />

            <TextField label="Тип кузова" value={form.bodyType} onChange={setField('bodyType')} required fullWidth placeholder="Седан, Хэтчбек, Внедорожник…" />

            <Box className="form-fields__row">
              <TextField label="Объём двигателя (L)" type="number" value={form.engineVolume} onChange={setField('engineVolume')} required fullWidth slotProps={{ htmlInput: { min: 0.1, max: 20, step: 0.1 } }} />
              <TextField label="Тип топлива" value={form.fuelType} onChange={setField('fuelType')} required fullWidth placeholder="Бензин, Дизель…" />
            </Box>

            <Box className="form-fields__row">
              <TextField label="Трансмиссия" value={form.transmissionType} onChange={setField('transmissionType')} required fullWidth placeholder="Механика, Автомат…" />
              <TextField label="Привод" value={form.driveType} onChange={setField('driveType')} required fullWidth placeholder="Передний, Задний, Полный" />
            </Box>

            <TextField label="Комплектация" value={form.trimLevelName} onChange={setField('trimLevelName')} fullWidth />
            <TextField label="Описание комплектации" value={form.trimLevelDescription} onChange={setField('trimLevelDescription')} multiline rows={2} fullWidth />
            <CatalogImagesEditor
              items={images}
              setItems={setImages}
              onRemoveStored={(key) => setRemovedImageKeys((current) => [...current, key])}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving || !form.brandId || !form.modelId || !form.generationId || !form.year || !form.price || !form.bodyType}>
            {saving ? '…' : 'Сохранить'}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить автомобиль</DialogTitle>
        <DialogContent><Typography>Удалить <strong>{selected?.brandName} {selected?.modelName}</strong>?</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog(false)}>Отмена</Button>
          <Button variant="contained" color="error" onClick={handleDelete}>Удалить</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  )
}
