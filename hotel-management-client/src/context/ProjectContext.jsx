import React, { createContext, useState, useContext, useCallback } from 'react'
import { hotelApi } from '../api/hotelApi'

const ProjectContext = createContext(null)

export const useProject = () => {
    const context = useContext(ProjectContext)
    if (!context) {
        throw new Error('useProject must be used within ProjectProvider')
    }
    return context
}

export const ProjectProvider = ({ children }) => {
    const [hotels, setHotels] = useState([])
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    const loadHotels = useCallback(async () => {
        setLoading(true)
        setError(null)
        try {
            const response = await hotelApi.getAll()
            setHotels(response.data.data?.items || response.data.data || [])
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка загрузки отелей')
        } finally {
            setLoading(false)
        }
    }, [])

    const createHotel = async (hotelData) => {
        setLoading(true)
        try {
            const response = await hotelApi.create(hotelData)
            await loadHotels()
            return response.data
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка создания отеля')
            throw err
        } finally {
            setLoading(false)
        }
    }

    const updateHotel = async (id, hotelData) => {
        setLoading(true)
        try {
            const response = await hotelApi.update(id, hotelData)
            await loadHotels()
            return response.data
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка обновления отеля')
            throw err
        } finally {
            setLoading(false)
        }
    }

    const deleteHotel = async (id) => {
        setLoading(true)
        try {
            await hotelApi.delete(id)
            await loadHotels()
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка удаления отеля')
            throw err
        } finally {
            setLoading(false)
        }
    }

    const value = {
        hotels,
        loading,
        error,
        loadHotels,
        createHotel,
        updateHotel,
        deleteHotel
    }

    return (
        <ProjectContext.Provider value={value}>
            {children}
        </ProjectContext.Provider>
    )
}