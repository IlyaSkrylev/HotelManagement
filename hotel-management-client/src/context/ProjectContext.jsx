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
    const [pagination, setPagination] = useState({
        currentPage: 1,
        pageSize: 6,
        totalCount: 0,
        totalPages: 0
    })

    const loadHotels = useCallback(async (page = 1, pageSize = 6) => {
        setLoading(true)
        setError(null)
        try {
            const response = await hotelApi.getAll(page, pageSize)
            setHotels(response.data?.items || [])
            setPagination({
                currentPage: response.data.page,
                pageSize: response.data.pageSize,
                totalCount: response.data.totalCount,
                totalPages: response.data.totalPages
            })
        } catch (err) {
            console.error('Load hotels error:', err)
            setError(err.response?.data?.message || 'Ошибка загрузки отелей')
        } finally {
            setLoading(false)
        }
    }, [])

    const createHotel = async (hotelData) => {
        setLoading(true)
        setError(null)
        try {
            const response = await hotelApi.create(hotelData)
            await loadHotels(1, pagination.pageSize)  
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
        setError(null)
        try {
            const response = await hotelApi.update(id, hotelData)
            await loadHotels(pagination.currentPage, pagination.pageSize)
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
        setError(null)
        try {
            await hotelApi.delete(id)
            if (hotels.length === 1 && pagination.currentPage > 1) {
                await loadHotels(pagination.currentPage - 1, pagination.pageSize)
            } else {
                await loadHotels(pagination.currentPage, pagination.pageSize)
            }
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
        pagination,
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