export const parseLocalizedNumber = (value) => {
  if (typeof value === 'number') return value
  return Number(String(value ?? '').trim().replace(',', '.'))
}

export const getApiErrorMessage = (data, fallback = 'Ошибка сохранения') => {
  const validationMessages = data?.errors
    ? Object.values(data.errors).flat().filter(Boolean)
    : []

  return validationMessages.join(' ') || data?.message || data?.title || fallback
}

export const validateCarForm = (form, generations) => {
  const errors = {}
  const isBlank = (value) => String(value ?? '').trim() === ''
  const year = Number(form.year)
  const mileage = Number(form.mileage)
  const price = parseLocalizedNumber(form.price)
  const engineVolume = parseLocalizedNumber(form.engineVolume)
  const generation = generations.find((item) => item.id === form.generationId)

  if (!Number.isInteger(year) || year < 1900 || year > 3000) {
    errors.year = 'Год выпуска должен быть в диапазоне от 1900 до 3000.'
  } else if (generation && (year < generation.yearFrom || (generation.yearTo && year > generation.yearTo))) {
    const upperBound = generation.yearTo || 'н. в.'
    errors.year = `Для выбранного поколения допустим год ${generation.yearFrom}–${upperBound}.`
  }

  if (isBlank(form.mileage) || !Number.isInteger(mileage) || mileage < 0) {
    errors.mileage = 'Пробег должен быть целым неотрицательным числом.'
  }
  if (!Number.isFinite(price) || price <= 0 || price > 1_000_000_000) {
    errors.price = 'Цена должна быть больше 0 и не превышать 1 000 000 000 ₽.'
  }
  if (!Number.isFinite(engineVolume) || engineVolume < 0.1 || engineVolume > 20) {
    errors.engineVolume = 'Объём двигателя должен быть от 0,1 до 20 литров.'
  }

  if (!form.brandId) errors.brandId = 'Выберите бренд.'
  if (!form.modelId) errors.modelId = 'Выберите модель.'
  if (!form.generationId) errors.generationId = 'Выберите поколение.'
  if (!form.bodyType) errors.bodyType = 'Выберите тип кузова.'
  if (!form.fuelType) errors.fuelType = 'Выберите тип двигателя.'
  if (!form.transmissionType) errors.transmissionType = 'Выберите трансмиссию.'
  if (!form.driveType) errors.driveType = 'Выберите привод.'

  return {
    errors,
    values: { year, mileage, price, engineVolume },
  }
}
