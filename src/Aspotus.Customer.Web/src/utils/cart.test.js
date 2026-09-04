import test from 'node:test'
import assert from 'node:assert/strict'
import { calculateTotal, getAvailableQuantity } from './cart.js'

test('автомобиль можно добавить только один раз, если он доступен', () => {
  assert.equal(getAvailableQuantity({ type: 'car', isAvailable: true }), 1)
  assert.equal(getAvailableQuantity({ type: 'car', isAvailable: false }), 0)
})

test('доступное количество запчасти не бывает отрицательным', () => {
  assert.equal(getAvailableQuantity({ type: 'part', availableStockQuantity: 3 }), 3)
  assert.equal(getAvailableQuantity({ type: 'part', availableStockQuantity: -2 }), 0)
})

test('итог считается по цене и количеству', () => {
  assert.equal(calculateTotal([{ price: 150, quantity: 2 }, { price: 75, quantity: 1 }]), 375)
})
