import { Container, Row, Col, Card, Button } from "react-bootstrap"
import { motion } from "framer-motion"
import { FiArrowRight, FiCheckCircle, FiMapPin, FiMessageCircle, FiShield, FiShoppingBag } from "react-icons/fi"
import { Link } from "react-router-dom"
import "../style/style.css"

const mapUrl = import.meta.env.VITE_YANDEX_MAP_URL || "https://yandex.ru/map-widget/v1/?um=constructor%3Aac00a063241e9da97f98bcea9b89e777c1d60a873abc1c58ecbaf44501ff73f7&amp;source=constructor"

export default function AboutPage() {
    return (
        <div className="about-page">
            <Container className="py-5">
                <motion.section className="about-intro" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: .6 }}>
                    <h1>Автомобильный маркетплейс,<br /><span>который экономит ваше время.</span></h1>
                    <p>Мы собрали автомобили и запчасти в одном понятном каталоге, чтобы путь от поиска до покупки был максимально простым.</p>
                </motion.section>

                <Row className="g-4 mb-5">
                    <Col md={4}>
                        <Card className="about-feature border-0 h-100">
                            <FiShoppingBag />
                            <h3>Автомобили</h3>
                            <p>Подробные карточки автомобилей с основными характеристиками и фотографиями.</p>
                        </Card>
                    </Col>
                    <Col md={4}>
                        <Card className="about-feature border-0 h-100">
                            <FiShield />
                            <h3>Запчасти</h3>
                            <p>Поиск по артикулам, производителям, категориям и заменяемым номерам.</p>
                        </Card>
                        </Col>
                    <Col md={4}>
                        <Card className="about-feature border-0 h-100">
                            <FiMessageCircle />
                            <h3>Поддержка</h3>
                            <p>Если не нашли нужную позицию, можно связаться с нами и уточнить наличие.</p>
                        </Card>
                    </Col>
                </Row>

                <section className="about-story">
                    <Row className="align-items-center g-5">
                        <Col lg={6}><span className="eyebrow">КАК МЫ РАБОТАЕМ</span>
                            <h2>Меньше лишних действий.<br />Больше полезной информации.</h2>
                            <div className="about-checks">
                                <p><FiCheckCircle /> Выбираете нужный раздел</p>
                                <p><FiCheckCircle /> Находите подходящий вариант через поиск</p>
                                <p><FiCheckCircle /> Открываете карточку и изучаете характеристики</p>
                                <p><FiCheckCircle /> Связываетесь с нами для уточнения деталей</p>
                            </div>
                        </Col>
                        <Col lg={6}>
                            <div className="about-quote">
                                <span>Наш принцип</span>
                                <strong>Понятный каталог должен помогать принять решение, а не усложнять его.</strong>
                            </div>
                        </Col>
                    </Row>
                </section>

                <section className="about-location">
                    <Row className="g-4 align-items-stretch">
                        <Col lg={5}>
                            <div className="about-location__info">
                                <span className="eyebrow">КОНТАКТЫ</span>
                                <h2>Мы на карте</h2>
                                <p>Здесь можно указать точный адрес, режим работы и способы связи с ASPOTUS.</p>
                                <div className="about-contact-item">
                                    <FiMapPin />
                                    <div>
                                        <strong>Адрес</strong>
                                        <span>Краснодар</span>
                                    </div>
                                </div>
                                <div className="about-contact-item">
                                    <FiMessageCircle />
                                    <div>
                                        <strong>Связь</strong>
                                        <span>Уточняйте актуальные контакты у менеджера</span>
                                    </div></div><Button as={Link} to="/cars">Перейти в каталог
                                    <FiArrowRight />
                                </Button>
                            </div>
                        </Col>
                        <Col lg={7}>
                            <div className="yandex-map">
                                <iframe src={mapUrl} title="ASPOTUS на карте" loading="lazy" allowFullScreen />
                            </div>
                        </Col>
                    </Row>
                </section>
            </Container>
        </div>
    )
}
