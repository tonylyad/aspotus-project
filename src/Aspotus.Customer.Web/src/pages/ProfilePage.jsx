import { useAuth } from "../context/AuthContext";
import { useEffect, useState } from "react";
import { Container, Row, Col, Card, Button, Alert} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { getMyOrders } from "../api/auth";


const Profile = () => {
    const { user, isLoading } = useAuth();
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        if (isLoading) return;

        if (!user?.id) {
            setLoading(false);
            return;
        }

        getMyOrders(user.id)
            .then((response) => setOrders(response.data))
            .catch((error) => console.error(error))
            .finally(() => setLoading(false));
    }, [user?.id, isLoading]);

    if (isLoading || loading) {
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
        );
    }


    if (!user) return <Alert>Ошибка авторизации</Alert>;

    return (
        <Container className="mt-5">
            <Row>

                <Col md={4}>
                    <Card className="shadow-sm p-3">
                        <h4>Профиль</h4>
                        <p><b>Имя:</b> {user.fullName || user.name || "—"}</p>
                        <p><b>Email:</b> {user.email}</p>
                    </Card>
                </Col>

                <Col md={8}>
                    <h4 className="mb-3">Мои заказы</h4>

                    {orders.length > 0 ? (
                        orders.map(order => (
                            <Card key={order.id} className="mb-3 shadow-sm">
                                <Card.Body className="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h6>Заказ #{order.id.slice(0, 8)}</h6>
                                        <div>💰 {order.totalAmount} ₸</div>
                                        <div>📅 {new Date(order.createdAtUtc).toLocaleString()}</div>
                                    </div>

                                    <Button
                                        variant="dark"
                                        onClick={() => navigate(`/orders/${order.id}`)}
                                    >
                                        Открыть
                                    </Button>
                                </Card.Body>
                            </Card>
                        ))
                    ) : (
                        <p>У тебя пока нет заказов</p>
                    )}
                </Col>
            </Row>
        </Container>
    );
};

export default Profile;
