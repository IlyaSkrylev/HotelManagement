import api from './axiosConfig'

export const hotelApi = {
    getAll: () => api.get('/hotels'),
    getById: (id) => api.get(`/hotels/${id}`),
    create: (data) => api.post('/hotels', data),
    update: (id, data) => api.put(`/hotels/${id}`, data),
    delete: (id) => api.delete(`/hotels/${id}`),
}