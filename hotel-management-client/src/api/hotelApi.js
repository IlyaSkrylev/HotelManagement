import api from './axiosConfig'

export const hotelApi = {
    getAll: (page = 1, pageSize = 6) => api.get(`/hotels?Page=${page}&PageSize=${pageSize}`),
    getMyHotels: (page = 1, pageSize = 10) => api.get(`/hotels/myhotels?Page=${page}&PageSize=${pageSize}`),
    getById: (id) => api.get(`/hotels/${id}`),
    getHotelAdminInfo: (hotelId) => api.get(`/hotels/${hotelId}/admin-info`),
    getUserRoleInHotel: (hotelId) => api.get(`/hotels/${hotelId}/user-role`),
    hireFromResume: (hotelId, userId, data) => api.post(`/hotels/${hotelId}/hire/${userId}`, data),
    updateEmployee: (employeeId, data) => api.put(`/employees/${employeeId}`, data),

    getEmployees: (hotelId, searchTerm = '', departmentName = '', page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        if (searchTerm) params.append('searchTerm', searchTerm)
        if (departmentName && departmentName !== 'all') params.append('departmentName', departmentName)
        params.append('page', page)
        params.append('pageSize', pageSize)
        return api.get(`/hotels/${hotelId}/employees${params.toString() ? `?${params.toString()}` : ''}`)
    },

    getApprovedResumes: (hotelId, searchTerm = '', page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        if (searchTerm) params.append('searchTerm', searchTerm)
        params.append('page', page)
        params.append('pageSize', pageSize)

        const url = `/hotels/${hotelId}/approved-resumes${params.toString() ? `?${params.toString()}` : ''}`
        return api.get(url)
    },

    getHotelDepartments: (hotelId) => api.get(`/hotels/${hotelId}/departments`),

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