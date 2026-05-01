import api from './axiosConfig'

export const resumeApi = {
    getHotelResumes: (hotelId, statusId = null, searchTerm = '', page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        if (statusId) params.append('statusId', statusId)
        if (searchTerm) params.append('searchTerm', searchTerm)
        params.append('page', page)
        params.append('pageSize', pageSize)

        const url = `/hotels/${hotelId}/resumes${params.toString() ? `?${params.toString()}` : ''}`
        return api.get(url)
    },

    getStatuses: () => api.get('/resume-statuses'),

    getMyResumes: () => api.get('/profile/resumes'),

    submitResume: (data) => {
        const formData = new FormData()
        formData.append('HotelId', data.hotelId)
        formData.append('DesiredPosition', data.desiredPosition)
        formData.append('Experience', data.experience || '')
        formData.append('Education', data.education || '')
        formData.append('UseProfileResume', data.useProfileResume)

        if (data.resumeFile) {
            formData.append('ResumeFile', data.resumeFile)
        }

        return api.post('/resume/submit', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
    },

    updateStatus: (resumeId, statusId) =>
        api.put(`/resumes/${resumeId}/status`, {
            resumeId: resumeId,
            statusId: statusId
        }),

    deleteResume: (resumeId) => api.delete(`/resumes/${resumeId}`)
}