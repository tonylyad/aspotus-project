function OrderStatusIndicator({ status }) {
    // 1. Нормализуем статус: если пришло число (10, 20...) — оставляем как есть.
    // Если пришла строка ("Created", "Processing") — конвертируем в число.
    const statusMapByName = {
        "Created": 10,
        "Processing": 20,
        "Completed": 30,
        "Cancelled": 40
    };

    const currentStatusValue = typeof status === "string"
        ? statusMapByName[status]
        : status;

    // 2. Определяем шаг (1, 2, 3) или null для отмененного
    const getStepNumber = (val) => {
        if (val === 10) return 1;
        if (val === 20) return 2;
        if (val === 30) return 3;
        if (val === 40) return null; // Cancelled
        return null;
    };

    const stepNumber = getStepNumber(currentStatusValue);
    const isCancelled = currentStatusValue === 40;

    // 3. Если отменен — показываем красный бейдж
    if (isCancelled) {
        return (
            <div className="text-center mb-4">
                <span className="badge bg-danger px-3 py-2 fw-bold text-uppercase" style={{ letterSpacing: "0.5px" }}>
                    Отменён
                </span>
            </div>
        );
    }

    const steps = [
        { label: "Создан", value: 1 },
        { label: "В обработке", value: 2 },
        { label: "Завершён", value: 3 },
    ];

    return (
        <div className="d-flex justify-content-center align-items-center gap-4 mb-4">
            {steps.map((step) => {
                const isActive = step.value === stepNumber;
                const isPast = step.value < (stepNumber || 0);

                // Цвета адаптированы под темную тему (как на твоем скриншоте)
                const bgColor = isActive
                    ? "#0d6efd"          // Синий (активный)
                    : isPast
                        ? "#198754"       // Зеленый (пройденный)
                        : "#343a40";      // Темно-серый (будущий)

                const textColor = isActive || isPast ? "#ffffff" : "#adb5bd";
                const labelColor = isActive || isPast ? "#adb5bd" : "#6c757d";

                return (
                    <div key={step.value} className="text-center">
                        <div
                            className="badge border-0 rounded-circle p-2 shadow-sm transition-all"
                            style={{
                                width: "28px",
                                height: "28px",
                                backgroundColor: bgColor,
                                color: textColor,
                                fontSize: "0.9rem",
                                lineHeight: "28px",
                                fontWeight: "bold",
                                boxShadow: isActive ? "0 0 0 3px rgba(13,110,253,0.3)" : "none",
                            }}
                        >
                            {step.value}
                        </div>
                        <div
                            style={{
                                fontSize: "0.8rem",
                                color: labelColor,
                                marginTop: "6px",
                                whiteSpace: "nowrap",
                            }}
                        >
                            {step.label}
                        </div>
                    </div>
                );
            })}
        </div>
    );
}

export default OrderStatusIndicator;