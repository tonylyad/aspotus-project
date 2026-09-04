export function getAvailableQuantity(item) {
  if (item.type === 'car') return item.isAvailable === false ? 0 : 1
  const available = Number(item.availableStockQuantity ?? item.stockQuantity ?? 0)
  return Math.max(0, Number.isFinite(available) ? available : 0)
}

export function calculateTotal(items) {
  return items.reduce((sum, item) => sum + Number(item.price || 0) * Number(item.quantity || 0), 0)
}

export function formatPrice(value) {
  return `${Number(value || 0).toLocaleString('ru-RU')} ₽`
}
