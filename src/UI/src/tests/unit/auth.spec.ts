import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { getToken, setToken, validateToken, isAuthenticated } from '@/utils/auth'

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {}
  return {
    getItem: vi.fn((key: string) => store[key] || null),
    setItem: vi.fn((key: string, value: string) => {
      store[key] = value
    }),
    removeItem: vi.fn((key: string) => {
      delete store[key]
    }),
    clear: vi.fn(() => {
      store = {}
    })
  }
})()

// Mock global.atob
const mockAtob = vi.fn()

describe('Auth Utilities', () => {
  beforeEach(() => {
    // Setup mocks
    vi.stubGlobal('localStorage', localStorageMock)
    vi.stubGlobal('atob', mockAtob)
    localStorageMock.clear()
  })

  afterEach(() => {
    vi.clearAllMocks()
    vi.unstubAllGlobals()
  })

  describe('getToken', () => {
    it('should return null when no token is stored', () => {
      expect(getToken()).toBeNull()
      expect(localStorageMock.getItem).toHaveBeenCalledWith('token')
    })

    it('should return the token when it is stored', () => {
      const testToken = 'test-jwt-token'
      localStorageMock.setItem('token', testToken)
      
      expect(getToken()).toBe(testToken)
      expect(localStorageMock.getItem).toHaveBeenCalledWith('token')
    })
  })

  describe('setToken', () => {
    it('should store the token in localStorage when a token is provided', () => {
      const testToken = 'test-jwt-token'
      setToken(testToken)
      
      expect(localStorageMock.setItem).toHaveBeenCalledWith('token', testToken)
    })

    it('should remove the token from localStorage when null is provided', () => {
      setToken(null)
      
      expect(localStorageMock.removeItem).toHaveBeenCalledWith('token')
    })
  })

  describe('validateToken', () => {
    it('should return false when no token is stored', async () => {
      expect(await validateToken()).toBe(false)
    })

    it('should return false when token format is invalid', async () => {
      localStorageMock.setItem('token', 'invalid-token')
      
      expect(await validateToken()).toBe(false)
    })

    it('should return false when token is expired', async () => {
      // Create an expired token (exp is in the past)
      const expiredPayload = { exp: Math.floor(Date.now() / 1000) - 3600 } // 1 hour ago
      const expiredToken = `header.${btoa(JSON.stringify(expiredPayload))}.signature`
      
      localStorageMock.setItem('token', expiredToken)
      mockAtob.mockReturnValue(JSON.stringify(expiredPayload))
      
      expect(await validateToken()).toBe(false)
      expect(localStorageMock.removeItem).toHaveBeenCalledWith('token')
    })

    it('should return true when token is valid and not expired', async () => {
      // Create a valid token (exp is in the future)
      const validPayload = { exp: Math.floor(Date.now() / 1000) + 3600 } // 1 hour in the future
      const validToken = `header.${btoa(JSON.stringify(validPayload))}.signature`
      
      localStorageMock.setItem('token', validToken)
      mockAtob.mockReturnValue(JSON.stringify(validPayload))
      
      expect(await validateToken()).toBe(true)
    })
  })

  describe('isAuthenticated', () => {
    it('should call validateToken and return its result', async () => {
      // Create a valid token
      const validPayload = { exp: Math.floor(Date.now() / 1000) + 3600 }
      const validToken = `header.${btoa(JSON.stringify(validPayload))}.signature`
      
      localStorageMock.setItem('token', validToken)
      mockAtob.mockReturnValue(JSON.stringify(validPayload))
      
      expect(await isAuthenticated()).toBe(true)
      
      // Test with invalid token
      localStorageMock.removeItem('token')
      expect(await isAuthenticated()).toBe(false)
    })
  })
})
