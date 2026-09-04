import { lazy, Suspense } from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import { getUserRoles } from './utils/auth.js'

const Dashboard = lazy(() => import('./pages/Dashboard.jsx'))
const Login = lazy(() => import('./pages/Login.jsx'))
const Generations = lazy(() => import('./pages/Generations.jsx'))
const Cars = lazy(() => import('./pages/Cars.jsx'))
const Categories = lazy(() => import('./pages/Categories.jsx'))
const Manufacturers = lazy(() => import('./pages/Manufacturers.jsx'))
const Users = lazy(() => import('./pages/Users.jsx'))
const Parts = lazy(() => import('./pages/Parts.jsx'))
const Brands = lazy(() => import('./pages/Brands.jsx'))
const Orders = lazy(() => import('./pages/Orders.jsx'))
const Requests = lazy(() => import('./pages/Requests.jsx'))

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
    <Suspense fallback={<div style={{ padding: 32 }}>Загрузка…</div>}><Routes>
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
        <Route path="/requests" element={<RoleRoute roles={['Admin', 'Operator']}><Requests /></RoleRoute>} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes></Suspense>
  )
}
