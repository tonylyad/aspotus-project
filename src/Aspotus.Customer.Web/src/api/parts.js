import { api } from "./axios"

export const getParts = async () => {

    const response =
        await api.get("/catalog/api/Parts")

    return response.data

}

export const getPartById = async (id) => {

    const response =
        await api.get(`/catalog/api/Parts/${id}`)

    return response.data

}

export const getPartsPage = async ({ page = 1, pageSize = 9, query = "" } = {}) => {
    const response = await api.get("/catalog/api/Parts/paged", {
        params: { page, pageSize, query: query || undefined }
    })
    return response.data
}
