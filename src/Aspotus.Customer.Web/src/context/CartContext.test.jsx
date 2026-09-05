import { act, renderHook } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { CartProvider, useCart } from './CartContext.jsx'

const wrapper = ({ children }) => <CartProvider>{children}</CartProvider>
const car = { id: 'car-1', type: 'car', price: 1000, isAvailable: true }
const part = { id: 'part-1', type: 'part', price: 100, availableStockQuantity: 2 }

describe('CartProvider', () => {
  it('создаёт пустую корзину по умолчанию', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    expect(result.current.cart).toEqual([])
  })

  it('восстанавливает корзину из localStorage', () => {
    localStorage.setItem('cart', JSON.stringify([car]))
    const { result } = renderHook(() => useCart(), { wrapper })
    expect(result.current.cart).toHaveLength(1)
  })

  it('игнорирует повреждённое состояние корзины', () => {
    localStorage.setItem('cart', '{broken')
    const { result } = renderHook(() => useCart(), { wrapper })
    expect(result.current.cart).toEqual([])
  })

  it('добавляет доступный автомобиль', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => expect(result.current.addToCart(car)).toBe(true))
    expect(result.current.cars[0]).toMatchObject({ id: 'car-1', quantity: 1 })
  })

  it('не увеличивает количество автомобиля выше одного', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => { result.current.addToCart(car); result.current.addToCart(car) })
    expect(result.current.cars[0].quantity).toBe(1)
  })

  it('не добавляет недоступный товар', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => expect(result.current.addToCart({ ...car, isAvailable: false })).toBe(false))
    expect(result.current.cart).toEqual([])
  })

  it('ограничивает количество запчасти доступным остатком', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => { result.current.addToCart(part); result.current.addToCart(part); result.current.addToCart(part) })
    expect(result.current.parts[0].quantity).toBe(2)
  })

  it('удаляет позицию при установке нулевого количества', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => result.current.addToCart(part))
    act(() => result.current.updateQuantity(part.id, 0))
    expect(result.current.cart).toEqual([])
  })

  it('считает суммы автомобилей и запчастей отдельно', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => { result.current.addToCart(car); result.current.addToCart(part) })
    expect(result.current.carsTotal).toBe(1000)
    expect(result.current.partsTotal).toBe(100)
  })

  it('очищает корзину и localStorage', () => {
    const { result } = renderHook(() => useCart(), { wrapper })
    act(() => result.current.addToCart(car))
    act(() => result.current.clearCart())
    expect(result.current.cart).toEqual([])
    expect(JSON.parse(localStorage.getItem('cart'))).toEqual([])
  })
})
