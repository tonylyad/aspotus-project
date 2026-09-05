import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import Pagination from './Pagination.jsx'

describe('Pagination', () => {
  it('не отображается для одной страницы', () => {
    const { container } = render(<Pagination page={1} totalPages={1} onPageChange={() => {}} />)
    expect(container).toBeEmptyDOMElement()
  })

  it('показывает первую, соседние и последнюю страницы', () => {
    render(<Pagination page={5} totalPages={10} onPageChange={() => {}} />)
    expect(screen.getByText('1')).toBeInTheDocument()
    expect(screen.getByText('4')).toBeInTheDocument()
    expect(screen.getByText('5').closest('li')).toHaveClass('active')
    expect(screen.getByText('6')).toBeInTheDocument()
    expect(screen.getByText('10')).toBeInTheDocument()
  })

  it('переходит на выбранную страницу', () => {
    const onPageChange = vi.fn()
    render(<Pagination page={2} totalPages={4} onPageChange={onPageChange} />)
    fireEvent.click(screen.getByText('3'))
    expect(onPageChange).toHaveBeenCalledWith(3)
  })

  it('переходит на следующую страницу', () => {
    const onPageChange = vi.fn()
    render(<Pagination page={2} totalPages={4} onPageChange={onPageChange} />)
    fireEvent.click(screen.getByRole('button', { name: 'Next' }))
    expect(onPageChange).toHaveBeenCalledWith(3)
  })

  it('блокирует переход назад на первой странице', () => {
    render(<Pagination page={1} totalPages={4} onPageChange={() => {}} />)
    expect(screen.getByText('Previous').closest('li')).toHaveClass('disabled')
    expect(screen.getByText('First').closest('li')).toHaveClass('disabled')
  })

  it('блокирует переход вперёд на последней странице', () => {
    render(<Pagination page={4} totalPages={4} onPageChange={() => {}} />)
    expect(screen.getByText('Next').closest('li')).toHaveClass('disabled')
    expect(screen.getByText('Last').closest('li')).toHaveClass('disabled')
  })
})
