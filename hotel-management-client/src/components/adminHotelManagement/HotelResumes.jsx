import React, { useState, useEffect, useRef } from 'react'
import { resumeApi } from '../../api/resumeApi'
import { getImageUrl, getIconUrl } from '../../index'
import ResumeDetailModal from './ResumeDetailModal'
import '../../styles/HotelResumes.css'

function HotelResumes({ hotelId }) {
    const [resumes, setResumes] = useState([])
    const [allResumes, setAllResumes] = useState([])
    const [statuses, setStatuses] = useState([])
    const [loading, setLoading] = useState(true)
    const [activeStatusId, setActiveStatusId] = useState(null)
    const [selectedResume, setSelectedResume] = useState(null)
    const [modalOpen, setModalOpen] = useState(false)
    const [openDropdownId, setOpenDropdownId] = useState(null)
    const [searchTerm, setSearchTerm] = useState('')
    const [searchInput, setSearchInput] = useState('')

    // Пагинация
    const [currentPage, setCurrentPage] = useState(1)
    const [totalPages, setTotalPages] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const pageSize = 20

    const dropdownRefs = useRef({})
    const profileIconUrl = getIconUrl('profile')

    // Debounce для поиска
    useEffect(() => {
        const timer = setTimeout(() => {
            setSearchTerm(searchInput)
            setCurrentPage(1)
        }, 500)

        return () => clearTimeout(timer)
    }, [searchInput])

    useEffect(() => {
        loadStatuses()
    }, [])

    useEffect(() => {
        if (statuses.length > 0 && !activeStatusId) {
            setActiveStatusId(statuses[0]?.id || null)
        }
    }, [statuses])

    useEffect(() => {
        if (activeStatusId !== null && hotelId) {
            loadResumes()
        }
    }, [hotelId, activeStatusId, currentPage, searchTerm])

    // Загружаем все резюме для подсчета статистики
    useEffect(() => {
        if (hotelId && statuses.length > 0) {
            loadAllResumesForStats()
        }
    }, [hotelId, statuses])

    const loadStatuses = async () => {
        try {
            const response = await resumeApi.getStatuses()
            const data = response.data.data || response.data || []
            setStatuses(data)
            if (data.length > 0) {
                setActiveStatusId(data[0].id)
            }
        } catch (error) {
            console.error('Error loading statuses:', error)
        }
    }

    const loadAllResumesForStats = async () => {
        try {
            const response = await resumeApi.getHotelResumes(hotelId, null, '', 1, 1000)
            const result = response.data.data || response.data
            setAllResumes(result.items || [])
        } catch (error) {
            console.error('Error loading all resumes for stats:', error)
        }
    }

    const loadResumes = async () => {
        setLoading(true)
        try {
            const response = await resumeApi.getHotelResumes(hotelId, activeStatusId, searchTerm, currentPage, pageSize)
            const result = response.data.data || response.data

            setResumes(result.items || [])
            setTotalCount(result.totalCount || 0)
            setTotalPages(result.totalPages || 1)
        } catch (error) {
            console.error('Error loading resumes:', error)
            setResumes([])
            setTotalCount(0)
            setTotalPages(1)
        } finally {
            setLoading(false)
        }
    }

    const handleStatusChange = async (resumeId, newStatusId) => {
        try {
            await resumeApi.updateStatus(resumeId, newStatusId)
            await loadResumes()
            await loadAllResumesForStats()
            setOpenDropdownId(null)
        } catch (error) {
            console.error('Error updating status:', error)
        }
    }

    const handlePageChange = (newPage) => {
        if (newPage >= 1 && newPage <= totalPages) {
            setCurrentPage(newPage)
        }
    }

    const getStatusColor = (statusId) => {
        const status = statuses.find(s => s.id === statusId)
        return status?.color || '#6c757d'
    }

    const getStatusName = (statusId) => {
        const status = statuses.find(s => s.id === statusId)
        return status?.name || 'Неизвестно'
    }

    const getStatusCount = (statusId) => {
        // Для поиска считаем из отфильтрованных allResumes
        if (searchTerm) {
            const searchLower = searchTerm.toLowerCase()
            return allResumes.filter(r =>
                r.statusId === statusId && (
                    r.firstName?.toLowerCase().includes(searchLower) ||
                    r.lastName?.toLowerCase().includes(searchLower) ||
                    (r.patronymic?.toLowerCase().includes(searchLower))
                )
            ).length
        }
        return allResumes.filter(r => r.statusId === statusId).length
    }

    const openResumeModal = (resume) => {
        setSelectedResume(resume)
        setModalOpen(true)
    }

    const closeModal = () => {
        setModalOpen(false)
        setSelectedResume(null)
    }

    if (loading && currentPage === 1 && resumes.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="hotel-resumes">
            <div className="resumes-header">
                <h2>Заявки от соискателей</h2>
            </div>

            {/* Поиск */}
            <div className="resumes-search">
                <input
                    type="text"
                    className="search-input"
                    placeholder="Поиск по ФИО..."
                    value={searchInput}
                    onChange={(e) => setSearchInput(e.target.value)}
                />
            </div>

            <div className="status-tabs">
                {statuses.map((status) => (
                    <button
                        key={status.id}
                        className={`status-tab ${activeStatusId === status.id ? 'active' : ''}`}
                        onClick={() => {
                            setActiveStatusId(status.id)
                            setCurrentPage(1)
                        }}
                    >
                        <span className="status-dot" style={{ backgroundColor: status.color }}></span>
                        {status.name}
                        <span className="status-count">{getStatusCount(status.id)}</span>
                    </button>
                ))}
            </div>

            <div className="resumes-list">
                {resumes.length === 0 ? (
                    <div className="no-resumes">
                        <p>Нет заявок в этой категории</p>
                    </div>
                ) : (
                    <>
                        {resumes.map((resume, index) => (
                            <div
                                key={resume.id}
                                className={`resume-card ${openDropdownId === resume.id ? 'dropdown-open' : ''}`}
                            >
                                <div className="resume-number">
                                    {(currentPage - 1) * pageSize + index + 1}
                                </div>

                                <div className="resume-avatar">
                                    {resume.avatarUrl ? (
                                        <img src={getImageUrl(resume.avatarUrl)} alt="avatar" />
                                    ) : (
                                        <img src={profileIconUrl} alt="avatar" />
                                    )}
                                </div>

                                <div className="resume-info">
                                    <div className="resume-name">
                                        {resume.lastName} {resume.firstName} {resume.patronymic || ''}
                                    </div>
                                    {resume.birthDate && (
                                        <div className="resume-birth">
                                            Дата рождения: {new Date(resume.birthDate).toLocaleDateString('ru-RU')}
                                        </div>
                                    )}
                                    <div className="resume-position">
                                        Желаемая должность: {resume.desiredPosition}
                                    </div>
                                </div>

                                <div className="resume-actions">
                                    <div
                                        className={`custom-select ${openDropdownId === resume.id ? 'open' : ''}`}
                                        ref={el => dropdownRefs.current[resume.id] = el}
                                    >
                                        <button
                                            className="custom-select-trigger"
                                            onClick={() => setOpenDropdownId(openDropdownId === resume.id ? null : resume.id)}
                                            style={{ borderColor: getStatusColor(resume.statusId) }}
                                        >
                                            <span className="current-status">
                                                <span
                                                    className="status-dot"
                                                    style={{ backgroundColor: getStatusColor(resume.statusId) }}
                                                ></span>
                                                {getStatusName(resume.statusId)}
                                            </span>
                                            <span className="arrow">{openDropdownId === resume.id ? '▲' : '▼'}</span>
                                        </button>

                                        {openDropdownId === resume.id && (
                                            <div className="custom-select-dropdown">
                                                {statuses.map(status => (
                                                    <div
                                                        key={status.id}
                                                        className={`dropdown-option ${resume.statusId === status.id ? 'active' : ''}`}
                                                        onClick={() => handleStatusChange(resume.id, status.id)}
                                                    >
                                                        <span
                                                            className="option-dot"
                                                            style={{ backgroundColor: status.color }}
                                                        ></span>
                                                        {status.name}
                                                        {resume.statusId === status.id && (
                                                            <span className="option-check">✓</span>
                                                        )}
                                                    </div>
                                                ))}
                                            </div>
                                        )}
                                    </div>

                                    <button
                                        className="view-btn"
                                        onClick={() => openResumeModal(resume)}
                                    >
                                        Подробнее
                                    </button>
                                </div>
                            </div>
                        ))}

                        {/* Пагинация */}
                        {totalPages > 1 && (
                            <div className="pagination">
                                <button
                                    className="pagination-btn"
                                    onClick={() => handlePageChange(currentPage - 1)}
                                    disabled={currentPage === 1}
                                >
                                    ← Назад
                                </button>
                                <span className="pagination-info">
                                    Страница {currentPage} из {totalPages} (всего: {totalCount})
                                </span>
                                <button
                                    className="pagination-btn"
                                    onClick={() => handlePageChange(currentPage + 1)}
                                    disabled={currentPage === totalPages}
                                >
                                    Вперед →
                                </button>
                            </div>
                        )}
                    </>
                )}
            </div>

            {modalOpen && selectedResume && (
                <ResumeDetailModal
                    resume={selectedResume}
                    onClose={closeModal}
                    onStatusUpdate={() => {
                        loadResumes()
                        loadAllResumesForStats()
                    }}
                    statuses={statuses}
                    hotelId={hotelId}
                />
            )}
        </div>
    )
}

export default HotelResumes