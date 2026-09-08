import { useEffect, useState } from "react";
import { Container, Card, Spinner, Button, Badge } from "react-bootstrap";
import { useParams, useNavigate } from "react-router-dom";
import { getOrderById } from "../api/auth";

import BackButton from "../components/common/BackButton"
import OrderStatusIndicator from "../components/common/OrderStatusIndicator"
import { formatPrice } from "../utils/cart.js"

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
    const isPartOrder = order.partItems?.length > 0;

    return (
        <Container className="mt-5 px-3">

            <div className="d-flex flex-wrap justify-content-between align-items-start mb-4 gap-3">
                <BackButton />
                <div className="text-end text-md-start">
                    <div className="small fw-bold text-uppercase" style={{ color: "#adb5bd" }}>
                        Заказ №
                    </div>
                    <div className="fs-4 fw-bold text-truncate" style={{ maxWidth: "100%" }}>
                        {order.id}
                    </div>
                </div>
            </div>

            <OrderStatusIndicator status={order.status} />

            {/* Информация о заказе — с группами и разделителями */}
            <Card className="mb-4 shadow-sm border-0" style={{ borderRadius: "12px", overflow: "hidden" }}>
                <Card.Body className="p-0">

                    {/* Заголовок */}
                    <div className="px-4 pt-4 pb-3">
                        <h5 className="fw-bold mb-0">Информация о заказе</h5>
                    </div>

                    {/* Группа: Заказчик */}
                    <div className="px-4 pb-4">
                        <div className="row g-3">
                            <div className="col-md-6">
                                <div className="small mb-1" style={{ color: "#adb5bd", fontSize: "0.75rem", textTransform: "uppercase", letterSpacing: "0.5px" }}>
                                    Имя
                                </div>
                                <div className="fw-semibold">{order.customerName || "-"}</div>
                            </div>
                            <div className="col-md-6">
                                <div className="small mb-1" style={{ color: "#adb5bd", fontSize: "0.75rem", textTransform: "uppercase", letterSpacing: "0.5px" }}>
                                    Email
                                </div>
                                <div className="fw-normal text-truncate">{order.customerEmail || "-"}</div>
                            </div>
                            <div className="col-md-6">
                                <div className="small mb-1" style={{ color: "#adb5bd", fontSize: "0.75rem", textTransform: "uppercase", letterSpacing: "0.5px" }}>
                                    Телефон
                                </div>
                                <div className="fw-normal">{order.customerPhone || "-"}</div>
                            </div>
                        </div>
                    </div>

                    {/* Разделитель */}
                    <div style={{ height: "1px", background: "rgba(255,255,255,0.1)" }} />

                    {/* Группа: Доставка */}
                    <div className="px-4 py-4">
                        <div className="row g-3">
                            <div className="col-md-6">
                                <div className="small mb-1" style={{ color: "#adb5bd", fontSize: "0.75rem", textTransform: "uppercase", letterSpacing: "0.5px" }}>
                                    Адрес доставки
                                </div>
                                <div className="fw-normal" style={{ wordBreak: "break-word" }}>
                                    {order.deliveryAddress || "-"}
                                </div>
                            </div>
                            <div className="col-md-6">
                                <div className="small mb-1" style={{ color: "#adb5bd", fontSize: "0.75rem", textTransform: "uppercase", letterSpacing: "0.5px" }}>
                                    Дата заказа
                                </div>
                                <div className="fw-normal">
                                    {order.createdAtUtc
                                        ? new Date(order.createdAtUtc).toLocaleString("ru-RU", {
                                            day: "2-digit",
                                            month: "2-digit",
                                            year: "numeric",
                                            hour: "2-digit",
                                            minute: "2-digit",
                                        })
                                        : "-"}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Разделитель */}
                    <div style={{ height: "1px", background: "rgba(255,255,255,0.1)" }} />

                    {/* Группа: Сумма — выделенная */}
                    <div className="px-4 py-4 d-flex justify-content-between align-items-center">
                        <div className="small fw-bold" style={{ color: "#adb5bd", textTransform: "uppercase", letterSpacing: "0.5px", fontSize: "0.8rem" }}>
                            Итоговая сумма
                        </div>
                        <div className="fs-4 fw-bold text-primary">
                            {formatPrice(order.totalAmount)}
                        </div>
                    </div>

                </Card.Body>
            </Card>

            <h5 className="fw-bold mb-3">Состав заказа</h5>

            {isCarOrder ? (
                order.carItems.length > 0 && (
                    order.carItems.map((item) => (
                        <Card
                        key={item.carId}
                        className="mb-2 shadow-sm border-0"
                        style={{ cursor: "pointer", borderRadius: "12px" }}
                        onClick={() => navigate(`/cars/${item.carId}`)}
                    >
                        <Card.Body className="p-3">
                            <div className="d-flex align-items-center gap-3 mb-2">
                                <Badge variant="primary" className="me-2">Авто</Badge>
                                <div>
                                    <h6 className="mb-0 fw-bold lh-sm">
                                        {item.brandName || ""}{" "}
                                        {item.modelName || ""}
                                    </h6>
                                    <div style={{ color: "#adb5bd", fontSize: "0.85rem" }}>
                                        {item.generationName}
                                        {item.year && ` (${item.year})`}
                                    </div>
                                </div>
                            </div>
                            <div className="d-flex justify-content-between align-items-end">
                                <div style={{ color: "#adb5bd", fontSize: "0.85rem" }}>Цена за авто</div>
                                <div className="fw-bold fs-4 text-primary">
                                    {formatPrice(item.price ?? order.totalAmount)}
                                </div>
                            </div>
                        </Card.Body>
                    </Card>
                    ))
                )
            ) : isPartOrder ? (
                order.partItems.map((item) => (
                    <Card
                        key={item.partId}
                        className="mb-2 shadow-sm border-0"
                        onClick={() => navigate(`/parts/${item.partId}`)}
                        style={{ cursor: "pointer", borderRadius: "12px" }}
                    >
                        <Card.Body className="p-3">
                            <div className="d-flex align-items-center gap-3 mb-2">
                                <Badge variant="success" className="me-2">Запчасть</Badge>
                                <div style={{ flex: 1 }}>
                                    <h6 className="mb-1 fw-bold lh-sm">{item.partName || "-"}</h6>
                                    {item.partArticle && (
                                        <div style={{ color: "#adb5bd", fontSize: "0.85rem" }}>
                                            Артикул: {item.partArticle}
                                        </div>
                                    )}
                                </div>
                            </div>
                            <div className="d-flex justify-content-between align-items-end">
                                <div style={{ color: "#adb5bd", fontSize: "0.85rem" }}>
                                    {item.quantity} × {formatPrice(item.unitPrice)}
                                </div>
                                <div className="fw-bold text-primary fs-5">
                                    {formatPrice(item.quantity * (item.unitPrice ?? 0))}
                                </div>
                            </div>
                        </Card.Body>
                    </Card>
                ))
            ) : (
                <div className="text-center py-5 border rounded" style={{ color: "#adb5bd", borderRadius: "12px" }}>
                    В заказе нет позиций.
                </div>
            )}
        </Container>
    );
}
