import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, expect, it, vi } from 'vitest'
import Header from './Navbar.jsx'

const cartState = vi.hoisted(() => ({ cart: [] }))

vi.mock('../../context/AuthContext', () => ({
  useAuth: () => ({ user: null, logout: vi.fn() }),
}))

vi.mock('../../context/CartContext', () => ({
  useCart: () => ({ cart: cartState.cart }),
}))

describe('Navbar', () => {
  it('показывает ссылку на корзину с общим количеством товаров', () => {
    cartState.cart = [{ id: 'car', quantity: 1 }, { id: 'part', quantity: 3 }]
    render(<MemoryRouter><Header /></MemoryRouter>)

    const cartLink = screen.getByRole('link', { name: 'Корзина: 4 товаров' })
    expect(cartLink).toHaveAttribute('href', '/cart')
    expect(cartLink).toHaveTextContent('4')
  })

  it('не показывает нулевой счётчик у пустой корзины', () => {
    cartState.cart = []
    render(<MemoryRouter><Header /></MemoryRouter>)

    const cartLink = screen.getByRole('link', { name: 'Корзина пуста' })
    expect(cartLink).toHaveAttribute('href', '/cart')
    expect(cartLink.querySelector('.header-cart-count')).toBeNull()
    expect(cartLink).not.toHaveTextContent('0')
  })
})
