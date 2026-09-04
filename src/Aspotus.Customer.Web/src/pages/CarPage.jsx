import { useEffect, useState } from 'react'
import { Alert, Container } from 'react-bootstrap'
import CarList from '../components/cars/CarList'
import CarSearch from '../components/cars/CarSearch'
import Pagination from '../components/common/Pagination'
import EmptyState from '../components/common/EmptyState'
import { getCarsPage } from '../api/cars'
import '../style/style.css'

const PAGE_SIZE = 9

export default function CarPage() {
  const [cars, setCars] = useState([])
  const [query, setQuery] = useState('')
  const [page, setPage] = useState(1)
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError(null)
    const timer = setTimeout(() => getCarsPage({ page, pageSize: PAGE_SIZE, query })
      .then((data) => {
        if (cancelled) return
        setCars(data.items || [])
        setTotalCount(data.totalCount || 0)
        setTotalPages(data.totalPages || 1)
      })
      .catch(() => !cancelled && setError('Ошибка загрузки автомобилей'))
      .finally(() => !cancelled && setLoading(false)), 250)
    return () => { cancelled = true; clearTimeout(timer) }
  }, [page, query])

  if (loading && cars.length === 0) return <CatalogLoader />
  if (error) return <Container className="py-5"><Alert variant="danger">{error}</Alert></Container>

  return <Container className="py-5 catalog-page">
    <div className="catalog-heading"><div><span className="eyebrow">ASPOTUS CATALOG</span><h1>Автомобили</h1>
      <p>Найдите автомобиль по марке, модели, году или характеристикам.</p></div>
      <div className="catalog-count"><strong>{totalCount}</strong><span>предложений</span></div></div>
    <CarSearch query={query} onQueryChange={(value) => { setQuery(value); setPage(1) }} resultCount={totalCount} totalCount={totalCount} />
    {cars.length ? <CarList cars={cars} /> : <EmptyState title="Автомобили не найдены" text="Измените запрос и попробуйте снова." />}
    <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
  </Container>
}

function CatalogLoader() {
  return <div className="centered-loader-overlay"><div className="loader-content">
    <img src="/loading.gif" alt="Загрузка" className="centered-loader-image" /><p className="loader-text">Пожалуйста, подождите...</p>
  </div></div>
}
