import { Pagination as BootstrapPagination } from "react-bootstrap"

export default function Pagination({ page, totalPages, onPageChange }) {
    if (totalPages <= 1) return null

    const items = []
    const addPage = (value) => {
        if (!items.includes(value)) items.push(value)
    }

    addPage(1)
    for (let i = Math.max(2, page - 1); i <= Math.min(totalPages - 1, page + 1); i += 1) addPage(i)
    if (totalPages > 1) addPage(totalPages)

    const rendered = []
    let previous = null

    items.forEach((item) => {
        if (previous !== null && item - previous > 1) {
            rendered.push(<BootstrapPagination.Ellipsis key={`ellipsis-${item}`} disabled />)
        }
        rendered.push(
            <BootstrapPagination.Item
                key={item}
                active={item === page}
                onClick={() => onPageChange(item)}
            >
                {item}
            </BootstrapPagination.Item>
        )
        previous = item
    })

    return (
        <div className="catalog-pagination">
            <BootstrapPagination className="mb-0">
                <BootstrapPagination.First disabled={page === 1} onClick={() => onPageChange(1)} />
                <BootstrapPagination.Prev disabled={page === 1} onClick={() => onPageChange(page - 1)} />
                {rendered}
                <BootstrapPagination.Next disabled={page === totalPages} onClick={() => onPageChange(page + 1)} />
                <BootstrapPagination.Last disabled={page === totalPages} onClick={() => onPageChange(totalPages)} />
            </BootstrapPagination>
        </div>
    )
}
