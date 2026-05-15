import React, { useState, useEffect } from 'react'
import { financialApi } from '../../api/financialApi'
import { getImageUrl, getIconUrl } from '../../index'
import Pagination from '../Pagination'
import '../../styles/FinancialOperationsModal.css'

function FinancialOperationsModal({ isOpen, onClose, employeeId, employeeName, employeeAvatar, currentSalary, currentBonus, currentEmployeeId }) {
    const [operations, setOperations] = useState([])
    const [loading, setLoading] = useState(true)
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState('')
    const [pagination, setPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    const [operationType, setOperationType] = useState('salary') 
    const [amount, setAmount] = useState('')
    const [description, setDescription] = useState('')
    const profileIconUrl = getIconUrl('profile')

    useEffect(() => {
        if (isOpen && employeeId) {
            loadOperations(1)

            if (currentSalary) {
                setAmount(currentSalary.toString())
            }
            if (operationType === 'bonus' && currentBonus) {
                setAmount(currentBonus.toString())
            }
        }
    }, [isOpen, employeeId, currentSalary, currentBonus])

    const loadOperations = async (page) => {
        setLoading(true)
        try {
            const response = await financialApi.getEmployeeOperations(employeeId, page, 20)
            const data = response.data.data
            setOperations(data.items || [])
            setPagination({
                currentPage: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages
            })
        } catch (error) {
            console.error('Error loading operations:', error)
            setError('Ошибка при загрузке операций')
        } finally {
            setLoading(false)
        }
    }

    const handlePageChange = (page) => {
        loadOperations(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleOperationTypeChange = (type) => {
        setOperationType(type)
        if (type === 'salary' && currentSalary) {
            setAmount(currentSalary.toString())
            setDescription('Выплата зарплаты')
        } else if (type === 'bonus' && currentBonus) {
            setAmount(currentBonus.toString())
            setDescription('Премия')
        } else if (type === 'fine') {
            setAmount('')
            setDescription('Штраф')
        }
    }

    const handleSubmit = async () => {
        setError('')

        const amountNum = parseInt(amount)
        if (isNaN(amountNum) || amountNum <= 0) {
            setError('Введите корректную сумму')
            return
        }

        let finalAmount = amountNum
        let finalDescription = description

        if (operationType === 'fine') {
            finalAmount = -Math.abs(amountNum)
            if (!finalDescription) finalDescription = 'Штраф'
        } else if (operationType === 'salary') {
            if (!finalDescription) finalDescription = 'Выплата зарплаты'
        } else if (operationType === 'bonus') {
            if (!finalDescription) finalDescription = 'Премия'
        }

        setSubmitting(true)
        try {
            await financialApi.createOperation({
                employeeId: employeeId,
                amount: finalAmount,
                description: finalDescription,
                createdById: currentEmployeeId
            })

            if (operationType === 'fine') {
                setAmount('')
                setDescription('Штраф')
            } else if (operationType === 'salary') {
                setDescription('Выплата зарплаты')
            } else if (operationType === 'bonus') {
                setDescription('Премия')
            }

            await loadOperations(1)
        } catch (error) {
            console.error('Error creating operation:', error)
            setError(error.response?.data?.message || 'Ошибка при создании операции')
        } finally {
            setSubmitting(false)
        }
    }

    const getAmountColor = (amount) => {
        return amount >= 0 ? 'positive' : 'negative'
    }

    const formatAmount = (amount) => {
        const absAmount = Math.abs(amount)
        if (amount >= 0) {
            return `+${absAmount} Br`
        }
        return `-${absAmount} Br`
    }

    const formatDate = (dateString) => {
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
    }

    if (!isOpen) return null

    return (
        <div className="financial-modal-overlay" onClick={onClose}>
            <div className="financial-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="financial-modal-header">
                    <h3>Финансовые операции</h3>
                    <h4>{employeeName}</h4>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="employee-info-center">
                    <div className="employee-avatar-large">
                        {employeeAvatar ? (
                            <img src={getImageUrl(employeeAvatar)} alt={employeeName} />
                        ) : (
                            <div className="avatar-placeholder-large">
                                {employeeName?.charAt(0) || '?'}
                            </div>
                        )}
                    </div>
                    <h4 className="employee-name-center">{employeeName}</h4>
                    <p className="employee-position-center">Сотрудник</p>
                </div>

                <div className="financial-modal-body">
                    {error && <div className="financial-error">{error}</div>}

                    <div className="financial-operation-form">
                        <h4>Новая операция</h4>
                        <div className="operation-type-buttons">
                            <button
                                className={`operation-type-btn ${operationType === 'salary' ? 'active' : ''}`}
                                onClick={() => handleOperationTypeChange('salary')}
                            >
                                Выдать зарплату
                            </button>
                            <button
                                className={`operation-type-btn ${operationType === 'bonus' ? 'active' : ''}`}
                                onClick={() => handleOperationTypeChange('bonus')}
                            >
                                Выдать премию
                            </button>
                            <button
                                className={`operation-type-btn ${operationType === 'fine' ? 'active' : ''}`}
                                onClick={() => handleOperationTypeChange('fine')}
                            >
                                Начислить штраф
                            </button>
                        </div>

                        <div className="form-group">
                            <label>Сумма (Br)</label>
                            <input
                                type="number"
                                value={amount}
                                onChange={(e) => setAmount(e.target.value)}
                                placeholder="Введите сумму"
                                min="1"
                            />
                            {operationType === 'salary' && currentSalary && (
                                <small>Оклад: {currentSalary} Br</small>
                            )}
                            {operationType === 'bonus' && currentBonus && (
                                <small>Надбавка: {currentBonus} Br</small>
                            )}
                        </div>

                        <div className="form-group">
                            <label>Описание</label>
                            <input
                                type="text"
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                                placeholder="Описание операции"
                            />
                        </div>

                        <button
                            className="submit-operation-btn"
                            onClick={handleSubmit}
                            disabled={submitting}
                        >
                            {submitting ? 'Сохранение...' : 'Сохранить операцию'}
                        </button>
                    </div>

                    <div className="financial-operations-list">
                        <h4>Последние операции</h4>
                        {loading ? (
                            <div className="loading">Загрузка...</div>
                        ) : operations.length === 0 ? (
                            <div className="no-data">Нет операций</div>
                        ) : (
                            <>
                                <div className="operations-table">
                                    <div className="operations-header">
                                        <div>Дата</div>
                                        <div>Сумма</div>
                                        <div>Описание</div>
                                    </div>
                                            {operations.map(op => (
                                                <div key={op.id} className="operation-row">
                                                    <div className="date" data-label="Дата">{formatDate(op.createdAt)}</div>
                                                    <div className="amount" data-label="Сумма">
                                                        <span className={op.amount >= 0 ? 'positive' : 'negative'}>
                                                            {formatAmount(op.amount)}
                                                        </span>
                                                    </div>
                                                    <div className="description" data-label="Описание">{op.description || '—'}</div>
                                                </div>
                                            ))}
                                </div>
                                {pagination.totalPages > 1 && (
                                    <Pagination
                                        currentPage={pagination.currentPage}
                                        totalPages={pagination.totalPages}
                                        onPageChange={handlePageChange}
                                    />
                                )}
                            </>
                        )}
                    </div>
                </div>

                <div className="financial-modal-footer">
                    <button className="close-btn" onClick={onClose}>Закрыть</button>
                </div>
            </div>
        </div>
    )
}

export default FinancialOperationsModal