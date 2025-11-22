import Cookies from 'js-cookie';
import api from '../services/api'

interface JwtPayload {
  exp: number;
  [key: string]: any;
}

export const getToken = (): string | null => localStorage.getItem('token')

export const setToken = (token: string | null): void => {
  if (token) {
    localStorage.setItem('token', token)
  } else {
    localStorage.removeItem('token')
  }
}

export const validateToken = async (): Promise<boolean> => {
  try {
    const token = getToken()
    if (!token) return false

    // Decode the token to check its expiration
    const tokenParts = token.split('.')
    if (tokenParts.length !== 3) return false // Not a valid JWT format

    // Decode the payload (middle part of the JWT)
    const payload = JSON.parse(atob(tokenParts[1])) as JwtPayload

    // Check if token has expired
    const expirationTime = payload.exp * 1000 // Convert to milliseconds
    if (Date.now() >= expirationTime) {
      // Token has expired, try to refresh
      return await refreshAccessToken()
    }

    return true
  } catch (error) {
    console.error('Token validation error:', error)
    // If there's an error validating the token, remove it
    setToken(null)
    return false
  }
}

export const refreshAccessToken = async (): Promise<boolean> => {
  try {

    // Call the backend refresh token endpoint
    const response = await api.post('/users/refresh-token',
      {  },
      { withCredentials: true }
    )

    if (response.data.token) {
      // Update tokens in local storage
      setToken(response.data.token)
      return true
    }

    // If refresh fails, logout the user
    await logout()
    return false
  } catch (error) {
    console.error('Token refresh error:', error)
    await logout()
    return false
  }
}

export const isAuthenticated = async (): Promise<boolean> => {
  return await validateToken()
}

export const logout = async (): Promise<boolean> => {
  try {
    // Call the logout endpoint if it exists
    const token = getToken()
    if (token) {
      try {
        // Attempt to call the server-side logout endpoint
        await api.post('/users/logout')
      } catch (error) {
        // If the endpoint doesn't exist or there's an error, just log it
        console.warn('Server-side logout failed:', error)
      }
    }

    // Always clear the tokens from localStorage
    setToken(null)
    Cookies.remove('refreshToken', { path: '/' });

    return true
  } catch (error) {
    console.error('Logout error:', error)
    // Even if there's an error, try to clear the tokens
    setToken(null)
    return false
  }
}
