import React, { useState, useEffect } from 'react'
import { hotelApi } from '../../api/hotelApi'
import { getImageUrl, getIconUrl } from '../../index'
import Pagination from '../Pagination'
import '../../styles/HotelEmployees.css'

function HotelEmployees({ hotelId }) {
    const [employees, setEmployees] = useState([])
    const [approvedUsers, setApprovedUsers] = useState([])
    const [activeList, setActiveList] = useState('employees')
    const [loading, setLoading] = useState(true)
    const [searchTerm, setSearchTerm] = useState('')
    const [searchInput, setSearchInput] = useState('')
    const [approvedSearchTerm, setApprovedSearchTerm] = useState('')
    const [approvedSearchInput, setApprovedSearchInput] = useState('')
    const [selectedDepartment, setSelectedDepartment] = useState('all')
    const [departments, setDepartments] = useState([])
    const [isFilterOpen, setIsFilterOpen] = useState(false)

    const [employeesPagination, setEmployeesPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    const [approvedPagination, setApprovedPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    const profileIconUrl = getIconUrl('profile')
    const filterIconUrl = getIconUrl('filter')

    useEffect(() => {
        if (hotelId) {
            loadDepartments()
            loadApprovedUsers(1, true)
        }
    }, [hotelId])

    useEffect(() => {
        const timer = setTimeout(() => {
            setSearchTerm(searchInput)
            setEmployeesPagination(prev => ({ ...prev, currentPage: 1 }))
        }, 500)
        return () => clearTimeout(timer)
    }, [searchInput])

    useEffect(() => {
        const timer = setTimeout(() => {
            setApprovedSearchTerm(approvedSearchInput)
            setApprovedPagination(prev => ({ ...prev, currentPage: 1 }))
        }, 500)
        return () => clearTimeout(timer)
    }, [approvedSearchInput])

    useEffect(() => {
        if (activeList === 'employees' && hotelId) {
            loadEmployees(1)
        }
    }, [hotelId, activeList, searchTerm, selectedDepartment])

    useEffect(() => {
        if (activeList === 'approved' && hotelId) {
            loadApprovedUsers(1)
        }
    }, [hotelId, activeList, approvedSearchTerm])

    const loadDepartments = async () => {
        try {
            const response = await hotelApi.getHotelDepartments(hotelId)
            const data = response.data.data || response.data || []
            const departmentNames = data.map(dept => dept.name)
            setDepartments(departmentNames)
        } catch (error) {
            console.error('Error loading departments:', error)
        }
    }

    const loadEmployees = async (page) => {
        setLoading(true)
        try {
            const response = await hotelApi.getEmployees(hotelId, searchTerm, selectedDepartment, page, 20)
            const data = response.data.data || response.data
            setEmployees(data.items || [])
            setEmployeesPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading employees:', error)
        } finally {
            setLoading(false)
        }
    }

    const loadApprovedUsers = async (page, silent = false) => {
        if (!silent) {
            setLoading(true)
        }
        try {
            const response = await hotelApi.getApprovedResumes(hotelId, approvedSearchTerm, page, 20)
            const data = response.data.data || response.data
            setApprovedUsers(data.items || [])
            setApprovedPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading approved users:', error)
        } finally {
            if (!silent) {
                setLoading(false)
            }
        }
    }

    const handleEmployeesPageChange = (page) => {
        loadEmployees(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleApprovedPageChange = (page) => {
        loadApprovedUsers(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const getDepartmentDisplayName = (deptName) => {
        return deptName
    }

    if (loading && employees.length === 0 && approvedUsers.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="hotel-employees">
            <div className="employees-header">
                <h2>Сотрудники и кандидаты</h2>
            </div>

            <div className="employees-tabs">
                <button
                    className={`tab-btn ${activeList === 'employees' ? 'active' : ''}`}
                    onClick={() => setActiveList('employees')}
                >
                    Сотрудники
                    <span className="count">{employeesPagination.totalCount}</span>
                </button>
                <button
                    className={`tab-btn ${activeList === 'approved' ? 'active' : ''}`}
                    onClick={() => setActiveList('approved')}
                >
                    Одобренные резюме
                    <span className="count">{approvedPagination.totalCount}</span>
                </button>
            </div>

            {activeList === 'employees' && (
                <>
                    <div className="employees-filters">
                        <div className="search-wrapper">
                            <input
                                type="text"
                                placeholder="Поиск по ФИО..."
                                value={searchInput}
                                onChange={(e) => setSearchInput(e.target.value)}
                                className="search-input"
                            />
                        </div>

                        <div className="filter-dropdown">
                            <button
                                className="filter-trigger"
                                onClick={() => setIsFilterOpen(!isFilterOpen)}
                            >
                                {filterIconUrl && <img src={filterIconUrl} alt="filter" className="filter-icon" />}
                                <span>{selectedDepartment === 'all' ? 'Все отделы' : getDepartmentDisplayName(selectedDepartment)}</span>
                                <span className="filter-arrow">{isFilterOpen ? '▲' : '▼'}</span>
                            </button>

                            {isFilterOpen && (
                                <div className="filter-menu">
                                    <div
                                        className={`filter-option ${selectedDepartment === 'all' ? 'active' : ''}`}
                                        onClick={() => {
                                            setSelectedDepartment('all')
                                            setIsFilterOpen(false)
                                        }}
                                    >
                                        Все отделы
                                    </div>
                                    {departments.map(dept => (
                                        <div
                                            key={dept}
                                            className={`filter-option ${selectedDepartment === dept ? 'active' : ''}`}
                                            onClick={() => {
                                                setSelectedDepartment(dept)
                                                setIsFilterOpen(false)
                                            }}
                                        >
                                            {dept}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="employees-list">
                        {employees.length === 0 ? (
                            <div className="no-data">Нет сотрудников</div>
                        ) : (
                            employees.map((emp, index) => (
                                <div key={emp.id} className="employee-card">
                                    <div className="employee-number">
                                        {(employeesPagination.currentPage - 1) * employeesPagination.pageSize + index + 1}
                                    </div>

                                    <div className="employee-avatar">
                                        {emp.avatarUrl ? (
                                            <img src={getImageUrl(emp.avatarUrl)} alt="avatar" />
                                        ) : (
                                            <img src={profileIconUrl} alt="avatar" />
                                        )}
                                    </div>

                                    <div className="employee-info">
                                        <div className="employee-name">
                                            {emp.lastName} {emp.firstName} {emp.patronymic || ''}
                                        </div>
                                        <div className="employee-details">
                                            <span className="detail-label">Отдел:</span>
                                            <span>{emp.departmentName}</span>
                                            <span className="detail-label">Должность:</span>
                                            <span>{emp.position}</span>
                                        </div>
                                    </div>

                                    <div className="employee-actions">
                                        <button className="edit-employee-btn">
                                            Изменить
                                        </button>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {employeesPagination.totalPages > 1 && (
                        <Pagination
                            currentPage={employeesPagination.currentPage}
                            totalPages={employeesPagination.totalPages}
                            onPageChange={handleEmployeesPageChange}
                        />
                    )}
                </>
            )}

            {activeList === 'approved' && (
                <>
                    <div className="approved-filters">
                        <div className="search-wrapper">
                            <input
                                type="text"
                                placeholder="Поиск по ФИО..."
                                value={approvedSearchInput}
                                onChange={(e) => setApprovedSearchInput(e.target.value)}
                                className="search-input"
                            />
                        </div>
                    </div>

                    <div className="employees-list">
                        {approvedUsers.length === 0 ? (
                            <div className="no-data">Нет одобренных резюме</div>
                        ) : (
                            approvedUsers.map((user, index) => (
                                <div key={user.id} className="employee-card">
                                    <div className="employee-number">
                                        {(approvedPagination.currentPage - 1) * approvedPagination.pageSize + index + 1}
                                    </div>

                                    <div className="employee-avatar">
                                        {user.avatarUrl ? (
                                            <img src={getImageUrl(user.avatarUrl)} alt="avatar" />
                                        ) : (
                                            <img src={profileIconUrl} alt="avatar" />
                                        )}
                                    </div>

                                    <div className="employee-info">
                                        <div className="employee-name">
                                            {user.lastName} {user.firstName} {user.patronymic || ''}
                                        </div>
                                        <div className="employee-details">
                                            <span className="detail-label">Желаемая должность:</span>
                                            <span>{user.desiredPosition}</span>
                                        </div>
                                        <div className="employee-date">
                                            Дата подачи: {new Date(user.createdAt).toLocaleDateString('ru-RU')}
                                        </div>
                                    </div>

                                    <div className="employee-actions">
                                        <button className="edit-employee-btn">
                                            Изменить
                                        </button>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {approvedPagination.totalPages > 1 && (
                        <Pagination
                            currentPage={approvedPagination.currentPage}
                            totalPages={approvedPagination.totalPages}
                            onPageChange={handleApprovedPageChange}
                        />
                    )}
                </>
            )}
        </div>
    )
}

export default HotelEmployees