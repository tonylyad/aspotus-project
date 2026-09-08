import { useState } from 'react'
import { Badge, Button, Card } from 'react-bootstrap'
import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'
import { useImage } from '../../hooks/useImage'
import { useAuth } from '../../context/AuthContext'
import { useCart } from '../../context/CartContext'
import { formatPrice } from '../../utils/cart.js'

export default function CarCard({ car }) {
  const [showAddedMessage, setShowAddedMessage] = useState(false)
  const { user } = useAuth()
  const { addToCart } = useCart()
  const navigate = useNavigate()
  const image = useImage(car.images?.[0]?.url)
  const available = car.isAvailable !== false

  const handleAdd = () => {
    if (!user) return navigate('/login')
    if (!addToCart({ ...car, type: 'car' })) return
    setShowAddedMessage(true)
    setTimeout(() => setShowAddedMessage(false), 2500)
  }

  const openDetails = () => navigate(`/cars/${car.id}`)

  return <motion.div whileHover={{ y: -8 }} className="h-100">
    <Card
      className="shadow h-100 border-0 catalog-product-card"
      role="link"
      tabIndex={0}
      aria-label={`Открыть автомобиль ${car.brandName || ''} ${car.modelName || ''}`.trim()}
      onClick={openDetails}
      onKeyDown={(event) => {
        if (event.target === event.currentTarget && event.key === 'Enter') openDetails()
      }}
    >
      <div className="image-container">
        <Card.Img variant="top" src={image} alt={`${car.brandName} ${car.modelName}`} loading="lazy" />
        {!available && <div className="corner-badge">Зарезервирован</div>}
      </div>
      <Card.Body>
        <div className="d-flex justify-content-between">
          <Badge bg="dark">{car.brand?.name || car.brandName || 'Без бренда'}</Badge>
          <Badge bg="secondary">{car.year}</Badge>
        </div>
        <Card.Title className="mt-3">{car.model?.name || car.modelName || 'Модель'}</Card.Title>
        <Card.Text>Поколение: {car.generationName || 'не указано'}</Card.Text>
        <Card.Text>Пробег: <strong>{Intl.NumberFormat('ru-RU').format(car.mileage || 0)} км</strong></Card.Text>
        <Card.Text className="fs-5 fw-bold">{formatPrice(car.price)}</Card.Text>
        <div className="d-flex justify-content-end align-items-center gap-2">
          <div className="cart-button-container">
            <div className={`added-to-cart-message ${showAddedMessage ? 'visible' : ''}`}>Добавлено в корзину!</div>
            <Button variant="outline-secondary" size="sm" disabled={!available} onClick={(event) => { event.stopPropagation(); handleAdd() }}
              title={available ? 'Добавить в корзину' : 'Зарезервирован'}>
              <img src="/cardIcon.png" alt="Корзина" style={{ width: 32, height: 32 }} />
            </Button>
          </div>
        </div>
      </Card.Body>
    </Card>
  </motion.div>
}
