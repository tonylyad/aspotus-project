import { createContext, useContext, useEffect, useState } from 'react'
import { calculateTotal, getAvailableQuantity } from '../utils/cart.js'

const CartContext = createContext()

export const CartProvider = ({ children }) => {
  const [cart, setCart] = useState(() => {
    try { return JSON.parse(localStorage.getItem('cart')) || [] }
    catch { return [] }
  })

  useEffect(() => { localStorage.setItem('cart', JSON.stringify(cart)) }, [cart])

  const addToCart = (item) => {
    const maxQuantity = getAvailableQuantity(item)
    if (maxQuantity === 0) return false

    setCart((current) => {
      const existing = current.find((entry) => entry.id === item.id)
      if (existing) {
        const quantity = Math.min(existing.quantity + 1, maxQuantity)
        return current.map((entry) => entry.id === item.id ? { ...entry, ...item, quantity } : entry)
      }
      return [...current, { ...item, quantity: 1 }]
    })
    return true
  }

  const removeFromCart = (id) => setCart((current) => current.filter((item) => item.id !== id))

  const updateQuantity = (id, requestedQuantity) => {
    if (requestedQuantity <= 0) return removeFromCart(id)
    setCart((current) => current.map((item) => item.id === id
      ? { ...item, quantity: Math.min(requestedQuantity, getAvailableQuantity(item)) }
      : item))
  }

  const clearCart = () => setCart([])
  const cars = cart.filter((item) => item.type === 'car')
  const parts = cart.filter((item) => item.type === 'part')

  return <CartContext.Provider value={{
    cart, cars, parts, addToCart, removeFromCart, updateQuantity, clearCart,
    partsTotal: calculateTotal(parts), carsTotal: calculateTotal(cars),
  }}>{children}</CartContext.Provider>
}

export const useCart = () => useContext(CartContext)
