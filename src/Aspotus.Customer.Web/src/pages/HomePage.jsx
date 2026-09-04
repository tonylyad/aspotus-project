import { Container, Row, Col, Button, Card, Badge, Alert } from "react-bootstrap"
import { motion } from "framer-motion"
import { Link } from "react-router-dom"
import { useEffect, useState } from "react"
import { FiArrowRight, FiCheckCircle, FiClock, FiMapPin, FiShield, FiShoppingBag, FiTool, FiSearch } from "react-icons/fi"
import { getCarsPage } from "../api/cars"
import { getPartsPage } from "../api/parts"
import "../style/style.css"

export default function HomePage() {
    const [cars, setCars] = useState([])
    const [parts, setParts] = useState([])
    const [carCount, setCarCount] = useState(0)
    const [partCount, setPartCount] = useState(0)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    useEffect(() => {
        Promise.all([getCarsPage({ pageSize: 3 }), getPartsPage({ pageSize: 3 })])
            .then(([carsData, partsData]) => {
                setCars(carsData.items || [])
                setParts(partsData.items || [])
                setCarCount(carsData.totalCount || 0)
                setPartCount(partsData.totalCount || 0)
            })
            .catch((error) => setError(error))
            .finally(() => setLoading(false))
    }, [])

    const previewCars = cars.slice(0, 3)
    const previewParts = parts.slice(0, 3)

    if (loading) {

        return (

            <div className="centered-loader-overlay">
                <div className="loader-content">
                    <img
                        src="/loading.gif"
                        alt="Загружаем данные, пожалуйста подождите"
                        className="centered-loader-image"
                    />
                    <p className="loader-text">Пожалуйста, подождите...</p>
                </div>
            </div>
        )

    }

    if (error) {

        return (

            <Container className="py-5">

                <Alert variant="danger">
                    {error}
                </Alert>

            </Container>

        )

    }

    return (
        <div className="home-page">
            <section className="home-hero">
                <Container>
                    <Row className="align-items-center g-5">
                        <Col lg={7}>
                            <motion.div initial={{ opacity: 0, y: 25 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: .6 }}>
                                <span className="eyebrow">АВТОМОБИЛИ · ЗАПЧАСТИ · СЕРВИС</span>
                                <h1>Всё для автомобиля<br /><span>в одном месте.</span></h1>
                                <p className="home-hero__lead">Подбирайте автомобиль или нужную деталь в каталоге ASPOTUS — быстро, удобно и без лишнего поиска.</p>
                                <div className="home-hero__actions">
                                    <Button as={Link} to="/cars" size="lg">Смотреть автомобили <FiArrowRight /></Button>
                                    <Button as={Link} to="/parts" size="lg" variant="outline-light">Найти запчасть</Button>
                                </div>
                                <div className="home-trust-row">
                                    <span><FiCheckCircle /> Каталог постоянно обновляется</span>
                                    <span><FiShield /> Понятные характеристики</span>
                                </div>
                            </motion.div>
                        </Col>
                        <Col lg={5}>
                            <motion.div className="home-hero__visual" initial={{ opacity: 0, scale: .94 }} animate={{ opacity: 1, scale: 1 }} transition={{ duration: .7 }}>
                                <div className="home-hero__glow" />
                                <img src="/logo.png" alt="ASPOTUS" />
                                <div className="home-hero__floating-card">
                                    <FiShoppingBag />
                                    <div><strong>{carCount + partCount}</strong>
                                        <span>позиций в каталоге</span>
                                    </div>
                                </div>
                            </motion.div>
                        </Col>
                    </Row>
                </Container>
            </section>

            <Container className="home-content">
                <section className="home-stats">
                    <div><strong>{carCount}</strong><span>автомобилей</span></div>
                    <div><strong>{partCount}</strong><span>запчастей</span></div>
                    <div><strong>24/7</strong><span>доступ к каталогу</span></div>
                </section>

                <section className="home-section">
                    <div className="section-head">
                        <div>
                            <span className="eyebrow">КАТАЛОГ</span>
                            <h2>Что ищете сегодня?</h2>
                        </div>
                    </div>
                    <Row className="g-4">
                        <Col md={6}>
                            <Card as={Link} to="/cars" className="home-category-card border-0">
                                <div className="home-category-card__icon">
                                    <FiTool />
                                </div>
                                <div>
                                    <h3>Автомобили</h3>
                                    <p>Подберите автомобиль по основным характеристикам и откройте подробную информацию.</p>
                                    <span>Перейти в каталог
                                        <FiArrowRight />
                                    </span>
                                </div>
                            </Card>
                        </Col>
                        <Col md={6}>
                            <Card as={Link} to="/parts" className="home-category-card border-0">
                                <div className="home-category-card__icon">
                                    <FiShoppingBag />
                                </div>
                                <div>
                                    <h3>Запчасти</h3>
                                    <p>Найдите деталь по названию, OEM-номеру, заменяемому артикулу или производителю.</p>
                                    <span>Перейти в каталог
                                        <FiArrowRight />
                                    </span>
                                </div>
                            </Card>
                        </Col>
                    </Row>
                </section>

                <section className="home-section">
                    <div className="section-head">
                        <div>
                            <span className="eyebrow">НОВЫЕ ПОЗИЦИИ</span>
                            <h2>Последние предложения</h2>
                        </div><Button as={Link} to="/cars" variant="outline-light">Весь каталог
                            <FiArrowRight />
                        </Button>
                    </div>
                    <Row className="g-4">
                        {previewCars.map((car) =>
                            <Col md={4} key={car.id}>
                                <Card as={Link} to={`/cars/${car.id}`} className="home-preview-card border-0">
                                    <div className="home-preview-card__image">
                                        <img src={car.images?.[0]?.url || "/noPhoto.png"} alt="" onError={(e) => { e.currentTarget.src = "/noPhoto.png" }} />
                                        <Badge bg="dark">{car.year}</Badge>
                                    </div>
                                    <Card.Body>
                                        <h4>{car.brandName} {car.modelName}</h4>
                                        <p>{car.generationName || car.bodyType || "Автомобиль"}</p>
                                        <span>Подробнее
                                            <FiArrowRight />
                                        </span>
                                    </Card.Body>
                                </Card>
                            </Col>)}
                        {!previewCars.length && <Col>
                            <div className="catalog-empty">Автомобили пока не добавлены.</div>
                        </Col>}
                    </Row>
                </section>

                <section className="home-section home-section--parts">
                    <div className="section-head"><div>
                        <span className="eyebrow">ЗАПЧАСТИ</span>
                        <h2>Популярные позиции</h2>
                    </div><Button as={Link} to="/parts" variant="outline-light">Все запчасти
                            <FiArrowRight />
                        </Button>
                    </div>
                    <Row className="g-4">
                        {previewParts.map((part) =>
                            <Col md={4} key={part.id}>
                                <Card as={Link} to={`/parts/${part.id}`} className="home-part-card border-0">
                                    <div className="home-preview-card__image">
                                        <img src={part.images?.[0]?.url || "/noPhoto.png"} alt="" onError={(e) => { e.currentTarget.src = "/noPhoto.png" }} />
                                        <Badge bg="dark">{part.isOriginal === true ? "OEM" : "Аналог"}</Badge>
                                    </div>
                                    <Card.Body>
                                        <h4>{part.name}</h4>
                                        <p>Артикул: <strong>{part.article || "—"}</strong></p>
                                        <div className="home-part-card__price">{part.price} ₸</div>
                                        <span>Подробнее
                                            <FiArrowRight />
                                        </span>
                                    </Card.Body>
                                </Card>
                            </Col>)}
                        {!previewParts.length && <Col>
                            <div className="catalog-empty">Запчасти пока не добавлены.</div>
                        </Col>}
                    </Row>
                </section>

                <section className="home-benefits">
                    <div className="section-head">
                        <div>
                            <span className="eyebrow">ПОЧЕМУ ASPOTUS</span>
                            <h2>Сделали каталог проще</h2>
                        </div>
                    </div>
                    <Row className="g-4">
                        <Col md={4}>
                            <div className="benefit">
                                <FiSearch />
                                <h4>Умный поиск</h4>
                                <p>Один запрос — и система ищет сразу по нескольким характеристикам.</p>
                            </div>
                        </Col>
                        <Col md={4}>
                            <div className="benefit">
                                <FiClock />
                                <h4>Быстрый выбор</h4>
                                <p>Карточки и фильтрация помогают быстро сравнивать подходящие варианты.</p>
                            </div>
                        </Col>
                        <Col md={4}>
                            <div className="benefit"><FiMapPin />
                                <h4>Всё рядом</h4>
                                <p>Контакты и расположение можно посмотреть на странице «О нас».</p>
                            </div>
                        </Col>
                    </Row>
                </section>
            </Container>
        </div>
    )
}
