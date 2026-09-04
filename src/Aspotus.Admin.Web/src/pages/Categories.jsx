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

export default function Categories() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [dialog, setDialog] = useState(false)
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selected, setSelected] = useState(null)
  const [form, setForm] = useState({ name: '', parentCategoryId: '' })
  const [saving, setSaving] = useState(false)
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchItems = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/categories`)
      setItems(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchItems() }, [fetchItems])

  function setField(field) {
    return (e) => setForm((prev) => ({ ...prev, [field]: e.target.value }))
  }

  const openCreate = () => {
    setSelected(null)
    setForm({ name: '', parentCategoryId: '' })
    setDialog(true)
  }

  const openEdit = (item) => {
    setSelected(item)
    setForm({ name: item.name, parentCategoryId: item.parentCategoryId || '' })
    setDialog(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      const body = {
        name: form.name,
        parentCategoryId: form.parentCategoryId || null,
      }
      if (selected) {
        await apiSave(`${API}/categories/${selected.id}`, 'PUT', body)
        setSnackbar({ open: true, message: 'Категория обновлена', severity: 'success' })
      } else {
        await apiSave(`${API}/categories`, 'POST', body)
        setSnackbar({ open: true, message: 'Категория создана', severity: 'success' })
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
      await apiSave(`${API}/categories/${selected.id}`, 'DELETE')
      setDeleteDialog(false)
      setSnackbar({ open: true, message: 'Категория удалена', severity: 'success' })
      fetchItems()
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const filtered = items.filter((i) => {
    if (!search) return true
    return i.name.toLowerCase().includes(search.toLowerCase())
  })

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Категории запчастей</Typography>
      <Box className="page-toolbar">
        <TextField size="small" placeholder="Поиск…" value={search} onChange={(e) => { setSearch(e.target.value); setPage(0) }} sx={{ minWidth: 280 }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>Создать</Button>
      </Box>
      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Название</TableCell>
              <TableCell>Родительская категория</TableCell>
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={3} align="center">Загрузка…</TableCell></TableRow>
            ) : filtered.length === 0 ? (
              <TableRow><TableCell colSpan={3} align="center">Категории не найдены</TableCell></TableRow>
            ) : (
              filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((i) => {
                const parent = items.find((p) => p.id === i.parentCategoryId)
                return (
                  <TableRow key={i.id} hover>
                    <TableCell>{i.name}</TableCell>
                    <TableCell>{parent?.name || '—'}</TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => openEdit(i)}><EditIcon fontSize="small" /></IconButton>
                      <IconButton size="small" onClick={() => { setSelected(i); setDeleteDialog(true) }} color="error"><DeleteIcon fontSize="small" /></IconButton>
                    </TableCell>
                  </TableRow>
                )
              })
            )}
          </TableBody>
        </Table>
        <TablePagination component="div" count={filtered.length} page={page} onPageChange={(_, p) => setPage(p)} rowsPerPage={rowsPerPage} onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }} rowsPerPageOptions={[5, 10, 25]} />
      </TableContainer>

      <Dialog open={dialog} onClose={() => setDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selected ? 'Редактировать категорию' : 'Создать категорию'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <TextField label="Название" value={form.name} onChange={setField('name')} required fullWidth />
            <FormControl fullWidth>
              <InputLabel>Родительская категория</InputLabel>
              <Select value={form.parentCategoryId} label="Родительская категория" onChange={setField('parentCategoryId')}>
                <MenuItem value="">— нет —</MenuItem>
                {items.filter((p) => !selected || p.id !== selected.id).map((p) => (
                  <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving || !form.name.trim()}>{saving ? '…' : 'Сохранить'}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить категорию</DialogTitle>
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
