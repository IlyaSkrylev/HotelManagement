import React from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useTheme } from '../context/ThemeContext'
import { getIconUrl } from '../index'
import '../styles/Navigation.css'

function Navigation() {
    const { user, logout, isAuthenticated } = useAuth()
    const { theme, toggleTheme, isDark } = useTheme()
    const navigate = useNavigate()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    const themeIconUrl = getIconUrl(isDark ? 'sun' : 'moon')
    const profileIconUrl = getIconUrl('profile')
    const hotelIconUrl = getIconUrl('hotel')

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <div className="navbar-logo">
                    <Link to="/" className="logo-link">
                        <img src={hotelIconUrl} alt="hotel" className="hotel-icon-img"/>
                        <span className="logo-text">HotelManager</span>
                    </Link>
                </div>

                <div className="navbar-links">
                    <Link to="/" className="nav-link">Главная</Link>
                    <Link to="/hotels" className="nav-link">Отели</Link>
                </div>

                <div className="navbar-actions">
                    {isAuthenticated ? (
                        <>
                            <div className="user-greeting">
                                <img src={profileIconUrl} alt="profile" className="user-avatar-img" />
                                <span className="user-name">{user?.firstName || user?.email}</span>
                            </div>
                            <button onClick={handleLogout} className="nav-btn logout-btn">
                                Выйти
                            </button>
                        </>
                    ) : (
                        <>
                            <Link to="/login" className="nav-btn login-btn">Вход</Link>
                            <Link to="/register" className="nav-btn register-btn">Регистрация</Link>
                        </>
                    )}

                    <button onClick={toggleTheme} className="theme-toggle">
                        <img src={themeIconUrl} alt="theme" className="theme-icon" />
                    </button>
                </div>
            </div>
        </nav>
    )
}

export default Navigation