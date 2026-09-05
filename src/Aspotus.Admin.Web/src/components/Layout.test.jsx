import { fireEvent, render, screen } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { describe, expect, it } from 'vitest'
import Layout from './Layout.jsx'

const tokenFor = (roles) => {
  const payload = { 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': roles }
  return `header.${btoa(JSON.stringify(payload))}.signature`
}

const renderLayout = (roles) => {
  localStorage.setItem('token', tokenFor(roles))
  localStorage.setItem('fullName', 'Тестовый пользователь')
  return render(<MemoryRouter initialEntries={['/']}><Routes>
    <Route element={<Layout />}><Route index element={<div>Главная</div>} /></Route>
    <Route path="/login" element={<div>Страница входа</div>} />
  </Routes></MemoryRouter>)
}

describe('Layout', () => {
  it('показывает имя текущего пользователя', () => {
    renderLayout('Admin')
    expect(screen.getByText('Тестовый пользователь')).toBeInTheDocument()
  })

  it('показывает администратору все основные разделы', () => {
    renderLayout('Admin')
    expect(screen.getByRole('link', { name: 'Пользователи' })).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Автомобили' })).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Заказы' })).toBeInTheDocument()
  })

  it('скрывает от модератора пользователей и заказы', () => {
    renderLayout('ContentModerator')
    expect(screen.getByRole('link', { name: 'Автомобили' })).toBeInTheDocument()
    expect(screen.queryByRole('link', { name: 'Пользователи' })).not.toBeInTheDocument()
    expect(screen.queryByRole('link', { name: 'Заказы' })).not.toBeInTheDocument()
  })

  it('оставляет оператору заказы и заявки', () => {
    renderLayout('Operator')
    expect(screen.getByRole('link', { name: 'Заказы' })).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Заявки клиентов' })).toBeInTheDocument()
    expect(screen.queryByRole('link', { name: 'Автомобили' })).not.toBeInTheDocument()
  })

  it('удаляет токен и открывает вход при выходе', () => {
    renderLayout('Admin')
    fireEvent.click(screen.getByRole('button', { name: 'Выйти' }))
    expect(localStorage.getItem('token')).toBeNull()
    expect(screen.getByText('Страница входа')).toBeInTheDocument()
  })
})
