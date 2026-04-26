import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import '../styles/Auth.css'

function Register() {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        repPassword: '',
        firstName: '',
        lastName: '',
        patronymic: '',
        phone: ''
    })
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)
    const { register, loginAfterRegister } = useAuth()
    const navigate = useNavigate()

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        })
        setError('')
    }

    const handleSubmit = async (e) => {
        e.preventDefault()

        if (formData.password !== formData.repPassword) {
            setError('Пароли не совпадают')
            return
        }

        setLoading(true)
        setError('')

        try {
            const { repPassword, ...registerData } = formData
            const result = await register(registerData)

            const { accessToken, refreshToken, ...userData } = result.data

            localStorage.setItem('accessToken', accessToken)
            localStorage.setItem('refreshToken', refreshToken)

            loginAfterRegister(userData)

            navigate('/')
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка регистрации')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="auth-container">
            <div className="auth-card auth-card-wide">
                <div className="auth-header">
                    <h2>Создать аккаунт</h2>
                    <p>Присоединяйтесь к системе управления отелем</p>
                </div>

                <form onSubmit={handleSubmit} className="auth-form">
                    <div className="form-row-2">
                        <div className="input-group">
                            <label>Имя *</label>
                            <div className="input-wrapper">
                                <input
                                    type="text"
                                    name="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    placeholder="Иван"
                                    required
                                />
                            </div>
                        </div>

                        <div className="input-group">
                            <label>Фамилия *</label>
                            <div className="input-wrapper">
                                <input
                                    type="text"
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    placeholder="Иванов"
                                    required
                                />
                            </div>
                        </div>
                    </div>

                    <div className="form-row-2">
                        <div className="input-group">
                            <label>Отчество</label>
                            <div className="input-wrapper">
                                <input
                                    type="text"
                                    name="patronymic"
                                    value={formData.patronymic}
                                    onChange={handleChange}
                                    placeholder="Иванович"
                                />
                            </div>
                        </div>

                        <div className="input-group">
                            <label>Телефон</label>
                            <div className="input-wrapper">
                                <input
                                    type="tel"
                                    name="phone"
                                    value={formData.phone}
                                    onChange={handleChange}
                                    placeholder="+375 XX XXX-XX-XX"
                                />
                            </div>
                        </div>
                    </div>

                    <div className="input-group">
                        <label>Email *</label>
                        <div className="input-wrapper">
                            <input
                                type="email"
                                name="email"
                                value={formData.email}
                                onChange={handleChange}
                                placeholder="ivan@example.com"
                                required
                            />
                        </div>
                    </div>

                    <div className="form-row-2">
                        <div className="input-group">
                            <label>Пароль *</label>
                            <div className="input-wrapper">
                                <input
                                    type="password"
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    placeholder="Минимум 6 символов"
                                    required
                                />
                            </div>
                        </div>

                        <div className="input-group">
                            <label>Повторите пароль *</label>
                            <div className="input-wrapper">
                                <input
                                    type="password"
                                    name="repPassword"
                                    value={formData.repPassword}
                                    onChange={handleChange}
                                    placeholder="Подтвердите пароль"
                                    required
                                />
                            </div>
                        </div>
                    </div>

                    {error && <div className="auth-error">{error}</div>}

                    <button type="submit" disabled={loading} className="auth-btn">
                        {loading ? 'Регистрация...' : 'Зарегистрироваться'}
                    </button>
                </form>

                <div className="auth-footer">
                    <p>
                        Уже есть аккаунт? <Link to="/login">Войти</Link>
                    </p>
                </div>
            </div>
        </div>
    )
}

export default Register