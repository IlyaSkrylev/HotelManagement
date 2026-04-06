import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useProject } from '../context/ProjectContext'

function Hotels() {
    const { hotels, loading, error, loadHotels, deleteHotel } = useProject()

    useEffect(() => {
        loadHotels()
    }, [])

    const handleDelete = async (id) => {
        if (window.confirm('Удалить гостиницу?')) {
            try {
                await deleteHotel(id)
            } catch (err) {
                alert('Ошибка удаления')
            }
        }
    }

    if (loading) return <div>Загрузка...</div>
    if (error) return <div style={{ color: 'red' }}>{error}</div>

    return (
        <div>
            <h1>Гостиницы</h1>
            <Link to="/hotels/create">
                <button>Создать гостиницу</button>
            </Link>
            <table border="1" cellPadding="8">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Название</th>
                        <th>Адрес</th>
                        <th>Телефон</th>
                        <th>Email</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    {hotels.map((hotel) => (
                        <tr key={hotel.id}>
                            <td>{hotel.id}</td>
                            <td>{hotel.name}</td>
                            <td>{hotel.address}</td>
                            <td>{hotel.phone}</td>
                            <td>{hotel.email}</td>
                            <td>
                                <Link to={`/hotels/edit/${hotel.id}`}>Редактировать</Link>
                                {' | '}
                                <button onClick={() => handleDelete(hotel.id)}>Удалить</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default Hotels