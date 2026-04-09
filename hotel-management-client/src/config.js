export const SERVER_IP = '192.168.0.143'  

export const HOST_IP = '0.0.0.0'

export const SERVER_PORT = '5030'
export const CLIENT_PORT = '5173'

export const SERVER_URL = `http://${SERVER_IP}:${SERVER_PORT}`
export const CLIENT_URL = `http://${SERVER_IP}:${CLIENT_PORT}`

export const API_URL = `${SERVER_URL}/api`

export const getImageUrl = (imagePath) => {
    if (!imagePath) return null
    return `${SERVER_URL}${imagePath}`
}