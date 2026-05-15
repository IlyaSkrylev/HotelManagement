import React, { useState, useEffect } from 'react'
import { financialApi } from '../../api/financialApi'
import Pagination from '../Pagination'
import { getIconUrl } from '../../index'
import '../../styles/EmployeeFinancialPage.css'

function EmployeeFinancialPage({ employeeId, employeeName, employeeAvatar }) {
    const [operations, setOperations] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')
    const [pagination, setPagination] = useState({
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
    })

    const [startDate, setStartDate] = useState('')
    const [endDate, setEndDate] = useState('')
    const [stats, setStats] = useState({
        totalSalary: 0,
        totalBonus: 0,
        totalFine: 0,
        totalEarned: 0
    })

    const profileIconUrl = getIconUrl('profile')

    useEffect(() => {
        if (employeeId) {
            loadOperations(1)
            loadStats()
        }
    }, [employeeId])

    useEffect(() => {
        if (employeeId) {
            loadOperations(1)
            loadStats()
        }
    }, [startDate, endDate])

    const loadOperations = async (page) => {
        setLoading(true)
        try {
            const response = await financialApi.getEmployeeOperations(employeeId, page, 20, startDate, endDate)
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

    const loadStats = async () => {
        try {
            const response = await financialApi.getEmployeeStats(employeeId, startDate, endDate)
            const data = response.data.data
            setStats({
                totalSalary: data.totalSalary || 0,
                totalBonus: data.totalBonus || 0,
                totalFine: data.totalFine || 0,
                totalEarned: data.totalEarned || 0
            })
        } catch (error) {
            console.error('Error loading stats:', error)
        }
    }

    const handlePageChange = (page) => {
        loadOperations(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleResetFilters = () => {
        setStartDate('')
        setEndDate('')
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
        <div className="employee-financial-page">
            <div className="employee-financial-header">
                <h2>Мои финансы</h2>
            </div>

            <div className="employee-financial-info">
                <div className="employee-avatar">
                    {employeeAvatar ? (
                        <img src={employeeAvatar} alt={employeeName} />
                    ) : (
                        <div className="avatar-placeholder">
                            {employeeName?.charAt(0) || '?'}
                        </div>
                    )}
                </div>
                <h3 className="employee-name">{employeeName}</h3>
            </div>

            {/* Статистика */}
            <div className="financial-stats">
                <div className="stat-card salary">
                    <div className="stat-label">Зарплата + Премии</div>
                    <div className="stat-value">+{stats.totalSalary} Br</div>
                </div>
                <div className="stat-card fine">
                    <div className="stat-label">Штрафы</div>
                    <div className="stat-value">-{stats.totalFine} Br</div>
                </div>
                <div className="stat-card total">
                    <div className="stat-label">Итого к получению</div>
                    <div className="stat-value">{stats.totalEarned >= 0 ? '+' : ''}{stats.totalEarned} Br</div>
                </div>
            </div>

            <div className="financial-filters">
                <div className="filter-group">
                    <label>Дата начала</label>
                    <input
                        type="date"
                        value={startDate}
                        onChange={(e) => setStartDate(e.target.value)}
                        className="date-input"
                    />
                </div>
                <div className="filter-group">
                    <label>Дата окончания</label>
                    <input
                        type="date"
                        value={endDate}
                        onChange={(e) => setEndDate(e.target.value)}
                        className="date-input"
                    />
                </div>
                <button className="reset-filters-btn" onClick={handleResetFilters}>
                    Сбросить фильтры
                </button>
            </div>

            {error && <div className="financial-error">{error}</div>}

            <div className="financial-operations">
                <h3>История операций</h3>

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
                            {operations.map((op, idx) => (
                                <div key={op.id || idx} className="operation-row">
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

export default EmployeeFinancialPage