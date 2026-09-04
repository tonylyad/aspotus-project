import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import { getUserRoles } from './utils/auth.js'
import Dashboard from './pages/Dashboard.jsx'
import Login from './pages/Login.jsx'
import Generations from './pages/Generations.jsx'
import Cars from './pages/Cars.jsx'
import Categories from './pages/Categories.jsx'
import Manufacturers from './pages/Manufacturers.jsx'
import Users from './pages/Users.jsx'
import Parts from './pages/Parts.jsx'
import Brands from './pages/Brands.jsx'
import Orders from './pages/Orders.jsx'

function ProtectedRoute({ children }) {
  const token = localStorage.getItem('token')
  if (!token) {
    return <Navigate to="/login" replace />
  }
  return children
}

function RoleRoute({ roles, children }) {
  const userRoles = getUserRoles()
  return roles.some((role) => userRoles.includes(role))
    ? children
    : <Navigate to="/" replace />
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<Dashboard />} />
        <Route path="/users" element={<RoleRoute roles={['Admin']}><Users /></RoleRoute>} />
        <Route path="/categories" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Categories /></RoleRoute>} />
        <Route path="/cars" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Cars /></RoleRoute>} />
        <Route path="/brands" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Brands /></RoleRoute>} />
        <Route path="/generations" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Generations /></RoleRoute>} />
        <Route path="/manufacturers" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Manufacturers /></RoleRoute>} />
        <Route path="/parts" element={<RoleRoute roles={['Admin', 'ContentModerator']}><Parts /></RoleRoute>} />
        <Route path="/orders" element={<RoleRoute roles={['Admin', 'Operator']}><Orders /></RoleRoute>} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
