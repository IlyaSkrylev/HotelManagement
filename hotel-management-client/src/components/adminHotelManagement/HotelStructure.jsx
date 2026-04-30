import React, { useState, useEffect } from 'react'
import { hotelStructureApi } from '../../api/hotelStructureApi'
import { getIconUrl } from '../../index'
import Pagination from '../Pagination'
import FloorModal from './FloorModal'
import RoomModal from './RoomModal'
import '../../styles/HotelStructure.css'

function HotelStructure({ hotelId }) {
    const [floors, setFloors] = useState([])
    const [rooms, setRooms] = useState([])
    const [roomStatuses, setRoomStatuses] = useState([])
    const [loading, setLoading] = useState(true)
    const [expandedFloorId, setExpandedFloorId] = useState(null)

    const [floorsPage, setFloorsPage] = useState(1)
    const [floorsTotalPages, setFloorsTotalPages] = useState(1)
    const [floorsTotalCount, setFloorsTotalCount] = useState(0)

    const [floorModalOpen, setFloorModalOpen] = useState(false)
    const [roomModalOpen, setRoomModalOpen] = useState(false)
    const [editingFloor, setEditingFloor] = useState(null)
    const [editingRoom, setEditingRoom] = useState(null)
    const [selectedFloorForRoom, setSelectedFloorForRoom] = useState(null)

    const pageSize = 20
    const roomsPageSize = 50

    const editIconUrl = getIconUrl('edit')
    const deleteIconUrl = getIconUrl('bin')

    useEffect(() => {
        loadFloors()
        loadRoomStatuses()
    }, [hotelId, floorsPage])

    const loadFloors = async (silent = false) => {
        if (!silent) {
            setLoading(true)
        }
        try {
            const response = await hotelStructureApi.getFloors(hotelId, floorsPage, pageSize)
            const data = response.data.data
            setFloors(data.items || [])
            setFloorsTotalCount(data.totalCount || 0)
            setFloorsTotalPages(data.totalPages || 1)
        } catch (error) {
            console.error('Error loading floors:', error)
        } finally {
            if (!silent) {
                setLoading(false)
            }
        }
    }

    const loadRoomsForFloor = async (floorId) => {
        try {
            const response = await hotelStructureApi.getRooms(hotelId, floorId, 1, roomsPageSize)
            const data = response.data.data
            setRooms(prev => ({
                ...prev,
                [floorId]: data.items || []
            }))
        } catch (error) {
            console.error('Error loading rooms:', error)
        }
    }

    const loadRoomStatuses = async () => {
        try {
            const response = await hotelStructureApi.getRoomStatuses()
            const data = response.data.data
            setRoomStatuses(data || [])
        } catch (error) {
            console.error('Error loading room statuses:', error)
        }
    }

    const handleToggleFloor = (floorId) => {
        if (expandedFloorId === floorId) {
            setExpandedFloorId(null)
        } else {
            setExpandedFloorId(floorId)
            if (!rooms[floorId]) {
                loadRoomsForFloor(floorId)
            }
        }
    }

    const handleCreateFloor = async (formData) => {
        await hotelStructureApi.createFloor(hotelId, formData)
        setFloorsPage(1)
        await loadFloors()
    }

    const handleUpdateFloor = async (id, formData) => {
        await hotelStructureApi.updateFloor(id, formData)
        await loadFloors()
    }

    const handleDeleteFloor = async (id, name) => {
        if (window.confirm(`Удалить этаж "${name}"? Все номера на этом этаже также будут удалены.`)) {
            try {
                await hotelStructureApi.deleteFloor(id)
                if (floors.length === 1 && floorsPage > 1) {
                    setFloorsPage(floorsPage - 1)
                } else {
                    await loadFloors()
                }
            } catch (error) {
                alert(error.response?.data?.message || 'Ошибка при удалении этажа')
            }
        }
    }

    const handleCreateRoom = async (formData) => {
        await hotelStructureApi.createRoom(hotelId, formData)
        await loadFloors(true)

        const targetFloorId = formData.floorId
        if (expandedFloorId === targetFloorId) {
            await loadRoomsForFloor(targetFloorId)
        }
        setRoomModalOpen(false)
    }

    const handleUpdateRoom = async (id, formData) => {
        const previousFloorId = editingRoom?.floorId
        await hotelStructureApi.updateRoom(id, formData)
        await loadFloors(true)

        if (expandedFloorId && (expandedFloorId === previousFloorId || expandedFloorId === formData.floorId)) {
            await loadRoomsForFloor(expandedFloorId)
        }
        setRoomModalOpen(false)
    }

    const handleDeleteRoom = async (id, roomNumber, floorId) => {
        if (window.confirm(`Удалить номер "${roomNumber}"?`)) {
            try {
                await hotelStructureApi.deleteRoom(id)
                await loadFloors(true)
                await loadRoomsForFloor(floorId)
            } catch (error) {
                alert(error.response?.data?.message || 'Ошибка при удалении номера')
            }
        }
    }

    const getStatusColor = (color) => {
        return color || '#6c757d'
    }

    if (loading && floors.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="hotel-structure">
            <div className="structure-header">
                <h2>Структура гостиницы</h2>
                <button className="add-floor-btn" onClick={() => {
                    setEditingFloor(null)
                    setFloorModalOpen(true)
                }}>
                    <span className="btn-icon">+</span>
                    Добавить этаж
                </button>
            </div>

            <div className="floors-container">
                {floors.length === 0 ? (
                    <div className="no-data">
                        <p>Нет этажей</p>
                        <button className="create-first-btn" onClick={() => {
                            setEditingFloor(null)
                            setFloorModalOpen(true)
                        }}>
                            + Создать первый этаж
                        </button>
                    </div>
                ) : (
                    <>
                        {floors.map((floor) => (
                            <div key={floor.id} className={`floor-item ${expandedFloorId === floor.id ? 'expanded' : ''}`}>
                                <div
                                    className="floor-header"
                                    onClick={() => handleToggleFloor(floor.id)}
                                >
                                    <div className="floor-header-left">
                                        <span className="expand-icon">
                                            {expandedFloorId === floor.id ? '▼' : '▶'}
                                        </span>
                                        <div className="floor-level">
                                            <span className="floor-number-badge">Этаж {floor.floorNumber}</span>
                                            <span className="floor-name">{floor.name || `Этаж ${floor.floorNumber}`}</span>
                                        </div>
                                        <div className="floor-stats">
                                            <span className="rooms-count-badge">
                                                {floor.roomsCount} номеров
                                            </span>
                                        </div>
                                    </div>
                                    <div className="floor-header-right">
                                        {floor.description && (
                                            <span className="floor-description-preview">
                                                {floor.description.length > 30
                                                    ? floor.description.substring(0, 30) + '...'
                                                    : floor.description}
                                            </span>
                                        )}
                                        <button
                                            className="floor-edit-btn"
                                            onClick={(e) => {
                                                e.stopPropagation()
                                                setEditingFloor(floor)
                                                setFloorModalOpen(true)
                                            }}
                                        >
                                            <img src={editIconUrl} alt="edit" className="icon-small" />
                                        </button>
                                        <button
                                            className="floor-delete-btn"
                                            onClick={(e) => {
                                                e.stopPropagation()
                                                handleDeleteFloor(floor.id, floor.name || `Этаж ${floor.floorNumber}`)
                                            }}
                                        >
                                            <img src={deleteIconUrl} alt="delete" className="icon-small" />
                                        </button>
                                    </div>
                                </div>

                                {expandedFloorId === floor.id && (
                                    <div className="floor-rooms">
                                        <div className="rooms-header">
                                            <h4>Номера на этаже</h4>
                                            <button
                                                className="add-room-btn"
                                                onClick={() => {
                                                    setSelectedFloorForRoom(floor)
                                                    setEditingRoom(null)
                                                    setRoomModalOpen(true)
                                                }}
                                            >
                                                <span className="btn-icon-small">+</span>
                                                Добавить номер
                                            </button>
                                        </div>

                                        <div className="rooms-grid">
                                            {!rooms[floor.id] ? (
                                                <div className="rooms-loading">Загрузка номеров...</div>
                                            ) : rooms[floor.id]?.length === 0 ? (
                                                <div className="no-rooms">
                                                    <p>Нет номеров на этом этаже</p>
                                                    <button
                                                        className="create-room-link"
                                                        onClick={() => {
                                                            setSelectedFloorForRoom(floor)
                                                            setEditingRoom(null)
                                                            setRoomModalOpen(true)
                                                        }}
                                                    >
                                                        + Добавить первый номер
                                                    </button>
                                                </div>
                                            ) : (
                                                rooms[floor.id].map(room => (
                                                    <div key={room.id} className="room-card">
                                                        <div className="room-card-header">
                                                            <span className="room-number">
                                                                {room.roomNumber}
                                                            </span>
                                                            <div className="room-actions-small">
                                                                <button
                                                                    className="room-edit-btn"
                                                                    onClick={() => {
                                                                        setEditingRoom(room)
                                                                        setSelectedFloorForRoom(floor)
                                                                        setRoomModalOpen(true)
                                                                    }}
                                                                >
                                                                    <img src={editIconUrl} alt="edit" className="icon-tiny" />
                                                                </button>
                                                                <button
                                                                    className="room-delete-btn"
                                                                    onClick={() => handleDeleteRoom(room.id, room.roomNumber, floor.id)}
                                                                >
                                                                    <img src={deleteIconUrl} alt="delete" className="icon-tiny" />
                                                                </button>
                                                            </div>
                                                        </div>
                                                        <div className="room-card-body">
                                                            <div className="room-status">
                                                                <span
                                                                    className="room-status-dot"
                                                                    style={{ backgroundColor: getStatusColor(room.roomStatusColor) }}
                                                                />
                                                                <span className="room-status-text">
                                                                    {room.roomStatusName}
                                                                </span>
                                                            </div>
                                                            {room.description && (
                                                                <div className="room-description-small">
                                                                    {room.description}
                                                                </div>
                                                            )}
                                                        </div>
                                                    </div>
                                                ))
                                            )}
                                        </div>
                                    </div>
                                )}
                            </div>
                        ))}

                        {floorsTotalPages > 1 && (
                            <div className="floors-pagination">
                                <Pagination
                                    currentPage={floorsPage}
                                    totalPages={floorsTotalPages}
                                    onPageChange={setFloorsPage}
                                />
                            </div>
                        )}
                    </>
                )}
            </div>

            <FloorModal
                isOpen={floorModalOpen}
                onClose={() => setFloorModalOpen(false)}
                onSubmit={editingFloor ?
                    (data) => handleUpdateFloor(editingFloor.id, data) :
                    (data) => handleCreateFloor(data)}
                initialData={editingFloor}
            />

            <RoomModal
                isOpen={roomModalOpen}
                onClose={() => setRoomModalOpen(false)}
                onSubmit={editingRoom ?
                    (data) => handleUpdateRoom(editingRoom.id, data) :
                    (data) => handleCreateRoom(data)}
                initialData={editingRoom}
                floors={floors}
                roomStatuses={roomStatuses}
                selectedFloor={selectedFloorForRoom}
            />
        </div>
    )
}

export default HotelStructure