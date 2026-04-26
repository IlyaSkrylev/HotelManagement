import config from './config'
import mediaConfig from './mediaConfig'
import { SERVER_URL, API_URL, getImageUrl } from './config'
import { mediaPaths, getIcon, getIconUrl, getHotelImageUrl, getMediaUrl } from './mediaConfig'

const fullConfig = {
    ...config,
    media: mediaConfig
}

export default fullConfig

export {
    SERVER_URL,
    API_URL,
    getImageUrl,
    mediaPaths,
    getIcon,
    getIconUrl,
    getHotelImageUrl,
    getMediaUrl
}