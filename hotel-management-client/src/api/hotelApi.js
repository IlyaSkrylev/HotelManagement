import api from './axiosConfig'

export const hotelApi = {
    getAll: (page = 1, pageSize = 6) => api.get(`/hotels?Page=${page}&PageSize=${pageSize}`),
    getById: (id) => api.get(`/hotels/${id}`),
    create: (data) => {
        const formData = new FormData()
        formData.append('Name', data.name)
        formData.append('Address', data.address || '')
        formData.append('Phone', data.phone || '')
        formData.append('Email', data.email || '')
        formData.append('Description', data.description || '')
        if (data.image) {
            formData.append('Image', data.image)
        }
        return api.post('/hotels', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
    },
    update: (id, data) => {
        const formData = new FormData()
        formData.append('Name', data.name)
        formData.append('Address', data.address || '')
        formData.append('Phone', data.phone || '')
        formData.append('Email', data.email || '')
        formData.append('Description', data.description || '')
        if (data.image) {
            formData.append('Image', data.image)
        }
        return api.put(`/hotels/${id}`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
    },
    delete: (id) => api.delete(`/hotels/${id}`),
}