import { useState } from 'react'
import { Badge, Button, Card } from 'react-bootstrap'
import { motion } from 'framer-motion'
import { Link, useNavigate } from 'react-router-dom'
import { useImage } from '../../hooks/useImage'
import { useAuth } from '../../context/AuthContext'
import { useCart } from '../../context/CartContext'
import { formatPrice } from '../../utils/cart.js'

export default function PartCard({ part }) {
  const [showAddedMessage, setShowAddedMessage] = useState(false)
  const { user } = useAuth()
  const { addToCart } = useCart()
  const navigate = useNavigate()
  const image = useImage(part.images?.[0]?.url)
  const available = (part.availableStockQuantity ?? part.stockQuantity ?? 0) > 0

  const handleAdd = () => {
    if (!user) return navigate('/login')
    if (!addToCart({ ...part, type: 'part' })) return
    setShowAddedMessage(true)
    setTimeout(() => setShowAddedMessage(false), 2500)
  }

  return <motion.div whileHover={{ scale: 1.03, y: -10 }} className="h-100">
    <Card className="shadow border-0 h-100">
      <div className="image-container"><Card.Img variant="top" src={image} alt={part.name} loading="lazy" />
        <div className="corner-badge">{part.conditionType === 1 ? 'Новое' : 'Б/У'}</div></div>
      <Card.Body>
        <Badge bg={available ? 'success' : 'secondary'}>{available ? `В наличии: ${part.availableStockQuantity ?? part.stockQuantity}` : 'Нет в наличии'}</Badge>
        <Card.Title className="mt-3">{part.name}</Card.Title>
        <Card.Text>Артикул: <strong>{part.article || '—'}</strong></Card.Text>
        <Card.Text className="fs-5 fw-bold">{formatPrice(part.price)}</Card.Text>
        <div className="d-flex justify-content-between align-items-center gap-2">
          <Button as={Link} to={`/parts/${part.id}`}>Подробнее</Button>
          <div className="cart-button-container">
            <div className={`added-to-cart-message ${showAddedMessage ? 'visible' : ''}`}>Добавлено в корзину!</div>
            <Button variant="outline-secondary" size="sm" disabled={!available} onClick={handleAdd}
              title={available ? 'Добавить в корзину' : 'Запчасти нет в наличии'}>
              <img src="/cardIcon.png" alt="Корзина" style={{ width: 32, height: 32 }} />
            </Button>
          </div>
        </div>
      </Card.Body>
    </Card>
  </motion.div>
}
