import { useState } from "react";
import { Form, Button, Container, Row, Col, Card, Alert } from "react-bootstrap";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";

import BackButton from "../components/common/BackButton"
import { createCustomerRequest } from "../api/auth"

export default function RequestPage() {
    const navigate = useNavigate();

    const [type, setType] = useState(""); // "car" | "part"
    const [formData, setFormData] = useState({
        customerName: "",
        customerEmail: "",
        customerPhone: "",
        comment: "",
        // car
        brand: "",
        model: "",
        trim: "",
        engine: "",
        transmission: "",
        year: "",
        bodyType: "",
        // part
        article: "",
        partName: "",
        condition: "", // "new" | "used"
        vinOrChassis: ""
    });

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);

    const handleTypeChange = (e) => {
        setType(e.target.value);

        setFormData((prev) => ({
            ...prev,
            brand: "", model: "", trim: "", engine: "", transmission: "", year: "", bodyType: "",
            article: "", partName: "", condition: "", vinOrChassis: ""
        }));
        setError("");
    };

    const handleInputChange = (field, value) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setIsSubmitting(true);

        if (!type) {
            setError("Выберите тип заявки");
            setIsSubmitting(false);
            return;
        }

        const requiredAuto = ["customerName", "customerEmail", "customerPhone", "brand", "model", "year", "bodyType"];
        const requiredSpare = ["customerName", "customerEmail", "customerPhone", "partName", "condition", "vinOrChassis"];

        const toCheck = type === "auto" ? requiredAuto : requiredSpare;

        for (const key of toCheck) {
            if (!formData[key]) {
                setError(`Поле "${key}" обязательно`);
                setIsSubmitting(false);
                return;
            }
        }

        try {
            const payload = {
                type,
                customerName: formData.customerName,
                customerEmail: formData.customerEmail,
                customerPhone: formData.customerPhone,
                comment: formData.comment || null,
                details: Object.fromEntries(Object.entries(formData).filter(([key]) =>
                    !["customerName", "customerEmail", "customerPhone", "comment"].includes(key)))
            };

            await createCustomerRequest(payload);

            setSuccess(true);
            setTimeout(() => {
                navigate(type === "auto" ? "/cars" : "/parts");
            }, 1500);
        } catch (err) {
            setError(err.message || "Не удалось отправить заявку");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Container>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
                <Row className="justify-content-center mt-4">
                    <Col md={8} lg={6} xl={5}>
                        <div className="mb-4">
                            <BackButton />
                        </div>
                        <Card className="auth-container auth-card">
                            <Card.Body>
                                <h2 className="auth-title">Заявка</h2>

                                {error && <Alert variant="danger">{error}</Alert>}
                                {success && <Alert variant="success">Заявка отправлена! Мы свяжемся с вами в ближайшее время.</Alert>}

                                <Form onSubmit={handleSubmit}>
                                    <Form.Group className="form-group-custom" controlId="formSelectType">
                                        <Form.Label className="form-label-custom">Тип запроса</Form.Label>
                                        <Form.Control
                                            as="select"
                                            onChange={handleTypeChange}
                                            value={type}
                                            required
                                        >
                                            <option value="">Выберите тип заявки</option>
                                            <option value="auto">Заявка на авто</option>
                                            <option value="spare">Заявка на запчасть</option>
                                        </Form.Control>
                                    </Form.Group>

                                    {/* Общие поля */}
                                    <Form.Group className="form-group-custom" controlId="formCustomerName">
                                        <Form.Label className="form-label-custom">Имя</Form.Label>
                                        <Form.Control
                                            type="text"
                                            value={formData.customerName}
                                            onChange={(e) => handleInputChange("customerName", e.target.value)}
                                            placeholder="Введите ваше ФИО"
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="form-group-custom" controlId="formCustomerEmail">
                                        <Form.Label className="form-label-custom">Email</Form.Label>
                                        <Form.Control
                                            type="email"
                                            value={formData.customerEmail}
                                            onChange={(e) => handleInputChange("customerEmail", e.target.value)}
                                            placeholder="Введите Email"
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="form-group-custom" controlId="formCustomerPhone">
                                        <Form.Label className="form-label-custom">Телефон</Form.Label>
                                        <Form.Control
                                            type="tel"
                                            value={formData.customerPhone}
                                            onChange={(e) => handleInputChange("customerPhone", e.target.value)}
                                            placeholder="+7 (999) 000-00-00"
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="form-group-custom" controlId="formDeliveryAddress">
                                        <Form.Label className="form-label-custom">Комментарии</Form.Label>
                                        <Form.Control
                                            as="textarea"
                                            rows={3}
                                            value={formData.comment}
                                            onChange={(e) => handleInputChange("comment", e.target.value)}
                                            placeholder="Ваш комментарий к заявке"
                                        />
                                    </Form.Group>

                                    {/* Поля для авто */}
                                    {type === "auto" && (
                                        <>
                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Марка</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.brand}
                                                    onChange={(e) => handleInputChange("brand", e.target.value)}
                                                    required
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Модель</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.model}
                                                    onChange={(e) => handleInputChange("model", e.target.value)}
                                                    required
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Комплектация</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.trim}
                                                    onChange={(e) => handleInputChange("trim", e.target.value)}
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Мотор</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.engine}
                                                    onChange={(e) => handleInputChange("engine", e.target.value)}
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>КПП</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.transmission}
                                                    onChange={(e) => handleInputChange("transmission", e.target.value)}
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Год выпуска</Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    min="1900"
                                                    max="2099"
                                                    value={formData.year}
                                                    onChange={(e) => handleInputChange("year", e.target.value)}
                                                    required
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Тип кузова</Form.Label>
                                                <Form.Control
                                                    as="select"
                                                    value={formData.bodyType}
                                                    onChange={(e) => handleInputChange("bodyType", e.target.value)}
                                                    required
                                                >
                                                    <option value="" disabled>Выберите тип кузова</option>
                                                    <option value="sedan">Седан</option>
                                                    <option value="hatchback">Хэтчбек</option>
                                                    <option value="suv">Кроссовер/SUV</option>
                                                    <option value="wagon">Универсал</option>
                                                    <option value="coupe">Купе</option>
                                                    <option value="convertible">Кабриолет</option>
                                                    <option value="minivan">Минивэн</option>
                                                    <option value="pickup">Пикап</option>
                                                    <option value="other">Другое</option>
                                                </Form.Control>
                                            </Form.Group>
                                        </>
                                    )}

                                    {/* Поля для запчасти */}
                                    {type === "spare" && (
                                        <>
                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Артикул (необязательно)</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.article}
                                                    onChange={(e) => handleInputChange("article", e.target.value)}
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Название детали</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.partName}
                                                    onChange={(e) => handleInputChange("partName", e.target.value)}
                                                    required
                                                />
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>Состояние</Form.Label>
                                                <Form.Control
                                                    as="select"
                                                    value={formData.condition}
                                                    onChange={(e) => handleInputChange("condition", e.target.value)}
                                                    required
                                                >
                                                    <option value="" disabled>Выберите состояние</option>
                                                    <option value="new">Новое</option>
                                                    <option value="used">Б/У</option>
                                                </Form.Control>
                                            </Form.Group>

                                            <Form.Group className="form-group-custom">
                                                <Form.Label>VIN авто или номер шасси</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={formData.vinOrChassis}
                                                    onChange={(e) => handleInputChange("vinOrChassis", e.target.value)}
                                                    placeholder="VIN или номер шасси"
                                                    required
                                                />
                                                <Alert variant="info" className="mt-2 small">
                                                    Пожалуйста, внимательно проверьте VIN/номер шасси перед отправкой.
                                                </Alert>
                                            </Form.Group>
                                        </>
                                    )}

                                    <Button
                                        variant="primary"
                                        type="submit"
                                        className="btn-auth btn-primary-auth w-100"
                                        size="lg"
                                        disabled={isSubmitting}
                                    >
                                        {isSubmitting ? "Отправка заявки..." : "Отправить заявку"}
                                    </Button>
                                </Form>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </motion.div>
        </Container>
    );
}
