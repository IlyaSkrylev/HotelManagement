import React from 'react'
import { BrowserRouter as Router } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { ProjectProvider } from './context/ProjectContext'
import Navigation from './components/Navigation'
import AppRoutes from './components/AppRoutes'
import './styles/global.css'

function App() {
    return (
        <AuthProvider>
            <ProjectProvider>
                <Router>
                    <div className="app">
                        <Navigation />
                        <main>
                            <AppRoutes />
                        </main>
                    </div>
                </Router>
            </ProjectProvider>
        </AuthProvider>
    )
}

export default App