import { describe, expect, it } from 'vitest'
import { getUserRoles, isAdmin, isContentModerator, isOperator, isUsersSectionBlocked } from './auth.js'

const tokenFor = (roles) => {
  const payload = { 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': roles }
  return `header.${btoa(JSON.stringify(payload)).replaceAll('=', '')}.signature`
}

describe('проверка ролей администратора', () => {
  it('возвращает пустой список без токена', () => {
    expect(getUserRoles()).toEqual([])
  })

  it('возвращает пустой список для повреждённого JWT', () => {
    localStorage.setItem('token', 'broken-token')
    expect(getUserRoles()).toEqual([])
  })

  it('возвращает пустой список без role claim', () => {
    localStorage.setItem('token', `header.${btoa('{}')}.signature`)
    expect(getUserRoles()).toEqual([])
  })

  it('нормализует одиночную роль в массив', () => {
    localStorage.setItem('token', tokenFor('Admin'))
    expect(getUserRoles()).toEqual(['Admin'])
  })

  it('сохраняет массив ролей', () => {
    localStorage.setItem('token', tokenFor(['Admin', 'Operator']))
    expect(getUserRoles()).toEqual(['Admin', 'Operator'])
  })

  it('распознаёт администратора', () => {
    localStorage.setItem('token', tokenFor('Admin'))
    expect(isAdmin()).toBe(true)
    expect(isUsersSectionBlocked()).toBe(false)
  })

  it('распознаёт оператора', () => {
    localStorage.setItem('token', tokenFor('Operator'))
    expect(isOperator()).toBe(true)
    expect(isAdmin()).toBe(false)
  })

  it('распознаёт контент-модератора', () => {
    localStorage.setItem('token', tokenFor('ContentModerator'))
    expect(isContentModerator()).toBe(true)
    expect(isUsersSectionBlocked()).toBe(true)
  })
})
