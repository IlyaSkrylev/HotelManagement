import api from './axiosConfig'

export const resumeApi = {
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
    }
}