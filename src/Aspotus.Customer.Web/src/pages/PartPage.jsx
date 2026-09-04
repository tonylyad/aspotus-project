import { useEffect, useState } from 'react'
import { Alert, Container } from 'react-bootstrap'
import PartList from '../components/parts/PartList'
import PartSearch from '../components/parts/PartSearch'
import Pagination from '../components/common/Pagination'
import EmptyState from '../components/common/EmptyState'
import { getPartsPage } from '../api/parts'
import '../style/style.css'

const PAGE_SIZE = 9

export default function PartPage() {
  const [parts, setParts] = useState([])
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
    const timer = setTimeout(() => getPartsPage({ page, pageSize: PAGE_SIZE, query })
      .then((data) => {
        if (cancelled) return
        setParts(data.items || [])
        setTotalCount(data.totalCount || 0)
        setTotalPages(data.totalPages || 1)
      })
      .catch(() => !cancelled && setError('Ошибка загрузки запчастей'))
      .finally(() => !cancelled && setLoading(false)), 250)
    return () => { cancelled = true; clearTimeout(timer) }
  }, [page, query])

  if (loading && parts.length === 0) return <CatalogLoader />
  if (error) return <Container className="py-5"><Alert variant="danger">{error}</Alert></Container>

  return <Container className="py-5 catalog-page">
    <div className="catalog-heading"><div><h1>Запчасти</h1><p>Ищите по названию, OEM-номеру, заменам и производителю.</p></div>
      <div className="catalog-count"><strong>{totalCount}</strong><span>позиций</span></div></div>
    <PartSearch query={query} onQueryChange={(value) => { setQuery(value); setPage(1) }} resultCount={totalCount} totalCount={totalCount} />
    {parts.length ? <PartList parts={parts} /> : <EmptyState text="Попробуйте другой артикул, название или производителя." />}
    <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
  </Container>
}

function CatalogLoader() {
  return <div className="centered-loader-overlay"><div className="loader-content">
    <img src="/loading.gif" alt="Загрузка" className="centered-loader-image" /><p className="loader-text">Пожалуйста, подождите...</p>
  </div></div>
}
