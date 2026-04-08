import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import Home from '../pages/Home'
import Login from '../pages/Login'
import Register from '../pages/Register'
import Hotels from '../pages/Hotels'
import HotelCreate from '../pages/HotelCreate'


function PrivateRoute({ children }) {
    const { isAuthenticated } = useAuth()
    return isAuthenticated ? children : <Navigate to="/login" />
}

function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/hotels" element={<Hotels />} />
            <Route path="/register" element={<Register />} />
            <Route
                path="/hotels/create"
                element={
                    <PrivateRoute>
                        <HotelCreate />
                    </PrivateRoute>
                }
            />
        </Routes>
    )
}

export default AppRoutes