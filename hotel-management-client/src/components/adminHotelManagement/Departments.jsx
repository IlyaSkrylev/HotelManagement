import React, { useState, useEffect } from 'react'
import { departmentApi } from '../../api/departmentApi'
import Pagination from '../Pagination'
import DepartmentModal from './DepartmentModal'
import '../../styles/Departments.css'

function Departments({ hotelId }) {
    const [departments, setDepartments] = useState([])
    const [loading, setLoading] = useState(true)
    const [searchTerm, setSearchTerm] = useState('')
    const [searchInput, setSearchInput] = useState('')
    const [currentPage, setCurrentPage] = useState(1)
    const [totalPages, setTotalPages] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [modalOpen, setModalOpen] = useState(false)
    const [editingDepartment, setEditingDepartment] = useState(null)

    const pageSize = 20

    // Debounce для поиска
    useEffect(() => {
        const timer = setTimeout(() => {
            setSearchTerm(searchInput)
            setCurrentPage(1)
        }, 500)
        return () => clearTimeout(timer)
    }, [searchInput])

    // Загрузка отделов при изменении параметров
    useEffect(() => {
        if (hotelId) {
            loadDepartments()
        }
    }, [hotelId, searchTerm, currentPage])

    const loadDepartments = async () => {
        setLoading(true)
        try {
            const response = await departmentApi.getDepartments(hotelId, searchTerm, currentPage, pageSize)
            const data = response.data.data
            setDepartments(data.items || [])
            setTotalCount(data.totalCount || 0)
            setTotalPages(data.totalPages || 1)
        } catch (error) {
            console.error('Error loading departments:', error)
        } finally {
            setLoading(false)
        }
    }

    const loadEmployeesForSelect = async (hotelId, search = '') => {
        try {
            const response = await departmentApi.getEmployeesForSelect(hotelId, search)
            return response.data.data || []
        } catch (error) {
            console.error('Error loading employees:', error)
            return []
        }
    }

    const openCreateModal = () => {
        setEditingDepartment(null)
        setModalOpen(true)
    }

    const openEditModal = (department) => {
        setEditingDepartment(department)
        setModalOpen(true)
    }

    const handleCreateDepartment = async (formData) => {
        await departmentApi.createDepartment(hotelId, {
            name: formData.name,
            description: formData.description,
            managerId: formData.managerId
        })
        // Сбрасываем на первую страницу после создания
        setCurrentPage(1)
        // Перезагружаем список
        await loadDepartments()
    }

    const handleUpdateDepartment = async (formData) => {
        await departmentApi.updateDepartment(editingDepartment.id, {
            name: formData.name,
            description: formData.description,
            managerId: formData.managerId
        })
        // Перезагружаем список на текущей странице
        await loadDepartments()
    }

    const handleSubmit = async (formData) => {
        if (editingDepartment) {
            await handleUpdateDepartment(formData)
        } else {
            await handleCreateDepartment(formData)
        }
    }

    const handleDelete = async (id, name) => {
        if (window.confirm(`Удалить отдел "${name}"?`)) {
            try {
                await departmentApi.deleteDepartment(id)
                // Проверяем, если после удаления на текущей странице не осталось элементов,
                // и это не первая страница, то переходим на предыдущую
                if (departments.length === 1 && currentPage > 1) {
                    setCurrentPage(currentPage - 1)
                } else {
                    await loadDepartments()
                }
            } catch (error) {
                console.error('Error deleting department:', error)
                alert('Ошибка при удалении отдела')
            }
        }
    }

    const handlePageChange = (page) => {
        setCurrentPage(page)
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    if (loading && currentPage === 1 && departments.length === 0) {
        return <div className="loading">Загрузка...</div>
    }

    return (
        <div className="departments-section">
            <div className="departments-header">
                <h2>Управление отделами</h2>
            </div>

            <div className="departments-toolbar">
                <div className="departments-search">
                    <input
                        type="text"
                        placeholder="Поиск по названию отдела или ФИО менеджера..."
                        value={searchInput}
                        onChange={(e) => setSearchInput(e.target.value)}
                        className="search-input"
                    />
                </div>
                <button className="add-department-btn" onClick={openCreateModal}>
                    + Добавить отдел
                </button>
            </div>

            <div className="departments-list">
                {departments.length === 0 ? (
                    <div className="no-departments">
                        <p>Нет отделов</p>
                    </div>
                ) : (
                    departments.map((dept, index) => (
                        <div key={dept.id} className="department-card">
                            <div className="department-number">
                                {(currentPage - 1) * pageSize + index + 1}
                            </div>

                            <div className="department-info">
                                <div className="department-name">{dept.name}</div>
                                {dept.description && (
                                    <div className="department-description">{dept.description}</div>
                                )}
                                <div className="department-manager">
                                    <span>Руководитель:</span>
                                    {dept.managerName ? (
                                        <>
                                            <span className="manager-name">{dept.managerName}</span>
                                            {dept.managerPosition && (
                                                <span className="manager-position">({dept.managerPosition})</span>
                                            )}
                                        </>
                                    ) : (
                                        <span className="manager-not-assigned">Не назначен</span>
                                    )}
                                </div>
                            </div>

                            <div className="department-stats">
                                <span className="employee-count">👥 {dept.employeeCount} сотрудников</span>
                            </div>

                            <div className="department-actions">
                                <button className="edit-dept-btn" onClick={() => openEditModal(dept)}>
                                    Изменить
                                </button>
                                <button className="delete-dept-btn" onClick={() => handleDelete(dept.id, dept.name)}>
                                    Удалить
                                </button>
                            </div>
                        </div>
                    ))
                )}
            </div>

            {totalPages > 1 && (
                <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={handlePageChange}
                />
            )}

            <DepartmentModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                onSubmit={handleSubmit}
                initialData={editingDepartment}
                hotelId={hotelId}
                loadEmployees={loadEmployeesForSelect}
            />
        </div>
    )
}

export default Departments