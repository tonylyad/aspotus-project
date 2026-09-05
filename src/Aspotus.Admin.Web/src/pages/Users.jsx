import { useState, useEffect } from 'react'
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
  Chip,
  TablePagination,
} from '@mui/material'
import { Edit as EditIcon, Delete as DeleteIcon, Add as AddIcon } from '@mui/icons-material'

const roleOptions = [
  { value: 'Customer', label: 'Покупатель' },
  { value: 'ContentModerator', label: 'Модератор контента' },
  { value: 'Operator', label: 'Оператор заказов' },
  { value: 'Admin', label: 'Администратор' },
]

const roleLabels = Object.fromEntries(roleOptions.map((role) => [role.value, role.label]))

function formatPhone(value) {
  const digits = value.replace(/\D/g, '').replace(/^7/, '').slice(0, 10)
  if (!digits) return ''
  const p1 = digits.slice(0, 3)
  const p2 = digits.slice(3, 6)
  const p3 = digits.slice(6, 10)
  let result = '+7'
  if (p1) result += ` ${p1}`
  if (p2) result += ` ${p2}`
  if (p3) result += ` ${p3}`
  return result
}

function isValidPhone(value) {
  if (!value) return true
  const digits = value.replace(/\D/g, '').replace(/^7/, '')
  return digits.length === 10
}

