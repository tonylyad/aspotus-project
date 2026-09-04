import { Button, Container } from 'react-bootstrap'
import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return <Container className="py-5 text-center"><h1>404</h1><p>Такой страницы нет.</p>
    <Button as={Link} to="/">На главную</Button></Container>
}
