import React from 'react'
import '../styles/Footer.css'
import { getIconUrl } from '../index'

function Footer() {
    const currentYear = new Date().getFullYear()

    const emailIconUrl = getIconUrl('email')
    const phoneIconUrl = getIconUrl('phone')
    const locationIconUrl = getIconUrl('location')

    return (
        <footer className="footer">
            <div className="footer-container">
                <div className="footer-section">
                    <h3 className="footer-logo">HotelManager</h3>
                    <p className="footer-description">
                        Современная система для управления персоналом гостиничного комплекса.
                        Автоматизация кадрового учёта, планирование задач и контроль выполнения работ.
                    </p>
                </div>

                <div className="footer-section">
                    <h4>Навигация</h4>
                    <ul className="footer-links">
                        <li><a href="/">Главная</a></li>
                        <li><a href="/hotels">Отели</a></li>
                        <li><a href="/login">Вход</a></li>
                        <li><a href="/register">Регистрация</a></li>
                    </ul>
                </div>

                <div className="footer-section">
                    <h4>Контакты</h4>
                    <ul className="footer-contacts">
                        <li>
                            <img src={emailIconUrl} alt="email" className="footer-icon" />
                            <span>support@hotelmanager.by</span>
                        </li>
                        <li>
                            <img src={phoneIconUrl} alt="phone" className="footer-icon" />
                            <span>+375 (17) 123-45-67</span>
                        </li>
                        <li>
                            <img src={locationIconUrl} alt="location" className="footer-icon" />
                            <span>г. Минск, пр-т Независимости, 10</span>
                        </li>
                    </ul>
                </div>

                <div className="footer-section">
                    <h4>О нас</h4>
                    <p className="footer-about">
                        Мы создаём современные решения для автоматизации гостиничного бизнеса.
                        Наша миссия — сделать управление отелем простым, прозрачным и эффективным.
                    </p>
                </div>
            </div>

            <div className="footer-bottom">
                <p>© {currentYear} HotelManager. Все права защищены.</p>
            </div>
        </footer>
    )
}

export default Footer