import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Requests from './Requests.jsx'

const requestItem = {
  id: 'request-1', createdAtUtc: '2026-01-01T10:00:00Z', type: 'spare', status: 'New',
  customerName: 'Пётр', customerEmail: 'petr@test.ru', customerPhone: '+7999',
  comment: 'Нужна срочно', detailsJson: JSON.stringify({ partName: 'Фильтр', condition: 'new' }),
}

const response = (body, ok = true) => ({ ok, json: async () => body })

describe('Requests', () => {
  beforeEach(() => {
    localStorage.setItem('token', 'jwt')
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(response([requestItem])))
  })

  it('загружает и отображает заявки клиентов', async () => {
    render(<Requests />)
    expect(await screen.findByText('Пётр')).toBeInTheDocument()
    expect(screen.getByText('Запчасть')).toBeInTheDocument()
    expect(screen.getByText(/partName: Фильтр/)).toBeInTheDocument()
  })

  it('фильтрует заявки по контактам', async () => {
    render(<Requests />)
    await screen.findByText('Пётр')
    fireEvent.change(screen.getByPlaceholderText('Поиск по заявкам…'), { target: { value: 'нет совпадений' } })
    expect(screen.getByText('Заявки не найдены')).toBeInTheDocument()
  })

  it('обновляет статус заявки', async () => {
    const updated = { ...requestItem, status: 'Processing' }
    fetch.mockImplementation((url, options = {}) => Promise.resolve(options.method === 'PATCH' ? response(updated) : response([requestItem])))
    render(<Requests />)
    await screen.findByText('Пётр')
    fireEvent.mouseDown(screen.getByRole('combobox', { name: 'Статус' }))
    fireEvent.click(await screen.findByRole('option', { name: 'В работе' }))
    await waitFor(() => expect(fetch).toHaveBeenCalledWith('/orders/api/requests/request-1/status', expect.objectContaining({
      method: 'PATCH', body: JSON.stringify({ status: 'Processing' }),
    })))
  })
})
