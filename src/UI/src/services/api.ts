import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'
import { getToken, refreshAccessToken } from '../utils/auth'
import router from '../router'

const api: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5214', // Adjust this to match your .NET backend URL
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
})

// Flag to prevent multiple simultaneous refresh attempts
let isRefreshing = false
let failedQueue: { resolve: (value: any) => void; reject: (reason?: any) => void }[] = []

const processQueue = (error: any) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error)
        } else {
            prom.resolve(true)
        }
    })
    failedQueue = []
}

// Add a request interceptor
api.interceptors.request.use(
    (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
        const token = getToken()
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`
        }
        return config
    },
    (error: AxiosError): Promise<AxiosError> => {
        return Promise.reject(error)
    }
)

// Add a response interceptor
api.interceptors.response.use(
    (response: AxiosResponse): AxiosResponse => response,
    async (error: AxiosError): Promise<any> => {
        const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

        // Check if the error is due to an unauthorized request
        if (error.response?.status === 401 && !originalRequest._retry) {
            if (isRefreshing) {
                // If a refresh is already in progress, queue this request
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject })
                })
            }

            originalRequest._retry = true
            isRefreshing = true

            try {
                // Attempt to refresh the token
                const refreshed = await refreshAccessToken()

                if (refreshed) {
                    // Update the authorization header with the new token
                    const newToken = getToken()
                    if (newToken && originalRequest.headers) {
                        originalRequest.headers.Authorization = `Bearer ${newToken}`
                    }

                    // Retry the original request
                    processQueue(null)
                    return api(originalRequest)
                } else {
                    // Refresh failed, redirect to login
                    processQueue(error)
                    router.push('/login')
                    return Promise.reject(error)
                }
            } catch (refreshError) {
                processQueue(refreshError)
                router.push('/login')
                return Promise.reject(refreshError)
            } finally {
                isRefreshing = false
            }
        }

        // Handle other error cases
        if (error.response) {
            switch (error.response.status) {
                case 400:
                    // Handle bad request
                    console.error('Bad Request:', error.response.data)
                    break
                default:
                    console.error('API Error:', error.response.data)
            }
        }

        return Promise.reject(error)
    }
)

export default api
