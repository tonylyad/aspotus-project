import { useCallback, useEffect, useMemo, useState } from 'react'
import { Alert, Box, Chip, FormControl, InputLabel, MenuItem, Paper, Select, Snackbar, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TablePagination, TextField, Typography } from '@mui/material'

const API = '/orders/api/requests'
const statusLabels = { New: 'Новая', Processing: 'В работе', Completed: 'Завершена', Cancelled: 'Отменена' }
const statusColors = { New: 'info', Processing: 'warning', Completed: 'success', Cancelled: 'error' }
const typeLabels = { auto: 'Автомобиль', spare: 'Запчасть' }

async function authorizedFetch(url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${localStorage.getItem('token')}`, ...options.headers },
  })
  const body = await response.json().catch(() => null)
  if (!response.ok) throw new Error(body?.message || 'Не удалось выполнить запрос')
  return body
}

export default function Requests() {
  const [items, setItems] = useState([])
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState({ open: false, text: '', severity: 'success' })

  const load = useCallback(async () => {
    try { setItems(await authorizedFetch(API)) }
    catch (error) { setMessage({ open: true, text: error.message, severity: 'error' }) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase()
    if (!query) return items
    return items.filter((item) => [item.customerName, item.customerEmail, item.customerPhone, item.comment, typeLabels[item.type], statusLabels[item.status]]
      .some((value) => value?.toLowerCase().includes(query)))
  }, [items, search])

  const updateStatus = async (item, status) => {
    try {
      const updated = await authorizedFetch(`${API}/${item.id}/status`, { method: 'PATCH', body: JSON.stringify({ status }) })
      setItems((current) => current.map((entry) => entry.id === item.id ? updated : entry))
      setMessage({ open: true, text: 'Статус заявки обновлён', severity: 'success' })
    } catch (error) { setMessage({ open: true, text: error.message, severity: 'error' }) }
  }

  const formatDetails = (json) => {
    try {
      return Object.entries(JSON.parse(json || '{}')).filter(([, value]) => value)
        .map(([key, value]) => `${key}: ${value}`).join(', ') || '—'
    } catch { return '—' }
  }

  return <Box>
    <Typography variant="h5" fontWeight={600} mb={2}>Заявки клиентов</Typography>
    <Box className="page-toolbar"><TextField size="small" placeholder="Поиск по заявкам…" value={search}
      onChange={(event) => { setSearch(event.target.value); setPage(0) }} sx={{ minWidth: 300 }} /></Box>
    <TableContainer component={Paper} elevation={1}>
      <Table size="small">
        <TableHead><TableRow><TableCell>Дата</TableCell><TableCell>Тип</TableCell><TableCell>Клиент</TableCell><TableCell>Контакты</TableCell><TableCell>Параметры</TableCell><TableCell>Комментарий</TableCell><TableCell>Статус</TableCell></TableRow></TableHead>
        <TableBody>{loading
          ? <TableRow><TableCell colSpan={7} align="center">Загрузка…</TableCell></TableRow>
          : filtered.length === 0
            ? <TableRow><TableCell colSpan={7} align="center">Заявки не найдены</TableCell></TableRow>
            : filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((item) => <TableRow key={item.id} hover>
              <TableCell sx={{ whiteSpace: 'nowrap' }}>{new Date(item.createdAtUtc).toLocaleString('ru-RU')}</TableCell>
              <TableCell>{typeLabels[item.type] || item.type}</TableCell><TableCell>{item.customerName}</TableCell>
              <TableCell>{item.customerPhone}<br /><small>{item.customerEmail}</small></TableCell>
              <TableCell sx={{ maxWidth: 300 }}>{formatDetails(item.detailsJson)}</TableCell><TableCell>{item.comment || '—'}</TableCell>
              <TableCell sx={{ minWidth: 170 }}><FormControl size="small" fullWidth><InputLabel id={`request-status-${item.id}`}>Статус</InputLabel>
                <Select labelId={`request-status-${item.id}`} value={item.status} label="Статус" onChange={(event) => updateStatus(item, event.target.value)}
                  renderValue={(value) => <Chip size="small" label={statusLabels[value] || value} color={statusColors[value] || 'default'} />}>
                  {Object.entries(statusLabels).map(([value, label]) => <MenuItem key={value} value={value}>{label}</MenuItem>)}
                </Select></FormControl></TableCell>
            </TableRow>)}</TableBody>
      </Table>
      <TablePagination component="div" count={filtered.length} page={page} onPageChange={(_, value) => setPage(value)} rowsPerPage={rowsPerPage}
        onRowsPerPageChange={(event) => { setRowsPerPage(Number(event.target.value)); setPage(0) }} rowsPerPageOptions={[5, 10, 25]} />
    </TableContainer>
    <Snackbar open={message.open} autoHideDuration={4000} onClose={() => setMessage({ ...message, open: false })}>
      <Alert severity={message.severity} onClose={() => setMessage({ ...message, open: false })}>{message.text}</Alert>
    </Snackbar>
  </Box>
}
