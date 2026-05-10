import React, { useState } from 'react'
import '../../styles/FireEmployeeModal.css'

function FireEmployeeModal({ isOpen, onClose, onConfirm, employeeName }) {
    const [reason, setReason] = useState('')
    const [error, setError] = useState('')
    const [submitting, setSubmitting] = useState(false)

    const handleConfirm = async () => {
        setError('')

        if (!reason.trim()) {
            setError('Укажите причину увольнения')
            return
        }

        setSubmitting(true)
        try {
            await onConfirm(reason)
            onClose()
            setReason('')
        } catch (error) {
            setError(error.response?.data?.message || 'Ошибка при увольнении')
        } finally {
            setSubmitting(false)
        }
    }

    const handleClose = () => {
        setReason('')
        setError('')
        onClose()
    }

    if (!isOpen) return null

    return (
        <div className="fire-modal-overlay" onClick={handleClose}>
            <div className="fire-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="fire-modal-header">
                    <h3>Увольнение сотрудника</h3>
                    <button className="modal-close" onClick={handleClose}>×</button>
                </div>

                <div className="fire-modal-body">
                    <p className="fire-modal-warning">
                        Вы действительно хотите уволить сотрудника <strong>{employeeName}</strong>?
                    </p>

                    {error && (
                        <div className="fire-modal-error">
                            {error}
                        </div>
                    )}

                    <div className="form-group">
                        <label>Причина увольнения *</label>
                        <textarea
                            rows="3"
                            value={reason}
                            onChange={(e) => setReason(e.target.value)}
                            placeholder="Укажите причину увольнения..."
                            autoFocus
                        />
                    </div>
                </div>

                <div className="fire-modal-footer">
                    <button className="cancel-btn" onClick={handleClose}>
                        Отмена
                    </button>
                    <button className="confirm-btn" onClick={handleConfirm} disabled={submitting}>
                        {submitting ? 'Увольнение...' : 'Уволить'}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default FireEmployeeModal