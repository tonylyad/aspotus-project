import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, expect, it, vi } from 'vitest'
import Header from './Navbar.jsx'

vi.mock('../../context/AuthContext', () => ({
  useAuth: () => ({ user: null, logout: vi.fn() }),
}))

vi.mock('../../context/CartContext', () => ({
  useCart: () => ({ cart: [{ id: 'car', quantity: 1 }, { id: 'part', quantity: 3 }] }),
}))

describe('Navbar', () => {
  it('показывает ссылку на корзину с общим количеством товаров', () => {
    render(<MemoryRouter><Header /></MemoryRouter>)

    const cartLink = screen.getByRole('link', { name: 'Корзина: 4 товаров' })
    expect(cartLink).toHaveAttribute('href', '/cart')
    expect(cartLink).toHaveTextContent('4')
  })
})
