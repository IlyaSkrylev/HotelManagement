// src/config/mediaConfig.js

import { SERVER_URL, getImageUrl } from './config'

const UPLOADS_PATH = '/uploads'
const MEDIA_CATEGORIES = {
    APP_PHOTOS: 'appsphotos',
    HOTEL_PHOTOS: 'hotels',
    USER_AVATARS: 'avatars',
    ICONS: 'icons'
}

export const mediaPaths = {
    homeBackground: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.APP_PHOTOS}/0036c769898cfdd25f5e9d02c2bab17c.jpg`,
    hotelPlaceholder: null,
    icons: {
        location: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/location.png`,
        phone: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/phone.png`,
        email: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/email.png`,
        search: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/search.png`,
        moon: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/moon.png`,
        sun: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/sun.png`,
        profile: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/profile.png`,
        hotel: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/hotel.png`,
        financial: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/finantialoperations.png`,
        timeManagement: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/timemanagement.png`,
        taskPlanning: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/taskplanning.png`,
        report: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/report.png`,
        photoGallery: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/photogalery.png`,
        clip: `${UPLOADS_PATH}/${MEDIA_CATEGORIES.ICONS}/clip.png`
    }
}

export const getIconUrl = (iconName) => {
    const iconMap = {
        location: mediaPaths.icons.location,
        phone: mediaPaths.icons.phone,
        email: mediaPaths.icons.email,
        search: mediaPaths.icons.search,
        moon: mediaPaths.icons.moon,
        sun: mediaPaths.icons.sun,
        profile: mediaPaths.icons.profile,
        hotel: mediaPaths.icons.hotel,
        financial: mediaPaths.icons.financial,
        timeManagement: mediaPaths.icons.timeManagement,
        taskPlanning: mediaPaths.icons.taskPlanning,
        report: mediaPaths.icons.report,
        photoGallery: mediaPaths.icons.photoGallery,
        clip: mediaPaths.icons.clip
    }

    const iconPath = iconMap[iconName]
    if (iconPath) {
        return getImageUrl(iconPath)
    }

    const emojiMap = {
        location: '📍',
        phone: '📞',
        email: '✉️',
        search: '🔍',
        moon: '🌙',
        sun: '☀️',
        profile: '👤',
        hotel: '🏨',
        financial: '💰',
        timeManagement: '⏰',
        taskPlanning: '📋',
        report: '📊',
        photoGallery: '🖼️'
    }
    return emojiMap[iconName] || '•'
}

export const getIcon = (iconName) => {
    const emojiMap = {
        location: '📍',
        phone: '📞',
        email: '✉️',
        search: '🔍',
        moon: '🌙',
        sun: '☀️',
        profile: '👤',
        hotel: '🏨',
        financial: '💰',
        timeManagement: '⏰',
        taskPlanning: '📋',
        report: '📊',
        photoGallery: '🖼️'
    }
    return emojiMap[iconName] || '•'
}

export const getHotelImageUrl = (imagePath) => {
    if (!imagePath) return null
    return getImageUrl(imagePath)
}

export const getMediaUrl = (path) => {
    if (!path) return null
    return getImageUrl(path)
}

const mediaConfig = {
    paths: mediaPaths,
    getIcon,
    getIconUrl,
    getHotelImageUrl,
    getMediaUrl
}

export default mediaConfig