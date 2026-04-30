import React, { useState } from 'react'
import { useParams } from 'react-router-dom'
import { getImageUrl, getIconUrl } from '../index'
import EditHotelForm from '../components/adminHotelManagement/EditHotelForm'
import HotelResumes from '../components/adminHotelManagement/HotelResumes'
import HotelEmployees from '../components/adminHotelManagement/HotelEmployees'
import Departments from '../components/adminHotelManagement/Departments'
import HotelStructure from '../components/adminHotelManagement/HotelStructure'
import '../styles/HotelManagement.css'

function HotelManagement() {
    const { hotelId } = useParams()
    const [activeTab, setActiveTab] = useState('edit')
    const [hotelInfo, setHotelInfo] = useState(null)

    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')

    const handleHotelUpdate = () => {
        window.location.reload()
    }

    return (
        <div className="hotel-management-container">
            <div className="hotel-management-sidebar">
                <div className="hotel-management-image">
                    {hotelInfo?.imageUrl ? (
                        <img src={getImageUrl(hotelInfo.imageUrl)} alt={hotelInfo.name} />
                    ) : (
                        <div className="image-placeholder">
                            <img src={hotelIconUrl} alt="hotel" />
                        </div>
                    )}
                </div>

                <div className="hotel-management-info">
                    <h3>{hotelInfo?.name || 'Загрузка...'}</h3>
                    {hotelInfo?.address && (
                        <p className="info-address">
                            <img src={locationIconUrl} alt="location" className="info-icon" />
                            <span>{hotelInfo.address}</span>
                        </p>
                    )}
                    {hotelInfo?.phone && (
                        <p className="info-phone">
                            <img src={phoneIconUrl} alt="phone" className="info-icon" />
                            <span>{hotelInfo.phone}</span>
                        </p>
                    )}
                    {hotelInfo?.email && (
                        <p className="info-email">
                            <img src={emailIconUrl} alt="email" className="info-icon" />
                            <span>{hotelInfo.email}</span>
                        </p>
                    )}
                    <p className="info-role">
                        <span className="label">Должность:</span>
                        <span className="value">{hotelInfo?.userRole || 'Администратор'}</span>
                    </p>
                    <p className="info-department">
                        <span className="label">Отдел:</span>
                        <span className="value">{hotelInfo?.departmentName || 'Администрация'}</span>
                    </p>
                </div>

                <button
                    className={`sidebar-tab ${activeTab === 'edit' ? 'active' : ''}`}
                    onClick={() => setActiveTab('edit')}
                >
                    Редактировать
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'structure' ? 'active' : ''}`}
                    onClick={() => setActiveTab('structure')}
                >
                    Структура
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
                <button
                    className={`sidebar-tab ${activeTab === 'resumes' ? 'active' : ''}`}
                    onClick={() => setActiveTab('resumes')}
                >
                    Заявки
                </button>
            </div>

            <div className="hotel-management-content">
                {activeTab === 'edit' && (
                    <EditHotelForm
                        hotelId={hotelId}
                        onUpdate={handleHotelUpdate}
                        onInfoLoad={setHotelInfo}
                    />
                )}

                {activeTab === 'departments' && (
                    <Departments hotelId={hotelId} />
                )}

                {activeTab === 'employees' && (
                    <HotelEmployees hotelId={hotelId} />
                )}

                {activeTab === 'tasks' && (
                    <div className="tasks-section">
                        <h2>Управление задачами</h2>
                        <p>Здесь будут задачи</p>
                    </div>
                )}

                {activeTab === 'resumes' && (
                    <HotelResumes hotelId={hotelId} />
                )}

                {activeTab === 'structure' && (
                    <HotelStructure hotelId={hotelId} />
                )}
            </div>
        </div>
    )
}

export default HotelManagement