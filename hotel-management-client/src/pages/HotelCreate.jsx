import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useProject } from '../context/ProjectContext'
import '../styles/HotelCreate.css'

function HotelCreate() {
    const navigate = useNavigate()
    const { createHotel } = useProject()
    const [loading, setLoading] = useState(false)
    const [errors, setErrors] = useState({})
    const [formData, setFormData] = useState({
        name: '',
        address: '',
        phone: '',
        email: '',
        description: '',
        image: null
    })

    const handleChange = (e) => {
        const { name, value, type, files } = e.target
        if (type === 'file') {
            setFormData(prev => ({ ...prev, [name]: files[0] }))
        } else {
            setFormData(prev => ({ ...prev, [name]: value }))
        }
        if (errors[name]) {
            setErrors(prev => ({ ...prev, [name]: '' }))
        }
    }

    const validate = () => {
        const newErrors = {}

        if (!formData.name.trim()) {
            newErrors.name = 'Название гостиницы обязательно'
        } else if (formData.name.length > 255) {
            newErrors.name = 'Название не должно превышать 255 символов'
        }

        if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
            newErrors.email = 'Неверный формат email'
        }

        if (formData.phone && formData.phone.length > 20) {
            newErrors.phone = 'Телефон не должен превышать 20 символов'
        }

        return newErrors
    }

    const handleSubmit = async (e) => {
        e.preventDefault()

        const validationErrors = validate()
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors)
            return
        }

        setLoading(true)
        setErrors({})

        try {
            await createHotel(formData)
            navigate('/hotels')
        } catch (err) {
            console.error('Create hotel error:', err)
            setErrors({
                submit: err.response?.data?.message || 'Ошибка создания гостиницы'
            })
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="hotel-create-container">
            <div className="hotel-create-card">
                <div className="hotel-create-header">
                    <h1>Создание гостиницы</h1>
                    <Link to="/hotels" className="back-link">← Вернуться к списку</Link>
                </div>

                <form onSubmit={handleSubmit} className="hotel-create-form" encType="multipart/form-data">
                    <div className="form-group">
                        <label htmlFor="name">
                            Название <span className="required">*</span>
                        </label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            placeholder="Введите название гостиницы"
                            className={errors.name ? 'error' : ''}
                        />
                        {errors.name && <span className="error-message">{errors.name}</span>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="address">Адрес</label>
                        <input
                            type="text"
                            id="address"
                            name="address"
                            value={formData.address}
                            onChange={handleChange}
                            placeholder="Введите адрес гостиницы"
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="phone">Телефон</label>
                            <input
                                type="tel"
                                id="phone"
                                name="phone"
                                value={formData.phone}
                                onChange={handleChange}
                                placeholder="+375 XX XXX-XX-XX"
                                className={errors.phone ? 'error' : ''}
                            />
                            {errors.phone && <span className="error-message">{errors.phone}</span>}
                        </div>

                        <div className="form-group">
                            <label htmlFor="email">Email</label>
                            <input
                                type="email"
                                id="email"
                                name="email"
                                value={formData.email}
                                onChange={handleChange}
                                placeholder="info@hotel.com"
                                className={errors.email ? 'error' : ''}
                            />
                            {errors.email && <span className="error-message">{errors.email}</span>}
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="description">Описание</label>
                        <textarea
                            id="description"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            placeholder="Введите описание гостиницы"
                            rows="4"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="image">Фото гостиницы</label>
                        <input
                            type="file"
                            id="image"
                            name="image"
                            accept="image/*"
                            onChange={handleChange}
                        />
                    </div>

                    {errors.submit && (
                        <div className="submit-error">
                            {errors.submit}
                        </div>
                    )}

                    <div className="form-actions">
                        <button
                            type="button"
                            onClick={() => navigate('/hotels')}
                            className="cancel-btn"
                        >
                            Отмена
                        </button>
                        <button
                            type="submit"
                            disabled={loading}
                            className="submit-btn"
                        >
                            {loading ? 'Создание...' : 'Создать гостиницу'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default HotelCreate