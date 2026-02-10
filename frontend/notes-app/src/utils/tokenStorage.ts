/**
 * Token Storage Utility
 *
 * Best Practice: Use httpOnly cookies for production (set by backend)
 * Development: Can use localStorage for easier debugging
 *
 * This utility provides a consistent interface for both approaches
 */

// In production, backend should set httpOnly cookies
// Frontend just needs to enable credentials in axios
const USE_COOKIES = import.meta.env.VITE_USE_COOKIES === 'true';

export const tokenStorage = {
  /**
   * Set access token
   * Note: In cookie mode, this is just for local state tracking
   * Real token is set by backend as httpOnly cookie
   */
  setAccessToken: (token: string) => {
    if (!USE_COOKIES) {
      localStorage.setItem('accessToken', token);
    }
    // In cookie mode, backend handles this via Set-Cookie header
  },

  /**
   * Get access token
   * Note: In cookie mode, you can't read httpOnly cookies
   * This returns null and relies on cookies being sent automatically
   */
  getAccessToken: (): string | null => {
    if (!USE_COOKIES) {
      return localStorage.getItem('accessToken');
    }
    // In cookie mode, cookies are sent automatically with requests
    // We don't need to manually add Authorization header
    return null;
  },

  /**
   * Set refresh token
   */
  setRefreshToken: (token: string) => {
    if (!USE_COOKIES) {
      localStorage.setItem('refreshToken', token);
    }
    // In cookie mode, backend handles this
  },

  /**
   * Get refresh token
   */
  getRefreshToken: (): string | null => {
    if (!USE_COOKIES) {
      return localStorage.getItem('refreshToken');
    }
    return null;
  },

  /**
   * Set user data (this can still use localStorage/sessionStorage)
   */
  setUser: (user: any) => {
    localStorage.setItem('user', JSON.stringify(user));
  },

  /**
   * Get user data
   */
  getUser: (): any | null => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  /**
   * Clear all auth data
   */
  clear: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    // Note: cookies are cleared by backend setting expired cookie
  },

  /**
   * Check if using cookie-based auth
   */
  isUsingCookies: () => USE_COOKIES
};

/**
 * SECURITY NOTES:
 *
 * 1. localStorage/sessionStorage:
 *    ❌ Vulnerable to XSS attacks
 *    ✅ Easy to implement and debug
 *    ✅ Works with any backend
 *
 * 2. httpOnly Cookies:
 *    ✅ Protected from XSS (JavaScript can't read them)
 *    ✅ Automatic CSRF protection with SameSite
 *    ✅ Backend controls expiration
 *    ❌ Requires backend cookie support
 *    ❌ Harder to debug (can't see in console)
 *
 * RECOMMENDATION:
 * - Development: localStorage (easier debugging)
 * - Production: httpOnly cookies (better security)
 */