export default function Users() {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)

  const [editDialog, setEditDialog] = useState(false)
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selectedUser, setSelectedUser] = useState(null)
  const [formData, setFormData] = useState({ email: '', login: '', password: '', fullName: '', phoneNumber: '', roleName: '' })
  const [phoneError, setPhoneError] = useState(false)

  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' })
  const [showPassword, setShowPassword] = useState(false)

  const [refreshKey, setRefreshKey] = useState(0)

  const token = localStorage.getItem('token')
  const headers = { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }

  useEffect(() => {
    let cancelled = false
    setLoading(true)

    const params = new URLSearchParams()
    if (search) params.set('search', search)
    if (roleFilter) params.set('role', roleFilter)

    fetch(`/api/users?${params}`, { headers })
      .then((res) => {
        if (!res.ok) throw new Error('Не удалось загрузить список пользователей')
        return res.json()
      })
      .then((data) => { if (!cancelled) setUsers(data) })
      .catch((err) => { if (!cancelled) setSnackbar({ open: true, message: err.message, severity: 'error' }) })
      .finally(() => { if (!cancelled) setLoading(false) })

    return () => { cancelled = true }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search, roleFilter, refreshKey])

  const openCreate = () => {
    setSelectedUser(null)
    setFormData({ email: '', login: '', password: '', fullName: '', phoneNumber: '', roleName: 'Customer' })
    setPhoneError(false)
    setShowPassword(false)
    setEditDialog(true)
  }

  const openEdit = (user) => {
    setSelectedUser(user)
    setFormData({
      email: user.email,
      fullName: user.fullName,
      phoneNumber: formatPhone(user.phoneNumber || ''),
      roleName: user.roles[0] || 'Customer',
    })
    setPhoneError(false)
    setEditDialog(true)
  }

  const handleSave = async () => {
    if (!isValidPhone(formData.phoneNumber)) {
      setPhoneError(true)
      return
    }
    setPhoneError(false)

    try {
      const url = selectedUser
        ? `/api/users/${selectedUser.id}`
        : '/api/users'
      const method = selectedUser ? 'PUT' : 'POST'

      const body = selectedUser
        ? formData
        : formData

      if (!selectedUser && (!formData.password || formData.password.length < 6)) {
        throw new Error('Пароль должен содержать минимум 6 символов')
      }

      if (!selectedUser && !formData.login.trim()) {
        throw new Error('Укажите логин пользователя')
      }

      const res = await fetch(url, { method, headers, body: JSON.stringify(body) })
      if (!res.ok) {
        const data = await res.json()
        throw new Error(data.message || 'Ошибка сохранения')
      }

      setEditDialog(false)
      setSnackbar({ open: true, message: selectedUser ? 'Пользователь обновлён' : 'Пользователь создан', severity: 'success' })
      setRefreshKey((k) => k + 1)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const openDelete = (user) => {
    setSelectedUser(user)
    setDeleteDialog(true)
  }

  const handleDelete = async () => {
    try {
      const res = await fetch(`/api/users/${selectedUser.id}`, { method: 'DELETE', headers })
      if (!res.ok) throw new Error('Не удалось удалить пользователя')

      setDeleteDialog(false)
      setSnackbar({ open: true, message: 'Пользователь удалён', severity: 'success' })
      setRefreshKey((k) => k + 1)
    } catch (err) {
      setSnackbar({ open: true, message: err.message, severity: 'error' })
    }
  }

  const filteredUsers = users

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={2}>Пользователи</Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        Управление учётными записями, контактными данными и ролями доступа.
      </Typography>

      <Box className="page-toolbar">
        <TextField
          size="small"
          placeholder="Поиск по логину, email, ФИО"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(0) }}
          sx={{ minWidth: 280 }}
        />
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Роль</InputLabel>
          <Select
            value={roleFilter}
            label="Роль"
            onChange={(e) => { setRoleFilter(e.target.value); setPage(0) }}
          >
            <MenuItem value="">Все</MenuItem>
            {roleOptions.map((role) => (
              <MenuItem key={role.value} value={role.value}>{role.label}</MenuItem>
            ))}
          </Select>
        </FormControl>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          Создать
        </Button>
      </Box>

      <TableContainer component={Paper} elevation={1}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Логин</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>ФИО</TableCell>
              <TableCell>Телефон</TableCell>
              <TableCell>Роли</TableCell>
              <TableCell align="right">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={6} align="center">Загрузка…</TableCell>
              </TableRow>
            ) : filteredUsers.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">Пользователи не найдены</TableCell>
              </TableRow>
            ) : (
              filteredUsers.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.login}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.fullName}</TableCell>
                  <TableCell>{user.phoneNumber}</TableCell>
                  <TableCell>
                    <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
                      {user.roles.map((role) => (
                        <Chip key={role} label={roleLabels[role] ?? role} size="small" color="primary" variant="outlined" />
                      ))}
                    </Box>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => openEdit(user)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" onClick={() => openDelete(user)} color="error"><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={filteredUsers.length}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0) }}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </TableContainer>

      <Dialog open={editDialog} onClose={() => setEditDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selectedUser ? 'Редактировать пользователя' : 'Создать пользователя'}</DialogTitle>
        <DialogContent sx={{ px: 3, py: 2 }}>
          <Box className="form-fields">
            <FormControl fullWidth>
              <InputLabel>Роль</InputLabel>
              <Select
                value={formData.roleName}
                label="Роль"
                onChange={(e) => setFormData({ ...formData, roleName: e.target.value })}
              >
                {roleOptions.map((role) => (
                  <MenuItem key={role.value} value={role.value}>{role.label}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <TextField
              label="Email"
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              required
              fullWidth
            />

            {!selectedUser && (
              <TextField
                label="Логин"
                value={formData.login}
                onChange={(e) => setFormData({ ...formData, login: e.target.value })}
                required
                fullWidth
                helperText="Используется для входа"
              />
            )}

            {!selectedUser && (
              <TextField
                label="Пароль"
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                required
                fullWidth
                helperText="Минимум 6 символов"
              />
            )}

            {!selectedUser && (
              <Box>
                <label>
                  <input
                    type="checkbox"
                    checked={showPassword}
                    onChange={(e) => setShowPassword(e.target.checked)}
                  />{' '}
                  Показать пароль
                </label>
              </Box>
            )}

            <TextField
              label="ФИО"
              value={formData.fullName}
              onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
              required
              fullWidth
            />

            <TextField
              label="Телефон"
              value={formData.phoneNumber}
              error={phoneError}
              helperText={phoneError ? 'Формат: +7 999 999 9999 (10 цифр)' : 'Формат +7 999 999 9999'}
              onChange={(e) => {
                setPhoneError(false)
                setFormData({ ...formData, phoneNumber: formatPhone(e.target.value) })
              }}
              fullWidth
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditDialog(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSave}>Сохранить</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Удалить пользователя</DialogTitle>
        <DialogContent>
          <Typography>
            Вы уверены, что хотите удалить пользователя <strong>{selectedUser?.login}</strong>?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog(false)}>Отмена</Button>
          <Button variant="contained" color="error" onClick={handleDelete}>Удалить</Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  )
}
