import React, { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function Register() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [repPassword, setRepPassword] = useState('')
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)
    const { register } = useAuth()
    const navigate = useNavigate()


    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')
        setLoading(true)
        try {
            if (password !== repPassword) {
                setError('Повторённый пароль не совпадает!')
            } else {
                await register()
                navigate('/');
            }
        } catch (err) {
            setError(err.response?.data?.message || 'Ошибка входа')
        } finally {
            setLoading(false)
        }
    } 

    return (
        <div>
            <h1>Регистрация</h1>
            <div>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label>Email</label>
                        <input
                            type = "email"
                            value = { email }
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label>Пароль</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label>Повторите пароль</label>
                        <input
                            type="password"
                            value={repPassword}
                            onChange={(e) => setRepPassword(e.target.value)}
                            required
                        />
                    </div>
                    {error && <div style={{ color: 'red' }}>{error}</div>}
                    <button type="submit" disabled={loading}>
                        {loading ? 'Вход...' : 'Войти'}
                    </button>
                </form>
            </div>
        </div>
    )
}

export default Register