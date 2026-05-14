import React, { useState, useEffect, useRef } from 'react'
import { taskApi } from '../../api/taskApi'
import { hotelStructureApi } from '../../api/hotelStructureApi'
import { getImageUrl, getIconUrl } from '../../index'
import '../../styles/TaskModal.css'

function TaskModal({
    isOpen,
    onClose,
    onSubmit,
    initialData,
    hotelId,
    departmentId,
    userRole,
    taskStatuses,
    taskPriorities,
    taskTypes,
    onTaskTypesChange
}) {
    const [formData, setFormData] = useState({
        taskTypeId: '',
        newTaskTypeName: '',
        priorityId: '',
        taskStatusId: '',
        assignedToId: '',
        roomId: '',
        dueDateTime: '',
        notes: '',
        requiresInspection: false
    })
    const [employees, setEmployees] = useState([])
    const [employeesSearch, setEmployeesSearch] = useState('')
    const [rooms, setRooms] = useState([])
    const [roomsSearch, setRoomsSearch] = useState('')
    const [taskTypesSearch, setTaskTypesSearch] = useState('')
    const [loadingEmployees, setLoadingEmployees] = useState(false)
    const [loadingRooms, setLoadingRooms] = useState(false)
    const [error, setError] = useState('')
    const [submitting, setSubmitting] = useState(false)
    const [isEmployeeDropdownOpen, setIsEmployeeDropdownOpen] = useState(false)
    const [isRoomDropdownOpen, setIsRoomDropdownOpen] = useState(false)
    const [isTypeDropdownOpen, setIsTypeDropdownOpen] = useState(false)
    const [isPriorityDropdownOpen, setIsPriorityDropdownOpen] = useState(false)
    const [isStatusDropdownOpen, setIsStatusDropdownOpen] = useState(false)
    const [isCustomTypeMode, setIsCustomTypeMode] = useState(false)

    const employeeDropdownRef = useRef(null)
    const roomDropdownRef = useRef(null)
    const typeDropdownRef = useRef(null)
    const priorityDropdownRef = useRef(null)
    const statusDropdownRef = useRef(null)
    const typeSearchInputRef = useRef(null)

    const profileIconUrl = getIconUrl('profile')
    const isEditing = !!initialData?.id
    const isManager = userRole === 'manager'
    const isAdmin = userRole === 'admin'
    const canCreateTask = isAdmin || isManager

    const getDefaultDateTime = () => {
        const date = new Date()
        date.setHours(date.getHours() + 1)
        return date.toISOString().slice(0, 16)
    }

    useEffect(() => {
        if (isOpen && hotelId && canCreateTask) {
            loadEmployees('')
            loadRooms('')
            setError('')
        }
    }, [isOpen, hotelId, canCreateTask])

    useEffect(() => {
        if (initialData) {
            setFormData({
                taskTypeId: initialData.taskTypeId || '',
                newTaskTypeName: '',
                priorityId: initialData.priorityId || '',
                taskStatusId: initialData.taskStatusId || '',
                assignedToId: initialData.assignedToId || '',
                roomId: initialData.roomId || '',
                dueDateTime: initialData.dueDate ? new Date(initialData.dueDate).toISOString().slice(0, 16) : getDefaultDateTime(),
                notes: initialData.notes || '',
                requiresInspection: initialData.requiresInspection || false
            })
            setIsCustomTypeMode(false)
        } else {
            resetForm()
        }
    }, [initialData])

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (employeeDropdownRef.current && !employeeDropdownRef.current.contains(event.target)) {
                setIsEmployeeDropdownOpen(false)
            }
            if (roomDropdownRef.current && !roomDropdownRef.current.contains(event.target)) {
                setIsRoomDropdownOpen(false)
            }
            if (typeDropdownRef.current && !typeDropdownRef.current.contains(event.target)) {
                setIsTypeDropdownOpen(false)
            }
            if (priorityDropdownRef.current && !priorityDropdownRef.current.contains(event.target)) {
                setIsPriorityDropdownOpen(false)
            }
            if (statusDropdownRef.current && !statusDropdownRef.current.contains(event.target)) {
                setIsStatusDropdownOpen(false)
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [])

    const loadEmployees = async (search = '') => {
        setLoadingEmployees(true)
        try {
            const response = await taskApi.getEmployeesForTask(hotelId, departmentId, search)
            const data = response.data.data
            setEmployees(data || [])
        } catch (error) {
            console.error('Error loading employees:', error)
        } finally {
            setLoadingEmployees(false)
        }
    }

    const loadRooms = async () => {
        setLoadingRooms(true)
        try {
            const response = await hotelStructureApi.getRooms(hotelId, null, 1, 1000)
            const data = response.data.data
            const roomsList = data.items || []
            setRooms(roomsList)
        } catch (error) {
            console.error('Error loading rooms:', error)
        } finally {
            setLoadingRooms(false)
        }
    }

    const resetForm = () => {
        const defaultTaskTypeId = taskTypes.length > 0 ? taskTypes[0].id : ''

        setFormData({
            taskTypeId: defaultTaskTypeId,
            newTaskTypeName: '',
            priorityId: taskPriorities.length > 0 ? taskPriorities[0].id : '',
            taskStatusId: '',
            assignedToId: '',
            roomId: '',
            dueDateTime: getDefaultDateTime(),
            notes: '',
            requiresInspection: false
        })
        setIsCustomTypeMode(false)
        setTaskTypesSearch('')
        setError('')
    }

    useEffect(() => {
        if (!initialData && taskTypes.length > 0 && !formData.taskTypeId) {
            setFormData(prev => ({ ...prev, taskTypeId: taskTypes[0]?.id || '' }))
        }
    }, [taskTypes])

    useEffect(() => {
        if (!initialData && taskPriorities.length > 0 && !formData.priorityId) {
            setFormData(prev => ({ ...prev, priorityId: taskPriorities[0]?.id || '' }))
        }
    }, [taskPriorities])

    const handleEmployeeSearch = (value) => {
        setEmployeesSearch(value)
        loadEmployees(value)
    }

    const handleRoomSearch = (value) => {
        setRoomsSearch(value)
        if (value) {
            const filtered = rooms.filter(r =>
                r.roomNumber?.toLowerCase().includes(value.toLowerCase())
            )
            setRooms(filtered)
        } else {
            loadRooms()
        }
    }

    const handleTaskTypeSearch = (value) => {
        setTaskTypesSearch(value)
        if (value && !taskTypes.some(t => t.name.toLowerCase() === value.toLowerCase())) {
            setIsCustomTypeMode(true)
            setFormData(prev => ({ ...prev, newTaskTypeName: value }))
        } else {
            setIsCustomTypeMode(false)
            const matchedType = taskTypes.find(t => t.name.toLowerCase() === value.toLowerCase())
            if (matchedType) {
                setFormData(prev => ({ ...prev, taskTypeId: matchedType.id, newTaskTypeName: '' }))
            } else if (value) {
                setIsCustomTypeMode(true)
                setFormData(prev => ({ ...prev, newTaskTypeName: value, taskTypeId: '' }))
            } else {
                setFormData(prev => ({ ...prev, taskTypeId: '', newTaskTypeName: '' }))
            }
        }
    }

    const handleTaskTypeSelect = (typeId, typeName) => {
        setTaskTypesSearch(typeName)
        setIsCustomTypeMode(false)
        setFormData({ ...formData, taskTypeId: typeId, newTaskTypeName: '' })
        setIsTypeDropdownOpen(false)
    }

    const handleSubmit = async () => {
        setError('')

        let taskTypeId = formData.taskTypeId
        let newTaskTypeName = null

        if (isCustomTypeMode || (taskTypesSearch && !formData.taskTypeId)) {
            const searchTerm = taskTypesSearch || formData.newTaskTypeName
            if (!searchTerm.trim()) {
                setError('Введите название типа задачи')
                return
            }
            newTaskTypeName = searchTerm.trim()
            taskTypeId = null
        } else if (!formData.taskTypeId && !newTaskTypeName) {
            setError('Выберите или введите тип задачи')
            return
        }

        if (!formData.priorityId) {
            setError('Выберите приоритет')
            return
        }

        if (!formData.assignedToId) {
            setError('Выберите исполнителя')
            return
        }

        setSubmitting(true)
        try {
            const submitData = {
                taskTypeId: taskTypeId,
                newTaskTypeName: newTaskTypeName,
                priorityId: parseInt(formData.priorityId),
                assignedToId: parseInt(formData.assignedToId),
                roomId: formData.roomId ? parseInt(formData.roomId) : null,
                dueDate: formData.dueDateTime ? new Date(formData.dueDateTime).toISOString() : null,
                notes: formData.notes,
                requiresInspection: formData.requiresInspection
            }

            if (isEditing && formData.taskStatusId) {
                submitData.taskStatusId = parseInt(formData.taskStatusId)
            }

            await onSubmit(submitData)
            onClose()
            resetForm()
            onTaskTypesChange()
        } catch (error) {
            console.error('Submit error:', error)
            setError(error.response?.data?.message || 'Ошибка при сохранении')
        } finally {
            setSubmitting(false)
        }
    }

    const getSelectedEmployeeName = () => {
        const emp = employees.find(e => e.id == formData.assignedToId)
        return emp?.fullName || 'Выберите сотрудника'
    }

    const getSelectedRoomNumber = () => {
        if (!formData.roomId) return 'Не выбран'
        const room = rooms.find(r => r.id == formData.roomId)
        return room?.roomNumber || 'Не выбран'
    }

    const getSelectedTypeName = () => {
        if (isCustomTypeMode) {
            return formData.newTaskTypeName || taskTypesSearch || 'Введите или выберите тип'
        }
        const type = taskTypes.find(t => t.id == formData.taskTypeId)
        return type?.name || 'Введите или выберите тип'
    }

    const getSelectedPriorityName = () => {
        const priority = taskPriorities.find(p => p.id == formData.priorityId)
        return priority?.name || 'Выберите приоритет'
    }

    const getSelectedStatusName = () => {
        const status = taskStatuses.find(s => s.id == formData.taskStatusId)
        return status?.name || 'Выберите статус'
    }

    const filteredTaskTypes = taskTypes.filter(t =>
        t.name.toLowerCase().includes(taskTypesSearch.toLowerCase())
    )

    if (!isOpen || !canCreateTask) return null

    return (
        <div className="task-modal-overlay" onClick={onClose}>
            <div className="task-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="task-modal-header">
                    <h3>{isEditing ? 'Редактирование задачи' : 'Новая задача'}</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="task-modal-body">
                    {error && <div className="modal-error">{error}</div>}

                    <div className="form-row two-columns">
                        <div className="form-group">
                            <label>Тип задачи *</label>
                            <div className="custom-select" ref={typeDropdownRef}>
                                <button
                                    className={`custom-select-trigger ${isTypeDropdownOpen ? 'open' : ''}`}
                                    onClick={() => {
                                        setIsTypeDropdownOpen(!isTypeDropdownOpen)
                                        setTimeout(() => typeSearchInputRef.current?.focus(), 100)
                                    }}
                                    type="button"
                                >
                                    <span>{getSelectedTypeName()}</span>
                                    <span className="select-arrow">{isTypeDropdownOpen ? '▲' : '▼'}</span>
                                </button>
                                {isTypeDropdownOpen && (
                                    <div className="custom-select-dropdown task-type-dropdown">
                                        <div className="dropdown-search">
                                            <input
                                                ref={typeSearchInputRef}
                                                type="text"
                                                placeholder="Поиск или введите новый тип..."
                                                value={taskTypesSearch}
                                                onChange={(e) => handleTaskTypeSearch(e.target.value)}
                                                onClick={(e) => e.stopPropagation()}
                                            />
                                        </div>
                                        <div className="dropdown-options">
                                            {filteredTaskTypes.length > 0 ? (
                                                filteredTaskTypes.map(type => (
                                                    <div
                                                        key={type.id}
                                                        className={`select-option ${formData.taskTypeId == type.id ? 'active' : ''}`}
                                                        onClick={() => handleTaskTypeSelect(type.id, type.name)}
                                                    >
                                                        {type.name}
                                                        {formData.taskTypeId == type.id && <span className="option-check">✓</span>}
                                                    </div>
                                                ))
                                            ) : (
                                                <div className="select-option-empty">
                                                    {taskTypesSearch ? 'Создать новый тип' : 'Нет типов задач'}
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="form-group">
                            <label>Приоритет *</label>
                            <div className="custom-select" ref={priorityDropdownRef}>
                                <button
                                    className={`custom-select-trigger ${isPriorityDropdownOpen ? 'open' : ''}`}
                                    onClick={() => setIsPriorityDropdownOpen(!isPriorityDropdownOpen)}
                                    type="button"
                                >
                                    <span>{getSelectedPriorityName()}</span>
                                    <span className="select-arrow">{isPriorityDropdownOpen ? '▲' : '▼'}</span>
                                </button>
                                {isPriorityDropdownOpen && (
                                    <div className="custom-select-dropdown">
                                        {taskPriorities.map(priority => (
                                            <div
                                                key={priority.id}
                                                className={`select-option ${formData.priorityId == priority.id ? 'active' : ''}`}
                                                onClick={() => {
                                                    setFormData({ ...formData, priorityId: priority.id })
                                                    setIsPriorityDropdownOpen(false)
                                                }}
                                            >
                                                <span
                                                    className="priority-dot"
                                                    style={{ backgroundColor: priority.color }}
                                                ></span>
                                                {priority.name}
                                                {formData.priorityId == priority.id && <span className="option-check">✓</span>}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>

                    {isEditing && (
                        <div className="form-group">
                            <label>Статус</label>
                            <div className="custom-select" ref={statusDropdownRef}>
                                <button
                                    className={`custom-select-trigger ${isStatusDropdownOpen ? 'open' : ''}`}
                                    onClick={() => setIsStatusDropdownOpen(!isStatusDropdownOpen)}
                                    type="button"
                                >
                                    <span>{getSelectedStatusName()}</span>
                                    <span className="select-arrow">{isStatusDropdownOpen ? '▲' : '▼'}</span>
                                </button>
                                {isStatusDropdownOpen && (
                                    <div className="custom-select-dropdown">
                                        {taskStatuses.map(status => (
                                            <div
                                                key={status.id}
                                                className={`select-option ${formData.taskStatusId == status.id ? 'active' : ''}`}
                                                onClick={() => {
                                                    setFormData({ ...formData, taskStatusId: status.id })
                                                    setIsStatusDropdownOpen(false)
                                                }}
                                            >
                                                <span
                                                    className="status-dot"
                                                    style={{ backgroundColor: status.color }}
                                                ></span>
                                                {status.name}
                                                {formData.taskStatusId == status.id && <span className="option-check">✓</span>}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        </div>
                    )}

                    {/* Исполнитель */}
                    <div className="form-group">
                        <label>Исполнитель *</label>
                        <div className="custom-select" ref={employeeDropdownRef}>
                            <button
                                className={`custom-select-trigger ${isEmployeeDropdownOpen ? 'open' : ''}`}
                                onClick={() => setIsEmployeeDropdownOpen(!isEmployeeDropdownOpen)}
                                type="button"
                            >
                                <span>{getSelectedEmployeeName()}</span>
                                <span className="select-arrow">{isEmployeeDropdownOpen ? '▲' : '▼'}</span>
                            </button>
                            {isEmployeeDropdownOpen && (
                                <div className="employee-select-dropdown">
                                    <div className="dropdown-search">
                                        <input
                                            type="text"
                                            placeholder="Поиск по ФИО..."
                                            value={employeesSearch}
                                            onChange={(e) => handleEmployeeSearch(e.target.value)}
                                            autoFocus
                                        />
                                    </div>
                                    <div className="dropdown-options">
                                        {loadingEmployees ? (
                                            <div className="select-option-empty">Загрузка...</div>
                                        ) : (
                                            employees.map(emp => (
                                                <div
                                                    key={emp.id}
                                                    className={`select-option ${formData.assignedToId == emp.id ? 'active' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, assignedToId: emp.id })
                                                        setIsEmployeeDropdownOpen(false)
                                                        setEmployeesSearch('')
                                                        loadEmployees('')
                                                    }}
                                                >
                                                    {emp.avatarUrl ? (
                                                        <img
                                                            src={getImageUrl(emp.avatarUrl)}
                                                            alt="avatar"
                                                            className="option-avatar"
                                                        />
                                                    ) : (
                                                        <img
                                                            src={profileIconUrl}
                                                            alt="avatar"
                                                            className="option-avatar"
                                                        />
                                                    )}
                                                    <div className="option-info">
                                                        <div className="option-name">{emp.fullName}</div>
                                                        <div className="option-position">{emp.position}</div>
                                                    </div>
                                                    {formData.assignedToId == emp.id && <span className="option-check">✓</span>}
                                                </div>
                                            ))
                                        )}
                                        {employees.length === 0 && employeesSearch && !loadingEmployees && (
                                            <div className="select-option-empty">Сотрудники не найдены</div>
                                        )}
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="form-row two-columns">
                        <div className="form-group">
                            <label>Номер (опционально)</label>
                            <div className="custom-select" ref={roomDropdownRef}>
                                <button
                                    className={`custom-select-trigger ${isRoomDropdownOpen ? 'open' : ''}`}
                                    onClick={() => setIsRoomDropdownOpen(!isRoomDropdownOpen)}
                                    type="button"
                                >
                                    <span>{getSelectedRoomNumber()}</span>
                                    <span className="select-arrow">{isRoomDropdownOpen ? '▲' : '▼'}</span>
                                </button>
                                {isRoomDropdownOpen && (
                                    <div className="custom-select-dropdown">
                                        <div className="dropdown-search">
                                            <input
                                                type="text"
                                                placeholder="Поиск номера..."
                                                value={roomsSearch}
                                                onChange={(e) => handleRoomSearch(e.target.value)}
                                            />
                                        </div>
                                        <div className="dropdown-options">
                                            <div
                                                className={`select-option ${!formData.roomId ? 'active' : ''}`}
                                                onClick={() => {
                                                    setFormData({ ...formData, roomId: '' })
                                                    setIsRoomDropdownOpen(false)
                                                }}
                                            >
                                                Не выбран
                                                {!formData.roomId && <span className="option-check">✓</span>}
                                            </div>
                                            {rooms.map(room => (
                                                <div
                                                    key={room.id}
                                                    className={`select-option ${formData.roomId == room.id ? 'active' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, roomId: room.id })
                                                        setIsRoomDropdownOpen(false)
                                                    }}
                                                >
                                                    {room.roomNumber}
                                                    {formData.roomId == room.id && <span className="option-check">✓</span>}
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="form-group">
                            <label>Срок выполнения</label>
                            <input
                                type="datetime-local"
                                value={formData.dueDateTime}
                                onChange={(e) => setFormData({ ...formData, dueDateTime: e.target.value })}
                                className="datetime-input"
                            />
                        </div>
                    </div>

                    <div className="form-group">
                        <label>Описание</label>
                        <textarea
                            rows="3"
                            value={formData.notes}
                            onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                            placeholder="Введите описание задачи"
                        />
                    </div>

                    <div className="form-group checkbox-group">
                        <label className="checkbox-label">
                            <input
                                type="checkbox"
                                checked={formData.requiresInspection}
                                onChange={(e) => setFormData({ ...formData, requiresInspection: e.target.checked })}
                            />
                            <span>Требуется проверка после выполнения</span>
                        </label>
                    </div>
                </div>

                <div className="task-modal-footer">
                    <button className="cancel-btn" onClick={onClose}>
                        Отмена
                    </button>
                    <button className="save-btn" onClick={handleSubmit} disabled={submitting}>
                        {submitting ? 'Сохранение...' : (isEditing ? 'Сохранить' : 'Создать')}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default TaskModal