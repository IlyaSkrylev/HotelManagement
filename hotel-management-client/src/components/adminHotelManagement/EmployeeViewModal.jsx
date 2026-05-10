import React from 'react'
import { getImageUrl, getIconUrl } from '../../index'
import '../../styles/EmployeeViewModal.css'

function EmployeeViewModal({ isOpen, onClose, employee }) {
    if (!isOpen) return null

    const profileIconUrl = getIconUrl('profile')

    return (
        <div className="employee-view-modal-overlay" onClick={onClose}>
            <div className="employee-view-modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="employee-view-modal-header">
                    <h3>Информация о сотруднике</h3>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                <div className="employee-view-modal-body">
                    <div className="employee-view-avatar">
                        {employee.avatarUrl ? (
                            <img src={getImageUrl(employee.avatarUrl)} alt="avatar" />
                        ) : (
                            <img src={profileIconUrl} alt="avatar" />
                        )}
                    </div>

                    <div className="employee-view-info">
                        <h4>{employee.lastName} {employee.firstName} {employee.patronymic || ''}</h4>

                        <div className="info-grid">
                            <div className="info-label">Роль:</div>
                            <div className="info-value">{employee.roleCode === 'admin' ? 'Администратор' : employee.roleCode === 'manager' ? 'Менеджер' : 'Сотрудник'}</div>

                            <div className="info-label">Отдел:</div>
                            <div className="info-value">{employee.departmentName}</div>

                            <div className="info-label">Должность:</div>
                            <div className="info-value">{employee.position}</div>

                            <div className="info-label">Оклад:</div>
                            <div className="info-value">{employee.salary ? `${employee.salary} Br` : '—'}</div>

                            <div className="info-label">Надбавка:</div>
                            <div className="info-value">{employee.salarySupplement ? `${employee.salarySupplement} Br` : '—'}</div>

                            <div className="info-label">Дата найма:</div>
                            <div className="info-value">{new Date(employee.hireDate).toLocaleDateString('ru-RU')}</div>

                            {employee.dismissalDate && (
                                <>
                                    <div className="info-label">Дата увольнения:</div>
                                    <div className="info-value">{new Date(employee.dismissalDate).toLocaleDateString('ru-RU')}</div>
                                </>
                            )}

                            {employee.dismissalReason && (
                                <>
                                    <div className="info-label">Причина увольнения:</div>
                                    <div className="info-value">{employee.dismissalReason}</div>
                                </>
                            )}

                            <div className="info-label">График:</div>
                            <div className="info-value">
                                {employee.workingDayShifts > 0 && `${employee.workingDayShifts} дн. `}
                                {employee.workingNightShifts > 0 && `${employee.workingNightShifts} ноч. `}
                                {employee.restDays > 0 && `${employee.restDays} отд.`}
                            </div>
                        </div>
                    </div>
                </div>

                <div className="employee-view-modal-footer">
                    <button className="close-btn" onClick={onClose}>Закрыть</button>
                </div>
            </div>
        </div>
    )
}

export default EmployeeViewModal