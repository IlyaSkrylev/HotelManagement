import React from 'react'
import { AuthProvider } from './context/AuthContext'
import { ProjectProvider } from './context/ProjectContext'
import Navigation from './components/Navigation'
import Footer from './components/Footer'
import AppRoutes from './components/AppRoutes'
import useTokenRefresh from './hooks/useTokenRefresh'
import './styles/global.css'

function TokenMonitor() {
    useTokenRefresh()
    return null
}

function App() {
    return (
        <AuthProvider>
            <ProjectProvider>
                <TokenMonitor />
                <div className="app-wrapper">
                    <Navigation />
                    <main className="app-main">
                        <AppRoutes />
                    </main>
                    <Footer />
                </div>
            </ProjectProvider>
        </AuthProvider>
    )
}

export default App