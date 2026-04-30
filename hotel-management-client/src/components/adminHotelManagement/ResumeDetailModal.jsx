import React, { useState, useEffect, useRef } from 'react'
import { resumeApi } from '../../api/resumeApi'
import { getImageUrl, getIconUrl } from '../../index'
import '../../styles/ResumeDetailModal.css'

function ResumeDetailModal({ resume, onClose, onStatusUpdate, statuses, hotelId }) {
    const [selectedStatusId, setSelectedStatusId] = useState(resume.statusId)
    const [updating, setUpdating] = useState(false)
    const [isDropdownOpen, setIsDropdownOpen] = useState(false)

    const dropdownRef = useRef(null)

    const profileIconUrl = getIconUrl('profile')
    const avatarUrl = resume.avatarUrl ? getImageUrl(resume.avatarUrl) : null

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsDropdownOpen(false)
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [])

    const handleStatusChange = async () => {
        if (selectedStatusId === resume.statusId) {
            onClose()
            return
        }

        setUpdating(true)
        try {
            await resumeApi.updateStatus(resume.id, selectedStatusId)
            await onStatusUpdate()
            onClose()
        } catch (error) {
            console.error('Error updating status:', error)
        } finally {
            setUpdating(false)
        }
    }

    const handleSelectStatus = (statusId) => {
        setSelectedStatusId(statusId)
        setIsDropdownOpen(false)
    }

    const getStatusColor = (statusId) => {
        const status = statuses.find(s => s.id === statusId)
        return status?.color || '#6c757d'
    }

    const getStatusName = (statusId) => {
        const status = statuses.find(s => s.id === statusId)
        return status?.name || 'Неизвестно'
    }

    const currentStatus = statuses.find(s => s.id === resume.statusId)
    const selectedStatus = statuses.find(s => s.id === selectedStatusId)

    return (
        <div className="resume-modal-overlay" onClick={onClose}>
            <div className="resume-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="resume-modal-header">
                    <h3>Детали резюме</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="resume-modal-body">
                    <div className="resume-modal-avatar">
                        {avatarUrl ? (
                            <img src={avatarUrl} alt="avatar" />
                        ) : (
                            <img src={profileIconUrl} alt="avatar" />
                        )}
                    </div>

                    <div className="resume-modal-info">
                        <h4>{resume.lastName} {resume.firstName} {resume.patronymic || ''}</h4>

                        <div className="info-wrapper">
                            <div className="info-grid">
                                <div className="info-label">Дата рождения:</div>
                                <div className="info-value">
                                    {resume.birthDate ? new Date(resume.birthDate).toLocaleDateString('ru-RU') : '—'}
                                </div>

                                <div className="info-label">Дата подачи:</div>
                                <div className="info-value">
                                    {new Date(resume.createdAt).toLocaleDateString('ru-RU')}
                                </div>

                                <div className="info-label">Желаемая должность:</div>
                                <div className="info-value">{resume.desiredPosition}</div>

                                <div className="info-label">Опыт работы:</div>
                                <div className="info-value">{resume.experience || '—'}</div>

                                <div className="info-label">Образование:</div>
                                <div className="info-value">{resume.education || '—'}</div>

                                <div className="info-label">Файл резюме:</div>
                                <div className="info-value">
                                    {resume.fileUrl ? (
                                        <a href={getImageUrl(resume.fileUrl)} target="_blank" rel="noopener noreferrer">Скачать</a>
                                    ) : '—'}
                                </div>

                                <div className="info-label">Текущий статус:</div>
                                <div className="info-value">
                                    <span className="status-badge" style={{ backgroundColor: getStatusColor(resume.statusId) }}>
                                        {currentStatus?.name || resume.statusName}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="resume-modal-status">
                        <label>Изменить статус:</label>

                        {/* Кастомный выпадающий список */}
                        <div
                            className={`modal-custom-select ${isDropdownOpen ? 'open' : ''}`}
                            ref={dropdownRef}
                        >
                            <div
                                className="modal-custom-select-trigger"
                                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                                style={{ borderColor: getStatusColor(selectedStatusId) }}
                            >
                                <span className="modal-current-status">
                                    <span
                                        className="modal-status-dot"
                                        style={{ backgroundColor: getStatusColor(selectedStatusId) }}
                                    ></span>
                                    {getStatusName(selectedStatusId)}
                                </span>
                                <span className="modal-arrow">{isDropdownOpen ? '▲' : '▼'}</span>
                            </div>

                            {isDropdownOpen && (
                                <div className="modal-status-dropdown">
                                    {statuses.map(status => (
                                        <div
                                            key={status.id}
                                            className={`modal-status-option ${selectedStatusId === status.id ? 'selected' : ''}`}
                                            onClick={() => handleSelectStatus(status.id)}
                                        >
                                            <span
                                                className="modal-option-dot"
                                                style={{ backgroundColor: status.color }}
                                            ></span>
                                            {status.name}
                                            {selectedStatusId === status.id && (
                                                <span className="modal-option-check">✓</span>
                                            )}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                <div className="resume-modal-footer">
                    <button className="cancel-btn" onClick={onClose}>Отмена</button>
                    <button
                        className="save-btn"
                        onClick={handleStatusChange}
                        disabled={updating}
                    >
                        {updating ? 'Сохранение...' : 'Сохранить'}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default ResumeDetailModal