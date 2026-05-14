import React, { useState, useEffect } from 'react'
import { employeeApi } from '../../api/employeeApi'
import '../../styles/EmployeeSchedule.css'

function EmployeeSchedule({ hotelId, currentEmployeeId }) {
    const [currentDate, setCurrentDate] = useState(new Date())
    const [schedule, setSchedule] = useState([])
    const [shiftType, setShiftType] = useState(null)
    const [loading, setLoading] = useState(true)
    const [openShift, setOpenShift] = useState(null)
    const [error, setError] = useState('')
    const [success, setSuccess] = useState('')
    const [vacationInfo, setVacationInfo] = useState(null)
    const [today, setToday] = useState(new Date())

    const currentYear = currentDate.getFullYear()
    const currentMonth = currentDate.getMonth() + 1
    const todayDate = today.getDate()
    const todayMonth = today.getMonth() + 1
    const todayYear = today.getFullYear()

    useEffect(() => {
        if (currentEmployeeId) {
            loadSchedule()
            checkOpenShift()
        }
    }, [currentEmployeeId, currentYear, currentMonth])

    const loadSchedule = async () => {
        setLoading(true)
        setError('')
        try {
            const response = await employeeApi.getSchedule(currentEmployeeId, currentYear, currentMonth)
            const data = response.data.data
            setSchedule(data.schedule || [])
            setShiftType(data.shiftType)
            setVacationInfo({
                startDate: data.vacationStartDate,
                endDate: data.vacationEndDate,
                type: data.vacationType
            })
        } catch (error) {
            console.error('Error loading schedule:', error)
            setError('Ошибка при загрузке графика')
        } finally {
            setLoading(false)
        }
    }

    const checkOpenShift = async () => {
        try {
            const response = await employeeApi.getOpenShift(currentEmployeeId)
            const data = response.data.data
            setOpenShift(data)
        } catch (error) {
            console.error('Error checking open shift:', error)
        }
    }

    const handleStartShift = async () => {
        setError('')
        setSuccess('')
        try {
            await employeeApi.startShift(currentEmployeeId)
            setSuccess('Смена начата')
            await loadSchedule()
            await checkOpenShift()
            setTimeout(() => setSuccess(''), 3000)
        } catch (error) {
            setError(error.response?.data?.message || 'Ошибка при начале смены')
            setTimeout(() => setError(''), 3000)
        }
    }

    const handleEndShift = async () => {
        setError('')
        setSuccess('')
        try {
            await employeeApi.endShift(currentEmployeeId)
            setSuccess('Смена завершена')
            await loadSchedule()
            await checkOpenShift()
            setTimeout(() => setSuccess(''), 3000)
        } catch (error) {
            setError(error.response?.data?.message || 'Ошибка при завершении смены')
            setTimeout(() => setError(''), 3000)
        }
    }

    const isWeekendDay = (date) => {
        const day = new Date(date).getDay()
        return day === 6 || day === 0 
    }

    const getDaysInMonth = (year, month) => {
        return new Date(year, month, 0).getDate()
    }

    const getFirstDayOfMonth = (year, month) => {
        const day = new Date(year, month - 1, 1).getDay()
        return day === 0 ? 6 : day - 1
    }

    const formatTime = (timeString, shiftType) => {
        if (!timeString) return ''

        let timeStr = String(timeString)

        if (shiftType === 'actual') {
            const date = new Date(timeStr)
            if (!isNaN(date.getTime())) {
                date.setHours(date.getHours() + 3)
                return date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
            }
            const parts = timeStr.split(':')
            if (parts.length >= 2) {
                let hours = parseInt(parts[0], 10)
                const minutes = parts[1].substring(0, 2)
                return `${hours.toString().padStart(2, '0')}:${minutes}`
            }
            return timeStr
        }

        if (timeStr.includes(':')) {
            const parts = timeStr.split(':')
            return `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0').substring(0, 2)}`
        }

        return timeStr
    }

    const getDaySchedule = (day) => {
        const targetDate = new Date(currentYear, currentMonth - 1, day)
        targetDate.setHours(0, 0, 0, 0)
        return schedule.find(s => {
            const scheduleDate = new Date(s.date)
            scheduleDate.setHours(0, 0, 0, 0)
            return scheduleDate.getTime() === targetDate.getTime()
        })
    }

    const getShiftClass = (daySchedule) => {
        if (daySchedule?.isVacation) return 'vacation'
        if (daySchedule?.shiftType === 'actual') return 'actual-shift'
        if (daySchedule?.shiftType === 'day') return 'day-shift'
        if (daySchedule?.shiftType === 'night') return 'night-shift'
        return 'rest-day'
    }

    const getShiftLabel = (daySchedule) => {
        if (daySchedule?.isVacation) {
            return daySchedule.vacationType === 'sick' ? 'Больничный' : 'Отпуск'
        }
        if (daySchedule?.shiftType === 'actual') {
            const start = daySchedule.startTime ? formatTime(daySchedule.startTime, 'actual') : ''
            const end = daySchedule.endTime ? formatTime(daySchedule.endTime, 'actual') : ''
            return end ? `${start} - ${end}` : `${start} (в работе)`
        }
        if (daySchedule?.shiftType === 'day') {
            const start = formatTime(daySchedule.startTime, 'day')
            const end = formatTime(daySchedule.endTime, 'day')
            return `${start} - ${end}`
        }
        if (daySchedule?.shiftType === 'night') {
            const start = formatTime(daySchedule.startTime, 'night')
            const end = formatTime(daySchedule.endTime, 'night')
            return `${start} - ${end}`
        }
        return ''
    }

    const isCurrentDay = (day) => {
        return day === todayDate && currentMonth === todayMonth && currentYear === todayYear
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

    if (loading) {
        return <div className="loading">Загрузка графика...</div>
    }

    return (
        <div className="employee-schedule">
            <div className="schedule-header">
                <h2>Мой график работы</h2>
                <div className="schedule-controls">
                    {!openShift ? (
                        <button className="schedule-start-btn" onClick={handleStartShift}>
                            Начать смену
                        </button>
                    ) : (
                        <button className="schedule-end-btn" onClick={handleEndShift}>
                            Закрыть смену
                        </button>
                    )}
                </div>
            </div>

            {error && <div className="schedule-error">{error}</div>}
            {success && <div className="schedule-success">{success}</div>}

            {vacationInfo?.startDate && (
                <div className="schedule-vacation-info">
                    <span className="schedule-vacation-badge">
                        {vacationInfo.type === 'sick' ? 'Больничный' : 'Отпуск'}
                        с {new Date(vacationInfo.startDate).toLocaleDateString('ru-RU')}
                        по {new Date(vacationInfo.endDate).toLocaleDateString('ru-RU')}
                    </span>
                </div>
            )}

            <div className="schedule-calendar-nav">
                <button className="schedule-nav-btn" onClick={() => navigateMonth(-1)}>
                    ‹
                </button>
                <h3>{monthNames[currentMonth - 1]} {currentYear}</h3>
                <button className="schedule-nav-btn" onClick={() => navigateMonth(1)}>
                    ›
                </button>
            </div>

            <div className="schedule-calendar">
                <div className="schedule-weekdays">
                    {weekDays.map(day => (
                        <div key={day} className="schedule-weekday">{day}</div>
                    ))}
                </div>
                <div className="schedule-grid">
                    {Array.from({ length: startOffset }).map((_, i) => (
                        <div key={`empty-${i}`} className="schedule-day empty"></div>
                    ))}
                    {Array.from({ length: daysInMonth }).map((_, i) => {
                        const day = i + 1
                        const daySchedule = getDaySchedule(day)
                        const shiftClass = getShiftClass(daySchedule)
                        const isCurrent = isCurrentDay(day)
                        const shiftLabel = getShiftLabel(daySchedule)
                        const dayDate = new Date(currentYear, currentMonth - 1, day)
                        const isWeekend = isWeekendDay(dayDate)

                        return (
                            <div
                                key={day}
                                className={`schedule-day ${shiftClass} ${isCurrent ? 'current' : ''} ${isWeekend ? 'weekend' : ''}`}
                            >
                                <div className="schedule-day-number">{day}</div>
                                {shiftLabel && <div className="schedule-day-info">{shiftLabel}</div>}
                            </div>
                        )
                    })}
                </div>
            </div>
        </div>
    )
}

export default EmployeeSchedule