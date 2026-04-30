import React, { useState, useEffect, useRef } from 'react'
import { departmentApi } from '../../api/departmentApi'
import { userRoleApi } from '../../api/userRoleApi'
import '../../styles/EmployeeEditModal.css'

function EmployeeEditModal({ isOpen, onClose, onSubmit, initialData, isApprovedResume = false, hotelId }) {
    const [formData, setFormData] = useState({
        roleId: '',
        departmentId: '',
        position: '',
        salary: '',
        salarySupplement: '',
        workingDayShifts: 0,
        workingNightShifts: 0,
        restDays: 0,
        dayShiftStart: '09:00',
        dayShiftEnd: '18:00',
        nightShiftStart: '21:00',
        nightShiftEnd: '06:00',
        shiftCycleStartsWithDay: true,
        shiftCycleStartDate: new Date().toISOString().split('T')[0]
    })
    const [error, setError] = useState('')
    const [submitting, setSubmitting] = useState(false)
    const [userRoles, setUserRoles] = useState([])
    const [departments, setDepartments] = useState([])
    const [totalCycleDays, setTotalCycleDays] = useState(0)
    const [isRoleDropdownOpen, setIsRoleDropdownOpen] = useState(false)
    const [isDepartmentDropdownOpen, setIsDepartmentDropdownOpen] = useState(false)
    const [isCycleStartDropdownOpen, setIsCycleStartDropdownOpen] = useState(false)

    const roleDropdownRef = useRef(null)
    const departmentDropdownRef = useRef(null)
    const cycleStartDropdownRef = useRef(null)

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (roleDropdownRef.current && !roleDropdownRef.current.contains(event.target)) {
                setIsRoleDropdownOpen(false)
            }
            if (departmentDropdownRef.current && !departmentDropdownRef.current.contains(event.target)) {
                setIsDepartmentDropdownOpen(false)
            }
            if (cycleStartDropdownRef.current && !cycleStartDropdownRef.current.contains(event.target)) {
                setIsCycleStartDropdownOpen(false)
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [])

    useEffect(() => {
        if (initialData) {
            setFormData({
                roleId: initialData.roleId || '',
                departmentId: initialData.departmentId || '',
                position: initialData.position || '',
                salary: initialData.salary || '',
                salarySupplement: initialData.salarySupplement || '',
                workingDayShifts: initialData.workingDayShifts || 0,
                workingNightShifts: initialData.workingNightShifts || 0,
                restDays: initialData.restDays || 0,
                dayShiftStart: initialData.dayShiftStart || '09:00',
                dayShiftEnd: initialData.dayShiftEnd || '18:00',
                nightShiftStart: initialData.nightShiftStart || '21:00',
                nightShiftEnd: initialData.nightShiftEnd || '06:00',
                shiftCycleStartsWithDay: initialData.shiftCycleStartsWithDay !== undefined ? initialData.shiftCycleStartsWithDay : true,
                shiftCycleStartDate: initialData.shiftCycleStartDate || new Date().toISOString().split('T')[0]
            })
            calculateTotalCycleDays(initialData.workingDayShifts || 0, initialData.workingNightShifts || 0, initialData.restDays || 0)
        } else {
            resetForm()
        }
    }, [initialData, isOpen])

    useEffect(() => {
        if (isOpen) {
            loadUserRoles()
            if (hotelId) {
                loadDepartments()
            }
        }
    }, [isOpen, hotelId])

    const loadUserRoles = async () => {
        try {
            const response = await userRoleApi.getAll()
            const data = response.data
            let roles = []
            if (data && data.data) {
                roles = data.data
            } else if (data && Array.isArray(data)) {
                roles = data
            }

           
            setUserRoles(roles)
        } catch (error) {
            console.error('Error loading user roles:', error)
           
        }
    }

    const loadDepartments = async () => {
        try {
            const response = await departmentApi.getDepartments(hotelId, '', 1, 100)
            const data = response.data.data
            setDepartments(data.items || [])
        } catch (error) {
            console.error('Error loading departments:', error)
        }
    }

    const resetForm = () => {
        setFormData({
            roleId: '',
            departmentId: '',
            position: '',
            salary: '',
            salarySupplement: '',
            workingDayShifts: 0,
            workingNightShifts: 0,
            restDays: 0,
            dayShiftStart: '09:00',
            dayShiftEnd: '18:00',
            nightShiftStart: '21:00',
            nightShiftEnd: '06:00',
            shiftCycleStartsWithDay: true,
            shiftCycleStartDate: new Date().toISOString().split('T')[0]
        })
        setTotalCycleDays(0)
        setError('')
    }

    const calculateTotalCycleDays = (dayShifts, nightShifts, rest) => {
        const total = (parseInt(dayShifts) || 0) + (parseInt(nightShifts) || 0) + (parseInt(rest) || 0)
        setTotalCycleDays(total)
        return total
    }

    const handleWorkingDaysChange = (value) => {
        const dayShifts = parseInt(value) || 0
        const nightShifts = formData.workingNightShifts
        const rest = formData.restDays
        setFormData({ ...formData, workingDayShifts: dayShifts })
        calculateTotalCycleDays(dayShifts, nightShifts, rest)
    }

    const handleWorkingNightChange = (value) => {
        const dayShifts = formData.workingDayShifts
        const nightShifts = parseInt(value) || 0
        const rest = formData.restDays
        setFormData({ ...formData, workingNightShifts: nightShifts })
        calculateTotalCycleDays(dayShifts, nightShifts, rest)
    }

    const handleRestDaysChange = (value) => {
        const dayShifts = formData.workingDayShifts
        const nightShifts = formData.workingNightShifts
        const rest = parseInt(value) || 0
        setFormData({ ...formData, restDays: rest })
        calculateTotalCycleDays(dayShifts, nightShifts, rest)
    }

    const handleSubmit = async () => {
        setError('')

        if (!formData.roleId) {
            setError('Выберите роль')
            return
        }

        const selectedRole = userRoles.find(r => r.id == formData.roleId)
        // Проверяем по code: manager или employee
        const needDept = selectedRole?.code === 'manager' || selectedRole?.code === 'employee'

        if (needDept && !formData.departmentId) {
            setError('Для роли "Менеджер" или "Сотрудник" необходимо выбрать отдел')
            return
        }

        if (!formData.position) {
            setError('Введите должность')
            return
        }

        const total = formData.workingDayShifts + formData.workingNightShifts + formData.restDays
        if (total === 0) {
            setError('Сумма рабочих дней, ночей и дней отдыха не может быть равна 0')
            return
        }

        if (formData.workingDayShifts > 0) {
            if (!formData.dayShiftStart || !formData.dayShiftEnd) {
                setError('Укажите время начала и окончания дневной смены')
                return
            }
        }

        if (formData.workingNightShifts > 0) {
            if (!formData.nightShiftStart || !formData.nightShiftEnd) {
                setError('Укажите время начала и окончания ночной смены')
                return
            }
        }

        setSubmitting(true)
        try {
            await onSubmit({
                ...formData,
                roleCode: selectedRole?.code,
                roleId: formData.roleId,
                totalCycleDays: total
            })
            onClose()
            resetForm()
        } catch (error) {
            setError(error.response?.data?.message || 'Ошибка при сохранении')
        } finally {
            setSubmitting(false)
        }
    }

    const getSelectedRoleName = () => {
        const role = userRoles.find(r => r.id == formData.roleId)
        return role?.name || 'Выберите роль'
    }

    const getSelectedDepartmentName = () => {
        const dept = departments.find(d => d.id == formData.departmentId)
        return dept?.name || 'Выберите отдел'
    }

    const getCycleStartLabel = () => {
        return formData.shiftCycleStartsWithDay ? 'Дневной смены' : 'Ночной смены'
    }

    const needDepartment = () => {
        const selectedRole = userRoles.find(r => r.id == formData.roleId)
        return selectedRole?.code === 'manager' || selectedRole?.code === 'employee'
    }

    if (!isOpen) return null

    const showDayShiftFields = formData.workingDayShifts > 0
    const showNightShiftFields = formData.workingNightShifts > 0

    return (
        <div className="employee-modal-overlay" onClick={onClose}>
            <div className="employee-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="employee-modal-header">
                    <h3>{isApprovedResume ? 'Принять на работу' : 'Редактирование сотрудника'}</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="employee-modal-body">
                    {error && <div className="modal-error">{error}</div>}

                    <div className="form-row">
                        <div className="form-group">
                            <label>Роль *</label>
                            <div className="custom-select" ref={roleDropdownRef}>
                                <button
                                    className={`custom-select-trigger ${isRoleDropdownOpen ? 'open' : ''}`}
                                    onClick={() => setIsRoleDropdownOpen(!isRoleDropdownOpen)}
                                    type="button"
                                >
                                    <span>{getSelectedRoleName()}</span>
                                    <span className="select-arrow">{isRoleDropdownOpen ? '▲' : '▼'}</span>
                                </button>
                                {isRoleDropdownOpen && (
                                    <div className="custom-select-dropdown">
                                        {userRoles.length === 0 ? (
                                            <div className="select-option-empty">Загрузка...</div>
                                        ) : (
                                            userRoles.map(role => (
                                                <div
                                                    key={role.id}
                                                    className={`select-option ${formData.roleId == role.id ? 'active' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, roleId: role.id, departmentId: '' })
                                                        setIsRoleDropdownOpen(false)
                                                    }}
                                                >
                                                    {role.name}
                                                    {formData.roleId == role.id && <span className="option-check">✓</span>}
                                                </div>
                                            ))
                                        )}
                                    </div>
                                )}
                            </div>
                        </div>

                        {needDepartment() && (
                            <div className="form-group">
                                <label>Отдел *</label>
                                <div className="custom-select" ref={departmentDropdownRef}>
                                    <button
                                        className={`custom-select-trigger ${isDepartmentDropdownOpen ? 'open' : ''}`}
                                        onClick={() => setIsDepartmentDropdownOpen(!isDepartmentDropdownOpen)}
                                        type="button"
                                    >
                                        <span>{getSelectedDepartmentName()}</span>
                                        <span className="select-arrow">{isDepartmentDropdownOpen ? '▲' : '▼'}</span>
                                    </button>
                                    {isDepartmentDropdownOpen && (
                                        <div className="custom-select-dropdown">
                                            {departments.length === 0 ? (
                                                <div className="select-option-empty">Нет отделов</div>
                                            ) : (
                                                departments.map(dept => (
                                                    <div
                                                        key={dept.id}
                                                        className={`select-option ${formData.departmentId == dept.id ? 'active' : ''}`}
                                                        onClick={() => {
                                                            setFormData({ ...formData, departmentId: dept.id })
                                                            setIsDepartmentDropdownOpen(false)
                                                        }}
                                                    >
                                                        {dept.name}
                                                        {formData.departmentId == dept.id && <span className="option-check">✓</span>}
                                                    </div>
                                                ))
                                            )}
                                        </div>
                                    )}
                                </div>
                            </div>
                        )}
                    </div>

                    <div className="form-group">
                        <label>Должность</label>
                        <input
                            type="text"
                            value={formData.position}
                            onChange={(e) => setFormData({ ...formData, position: e.target.value })}
                            placeholder="Введите должность"
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label>Оклад</label>
                            <input
                                type="number"
                                value={formData.salary}
                                onChange={(e) => setFormData({ ...formData, salary: e.target.value })}
                                placeholder="Оклад"
                            />
                        </div>
                        <div className="form-group">
                            <label>Надбавка</label>
                            <input
                                type="number"
                                value={formData.salarySupplement}
                                onChange={(e) => setFormData({ ...formData, salarySupplement: e.target.value })}
                                placeholder="Надбавка"
                            />
                        </div>
                    </div>

                    <div className="schedule-section">
                        <h4>Настройка графика работы</h4>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Рабочих дней</label>
                                <input
                                    type="number"
                                    min="0"
                                    value={formData.workingDayShifts}
                                    onChange={(e) => handleWorkingDaysChange(e.target.value)}
                                    placeholder="Количество рабочих дней"
                                />
                            </div>
                            <div className="form-group">
                                <label>Рабочих ночей</label>
                                <input
                                    type="number"
                                    min="0"
                                    value={formData.workingNightShifts}
                                    onChange={(e) => handleWorkingNightChange(e.target.value)}
                                    placeholder="Количество рабочих ночей"
                                />
                            </div>
                            <div className="form-group">
                                <label>Дней отдыха</label>
                                <input
                                    type="number"
                                    min="0"
                                    value={formData.restDays}
                                    onChange={(e) => handleRestDaysChange(e.target.value)}
                                    placeholder="Количество дней отдыха"
                                />
                            </div>
                        </div>

                        <div className="total-cycle">
                            <span className="total-label">Общая длительность цикла:</span>
                            <span className="total-value">{totalCycleDays} дней</span>
                        </div>

                        {showDayShiftFields && (
                            <div className="shift-time-section">
                                <h5>Дневная смена</h5>
                                <div className="form-row">
                                    <div className="form-group">
                                        <label>Начало смены</label>
                                        <input
                                            type="time"
                                            value={formData.dayShiftStart}
                                            onChange={(e) => setFormData({ ...formData, dayShiftStart: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Окончание смены</label>
                                        <input
                                            type="time"
                                            value={formData.dayShiftEnd}
                                            onChange={(e) => setFormData({ ...formData, dayShiftEnd: e.target.value })}
                                        />
                                    </div>
                                </div>
                            </div>
                        )}

                        {showNightShiftFields && (
                            <div className="shift-time-section">
                                <h5>Ночная смена</h5>
                                <div className="form-row">
                                    <div className="form-group">
                                        <label>Начало смены</label>
                                        <input
                                            type="time"
                                            value={formData.nightShiftStart}
                                            onChange={(e) => setFormData({ ...formData, nightShiftStart: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Окончание смены</label>
                                        <input
                                            type="time"
                                            value={formData.nightShiftEnd}
                                            onChange={(e) => setFormData({ ...formData, nightShiftEnd: e.target.value })}
                                        />
                                    </div>
                                </div>
                            </div>
                        )}

                        <div className="cycle-settings">
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Первый день цикла</label>
                                    <input
                                        type="date"
                                        value={formData.shiftCycleStartDate}
                                        onChange={(e) => setFormData({ ...formData, shiftCycleStartDate: e.target.value })}
                                    />
                                </div>
                                <div className="form-group">
                                    <label>Начать цикл с</label>
                                    <div className="custom-select" ref={cycleStartDropdownRef}>
                                        <button
                                            className={`custom-select-trigger ${isCycleStartDropdownOpen ? 'open' : ''}`}
                                            onClick={() => setIsCycleStartDropdownOpen(!isCycleStartDropdownOpen)}
                                            type="button"
                                        >
                                            <span>{getCycleStartLabel()}</span>
                                            <span className="select-arrow">{isCycleStartDropdownOpen ? '▲' : '▼'}</span>
                                        </button>
                                        {isCycleStartDropdownOpen && (
                                            <div className="custom-select-dropdown">
                                                <div
                                                    className={`select-option ${formData.shiftCycleStartsWithDay === true ? 'active' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, shiftCycleStartsWithDay: true })
                                                        setIsCycleStartDropdownOpen(false)
                                                    }}
                                                >
                                                    Дневной смены
                                                    {formData.shiftCycleStartsWithDay === true && <span className="option-check">✓</span>}
                                                </div>
                                                <div
                                                    className={`select-option ${formData.shiftCycleStartsWithDay === false ? 'active' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, shiftCycleStartsWithDay: false })
                                                        setIsCycleStartDropdownOpen(false)
                                                    }}
                                                >
                                                    Ночной смены
                                                    {formData.shiftCycleStartsWithDay === false && <span className="option-check">✓</span>}
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="employee-modal-footer">
                    <button className="cancel-btn" onClick={onClose}>Отмена</button>
                    <button className="save-btn" onClick={handleSubmit} disabled={submitting}>
                        {submitting ? 'Сохранение...' : (isApprovedResume ? 'Принять на работу' : 'Сохранить')}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default EmployeeEditModal