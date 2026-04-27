import api from './axiosConfig'

export const profileApi = {
    getProfile: () => api.get('/profile'),

    updateProfile: (data) => {
        const formData = new FormData()
        formData.append('FirstName', data.firstName)
        formData.append('LastName', data.lastName)
        formData.append('Patronymic', data.patronymic || '')
        formData.append('Phone', data.phone || '')

        if (data.birthDate) {
            formData.append('BirthDate', data.birthDate)
        }

        if (data.avatar) {
            formData.append('Avatar', data.avatar)
        }

        return api.put('/profile', formData)
    },

    changePassword: (oldPassword, newPassword) =>
        api.post('/profile/change-password', { oldPassword, newPassword }),

    uploadResume: (file) => {
        const formData = new FormData()
        formData.append('Resume', file)
        return api.post('/profile/upload-resume', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
    },

    downloadResume: () => api.get('/profile/resume', { responseType: 'blob' })
}