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
        const loadUser = async () => {
            const accessToken = localStorage.getItem('accessToken')
            const refreshToken = localStorage.getItem('refreshToken')

            if (!accessToken || !refreshToken) {
                setLoading(false)
                return
            }

            try {
                const response = await authApi.getProfile()
                console.log('Profile response:', response.data)

                const userData = response.data.data
                setUser(userData)
                setIsAuthenticated(true)
            } catch (error) {
                console.error('Error loading profile:', error.response?.data)

                try {
                    const refreshToken = localStorage.getItem('refreshToken')
                    const refreshResponse = await authApi.refreshToken(refreshToken)

                    const { accessToken: newAccessToken, refreshToken: newRefreshToken } = refreshResponse.data.data
                    localStorage.setItem('accessToken', newAccessToken)
                    localStorage.setItem('refreshToken', newRefreshToken)

                    const profileResponse = await authApi.getProfile()
                    setUser(profileResponse.data.data)
                    setIsAuthenticated(true)
                } catch (refreshError) {
                    console.error('Refresh failed:', refreshError)
                    localStorage.removeItem('accessToken')
                    localStorage.removeItem('refreshToken')
                    setIsAuthenticated(false)
                }
            } finally {
                setLoading(false)
            }
        }

        loadUser()
    }, [])

    const login = async (email, password) => {
        const response = await authApi.login(email, password)
        const { accessToken, refreshToken } = response.data.data

        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', refreshToken)

        try {
            const profileResponse = await authApi.getProfile()
            const userData = profileResponse.data.data
            setUser(userData)
            setIsAuthenticated(true)
            return { ...response.data, data: userData }
        } catch (error) {
            console.error('Failed to load profile after login:', error)
            const { accessToken: token, refreshToken: rToken, ...userData } = response.data.data
            setUser(userData)
            setIsAuthenticated(true)
            return response.data
        }
    }

    const register = async (userData) => {
        const response = await authApi.register(userData)
        const { accessToken, refreshToken } = response.data.data

        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', refreshToken)

        try {
            const profileResponse = await authApi.getProfile()
            const registeredUser = profileResponse.data.data
            setUser(registeredUser)
            setIsAuthenticated(true)
            return { ...response.data, data: registeredUser }
        } catch (error) {
            const { accessToken: token, refreshToken: rToken, ...registeredUser } = response.data.data
            setUser(registeredUser)
            setIsAuthenticated(true)
            return response.data
        }
    }

    const logout = () => {
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        setUser(null)
        setIsAuthenticated(false)
    }

    const updateProfile = (data) => {
        setUser(prevUser => ({ ...prevUser, ...data }))
        return data
    }

    const refreshToken = async (refreshTokenStr) => {
        const response = await authApi.refreshToken(refreshTokenStr)
        const { accessToken, refreshToken: newRefreshToken } = response.data.data

        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', newRefreshToken)

        return { accessToken, refreshToken: newRefreshToken }
    }

    const value = {
        user,
        loading,
        isAuthenticated,
        login,
        register,
        logout,
        updateProfile,
        refreshToken
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}