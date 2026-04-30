import React, { useState, useEffect, useRef } from 'react'
import { getImageUrl, getIconUrl } from '../../index'

function DepartmentModal({
    isOpen,
    onClose,
    onSubmit,
    initialData,
    hotelId,
    loadEmployees: loadEmployeesFromParent
}) {
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        managerId: null
    })
    const [employees, setEmployees] = useState([])
    const [employeesSearch, setEmployeesSearch] = useState('')
    const [isManagerDropdownOpen, setIsManagerDropdownOpen] = useState(false)
    const [submitting, setSubmitting] = useState(false)
    const [loadingEmployees, setLoadingEmployees] = useState(false)
    const [error, setError] = useState('')

    const dropdownRef = useRef(null)
    const profileIconUrl = getIconUrl('profile')

    const isEditing = !!initialData?.id

    useEffect(() => {
        if (isOpen && hotelId) {
            loadEmployees('')
            setError('')
        }
    }, [isOpen, hotelId])

    useEffect(() => {
        if (initialData) {
            setFormData({
                name: initialData.name || '',
                description: initialData.description || '',
                managerId: initialData.managerId || null
            })
        } else {
            setFormData({
                name: '',
                description: '',
                managerId: null
            })
        }
    }, [initialData])

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsManagerDropdownOpen(false)
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [])

    const loadEmployees = async (search = '') => {
        setLoadingEmployees(true)
        try {
            const response = await loadEmployeesFromParent(hotelId, search)
            setEmployees(response || [])
        } catch (error) {
            console.error('Error loading employees:', error)
        } finally {
            setLoadingEmployees(false)
        }
    }

    const handleManagerSearch = (value) => {
        setEmployeesSearch(value)
        loadEmployees(value)
    }

    const handleSubmit = async () => {
        setError('')

        if (!formData.name.trim()) {
            setError('Введите название отдела')
            return
        }

        setSubmitting(true)
        try {
            await onSubmit(formData)
            onClose()
        } catch (error) {
            console.error('Error in submit:', error)
            const responseData = error.response?.data
            if (responseData?.message) {
                setError(responseData.message)
            } else if (responseData?.title) {
                setError(responseData.title)
            } else if (responseData?.errors) {
                const firstError = Object.values(responseData.errors).flat()?.[0]
                setError(firstError || 'Ошибка при сохранении отдела')
            } else if (error.message) {
                setError(error.message)
            } else {
                setError('Ошибка при сохранении отдела')
            }
        } finally {
            setSubmitting(false)
        }
    }

    const getSelectedManager = () => {
        if (!formData.managerId) return null
        return employees.find(e => e.id === formData.managerId)
    }

    const selectedManager = getSelectedManager()

    if (!isOpen) return null

    return (
        <div className="department-modal-overlay" onClick={onClose}>
            <div className="department-modal" onClick={(e) => e.stopPropagation()}>
                <div className="department-modal-header">
                    <h3>{isEditing ? 'Редактирование отдела' : 'Новый отдел'}</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="department-modal-body">
                    {error && (
                        <div className="modal-error">
                            {error}
                        </div>
                    )}

                    <div className="form-group">
                        <label>Название отдела *</label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            placeholder="Введите название отдела"
                            autoFocus
                        />
                    </div>

                    <div className="form-group">
                        <label>Описание</label>
                        <textarea
                            rows="3"
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            placeholder="Введите описание отдела"
                        />
                    </div>

                    <div className="form-group">
                        <label>Руководитель отдела</label>
                        <div className="manager-select" ref={dropdownRef}>
                            <div
                                className={`manager-select-trigger ${isManagerDropdownOpen ? 'open' : ''}`}
                                onClick={() => setIsManagerDropdownOpen(!isManagerDropdownOpen)}
                            >
                                {selectedManager ? (
                                    <div className="selected-manager">
                                        {selectedManager.avatarUrl ? (
                                            <img
                                                src={getImageUrl(selectedManager.avatarUrl)}
                                                alt="avatar"
                                                className="selected-manager-avatar"
                                            />
                                        ) : (
                                            <img
                                                src={profileIconUrl}
                                                alt="avatar"
                                                className="selected-manager-avatar"
                                            />
                                        )}
                                        <div className="selected-manager-info">
                                            <span className="selected-manager-name">{selectedManager.fullName}</span>
                                            <span className="selected-manager-position">{selectedManager.position}</span>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="selected-manager">
                                        <span className="selected-manager-name">Не назначен</span>
                                    </div>
                                )}
                                <span className="manager-select-arrow">{isManagerDropdownOpen ? '▲' : '▼'}</span>
                            </div>

                            {isManagerDropdownOpen && (
                                <div className="manager-dropdown">
                                    <div className="manager-search">
                                        <input
                                            type="text"
                                            placeholder="Поиск по ФИО..."
                                            value={employeesSearch}
                                            onChange={(e) => handleManagerSearch(e.target.value)}
                                            autoFocus
                                        />
                                    </div>
                                    <div className="manager-options-list">
                                        {loadingEmployees ? (
                                            <div className="loading-employees">Загрузка...</div>
                                        ) : (
                                            <>
                                                <div
                                                    className={`manager-option ${!formData.managerId ? 'selected' : ''}`}
                                                    onClick={() => {
                                                        setFormData({ ...formData, managerId: null })
                                                        setIsManagerDropdownOpen(false)
                                                        setEmployeesSearch('')
                                                        loadEmployees('')
                                                    }}
                                                >
                                                    <div className="manager-option-info">
                                                        <div className="manager-option-name">Не назначен</div>
                                                        <div className="manager-option-position">Без руководителя</div>
                                                    </div>
                                                    {!formData.managerId && (
                                                        <span className="option-check">✓</span>
                                                    )}
                                                </div>
                                                {employees.map(emp => (
                                                    <div
                                                        key={emp.id}
                                                        className={`manager-option ${formData.managerId === emp.id ? 'selected' : ''}`}
                                                        onClick={() => {
                                                            setFormData({ ...formData, managerId: emp.id })
                                                            setIsManagerDropdownOpen(false)
                                                            setEmployeesSearch('')
                                                            loadEmployees('')
                                                        }}
                                                    >
                                                        {emp.avatarUrl ? (
                                                            <img
                                                                src={getImageUrl(emp.avatarUrl)}
                                                                alt="avatar"
                                                                className="manager-option-avatar"
                                                            />
                                                        ) : (
                                                            <img
                                                                src={profileIconUrl}
                                                                alt="avatar"
                                                                className="manager-option-avatar"
                                                            />
                                                        )}
                                                        <div className="manager-option-info">
                                                            <div className="manager-option-name">{emp.fullName}</div>
                                                            <div className="manager-option-position">{emp.position}</div>
                                                        </div>
                                                        {formData.managerId === emp.id && (
                                                            <span className="option-check">✓</span>
                                                        )}
                                                    </div>
                                                ))}
                                                {employees.length === 0 && employeesSearch && !loadingEmployees && (
                                                    <div className="no-employees">
                                                        Сотрудники не найдены
                                                    </div>
                                                )}
                                            </>
                                        )}
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                <div className="department-modal-footer">
                    <button className="cancel-btn" onClick={onClose}>
                        Отмена
                    </button>
                    <button className="save-btn" onClick={handleSubmit} disabled={submitting}>
                        {submitting ? 'Сохранение...' : 'Сохранить'}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default DepartmentModal