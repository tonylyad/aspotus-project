import { useState } from 'react'
import { Alert, Button, Card, Col, Container, Form, ListGroup, Row } from 'react-bootstrap'
import { motion } from 'framer-motion'
import { useNavigate, useParams } from 'react-router-dom'
import { createCarOrder, createPartOrder } from '../api/auth'
import BackButton from '../components/common/BackButton'
import { useAuth } from '../context/AuthContext'
import { useCart } from '../context/CartContext'
import { formatPrice } from '../utils/cart.js'

export default function CheckoutPage() {
  const { type } = useParams()
  const { cars, parts, removeFromCart, partsTotal, carsTotal } = useCart()
  const { user } = useAuth()
  const navigate = useNavigate()
  const isCars = type === 'cars'
  const items = isCars ? cars : parts
  const total = isCars ? carsTotal : partsTotal
  const [form, setForm] = useState({ customerName: user?.name || '', customerEmail: user?.email || '', customerPhone: '', deliveryAddress: '' })
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    if (!items.length) return setError('В корзине нет товаров для этого заказа.')
    setIsSubmitting(true)
    try {
      const createOrder = isCars ? createCarOrder : createPartOrder
      await createOrder(items, form, user)
      items.forEach((item) => removeFromCart(item.id))
      navigate('/profile')
    } catch (requestError) {
      const data = requestError.response?.data
      let message = data?.message || 'Не удалось оформить заказ. Попробуйте позже.'
      if (data?.errors) message = Array.isArray(data.errors) ? data.errors.join('; ') : Object.values(data.errors).flat().join('; ')
      setError(message)
    } finally { setIsSubmitting(false) }
  }

  const change = (event) => setForm((current) => ({ ...current, [event.target.name]: event.target.value }))

  return <Container>
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
      <Row className="justify-content-center mt-4"><Col md={8} lg={6} xl={5}>
        <div className="mb-4"><BackButton /></div>
        <Card className="auth-container auth-card"><Card.Body>
          <h2 className="auth-title">Оформление заказа: {isCars ? 'автомобиль' : 'запчасти'}</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group className="form-group-custom"><Form.Label>Имя</Form.Label>
              <Form.Control name="customerName" value={form.customerName} onChange={change} required /></Form.Group>
            <Form.Group className="form-group-custom"><Form.Label>Email</Form.Label>
              <Form.Control type="email" name="customerEmail" value={form.customerEmail} onChange={change} required /></Form.Group>
            <Form.Group className="form-group-custom"><Form.Label>Телефон</Form.Label>
              <Form.Control type="tel" name="customerPhone" value={form.customerPhone} onChange={change} placeholder="+7 (999) 000-00-00" required /></Form.Group>
            <Form.Group className="form-group-custom"><Form.Label>Адрес доставки</Form.Label>
              <Form.Control as="textarea" rows={3} name="deliveryAddress" value={form.deliveryAddress} onChange={change} required /></Form.Group>

            <div className="mt-4 mb-3 p-3 bg-dark rounded"><h5>Состав заказа</h5>
              <ListGroup variant="flush">{items.map((item) => <ListGroup.Item key={item.id} className="d-flex justify-content-between align-items-center">
                <div><strong>{isCars ? `${item.brandName} ${item.modelName}` : item.name}</strong>
                  <small className="text-muted d-block">{isCars ? `${item.generationName || ''} ${item.year || ''}` : `Артикул: ${item.article || '—'} · ${item.quantity} шт.`}</small></div>
                <strong>{formatPrice(item.price * (isCars ? 1 : item.quantity))}</strong>
              </ListGroup.Item>)}</ListGroup>
              <div className="d-flex justify-content-between mt-3 fw-bold border-top pt-3"><span>Итого:</span><span>{formatPrice(total)}</span></div>
            </div>

            <Button className="w-100 mt-3" type="submit" disabled={isSubmitting || !items.length}>
              {isSubmitting ? 'Оформляем заказ...' : 'Оформить заказ'}
            </Button>
          </Form>
        </Card.Body></Card>
      </Col></Row>
    </motion.div>
  </Container>
}
