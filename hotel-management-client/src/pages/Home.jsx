import React from 'react'
import { useAuth } from '../context/AuthContext'

function Home() {
    const { isAuthenticated, user } = useAuth()

    return (
        <div>
            <h1>Добро пожаловать в Hotel Management</h1>
            {isAuthenticated ? (
                <p>Здравствуйте, {user?.firstName} {user?.lastName}!</p>
            ) : (
                <p>Пожалуйста, войдите в систему</p>
            )}
        </div>
    )
}

export default Home