import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Orders from './Orders.jsx'

const order = {
  id: '12345678-0000-0000-0000-000000000000', createdAtUtc: '2026-01-01T10:00:00Z',
  customerName: 'Иван Иванов', customerEmail: 'ivan@test.ru', customerPhone: '+7999',
  deliveryAddress: 'Москва', orderType: 'Car', status: 'Created', totalAmount: 2500000,
  carItems: [{ id: 'item-1', brandName: 'Toyota', modelName: 'Camry', generationName: 'XV70', year: 2020, price: 2500000 }],
  partItems: [],
}

const response = (body, ok = true) => ({ ok, json: async () => body })

describe('Orders', () => {
  beforeEach(() => {
    localStorage.setItem('token', 'jwt')
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(response([order])))
  })

  it('загружает и показывает заказы', async () => {
    render(<Orders />)
    expect(await screen.findByText('Иван Иванов')).toBeInTheDocument()
    expect(screen.getByText('Автомобиль')).toBeInTheDocument()
    expect(fetch).toHaveBeenCalledWith('/orders/api/orders', expect.objectContaining({ headers: { Authorization: 'Bearer jwt' } }))
  })

  it('фильтрует заказы по клиенту', async () => {
    render(<Orders />)
    await screen.findByText('Иван Иванов')
    fireEvent.change(screen.getByPlaceholderText('Поиск по заказам…'), { target: { value: 'другой' } })
    expect(screen.getByText('Заказы не найдены')).toBeInTheDocument()
  })

  it('открывает карточку заказа с сохранёнными данными автомобиля', async () => {
    const { container } = render(<Orders />)
    await screen.findByText('Иван Иванов')
    fireEvent.click(container.querySelector('tbody button'))
    expect(await screen.findByText('Заказ #12345678')).toBeInTheDocument()
    expect(screen.getByText('Camry')).toBeInTheDocument()
  })

  it('переводит созданный заказ в обработку', async () => {
    const updated = { ...order, status: 'Processing' }
    fetch.mockImplementation((url, options = {}) => Promise.resolve(
      options.method === 'PATCH' ? response(updated) : response([order]),
    ))
    const { container } = render(<Orders />)
    await screen.findByText('Иван Иванов')
    fireEvent.click(container.querySelector('tbody button'))
    fireEvent.mouseDown(await screen.findByRole('combobox', { name: 'Статус заказа' }))
    fireEvent.click(await screen.findByRole('option', { name: 'В обработке' }))
    await waitFor(() => expect(fetch).toHaveBeenCalledWith(
      '/orders/api/orders/12345678-0000-0000-0000-000000000000/status',
      expect.objectContaining({ method: 'PATCH', body: JSON.stringify({ status: 'Processing' }) }),
    ))
  })
})
