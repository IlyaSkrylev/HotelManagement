import api from './axiosConfig'

export const taskApi = {
    getTasks: (hotelId, departmentId = null, includeInactive = false, priorityId = null, taskStatusId = null, searchTerm = '', page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        if (departmentId) params.append('departmentId', departmentId)
        params.append('includeInactive', includeInactive ? 'true' : 'false')
        if (priorityId && priorityId !== 'all') params.append('priorityId', priorityId)
        if (taskStatusId && taskStatusId !== 'all') params.append('taskStatusId', taskStatusId)
        if (searchTerm) params.append('searchTerm', searchTerm)
        params.append('page', page)
        params.append('pageSize', pageSize)
        return api.get(`/hotels/${hotelId}/tasks${params.toString() ? `?${params.toString()}` : ''}`)
    },

    getMyTasks: (hotelId, employeeId, includeInactive = false, priorityId = null, page = 1, pageSize = 20) => {
        const params = new URLSearchParams()
        params.append('includeInactive', includeInactive ? 'true' : 'false')
        if (priorityId && priorityId !== 'all') params.append('priorityId', priorityId)
        params.append('page', page)
        params.append('pageSize', pageSize)
        return api.get(`/hotels/${hotelId}/employees/${employeeId}/tasks${params.toString() ? `?${params.toString()}` : ''}`)
    },

    getTaskTypes: (departmentId = null) => {
        const params = new URLSearchParams()
        if (departmentId) params.append('departmentId', departmentId)
        return api.get(`/hotels/tasks/types${params.toString() ? `?${params.toString()}` : ''}`)
    },

    getTaskPriorities: () => api.get('/hotels/tasks/priorities'),

    getTaskStatuses: () => api.get('/hotels/tasks/statuses'),

    getEmployeesForTask: (hotelId, departmentId = null, searchTerm = '') => {
        const params = new URLSearchParams()
        if (departmentId) params.append('departmentId', departmentId)
        if (searchTerm) params.append('searchTerm', searchTerm)
        return api.get(`/hotels/${hotelId}/tasks/employees${params.toString() ? `?${params.toString()}` : ''}`)
    },

    createTask: (hotelId, data) => api.post(`/hotels/${hotelId}/tasks`, data),

    updateTask: (id, data) => api.put(`/hotels/tasks/${id}`, data),

    deleteTask: (id) => api.delete(`/hotels/tasks/${id}`)
}