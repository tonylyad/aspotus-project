import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import EmptyState from './EmptyState.jsx'

const navigate = vi.fn()
vi.mock('react-router-dom', () => ({ useNavigate: () => navigate }))

describe('EmptyState', () => {
  it('показывает переданные заголовок и описание', () => {
    render(<EmptyState title="Пусто" text="Измените фильтр" />)
    expect(screen.getByText('Пусто')).toBeInTheDocument()
    expect(screen.getByText('Измените фильтр')).toBeInTheDocument()
  })

  it('открывает страницу заявки', () => {
    render(<EmptyState />)
    fireEvent.click(screen.getByRole('button', { name: 'Оставить заявку' }))
    expect(navigate).toHaveBeenCalledWith('/request')
  })
})
