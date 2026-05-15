import React, { useState, useEffect } from 'react'
import { financialApi } from '../../api/financialApi'
import Pagination from '../Pagination'
import '../../styles/DepartmentFinancialPage.css'

function DepartmentFinancialPage({ departmentId, departmentName, currentEmployeeId }) {
    const [operations, setOperations] = useState([])
    const [loading, setLoading] = useState(true)
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState('')
    const [success, setSuccess] = useState('')
    const [pagination, setPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    useEffect(() => {
        if (departmentId) {
            loadOperations(1)
        }
    }, [departmentId])

    const loadOperations = async (page) => {
        setLoading(true)
        try {
            console.log('=== Загрузка операций отдела ===')
            console.log('departmentId:', departmentId)
            console.log('page:', page)

            const response = await financialApi.getDepartmentOperations(departmentId, page, 20)
            console.log('Полный ответ:', response.data)

            const data = response.data.data
            console.log('data:', data)
            console.log('Items:', data?.Items)
            console.log('items:', data?.items)

            const items = data?.Items || data?.items || []
            const totalCount = data?.TotalCount || data?.totalCount || 0
            const currentPage = data?.Page || data?.page || page
            const pageSize = data?.PageSize || data?.pageSize || 20
            const totalPages = data?.TotalPages || data?.totalPages || Math.ceil(totalCount / pageSize)

            console.log('Извлеченные items:', items)
            console.log('totalCount:', totalCount)
            console.log('totalPages:', totalPages)

            setOperations(items)
            setPagination({
                currentPage: currentPage,
                pageSize: pageSize,
                totalCount: totalCount,
                totalPages: totalPages
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

    const handlePaySalary = async () => {
        setSubmitting(true)
        setError('')
        setSuccess('')
        try {
            const response = await financialApi.payDepartmentSalary(departmentId, currentEmployeeId, true)
            console.log('Ответ при выплате зарплаты:', response.data)
            setSuccess(`Зарплата успешно выплачена ${response.data.data} сотрудникам`)
            await loadOperations(1)
        } catch (error) {
            console.error('Error paying salary:', error)
            setError(error.response?.data?.message || 'Ошибка при выплате зарплаты')
        } finally {
            setSubmitting(false)
            setTimeout(() => setSuccess(''), 3000)
        }
    }

    const handlePayBonus = async () => {
        setSubmitting(true)
        setError('')
        setSuccess('')
        try {
            const response = await financialApi.payDepartmentSalary(departmentId, currentEmployeeId, false)
            console.log('Ответ при выплате премии:', response.data)
            setSuccess(`Премия успешно выплачена ${response.data.data} сотрудникам`)
            await loadOperations(1)
        } catch (error) {
            console.error('Error paying bonus:', error)
            setError(error.response?.data?.message || 'Ошибка при выплате премии')
        } finally {
            setSubmitting(false)
            setTimeout(() => setSuccess(''), 3000)
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

    return (
        <div className="department-financial-page">
            <div className="department-financial-header">
                <h2>Финансы отдела: {departmentName}</h2>
            </div>

            <div className="department-financial-actions">
                <button
                    className="pay-salary-btn"
                    onClick={handlePaySalary}
                    disabled={submitting}
                >
                    {submitting ? 'Обработка...' : 'Выдать зарплату всем сотрудникам'}
                </button>
                <button
                    className="pay-bonus-btn"
                    onClick={handlePayBonus}
                    disabled={submitting}
                >
                    {submitting ? 'Обработка...' : 'Выдать премию всем сотрудникам'}
                </button>
            </div>

            {error && <div className="department-financial-error">{error}</div>}
            {success && <div className="department-financial-success">{success}</div>}

            <div className="department-financial-operations">
                <h3>Последние финансовые операции отдела</h3>

                {loading ? (
                    <div className="loading">Загрузка...</div>
                ) : operations.length === 0 ? (
                    <div className="no-data">Нет операций</div>
                ) : (
                    <>
                        <div className="operations-table">
                                    <div className="operations-header">
                                        <div>Дата</div>
                                        <div>Сотрудник</div>
                                        <div>Сумма</div>
                                        <div>Описание</div>
                                    </div>
                                    {operations.map((op, idx) => (
                                        <div key={op.id || idx} className="operation-row">
                                            <div className="date" data-label="Дата">{formatDate(op.createdAt)}</div>
                                            <div className="employee" data-label="Сотрудник">{op.employeeName}</div>
                                            <div className="amount" data-label="Сумма">
                                                <span className={op.amount >= 0 ? 'positive' : 'negative'}>
                                                    {formatAmount(op.amount)}
                                                </span>
                                            </div>
                                            <div className="description" data-label="Описание">{op.description || '—'}</div>
                                        </div>
                                    ))}
                        </div>

                        <div className="pagination-info">
                            Показано {operations.length} из {pagination.totalCount} операций
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
    )
}

export default DepartmentFinancialPage