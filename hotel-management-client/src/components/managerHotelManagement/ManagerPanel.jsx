import React from 'react'

function ManagerPanel({ activeTab, setActiveTab, hotelInfo, locationIconUrl, phoneIconUrl, emailIconUrl, hotelIconUrl }) {
    return (
        <div className="hotel-management-sidebar">
            <div className="hotel-management-image">
                {hotelInfo?.imageUrl ? (
                    <img src={hotelInfo.imageUrl} alt={hotelInfo.name} />
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
                    <span className="value">{hotelInfo?.userRole || 'Менеджер'}</span>
                </p>
                <p className="info-department">
                    <span className="label">Отдел:</span>
                    <span className="value">{hotelInfo?.departmentName || 'Отдел'}</span>
                </p>
            </div>

            <div className="sidebar-divider">Управление отделом</div>

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
                Задачи отдела
            </button>

            <button
                className={`sidebar-tab ${activeTab === 'department-schedule' ? 'active' : ''}`}
                onClick={() => setActiveTab('department-schedule')}
            >
                График отдела
            </button>

            <div className="sidebar-divider">Личный кабинет</div>

            <button
                className={`sidebar-tab ${activeTab === 'my-tasks' ? 'active' : ''}`}
                onClick={() => setActiveTab('my-tasks')}
            >
                Мои задачи
            </button>
            <button
                className={`sidebar-tab ${activeTab === 'my-schedule' ? 'active' : ''}`}
                onClick={() => setActiveTab('my-schedule')}
            >
                Мой график
            </button>
        </div>
    )
}

export default ManagerPanel