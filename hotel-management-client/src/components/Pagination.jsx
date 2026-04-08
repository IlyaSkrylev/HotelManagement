import React from 'react'
import '../styles/Pagination.css'

function Pagination({ currentPage, totalPages, onPageChange }) {
    if (totalPages <= 1) return null

    const getPageNumbers = () => {
        const pages = []
        const maxVisible = 3
        let startPage = Math.max(1, currentPage - Math.floor(maxVisible / 2))
        let endPage = Math.min(totalPages, startPage + maxVisible - 1)

        if (endPage - startPage + 1 < maxVisible) {
            startPage = Math.max(1, endPage - maxVisible + 1)
        }

        for (let i = startPage; i <= endPage; i++) {
            pages.push(i)
        }
        return pages
    }

    const handlePageChange = (page) => {
        if (page !== currentPage && page >= 1 && page <= totalPages) {
            onPageChange(page)
        }
    }

    const hasPrevPages = currentPage > 1
    const hasNextPages = currentPage < totalPages

    return (
        <div className="pagination">
            {hasPrevPages && (
                <button
                    className="pagination-btn"
                    onClick={() => handlePageChange(1)}
                >
                    ⏮ Первая
                </button>
            )}

            {hasPrevPages && (
                <button
                    className="pagination-btn"
                    onClick={() => handlePageChange(currentPage - 1)}
                >
                    ◀ Назад
                </button>
            )}

            <div className="pagination-numbers">
                {getPageNumbers().map(page => (
                    <button
                        key={page}
                        className={`pagination-number ${page === currentPage ? 'active' : ''}`}
                        onClick={() => handlePageChange(page)}
                    >
                        {page}
                    </button>
                ))}
            </div>

            {hasNextPages && (
                <button
                    className="pagination-btn"
                    onClick={() => handlePageChange(currentPage + 1)}
                >
                    Вперед ▶
                </button>
            )}

            {hasNextPages && (
                <button
                    className="pagination-btn"
                    onClick={() => handlePageChange(totalPages)}
                >
                    Последняя ⏭
                </button>
            )}
        </div>
    )
}

export default Pagination