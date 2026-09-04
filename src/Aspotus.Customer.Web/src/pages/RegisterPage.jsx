import { useState } from 'react';

import {
    Container,
    Row,
    Col,
    Card,
    Form,
    Button,
    Alert,
} from 'react-bootstrap';

import { useNavigate } from "react-router-dom"
import { useAuth } from "../context/AuthContext";

const Register = () => {
    const { register } = useAuth();
    const [form, setForm] = useState({ login: "", fullName: "", email: "", phoneNumber: "", password: "" });
    const navigate = useNavigate();
    const [error, setError] = useState("");


    const handleChange = (e) => {
        setForm({
            ...form,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        if (!form.login || !form.fullName || !form.email || !form.password) {
            return setError("Пожалуйста, заполните все поля");
        }

        try {
            await register(form);

            navigate('/');
        } catch (err) {
            console.error(err);

            setError(
                err?.message || "Ошибка регистрации. Попробуйте снова."
            );
        }
    };


    return (
        <Container className="py-5">
            <Row className="justify-content-center">
                <Col md={6}>
                    <Card className="shadow border-0 rounded-4 p-4">
                        <h2 className="mb-4 text-center">
                            Регистрация
                        </h2>

                        {error && <Alert variant="danger">{error}</Alert>}

                        <Form onSubmit={handleSubmit}>
                            <Form.Group className="mb-3">
                                <Form.Label>Логин</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="login"
                                    value={form.login}
                                    onChange={handleChange}
                                    required
                                />
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Label>Имя</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="fullName"
                                    value={form.fullName}
                                    onChange={handleChange}
                                    required
                                />
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Label>Email</Form.Label>
                                <Form.Control
                                    type="email"
                                    name="email"
                                    value={form.email}
                                    onChange={handleChange}
                                    required
                                />
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Label>Телефон</Form.Label>
                                <Form.Control
                                    type="tel"
                                    name="phoneNumber"
                                    value={form.phoneNumber}
                                    onChange={handleChange}
                                />
                            </Form.Group>

                            <Form.Group className="mb-4">
                                <Form.Label>Пароль</Form.Label>
                                <Form.Control
                                    type="password"
                                    name="password"
                                    value={form.password}
                                    onChange={handleChange}
                                    minLength={6}
                                    required
                                />
                            </Form.Group>


                            <Button
                                type="submit"
                                className="w-100"
                            >
                                Зарегистрироваться
                            </Button>
                        </Form>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default Register;
