import { useEffect, useState } from 'react'
import { Alert, Badge, Button, Card, Col, Container, Row } from 'react-bootstrap'
import { useNavigate, useParams } from 'react-router-dom'
import { getCarById } from '../api/cars'
import BackButton from '../components/common/BackButton'
import ImageGallery from '../components/common/ImageGallery'
import { useAuth } from '../context/AuthContext'
import { useCart } from '../context/CartContext'
import { formatPrice } from '../utils/cart.js'
import '../style/style.css'

export default function CarDetailsPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const { addToCart } = useCart()
  const [car, setCar] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => { getCarById(id).then(setCar).catch(() => setError('Ошибка загрузки автомобиля')).finally(() => setLoading(false)) }, [id])
  if (loading) return <Loader />
  if (error || !car) return <Container className="py-5"><Alert variant="danger">{error || 'Автомобиль не найден'}</Alert></Container>

  const available = car.isAvailable !== false
  const images = car.images?.map((image) => image.url).filter(Boolean) || []
  if (!images.length) images.push('/noPhoto.png')
  const add = () => {
    if (!user) return navigate('/login')
    if (addToCart({ ...car, type: 'car' })) navigate('/cart')
  }

  return <Container className="py-5 details-page"><div className="mb-4"><BackButton /></div>
    <Card className="details-card border-0"><Row className="g-0">
      <Col lg={7} className="details-gallery-column"><ImageGallery images={images} alt={`${car.brandName} ${car.modelName}`} /></Col>
      <Col lg={5}><div className="details-info">
        <div className="d-flex gap-2 flex-wrap mb-3"><Badge bg="secondary">{car.year || 'Год не указан'}</Badge>
          {car.bodyType && <Badge bg="dark">{car.bodyType}</Badge>}<Badge bg={available ? 'success' : 'warning'}>{available ? 'Доступен' : 'В заказе'}</Badge></div>
        <h1>{car.brandName || 'Бренд'} {car.modelName || 'Модель'}</h1><p className="details-subtitle">{car.generationName}</p>
        <div className="details-specs"><div><span>Цена</span><strong className="details-price">{formatPrice(car.price)}</strong></div>
          <div><span>Пробег</span><strong>{Intl.NumberFormat('ru-RU').format(car.mileage || 0)} км</strong></div>
          <div><span>Двигатель</span><strong>{[car.engineVolume, car.fuelType].filter(Boolean).join(', ') || '—'}</strong></div>
          <div><span>КПП / привод</span><strong>{[car.transmissionType, car.driveType].filter(Boolean).join(', ') || '—'}</strong></div>
          <div><span>Комплектация</span><strong>{car.trimLevelName || '—'}</strong></div></div>
        {car.trimLevelDescription && <div className="details-description"><h5>О комплектации</h5><p>{car.trimLevelDescription}</p></div>}
        <Button className="mt-3 w-100" disabled={!available} onClick={add}>{available ? 'Добавить в корзину' : 'Автомобиль уже в заказе'}</Button>
      </div></Col>
    </Row></Card>
  </Container>
}

function Loader() { return <div className="centered-loader-overlay"><img src="/loading.gif" alt="Загрузка" className="centered-loader-image" /></div> }
