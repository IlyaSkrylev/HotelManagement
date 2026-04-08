import React from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useTheme } from '../context/ThemeContext'

function Navigation() {
    const { user, logout, isAuthenticated } = useAuth()
    const { theme, toggleTheme, isDark } = useTheme()
    const navigate = useNavigate()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    return (
        <nav style={{
            padding: '10px',
            borderBottom: '1px solid var(--border-color)',
            backgroundColor: 'var(--nav-bg)'
        }}>
            <Link to="/" style={{ color: 'var(--text-primary)' }}>Главная</Link>
            {' | '}
            <Link to="/hotels" style={{ color: 'var(--text-primary)' }}>Отели</Link>
            {' | '}
            <Link to="/employees" style={{ color: 'var(--text-primary)' }}>Сотрудники</Link>

            {isAuthenticated ? (
                <>
                    {' | '}
                    <Link to="/dashboard" style={{ color: 'var(--text-primary)' }}>Панель</Link>
                    {' | '}
                    <Link to="/profile" style={{ color: 'var(--text-primary)' }}>Профиль ({user?.email})</Link>
                    {' | '}
                    <button onClick={handleLogout} style={{
                        background: 'none',
                        border: 'none',
                        color: 'var(--button-danger)',
                        cursor: 'pointer'
                    }}>Выйти</button>
                </>
            ) : (
                <>
                    {' | '}
                    <Link to="/login" style={{ color: 'var(--text-primary)' }}>Вход</Link>
                    {' | '}
                    <Link to="/register" style={{ color: 'text-primary)' }}>Регистрация</Link>
                </>
            )}
            {' | '}
            <button onClick={toggleTheme} style={{
                background: 'none',
                border: 'none',
                cursor: 'pointer',
                fontSize: '1.2rem'
            }}>
                {isDark ? '☀️' : '🌙'}
            </button>
        </nav>
    )
}

export default Navigation