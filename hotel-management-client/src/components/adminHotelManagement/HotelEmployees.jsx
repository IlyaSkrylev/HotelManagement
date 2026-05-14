import React, { useState, useEffect } from 'react'
import { hotelApi } from '../../api/hotelApi'
import { getImageUrl, getIconUrl } from '../../index'
import Pagination from '../Pagination'
import EmployeeEditModal from './EmployeeEditModal'
import EmployeeViewModal from './EmployeeViewModal'
import FireEmployeeModal from './FireEmployeeModal'
import '../../styles/HotelEmployees.css'

function HotelEmployees({ hotelId, userRole, currentUserDepartmentId, currentUserDepartmentName }) {
    const [employees, setEmployees] = useState([])
    const [archivedEmployees, setArchivedEmployees] = useState([])
    const [approvedUsers, setApprovedUsers] = useState([])
    const [activeList, setActiveList] = useState('employees')
    const [showArchived, setShowArchived] = useState(false)
    const [loading, setLoading] = useState(true)
    const [searchTerm, setSearchTerm] = useState('')
    const [searchInput, setSearchInput] = useState('')
    const [archivedSearchTerm, setArchivedSearchTerm] = useState('')
    const [archivedSearchInput, setArchivedSearchInput] = useState('')
    const [approvedSearchTerm, setApprovedSearchTerm] = useState('')
    const [approvedSearchInput, setApprovedSearchInput] = useState('')
    const [selectedDepartment, setSelectedDepartment] = useState('all')
    const [departments, setDepartments] = useState([])
    const [isFilterOpen, setIsFilterOpen] = useState(false)
    const [editModalOpen, setEditModalOpen] = useState(false)
    const [viewModalOpen, setViewModalOpen] = useState(false)
    const [editingEmployee, setEditingEmployee] = useState(null)
    const [viewingEmployee, setViewingEmployee] = useState(null)
    const [isApprovedResumeMode, setIsApprovedResumeMode] = useState(false)
    const [fireModalOpen, setFireModalOpen] = useState(false)
    const [employeeToFire, setEmployeeToFire] = useState(null)
    const [error, setError] = useState('')

    const isManager = userRole === 'manager'
    const isAdmin = userRole === 'admin'

    const [employeesPagination, setEmployeesPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    const [archivedPagination, setArchivedPagination] = useState({
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
        if (hotelId && isAdmin) {
            loadDepartments()
        }
    }, [hotelId, isAdmin])

    useEffect(() => {
        if (hotelId) {
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
            setArchivedSearchTerm(archivedSearchInput)
            setArchivedPagination(prev => ({ ...prev, currentPage: 1 }))
        }, 500)
        return () => clearTimeout(timer)
    }, [archivedSearchInput])

    useEffect(() => {
        const timer = setTimeout(() => {
            setApprovedSearchTerm(approvedSearchInput)
            setApprovedPagination(prev => ({ ...prev, currentPage: 1 }))
        }, 500)
        return () => clearTimeout(timer)
    }, [approvedSearchInput])

    useEffect(() => {
        if (activeList === 'employees' && hotelId && !showArchived) {
            loadEmployees(employeesPagination.currentPage)
        } else if (activeList === 'employees' && hotelId && showArchived && isAdmin) {
            loadArchivedEmployees(archivedPagination.currentPage)
        }
    }, [hotelId, activeList, searchTerm, selectedDepartment, showArchived, archivedSearchTerm])

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
            const departmentParam = isManager ? currentUserDepartmentName : (selectedDepartment === 'all' ? '' : selectedDepartment)
            const response = await hotelApi.getEmployees(hotelId, searchTerm, departmentParam, false, page, 20)
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

    const loadArchivedEmployees = async (page) => {
        setLoading(true)
        try {
            const departmentParam = selectedDepartment === 'all' ? '' : selectedDepartment
            const response = await hotelApi.getEmployees(hotelId, archivedSearchTerm, departmentParam, true, page, 20)
            const data = response.data.data || response.data
            setArchivedEmployees(data.items || [])
            setArchivedPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading archived employees:', error)
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

    const handleArchivedPageChange = (page) => {
        loadArchivedEmployees(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleApprovedPageChange = (page) => {
        loadApprovedUsers(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const getDepartmentDisplayName = (deptName) => {
        return deptName
    }

    const handleEditEmployee = (employee) => {
        setEditingEmployee(employee)
        setEditModalOpen(true)
    }

    const handleViewEmployee = (employee) => {
        setViewingEmployee(employee)
        setViewModalOpen(true)
    }

    const handleFireEmployee = (employee) => {
        setEmployeeToFire(employee)
        setFireModalOpen(true)
    }

    const handleHireFromResume = (user) => {
        setEditingEmployee({
            resumeId: user.id,
            userId: user.userId,
            position: user.desiredPosition,
            salary: '',
            salarySupplement: '',
            shiftTypeId: '',
            workingDayShifts: 0,
            workingNightShifts: 0,
            restDays: 0,
            dayShiftStart: '09:00',
            dayShiftEnd: '18:00',
            nightShiftStart: '21:00',
            nightShiftEnd: '06:00',
            shiftCycleStartsWithDay: true,
            shiftCycleStartDate: new Date().toISOString().split('T')[0],
            departmentId: null,
            roleId: null
        })
        setIsApprovedResumeMode(true)
        setEditModalOpen(true)
    }

    const handleEmployeeSubmit = async (data) => {
        if (isApprovedResumeMode) {
            try {
                await hotelApi.hireFromResume(hotelId, editingEmployee.resumeId, {
                    roleId: data.roleId,
                    departmentId: data.departmentId,
                    position: data.position,
                    salary: data.salary,
                    salarySupplement: data.salarySupplement,
                    workingDayShifts: data.workingDayShifts || 0,
                    workingNightShifts: data.workingNightShifts || 0,
                    restDays: data.restDays || 0,
                    dayShiftStart: data.dayShiftStart || '09:00',
                    dayShiftEnd: data.dayShiftEnd || '18:00',
                    nightShiftStart: data.nightShiftStart || '21:00',
                    nightShiftEnd: data.nightShiftEnd || '06:00',
                    shiftCycleStartsWithDay: data.shiftCycleStartsWithDay,
                    shiftCycleStartDate: data.shiftCycleStartDate,
                    totalCycleDays: data.totalCycleDays || 0
                })

                setEmployeesPagination(prev => ({ ...prev, currentPage: 1 }))
                await loadEmployees(1)
                setApprovedPagination(prev => ({ ...prev, currentPage: 1 }))
                await loadApprovedUsers(1)
                setIsApprovedResumeMode(false)
                setEditingEmployee(null)
                setEditModalOpen(false)
            } catch (error) {
                console.error('Error hiring employee:', error)
                setError(error.response?.data?.message || 'Ошибка при найме сотрудника')
            }
        } else {
            try {
                const updateData = {
                    roleId: data.roleId,
                    departmentId: data.departmentId,
                    position: data.position,
                    salary: data.salary,
                    salarySupplement: data.salarySupplement,
                    workingDayShifts: data.workingDayShifts || 0,
                    workingNightShifts: data.workingNightShifts || 0,
                    restDays: data.restDays || 0,
                    dayShiftStart: data.dayShiftStart || '09:00',
                    dayShiftEnd: data.dayShiftEnd || '18:00',
                    nightShiftStart: data.nightShiftStart || '21:00',
                    nightShiftEnd: data.nightShiftEnd || '06:00',
                    shiftCycleStartsWithDay: data.shiftCycleStartsWithDay,
                    shiftCycleStartDate: data.shiftCycleStartDate,
                    totalCycleDays: data.totalCycleDays || 0,
                    vacationStartDate: data.vacationStartDate,
                    vacationEndDate: data.vacationEndDate,
                    vacationType: data.vacationType
                }

                await hotelApi.updateEmployee(editingEmployee.id, updateData)
                await loadEmployees(employeesPagination.currentPage)
                setEditModalOpen(false)
            } catch (error) {
                console.error('Error updating employee:', error)
                setError(error.response?.data?.message || 'Ошибка при обновлении сотрудника')
            }
        }
    }

    const confirmFireEmployee = async (reason) => {
        try {
            await hotelApi.fireEmployee(employeeToFire.id, reason)
            await loadEmployees(employeesPagination.currentPage)
            if (isAdmin && showArchived) {
                await loadArchivedEmployees(archivedPagination.currentPage)
            }
            setFireModalOpen(false)
            setEmployeeToFire(null)
        } catch (error) {
            console.error('Error firing employee:', error)
            throw error
        }
    }

    if (loading && employees.length === 0 && archivedEmployees.length === 0 && approvedUsers.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="hotel-employees">
            <div className="employees-header">
                <h2>Сотрудники {isManager && currentUserDepartmentName ? `отдела ${currentUserDepartmentName}` : ''}</h2>
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
                    {isAdmin && (
                        <div className="employees-archive-toggle">
                            <button
                                className={`archive-toggle-btn ${!showArchived ? 'active' : ''}`}
                                onClick={() => {
                                    setShowArchived(false)
                                    setEmployeesPagination(prev => ({ ...prev, currentPage: 1 }))
                                }}
                            >
                                Активные сотрудники
                            </button>
                            <button
                                className={`archive-toggle-btn ${showArchived ? 'active' : ''}`}
                                onClick={() => {
                                    setShowArchived(true)
                                    setArchivedPagination(prev => ({ ...prev, currentPage: 1 }))
                                }}
                            >
                                Архив
                            </button>
                        </div>
                    )}

                    <div className="employees-filters">
                        <div className="search-wrapper">
                            <input
                                type="text"
                                placeholder="Поиск по ФИО..."
                                value={showArchived ? archivedSearchInput : searchInput}
                                onChange={(e) => showArchived ? setArchivedSearchInput(e.target.value) : setSearchInput(e.target.value)}
                                className="search-input"
                            />
                        </div>

                        {isAdmin && !showArchived && (
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
                        )}
                    </div>

                    <div className="employees-list">
                        {(showArchived ? archivedEmployees : employees).length === 0 ? (
                            <div className="no-data">{showArchived ? 'Нет уволенных сотрудников' : 'Нет сотрудников'}</div>
                        ) : (
                            (showArchived ? archivedEmployees : employees).map((emp, index) => (
                                <div key={emp.id} className="employee-card">
                                    <div className="employee-number">
                                        {((showArchived ? archivedPagination.currentPage : employeesPagination.currentPage) - 1) * 20 + index + 1}
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
                                        {showArchived && emp.dismissalDate && (
                                            <div className="employee-dismissal-date">
                                                <span className="detail-label">Дата увольнения:</span>
                                                <span>{new Date(emp.dismissalDate).toLocaleDateString('ru-RU')}</span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="employee-actions">
                                        {showArchived ? (
                                            <button className="view-employee-btn" onClick={() => handleViewEmployee(emp)}>
                                                Просмотр
                                            </button>
                                        ) : (
                                            <>
                                                <button className="edit-employee-btn" onClick={() => handleEditEmployee(emp)}>
                                                    Изменить
                                                </button>
                                                <button className="fire-employee-btn" onClick={() => handleFireEmployee(emp)}>
                                                    Уволить
                                                </button>
                                            </>
                                        )}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {(showArchived ? archivedPagination.totalPages : employeesPagination.totalPages) > 1 && (
                        <Pagination
                            currentPage={showArchived ? archivedPagination.currentPage : employeesPagination.currentPage}
                            totalPages={showArchived ? archivedPagination.totalPages : employeesPagination.totalPages}
                            onPageChange={showArchived ? handleArchivedPageChange : handleEmployeesPageChange}
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
                                        {(approvedPagination.currentPage - 1) * 20 + index + 1}
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
                                        <button className="edit-employee-btn" onClick={() => handleHireFromResume(user)}>
                                            Принять на работу
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

            <EmployeeEditModal
                isOpen={editModalOpen}
                onClose={() => setEditModalOpen(false)}
                onSubmit={handleEmployeeSubmit}
                initialData={editingEmployee}
                isApprovedResume={isApprovedResumeMode}
                hotelId={hotelId}
                userRole={userRole}
                currentUserDepartmentId={currentUserDepartmentId}
                currentUserDepartmentName={currentUserDepartmentName}
            />

            {viewModalOpen && viewingEmployee && (
                <EmployeeViewModal
                    isOpen={viewModalOpen}
                    onClose={() => setViewModalOpen(false)}
                    employee={viewingEmployee}
                />
            )}

            <FireEmployeeModal
                isOpen={fireModalOpen}
                onClose={() => {
                    setFireModalOpen(false)
                    setEmployeeToFire(null)
                }}
                onConfirm={confirmFireEmployee}
                employeeName={employeeToFire ? `${employeeToFire.lastName} ${employeeToFire.firstName}` : ''}
            />
        </div>
    )
}

export default HotelEmployees