import React from 'react'
import { useAuth } from '../context/AuthContext'
import { getImageUrl, getIconUrl } from '../index'
import '../styles/Home.css'

function Home() {
    const { isAuthenticated, user } = useAuth()
    const backgroundImageUrl = getImageUrl('/uploads/appsphotos/0036c769898cfdd25f5e9d02c2bab17c.jpg')

    const features = [
        { name: 'Управление отделами', icon: 'hotel', description: 'Создание и управление отделами, назначение менеджеров' },
        { name: 'Графики работы', icon: 'timeManagement', description: 'Планирование смен, учёт рабочего времени, контроль выходов' },
        { name: 'Управление задачами', icon: 'taskPlanning', description: 'Постановка задач сотрудникам и контроль выполнения' },
        { name: 'Финансовый учёт', icon: 'financial', description: 'Расчёт зарплаты, начисление премий и штрафов' },
        { name: 'Фотогалерея отелей', icon: 'photoGallery', description: 'Загрузка и отображение фотографий гостиниц' },
        { name: 'Отчётность', icon: 'report', description: 'Аналитика по сотрудникам, задачам и финансам' }
    ]

    return (
        <div className="home-container">
            <div className="home-hero">
                <div className="home-hero-content">
                    <h1 className="home-title">
                        Hotel<span className="title-accent">Manager</span>
                    </h1>
                    <p className="home-subtitle">
                        Современная система управления персоналом<br />
                        для гостиничных комплексов и отелей
                    </p>
                    {!isAuthenticated && (
                        <div className="home-buttons">
                            <a href="/login" className="home-btn primary">Вход</a>
                            <a href="/register" className="home-btn secondary">Регистрация</a>
                        </div>
                    )}
                    {isAuthenticated && (
                        <div className="home-welcome">
                            <p className="welcome-text">
                                Добро пожаловать, <strong>{user?.firstName} {user?.lastName}</strong>!
                            </p>
                            <p className="welcome-hint">
                                Выберите отель для работы в меню выше
                            </p>
                        </div>
                    )}
                </div>
                <div className="home-hero-image">
                    <img src={backgroundImageUrl} alt="Hotel Manager" />
                </div>
            </div>

            <div className="home-features">
                <h2 className="features-title">Ключевые возможности</h2>
                <div className="features-grid">
                    {features.map((feature, index) => {
                        const iconUrl = getIconUrl(feature.icon)
                        return (
                            <div className="feature-card" key={index}>
                                <div className="feature-icon">
                                   <img src={iconUrl} alt={feature.name} className="feature-icon-img" />
                                </div>
                                <h3>{feature.name}</h3>
                                <p>{feature.description}</p>
                            </div>
                        )
                    })}
                </div>
            </div>
        </div>
    )
}

export default Home