import React, { useState, useEffect, useRef } from 'react'
import { taskApi } from '../../api/taskApi'
import { getImageUrl, getIconUrl } from '../../index'
import Pagination from '../Pagination'
import TaskModal from './TaskModal'
import '../../styles/HotelTasks.css'

function HotelTasks({ hotelId, userRole, currentUserDepartmentId, currentUserDepartmentName }) {
    const [tasks, setTasks] = useState([])
    const [archivedTasks, setArchivedTasks] = useState([])
    const [loading, setLoading] = useState(true)
    const [showArchived, setShowArchived] = useState(false)
    const [searchTerm, setSearchTerm] = useState('')
    const [searchInput, setSearchInput] = useState('')
    const [archivedSearchTerm, setArchivedSearchTerm] = useState('')
    const [archivedSearchInput, setArchivedSearchInput] = useState('')
    const [modalOpen, setModalOpen] = useState(false)
    const [editingTask, setEditingTask] = useState(null)
    const [taskStatuses, setTaskStatuses] = useState([])
    const [taskPriorities, setTaskPriorities] = useState([])
    const [taskTypes, setTaskTypes] = useState([])
    const [openStatusDropdownId, setOpenStatusDropdownId] = useState(null)
    const [selectedPriority, setSelectedPriority] = useState('all')
    const [selectedStatus, setSelectedStatus] = useState('all')
    const [isPriorityFilterOpen, setIsPriorityFilterOpen] = useState(false)
    const [isStatusFilterOpen, setIsStatusFilterOpen] = useState(false)

    const dropdownRefs = useRef({})

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

    const isManager = userRole === 'manager'
    const isAdmin = userRole === 'admin'
    const departmentId = isManager ? currentUserDepartmentId : null
    const canEditTask = isAdmin || isManager
    const filterIconUrl = getIconUrl('filter')
    useEffect(() => {
        const timer = setTimeout(() => {
            if (showArchived) {
                setArchivedSearchTerm(archivedSearchInput)
                setArchivedPagination(prev => ({ ...prev, currentPage: 1 }))
            } else {
                setSearchTerm(searchInput)
                setPagination(prev => ({ ...prev, currentPage: 1 }))
            }
        }, 500)
        return () => clearTimeout(timer)
    }, [searchInput, archivedSearchInput, showArchived])

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (openStatusDropdownId !== null) {
                const dropdownElement = dropdownRefs.current[openStatusDropdownId]
                if (dropdownElement && !dropdownElement.contains(event.target)) {
                    setOpenStatusDropdownId(null)
                }
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [openStatusDropdownId])

    useEffect(() => {
        loadTaskStatuses()
        loadTaskPriorities()
        loadTaskTypes()
    }, [hotelId, departmentId])

    useEffect(() => {
        if (!showArchived) {
            loadTasks(1)
        } else {
            loadArchivedTasks(1)
        }
    }, [hotelId, departmentId, showArchived, searchTerm, archivedSearchTerm, selectedPriority, selectedStatus])

    useEffect(() => {
        loadArchivedTasksCount()
    }, [hotelId, departmentId, selectedPriority, selectedStatus])

    const loadArchivedTasksCount = async () => {
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const statusParam = selectedStatus !== 'all' ? selectedStatus : null
            const response = await taskApi.getTasks(hotelId, departmentId, true, priorityParam, statusParam, '', 1, 20)
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

    const loadTaskStatuses = async () => {
        try {
            const response = await taskApi.getTaskStatuses()
            const data = response.data.data
            setTaskStatuses(data || [])
        } catch (error) {
            console.error('Error loading task statuses:', error)
        }
    }

    const loadTaskPriorities = async () => {
        try {
            const response = await taskApi.getTaskPriorities()
            const data = response.data.data
            setTaskPriorities(data || [])
        } catch (error) {
            console.error('Error loading task priorities:', error)
        }
    }

    const loadTaskTypes = async () => {
        try {
            const response = await taskApi.getTaskTypes(departmentId)
            const data = response.data.data
            setTaskTypes(data || [])
        } catch (error) {
            console.error('Error loading task types:', error)
        }
    }

    const getPriorityName = (priorityId) => {
        const priority = taskPriorities.find(p => p.id === priorityId)
        return priority?.name || 'Неизвестно'
    }

    const loadTasks = async (page) => {
        setLoading(true)
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const statusParam = selectedStatus !== 'all' ? selectedStatus : null
            const response = await taskApi.getTasks(hotelId, departmentId, false, priorityParam, statusParam, searchTerm, page, 20)
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
        setLoading(true)
        try {
            const priorityParam = selectedPriority !== 'all' ? selectedPriority : null
            const statusParam = selectedStatus !== 'all' ? selectedStatus : null
            const response = await taskApi.getTasks(hotelId, departmentId, true, priorityParam, statusParam, archivedSearchTerm, page, 20)
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

    const handleCreateTask = async (data) => {
        await taskApi.createTask(hotelId, data)
        if (showArchived) {
            setShowArchived(false)
        }
        setPagination(prev => ({ ...prev, currentPage: 1 }))
        await loadTasks(1)
        setModalOpen(false)
    }

    const handleUpdateTask = async (id, data) => {
        await taskApi.updateTask(id, data)
        await loadTasks(pagination.currentPage)
        await loadArchivedTasks(archivedPagination.currentPage)
        setModalOpen(false)
    }

    const handleDeleteTask = async (id, taskInfo) => {
        if (window.confirm(`Удалить задачу "${taskInfo}"?`)) {
            try {
                await taskApi.deleteTask(id)
                await loadTasks(pagination.currentPage)
                await loadArchivedTasks(archivedPagination.currentPage)
            } catch (error) {
                console.error('Error deleting task:', error)
                alert('Ошибка при удалении задачи')
            }
        }
    }

    const handleStatusChange = async (taskId, newStatusId) => {
        try {
            await taskApi.updateTask(taskId, { taskStatusId: newStatusId })
            await loadTasks(pagination.currentPage)
            await loadArchivedTasks(archivedPagination.currentPage)
            setOpenStatusDropdownId(null)
        } catch (error) {
            console.error('Error updating task status:', error)
            alert('Ошибка при изменении статуса')
        }
    }

    const getStatusColor = (color) => {
        return color || '#6c757d'
    }

    const getPriorityColor = (color) => {
        return color || '#6c757d'
    }

    const getStatusName = (statusId) => {
        const status = taskStatuses.find(s => s.id === statusId)
        return status?.name || 'Неизвестно'
    }

    const formatDateTime = (dateString) => {
        if (!dateString) return '—'
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
    }

    const canCreateTask = isAdmin || isManager

    if (loading && tasks.length === 0 && archivedTasks.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    const currentTasks = showArchived ? archivedTasks : tasks
    const currentPagination = showArchived ? archivedPagination : pagination

    return (
        <div className="hotel-tasks">
            <div className="tasks-header">
                <h2>Управление задачами {isManager && currentUserDepartmentName ? `отдела ${currentUserDepartmentName}` : ''}</h2>
                {canCreateTask && !showArchived && (
                    <button className="create-task-btn" onClick={() => {
                        setEditingTask(null)
                        setModalOpen(true)
                    }}>
                        + Создать задачу
                    </button>
                )}
            </div>

            <div className="tasks-tabs">
                <button
                    className={`tab-btn ${!showArchived ? 'active' : ''}`}
                    onClick={() => {
                        setShowArchived(false)
                        setSearchInput('')
                    }}
                >
                    Активные задачи
                    <span className="count">{pagination.totalCount}</span>
                </button>
                <button
                    className={`tab-btn ${showArchived ? 'active' : ''}`}
                    onClick={() => {
                        setShowArchived(true)
                        setArchivedSearchInput('')
                    }}
                >
                    Архив
                    <span className="count">{archivedPagination.totalCount}</span>
                </button>
            </div>

            <div className="tasks-filters">
                <div className="filters-row">
                    <div className="search-wrapper">
                        <input
                            type="text"
                            placeholder="Поиск по описанию или ФИО сотрудника..."
                            value={showArchived ? archivedSearchInput : searchInput}
                            onChange={(e) => showArchived ? setArchivedSearchInput(e.target.value) : setSearchInput(e.target.value)}
                            className="search-input"
                        />
                    </div>

                    <div className="filter-dropdown">
                        <button
                            className="filter-trigger"
                            onClick={() => setIsPriorityFilterOpen(!isPriorityFilterOpen)}
                        >
                            {filterIconUrl && <img src={filterIconUrl} alt="filter" className="filter-icon" />}
                            <span>{selectedPriority === 'all' ? 'Приоритет' : getPriorityName(parseInt(selectedPriority))}</span>
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
                                {taskPriorities.map(priority => (
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

                    {/* Фильтр по статусу */}
                    <div className="filter-dropdown">
                        <button
                            className="filter-trigger"
                            onClick={() => setIsStatusFilterOpen(!isStatusFilterOpen)}
                        >
                            {filterIconUrl && <img src={filterIconUrl} alt="filter" className="filter-icon" />}
                            <span>{selectedStatus === 'all' ? 'Статус' : taskStatuses.find(s => s.id == selectedStatus)?.name || 'Статус'}</span>
                            <span className="filter-arrow">{isStatusFilterOpen ? '▲' : '▼'}</span>
                        </button>
                        {isStatusFilterOpen && (
                            <div className="filter-menu">
                                <div
                                    className={`filter-option ${selectedStatus === 'all' ? 'active' : ''}`}
                                    onClick={() => {
                                        setSelectedStatus('all')
                                        setIsStatusFilterOpen(false)
                                    }}
                                >
                                    Все статусы
                                </div>
                                {taskStatuses.map(status => (
                                    <div
                                        key={status.id}
                                        className={`filter-option ${selectedStatus === String(status.id) ? 'active' : ''}`}
                                        onClick={() => {
                                            setSelectedStatus(String(status.id))
                                            setIsStatusFilterOpen(false)
                                        }}
                                    >
                                        <span
                                            className="status-dot"
                                            style={{ backgroundColor: status.color }}
                                        ></span>
                                        {status.name}
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
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
                                    <div className="task-assignee">
                                        <span className="label">Исполнитель:</span>
                                        <span>{task.assignedToName}</span>
                                    </div>
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
                                {canEditTask && (
                                    <div
                                        className="custom-select status-select"
                                        ref={el => dropdownRefs.current[task.id] = el}
                                    >
                                        <button
                                            className={`status-select-trigger ${openStatusDropdownId === task.id ? 'open' : ''}`}
                                            onClick={() => setOpenStatusDropdownId(openStatusDropdownId === task.id ? null : task.id)}
                                        >
                                            <span>{getStatusName(task.taskStatusId)}</span>
                                            <span className="select-arrow">{openStatusDropdownId === task.id ? '▲' : '▼'}</span>
                                        </button>
                                        {openStatusDropdownId === task.id && (
                                            <div className="status-select-dropdown">
                                                {taskStatuses.map(status => (
                                                    <div
                                                        key={status.id}
                                                        className={`status-option ${task.taskStatusId === status.id ? 'active' : ''}`}
                                                        onClick={() => handleStatusChange(task.id, status.id)}
                                                    >
                                                        <span
                                                            className="status-dot"
                                                            style={{ backgroundColor: status.color }}
                                                        ></span>
                                                        {status.name}
                                                        {task.taskStatusId === status.id && <span className="option-check">✓</span>}
                                                    </div>
                                                ))}
                                            </div>
                                        )}
                                    </div>
                                )}

                                {canEditTask && (
                                    <>
                                        <button
                                            className="edit-task-btn"
                                            onClick={() => {
                                                setEditingTask(task)
                                                setModalOpen(true)
                                            }}
                                        >
                                            Изменить
                                        </button>
                                        <button
                                            className="delete-task-btn"
                                            onClick={() => handleDeleteTask(task.id, `${task.taskTypeName} (${task.assignedToName})`)}
                                        >
                                            Удалить
                                        </button>
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

            <TaskModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                onSubmit={editingTask ?
                    (data) => handleUpdateTask(editingTask.id, data) :
                    (data) => handleCreateTask(data)}
                initialData={editingTask}
                hotelId={hotelId}
                departmentId={departmentId}
                userRole={userRole}
                taskStatuses={taskStatuses}
                taskPriorities={taskPriorities}
                taskTypes={taskTypes}
                onTaskTypesChange={loadTaskTypes}
            />
        </div>
    )
}

export default HotelTasks