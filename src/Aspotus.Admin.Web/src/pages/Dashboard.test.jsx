import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Dashboard from './Dashboard.jsx'
import { isAdmin, isContentModerator, isOperator } from '../utils/auth.js'

vi.mock('../utils/auth.js', () => ({ isAdmin: vi.fn(), isContentModerator: vi.fn(), isOperator: vi.fn() }))

describe('Dashboard', () => {
  beforeEach(() => {
    isAdmin.mockReturnValue(false)
    isContentModerator.mockReturnValue(false)
    isOperator.mockReturnValue(false)
  })

  it('загружает реальные количества заказов, пользователей и операторов', async () => {
    isAdmin.mockReturnValue(true)
    vi.stubGlobal('fetch', vi.fn()
      .mockResolvedValueOnce({ ok: true, json: async () => [{}, {}] })
      .mockResolvedValueOnce({ ok: true, json: async () => [
        { roles: ['Admin'] }, { roles: ['Operator'] }, { roles: ['Customer'] },
      ] }))
    render(<MemoryRouter><Dashboard /></MemoryRouter>)
    await waitFor(() => expect(fetch).toHaveBeenCalledTimes(2))
    const metrics = screen.getAllByRole('heading', { level: 4 }).map((node) => node.textContent)
    expect(metrics).toEqual(['2', '3', '1'])
  })

  it('показывает ошибку загрузки метрик администратора', async () => {
    isAdmin.mockReturnValue(true)
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: false }))
    render(<MemoryRouter><Dashboard /></MemoryRouter>)
    expect(await screen.findByText(/Не удалось загрузить показатели Dashboard/)).toBeInTheDocument()
  })

  it('показывает модератору кликабельные разделы каталога', () => {
    isContentModerator.mockReturnValue(true)
    render(<MemoryRouter><Dashboard /></MemoryRouter>)
    expect(screen.getByRole('link', { name: /Автомобили/ })).toHaveAttribute('href', '/cars')
    expect(screen.getByRole('link', { name: /Запчасти/ })).toHaveAttribute('href', '/parts')
  })

  it('загружает реальные заказы для операторского dashboard', async () => {
    isOperator.mockReturnValue(true)
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, json: async () => [
      { id: '11111111-1111-1111-1111-111111111111', customerName: 'Иван', customerEmail: 'ivan@example.com', orderType: 'Car', status: 'Created', totalAmount: 500000, createdAtUtc: '2026-09-08T10:00:00Z' },
      { id: '22222222-2222-2222-2222-222222222222', customerName: 'Анна', customerEmail: 'anna@example.com', orderType: 'Part', status: 'Processing', totalAmount: 10000, createdAtUtc: '2026-09-08T11:00:00Z' },
    ] })
    vi.stubGlobal('fetch', fetchMock)
    render(<MemoryRouter><Dashboard /></MemoryRouter>)
    expect(screen.getByText('Dashboard оператора')).toBeInTheDocument()
    await waitFor(() => expect(fetchMock).toHaveBeenCalledWith('/orders/api/orders', expect.objectContaining({
      headers: expect.objectContaining({ Authorization: expect.any(String) }),
    })))
    expect(await screen.findByText('Иван')).toBeInTheDocument()
    expect(screen.getByText('500 000 ₽')).toBeInTheDocument()
  })
})
