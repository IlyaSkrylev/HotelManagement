import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { getHotelImageUrl, getIconUrl } from '../index'
import '../styles/HotelCard.css'

function HotelCard({ hotel, onApplyResume }) {
    const { isAuthenticated } = useAuth()
    const navigate = useNavigate()

    const handleApplyResume = () => {
        if (!isAuthenticated) {
            navigate('/login')
            return
        }
        onApplyResume(hotel.id)
    }

    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')

    return (
        <div className="hotel-card">
            <div className="hotel-card__image">
                {hotel.imageUrl ? (
                    <img
                        src={getHotelImageUrl(hotel.imageUrl)}
                        alt={hotel.name}
                    />
                ) : (
                    <div className="hotel-card__image-placeholder">
                        <img src={hotelIconUrl} alt="hotel" className="placeholder-icon" />
                    </div>
                )}
            </div>

            <div className="hotel-card__info">
                <h3 className="hotel-card__title">{hotel.name}</h3>

                {hotel.address && (
                    <div className="hotel-card__detail">
                        <img src={locationIconUrl} alt="location" className="icon-img" />
                        <span>{hotel.address}</span>
                    </div>
                )}

                {hotel.phone && (
                    <div className="hotel-card__detail">
                        <img src={phoneIconUrl} alt="phone" className="icon-img" />
                        <span>{hotel.phone}</span>
                    </div>
                )}

                {hotel.email && (
                    <div className="hotel-card__detail">
                        <img src={emailIconUrl} alt="email" className="icon-img" />
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

            <div className="hotel-card__actions">
                <button onClick={handleApplyResume} className="resume-btn">
                    📄 Подать резюме
                </button>
            </div>
        </div>
    )
}

export default HotelCard