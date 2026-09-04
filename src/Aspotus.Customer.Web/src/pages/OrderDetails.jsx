import { useEffect, useState } from "react";
import { Container, Card, Spinner, Button, Badge } from "react-bootstrap";
import { useParams, useNavigate } from "react-router-dom";
import { getOrderById } from "../api/auth";

import BackButton from "../components/common/BackButton"

export default function OrderDetails() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getOrderById(id)
            .then((response) => setOrder(response.data))
            .catch((error) => console.error(error))
            .finally(() => setLoading(false));
    }, [id]);

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <Spinner />
            </Container>
        );
    }

    if (!order) {
        return (
            <Container>
                <p>Заказ не найден</p>
                <Button variant="secondary" onClick={() => navigate("/orders")}>
                    К списку заказов
                </Button>
            </Container>
        );
    }

    const isCarOrder = order.carItems?.length > 0;

    return (
        <Container className="mt-5">

            <div className="mb-4">
                <BackButton/>
            </div>

            <h3 className="mb-4">Заказ #{order.id}</h3>

            <Card className="mb-4 p-3 shadow-sm">
                <h5>Информация</h5>
                <p><b>Имя:</b> {order.customerName}</p>
                <p><b>Email:</b> {order.customerEmail}</p>
                <p><b>Телефон:</b> {order.customerPhone}</p>
                <p><b>Адрес:</b> {order.deliveryAddress}</p>
                <p><b>Дата:</b> {new Date(order.createdAtUtc).toLocaleString()}</p>
                <p><b>Сумма:</b> {order.totalAmount} ₸</p>
            </Card>

            <h5>Состав заказа</h5>

            {isCarOrder ? (
                // Блок для авто (один автомобиль)
                <Card className="mb-2 p-2 shadow-sm" style={{ cursor: 'pointer' }} onClick={() => navigate(`/cars/${order.carItems[0].carId}`)}>
                    <div className="d-flex justify-content-between align-items-center">
                        <div>
                            <Badge variant="primary" className="me-2">Авто</Badge>
                            <b>{order.carItems[0].brandName} {order.carItems[0].modelName}</b>
                            <div className="text-muted">
                                {order.carItems[0].generationName}{order.carItems[0].year && ` (${order.carItems[0].year})`}
                            </div>
                        </div>
                        <div className="fw-bold">{order.carItems[0].price} ₸</div>
                    </div>
                </Card>
            ) : (
                // Блок для запчастей (список)
                order.partItems?.length > 0 ? (
                    order.partItems.map(item => (
                        <Card
                            key={item.partId}
                            className="mb-2 p-2 shadow-sm"
                            onClick={() => navigate(`/parts/${item.partId}`)}
                            style={{ cursor: 'pointer' }}
                        >
                            <div className="d-flex justify-content-between align-items-center">
                                <div>
                                    <Badge variant="success" className="me-2">Запчасть</Badge>
                                    <b>{item.partName}</b>
                                    {item.partArticle && (
                                        <div className="text-muted small">Артикул: {item.partArticle}</div>
                                    )}
                                </div>
                                <div className="fw-bold">
                                    {item.quantity} × {item.unitPrice} ₸
                                </div>
                            </div>
                        </Card>
                    ))
                ) : (
                    <p className="text-muted">В заказе нет позиций.</p>
                )
            )}
        </Container>
    );
}
