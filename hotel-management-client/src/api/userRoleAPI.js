import api from './axiosConfig'

export const userRoleApi = {
    getAll: () => api.get('/user-roles'),
    getById: (id) => api.get(`/user-roles/${id}`),
    getByCode: (code) => api.get(`/user-roles/code/${code}`),
}