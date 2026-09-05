import { act, renderHook, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { AuthProvider, useAuth } from './AuthContext.jsx'
import * as api from '../api/auth.js'

vi.mock('../api/auth.js', () => ({
  getProfile: vi.fn(),
  login: vi.fn(),
  register: vi.fn(),
}))

const wrapper = ({ children }) => <AuthProvider>{children}</AuthProvider>

describe('AuthProvider', () => {
  beforeEach(() => vi.clearAllMocks())

  it('завершает загрузку без сохранённого токена', async () => {
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    expect(result.current.isAuthenticated).toBe(false)
  })

  it('восстанавливает сохранённого пользователя', async () => {
    localStorage.setItem('user', JSON.stringify({ id: 'user-1', email: 'user@test.ru' }))
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    expect(result.current.user.email).toBe('user@test.ru')
  })

  it('удаляет повреждённого сохранённого пользователя', async () => {
    localStorage.setItem('user', '{broken')
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    expect(result.current.user).toBeNull()
    expect(localStorage.getItem('user')).toBeNull()
  })

  it('проверяет активную сессию и нормализует claims', async () => {
    localStorage.setItem('authToken', 'token')
    api.getProfile.mockResolvedValue({ data: [
      { type: 'sub', value: 'user-1' },
      { type: 'email', value: 'user@test.ru' },
      { type: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', value: 'Customer' },
    ] })
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isAuthenticated).toBe(true))
    expect(result.current.user).toMatchObject({ id: 'user-1', email: 'user@test.ru', role: 'Customer' })
  })

  it('сохраняет токен после входа и загружает профиль', async () => {
    api.login.mockResolvedValue({ data: { token: 'new-token' } })
    api.getProfile.mockResolvedValue({ data: { id: 'user-2', email: 'login@test.ru' } })
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    await act(() => result.current.login({ login: 'user', password: '123456' }))
    expect(localStorage.getItem('authToken')).toBe('new-token')
    expect(result.current.user.id).toBe('user-2')
  })

  it('сохраняет токен после регистрации и загружает профиль', async () => {
    api.register.mockResolvedValue({ data: { token: 'register-token' } })
    api.getProfile.mockResolvedValue({ data: { user: { id: 'user-3' } } })
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    await act(() => result.current.register({ email: 'new@test.ru' }))
    expect(localStorage.getItem('authToken')).toBe('register-token')
    expect(result.current.user.id).toBe('user-3')
  })

  it('очищает данные при выходе', async () => {
    localStorage.setItem('authToken', 'token')
    localStorage.setItem('user', JSON.stringify({ id: 'user-1' }))
    api.getProfile.mockResolvedValue({ data: { id: 'user-1' } })
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isAuthenticated).toBe(true))
    act(() => result.current.logout())
    expect(result.current.user).toBeNull()
    expect(localStorage.getItem('authToken')).toBeNull()
  })

  it('выходит при ошибке проверки сессии', async () => {
    localStorage.setItem('authToken', 'expired')
    api.getProfile.mockRejectedValue(new Error('unauthorized'))
    vi.spyOn(console, 'error').mockImplementation(() => {})
    const { result } = renderHook(() => useAuth(), { wrapper })
    await waitFor(() => expect(result.current.isLoading).toBe(false))
    expect(result.current.isAuthenticated).toBe(false)
    expect(localStorage.getItem('authToken')).toBeNull()
  })
})
