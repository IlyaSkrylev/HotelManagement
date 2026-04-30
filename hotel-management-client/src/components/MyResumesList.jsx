import React, { useState, useEffect } from 'react'
import { resumeApi } from '../api/resumeApi'
import ResumeViewModal from './ResumeViewModal'
import '../styles/MyResumesList.css'

function MyResumesList() {
    const [resumes, setResumes] = useState([])
    const [loading, setLoading] = useState(true)
    const [selectedResume, setSelectedResume] = useState(null)
    const [modalOpen, setModalOpen] = useState(false)

    useEffect(() => {
        loadMyResumes()
    }, [])

    const loadMyResumes = async () => {
        setLoading(true)
        try {
            const response = await resumeApi.getMyResumes()
            setResumes(response.data.data || [])
        } catch (error) {
            console.error('Error loading my resumes:', error)
        } finally {
            setLoading(false)
        }
    }

    const openResumeModal = (resume) => {
        setSelectedResume(resume)
        setModalOpen(true)
    }

    const closeModal = () => {
        setModalOpen(false)
        setSelectedResume(null)
    }

    if (loading) {
        return <div className="my-resumes-loading">Загрузка...</div>
    }

    if (resumes.length === 0) {
        return (
            <div className="my-resumes-empty">
                <p>Вы ещё не подавали резюме ни в одну гостиницу</p>
            </div>
        )
    }

    return (
        <div className="my-resumes-container">
            <div className="my-resumes-list">
                {resumes.map((resume, index) => (
                    <div key={resume.id} className="my-resume-card">
                        <div className="my-resume-number">{index + 1}</div>

                        <div className="my-resume-info">
                            <div className="my-resume-hotel">
                                {resume.hotelName}
                            </div>
                            <div className="my-resume-position">
                                Должность: {resume.desiredPosition}
                            </div>
                            <div className="my-resume-date">
                                Дата подачи: {new Date(resume.createdAt).toLocaleDateString('ru-RU')}
                            </div>
                        </div>

                        <div className="my-resume-status">
                            <span
                                className="status-badge"
                                style={{ backgroundColor: resume.statusColor || '#6c757d' }}
                            >
                                {resume.statusName}
                            </span>
                        </div>

                        <div className="my-resume-actions">
                            <button
                                className="view-resume-btn"
                                onClick={() => openResumeModal(resume)}
                            >
                                Подробнее
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {modalOpen && selectedResume && (
                <ResumeViewModal
                    resume={selectedResume}
                    onClose={closeModal}
                />
            )}
        </div>
    )
}

export default MyResumesList