import { api } from "./axios";

const normalizeUser = (user) => {
    if (!user) return null;

    if (Array.isArray(user)) {
        const get = (type) => user.find(x => x.type.includes(type))?.value;

        return {
            id: get("nameidentifier") || get("sub"),
            name: get("name") || get("sub"),
            email: get("email")
        };
    }

    return user;

};

export const createPartOrder = async (cart, delivery, user) => {
    const normalizedUser = normalizeUser(user);
    if (!normalizedUser) {
        throw new Error("User is not defined");
    }

    const orderRequest = {
        customerName: delivery.customerName || normalizedUser.name,
        customerEmail: delivery.customerEmail || normalizedUser.email,
        customerPhone: delivery.customerPhone,
        deliveryAddress: delivery.deliveryAddress,
        items: cart.map(item => ({
            partId: item.id,
            quantity: item.quantity
        }))
    };

    const response = await api.post("/orders/api/orders/parts", orderRequest);
    return response.data;
};

export const createCarOrder = async (cart, delivery, user) => {
    const normalizedUser = normalizeUser(user);
    if (!normalizedUser) {
        throw new Error("User is not defined");
    }

    const orderRequest = {
        customerName: delivery.customerName || normalizedUser.name,
        customerEmail: delivery.customerEmail || normalizedUser.email,
        customerPhone: delivery.customerPhone,
        deliveryAddress: delivery.deliveryAddress,
        car: {
            carId: cart[0].id
        }
    };

    const response = await api.post("/orders/api/orders/cars", orderRequest);
    return response.data;
};

export const getMyOrders = (userId) => {
    if (!userId) {
        throw new Error("User ID not found");
    }

    return api.get(`/orders/api/orders/by-user/${userId}`);
}


export const getOrderById = (id) =>
    api.get(`/orders/api/orders/${id}`);

export const login = (data) => api.post("/api/Auth/login", data);
export const register = (data) => api.post("/api/Auth/register", data);
export const getProfile = () => api.get("/api/Auth/me");

export const createCustomerRequest = (data) => api.post("/orders/api/requests", data);
