import { motion } from "framer-motion";
import { Form, Button, Container, Row, Col, Card, Alert } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function Login() {
    const { login } = useAuth();
    const [form, setForm] = useState({ login: "", password: "" });
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        try {
            await login(form);
            navigate("/");
        } catch (err) {
            let message = "Произошла ошибка при входе. Попробуйте позже.";

            if (err.response?.data?.message) {
                message = err.response.data.message;
            }

            setError(message);
        }
    };

    return (
        <Container>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
                <Row className="justify-content-center mt-4">
                    <Col md={8} lg={6} xl={5}>
                        <Card className="auth-container auth-card">
                            <Card.Body>
                                <h2 className="auth-title">Вход в систему</h2>

                                {error && (
                                    <Alert variant="danger" className="mb-3">
                                        {error}
                                    </Alert>
                                )}

                                <Form onSubmit={handleSubmit}>
                                    <Form.Group className="form-group-custom" controlId="formLogin">
                                        <Form.Label className="form-label-custom">Логин</Form.Label>
                                        <Form.Control
                                            type="text"
                                            name="login"
                                            value={form.login}
                                            onChange={(e) => setForm({ ...form, login: e.target.value })}
                                            placeholder="Введите ваш логин"
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="form-group-custom" controlId="formPassword">
                                        <Form.Label className="form-label-custom">Пароль</Form.Label>
                                        <Form.Control
                                            type="password"
                                            name="password"
                                            value={form.password}
                                            onChange={(e) => setForm({ ...form, password: e.target.value })}
                                            placeholder="Введите пароль"
                                            required
                                        />
                                    </Form.Group>

                                    <Button
                                        variant="primary"
                                        type="submit"
                                        className="btn-auth btn-primary-auth w-100"
                                        size="lg"
                                    >
                                        Войти
                                    </Button>
                                </Form>

                                <div className="text-center mt-4">
                                    <p className="auth-switch-text">Нет аккаунта?</p>
                                    <Link to="/register" className="btn btn-outline-secondary btn-outline-auth">
                                        Зарегистрироваться
                                    </Link>
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </motion.div>
        </Container>
    );
}
