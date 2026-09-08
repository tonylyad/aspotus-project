import { describe, expect, it } from 'vitest'
import { getApiErrorMessage, parseLocalizedNumber, validateCarForm } from './carForm.js'

const validForm = {
  brandId: 'brand-1', modelId: 'model-1', generationId: 'generation-1',
  year: '1980', mileage: '300000', price: '4000000', engineVolume: '4,2',
  bodyType: 'Suv', fuelType: 'Petrol', transmissionType: 'Manual', driveType: 'Awd',
}

describe('carForm', () => {
  it('понимает десятичную запятую', () => {
    expect(parseLocalizedNumber('4,2')).toBe(4.2)
  })

  it('проверяет год по диапазону выбранного поколения', () => {
    const result = validateCarForm(
      { ...validForm, year: '1780' },
      [{ id: 'generation-1', yearFrom: 1974, yearTo: null }],
    )

    expect(result.errors.year).toContain('1900')
  })

  it('возвращает нормализованные числовые значения', () => {
    const result = validateCarForm(validForm, [{ id: 'generation-1', yearFrom: 1974 }])

    expect(result.errors).toEqual({})
    expect(result.values).toMatchObject({ year: 1980, mileage: 300000, price: 4000000, engineVolume: 4.2 })
  })

  it('извлекает конкретные сообщения model validation из ответа API', () => {
    expect(getApiErrorMessage({
      title: 'One or more validation errors occurred.',
      errors: { Year: ['Год выпуска указан неверно.'], EngineVolume: ['Объём двигателя указан неверно.'] },
    })).toBe('Год выпуска указан неверно. Объём двигателя указан неверно.')
  })
})
