import api from './axiosConfig'

export const employeeApi = {
    getSchedule: (employeeId, year, month) =>
        api.get(`/employees/${employeeId}/schedule?year=${year}&month=${month}`),

    startShift: (employeeId) =>
        api.post(`/employees/${employeeId}/schedule/start-shift`),

    endShift: (employeeId) =>
        api.post(`/employees/${employeeId}/schedule/end-shift`),

    getOpenShift: (employeeId) =>
        api.get(`/employees/${employeeId}/schedule/open-shift`)
}