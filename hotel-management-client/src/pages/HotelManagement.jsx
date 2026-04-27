import React, { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { hotelApi } from '../api/hotelApi'
import { getImageUrl, getIconUrl } from '../index'
import '../styles/HotelManagement.css'

function HotelManagement() {
    const { hotelId } = useParams()
    const navigate = useNavigate()
    const { user } = useAuth()
    const [loading, setLoading] = useState(false)
    const [updating, setUpdating] = useState(false)
    const [message, setMessage] = useState({ text: '', type: '' })
    const [activeTab, setActiveTab] = useState('edit')
    const [hotelInfo, setHotelInfo] = useState(null)
    const [editForm, setEditForm] = useState({
        name: '',
        address: '',
        phone: '',
        email: '',
        description: '',
        image: null
    })
    const [imagePreview, setImagePreview] = useState(null)

    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')
    const clipIconUrl = getIconUrl('clip')

    useEffect(() => {
        loadHotelInfo()
    }, [hotelId])

    const loadHotelInfo = async () => {
        setLoading(true)
        try {
            const response = await hotelApi.getHotelAdminInfo(hotelId)
            const data = response.data.data
            setHotelInfo(data)
            setEditForm({
                name: data.name || '',
                address: data.address || '',
                phone: data.phone || '',
                email: data.email || '',
                description: data.description || '',
                image: null
            })
            setImagePreview(data.imageUrl ? getImageUrl(data.imageUrl) : null)
        } catch (error) {
            console.error('Error loading hotel info:', error)
            setMessage({ text: 'Ошибка загрузки информации об отеле', type: 'error' })
        } finally {
            setLoading(false)
        }
    }

    const handleEditChange = (e) => {
        setEditForm({ ...editForm, [e.target.name]: e.target.value })
    }

    const handleImageChange = (e) => {
        const file = e.target.files[0]
        if (file) {
            setEditForm({ ...editForm, image: file })
            setImagePreview(URL.createObjectURL(file))
        }
    }

    const handleEditSubmit = async (e) => {
        e.preventDefault()
        setUpdating(true)
        setMessage({ text: '', type: '' })

        try {
            await hotelApi.updateHotel(hotelId, editForm)
            setMessage({ text: 'Информация об отеле обновлена', type: 'success' })
            loadHotelInfo()
            setEditForm(prev => ({ ...prev, image: null }))
        } catch (error) {
            console.error('Update error:', error)
            setMessage({ text: error.response?.data?.message || 'Ошибка обновления', type: 'error' })
        } finally {
            setUpdating(false)
        }
    }

    if (loading) {
        return <div className="loading">Загрузка...</div>
    }

    if (!hotelInfo) {
        return <div className="error">Гостиница не найдена</div>
    }

    return (
        <div className="hotel-management-container">
            <div className="hotel-management-sidebar">
                <div className="hotel-management-image">
                    {imagePreview ? (
                        <img src={imagePreview} alt={hotelInfo.name} />
                    ) : hotelInfo.imageUrl ? (
                        <img src={getImageUrl(hotelInfo.imageUrl)} alt={hotelInfo.name} />
                    ) : (
                        <div className="image-placeholder">
                            <img src={hotelIconUrl} alt="hotel" />
                        </div>
                    )}
                </div>

                <div className="hotel-management-info">
                    <h3>{hotelInfo.name}</h3>
                    {hotelInfo.address && (
                        <p className="info-address">
                            <img src={locationIconUrl} alt="location" className="info-icon" />
                            <span>{hotelInfo.address}</span>
                        </p>
                    )}
                    {hotelInfo.phone && (
                        <p className="info-phone">
                            <img src={phoneIconUrl} alt="phone" className="info-icon" />
                            <span>{hotelInfo.phone}</span>
                        </p>
                    )}
                    {hotelInfo.email && (
                        <p className="info-email">
                            <img src={emailIconUrl} alt="email" className="info-icon" />
                            <span>{hotelInfo.email}</span>
                        </p>
                    )}
                    <p className="info-role">
                        <span className="label">Должность:</span>
                        <span className="value">{hotelInfo.userRole}</span>
                    </p>
                    <p className="info-department">
                        <span className="label">Отдел:</span>
                        <span className="value">{hotelInfo.departmentName}</span>
                    </p>
                </div>

                <button
                    className={`sidebar-tab ${activeTab === 'edit' ? 'active' : ''}`}
                    onClick={() => setActiveTab('edit')}
                >
                    Редактировать
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'departments' ? 'active' : ''}`}
                    onClick={() => setActiveTab('departments')}
                >
                    Отделы
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'employees' ? 'active' : ''}`}
                    onClick={() => setActiveTab('employees')}
                >
                    Сотрудники
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'tasks' ? 'active' : ''}`}
                    onClick={() => setActiveTab('tasks')}
                >
                    Задачи
                </button>
            </div>

            <div className="hotel-management-content">
                {message.text && (
                    <div className={`message ${message.type}`}>
                        {message.text}
                    </div>
                )}

                {activeTab === 'edit' && (
                    <form onSubmit={handleEditSubmit} className="edit-form">
                        <h2>Редактирование гостиницы</h2>

                        <div className="form-group">
                            <label>Название</label>
                            <input
                                type="text"
                                name="name"
                                value={editForm.name}
                                onChange={handleEditChange}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label>Адрес</label>
                            <input
                                type="text"
                                name="address"
                                value={editForm.address}
                                onChange={handleEditChange}
                            />
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Телефон</label>
                                <input
                                    type="tel"
                                    name="phone"
                                    value={editForm.phone}
                                    onChange={handleEditChange}
                                />
                            </div>
                            <div className="form-group">
                                <label>Email</label>
                                <input
                                    type="email"
                                    name="email"
                                    value={editForm.email}
                                    onChange={handleEditChange}
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label>Описание</label>
                            <textarea
                                name="description"
                                value={editForm.description}
                                onChange={handleEditChange}
                                rows="4"
                            />
                        </div>

                        <div className="form-group">
                            <label>Фотография гостиницы</label>
                            <div className="file-upload-wrapper">
                                <label className="file-upload-label">
                                    <img src={clipIconUrl} alt="clip" className="clip-icon" />
                                    <span>{editForm.image ? editForm.image.name : 'Выберите фотографию'}</span>
                                    <input
                                        type="file"
                                        accept="image/*"
                                        onChange={handleImageChange}
                                        className="file-input-hidden"
                                    />
                                </label>
                            </div>
                            {imagePreview && (
                                <div className="image-preview">
                                    <img src={imagePreview} alt="preview" />
                                </div>
                            )}
                        </div>

                        <button type="submit" disabled={updating} className="submit-btn">
                            {updating ? 'Сохранение...' : 'Сохранить изменения'}
                        </button>
                    </form>
                )}

                {activeTab === 'departments' && (
                    <div className="departments-section">
                        <h2>Управление отделами</h2>
                        <p>Здесь будет список отделов</p>
                    </div>
                )}

                {activeTab === 'employees' && (
                    <div className="employees-section">
                        <h2>Управление сотрудниками</h2>
                        <p>Здесь будет список сотрудников</p>
                    </div>
                )}

                {activeTab === 'tasks' && (
                    <div className="tasks-section">
                        <h2>Управление задачами</h2>
                        <p>Здесь будут задачи</p>
                    </div>
                )}
            </div>
        </div>
    )
}

export default HotelManagement