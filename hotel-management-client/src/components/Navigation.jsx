import React from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function Navigation() {
    const { user, logout, isAuthenticated } = useAuth()
    const navigate = useNavigate()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    return (
        <nav style={{ padding: '10px', borderBottom: '1px solid #ccc' }}>
            <Link to="/">Главная</Link>
            {' | '}
            <Link to="/hotels">Отели</Link>
            {' | '}
            <Link to="/employees">Сотрудники</Link>

            {isAuthenticated ? (
                <>
                    {' | '}
                    <Link to="/dashboard">Панель</Link>
                    {' | '}
                    <Link to="/profile">Профиль ({user?.email})</Link>
                    {' | '}
                    <button onClick={handleLogout}>Выйти</button>
                </>
            ) : (
                <>
                    {' | '}
                    <Link to="/login">Вход</Link>
                    {' | '}
                    <Link to="/register">Регистрация</Link>
                </>
            )}
        </nav>
    )
}

export default Navigation