import React, { useState, useEffect } from 'react'
import { getIconUrl } from '../../index'

function RoomModal({ isOpen, onClose, onSubmit, initialData, floors, roomStatuses, selectedFloor }) {
    const [formData, setFormData] = useState({
        floorId: '',
        roomNumber: '',
        roomStatusId: '',
        description: ''
    })
    const [error, setError] = useState('')
    const [submitting, setSubmitting] = useState(false)
    const [isFloorDropdownOpen, setIsFloorDropdownOpen] = useState(false)
    const [isStatusDropdownOpen, setIsStatusDropdownOpen] = useState(false)

    useEffect(() => {
        if (initialData) {
            setFormData({
                floorId: initialData.floorId || '',
                roomNumber: initialData.roomNumber || '',
                roomStatusId: initialData.roomStatusId || '',
                description: initialData.description || ''
            })
        } else if (selectedFloor) {
            setFormData({
                floorId: selectedFloor.id,
                roomNumber: '',
                roomStatusId: roomStatuses[0]?.id || '',
                description: ''
            })
        } else {
            setFormData({
                floorId: floors[0]?.id || '',
                roomNumber: '',
                roomStatusId: roomStatuses[0]?.id || '',
                description: ''
            })
        }
        setError('')
        setIsFloorDropdownOpen(false)
        setIsStatusDropdownOpen(false)
    }, [initialData, isOpen, floors, roomStatuses, selectedFloor])

    const selectedFloorData = floors.find(floor => floor.id === Number(formData.floorId))
    const selectedStatusData = roomStatuses.find(status => status.id === Number(formData.roomStatusId))

    const handleSubmit = async () => {
        setError('')

        if (!formData.roomNumber) {
            setError('Введите номер комнаты')
            return
        }
        if (!formData.floorId) {
            setError('Выберите этаж')
            return
        }
        if (!formData.roomStatusId) {
            setError('Выберите статус')
            return
        }

        setSubmitting(true)
        try {
            await onSubmit({
                floorId: parseInt(formData.floorId),
                roomNumber: formData.roomNumber.trim(),
                roomStatusId: parseInt(formData.roomStatusId),
                description: formData.description
            })
            onClose()
        } catch (error) {
            setError(error.response?.data?.message || 'Ошибка при сохранении')
        } finally {
            setSubmitting(false)
        }
    }

    if (!isOpen) return null

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h3>{initialData ? 'Редактирование номера' : 'Новый номер'}</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="modal-body">
                    {error && <div className="modal-error">{error}</div>}

                    <div className="form-group">
                        <label>Этаж</label>
                        <div className="filter-dropdown modal-filter-dropdown">
                            <button
                                type="button"
                                className="filter-trigger modal-filter-trigger"
                                onClick={() => {
                                    setIsFloorDropdownOpen(prev => !prev)
                                    setIsStatusDropdownOpen(false)
                                }}
                            >
                                <span>{selectedFloorData?.name || `Этаж ${selectedFloorData?.floorNumber}` || 'Выберите этаж'}</span>
                                <span className="filter-arrow">{isFloorDropdownOpen ? '▲' : '▼'}</span>
                            </button>
                            {isFloorDropdownOpen && (
                                <div className="filter-menu modal-filter-menu">
                                    {floors.map(floor => (
                                        <div
                                            key={floor.id}
                                            className={`filter-option ${Number(formData.floorId) === floor.id ? 'active' : ''}`}
                                            onClick={() => {
                                                setFormData({ ...formData, floorId: floor.id })
                                                setIsFloorDropdownOpen(false)
                                            }}
                                        >
                                            {floor.name || `Этаж ${floor.floorNumber}`}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                        <small className="form-hint">Выберите этаж, на котором находится номер</small>
                    </div>

                    <div className="form-group">
                        <label>Номер комнаты</label>
                        <input
                            type="text"
                            value={formData.roomNumber}
                            onChange={(e) => setFormData({ ...formData, roomNumber: e.target.value })}
                            placeholder="Например: 101, 202, A101"
                        />
                        <small className="form-hint">Уникальный номер комнаты в отеле</small>
                    </div>

                    <div className="form-group">
                        <label>Статус</label>
                        <div className="filter-dropdown modal-filter-dropdown">
                            <button
                                type="button"
                                className="filter-trigger modal-filter-trigger"
                                onClick={() => {
                                    setIsStatusDropdownOpen(prev => !prev)
                                    setIsFloorDropdownOpen(false)
                                }}
                            >
                                <span>{selectedStatusData?.name || 'Выберите статус'}</span>
                                <span className="filter-arrow">{isStatusDropdownOpen ? '▲' : '▼'}</span>
                            </button>
                            {isStatusDropdownOpen && (
                                <div className="filter-menu modal-filter-menu">
                                    {roomStatuses.map(status => (
                                        <div
                                            key={status.id}
                                            className={`filter-option ${Number(formData.roomStatusId) === status.id ? 'active' : ''}`}
                                            onClick={() => {
                                                setFormData({ ...formData, roomStatusId: status.id })
                                                setIsStatusDropdownOpen(false)
                                            }}
                                        >
                                            {status.name}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                        <small className="form-hint">Текущее состояние номера</small>
                    </div>

                    <div className="form-group">
                        <label>Описание</label>
                        <textarea
                            rows="4"
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            placeholder="Вид из окна, особенности номера, amenities..."
                        />
                        <small className="form-hint">Дополнительная информация о номере</small>
                    </div>
                </div>

                <div className="modal-footer">
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

export default RoomModal