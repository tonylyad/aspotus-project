export const adminDashboardMock = {
  metrics: {
    totalOrders: 1248,
    activeManagers: 17,
  },
  notifications: [
    {
      id: 'n-1',
      level: 'warning',
      title: 'Рост нагрузки на обработку заказов',
      message: 'За последние 2 часа количество новых заказов выросло на 32%.',
      createdAt: '2026-05-26T09:30:00Z',
    },
    {
      id: 'n-2',
      level: 'info',
      title: 'Плановая проверка каталога',
      message: 'Сегодня в 18:00 запланирована сверка карточек товаров и остатков.',
      createdAt: '2026-05-26T07:10:00Z',
    },
    {
      id: 'n-3',
      level: 'error',
      title: 'Сбой синхронизации поставщиков',
      message: 'Один из каналов импорта поставщика вернул ошибку аутентификации.',
      createdAt: '2026-05-26T06:45:00Z',
    },
  ],
}
