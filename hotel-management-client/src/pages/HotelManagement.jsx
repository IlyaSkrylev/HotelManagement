import React, { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { getImageUrl, getIconUrl } from '../index'
import { hotelApi } from '../api/hotelApi'
import EditHotelForm from '../components/adminHotelManagement/EditHotelForm'
import HotelResumes from '../components/adminHotelManagement/HotelResumes'
import HotelEmployees from '../components/adminHotelManagement/HotelEmployees'
import Departments from '../components/adminHotelManagement/Departments'
import HotelStructure from '../components/adminHotelManagement/HotelStructure'
import HotelTasks from '../components/adminHotelManagement/HotelTasks'
import AdminPanel from '../components/adminHotelManagement/AdminPanel'
import ManagerPanel from '../components/managerHotelManagement/ManagerPanel'
import DepartmentSchedule from '../components/managerHotelManagement/DepartmentSchedule'
import EmployeePanel from '../components/employeeHotelManagement/EmployeePanel'
import EmployeeTasks from '../components/employeeHotelManagement/EmployeeTasks'
import EmployeeSchedule from '../components/employeeHotelManagement/EmployeeSchedule'
import '../styles/HotelManagement.css'

function HotelManagement() {
    const { hotelId } = useParams()
    const [activeTab, setActiveTab] = useState('edit')
    const [hotelInfo, setHotelInfo] = useState(null)
    const [userRole, setUserRole] = useState(null)
    const [loading, setLoading] = useState(true)
    const [currentUserDepartmentId, setCurrentUserDepartmentId] = useState(null)
    const [currentUserDepartmentName, setCurrentUserDepartmentName] = useState(null)
    const [currentEmployeeId, setCurrentEmployeeId] = useState(null)

    const locationIconUrl = getIconUrl('location')
    const phoneIconUrl = getIconUrl('phone')
    const emailIconUrl = getIconUrl('email')
    const hotelIconUrl = getIconUrl('hotel')

    useEffect(() => {
        if (hotelId) {
            loadUserRole()
        }
    }, [hotelId])

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

            if (role?.code === 'manager') {
                await loadManagerDepartment()
            }

            await loadCurrentEmployee()

            if (role?.code === 'admin') {
                setActiveTab('edit')
            } else if (role?.code === 'manager') {
                setActiveTab('departments')
            } else if (role?.code === 'employee') {
                setActiveTab('my-tasks')
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
            setHotelInfo({
                name: 'Загрузка...',
                address: '',
                phone: '',
                email: '',
                imageUrl: null
            })
        }
    }

    const loadCurrentEmployee = async () => {
        try {
            const response = await hotelApi.getCurrentUserEmployeeInfo(hotelId)
            const data = response.data.data
            if (data && data.id) {
                setCurrentEmployeeId(data.id)
            }
        } catch (error) {
            console.error('Error loading current employee:', error)
        }
    }

    const handleHotelUpdate = () => {
        window.location.reload()
    }

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
                        <HotelTasks
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                        />
                    )
                case 'resumes':
                    return <HotelResumes hotelId={hotelId} />
                case 'my-tasks':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeTasks
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
                case 'my-schedule':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeSchedule
                            hotelId={hotelId}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
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
                        <HotelTasks
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                        />
                    )
                case 'my-tasks':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeTasks
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
                case 'my-schedule':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeSchedule
                            hotelId={hotelId}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
                case 'department-schedule':
                    if (!currentUserDepartmentId) {
                        return <div className="loading">Загрузка...</div>
                    }
                    return (
                        <DepartmentSchedule
                            departmentId={currentUserDepartmentId}
                            departmentName={currentUserDepartmentName}
                        />
                    )
                default:
                    return null
            }
        } else if (userRole === 'employee') {
            switch (activeTab) {
                case 'my-tasks':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeTasks
                            hotelId={hotelId}
                            userRole={userRole}
                            currentUserDepartmentId={currentUserDepartmentId}
                            currentUserDepartmentName={currentUserDepartmentName}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
                case 'my-schedule':
                    if (!currentEmployeeId) {
                        return <div className="loading">Загрузка данных сотрудника...</div>
                    }
                    return (
                        <EmployeeSchedule
                            hotelId={hotelId}
                            currentEmployeeId={currentEmployeeId}
                        />
                    )
                default:
                    return null
            }
        }
        return null
    }

    const renderPanel = () => {
        const commonProps = {
            activeTab,
            setActiveTab,
            hotelInfo,
            locationIconUrl,
            phoneIconUrl,
            emailIconUrl,
            hotelIconUrl,
            hotelId,
            currentEmployeeId
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