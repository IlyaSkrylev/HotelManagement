import React from 'react'
import { AuthProvider } from './context/AuthContext'
import { ProjectProvider } from './context/ProjectContext'
import Navigation from './components/Navigation'
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
                <div className="app">
                    <Navigation />
                    <main>
                        <AppRoutes />
                    </main>
                </div>
            </ProjectProvider>
        </AuthProvider>
    )
}

export default App