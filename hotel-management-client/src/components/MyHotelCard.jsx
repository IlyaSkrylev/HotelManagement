import React from 'react'
import { Link } from 'react-router-dom'
import { getHotelImageUrl, getIconUrl } from '../index'
import '../styles/MyHotelCard.css'

function MyHotelCard({ hotel }) {
    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')

    return (
        <div className="my-hotel-card">
            <div className="my-hotel-card__image">
                {hotel.imageUrl ? (
                    <img
                        src={getHotelImageUrl(hotel.imageUrl)}
                        alt={hotel.name}
                    />
                ) : (
                    <div className="my-hotel-card__image-placeholder">
                        <img src={hotelIconUrl} alt="hotel" className="placeholder-icon" />
                    </div>
                )}
            </div>

            <div className="my-hotel-card__info">
                <h3 className="my-hotel-card__title">{hotel.name}</h3>

                <div className="my-hotel-card__position">
                    <span className="position-label">Должность:</span>
                    <span className="position-value">{hotel.position}</span>
                </div>

                {hotel.departmentName && (
                    <div className="my-hotel-card__department">
                        <span className="department-label">Отдел:</span>
                        <span className="department-value">{hotel.departmentName}</span>
                    </div>
                )}

                {hotel.hireDate && (
                    <div className="my-hotel-card__hire-date">
                        <span className="hire-date-label">Дата приёма:</span>
                        <span className="hire-date-value">
                            {new Date(hotel.hireDate).toLocaleDateString('ru-RU')}
                        </span>
                    </div>
                )}

                {hotel.address && (
                    <div className="my-hotel-card__detail">
                        <img src={locationIconUrl} alt="location" className="icon-img" />
                        <span>{hotel.address}</span>
                    </div>
                )}

                {hotel.phone && (
                    <div className="my-hotel-card__detail">
                        <img src={phoneIconUrl} alt="phone" className="icon-img" />
                        <span>{hotel.phone}</span>
                    </div>
                )}

                {hotel.email && (
                    <div className="my-hotel-card__detail">
                        <img src={emailIconUrl} alt="email" className="icon-img" />
                        <span>{hotel.email}</span>
                    </div>
                )}
            </div>

            <div className="my-hotel-card__divider"></div>

            <div className="my-hotel-card__description">
                <p>
                    {hotel.description && hotel.description.length > 120
                        ? hotel.description.substring(0, 120) + '...'
                        : hotel.description || 'Нет описания'}
                </p>
            </div>

            <div className="my-hotel-card__actions">
                <Link to={`/hotels/${hotel.id}/management`} className="dashboard-btn">
                    Перейти к управлению
                </Link>
            </div>
        </div>
    )
}

export default MyHotelCard