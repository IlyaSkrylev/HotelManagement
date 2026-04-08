import React from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import '../styles/HotelCard.css'

const LocationIcon = () => <span className="icon">📍</span>
const PhoneIcon = () => <span className="icon">📞</span>
const EmailIcon = () => <span className="icon">✉️</span>

function HotelCard({ hotel, onDelete }) {
    const { isAuthenticated } = useAuth()

    const handleDelete = () => {
        if (window.confirm(`Удалить гостиницу "${hotel.name}"?`)) {
            onDelete(hotel.id)
        }
    }

    return (
        <div className="hotel-card">
            <div className="hotel-card__image">
                {hotel.imageUrl ? (
                    <img src={`http://localhost:5030${hotel.imageUrl}`} alt={hotel.name} />
                ) : (
                    <div className="hotel-card__image-placeholder">
                        🏨
                    </div>
                )}
            </div>

            <div className="hotel-card__info">
                <h3 className="hotel-card__title">{hotel.name}</h3>

                {hotel.address && (
                    <div className="hotel-card__detail">
                        <LocationIcon />
                        <span>{hotel.address}</span>
                    </div>
                )}

                {hotel.phone && (
                    <div className="hotel-card__detail">
                        <PhoneIcon />
                        <span>{hotel.phone}</span>
                    </div>
                )}

                {hotel.email && (
                    <div className="hotel-card__detail">
                        <EmailIcon />
                        <span>{hotel.email}</span>
                    </div>
                )}
            </div>

            <div className="hotel-card__divider"></div>

            <div className="hotel-card__description">
                <p>
                    {hotel.description && hotel.description.length > 120
                        ? hotel.description.substring(0, 120) + '...'
                        : hotel.description || 'Нет описания'}
                </p>
            </div>

            {isAuthenticated && (
                <div className="hotel-card__actions">
                    <Link to={`/hotels/edit/${hotel.id}`} className="edit-btn">
                        Редактировать
                    </Link>
                    <button onClick={handleDelete} className="delete-btn">
                        Удалить
                    </button>
                </div>
            )}
        </div>
    )
}

export default HotelCard