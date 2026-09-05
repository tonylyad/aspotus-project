import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Login from './Login.jsx'

const renderLogin = () => render(<MemoryRouter initialEntries={['/login']}><Routes>
  <Route path="/login" element={<Login />} />
  <Route path="/" element={<div>Панель открыта</div>} />
</Routes></MemoryRouter>)

const fillAndSubmit = () => {
  fireEvent.change(screen.getByLabelText(/^Логин/), { target: { value: 'admin' } })
  fireEvent.change(screen.getByLabelText(/^Пароль/), { target: { value: '123456' } })
  fireEvent.click(screen.getByRole('button', { name: 'Войти' }))
}

describe('Login', () => {
  beforeEach(() => vi.stubGlobal('fetch', vi.fn()))

  it('отправляет введённые учётные данные', async () => {
    fetch.mockResolvedValue({ ok: true, json: async () => ({ token: 'jwt', fullName: 'Администратор' }) })
    renderLogin()
    fillAndSubmit()
    await screen.findByText('Панель открыта')
    expect(fetch).toHaveBeenCalledWith('/api/auth/admin-login', expect.objectContaining({
      method: 'POST', body: JSON.stringify({ login: 'admin', password: '123456' }),
    }))
  })

  it('сохраняет токен и имя после успешного входа', async () => {
    fetch.mockResolvedValue({ ok: true, json: async () => ({ token: 'jwt', fullName: 'Администратор' }) })
    renderLogin()
    fillAndSubmit()
    await screen.findByText('Панель открыта')
    expect(localStorage.getItem('token')).toBe('jwt')
    expect(localStorage.getItem('fullName')).toBe('Администратор')
  })

  it('показывает сообщение API при неправильном пароле', async () => {
    fetch.mockResolvedValue({ ok: false, json: async () => ({ message: 'Неверный пароль' }) })
    renderLogin()
    fillAndSubmit()
    expect(await screen.findByText('Неверный пароль')).toBeInTheDocument()
  })

  it('показывает ошибку подключения', async () => {
    fetch.mockRejectedValue(new Error('offline'))
    renderLogin()
    fillAndSubmit()
    await waitFor(() => expect(screen.getByRole('alert')).toBeInTheDocument())
    expect(screen.getByRole('alert')).toHaveTextContent('Не удалось подключиться к серверу')
  })
})
