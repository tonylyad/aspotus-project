import { beforeEach, describe, expect, it, vi } from 'vitest'
import { deleteCatalogImages, toImageRequests, uploadCatalogImages } from './catalogImages.js'

describe('toImageRequests', () => {
  it('назначает порядок по позиции в массиве', () => {
    expect(toImageRequests([{ fileKey: 'a' }, { fileKey: 'b' }]).map((item) => item.sortOrder)).toEqual([0, 1])
  })

  it('делает основным только первое изображение', () => {
    expect(toImageRequests([{ fileKey: 'a' }, { fileKey: 'b' }]).map((item) => item.isPrimary)).toEqual([true, false])
  })

  it('поддерживает key ответа файлового API', () => {
    expect(toImageRequests([{ key: 'cars/1.jpg', url: 'url' }])[0]).toMatchObject({ fileKey: 'cars/1.jpg', url: 'url' })
  })
})

describe('uploadCatalogImages', () => {
  beforeEach(() => {
    localStorage.setItem('token', 'jwt')
    vi.spyOn(globalThis.crypto, 'randomUUID').mockReturnValue('file-id')
  })

  it('не загружает уже сохранённые изображения повторно', async () => {
    const fetchMock = vi.fn()
    vi.stubGlobal('fetch', fetchMock)
    const stored = { fileKey: 'cars/stored.jpg', url: 'stored' }
    await expect(uploadCatalogImages('cars', 'car-1', [stored])).resolves.toEqual([stored])
    expect(fetchMock).not.toHaveBeenCalled()
  })

  it('загружает новый файл с токеном и корректным ключом', async () => {
    const response = { key: 'cars/car-1/file-id.jpeg', url: 'uploaded' }
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, json: async () => response })
    vi.stubGlobal('fetch', fetchMock)
    const file = new File(['image'], 'PHOTO.JPEG', { type: 'image/jpeg' })
    await expect(uploadCatalogImages('cars', 'car-1', [{ pending: true, file }])).resolves.toEqual([response])
    expect(fetchMock).toHaveBeenCalledWith('/files/Files/cars/car-1/file-id.jpeg', expect.objectContaining({
      method: 'POST', body: file, headers: expect.objectContaining({ Authorization: 'Bearer jwt' }),
    }))
  })

  it('прерывает сохранение при ошибке файлового API', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: false }))
    const file = new File(['image'], 'photo.png', { type: 'image/png' })
    await expect(uploadCatalogImages('parts', 'part-1', [{ pending: true, file }])).rejects.toThrow('photo.png')
  })
})

describe('deleteCatalogImages', () => {
  it('удаляет каждый переданный ключ', async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 204 })
    vi.stubGlobal('fetch', fetchMock)
    await deleteCatalogImages(['a.jpg', 'b.jpg'])
    expect(fetchMock).toHaveBeenCalledTimes(2)
    expect(fetchMock).toHaveBeenCalledWith('/files/Files/a.jpg', expect.objectContaining({ method: 'DELETE' }))
  })

  it('считает отсутствующий файл успешно удалённым', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: false, status: 404 }))
    await expect(deleteCatalogImages(['missing.jpg'])).resolves.toBeUndefined()
  })

  it('сообщает об ошибке хранилища', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: false, status: 500 }))
    await expect(deleteCatalogImages(['failed.jpg'])).rejects.toThrow()
  })
})
