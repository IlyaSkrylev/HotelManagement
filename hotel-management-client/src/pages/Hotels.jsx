import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useProject } from '../context/ProjectContext'
import { useAuth } from '../context/AuthContext'
import HotelCard from '../components/HotelCard'
import Pagination from '../components/Pagination'
import '../styles/Hotels.css'

function Hotels() {
    const { hotels, loading, error, pagination, loadHotels } = useProject()
    const { isAuthenticated } = useAuth()

    useEffect(() => {
        loadHotels(1, 6)
    }, [])

    const handlePageChange = (page) => {
        loadHotels(page, pagination.pageSize)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    if (loading) return <div className="loading">Загрузка...</div>
    if (error) return <div className="error">{error}</div>

    return (
        <div className="hotels-container">
            <div className="hotels-header">
                <h1>Гостиницы</h1>
                {isAuthenticated && (
                    <Link to="/hotels/create" className="create-button">
                        + Создать гостиницу
                    </Link>
                )}
            </div>

            <div className="hotels-grid">
                {hotels.map((hotel) => (
                    <HotelCard key={hotel.id} hotel={hotel} />
                ))}
            </div>

            {hotels.length === 0 && (
                <div className="no-hotels">
                    <p>Нет добавленных гостиниц</p>
                    {isAuthenticated && (
                        <Link to="/hotels/create" className="create-button">
                            Создать первую гостиницу
                        </Link>
                    )}
                </div>
            )}

            <Pagination
                currentPage={pagination.currentPage}
                totalPages={pagination.totalPages}
                onPageChange={handlePageChange}
            />
        </div>
    )
}

export default Hotels