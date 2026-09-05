import { describe, expect, it } from 'vitest'
import { calculateTotal, formatPrice, getAvailableQuantity } from './cart.js'

describe('getAvailableQuantity', () => {
  it('разрешает добавить доступный автомобиль один раз', () => {
    expect(getAvailableQuantity({ type: 'car', isAvailable: true })).toBe(1)
  })

  it('не разрешает добавить автомобиль, находящийся в заказе', () => {
    expect(getAvailableQuantity({ type: 'car', isAvailable: false })).toBe(0)
  })

  it('считает автомобиль доступным, если флаг не передан', () => {
    expect(getAvailableQuantity({ type: 'car' })).toBe(1)
  })

  it('использует доступный остаток запчасти', () => {
    expect(getAvailableQuantity({ type: 'part', availableStockQuantity: 3 })).toBe(3)
  })

  it('поддерживает старое поле общего остатка', () => {
    expect(getAvailableQuantity({ type: 'part', stockQuantity: 4 })).toBe(4)
  })

  it('не возвращает отрицательный остаток', () => {
    expect(getAvailableQuantity({ type: 'part', availableStockQuantity: -2 })).toBe(0)
  })

  it('возвращает ноль для некорректного остатка', () => {
    expect(getAvailableQuantity({ type: 'part', availableStockQuantity: 'нет' })).toBe(0)
  })
})

describe('calculateTotal', () => {
  it('считает сумму по цене и количеству', () => {
    expect(calculateTotal([{ price: 150, quantity: 2 }, { price: 75, quantity: 1 }])).toBe(375)
  })

  it('возвращает ноль для пустой корзины', () => {
    expect(calculateTotal([])).toBe(0)
  })

  it('безопасно обрабатывает отсутствующие значения', () => {
    expect(calculateTotal([{ quantity: 2 }, { price: 10 }])).toBe(0)
  })
})

describe('formatPrice', () => {
  it('добавляет валюту к цене', () => {
    expect(formatPrice(1500)).toContain('₽')
  })

  it('форматирует отсутствующую цену как ноль', () => {
    expect(formatPrice(null)).toContain('0')
  })
})
