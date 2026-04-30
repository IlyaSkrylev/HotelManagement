import api from './axiosConfig'

export const hotelStructureApi = {
    getFloors: (hotelId, page = 1, pageSize = 20) =>
        api.get(`/hotels/${hotelId}/structure/floors?page=${page}&pageSize=${pageSize}`),

    createFloor: (hotelId, data) =>
        api.post(`/hotels/${hotelId}/structure/floors`, data),

    updateFloor: (id, data) =>
        api.put(`/hotels/structure/floors/${id}`, data),

    deleteFloor: (id) =>
        api.delete(`/hotels/structure/floors/${id}`),

    getRooms: (hotelId, floorId = null, page = 1, pageSize = 20) => {
        let url = `/hotels/${hotelId}/structure/rooms?page=${page}&pageSize=${pageSize}`
        if (floorId) url += `&floorId=${floorId}`
        return api.get(url)
    },

    getRoomStatuses: () =>
        api.get('/hotels/structure/room-statuses'),

    createRoom: (hotelId, data) =>
        api.post(`/hotels/${hotelId}/structure/rooms`, data),

    updateRoom: (id, data) =>
        api.put(`/hotels/structure/rooms/${id}`, data),

    deleteRoom: (id) =>
        api.delete(`/hotels/structure/rooms/${id}`)
}