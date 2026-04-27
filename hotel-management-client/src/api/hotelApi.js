import api from './axiosConfig'

export const hotelApi = {
    getAll: (page = 1, pageSize = 6) => api.get(`/hotels?Page=${page}&PageSize=${pageSize}`),
    getMyHotels: (page = 1, pageSize = 10) => api.get(`/hotels/myhotels?Page=${page}&PageSize=${pageSize}`),
    getById: (id) => api.get(`/hotels/${id}`),
    getHotelAdminInfo: (hotelId) => api.get(`/hotels/${hotelId}/admin-info`),
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
    updateHotel: (id, data) => {
        const formData = new FormData()
        formData.append('Name', data.name)
        formData.append('Address', data.address || '')
        formData.append('Phone', data.phone || '')
        formData.append('Email', data.email || '')
        formData.append('Description', data.description || '')
        if (data.image) {
            formData.append('Image', data.image)
        }
        return api.put(`/hotels/${id}`, formData)
    },
    delete: (id) => api.delete(`/hotels/${id}`),
}