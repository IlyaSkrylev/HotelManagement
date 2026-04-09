import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { CLIENT_PORT, SERVER_URL, HOST_IP } from './src/config'


export default defineConfig({
    plugins: [react()],
    server: {
        port: CLIENT_PORT,
        host: HOST_IP,
        proxy: {
            '/api': {
                target: SERVER_URL,
                changeOrigin: true,
            },
            '/uploads': {
                target: SERVER_URL,
                changeOrigin: true,
            }
        }
    }
})