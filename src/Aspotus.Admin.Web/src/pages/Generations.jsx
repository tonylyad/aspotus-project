import { useState, useEffect, useCallback } from 'react'
import {
  Box, Button, TextField, Typography, Table, TableBody, TableCell,
  TableContainer, TableHead, TableRow, Paper, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions,
  Select, MenuItem, FormControl, InputLabel,
  Alert, Snackbar, TablePagination,
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

export default function Generations() {
  const [items, setItems] = useState([])
  const [models, setModels] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [dialog, setDialog] = useState(false)
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selected, setSelected] = useState(null)
  const [form, setForm] = useState({ name: '', yearFrom: '', yearTo: '', modelId: '' })
  const [saving, setSaving] = useState(false)
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchItems = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/generations`)
      setItems(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setLoading(false)
    }
  }, [])

  const fetchModels = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/models`)
      setModels(data)
    } catch { /* ignore */ }
  }, [])

  useEffect(() => { fetchModels() }, [fetchModels])
  useEffect(() => { fetchItems() }, [fetchItems])

  function setField(field) {
    return (e) => setForm((prev) => ({ ...prev, [field]: e.target.value }))
  }

  const openCreate = () => {
    setSelected(null)
    setForm({ name: '', yearFrom: '', yearTo: '', modelId: '' })
    setDialog(true)
  }

  const openEdit = (item) => {
    setSelected(item)
    setForm({
      name: item.name,
      yearFrom: String(item.yearFrom),
      yearTo: item.yearTo != null ? String(item.yearTo) : '',
      modelId: item.modelId,
    })
    setDialog(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      const body = {
        name: form.name,
        yearFrom: parseInt(form.yearFrom, 10),
        yearTo: form.yearTo ? parseInt(form.yearTo, 10) : null,
        modelId: form.modelId,
      }
      if (selected) {
        await apiSave(`${API}/generations/${selected.id}`, 'PUT', body)
        setSnackbar({ open: true, message: 'Поколение обновлено', severity: 'success' })
      } else {
        await apiSave(`${API}/generations`, 'POST', body)
        setSnackbar({ open: true, message: 'Поколение создано', severity: 'success' })
      }
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
      await apiSave(`${API}/generations/${selected.id}`, 'DELETE')
      setDeleteDialog(false)
      setSnackbar({ open: true, message: 'Поколение удалено', severity: 'success' })
      fetchItems()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const filtered = items.filter((i) => {
    if (!search) return true
    const q = search.toLowerCase()
    return i.name.toLowerCase().includes(q) || (i.modelName || '').toLowerCase().includes(q)
  })

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Поколения</Typography>
      <Box className="page-toolbar">
        <TextField size="small" placeholder="Поиск…" value={search} onChange={(e) => { setSearch(e.target.value); setPage(0) }} sx={{ minWidth: 280 }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>Создать</Button>
      </Box>
      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Название</TableCell>
              <TableCell>Годы</TableCell>
              <TableCell>Модель</TableCell>
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={4} align="center">Загрузка…</TableCell></TableRow>
            ) : filtered.length === 0 ? (
              <TableRow><TableCell colSpan={4} align="center">Поколения не найдены</TableCell></TableRow>
            ) : (
              filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((i) => (
                <TableRow key={i.id} hover>
                  <TableCell>{i.name}</TableCell>
                  <TableCell>{i.yearFrom}{i.yearTo ? ` — ${i.yearTo}` : ' — н.в.'}</TableCell>
                  <TableCell>{i.modelName}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => openEdit(i)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => { setSelected(i); setDeleteDialog(true) }} color="error"><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination component="div" count={filtered.length} page={page} onPageChange={(_, p) => setPage(p)} rowsPerPage={rowsPerPage} onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }} rowsPerPageOptions={[5, 10, 25]} />
      </TableContainer>

      <Dialog open={dialog} onClose={() => setDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selected ? 'Редактировать поколение' : 'Создать поколение'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <TextField label="Название" value={form.name} onChange={setField('name')} required fullWidth />
            <Box className="form-fields__row">
              <TextField label="Год с" type="number" value={form.yearFrom} onChange={setField('yearFrom')} required fullWidth slotProps={{ htmlInput: { min: 1900, max: 3000 } }} />
              <TextField label="Год по" type="number" value={form.yearTo} onChange={setField('yearTo')} fullWidth slotProps={{ htmlInput: { min: 1900, max: 3000 } }} />
            </Box>
            <FormControl fullWidth>
              <InputLabel>Модель</InputLabel>
              <Select value={form.modelId} label="Модель" onChange={setField('modelId')} required>
                {models.map((m) => (
                  <MenuItem key={m.id} value={m.id}>{m.name} ({m.brandName})</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving || !form.name.trim() || !form.yearFrom || !form.modelId}>
            {saving ? '…' : 'Сохранить'}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить поколение</DialogTitle>
        <DialogContent><Typography>Удалить <strong>{selected?.name}</strong>?</Typography></DialogContent>
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
