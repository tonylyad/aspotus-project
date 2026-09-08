import { render, screen } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { describe, expect, it, vi } from 'vitest'
import CheckoutPage from './CheckoutPage.jsx'

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({
    user: {
      id: 'user-1',
      name: 'Иван Иванов',
      email: 'ivan@test.ru',
      phoneNumber: '+79990000000',
    },
  }),
}))

vi.mock('../context/CartContext', () => ({
  useCart: () => ({
    cars: [],
    parts: [{ id: 'part-1', name: 'Фильтр', article: 'A-1', quantity: 1, price: 1000 }],
    removeFromCart: vi.fn(),
    partsTotal: 1000,
    carsTotal: 0,
  }),
}))

describe('CheckoutPage', () => {
  it('подставляет телефон из профиля в заказ', () => {
    render(
      <MemoryRouter initialEntries={['/checkout/parts']}>
        <Routes>
          <Route path="/checkout/:type" element={<CheckoutPage />} />
        </Routes>
      </MemoryRouter>,
    )

    expect(screen.getByLabelText('Телефон')).toHaveValue('+79990000000')
  })
})
