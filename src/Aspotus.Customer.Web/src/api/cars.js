import { api } from "./axios"

export const getCars = async () => {

    const response =
        await api.get("/catalog/api/Cars")

    return response.data

}

export const getCarById = async (id) => {

    const response =
        await api.get(`/catalog/api/Cars/${id}`)

    return response.data

}

export const getCarsPage = async ({ page = 1, pageSize = 9, query = "" } = {}) => {
    const response = await api.get("/catalog/api/Cars/paged", {
        params: { page, pageSize, query: query || undefined }
    })
    return response.data
}
