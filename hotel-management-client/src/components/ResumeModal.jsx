import React, { useState, useEffect } from 'react'
import { profileApi } from '../api/profileApi'
import { resumeApi } from '../api/resumeApi'
import { getIconUrl } from '../index'
import '../styles/ResumeModal.css'

function ResumeModal({ hotelId, hotelName, onClose, onSuccess }) {
    const [formData, setFormData] = useState({
        desiredPosition: '',
        experience: '',
        education: '',
        resumeFile: null,
        useProfileResume: false
    })
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState('')
    const [hasProfileResume, setHasProfileResume] = useState(false)
    const [profileResumeName, setProfileResumeName] = useState('')

    const clipIconUrl = getIconUrl('clip')

    useEffect(() => {
        checkProfileResume()
    }, [])

    const checkProfileResume = async () => {
        try {
            const response = await profileApi.getProfile()
            const resumeUrl = response.data.data?.resumeUrl
            if (resumeUrl) {
                setHasProfileResume(true)
                const fileName = resumeUrl.split('/').pop()
                setProfileResumeName(fileName)
            } else {
                setHasProfileResume(false)
                setProfileResumeName('')
            }
        } catch (error) {
            console.error('Error checking profile resume:', error)
            setHasProfileResume(false)
        }
    }

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value })
    }

    const handleFileChange = (e) => {
        const file = e.target.files[0]
        if (file) {
            // Проверка формата - ТОЛЬКО PDF
            if (file.type !== 'application/pdf') {
                setError('Можно загружать только файлы в формате PDF')
                return
            }
        }
        setFormData({
            ...formData,
            resumeFile: file,
            useProfileResume: false
        })
        setError('')
    }

    const handleUseProfileResume = () => {
        setFormData({
            ...formData,
            useProfileResume: true,
            resumeFile: null
        })
        setError('')
    }

    const handleUploadNewResume = () => {
        setFormData({
            ...formData,
            useProfileResume: false
        })
        setError('')
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        if (!formData.desiredPosition) {
            setError('Укажите желаемую должность')
            setLoading(false)
            return
        }

        if (formData.useProfileResume && !hasProfileResume) {
            setError('В профиле нет загруженного резюме')
            setLoading(false)
            return
        }

        if (!formData.useProfileResume && !formData.resumeFile) {
            setError('Прикрепите файл резюме или используйте загруженное в профиле')
            setLoading(false)
            return
        }

        try {
            const submitData = {
                hotelId,
                desiredPosition: formData.desiredPosition,
                experience: formData.experience,
                education: formData.education,
                useProfileResume: formData.useProfileResume,
                resumeFile: formData.resumeFile
            }

            await resumeApi.submitResume(submitData)
            onSuccess()
            onClose()
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка подачи резюме')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h3>Подача резюме в отель <span className="hotel-name">{hotelName}</span></h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <form onSubmit={handleSubmit} className="modal-form">
                    <div className="form-group">
                        <label>Желаемая должность</label>
                        <input
                            type="text"
                            name="desiredPosition"
                            value={formData.desiredPosition}
                            onChange={handleChange}
                            placeholder="Например: Администратор"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label>Опыт работы</label>
                        <textarea
                            name="experience"
                            value={formData.experience}
                            onChange={handleChange}
                            placeholder="Опишите ваш опыт работы..."
                            rows="3"
                        />
                    </div>

                    <div className="form-group">
                        <label>Образование</label>
                        <textarea
                            name="education"
                            value={formData.education}
                            onChange={handleChange}
                            placeholder="Укажите ваше образование..."
                            rows="2"
                        />
                    </div>

                    <div className="form-group">
                        <label>Резюме</label>

                        <div className="resume-options-group">
                            {/* Вариант 1: Загрузить новое резюме */}
                            <div className="resume-option">
                                <label className="resume-radio">
                                    <input
                                        type="radio"
                                        name="resumeSource"
                                        checked={!formData.useProfileResume}
                                        onChange={handleUploadNewResume}
                                    />
                                    <span className="radio-label">Загрузить новое резюме</span>
                                </label>

                                {!formData.useProfileResume && (
                                    <div className="file-upload-wrapper">
                                        <label className="file-upload-btn">
                                            {clipIconUrl && <img src={clipIconUrl} alt="clip" className="clip-icon" />}
                                            <span>{formData.resumeFile ? formData.resumeFile.name : 'Выберите файл (PDF)'}</span>
                                            <input
                                                type="file"
                                                accept=".pdf"
                                                onChange={handleFileChange}
                                                hidden
                                            />
                                        </label>
                                    </div>
                                )}
                            </div>

                            {/* Вариант 2: Использовать резюме из профиля */}
                            <div className={`resume-option ${!hasProfileResume ? 'disabled' : ''}`}>
                                <label className={`resume-radio ${!hasProfileResume ? 'disabled' : ''}`}>
                                    <input
                                        type="radio"
                                        name="resumeSource"
                                        checked={formData.useProfileResume}
                                        onChange={handleUseProfileResume}
                                        disabled={!hasProfileResume}
                                    />
                                    <span className="radio-label">Использовать резюме из профиля</span>
                                </label>

                                <div className="profile-resume-info">
                                    {hasProfileResume ? (
                                        <span className="profile-resume-name">{profileResumeName}</span>
                                    ) : (
                                        <span className="profile-resume-name disabled">Резюме не загружено</span>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>

                    {error && <div className="error-message">{error}</div>}

                    <div className="modal-buttons">
                        <button type="button" onClick={onClose} className="cancel-btn">
                            Отмена
                        </button>
                        <button type="submit" disabled={loading} className="submit-btn">
                            {loading ? 'Отправка...' : 'Отправить'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default ResumeModal