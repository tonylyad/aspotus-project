import { Badge, Button, Form } from "react-bootstrap"

export default function PartSearch({ query, onQueryChange, resultCount, totalCount }) {
    const hasQuery = query.trim().length > 0

    return (
        <div className="parts-search mb-4">
            <div className="parts-search__box">
                <span className="parts-search__icon" aria-hidden="true">
                    <svg viewBox="0 0 24 24" fill="none">
                        <circle cx="11" cy="11" r="6.5" stroke="currentColor" strokeWidth="2" />
                        <path d="m16 16 4.5 4.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                    </svg>
                </span>

                <Form.Control
                    value={query}
                    onChange={(event) => onQueryChange(event.target.value)}
                    placeholder="Поиск по названию, артикулу или номеру детали..."
                    aria-label="Поиск запчастей"
                    className="parts-search__input"
                />

                {hasQuery && (
                    <Button
                        variant="link"
                        onClick={() => onQueryChange("")}
                        className="parts-search__clear"
                        aria-label="Очистить поиск"
                    >
                        ×
                    </Button>
                )}
            </div>

            <div className="parts-search__meta">
                <span>{hasQuery ? `Найдено: ${resultCount}` : `Всего запчастей: ${totalCount}`}</span>

                {hasQuery && (
                    <Badge bg="secondary" className="parts-search__badge">
                        Поиск: {query}
                    </Badge>
                )}
            </div>
        </div>
    )
}
