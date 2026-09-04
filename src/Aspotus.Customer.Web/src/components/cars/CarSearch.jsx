import { Badge, Button, Form } from "react-bootstrap"
import { FiSearch, FiX } from "react-icons/fi"

export default function CarSearch({ query, onQueryChange, resultCount, totalCount }) {
    const hasQuery = query.trim().length > 0
    return (
        <div className="catalog-search mb-4">
            <div className="catalog-search__box">
                <FiSearch className="catalog-search__icon" aria-hidden="true" />
                <Form.Control
                    value={query}
                    onChange={(event) => onQueryChange(event.target.value)}
                    placeholder="Поиск по марке, модели, поколению, кузову, году..."
                    aria-label="Поиск автомобилей"
                    className="catalog-search__input"
                />
                {hasQuery && (
                    <Button variant="link" className="catalog-search__clear" onClick={() => onQueryChange("")} aria-label="Очистить поиск">
                        <FiX />
                    </Button>
                )}
            </div>
            <div className="catalog-search__meta">
                <span>{hasQuery ? `Найдено: ${resultCount}` : `Всего автомобилей: ${totalCount}`}</span>
                {hasQuery && <Badge bg="secondary">{query}</Badge>}
            </div>
        </div>
    )
}
