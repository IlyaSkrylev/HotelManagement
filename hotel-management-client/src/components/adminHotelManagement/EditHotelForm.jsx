import React, { useState, useEffect } from 'react'
import { hotelApi } from '../../api/hotelApi'
import { getImageUrl, getIconUrl } from '../../index'
import '../../styles/HotelManagement.css'

function EditHotelForm({ hotelId, onUpdate, onInfoLoad }) {
    const [loading, setLoading] = useState(false)
    const [updating, setUpdating] = useState(false)
    const [message, setMessage] = useState({ text: '', type: '' })
    const [editForm, setEditForm] = useState({
        name: '',
        address: '',
        phone: '',
        email: '',
        description: '',
        image: null
    })
    const [imagePreview, setImagePreview] = useState(null)

    const clipIconUrl = getIconUrl('clip')
    const hotelIconUrl = getIconUrl('hotel')
    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')

    useEffect(() => {
        loadHotelInfo()
    }, [hotelId])

    const loadHotelInfo = async () => {
        setLoading(true)
        try {
            const response = await hotelApi.getHotelAdminInfo(hotelId)
            const data = response.data.data
            if (onInfoLoad) onInfoLoad(data)
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
            await loadHotelInfo()
            setEditForm(prev => ({ ...prev, image: null }))
            if (onUpdate) onUpdate()
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

    return (
        <div className="edit-hotel-container">
            {message.text && (
                <div className={`message ${message.type}`}>
                    {message.text}
                </div>
            )}

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

                <div className="form-group image-edit-group">
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

                <button type="submit" disabled={updating} className="submit-btn centered-submit-btn">
                    {updating ? 'Сохранение...' : 'Сохранить изменения'}
                </button>
            </form>
        </div>
    )
}

export default EditHotelForm