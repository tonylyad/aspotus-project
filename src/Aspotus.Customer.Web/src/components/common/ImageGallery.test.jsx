import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import ImageGallery from './ImageGallery.jsx'

describe('ImageGallery', () => {
  it('открывает фотографию в портале и закрывает крестиком', () => {
    render(<div style={{ transform: 'translateY(-8px)' }}><ImageGallery images={['/car.jpg']} alt="Автомобиль" /></div>)

    fireEvent.click(screen.getByRole('button', { name: 'Открыть фото на весь экран' }))
    expect(screen.getByRole('dialog', { name: 'Просмотр фотографии' }).parentElement).toBe(document.body)

    fireEvent.click(screen.getByRole('button', { name: 'Закрыть фотографию' }))
    expect(screen.queryByRole('dialog', { name: 'Просмотр фотографии' })).not.toBeInTheDocument()
  })

  it('закрывает фотографию по клику на фон и по Escape', () => {
    render(<ImageGallery images={['/car.jpg']} />)
    const open = screen.getByRole('button', { name: 'Открыть фото на весь экран' })

    fireEvent.click(open)
    fireEvent.click(screen.getByRole('dialog', { name: 'Просмотр фотографии' }))
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()

    fireEvent.click(open)
    fireEvent.keyDown(window, { key: 'Escape' })
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
  })
})
