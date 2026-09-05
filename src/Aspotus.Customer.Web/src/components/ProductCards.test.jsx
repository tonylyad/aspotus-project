import { fireEvent, render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import CarCard from './cars/CarCard.jsx'
import PartCard from './parts/PartCard.jsx'
import { useAuth } from '../context/AuthContext.jsx'
import { useCart } from '../context/CartContext.jsx'

const navigate = vi.fn()
vi.mock('react-router-dom', async () => ({
  ...(await vi.importActual('react-router-dom')),
  useNavigate: () => navigate,
}))
vi.mock('framer-motion', () => ({
  motion: { div: ({ children, className }) => <div className={className}>{children}</div> },
}))
vi.mock('../hooks/useImage.js', () => ({ useImage: (url) => url || '/noPhoto.png' }))
vi.mock('../context/AuthContext.jsx', () => ({ useAuth: vi.fn() }))
vi.mock('../context/CartContext.jsx', () => ({ useCart: vi.fn() }))

const addToCart = vi.fn()
const car = { id: 'car-1', brandName: 'Toyota', modelName: 'Camry', generationName: 'XV70', year: 2020, mileage: 10000, price: 2500000, isAvailable: true }
const part = { id: 'part-1', name: 'Фильтр', article: 'A-1', price: 500, availableStockQuantity: 3 }
const renderCard = (component) => render(<MemoryRouter>{component}</MemoryRouter>)

describe('карточки каталога', () => {
  beforeEach(() => {
    navigate.mockReset()
    addToCart.mockReset().mockReturnValue(true)
    useAuth.mockReturnValue({ user: { id: 'user-1' } })
    useCart.mockReturnValue({ addToCart })
  })

  it('показывает основные данные автомобиля', () => {
    renderCard(<CarCard car={car} />)
    expect(screen.getByText('Toyota')).toBeInTheDocument()
    expect(screen.getByText('Camry')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Подробнее/i })).toHaveAttribute('href', '/cars/car-1')
  })

  it('блокирует добавление недоступного автомобиля', () => {
    renderCard(<CarCard car={{ ...car, isAvailable: false }} />)
    expect(screen.getByRole('button', { name: 'Корзина' })).toBeDisabled()
    expect(screen.getByText('В заказе')).toBeInTheDocument()
  })

  it('направляет гостя на вход', () => {
    useAuth.mockReturnValue({ user: null })
    renderCard(<CarCard car={car} />)
    fireEvent.click(screen.getByRole('button', { name: 'Корзина' }))
    expect(navigate).toHaveBeenCalledWith('/login')
    expect(addToCart).not.toHaveBeenCalled()
  })

  it('добавляет автомобиль авторизованному пользователю', () => {
    renderCard(<CarCard car={car} />)
    fireEvent.click(screen.getByRole('button', { name: 'Корзина' }))
    expect(addToCart).toHaveBeenCalledWith(expect.objectContaining({ id: 'car-1', type: 'car' }))
  })

  it('показывает остаток и ссылку запчасти', () => {
    renderCard(<PartCard part={part} />)
    expect(screen.getByText('В наличии: 3')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Подробнее/i })).toHaveAttribute('href', '/parts/part-1')
  })

  it('блокирует добавление отсутствующей запчасти', () => {
    renderCard(<PartCard part={{ ...part, availableStockQuantity: 0 }} />)
    expect(screen.getByText('Нет в наличии')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Корзина' })).toBeDisabled()
  })

  it('добавляет запчасть с правильным типом', () => {
    renderCard(<PartCard part={part} />)
    fireEvent.click(screen.getByRole('button', { name: 'Корзина' }))
    expect(addToCart).toHaveBeenCalledWith(expect.objectContaining({ id: 'part-1', type: 'part' }))
  })
})
