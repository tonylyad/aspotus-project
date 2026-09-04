import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Route, Routes, useLocation } from 'react-router-dom'
import Navbar from '../components/layout/Navbar'
import { useAuth } from '../context/AuthContext'

const HomePage = lazy(() => import('../pages/HomePage'))
const CarPage = lazy(() => import('../pages/CarPage'))
const CarDetailsPage = lazy(() => import('../pages/CarDetailsPage'))
const PartPage = lazy(() => import('../pages/PartPage'))
const PartDetailsPage = lazy(() => import('../pages/PartDetailsPage'))
const AboutPage = lazy(() => import('../pages/AboutPage'))
const ProfilePage = lazy(() => import('../pages/ProfilePage'))
const LoginPage = lazy(() => import('../pages/LoginPage'))
const RegisterPage = lazy(() => import('../pages/RegisterPage'))
const CartPage = lazy(() => import('../pages/CartPage'))
const OrderDetails = lazy(() => import('../pages/OrderDetails'))
const CheckoutPage = lazy(() => import('../pages/CheckoutPage'))
const RequestPage = lazy(() => import('../pages/RequestPage'))
const NotFoundPage = lazy(() => import('../pages/NotFoundPage'))

function ProtectedRoute({ children }) {
  const { isAuthenticated, isLoading } = useAuth()
  const location = useLocation()
  if (isLoading) return <PageLoader />
  return isAuthenticated ? children : <Navigate to="/login" replace state={{ from: location }} />
}

function PageLoader() {
  return <div className="centered-loader-overlay"><div className="loader-content">
    <img src="/loading.gif" alt="Загрузка" className="centered-loader-image" />
  </div></div>
}

export default function AppRouter() {
  const protect = (element) => <ProtectedRoute>{element}</ProtectedRoute>
  return <BrowserRouter><Navbar /><Suspense fallback={<PageLoader />}><Routes>
    <Route path="/" element={<HomePage />} />
    <Route path="/cars" element={<CarPage />} /><Route path="/cars/:id" element={<CarDetailsPage />} />
    <Route path="/parts" element={<PartPage />} /><Route path="/parts/:id" element={<PartDetailsPage />} />
    <Route path="/about" element={<AboutPage />} /><Route path="/request" element={<RequestPage />} />
    <Route path="/login" element={<LoginPage />} /><Route path="/register" element={<RegisterPage />} />
    <Route path="/profile" element={protect(<ProfilePage />)} /><Route path="/cart" element={protect(<CartPage />)} />
    <Route path="/orders/:id" element={protect(<OrderDetails />)} /><Route path="/checkout/:type" element={protect(<CheckoutPage />)} />
    <Route path="*" element={<NotFoundPage />} />
  </Routes></Suspense></BrowserRouter>
}
