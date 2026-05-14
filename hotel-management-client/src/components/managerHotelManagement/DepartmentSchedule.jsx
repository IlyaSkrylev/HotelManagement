import React, { useState, useEffect } from 'react'
import { departmentApi } from '../../api/departmentApi'
import { getImageUrl, getIconUrl } from '../../index'
import '../../styles/DepartmentSchedule.css'

function DepartmentSchedule({ departmentId, departmentName }) {
    const [currentDate, setCurrentDate] = useState(new Date())
    const [schedule, setSchedule] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')
    const [totalEmployees, setTotalEmployees] = useState(0)
    const [hoveredEmployee, setHoveredEmployee] = useState(null)
    const [tooltipPosition, setTooltipPosition] = useState({ x: 0, y: 0 })

    const currentYear = currentDate.getFullYear()
    const currentMonth = currentDate.getMonth() + 1

    useEffect(() => {
        if (departmentId) {
            loadSchedule()
        }
    }, [departmentId, currentYear, currentMonth])

    const loadSchedule = async () => {
        setLoading(true)
        setError('')
        try {
            const response = await departmentApi.getSchedule(departmentId, currentYear, currentMonth)
            const data = response.data.data
            setSchedule(data.schedule || [])
            setTotalEmployees(data.totalEmployees || 0)
        } catch (error) {
            console.error('Error loading schedule:', error)
            setError('Ошибка при загрузке графика')
        } finally {
            setLoading(false)
        }
    }

    const getDaysInMonth = (year, month) => {
        return new Date(year, month, 0).getDate()
    }

    const getFirstDayOfMonth = (year, month) => {
        const day = new Date(year, month - 1, 1).getDay()
        return day === 0 ? 6 : day - 1
    }

    const getDayEmployees = (day) => {
        const targetDate = new Date(currentYear, currentMonth - 1, day)
        targetDate.setHours(0, 0, 0, 0)
        const daySchedule = schedule.find(s => {
            const scheduleDate = new Date(s.date)
            scheduleDate.setHours(0, 0, 0, 0)
            return scheduleDate.getTime() === targetDate.getTime()
        })
        return daySchedule?.employees || []
    }

    const formatTime = (timeString, shiftType) => {
        if (!timeString) return ''
        console.log('formatTime input:', { timeString, shiftType })
        if (shiftType === 'actual') {
            const date = new Date(timeString)
            if (!isNaN(date.getTime())) {
                let hours = date.getUTCHours()
                const minutes = date.getUTCMinutes()
                hours = (hours + 3) % 24
                return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`
            }

            const parts = timeString.split(':')
            if (parts.length >= 2) {
                const hours = parts[0].padStart(2, '0')
                const minutes = parts[1].substring(0, 2).padStart(2, '0')
                return `${hours}:${minutes}`
            }
            return timeString
        }

        if (timeString.includes(':')) {
            const parts = timeString.split(':')
            const hours = parts[0].padStart(2, '0')
            const minutes = parts[1].padStart(2, '0').substring(0, 2)
            return `${hours}:${minutes}`
        }

        return timeString
    }

    const handleMouseEnter = (e, employee) => {
        const rect = e.currentTarget.getBoundingClientRect()
        setTooltipPosition({
            x: rect.left + rect.width / 2,
            y: rect.top - 10
        })
        setHoveredEmployee(employee)
    }

    const handleMouseLeave = () => {
        setHoveredEmployee(null)
    }

    const navigateMonth = (direction) => {
        setCurrentDate(prevDate => {
            const newDate = new Date(prevDate)
            newDate.setMonth(prevDate.getMonth() + direction)
            return newDate
        })
    }

    const monthNames = [
        'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
        'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
    ]

    const weekDays = ['ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ', 'ВС']

    const daysInMonth = getDaysInMonth(currentYear, currentMonth)
    const startOffset = getFirstDayOfMonth(currentYear, currentMonth)
    const today = new Date()
    const todayDate = today.getDate()
    const todayMonth = today.getMonth() + 1
    const todayYear = today.getFullYear()

    const isCurrentDay = (day) => {
        return day === todayDate && currentMonth === todayMonth && currentYear === todayYear
    }

    const isWeekendDay = (day) => {
        const date = new Date(currentYear, currentMonth - 1, day)
        const dayOfWeek = date.getDay()
        return dayOfWeek === 6 || dayOfWeek === 0
    }

    if (loading) {
        return <div className="loading">Загрузка графика отдела...</div>
    }

    if (error) {
        return <div className="error-message">{error}</div>
    }

    return (
        <div className="department-schedule">
            <div className="schedule-header">
                <h2>График работы отдела: {departmentName}</h2>
                <div className="schedule-stats">
                    <span className="employees-count">{totalEmployees} сотрудников</span>
                </div>
            </div>

            <div className="calendar-navigation">
                <button className="nav-btn" onClick={() => navigateMonth(-1)}>‹</button>
                <h3>{monthNames[currentMonth - 1]} {currentYear}</h3>
                <button className="nav-btn" onClick={() => navigateMonth(1)}>›</button>
            </div>

            <div className="calendar">
                <div className="calendar-weekdays">
                    {weekDays.map(day => (
                        <div key={day} className="weekday">{day}</div>
                    ))}
                </div>
                <div className="calendar-grid">
                    {Array.from({ length: startOffset }).map((_, i) => (
                        <div key={`empty-${i}`} className="calendar-day empty"></div>
                    ))}
                    {Array.from({ length: daysInMonth }).map((_, i) => {
                        const day = i + 1
                        const employees = getDayEmployees(day)
                        const isCurrent = isCurrentDay(day)
                        const isWeekend = isWeekendDay(day)

                        return (
                            <div
                                key={day}
                                className={`calendar-day ${isCurrent ? 'current' : ''} ${isWeekend ? 'weekend' : ''} ${employees.length === 0 ? 'empty-day' : ''}`}
                            >
                                <div className="day-number">{day}</div>
                                <div className="employees-avatars">
                                    {employees.slice(0, 5).map(emp => (
                                        <div
                                            key={emp.employeeId}
                                            className="employee-avatar"
                                            onMouseEnter={(e) => handleMouseEnter(e, emp)}
                                            onMouseLeave={handleMouseLeave}
                                        >
                                            {emp.avatarUrl ? (
                                                <img
                                                    src={getImageUrl(emp.avatarUrl)}
                                                    alt={emp.fullName}
                                                />
                                            ) : (
                                                <div className="avatar-placeholder-small">
                                                    {emp.fullName.charAt(0)}
                                                </div>
                                            )}
                                        </div>
                                    ))}
                                    {employees.length > 5 && (
                                        <div className="more-employees">
                                            +{employees.length - 5}
                                        </div>
                                    )}
                                </div>
                                {employees.length === 0 && (
                                    <div className="no-employees">—</div>
                                )}
                            </div>
                        )
                    })}
                </div>
            </div>

            {hoveredEmployee && (
                <div
                    className="employee-tooltip"
                    style={{
                        left: tooltipPosition.x,
                        top: tooltipPosition.y,
                        transform: 'translateX(-50%) translateY(-100%)'
                    }}
                >
                    <div className="tooltip-avatar">
                        {hoveredEmployee.avatarUrl ? (
                            <img src={getImageUrl(hoveredEmployee.avatarUrl)} alt={hoveredEmployee.fullName} />
                        ) : (
                            <div className="tooltip-avatar-placeholder">
                                {hoveredEmployee.fullName.charAt(0)}
                            </div>
                        )}
                    </div>
                    <div className="tooltip-info">
                        <div className="tooltip-name">{hoveredEmployee.fullName}</div>
                        <div className="tooltip-position">{hoveredEmployee.position}</div>
                        <div className="tooltip-shift">
                            <span className="shift-label">Смена:</span>
                            {hoveredEmployee.startTime && hoveredEmployee.endTime ? (
                                <span>
                                    {formatTime(hoveredEmployee.startTime, hoveredEmployee.shiftType)} -
                                    {formatTime(hoveredEmployee.endTime, hoveredEmployee.shiftType)}
                                </span>
                            ) : (
                                <span>Выходной</span>
                            )}
                        </div>
                    </div>
                </div>
            )}
        </div>
    )
}

export default DepartmentSchedule