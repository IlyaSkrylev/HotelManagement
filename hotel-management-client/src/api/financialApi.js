import api from './axiosConfig'

export const financialApi = {
    getEmployeeOperations: (employeeId, page = 1, pageSize = 20, startDate = '', endDate = '') => {
        const params = new URLSearchParams()
        params.append('page', page)
        params.append('pageSize', pageSize)
        if (startDate) params.append('startDate', startDate)
        if (endDate) params.append('endDate', endDate)
        return api.get(`/financial/employee/${employeeId}?${params.toString()}`)
    },

    getEmployeeStats: (employeeId, startDate = '', endDate = '') => {
        const params = new URLSearchParams()
        if (startDate) params.append('startDate', startDate)
        if (endDate) params.append('endDate', endDate)
        const url = params.toString()
            ? `/financial/employee/${employeeId}/stats?${params.toString()}`
            : `/financial/employee/${employeeId}/stats`
        return api.get(url)
    },

    getDepartmentOperations: (departmentId, page = 1, pageSize = 20) =>
        api.get(`/financial/department/${departmentId}?page=${page}&pageSize=${pageSize}`),

    createOperation: (data) =>
        api.post('/financial', data),

    payDepartmentSalary: (departmentId, createdById, isSalary) =>
        api.post(`/financial/department/${departmentId}/pay`, { createdById, isSalary })
}