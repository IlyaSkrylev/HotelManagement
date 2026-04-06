import React, { createContext, useState, useContext, useEffect } from 'react'
import { authApi } from '../api/authApi'

const AuthContext = createContext(null)

export const useAuth = () => {
    const context = useContext(AuthContext)
    if (!context) {
        throw new Error('useAuth must be used within AuthProvider')
    }
    return context
}

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null)
    const [loading, setLoading] = useState(true)
    const [isAuthenticated, setIsAuthenticated] = useState(false)

    useEffect(() => {
        const token = localStorage.getItem('token')
        if (token) {
            // Проверка токена и получение данных пользователя
            authApi.getProfile()
                .then(response => {
                    setUser(response.data)
                    setIsAuthenticated(true)
                })
                .catch(() => {
                    localStorage.removeItem('token')
                    setIsAuthenticated(false)
                })
                .finally(() => setLoading(false))
        } else {
            setLoading(false)
        }
    }, [])

    const login = async (email, password) => {
        const response = await authApi.login(email, password)
        const { token, user: userData } = response.data
        localStorage.setItem('token', token)
        setUser(userData)
        setIsAuthenticated(true)
        return response.data
    }

    const register = async (userData) => {
        const response = await authApi.register(userData)
        return response.data
    }

    const logout = () => {
        localStorage.removeItem('token')
        setUser(null)
        setIsAuthenticated(false)
    }

    const updateProfile = async (data) => {
        const response = await authApi.updateProfile(data)
        setUser(response.data)
        return response.data
    }

    const value = {
        user,
        loading,
        isAuthenticated,
        login,
        register,
        logout,
        updateProfile
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}