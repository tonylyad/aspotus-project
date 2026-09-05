import { fireEvent, render, screen } from '@testing-library/react'
import { useState } from 'react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import CatalogImagesEditor from './CatalogImagesEditor.jsx'

const stored = (key) => ({ fileKey: key, url: `https://storage/${key}` })

function Harness({ initial = [], onRemoveStored = () => {} }) {
  const [items, setItems] = useState(initial)
  return <>
    <CatalogImagesEditor items={items} setItems={setItems} onRemoveStored={onRemoveStored} />
    <output data-testid="order">{items.map((item) => item.fileKey || item.file.name).join(',')}</output>
  </>
}

describe('CatalogImagesEditor', () => {
  beforeEach(() => {
    vi.stubGlobal('URL', {
      ...URL,
      createObjectURL: vi.fn((file) => `blob:${file.name}`),
      revokeObjectURL: vi.fn(),
    })
  })

  it('показывает пять свободных мест для пустой карточки', () => {
    render(<Harness />)
    expect(screen.getAllByText('Свободное место')).toHaveLength(5)
    expect(screen.getByText('0 из 5')).toBeInTheDocument()
  })

  it('показывает сохранённые изображения и оставшиеся места', () => {
    render(<Harness initial={[stored('a.jpg'), stored('b.jpg')]} />)
    expect(screen.getAllByRole('img')).toHaveLength(2)
    expect(screen.getAllByText('Свободное место')).toHaveLength(3)
    expect(screen.getByText('2 из 5')).toBeInTheDocument()
  })

  it('добавляет изображения через обычный input', () => {
    const { container } = render(<Harness />)
    const input = container.querySelector('input[type="file"]')
    fireEvent.change(input, { target: { files: [new File(['x'], 'one.png', { type: 'image/png' })] } })
    expect(screen.getByTestId('order')).toHaveTextContent('one.png')
    expect(URL.createObjectURL).toHaveBeenCalledTimes(1)
  })

  it('игнорирует файлы, которые не являются изображениями', () => {
    const { container } = render(<Harness />)
    fireEvent.change(container.querySelector('input[type="file"]'), {
      target: { files: [new File(['x'], 'readme.txt', { type: 'text/plain' })] },
    })
    expect(screen.getByTestId('order')).toBeEmptyDOMElement()
  })

  it('не позволяет добавить больше пяти изображений', () => {
    const { container } = render(<Harness initial={[stored('a.jpg'), stored('b.jpg'), stored('c.jpg'), stored('d.jpg')]} />)
    const files = [1, 2, 3].map((index) => new File(['x'], `${index}.jpg`, { type: 'image/jpeg' }))
    fireEvent.change(container.querySelector('input[type="file"]'), { target: { files } })
    expect(screen.getAllByRole('img')).toHaveLength(5)
    expect(screen.getByText('5 из 5')).toBeInTheDocument()
  })

  it('передаёт ключ сохранённого файла при удалении', () => {
    const onRemoveStored = vi.fn()
    render(<Harness initial={[stored('a.jpg')]} onRemoveStored={onRemoveStored} />)
    fireEvent.click(screen.getByTitle('Удалить'))
    expect(onRemoveStored).toHaveBeenCalledWith('a.jpg')
    expect(screen.getByTestId('order')).toBeEmptyDOMElement()
  })

  it('освобождает preview при удалении нового файла', () => {
    const file = new File(['x'], 'new.jpg', { type: 'image/jpeg' })
    render(<Harness initial={[{ clientId: 'new', file, preview: 'blob:new.jpg', pending: true }]} />)
    fireEvent.click(screen.getByTitle('Удалить'))
    expect(URL.revokeObjectURL).toHaveBeenCalledWith('blob:new.jpg')
  })

  it('переставляет изображения drag-and-drop и показывает место вставки', () => {
    render(<Harness initial={[stored('a.jpg'), stored('b.jpg')]} />)
    const cards = screen.getAllByRole('img').map((image) => image.closest('.catalog-image'))
    const data = {}
    const dataTransfer = {
      files: [], effectAllowed: '', dropEffect: '',
      setData: (type, value) => { data[type] = value },
      getData: (type) => data[type] || '',
    }
    vi.spyOn(cards[1], 'getBoundingClientRect').mockReturnValue({ left: 0, width: 100 })
    fireEvent.dragStart(cards[0], { dataTransfer })
    fireEvent.dragOver(cards[1], { dataTransfer, clientX: 75 })
    expect(cards[1]).toHaveClass('catalog-image--drop-after')
    fireEvent.drop(cards[1], { dataTransfer, clientX: 75 })
    expect(screen.getByTestId('order')).toHaveTextContent('b.jpg,a.jpg')
  })
})
