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
} from '@mui/material'
import { Edit as EditIcon, Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material'

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

function CrudTable({ title, items, loading, search, onSearchChange, columns, onEdit, onDelete, onCreate }) {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)

  const filtered = items.filter((i) => {
    if (!search) return true
    const q = search.toLowerCase()
    return columns.some((c) => String(i[c.field] || '').toLowerCase().includes(q))
  })

  return (
    <Box>
      <Typography variant="h6" fontWeight={600} mb={1.5}>{title}</Typography>
      <Box className="page-toolbar page-toolbar--compact">
        <TextField size="small" placeholder="Поиск…" value={search} onChange={(e) => { onSearchChange(e.target.value); setPage(0) }} sx={{ flex: 1 }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={onCreate}>Создать</Button>
      </Box>
      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              {columns.map((c) => <TableCell key={c.field}>{c.label}</TableCell>)}
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={columns.length + 1} align="center">Загрузка…</TableCell></TableRow>
            ) : filtered.length === 0 ? (
              <TableRow><TableCell colSpan={columns.length + 1} align="center">Нет данных</TableCell></TableRow>
            ) : (
              filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((i) => (
                <TableRow key={i.id} hover>
                  {columns.map((c) => <TableCell key={c.field}>{i[c.field]}</TableCell>)}
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => onEdit(i)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => onDelete(i)} color="error"><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={filtered.length}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </TableContainer>
    </Box>
  )
}

export default function Brands() {
  const [brands, setBrands] = useState([])
  const [models, setModels] = useState([])
  const [brandsLoading, setBrandsLoading] = useState(true)
  const [modelsLoading, setModelsLoading] = useState(true)
  const [brandSearch, setBrandSearch] = useState('')
  const [modelSearch, setModelSearch] = useState('')

  const [brandDialog, setBrandDialog] = useState(false)
  const [brandDelete, setBrandDelete] = useState(false)
  const [selectedBrand, setSelectedBrand] = useState(null)
  const [brandName, setBrandName] = useState('')

  const [modelDialog, setModelDialog] = useState(false)
  const [modelDelete, setModelDelete] = useState(false)
  const [selectedModel, setSelectedModel] = useState(null)
  const [modelName, setModelName] = useState('')
  const [modelBrandId, setModelBrandId] = useState('')

  const [saving, setSaving] = useState(false)
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchBrands = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/brands`)
      setBrands(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setBrandsLoading(false)
    }
  }, [])

  const fetchModels = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/models`)
      setModels(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setModelsLoading(false)
    }
  }, [])

  useEffect(() => { fetchBrands() }, [fetchBrands])
  useEffect(() => { fetchModels() }, [fetchModels])

  // Brand CRUD
  const openBrandCreate = () => { setSelectedBrand(null); setBrandName(''); setBrandDialog(true) }
  const openBrandEdit = (b) => { setSelectedBrand(b); setBrandName(b.name); setBrandDialog(true) }
  const openBrandDelete = (b) => { setSelectedBrand(b); setBrandDelete(true) }

  const saveBrand = async () => {
    setSaving(true)
    try {
      if (selectedBrand) {
        await apiSave(`${API}/brands/${selectedBrand.id}`, 'PUT', { name: brandName })
        setSnackbar({ open: true, message: 'Бренд обновлён', severity: 'success' })
      } else {
        await apiSave(`${API}/brands`, 'POST', { name: brandName })
        setSnackbar({ open: true, message: 'Бренд создан', severity: 'success' })
      }
      setBrandDialog(false)
      fetchBrands()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally { setSaving(false) }
  }

  const deleteBrand = async () => {
    try {
      await apiSave(`${API}/brands/${selectedBrand.id}`, 'DELETE')
      setBrandDelete(false)
      setSnackbar({ open: true, message: 'Бренд удалён', severity: 'success' })
      fetchBrands()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  // Model CRUD
  const openModelCreate = (prefilledBrandId) => {
    setSelectedModel(null)
    setModelName('')
    setModelBrandId(prefilledBrandId || '')
    setModelDialog(true)
  }
  const openModelEdit = (m) => {
    setSelectedModel(m)
    setModelName(m.name)
    setModelBrandId(m.brandId)
    setModelDialog(true)
  }
  const openModelDelete = (m) => { setSelectedModel(m); setModelDelete(true) }

  const saveModel = async () => {
    setSaving(true)
    try {
      const body = { name: modelName, brandId: modelBrandId }
      if (selectedModel) {
        await apiSave(`${API}/models/${selectedModel.id}`, 'PUT', body)
        setSnackbar({ open: true, message: 'Модель обновлена', severity: 'success' })
      } else {
        await apiSave(`${API}/models`, 'POST', body)
        setSnackbar({ open: true, message: 'Модель создана', severity: 'success' })
      }
      setModelDialog(false)
      fetchModels()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally { setSaving(false) }
  }

  const deleteModel = async () => {
    try {
      await apiSave(`${API}/models/${selectedModel.id}`, 'DELETE')
      setModelDelete(false)
      setSnackbar({ open: true, message: 'Модель удалена', severity: 'success' })
      fetchModels()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const modelColumns = [
    { field: 'name', label: 'Название' },
    { field: 'brandName', label: 'Бренд' },
  ]

  return (
    <Box>
      <Box sx={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
        <Box sx={{ flex: '1 1 50%', minWidth: 0 }}>
          <CrudTable
            title="Бренды"
            items={brands}
            loading={brandsLoading}
            search={brandSearch}
            onSearchChange={setBrandSearch}
            columns={[{ field: 'name', label: 'Название' }]}
            onEdit={openBrandEdit}
            onDelete={openBrandDelete}
            onCreate={openBrandCreate}
          />
        </Box>
        <Box sx={{ flex: '1 1 50%', minWidth: 0 }}>
          <CrudTable
            title="Модели"
            items={models}
            loading={modelsLoading}
            search={modelSearch}
            onSearchChange={setModelSearch}
            columns={modelColumns}
            onEdit={openModelEdit}
            onDelete={openModelDelete}
            onCreate={() => openModelCreate('')}
          />
        </Box>
      </Box>

      {/* Brand dialogs */}
      <Dialog open={brandDialog} onClose={() => setBrandDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selectedBrand ? 'Редактировать бренд' : 'Создать бренд'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <TextField label="Название" value={brandName} onChange={(e) => setBrandName(e.target.value)} required fullWidth />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setBrandDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={saveBrand} disabled={saving || !brandName.trim()}>{saving ? '…' : 'Сохранить'}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={brandDelete} onClose={() => setBrandDelete(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить бренд</DialogTitle>
        <DialogContent><Typography>Удалить <strong>{selectedBrand?.name}</strong>?</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setBrandDelete(false)}>Отмена</Button>
          <Button variant="contained" color="error" onClick={deleteBrand}>Удалить</Button>
        </DialogActions>
      </Dialog>

      {/* Model dialogs */}
      <Dialog open={modelDialog} onClose={() => setModelDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selectedModel ? 'Редактировать модель' : 'Создать модель'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <TextField label="Название" value={modelName} onChange={(e) => setModelName(e.target.value)} required fullWidth />
            <FormControl fullWidth>
              <InputLabel>Бренд</InputLabel>
              <Select value={modelBrandId} label="Бренд" onChange={(e) => setModelBrandId(e.target.value)} required>
                {brands.map((b) => <MenuItem key={b.id} value={b.id}>{b.name}</MenuItem>)}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setModelDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={saveModel} disabled={saving || !modelName.trim() || !modelBrandId}>{saving ? '…' : 'Сохранить'}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={modelDelete} onClose={() => setModelDelete(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить модель</DialogTitle>
        <DialogContent><Typography>Удалить <strong>{selectedModel?.name}</strong>?</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setModelDelete(false)}>Отмена</Button>
          <Button variant="contained" color="error" onClick={deleteModel}>Удалить</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  )
}
