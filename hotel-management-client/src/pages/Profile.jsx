import React, { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { profileApi } from '../api/profileApi'
import { hotelApi } from '../api/hotelApi'
import { resumeApi } from '../api/resumeApi'
import { getImageUrl, getIconUrl } from '../index'
import MyHotelCard from '../components/MyHotelCard'
import MyResumesList from '../components/MyResumesList'
import '../styles/Profile.css'

function Profile() {
    const { user, updateProfile: updateAuthUser } = useAuth()
    const [loading, setLoading] = useState(false)
    const [uploading, setUploading] = useState(false)
    const [message, setMessage] = useState({ text: '', type: '' })
    const [activeTab, setActiveTab] = useState('profile')
    const [myHotels, setMyHotels] = useState([])
    const [myHotelsLoading, setMyHotelsLoading] = useState(false)
    const [profileForm, setProfileForm] = useState({
        firstName: '',
        lastName: '',
        patronymic: '',
        phone: '',
        birthDate: '',
        avatar: null
    })
    const [avatarPreview, setAvatarPreview] = useState(null)
    const [passwordForm, setPasswordForm] = useState({
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
    })
    const [resumeFile, setResumeFile] = useState(null)
    const [hasResume, setHasResume] = useState(false)
    const [resumeFileName, setResumeFileName] = useState('')
    const [resumeUrl, setResumeUrl] = useState(null)

    const [refreshKey, setRefreshKey] = useState(0)

    const clipIconUrl = getIconUrl('clip')

    useEffect(() => {
        loadProfile()
    }, [])

    useEffect(() => {
        if (activeTab === 'my-hotels') {
            loadMyHotels()
        }
    }, [activeTab])

    const refreshResumes = () => {
        setRefreshKey(prev => prev + 1)
    }

    const loadProfile = async () => {
        try {
            const response = await profileApi.getProfile()
            const data = response.data.data
            setProfileForm({
                firstName: data.firstName || '',
                lastName: data.lastName || '',
                patronymic: data.patronymic || '',
                phone: data.phone || '',
                birthDate: data.birthDate ? data.birthDate.split('T')[0] : '',
                avatar: null
            })
            setAvatarPreview(data.avatarUrl ? getImageUrl(data.avatarUrl) : null)

            const hasResumeFile = !!data.resumeUrl
            setHasResume(hasResumeFile)
            setResumeUrl(data.resumeUrl)

            if (hasResumeFile) {
                const fileName = data.resumeUrl.split('/').pop()
                setResumeFileName(fileName)
            } else {
                setResumeFileName('')
            }
        } catch (error) {
            console.error('Error loading profile:', error)
        }
    }

    const loadMyHotels = async () => {
        setMyHotelsLoading(true)
        try {
            const response = await hotelApi.getMyHotels()
            const hotelsData = response.data.data?.items || response.data.data || []
            setMyHotels(hotelsData)
        } catch (error) {
            console.error('Error loading my hotels:', error)
        } finally {
            setMyHotelsLoading(false)
        }
    }

    const handleAvatarChange = async (e) => {
        const file = e.target.files[0]
        if (!file) return

        if (!file.type.startsWith('image/')) {
            setMessage({ text: 'Можно загружать только изображения', type: 'error' })
            return
        }

        setUploading(true)
        setMessage({ text: '', type: '' })

        try {
            const updateData = {
                firstName: profileForm.firstName,
                lastName: profileForm.lastName,
                patronymic: profileForm.patronymic,
                phone: profileForm.phone,
                birthDate: profileForm.birthDate || null,
                avatar: file
            }

            const response = await profileApi.updateProfile(updateData)
            await updateAuthUser(response.data.data)
            setAvatarPreview(URL.createObjectURL(file))
            setMessage({ text: 'Аватар обновлён', type: 'success' })
            loadProfile()
        } catch (error) {
            console.error('Avatar upload error:', error.response?.data)
            setMessage({ text: error.response?.data?.message || 'Ошибка загрузки аватара', type: 'error' })
        } finally {
            setUploading(false)
        }
    }

    const handleProfileChange = (e) => {
        setProfileForm({ ...profileForm, [e.target.name]: e.target.value })
    }

    const handleProfileSubmit = async (e) => {
        e.preventDefault()
        setLoading(true)
        setMessage({ text: '', type: '' })

        try {
            const updateData = {
                firstName: profileForm.firstName,
                lastName: profileForm.lastName,
                patronymic: profileForm.patronymic,
                phone: profileForm.phone,
                birthDate: profileForm.birthDate || null
            }

            const response = await profileApi.updateProfile(updateData)
            await updateAuthUser(response.data.data)
            setMessage({ text: 'Профиль успешно обновлён', type: 'success' })
            loadProfile()
        } catch (error) {
            console.error('Update error:', error.response?.data)
            setMessage({ text: error.response?.data?.message || 'Ошибка обновления профиля', type: 'error' })
        } finally {
            setLoading(false)
        }
    }

    const handlePasswordChange = (e) => {
        setPasswordForm({ ...passwordForm, [e.target.name]: e.target.value })
    }

    const handlePasswordSubmit = async (e) => {
        e.preventDefault()
        setLoading(true)
        setMessage({ text: '', type: '' })

        if (passwordForm.newPassword !== passwordForm.confirmPassword) {
            setMessage({ text: 'Новый пароль и подтверждение не совпадают', type: 'error' })
            setLoading(false)
            return
        }

        try {
            await profileApi.changePassword(passwordForm.oldPassword, passwordForm.newPassword)
            setMessage({ text: 'Пароль успешно изменён', type: 'success' })
            setPasswordForm({ oldPassword: '', newPassword: '', confirmPassword: '' })
        } catch (error) {
            setMessage({ text: error.response?.data?.message || 'Ошибка изменения пароля', type: 'error' })
        } finally {
            setLoading(false)
        }
    }

    const handleResumeUpload = async () => {
        if (!resumeFile) return

        if (resumeFile.type !== 'application/pdf') {
            setMessage({ text: 'Можно загружать только файлы в формате PDF', type: 'error' })
            return
        }

        setUploading(true)
        setMessage({ text: '', type: '' })

        try {
            await profileApi.uploadResume(resumeFile)
            setMessage({ text: 'Резюме успешно загружено', type: 'success' })
            setResumeFile(null)
            await loadProfile()
        } catch (error) {
            setMessage({ text: error.response?.data?.message || 'Ошибка загрузки резюме', type: 'error' })
        } finally {
            setUploading(false)
        }
    }

    const handleDownloadResume = async () => {
        if (!resumeUrl) {
            setMessage({ text: 'Резюме не найдено', type: 'error' })
            return
        }

        try {
            const response = await profileApi.downloadResume()
            const url = window.URL.createObjectURL(new Blob([response.data]))
            const link = document.createElement('a')
            link.href = url
            link.setAttribute('download', resumeFileName || 'resume.pdf')
            document.body.appendChild(link)
            link.click()
            link.remove()
            window.URL.revokeObjectURL(url)
        } catch (error) {
            setMessage({ text: 'Ошибка скачивания резюме', type: 'error' })
        }
    }

    return (
        <div className="profile-container">
            <div className="profile-sidebar">
                <div className="profile-avatar">
                    {avatarPreview ? (
                        <img src={avatarPreview} alt="Avatar" className="avatar-img" />
                    ) : (
                        <div className="avatar-placeholder">
                            {profileForm.firstName?.charAt(0)}{profileForm.lastName?.charAt(0)}
                        </div>
                    )}
                    <label className="avatar-upload-btn">
                        {uploading ? 'Загрузка...' : 'Загрузить фото'}
                        <input type="file" accept="image/*" onChange={handleAvatarChange} disabled={uploading} hidden />
                    </label>
                </div>
                <div className="profile-user-info">
                    <h3>{profileForm.firstName} {profileForm.lastName}</h3>
                    <p>{user?.email}</p>
                </div>

                <button
                    className={`sidebar-tab ${activeTab === 'profile' ? 'active' : ''}`}
                    onClick={() => setActiveTab('profile')}
                >
                    Личные данные
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'my-hotels' ? 'active' : ''}`}
                    onClick={() => setActiveTab('my-hotels')}
                >
                    Мои гостиницы
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'password' ? 'active' : ''}`}
                    onClick={() => setActiveTab('password')}
                >
                    Изменить пароль
                </button>
                <button
                    className={`sidebar-tab ${activeTab === 'resume' ? 'active' : ''}`}
                    onClick={() => setActiveTab('resume')}
                >
                    Резюме
                </button>
            </div>

            <div className="profile-content">
                {message.text && (
                    <div className={`message ${message.type}`}>
                        {message.text}
                    </div>
                )}

                {activeTab === 'profile' && (
                    <form onSubmit={handleProfileSubmit} className="profile-form">
                        <h2>Личные данные</h2>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Имя</label>
                                <input
                                    type="text"
                                    name="firstName"
                                    value={profileForm.firstName}
                                    onChange={handleProfileChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label>Фамилия</label>
                                <input
                                    type="text"
                                    name="lastName"
                                    value={profileForm.lastName}
                                    onChange={handleProfileChange}
                                    required
                                />
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Отчество</label>
                                <input
                                    type="text"
                                    name="patronymic"
                                    value={profileForm.patronymic}
                                    onChange={handleProfileChange}
                                />
                            </div>
                            <div className="form-group">
                                <label>Телефон</label>
                                <input
                                    type="tel"
                                    name="phone"
                                    value={profileForm.phone}
                                    onChange={handleProfileChange}
                                />
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Дата рождения</label>
                                <input
                                    type="date"
                                    name="birthDate"
                                    value={profileForm.birthDate}
                                    onChange={handleProfileChange}
                                />
                            </div>
                            <div className="form-group">
                                <label>Email</label>
                                <input type="email" value={user?.email || ''} disabled />
                            </div>
                        </div>

                        <button type="submit" disabled={loading} className="submit-btn">
                            {loading ? 'Сохранение...' : 'Сохранить изменения'}
                        </button>
                    </form>
                )}

                {activeTab === 'my-hotels' && (
                    <div className="my-hotels-section">
                        <h2>Мои гостиницы</h2>
                        {myHotelsLoading ? (
                            <div className="loading">Загрузка...</div>
                        ) : myHotels.length === 0 ? (
                            <div className="no-hotels">
                                <p>Вы пока не работаете ни в одной гостинице</p>
                            </div>
                        ) : (
                            <div className="hotels-grid">
                                {myHotels.map((hotel) => (
                                    <MyHotelCard key={hotel.id} hotel={hotel} />
                                ))}
                            </div>
                        )}
                    </div>
                )}

                {activeTab === 'password' && (
                    <form onSubmit={handlePasswordSubmit} className="profile-form">
                        <h2>Изменение пароля</h2>

                        <div className="form-group">
                            <label>Старый пароль</label>
                            <input
                                type="password"
                                name="oldPassword"
                                value={passwordForm.oldPassword}
                                onChange={handlePasswordChange}
                                required
                            />
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label>Новый пароль</label>
                                <input
                                    type="password"
                                    name="newPassword"
                                    value={passwordForm.newPassword}
                                    onChange={handlePasswordChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label>Подтвердите пароль</label>
                                <input
                                    type="password"
                                    name="confirmPassword"
                                    value={passwordForm.confirmPassword}
                                    onChange={handlePasswordChange}
                                    required
                                />
                            </div>
                        </div>

                        <button type="submit" disabled={loading} className="submit-btn">
                            {loading ? 'Изменение...' : 'Изменить пароль'}
                        </button>
                    </form>
                )}

                {activeTab === 'resume' && (
                    <div className="profile-resume">
                        <h2>Моё резюме</h2>
                        <p className="resume-description">
                            Загрузите ваше резюме в формате PDF.
                        </p>

                        {hasResume && (
                            <div className="resume-actions">
                                <button onClick={handleDownloadResume} className="download-btn">
                                    Скачать текущее резюме
                                </button>
                                {resumeFileName && (
                                    <div className="current-resume-info">
                                        <p><strong>Файл:</strong> {resumeFileName}</p>
                                    </div>
                                )}
                            </div>
                        )}

                        <div className="resume-upload">
                            <p>Загрузить новое резюме</p>

                            <div className="file-upload-wrapper">
                                <label className="file-upload-label">
                                    {clipIconUrl && <img src={clipIconUrl} alt="attachment" className="clip-icon" />}
                                    <span>{resumeFile ? resumeFile.name : 'Выберите файл (PDF)'}</span>
                                    <input
                                        type="file"
                                        accept=".pdf"
                                        onChange={(e) => setResumeFile(e.target.files[0])}
                                        className="file-input-hidden"
                                    />
                                </label>
                            </div>

                            {resumeFile && (
                                <button
                                    onClick={handleResumeUpload}
                                    disabled={uploading}
                                    className="upload-btn"
                                >
                                    {uploading ? 'Загрузка...' : 'Загрузить резюме'}
                                </button>
                            )}
                        </div>

                        <div className="my-resumes-section">
                            <h3>История подачи резюме</h3>
                            <MyResumesList key={refreshKey} />
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}

export default Profile