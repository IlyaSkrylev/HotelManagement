import api from './axiosConfig'

export const authApi = {
    login: (email, password) => api.post('/auth/login', { email, password }),
    register: (data) => api.post('/auth/register', data),
    refreshToken: (refreshToken) => api.post('/auth/refresh', { refreshToken }),
    getProfile: () => api.get('/profile'),  
    updateProfile: (data) => api.put('/profile', data),
    changePassword: (oldPassword, newPassword) => api.post('/profile/change-password', { oldPassword, newPassword }),
}