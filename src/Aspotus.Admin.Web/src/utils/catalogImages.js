export async function uploadCatalogImages(kind, entityId, items) {
  const token = localStorage.getItem('token')
  const resolved = []

  for (const item of items) {
    if (!item.pending) {
      resolved.push(item)
      continue
    }

    const extension = item.file.name.split('.').pop()?.toLowerCase() || 'jpg'
    const key = `${kind}/${entityId}/${crypto.randomUUID()}.${extension}`
    const response = await fetch(`/files/Files/${key}`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/octet-stream',
      },
      body: item.file,
    })

    if (!response.ok) throw new Error(`Не удалось загрузить файл ${item.file.name}`)
    resolved.push(await response.json())
  }

  return resolved
}

export async function deleteCatalogImages(keys) {
  const token = localStorage.getItem('token')
  const responses = await Promise.all(keys.map((key) => fetch(`/files/Files/${key}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  })))

  if (responses.some((response) => !response.ok && response.status !== 404)) {
    throw new Error('Не удалось удалить один или несколько файлов из хранилища')
  }
}

export function toImageRequests(images) {
  return images.map((image, index) => ({
    fileKey: image.fileKey || image.key,
    url: image.url,
    sortOrder: index,
    isPrimary: index === 0,
  }))
}
