import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  TextField,
  Button,
  Typography,
  Alert,
  Paper,
} from '@mui/material'
import logo from '../assets/logo.png'

export default function Login() {
  const navigate = useNavigate()
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const res = await fetch('/api/auth/admin-login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ login, password }),
      })

      const data = await res.json()

      if (!res.ok) {
        setError(data.message || data.title || 'Ошибка входа')
        return
      }

      localStorage.setItem('token', data.token)
      localStorage.setItem('fullName', data.fullName)
      navigate('/', { replace: true })
    } catch {
      setError('Не удалось подключиться к серверу')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
        bgcolor: 'background.default',
        p: 2,
      }}
    >
      <Box
        component="img"
        src={logo}
        alt="Aspotus"
        sx={{ width: '100%', maxWidth: 400, mb: 3 }}
      />

      <Paper
        component="form"
        onSubmit={handleSubmit}
        sx={{ width: '100%', maxWidth: 400, p: 4 }}
        elevation={1}
      >
        <Typography variant="h5" fontWeight={700} textAlign="center" gutterBottom>
          Aspotus Admin
        </Typography>
        <Typography
          variant="body2"
          color="text.secondary"
          textAlign="center"
          sx={{ mb: 3 }}
        >
          Войдите в панель управления
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2, fontSize: 13 }}>
            {error}
          </Alert>
        )}

        <TextField
          label="Логин"
          type="text"
          value={login}
          onChange={(e) => setLogin(e.target.value)}
          placeholder="yourlogin"
          required
          autoFocus
          fullWidth
          size="small"
          sx={{ mb: 2 }}
        />

        <TextField
          label="Пароль"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="••••••••"
          required
          fullWidth
          size="small"
          inputProps={{ autoComplete: 'new-password' }}
          sx={{ mb: 3 }}
        />

        <Button
          type="submit"
          variant="contained"
          fullWidth
          disabled={loading}
          size="medium"
        >
          {loading ? 'Вход…' : 'Войти'}
        </Button>
      </Paper>
    </Box>
  )
}
