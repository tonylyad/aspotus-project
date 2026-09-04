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

export const operatorDashboardMock = {
  metrics: {
    totalOrders: 386,
    newOrders: 24,
    unpaidOrders: 11,
    myOrders: 39,
  },
  customerCallbacks: [
    { id: 'cb-1', name: 'Игорь Смирнов', phone: '+7 901 223 11 00' },
    { id: 'cb-2', name: 'Марина Петрова', phone: '+7 963 700 84 19' },
    { id: 'cb-3', name: 'Александр Котов', phone: '+7 902 119 43 52' },
    { id: 'cb-4', name: 'Екатерина Орлова', phone: '+7 999 451 27 06' },
  ],
  unpaidOrdersTable: [
    { id: 'ORD-10542', customer: 'ООО АвтоРитм', amount: 148000, dueDate: '2026-05-28', status: 'Ожидает оплату' },
    { id: 'ORD-10558', customer: 'ИП Соколов', amount: 32250, dueDate: '2026-05-27', status: 'Частично оплачено' },
    { id: 'ORD-10574', customer: 'Павел Кузнецов', amount: 89600, dueDate: '2026-05-30', status: 'Ожидает оплату' },
    { id: 'ORD-10591', customer: 'Автоцентр Север', amount: 213400, dueDate: '2026-05-29', status: 'Просрочка 1 день' },
  ],
  myOrdersTable: [
    { id: 'ORD-10603', customer: 'Виктор Лебедев', type: 'Запчасти', amount: 24500, stage: 'Подтверждение' },
    { id: 'ORD-10609', customer: 'ТК Магистраль', type: 'Автомобиль', amount: 980000, stage: 'Согласование оплаты' },
    { id: 'ORD-10611', customer: 'Анна Громова', type: 'Запчасти', amount: 17800, stage: 'Сборка заказа' },
    { id: 'ORD-10615', customer: 'Сергей Уваров', type: 'Запчасти', amount: 43200, stage: 'Подготовка отгрузки' },
  ],
}
