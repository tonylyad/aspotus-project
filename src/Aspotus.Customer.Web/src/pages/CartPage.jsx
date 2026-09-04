import { Button, Card, Form } from 'react-bootstrap'
import { useNavigate } from 'react-router-dom'
import { useCart } from '../context/CartContext'
import { formatPrice, getAvailableQuantity } from '../utils/cart.js'

export default function CartPage() {
  const { cars, parts, removeFromCart, updateQuantity, partsTotal, carsTotal } = useCart()
  const navigate = useNavigate()

  return <div className="container mt-4 mb-5">
    <h2>Корзина</h2>
    {cars.length === 0 && parts.length === 0 && <p>Корзина пуста</p>}

    {cars.length > 0 && <section className="mb-5"><h3>Автомобили ({cars.length})</h3>
      {cars.map((item) => <Card key={item.id} className="mb-2 p-3"><div className="d-flex justify-content-between align-items-center gap-3">
        <div><h5>{item.brandName} {item.modelName} {item.generationName}</h5><strong>{formatPrice(item.price)}</strong></div>
        <Button variant="outline-danger" onClick={() => removeFromCart(item.id)}>Удалить</Button>
      </div></Card>)}
      <h4 className="mt-3">Итого: {formatPrice(carsTotal)}</h4>
      <Button onClick={() => navigate('/checkout/cars')}>Оформить автомобиль</Button>
    </section>}

    {parts.length > 0 && <section><h3>Запчасти ({parts.length})</h3>
      {parts.map((item) => <Card key={item.id} className="mb-2 p-3"><div className="d-flex justify-content-between align-items-center gap-3 flex-wrap">
        <div><h5>{item.name}</h5><span>{formatPrice(item.price)} за шт.</span></div>
        <Form.Control type="number" min={1} max={getAvailableQuantity(item)} value={item.quantity}
          onChange={(event) => updateQuantity(item.id, Number(event.target.value))} style={{ width: 90 }} aria-label="Количество" />
        <strong>{formatPrice(item.price * item.quantity)}</strong>
        <Button variant="outline-danger" onClick={() => removeFromCart(item.id)}>Удалить</Button>
      </div></Card>)}
      <h4 className="mt-3">Итого: {formatPrice(partsTotal)}</h4>
      <Button onClick={() => navigate('/checkout/parts')}>Оформить запчасти</Button>
    </section>}
  </div>
}
