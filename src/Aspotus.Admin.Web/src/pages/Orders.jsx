import { useState, useEffect, useCallback } from 'react'
import {
  Box, Typography, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, Paper, IconButton, TextField,
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Alert, Snackbar, TablePagination, Chip, FormControl, InputLabel, MenuItem, Select,
} from '@mui/material'
import { Visibility as ViewIcon } from '@mui/icons-material'

const API = '/orders/api'

async function apiGet(url) {
  const token = localStorage.getItem('token')
  const res = await fetch(url, { headers: { Authorization: `Bearer ${token}` } })
  if (!res.ok) throw new Error('Ошибка загрузки')
  return res.json()
}

async function apiPatch(url, body) {
  const token = localStorage.getItem('token')
  const res = await fetch(url, {
    method: 'PATCH',
    headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
  const data = await res.json().catch(() => null)
  if (!res.ok) throw new Error(data?.message || 'Не удалось изменить статус заказа')
  return data
}

const statusColors = {
  Created: 'info',
  Processing: 'warning',
  Completed: 'success',
  Cancelled: 'error',
}

const statusLabels = {
  Created: 'Создан',
  Processing: 'В обработке',
  Completed: 'Завершён',
  Cancelled: 'Отменён',
}

const typeLabels = {
  Part: 'Запчасти',
  Car: 'Автомобиль',
}

export default function Orders() {
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [detailOrder, setDetailOrder] = useState(null)
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })

  const fetchOrders = useCallback(async () => {
    try {
      const data = await apiGet(`${API}/orders`)
      setOrders(data)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchOrders() }, [fetchOrders])

  const updateStatus = async (status) => {
    try {
      const updated = await apiPatch(`${API}/orders/${detailOrder.id}/status`, { status })
      setOrders((current) => current.map((order) => order.id === updated.id ? updated : order))
      setDetailOrder(updated)
      setSnackbar({ open: true, message: 'Статус заказа обновлён', severity: 'success' })
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const availableStatuses = detailOrder?.status === 'Created'
    ? ['Created', 'Processing', 'Cancelled']
    : detailOrder?.status === 'Processing'
      ? ['Processing', 'Completed', 'Cancelled']
      : [detailOrder?.status].filter(Boolean)

  function formatDate(utcStr) {
    if (!utcStr) return '—'
    return new Date(utcStr).toLocaleString('ru-RU')
  }

  function formatPrice(val) {
    return Number(val).toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' ₽'
  }

  const filtered = orders.filter((o) => {
    if (!search) return true
    const q = search.toLowerCase()
    return (
      o.customerName?.toLowerCase().includes(q) ||
      o.customerEmail?.toLowerCase().includes(q) ||
      statusLabels[o.status]?.toLowerCase().includes(q) ||
      typeLabels[o.orderType]?.toLowerCase().includes(q)
    )
  })

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Заказы</Typography>
      <Box className="page-toolbar">
        <TextField size="small" placeholder="Поиск по заказам…" value={search} onChange={(e) => { setSearch(e.target.value); setPage(0) }} sx={{ minWidth: 280 }} />
      </Box>
      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Дата</TableCell>
              <TableCell>Клиент</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Тип</TableCell>
              <TableCell>Статус</TableCell>
              <TableCell align="right">Сумма</TableCell>
              <TableCell align="right" />
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={7} align="center">Загрузка…</TableCell></TableRow>
            ) : filtered.length === 0 ? (
              <TableRow><TableCell colSpan={7} align="center">Заказы не найдены</TableCell></TableRow>
            ) : (
              filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((o) => (
                <TableRow key={o.id} hover>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>{formatDate(o.createdAtUtc)}</TableCell>
                  <TableCell>{o.customerName}</TableCell>
                  <TableCell>{o.customerEmail}</TableCell>
                  <TableCell>{typeLabels[o.orderType] || o.orderType}</TableCell>
                  <TableCell>
                    <Chip label={statusLabels[o.status] || o.status} color={statusColors[o.status] || 'default'} size="small" />
                  </TableCell>
                  <TableCell align="right" sx={{ whiteSpace: 'nowrap' }}>{formatPrice(o.totalAmount)}</TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => setDetailOrder(o)}><ViewIcon fontSize="small" /></IconButton>
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

      <Dialog open={!!detailOrder} onClose={() => setDetailOrder(null)} maxWidth="md" fullWidth>
        <DialogTitle>
          Заказ #{detailOrder?.id?.slice(0, 8)}
          <Chip
            label={statusLabels[detailOrder?.status] || detailOrder?.status}
            color={statusColors[detailOrder?.status] || 'default'}
            size="small"
            sx={{ ml: 1.5 }}
          />
        </DialogTitle>
        <DialogContent dividers>
          {detailOrder && (
            <Box display="flex" flexDirection="column" gap={2}>
              <Box display="flex" gap={4} flexWrap="wrap">
                <Box>
                  <Typography variant="caption" color="text.secondary">Клиент</Typography>
                  <Typography>{detailOrder.customerName}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">Email</Typography>
                  <Typography>{detailOrder.customerEmail}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">Телефон</Typography>
                  <Typography>{detailOrder.customerPhone || '—'}</Typography>
                </Box>
              </Box>
              <Box display="flex" gap={4} flexWrap="wrap">
                <Box>
                  <Typography variant="caption" color="text.secondary">Адрес доставки</Typography>
                  <Typography>{detailOrder.deliveryAddress || '—'}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">Тип заказа</Typography>
                  <Typography>{typeLabels[detailOrder.orderType] || detailOrder.orderType}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">Дата создания</Typography>
                  <Typography>{formatDate(detailOrder.createdAtUtc)}</Typography>
                </Box>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">Сумма заказа</Typography>
                <Typography variant="h6">{formatPrice(detailOrder.totalAmount)}</Typography>
              </Box>
              {detailOrder.partItems?.length > 0 && (
                <Box>
                  <Typography variant="subtitle2" gutterBottom>Запчасти</Typography>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Название</TableCell>
                        <TableCell>Артикул</TableCell>
                        <TableCell align="right">Цена</TableCell>
                        <TableCell align="right">Кол-во</TableCell>
                        <TableCell align="right">Сумма</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {detailOrder.partItems.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>{item.partName}</TableCell>
                          <TableCell>{item.partArticle}</TableCell>
                          <TableCell align="right">{formatPrice(item.unitPrice)}</TableCell>
                          <TableCell align="right">{item.quantity}</TableCell>
                          <TableCell align="right">{formatPrice(item.totalPrice)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </Box>
              )}
              {detailOrder.carItems?.length > 0 && (
                <Box>
                  <Typography variant="subtitle2" gutterBottom>Автомобили</Typography>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Марка</TableCell>
                        <TableCell>Модель</TableCell>
                        <TableCell>Поколение</TableCell>
                        <TableCell align="right">Год</TableCell>
                        <TableCell align="right">Цена</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {detailOrder.carItems.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>{item.brandName}</TableCell>
                          <TableCell>{item.modelName}</TableCell>
                          <TableCell>{item.generationName}</TableCell>
                          <TableCell align="right">{item.year}</TableCell>
                          <TableCell align="right">{formatPrice(item.price)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </Box>
              )}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          {detailOrder && (
            <FormControl size="small" sx={{ minWidth: 190, mr: 'auto' }}>
              <InputLabel>Статус заказа</InputLabel>
              <Select value={detailOrder.status} label="Статус заказа" onChange={(event) => updateStatus(event.target.value)}>
                {availableStatuses.map((status) => <MenuItem key={status} value={status}>{statusLabels[status]}</MenuItem>)}
              </Select>
            </FormControl>
          )}
          <Button onClick={() => setDetailOrder(null)}>Закрыть</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={4000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  )
}
