import api from './axiosConfig'

export const departmentApi = {
    getDepartments: (hotelId, searchTerm = '', page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        if (searchTerm) params.append('searchTerm', searchTerm)
        params.append('page', page)
        params.append('pageSize', pageSize)

        const url = `/hotels/${hotelId}/departments/paginated${params.toString() ? `?${params.toString()}` : ''}`
        return api.get(url)
    },

    getEmployeesForSelect: (hotelId, searchTerm = '', roleCode = '') => {
        const params = new URLSearchParams()
        if (searchTerm) params.append('searchTerm', searchTerm)
        if (roleCode) params.append('roleCode', roleCode)
        const url = `/hotels/${hotelId}/departments/employees${params.toString() ? `?${params.toString()}` : ''}`
        return api.get(url)
    },

    createDepartment: (hotelId, data) => api.post(`/hotels/${hotelId}/departments`, data),

    updateDepartment: (id, data) => api.put(`/departments/${id}`, data),

    deleteDepartment: (id) => api.delete(`/departments/${id}`)
}