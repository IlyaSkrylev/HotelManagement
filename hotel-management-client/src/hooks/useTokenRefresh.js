import { useEffect, useRef } from 'react'
import { useAuth } from '../context/AuthContext'

const useTokenRefresh = () => {
    const { refreshToken, logout } = useAuth()
    const intervalRef = useRef(null)

    const decodeToken = (token) => {
        try {
            if (!token || typeof token !== 'string') return null
            const parts = token.split('.')
            if (parts.length !== 3) return null

            const payload = parts[1]
                .replace(/-/g, '+')
                .replace(/_/g, '/')
                .padEnd(Math.ceil(parts[1].length / 4) * 4, '=')

            const decoded = JSON.parse(atob(payload))
            return decoded
        } catch (error) {
            console.error('Ошибка декодирования токена:', error)
            return null
        }
    }

    const getTimeUntilExpiry = (token) => {
        const decoded = decodeToken(token)
        if (!decoded || !decoded.exp) return null

        const expiryTime = decoded.exp * 1000 
        const currentTime = Date.now()
        const timeLeft = expiryTime - currentTime

        return timeLeft > 0 ? timeLeft : 0
    }

    const formatTimeLeft = (milliseconds) => {
        if (milliseconds <= 0) return '0 сек'

        const seconds = Math.floor(milliseconds / 1000)
        const minutes = Math.floor(seconds / 60)
        const remainingSeconds = seconds % 60

        if (minutes > 0) {
            return `${minutes} мин ${remainingSeconds} сек`
        }
        return `${seconds} сек`
    }

    const logTokenStatus = () => {
        const accessToken = localStorage.getItem('accessToken')

        if (!accessToken) {
            console.log('Нет access токена')
            return
        }

        const timeLeft = getTimeUntilExpiry(accessToken)

        if (timeLeft === null) {
            console.log('Access токен невалидный, выполняем выход')
            logout()
            return
        }

        if (timeLeft <= 0) {
            console.log('Access токен ИСТЁК!')
        } else if (timeLeft < 60000) { 
            console.log(`Осталось до обновления: ${formatTimeLeft(timeLeft)} (скоро истечёт!)`)
        } else if (timeLeft < 300000) { 
            console.log(`Осталось до обновления: ${formatTimeLeft(timeLeft)}`)
        } else {
            console.log(`Осталось до обновления: ${formatTimeLeft(timeLeft)}`)
        }
    }

    const performRefresh = async () => {
        const refreshTokenStr = localStorage.getItem('refreshToken')
        if (!refreshTokenStr) {
            console.log('Нет refresh токена, пропускаем обновление')
            return false
        }

        try {
            console.log('Попытка обновления токенов...')
            const newTokens = await refreshToken(refreshTokenStr)
            console.log('Токены успешно обновлены!')
            console.log(`Новый access токен истечёт через: ${formatTimeLeft(getTimeUntilExpiry(newTokens.accessToken))}`)
            return true
        } catch (error) {
            console.error('Ошибка при обновлении токенов:', error)
            return false
        }
    }

    const checkAndRefresh = async () => {
        const accessToken = localStorage.getItem('accessToken')

        if (!accessToken) {
            console.log('Нет access токена')
            return
        }

        const timeLeft = getTimeUntilExpiry(accessToken)

        if (timeLeft === null) {
            console.log('Access токен невалидный, выполняем выход')
            logout()
            return
        }

        if (timeLeft <= 30000 && timeLeft > 0) {
            console.log(`Осталось ${formatTimeLeft(timeLeft)} — запускаем обновление...`)
            await performRefresh()
        } else if (timeLeft <= 0) {
            console.log('Токен истёк! Запускаем обновление...')
            await performRefresh()
        }
    }

    useEffect(() => {
        logTokenStatus()

        intervalRef.current = setInterval(() => {
            logTokenStatus()
            checkAndRefresh()
        }, 5000) 

        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current)
            }
        }
    }, [refreshToken, logout])
}

export default useTokenRefresh