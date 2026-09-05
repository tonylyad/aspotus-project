import { fireEvent, render, screen, waitFor, within } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Users from './Users.jsx'

const users = [{
  id: 'user-1', login: 'ivan', email: 'ivan@test.ru', fullName: 'Иван Иванов',
  phoneNumber: '+79990000000', roles: ['ContentModerator'],
}]

const response = (body, ok = true) => ({ ok, json: async () => body })

describe('Users', () => {
  beforeEach(() => {
    localStorage.setItem('token', 'jwt')
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(response(users)))
  })

  it('загружает пользователей и переводит названия ролей', async () => {
    render(<Users />)
    expect(await screen.findByText('ivan')).toBeInTheDocument()
    expect(screen.getByText('Модератор контента')).toBeInTheDocument()
    expect(fetch).toHaveBeenCalledWith('/api/users?', expect.objectContaining({ headers: expect.objectContaining({ Authorization: 'Bearer jwt' }) }))
  })

  it('передаёт поисковую строку в API', async () => {
    render(<Users />)
    await screen.findByText('ivan')
    fireEvent.change(screen.getByPlaceholderText('Поиск по логину, email, ФИО'), { target: { value: 'ivan' } })
    await waitFor(() => expect(fetch).toHaveBeenLastCalledWith('/api/users?search=ivan', expect.anything()))
  })

  it('показывает ошибку загрузки', async () => {
    fetch.mockResolvedValue(response({}, false))
    render(<Users />)
    expect(await screen.findByRole('alert')).toHaveTextContent('Не удалось загрузить список пользователей')
  })

  it('не создаёт пользователя с коротким паролем', async () => {
    render(<Users />)
    await screen.findByText('ivan')
    fireEvent.click(screen.getByRole('button', { name: 'Создать' }))
    fireEvent.change(screen.getByLabelText(/^Пароль/), { target: { value: '123' } })
    fireEvent.click(screen.getByRole('button', { name: 'Сохранить' }))
    expect(await screen.findByText(/минимум 6 символов/)).toBeInTheDocument()
  })

  it('удаляет выбранного пользователя и обновляет список', async () => {
    fetch.mockImplementation((url, options = {}) => Promise.resolve(
      options.method === 'DELETE' ? response(null) : response(users),
    ))
    const { container } = render(<Users />)
    await screen.findByText('ivan')
    const rowButtons = container.querySelectorAll('tbody button')
    fireEvent.click(rowButtons[1])
    const dialog = await screen.findByRole('dialog')
    fireEvent.click(within(dialog).getByRole('button', { name: 'Удалить' }))
    await waitFor(() => expect(fetch).toHaveBeenCalledWith('/api/users/user-1', expect.objectContaining({ method: 'DELETE' })))
  })
})
