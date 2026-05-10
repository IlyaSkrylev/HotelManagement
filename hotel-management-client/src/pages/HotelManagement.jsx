import React, { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { getImageUrl, getIconUrl } from '../index'
import { hotelApi } from '../api/hotelApi'
import EditHotelForm from '../components/adminHotelManagement/EditHotelForm'
import HotelResumes from '../components/adminHotelManagement/HotelResumes'
import HotelEmployees from '../components/adminHotelManagement/HotelEmployees'
import Departments from '../components/adminHotelManagement/Departments'
import HotelStructure from '../components/adminHotelManagement/HotelStructure'
import AdminPanel from '../components/adminHotelManagement/AdminPanel'
import ManagerPanel from '../components/managerHotelManagement/ManagerPanel'
import EmployeePanel from '../components/employeeHotelManagement/EmployeePanel'
import '../styles/HotelManagement.css'

function HotelManagement() {
    const { hotelId } = useParams()
    const [activeTab, setActiveTab] = useState('edit')
    const [hotelInfo, setHotelInfo] = useState(null)
    const [userRole, setUserRole] = useState(null)
    const [loading, setLoading] = useState(true)
    const [currentUserDepartmentId, setCurrentUserDepartmentId] = useState(null)
    const [currentUserDepartmentName, setCurrentUserDepartmentName] = useState(null)

    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')

    useEffect(() => {
        if (hotelId) {
            loadUserRole()
        }
    }, [hotelId])

    // Загрузка информации о гостинице для всех ролей
    useEffect(() => {
        if (hotelId && userRole) {
            loadHotelInfo()
        }
    }, [hotelId, userRole])

    const loadUserRole = async () => {
        try {
            const response = await hotelApi.getUserRoleInHotel(hotelId)
            const role = response.data.data
            setUserRole(role?.code || 'admin')

            // Если менеджер, получаем его отдел
            if (role?.code === 'manager') {
                await loadManagerDepartment()
            }

            if (role?.code === 'admin') {
                setActiveTab('edit')
            } else if (role?.code === 'manager') {
                setActiveTab('departments')
            } else if (role?.code === 'employee') {
                setActiveTab('tasks')
            }
        } catch (error) {
            console.error('Error loading user role:', error)
            setUserRole('admin')
        } finally {
            setLoading(false)
        }
    }

    const loadManagerDepartment = async () => {
        try {
            const response = await hotelApi.getCurrentUserEmployeeInfo(hotelId)
            const data = response.data.data
            setCurrentUserDepartmentId(data.departmentId)
            setCurrentUserDepartmentName(data.departmentName)
        } catch (error) {
            console.error('Error loading manager department:', error)
        }
    }

    const loadHotelInfo = async () => {
        try {
            const response = await hotelApi.getHotelAdminInfo(hotelId)
            const data = response.data.data
            setHotelInfo({
                id: data.id,
                name: data.name,
                address: data.address,
                phone: data.phone,
                email: data.email,
                description: data.description,
                imageUrl: data.imageUrl,
                userRole: data.userRole || (userRole === 'admin' ? 'Администратор' : userRole === 'manager' ? 'Менеджер' : 'Сотрудник'),
                departmentName: data.departmentName || 'Отдел'
            })
        } catch (error) {
            console.error('Error loading hotel info:', error)
            // Устанавливаем минимальную информацию, чтобы панель отображалась
            setHotelInfo({
                name: 'Загрузка...',
                address: '',
                phone: '',
                email: '',
                imageUrl: null
            })
        }
    }

    const handleHotelUpdate = () => {
        window.location.reload()
    }

    // Рендер контента в зависимости от роли
    // Рендер контента в зависимости от роли
    const renderContent = () => {
        if (userRole === 'admin') {
            switch (activeTab) {
                case 'edit':
                    return (
                        <EditHotelForm
                            hotelId={hotelId}
                            onUpdate={handleHotelUpdate}
                            onInfoLoad={setHotelInfo}
                        />
                    )
                case 'structure':
                    return <HotelStructure hotelId={hotelId} />
                case 'departments':
                    return <Departments hotelId={hotelId} />
                case 'employees':
                    return (
                        <HotelEmployees
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                        />
                    )
                case 'tasks':
                    return (
                        <div className="tasks-section">
                            <h2>Управление задачами</h2>
                            <p>Здесь будут задачи</p>
                        </div>
                    )
                case 'resumes':
                    return <HotelResumes hotelId={hotelId} />
                default:
                    return null
            }
        } else if (userRole === 'manager') {
            switch (activeTab) {
                case 'departments':
                    return <Departments hotelId={hotelId} />
                case 'employees':
                    return (
                        <HotelEmployees
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                        />
                    )
                case 'tasks':
                    return (
                        <div className="tasks-section">
                            <h2>Управление задачами</h2>
                            <p>Здесь будут задачи для менеджера</p>
                        </div>
                    )
                default:
                    return null
            }
        } else if (userRole === 'employee') {
            switch (activeTab) {
                case 'tasks':
                    return (
                        <div className="tasks-section">
                            <h2>Мои задачи</h2>
                            <p>Здесь будут задачи сотрудника</p>
                        </div>
                    )
                case 'schedule':
                    return (
                        <div className="schedule-section">
                            <h2>Мой график работы</h2>
                            <p>Здесь будет график работы</p>
                        </div>
                    )
                default:
                    return null
            }
        }
        return null
    }

    // Рендер панели в зависимости от роли
    const renderPanel = () => {
        const commonProps = {
            activeTab,
            setActiveTab,
            hotelInfo,
            locationIconUrl,
            phoneIconUrl,
            emailIconUrl,
            hotelIconUrl,
            hotelId
        }

        if (userRole === 'admin') {
            return <AdminPanel {...commonProps} />
        } else if (userRole === 'manager') {
            return <ManagerPanel {...commonProps} />
        } else if (userRole === 'employee') {
            return <EmployeePanel {...commonProps} />
        }
        return null
    }

    if (loading) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="hotel-management-container">
            {renderPanel()}
            <div className="hotel-management-content">
                {renderContent()}
            </div>
        </div>
    )
}

export default HotelManagement