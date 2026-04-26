export const SERVER_IP = '192.168.0.143'
export const HOST_IP = '0.0.0.0'
export const SERVER_PORT = '5030'
export const CLIENT_PORT = '5173'

export const SERVER_URL = `http://${SERVER_IP}:${SERVER_PORT}`
export const CLIENT_URL = `http://${SERVER_IP}:${CLIENT_PORT}`
export const API_URL = `${SERVER_URL}/api`

export const getImageUrl = (imagePath) => {
    if (!imagePath) return null
    if (imagePath.startsWith('http')) return imagePath
    return `${SERVER_URL}${imagePath}`
}

const config = {
    server: {
        ip: SERVER_IP,
        host: HOST_IP,
        port: SERVER_PORT,
        url: SERVER_URL
    },
    client: {
        port: CLIENT_PORT,
        url: CLIENT_URL
    },
    api: {
        url: API_URL
    },
    getImageUrl
}

export default config