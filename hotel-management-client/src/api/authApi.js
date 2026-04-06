import api from './axiosConfig'

export const authApi = {
    login: (email, password) => api.post('/auth/login', { email, password }),
    register: (data) => api.post('/auth/register', data),
    getProfile: () => api.get('/auth/profile'),
    updateProfile: (data) => api.put('/auth/profile', data),
    changePassword: (oldPassword, newPassword) => api.post('/auth/change-password', { oldPassword, newPassword }),
}