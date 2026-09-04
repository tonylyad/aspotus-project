import { useEffect, useState } from 'react'
import {
  Box,
  Card,
  CardActionArea,
  CardContent,
  Chip,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined'
import GroupOutlinedIcon from '@mui/icons-material/GroupOutlined'
import NotificationsActiveOutlinedIcon from '@mui/icons-material/NotificationsActiveOutlined'
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined'
import PaymentOutlinedIcon from '@mui/icons-material/PaymentOutlined'
import AssignmentIndOutlinedIcon from '@mui/icons-material/AssignmentIndOutlined'
import PhoneInTalkOutlinedIcon from '@mui/icons-material/PhoneInTalkOutlined'
import { adminDashboardMock, operatorDashboardMock } from '../mocks/dashboardMock.js'
import { isAdmin, isContentModerator, isOperator } from '../utils/auth.js'

function formatNotificationTime(isoDate) {
  return new Date(isoDate).toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function MetricCard({ title, value, subtitle, icon }) {
  return (
    <Card sx={{ borderRadius: 3, boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)' }}>
      <CardContent>
        <Stack direction="row" justifyContent="space-between" alignItems="flex-start" spacing={2}>
          <Box>
            <Typography variant="body2" color="text.secondary">{title}</Typography>
            <Typography variant="h4" sx={{ mt: 0.75, fontWeight: 700 }}>{value}</Typography>
            <Typography variant="caption" color="text.secondary">{subtitle}</Typography>
          </Box>
          <Box
            sx={{
              width: 44,
              height: 44,
              borderRadius: 2,
              backgroundColor: 'rgba(25, 118, 210, 0.1)',
              color: 'primary.main',
              display: 'grid',
              placeItems: 'center',
            }}
          >
            {icon}
          </Box>
        </Stack>
      </CardContent>
    </Card>
  )
}

function formatAmount(value) {
  return `${Number(value).toLocaleString('ru-RU')} ₽`
}

function formatDate(value) {
  return new Date(value).toLocaleDateString('ru-RU')
}

export default function Dashboard() {
  const operator = isOperator()
  const admin = isAdmin()
  const contentModerator = isContentModerator()
  const [unpaidStatusFilter, setUnpaidStatusFilter] = useState('all')
  const [myOrdersSearch, setMyOrdersSearch] = useState('')
  const [adminMetrics, setAdminMetrics] = useState({ totalOrders: 0, totalUsers: 0, operators: 0 })
  const [adminMetricsLoading, setAdminMetricsLoading] = useState(admin)
  const [adminMetricsError, setAdminMetricsError] = useState('')

  useEffect(() => {
    if (!admin) return undefined

    const controller = new AbortController()
    const token = localStorage.getItem('token')
    const requestOptions = {
      headers: { Authorization: `Bearer ${token}` },
      signal: controller.signal,
    }

    async function loadAdminMetrics() {
      try {
        const [ordersResponse, usersResponse] = await Promise.all([
          fetch('/orders/api/orders', requestOptions),
          fetch('/api/users', requestOptions),
        ])

        if (!ordersResponse.ok || !usersResponse.ok) {
          throw new Error('Не удалось загрузить показатели Dashboard')
        }

        const [orders, users] = await Promise.all([
          ordersResponse.json(),
          usersResponse.json(),
        ])

        setAdminMetrics({
          totalOrders: orders.length,
          totalUsers: users.length,
          operators: users.filter((user) => user.roles?.includes('Operator')).length,
        })
      } catch (error) {
        if (error.name !== 'AbortError') {
          setAdminMetricsError(error.message)
        }
      } finally {
        if (!controller.signal.aborted) {
          setAdminMetricsLoading(false)
        }
      }
    }

    loadAdminMetrics()
    return () => controller.abort()
  }, [admin])

  if (operator) {
    // TODO: Replace with API request for operator dashboard data.
    const data = operatorDashboardMock
    const unpaidOrders = data.unpaidOrdersTable.filter((row) => unpaidStatusFilter === 'all' || row.status === unpaidStatusFilter)
    const myOrders = data.myOrdersTable.filter((row) => {
      if (!myOrdersSearch) return true
      const query = myOrdersSearch.toLowerCase().trim()
      return row.id.toLowerCase().includes(query) || row.customer.toLowerCase().includes(query)
    })

    return (
      <Box>
        <Typography variant="h5" fontWeight={700} mb={0.5}>Dashboard оператора</Typography>
        <Typography variant="body2" color="text.secondary" mb={3}>
          Оперативные показатели, клиентские обращения и заказы в работе.
        </Typography>

        <Box
          sx={{
            display: 'grid',
            gap: 2,
            gridTemplateColumns: { xs: '1fr', md: '1fr 1fr', xl: '1fr 1fr 1fr 1fr' },
            mb: 3,
          }}
        >
          <MetricCard title="Всего заказов" value={data.metrics.totalOrders} subtitle="За все время" icon={<ReceiptLongOutlinedIcon fontSize="small" />} />
          <MetricCard title="Новые заказы" value={data.metrics.newOrders} subtitle="За текущую смену" icon={<ShoppingCartOutlinedIcon fontSize="small" />} />
          <MetricCard title="Неоплаченные" value={data.metrics.unpaidOrders} subtitle="Требуют контроль" icon={<PaymentOutlinedIcon fontSize="small" />} />
          <MetricCard title="Мои заказы" value={data.metrics.myOrders} subtitle="Назначены на меня" icon={<AssignmentIndOutlinedIcon fontSize="small" />} />
        </Box>

        <Card sx={{ borderRadius: 3, boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)', mb: 3 }}>
          <CardContent>
            <Stack direction="row" alignItems="center" spacing={1} mb={2}>
              <PhoneInTalkOutlinedIcon color="primary" fontSize="small" />
              <Typography variant="h6" fontWeight={700}>Обращения клиентов</Typography>
            </Stack>
            <Stack spacing={1.25}>
              {data.customerCallbacks.map((item) => (
                <Box key={item.id} sx={{ p: 1.5, borderRadius: 2, border: '1px solid', borderColor: 'divider' }}>
                  <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" spacing={0.5}>
                    <Typography fontWeight={600}>{item.name}</Typography>
                    <Typography variant="body2" color="text.secondary">{item.phone}</Typography>
                  </Stack>
                  <Typography variant="caption" color="text.secondary">Перезвонить</Typography>
                </Box>
              ))}
            </Stack>
          </CardContent>
        </Card>

        <Box sx={{ display: 'grid', gap: 2.5, gridTemplateColumns: { xs: '1fr', xl: '1fr 1fr' } }}>
          <Card sx={{ borderRadius: 3, boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)' }}>
            <CardContent>
              <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" spacing={1.5} mb={1.5}>
                <Typography variant="h6" fontWeight={700}>Неоплаченные заказы</Typography>
                <FormControl size="small" sx={{ minWidth: 220 }}>
                  <InputLabel>Фильтр по статусу</InputLabel>
                  <Select
                    value={unpaidStatusFilter}
                    label="Фильтр по статусу"
                    onChange={(e) => setUnpaidStatusFilter(e.target.value)}
                  >
                    <MenuItem value="all">Все</MenuItem>
                    <MenuItem value="Ожидает оплату">Ожидает оплату</MenuItem>
                    <MenuItem value="Частично оплачено">Частично оплачено</MenuItem>
                    <MenuItem value="Просрочка 1 день">Просрочка 1 день</MenuItem>
                  </Select>
                </FormControl>
              </Stack>
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>№ заказа</TableCell>
                      <TableCell>Клиент</TableCell>
                      <TableCell align="right">Сумма</TableCell>
                      <TableCell>Срок оплаты</TableCell>
                      <TableCell>Статус</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {unpaidOrders.map((row) => (
                      <TableRow key={row.id} hover>
                        <TableCell>{row.id}</TableCell>
                        <TableCell>{row.customer}</TableCell>
                        <TableCell align="right">{formatAmount(row.amount)}</TableCell>
                        <TableCell>{formatDate(row.dueDate)}</TableCell>
                        <TableCell>{row.status}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>

          <Card sx={{ borderRadius: 3, boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)' }}>
            <CardContent>
              <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" spacing={1.5} mb={1.5}>
                <Typography variant="h6" fontWeight={700}>Мои заказы</Typography>
                <TextField
                  size="small"
                  value={myOrdersSearch}
                  onChange={(e) => setMyOrdersSearch(e.target.value)}
                  placeholder="Поиск по № или клиенту"
                  sx={{ minWidth: 260 }}
                />
              </Stack>
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>№ заказа</TableCell>
                      <TableCell>Клиент</TableCell>
                      <TableCell>Тип</TableCell>
                      <TableCell align="right">Сумма</TableCell>
                      <TableCell>Этап</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {myOrders.map((row) => (
                      <TableRow key={row.id} hover>
                        <TableCell>{row.id}</TableCell>
                        <TableCell>{row.customer}</TableCell>
                        <TableCell>{row.type}</TableCell>
                        <TableCell align="right">{formatAmount(row.amount)}</TableCell>
                        <TableCell>{row.stage}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Box>

        {/* TODO: Add row actions and navigation to order card after backend routes are finalized. */}
      </Box>
    )
  }

  const data = {
    ...adminDashboardMock,
    metrics: adminMetrics,
  }

  if (contentModerator) {
    const sections = [
      { title: 'Категории', to: '/categories' },
      { title: 'Автомобили', to: '/cars' },
      { title: 'Бренды и модели', to: '/brands' },
      { title: 'Поколения', to: '/generations' },
      { title: 'Производители', to: '/manufacturers' },
      { title: 'Запчасти', to: '/parts' },
    ]

    return (
      <Box>
        <Typography variant="h5" fontWeight={700} mb={0.5}>Панель модератора контента</Typography>
        <Typography variant="body2" color="text.secondary" mb={3}>
          Управление справочниками и товарным каталогом Aspotus.
        </Typography>
        <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', lg: 'repeat(3, 1fr)' } }}>
          {sections.map((section) => (
            <Card
              key={section.to}
              sx={{
                borderRadius: 3,
                boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)',
                transition: 'transform 160ms ease, box-shadow 160ms ease',
                '&:hover': {
                  transform: 'translateY(-3px)',
                  boxShadow: '0 12px 32px rgba(15, 23, 42, 0.12)',
                },
              }}
            >
              <CardActionArea
                component={RouterLink}
                to={section.to}
                sx={{ height: '100%', color: 'inherit', '&:hover': { textDecoration: 'none' } }}
              >
                <CardContent>
                  <Typography fontWeight={700}>{section.title}</Typography>
                  <Typography variant="body2" color="text.secondary" mt={0.5}>
                    Просмотр, создание, редактирование и удаление записей.
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          ))}
        </Box>
      </Box>
    )
  }

  return (
    <Box>
      <Typography variant="h5" fontWeight={700} mb={0.5}>Dashboard администратора</Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        Краткий срез ключевых метрик и системных событий.
      </Typography>

      <Box
        sx={{
          display: 'grid',
          gap: 2.5,
          gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' },
          mb: 3,
        }}
      >
        <Box>
          <MetricCard
            title="Всего заказов"
            value={adminMetricsLoading ? '—' : data.metrics.totalOrders.toLocaleString('ru-RU')}
            subtitle="За все время"
            icon={<ShoppingCartOutlinedIcon fontSize="small" />}
          />
        </Box>
        <Box>
          <MetricCard
            title="Пользователи"
            value={adminMetricsLoading ? '—' : data.metrics.totalUsers.toLocaleString('ru-RU')}
            subtitle="Зарегистрировано"
            icon={<GroupOutlinedIcon fontSize="small" />}
          />
        </Box>
        <Box>
          <MetricCard
            title="Операторы"
            value={adminMetricsLoading ? '—' : data.metrics.operators.toLocaleString('ru-RU')}
            subtitle="Зарегистрировано"
            icon={<AssignmentIndOutlinedIcon fontSize="small" />}
          />
        </Box>
      </Box>

      {adminMetricsError && (
        <Typography color="error" variant="body2" mb={2}>{adminMetricsError}</Typography>
      )}

      <Card sx={{ borderRadius: 3, boxShadow: '0 8px 28px rgba(15, 23, 42, 0.06)' }}>
        <CardContent>
          <Stack direction="row" alignItems="center" spacing={1} mb={2}>
            <NotificationsActiveOutlinedIcon color="primary" fontSize="small" />
            <Typography variant="h6" fontWeight={700}>Системные уведомления</Typography>
          </Stack>

          <Stack spacing={1.5}>
            {data.notifications.map((item) => (
              <Box
                key={item.id}
                sx={{
                  p: 1.5,
                  borderRadius: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  backgroundColor: 'background.paper',
                }}
              >
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1} justifyContent="space-between" mb={0.5}>
                  <Stack direction="row" spacing={1} alignItems="center">
                    <Typography fontWeight={600}>{item.title}</Typography>
                    <Chip size="small" label={item.level} variant="outlined" />
                  </Stack>
                  <Typography variant="caption" color="text.secondary">
                    {formatNotificationTime(item.createdAt)}
                  </Typography>
                </Stack>
                <Typography variant="body2" color="text.secondary">{item.message}</Typography>
              </Box>
            ))}
          </Stack>

          {/* TODO: Add actions (mark as read, filter by level) after backend endpoints are ready. */}
        </CardContent>
      </Card>
    </Box>
  )
}
