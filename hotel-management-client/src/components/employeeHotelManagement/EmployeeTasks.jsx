import React, { useState, useEffect } from 'react'
import { taskApi } from '../../api/taskApi'
import { getImageUrl, getIconUrl } from '../../index'
import Pagination from '../Pagination'
import '../../styles/EmployeeTasks.css'

function EmployeeTasks({ hotelId, userRole, currentUserDepartmentId, currentUserDepartmentName, currentEmployeeId }) {
    const [tasks, setTasks] = useState([])
    const [archivedTasks, setArchivedTasks] = useState([])
    const [loading, setLoading] = useState(true)
    const [showArchived, setShowArchived] = useState(false)
    const [selectedPriority, setSelectedPriority] = useState('all')
    const [priorities, setPriorities] = useState([])
    const [isPriorityFilterOpen, setIsPriorityFilterOpen] = useState(false)
    const [updatingTaskId, setUpdatingTaskId] = useState(null)

    const [pagination, setPagination] = useState({
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

    const filterIconUrl = getIconUrl('filter')

    useEffect(() => {
        loadPriorities()
    }, [])

    useEffect(() => {
        if (!currentEmployeeId) return 

        if (!showArchived) {
            loadTasks(1)
        } else {
            loadArchivedTasks(1)
        }
    }, [hotelId, showArchived, selectedPriority, currentEmployeeId])  

    useEffect(() => {
        if (!currentEmployeeId) return
        loadArchivedTasksCount()
    }, [currentEmployeeId, hotelId, selectedPriority])

    const loadArchivedTasksCount = async () => {
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const response = await taskApi.getMyTasks(hotelId, currentEmployeeId, true, priorityParam, 1, 1)
            const data = response.data.data
            setArchivedPagination(prev => ({
                ...prev,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            }))
        } catch (error) {
            console.error('Error loading archived tasks count:', error)
        }
    }

    const loadPriorities = async () => {
        try {
            const response = await taskApi.getTaskPriorities()
            const data = response.data.data
            setPriorities(data || [])
        } catch (error) {
            console.error('Error loading priorities:', error)
        }
    }

    const loadTasks = async (page) => {
        if (!currentEmployeeId) {
            console.log('No employeeId, skipping loadTasks')
            return
        }

        setLoading(true)
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const response = await taskApi.getMyTasks(hotelId, currentEmployeeId, false, priorityParam, page, 20)
            const data = response.data.data
            setTasks(data.items || [])
            setPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading tasks:', error)
        } finally {
            setLoading(false)
        }
    }

    const loadArchivedTasks = async (page) => {
        if (!currentEmployeeId) {
            console.log('No employeeId, skipping loadArchivedTasks')
            return
        }

        setLoading(true)
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const response = await taskApi.getMyTasks(hotelId, currentEmployeeId, true, priorityParam, page, 20)
            const data = response.data.data
            setArchivedTasks(data.items || [])
            setArchivedPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading archived tasks:', error)
        } finally {
            setLoading(false)
        }
    }

    const handlePageChange = (page) => {
        if (showArchived) {
            loadArchivedTasks(page)
        } else {
            loadTasks(page)
        }
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleAcceptTask = async (taskId) => {
        setUpdatingTaskId(taskId)
        try {
            const inProgressStatus = await taskApi.getTaskStatuses().then(res =>
                res.data.data.find(s => s.code === 'in_progress')
            )
            if (inProgressStatus) {
                await taskApi.updateTask(taskId, { taskStatusId: inProgressStatus.id })
                await loadTasks(pagination.currentPage)
            }
        } catch (error) {
            console.error('Error accepting task:', error)
            alert('Ошибка при принятии задачи')
        } finally {
            setUpdatingTaskId(null)
        }
    }

    const handleCompleteTask = async (taskId, requiresInspection) => {
        setUpdatingTaskId(taskId)
        try {
            let statusCode = 'completed'
            if (requiresInspection) {
                statusCode = 'review'
            }

            const targetStatus = await taskApi.getTaskStatuses().then(res =>
                res.data.data.find(s => s.code === statusCode)
            )

            if (targetStatus) {
                await taskApi.updateTask(taskId, { taskStatusId: targetStatus.id })
                await loadTasks(pagination.currentPage)
                await loadArchivedTasks(archivedPagination.currentPage)
            }
        } catch (error) {
            console.error('Error completing task:', error)
            alert('Ошибка при завершении задачи')
        } finally {
            setUpdatingTaskId(null)
        }
    }

    const getPriorityColor = (color) => {
        return color || '#6c757d'
    }

    const formatDateTime = (dateString) => {
        if (!dateString) return '—'
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
    }

    const getPriorityName = (priorityId) => {
        const priority = priorities.find(p => p.id === priorityId)
        return priority?.name || 'Неизвестно'
    }

    const getPriorityLevel = (priorityId) => {
        const priority = priorities.find(p => p.id === priorityId)
        return priority?.level || 0
    }

    if (!currentEmployeeId) {
        return <div className="loading">Загрузка информации о сотруднике...</div>
    }

    if (loading && tasks.length === 0 && archivedTasks.length === 0) {
        return <div className="loading">Загрузка задач...</div>
    }

    const currentTasks = showArchived ? archivedTasks : tasks
    const currentPagination = showArchived ? archivedPagination : pagination

    return (
        <div className="employee-tasks">
            <div className="tasks-header">
                <h2>Мои задачи</h2>
            </div>

            <div className="tasks-tabs">
                <button
                    className={`tab-btn ${!showArchived ? 'active' : ''}`}
                    onClick={() => {
                        setShowArchived(false)
                        setPagination(prev => ({ ...prev, currentPage: 1 }))
                    }}
                >
                    Активные задачи
                    <span className="count">{pagination.totalCount}</span>
                </button>
                <button
                    className={`tab-btn ${showArchived ? 'active' : ''}`}
                    onClick={() => {
                        setShowArchived(true)
                        setArchivedPagination(prev => ({ ...prev, currentPage: 1 }))
                    }}
                >
                    Архив
                    <span className="count">{archivedPagination.totalCount}</span>
                </button>
            </div>

            <div className="tasks-filters">
                <div className="filter-dropdown">
                    <button
                        className="filter-trigger"
                        onClick={() => setIsPriorityFilterOpen(!isPriorityFilterOpen)}
                    >
                        {filterIconUrl && <img src={filterIconUrl} alt="filter" className="filter-icon" />}
                        <span>{selectedPriority === 'all' ? 'Все приоритеты' : getPriorityName(parseInt(selectedPriority))}</span>
                        <span className="filter-arrow">{isPriorityFilterOpen ? '▲' : '▼'}</span>
                    </button>

                    {isPriorityFilterOpen && (
                        <div className="filter-menu">
                            <div
                                className={`filter-option ${selectedPriority === 'all' ? 'active' : ''}`}
                                onClick={() => {
                                    setSelectedPriority('all')
                                    setIsPriorityFilterOpen(false)
                                }}
                            >
                                Все приоритеты
                            </div>
                            {priorities.map(priority => (
                                <div
                                    key={priority.id}
                                    className={`filter-option ${selectedPriority === String(priority.id) ? 'active' : ''}`}
                                    onClick={() => {
                                        setSelectedPriority(String(priority.id))
                                        setIsPriorityFilterOpen(false)
                                    }}
                                >
                                    <span
                                        className="priority-dot"
                                        style={{ backgroundColor: priority.color }}
                                    ></span>
                                    {priority.name}
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            <div className="tasks-list">
                {currentTasks.length === 0 ? (
                    <div className="no-data">
                        {showArchived ? 'Нет завершённых задач' : 'Нет активных задач'}
                    </div>
                ) : (
                    currentTasks.map((task, index) => (
                        <div key={task.id} className="task-card">
                            <div className="task-number">
                                {(currentPagination.currentPage - 1) * currentPagination.pageSize + index + 1}
                            </div>

                            <div className="task-content">
                                <div className="task-header">
                                    <div className="task-type">
                                        <span className="type-badge">{task.taskTypeName}</span>
                                    </div>
                                    <div className="task-priority">
                                        <span
                                            className="priority-badge"
                                            style={{ backgroundColor: getPriorityColor(task.priorityColor) }}
                                        >
                                            {task.priorityName}
                                        </span>
                                    </div>
                                </div>

                                <div className="task-info">
                                    <div className="task-creator">
                                        <span className="label">Создал:</span>
                                        <span>{task.createdByName}</span>
                                    </div>
                                    {task.dueDate && (
                                        <div className="task-due-date">
                                            <span className="label">Срок:</span>
                                            <span>{formatDateTime(task.dueDate)}</span>
                                        </div>
                                    )}
                                    {task.roomNumber && (
                                        <div className="task-room">
                                            <span className="label">Номер:</span>
                                            <span>{task.roomNumber}</span>
                                        </div>
                                    )}
                                    {task.notes && (
                                        <div className="task-notes">
                                            <span className="label">Описание:</span>
                                            <span>{task.notes}</span>
                                        </div>
                                    )}
                                    {task.completedAt && (
                                        <div className="task-completed">
                                            <span className="label">Выполнена:</span>
                                            <span>{formatDateTime(task.completedAt)}</span>
                                        </div>
                                    )}
                                </div>
                            </div>

                            <div className="task-actions">
                                {!showArchived && (
                                    <>
                                        {task.taskStatusCode === 'pending' && (
                                            <button
                                                className="accept-task-btn"
                                                onClick={() => handleAcceptTask(task.id)}
                                                disabled={updatingTaskId === task.id}
                                            >
                                                {updatingTaskId === task.id ? 'Принятие...' : 'Принять'}
                                            </button>
                                        )}
                                        {task.taskStatusCode === 'in_progress' && (
                                            <button
                                                className="complete-task-btn"
                                                onClick={() => handleCompleteTask(task.id, task.requiresInspection)}
                                                disabled={updatingTaskId === task.id}
                                            >
                                                {updatingTaskId === task.id ? 'Завершение...' : 'Выполнить'}
                                            </button>
                                        )}
                                        {task.taskStatusCode === 'review' && (
                                            <div className="review-status">
                                                <span className="review-badge">На проверке</span>
                                            </div>
                                        )}
                                    </>
                                )}
                            </div>
                        </div>
                    ))
                )}
            </div>

            {currentPagination.totalPages > 1 && (
                <Pagination
                    currentPage={currentPagination.currentPage}
                    totalPages={currentPagination.totalPages}
                    onPageChange={handlePageChange}
                />
            )}
        </div>
    )
}

export default EmployeeTasks