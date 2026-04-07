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
        const accessToken = localStorage.getItem('accessToken')
        const refreshToken = localStorage.getItem('refreshToken')

        if (accessToken && refreshToken) {
            // Проверяем валидность access токена
            authApi.getProfile()
                .then(response => {
                    setUser(response.data.data)
                    setIsAuthenticated(true)
                })
                .catch(async () => {
                    // Если access токен истёк, пробуем обновить
                    try {
                        const newTokens = await authApi.refreshToken(refreshToken)
                        localStorage.setItem('accessToken', newTokens.data.data.accessToken)
                        localStorage.setItem('refreshToken', newTokens.data.data.refreshToken)

                        const profile = await authApi.getProfile()
                        setUser(profile.data.data)
                        setIsAuthenticated(true)
                    } catch (error) {
                        localStorage.removeItem('accessToken')
                        localStorage.removeItem('refreshToken')
                        setIsAuthenticated(false)
                    }
                })
                .finally(() => setLoading(false))
        } else {
            setLoading(false)
        }
    }, [])

    const login = async (email, password) => {
        const response = await authApi.login(email, password)
        const { accessToken, refreshToken, ...userData } = response.data.data
        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', refreshToken)
        setUser(userData)
        setIsAuthenticated(true)
        return response.data
    }

    const register = async (userData) => {
        const response = await authApi.register(userData)
        return response.data 
    }

    const loginAfterRegister = (userData) => {
        setUser(userData)
        setIsAuthenticated(true)
    }

    const logout = () => {
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        setUser(null)
        setIsAuthenticated(false)
    }

    const updateProfile = async (data) => {
        const response = await authApi.updateProfile(data)
        setUser(response.data.data)
        return response.data
    }

    const value = {
        user,
        loading,
        isAuthenticated,
        login,
        register,
        loginAfterRegister,
        logout,
        updateProfile
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}