import axios from 'axios'
import { getCookie } from '../../utils/cookieUtils';

const apiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
        'ApiKey': import.meta.env.VITE_API_KEY
    }
});

const authApiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
        'ApiKey': import.meta.env.VITE_API_KEY,
        'Authorization': `Bearer ${getCookie("token")}`
    }
});

export { authApiClient, apiClient };