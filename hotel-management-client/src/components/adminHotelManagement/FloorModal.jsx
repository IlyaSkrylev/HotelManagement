import React, { useState, useEffect } from 'react'

function FloorModal({ isOpen, onClose, onSubmit, initialData }) {
    const [formData, setFormData] = useState({
        floorNumber: '',
        name: '',
        description: ''
    })
    const [error, setError] = useState('')
    const [submitting, setSubmitting] = useState(false)

    useEffect(() => {
        if (initialData) {
            setFormData({
                floorNumber: initialData.floorNumber || '',
                name: initialData.name || '',
                description: initialData.description || ''
            })
        } else {
            setFormData({
                floorNumber: '',
                name: '',
                description: ''
            })
        }
        setError('')
    }, [initialData, isOpen])

    const handleSubmit = async () => {
        setError('')

        if (!formData.floorNumber) {
            setError('Введите номер этажа')
            return
        }

        if (formData.floorNumber < 1) {
            setError('Номер этажа должен быть больше 0')
            return
        }

        setSubmitting(true)
        try {
            await onSubmit({
                floorNumber: parseInt(formData.floorNumber),
                name: formData.name,
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
                    <h3>{initialData ? 'Редактирование этажа' : 'Новый этаж'}</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="modal-body">
                    {error && <div className="modal-error">{error}</div>}

                    <div className="form-group">
                        <label>Номер этажа</label>
                        <input
                            type="number"
                            value={formData.floorNumber}
                            onChange={(e) => setFormData({ ...formData, floorNumber: e.target.value })}
                            placeholder="Введите номер этажа"
                            min="1"
                        />
                        <small className="form-hint">Например: 1, 2, 3 или -1 для цокольного</small>
                    </div>

                    <div className="form-group">
                        <label>Название этажа</label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            placeholder="Например: Первый этаж, Цокольный этаж"
                        />
                        <small className="form-hint">Необязательное поле</small>
                    </div>

                    <div className="form-group">
                        <label>Описание</label>
                        <textarea
                            rows="4"
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            placeholder="Введите описание этажа..."
                        />
                        <small className="form-hint">Дополнительная информация об этаже</small>
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

export default FloorModal