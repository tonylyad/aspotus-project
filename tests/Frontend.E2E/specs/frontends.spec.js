import { expect, test } from '@playwright/test'

const gatewayUrl = 'http://localhost:5230'
const customerUrl = 'http://localhost:5173'
const adminUrl = 'http://localhost:5174'

test('клиентский каталог открывается', async ({ page }) => {
  await page.goto(customerUrl)
  await expect(page.getByRole('link', { name: 'ASPOTUS' })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Авто' })).toBeVisible()
})

test('гостя перенаправляет с корзины на страницу входа', async ({ page }) => {
  await page.goto(`${customerUrl}/cart`)
  await expect(page).toHaveURL(/\/login$/)
  await expect(page.getByRole('heading', { name: 'Вход в систему' })).toBeVisible()
})

test('администратор входит и видит разделы по роли', async ({ page }) => {
  await page.goto(`${adminUrl}/login`)
  await page.getByLabel(/^Логин/).fill('admin')
  await page.getByLabel(/^Пароль/).fill('123456')
  await page.getByRole('button', { name: 'Войти' }).click()
  await expect(page).toHaveURL(`${adminUrl}/`)
  await expect(page.getByRole('link', { name: 'Пользователи' })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Заказы' })).toBeVisible()
})

test('заказ резервирует автомобиль, а отмена освобождает его', async ({ request }) => {
  const login = async (user) => {
    const response = await request.post(`${gatewayUrl}/api/auth/login`, { data: { login: user, password: '123456' } })
    expect(response.ok()).toBeTruthy()
    return (await response.json()).token
  }

  const customerToken = await login('customer')
  const adminToken = await login('admin')
  const customerHeaders = { Authorization: `Bearer ${customerToken}` }
  const adminHeaders = { Authorization: `Bearer ${adminToken}` }
  const carsResponse = await request.get(`${gatewayUrl}/catalog/api/cars`)
  const cars = await carsResponse.json()
  const car = cars.find((item) => item.isAvailable && item.price > 0)
  expect(car, 'Нужен хотя бы один доступный автомобиль с ценой').toBeTruthy()

  let orderId
  try {
    const payload = {
      customerName: `Playwright E2E ${Date.now()}`,
      customerEmail: 'customer@aspotus.com',
      customerPhone: '+79990000000',
      deliveryAddress: 'E2E cleanup address',
      car: { carId: car.id },
    }
    const created = await request.post(`${gatewayUrl}/orders/api/orders/cars`, { headers: customerHeaders, data: payload })
    expect(created.status()).toBe(201)
    orderId = (await created.json()).id

    const reservedCar = await request.get(`${gatewayUrl}/catalog/api/cars/${car.id}`)
    expect((await reservedCar.json()).isAvailable).toBe(false)

    const duplicate = await request.post(`${gatewayUrl}/orders/api/orders/cars`, { headers: customerHeaders, data: payload })
    expect(duplicate.status()).toBe(400)

    const cancelled = await request.patch(`${gatewayUrl}/orders/api/orders/${orderId}/status`, {
      headers: adminHeaders,
      data: { status: 'Cancelled' },
    })
    expect(cancelled.ok()).toBeTruthy()

    const releasedCar = await request.get(`${gatewayUrl}/catalog/api/cars/${car.id}`)
    expect((await releasedCar.json()).isAvailable).toBe(true)
  } finally {
    if (orderId) {
      await request.patch(`${gatewayUrl}/orders/api/orders/${orderId}/status`, { headers: adminHeaders, data: { status: 'Cancelled' } })
      await request.delete(`${gatewayUrl}/orders/api/orders/${orderId}`, { headers: adminHeaders })
    }
  }
})
