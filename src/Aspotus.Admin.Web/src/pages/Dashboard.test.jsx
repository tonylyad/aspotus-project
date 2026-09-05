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

  it('показывает операторский dashboard без запросов метрик администратора', () => {
    isOperator.mockReturnValue(true)
    const fetchMock = vi.fn()
    vi.stubGlobal('fetch', fetchMock)
    render(<MemoryRouter><Dashboard /></MemoryRouter>)
    expect(screen.getByText('Dashboard оператора')).toBeInTheDocument()
    expect(fetchMock).not.toHaveBeenCalled()
  })
})
