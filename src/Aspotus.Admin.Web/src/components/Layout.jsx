import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import './Layout.css'
import { getUserRoles } from '../utils/auth.js'

const navItems = [
    { to: '/', label: 'Dashboard', end: true, roles: ['Admin', 'Operator', 'ContentModerator'] },
    { to: '/users', label: 'Пользователи', roles: ['Admin'] },
    { to: '/categories', label: 'Категории', roles: ['Admin', 'ContentModerator'] },
    { to: '/brands', label: 'Бренды и модели', roles: ['Admin', 'ContentModerator'] },
    { to: '/generations', label: 'Поколения', roles: ['Admin', 'ContentModerator'] },
    { to: '/manufacturers', label: 'Производители', roles: ['Admin', 'ContentModerator'] },
    { to: '/cars', label: 'Автомобили', roles: ['Admin', 'ContentModerator'] },
    { to: '/parts', label: 'Запчасти', roles: ['Admin', 'ContentModerator'] },
    { to: '/orders', label: 'Заказы', roles: ['Admin', 'Operator'] },
]

export default function Layout() {
  const navigate = useNavigate()
  const userRoles = getUserRoles()
  const visibleNavItems = navItems.filter((item) =>
    item.roles.some((role) => userRoles.includes(role)))

  const handleLogout = () => {
    localStorage.removeItem('token')
    navigate('/login')
  }

  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h2>Aspotus Admin</h2>
        </div>
        <nav className="sidebar-nav">
          {visibleNavItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={({ isActive }) =>
                `nav-link${isActive ? ' active' : ''}`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      <div className="main-area">
        <header className="topbar">
          <span className="topbar-title">Admin Panel</span>
          <div className="topbar-right">
            <span className="topbar-user">{localStorage.getItem('fullName')}</span>
            <button className="logout-btn" onClick={handleLogout}>
              Logout
            </button>
          </div>
        </header>

        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
