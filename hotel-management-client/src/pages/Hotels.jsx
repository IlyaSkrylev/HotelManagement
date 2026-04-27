import React, { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useProject } from '../context/ProjectContext'
import { useAuth } from '../context/AuthContext'
import { getIconUrl } from '../index'
import HotelCard from '../components/HotelCard'
import Pagination from '../components/Pagination'
import ResumeModal from '../components/ResumeModal'
import '../styles/Hotels.css'

function Hotels() {
    const { hotels, loading, error, pagination, loadHotels } = useProject()
    const { isAuthenticated } = useAuth()
    const navigate = useNavigate()
    const [searchTerm, setSearchTerm] = useState('')
    const [filteredHotels, setFilteredHotels] = useState([])
    const searchIconUrl = getIconUrl('search')
    const [showResumeModal, setShowResumeModal] = useState(false)
    const [selectedHotel, setSelectedHotel] = useState(null)

    useEffect(() => {
        loadHotels(1, 100)
    }, [])

    useEffect(() => {
        if (searchTerm.trim() === '') {
            setFilteredHotels(hotels)
        } else {
            const filtered = hotels.filter(hotel =>
                hotel.name.toLowerCase().includes(searchTerm.toLowerCase())
            )
            setFilteredHotels(filtered)
        }
    }, [searchTerm, hotels])

    const handlePageChange = (page) => {
        loadHotels(page, pagination.pageSize)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const handleApplyResume = (hotelId, hotelName) => {
        if (!isAuthenticated) {
            navigate('/login')
            return
        }
        setSelectedHotel({ id: hotelId, name: hotelName })
        setShowResumeModal(true)
    }

    const handleResumeSuccess = () => {
        alert('Резюме успешно подано')
    }

    if (loading) return <div className="loading">Загрузка...</div>
    if (error) return <div className="error">{error}</div>

    return (
        <div className="hotels-container">
            <div className="hotels-header">
                <h1>Гостиницы</h1>
                <div className="header-right">
                    <div className="search-wrapper">
                        <input
                            type="text"
                            className="search-input"
                            placeholder="Поиск по названию..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                        
                        <img src={searchIconUrl} className="search-icon" />
                    </div>
                    {isAuthenticated && (
                        <Link to="/hotels/create" className="create-button">
                            + Создать гостиницу
                        </Link>
                    )}
                </div>
            </div>

            <div className="hotels-grid">
                {filteredHotels.map((hotel) => (
                    <HotelCard
                        key={hotel.id}
                        hotel={hotel}
                        onApplyResume={handleApplyResume}
                        showActions={false}
                    />
                ))}
            </div>

            {filteredHotels.length === 0 && (
                <div className="no-hotels">
                    <p>Гостиницы не найдены</p>
                </div>
            )}

            {filteredHotels.length > 0 && searchTerm.trim() === '' && (
                <Pagination
                    currentPage={pagination.currentPage}
                    totalPages={pagination.totalPages}
                    onPageChange={handlePageChange}
                />
            )}

            {showResumeModal && selectedHotel && (
                <ResumeModal
                    hotelId={selectedHotel.id}
                    hotelName={selectedHotel.name}
                    onClose={() => setShowResumeModal(false)}
                    onSuccess={handleResumeSuccess}
                />
            )}
        </div>
    )
}

export default Hotels