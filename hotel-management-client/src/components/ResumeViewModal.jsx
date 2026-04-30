import React from 'react'
import { getImageUrl } from '../index'
import '../styles/ResumeViewModal.css'

function ResumeViewModal({ resume, onClose }) {
    return (
        <div className="resume-view-modal-overlay" onClick={onClose}>
            <div className="resume-view-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="resume-view-modal-header">
                    <h3>Детали резюме</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="resume-view-modal-body">
                    <h4>{resume.hotelName}</h4>

                    <div className="info-wrapper">
                        <div className="info-grid">
                            <div className="info-label">Дата подачи:</div>
                            <div className="info-value">
                                {new Date(resume.createdAt).toLocaleDateString('ru-RU')}
                            </div>

                            <div className="info-label">Желаемая должность:</div>
                            <div className="info-value">{resume.desiredPosition}</div>

                            <div className="info-label">Опыт работы:</div>
                            <div className="info-value">{resume.experience || '—'}</div>

                            <div className="info-label">Образование:</div>
                            <div className="info-value">{resume.education || '—'}</div>

                            <div className="info-label">Файл резюме:</div>
                            <div className="info-value">
                                {resume.fileUrl ? (
                                    <a href={getImageUrl(resume.fileUrl)} target="_blank" rel="noopener noreferrer">Скачать</a>
                                ) : '—'}
                            </div>

                            <div className="info-label">Статус:</div>
                            <div className="info-value">
                                <span
                                    className="status-badge"
                                    style={{ backgroundColor: resume.statusColor || '#6c757d' }}
                                >
                                    {resume.statusName}
                                </span>
                            </div>

                            {resume.reviewedAt && (
                                <>
                                    <div className="info-label">Дата рассмотрения:</div>
                                    <div className="info-value">
                                        {new Date(resume.reviewedAt).toLocaleDateString('ru-RU')}
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>

                <div className="resume-view-modal-footer">
                    <button className="close-btn" onClick={onClose}>Закрыть</button>
                </div>
            </div>
        </div>
    )
}

export default ResumeViewModal