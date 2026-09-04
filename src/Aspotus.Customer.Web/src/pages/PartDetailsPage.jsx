import { useEffect, useState } from 'react'
import { Alert, Badge, Button, Card, Col, Container, Row } from 'react-bootstrap'
import { useNavigate, useParams } from 'react-router-dom'
import { getPartById } from '../api/parts'
import BackButton from '../components/common/BackButton'
import ImageGallery from '../components/common/ImageGallery'
import { useAuth } from '../context/AuthContext'
import { useCart } from '../context/CartContext'
import { formatPrice } from '../utils/cart.js'
import '../style/style.css'

export default function PartDetailsPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const { addToCart } = useCart()
  const [part, setPart] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => { getPartById(id).then(setPart).catch(() => setError('Ошибка загрузки запчасти')).finally(() => setLoading(false)) }, [id])
  if (loading) return <Loader />
  if (error || !part) return <Container className="py-5"><Alert variant="danger">{error || 'Запчасть не найдена'}</Alert></Container>

  const availableQuantity = part.availableStockQuantity ?? part.stockQuantity ?? 0
  const images = part.images?.map((image) => image.url).filter(Boolean) || []
  if (!images.length) images.push('/noPhoto.png')
  const replacements = Array.isArray(part.replacementArticles) ? part.replacementArticles : []
  const add = () => {
    if (!user) return navigate('/login')
    if (addToCart({ ...part, type: 'part' })) navigate('/cart')
  }

  return <Container className="py-5 details-page"><div className="mb-4"><BackButton /></div>
    <Card className="details-card border-0"><Row className="g-0">
      <Col lg={7} className="details-gallery-column"><ImageGallery images={images} alt={part.name} /></Col>
      <Col lg={5}><div className="details-info">
        <div className="d-flex gap-2 flex-wrap mb-3"><Badge bg={part.conditionType === 1 ? 'success' : 'warning'}>{part.conditionType === 1 ? 'Новое' : 'Б/У'}</Badge>
          <Badge bg="dark">{part.isOriginal ? 'Оригинал' : 'Аналог'}</Badge><Badge bg={availableQuantity > 0 ? 'success' : 'secondary'}>Доступно: {availableQuantity}</Badge></div>
        <h1>{part.name}</h1><p className="details-subtitle">OEM: {part.article || '—'}</p>
        <div className="details-specs"><div><span>Цена</span><strong className="details-price">{formatPrice(part.price)}</strong></div>
          <div><span>Узел</span><strong>{part.categoryName || '—'}</strong></div>
          {part.manufacturerName && <div><span>Производитель</span><strong>{part.manufacturerName}</strong></div>}
          {part.conditionType === 2 && <div><span>Состояние</span><strong>{part.conditionPercent ?? '—'}%</strong></div>}</div>
        {replacements.length > 0 && <div className="details-description"><h5>Заменяемые артикулы</h5><p>{replacements.join(', ')}</p></div>}
        {part.description && <div className="details-description"><h5>Описание</h5><p>{part.description}</p></div>}
        <Button className="mt-3 w-100" disabled={availableQuantity <= 0} onClick={add}>{availableQuantity > 0 ? 'Добавить в корзину' : 'Нет в наличии'}</Button>
      </div></Col>
    </Row></Card>
  </Container>
}

function Loader() { return <div className="centered-loader-overlay"><img src="/loading.gif" alt="Загрузка" className="centered-loader-image" /></div> }
