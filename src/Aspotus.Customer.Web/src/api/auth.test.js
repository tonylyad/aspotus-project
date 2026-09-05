import { beforeEach, describe, expect, it, vi } from 'vitest'
import { api } from './axios.js'
import {
  createCarOrder, createCustomerRequest, createPartOrder, getMyOrders,
  getOrderById, getProfile, login, register,
} from './auth.js'

vi.mock('./axios.js', () => ({ api: { get: vi.fn(), post: vi.fn() } }))

describe('API заказов и авторизации', () => {
  beforeEach(() => vi.clearAllMocks())

  it('отправляет заказ запчастей только с идентификаторами и количеством', async () => {
    api.post.mockResolvedValue({ data: { id: 'order-1' } })
    const result = await createPartOrder(
      [{ id: 'part-1', quantity: 2, price: 999, name: 'Не должно уйти' }],
      { customerName: 'Иван', customerEmail: 'ivan@test.ru', customerPhone: '+7999', deliveryAddress: 'Москва' },
      { id: 'user-1', name: 'Иван', email: 'ivan@test.ru' },
    )
    expect(result).toEqual({ id: 'order-1' })
    expect(api.post).toHaveBeenCalledWith('/orders/api/orders/parts', expect.objectContaining({
      items: [{ partId: 'part-1', quantity: 2 }],
    }))
  })

  it('отправляет заказ автомобиля только с идентификатором каталога', async () => {
    api.post.mockResolvedValue({ data: { id: 'order-2' } })
    await createCarOrder(
      [{ id: 'car-1', price: 999999, brandName: 'Подмена' }],
      { customerPhone: '+7999', deliveryAddress: 'Москва' },
      { id: 'user-1', name: 'Иван', email: 'ivan@test.ru' },
    )
    expect(api.post).toHaveBeenCalledWith('/orders/api/orders/cars', {
      customerName: 'Иван', customerEmail: 'ivan@test.ru', customerPhone: '+7999', deliveryAddress: 'Москва', car: { carId: 'car-1' },
    })
  })

  it('понимает пользователя, представленного claims', async () => {
    api.post.mockResolvedValue({ data: {} })
    await createCarOrder([{ id: 'car-1' }], { customerPhone: '1', deliveryAddress: 'A' }, [
      { type: 'nameidentifier', value: 'user-1' },
      { type: 'name', value: 'Иван' },
      { type: 'email', value: 'ivan@test.ru' },
    ])
    expect(api.post).toHaveBeenCalledWith('/orders/api/orders/cars', expect.objectContaining({ customerName: 'Иван', customerEmail: 'ivan@test.ru' }))
  })

  it('не создаёт заказ без пользователя', async () => {
    await expect(createPartOrder([], {}, null)).rejects.toThrow('User is not defined')
    expect(api.post).not.toHaveBeenCalled()
  })

  it('не запрашивает заказы без идентификатора пользователя', () => {
    expect(() => getMyOrders()).toThrow('User ID not found')
  })

  it('использует правильные адреса чтения заказов', () => {
    getMyOrders('user-1')
    getOrderById('order-1')
    expect(api.get).toHaveBeenNthCalledWith(1, '/orders/api/orders/by-user/user-1')
    expect(api.get).toHaveBeenNthCalledWith(2, '/orders/api/orders/order-1')
  })

  it('использует правильные адреса авторизации', () => {
    login({ login: 'user' }); register({ email: 'new@test.ru' }); getProfile()
    expect(api.post).toHaveBeenNthCalledWith(1, '/api/Auth/login', { login: 'user' })
    expect(api.post).toHaveBeenNthCalledWith(2, '/api/Auth/register', { email: 'new@test.ru' })
    expect(api.get).toHaveBeenCalledWith('/api/Auth/me')
  })

  it('отправляет клиентскую заявку в Orders API', () => {
    const request = { type: 'auto', customerName: 'Иван' }
    createCustomerRequest(request)
    expect(api.post).toHaveBeenCalledWith('/orders/api/requests', request)
  })
})
