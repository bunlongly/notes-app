import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authService } from '@/services/auth.service';
import type { LoginRequest, RegisterRequest, User } from '@/types';

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null);
  const accessToken = ref<string | null>(null);
  const refreshToken = ref<string | null>(null);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const isAuthenticated = computed(() => !!accessToken.value);

  // Check for existing auth session on app load
  function checkAuth() {
    const savedAccessToken = localStorage.getItem('accessToken');
    const savedRefreshToken = localStorage.getItem('refreshToken');
    const savedUser = localStorage.getItem('user');

    if (savedAccessToken && savedRefreshToken && savedUser) {
      accessToken.value = savedAccessToken;
      refreshToken.value = savedRefreshToken;
      user.value = JSON.parse(savedUser);
    }
  }

  async function login(credentials: LoginRequest) {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await authService.login(credentials);

      accessToken.value = response.accessToken;
      refreshToken.value = response.refreshToken;
      user.value = {
        userId: response.userId,
        email: response.email,
        fullName: response.fullName
      };

      // Store tokens in localStorage for persistence
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      localStorage.setItem('user', JSON.stringify(user.value));

      return true;
    } catch (err: any) {
      // Handle rate limit errors with detailed message
      if (err.response?.status === 429) {
        const retryAfter = err.response?.data?.retryAfter || 'a few moments';
        error.value = `Too many login attempts. Please try again after ${retryAfter}.`;
      } else {
        error.value =
          err.response?.data?.message ||
          'Login failed. Please check your credentials.';
      }
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  async function register(userData: RegisterRequest) {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await authService.register(userData);

      accessToken.value = response.accessToken;
      refreshToken.value = response.refreshToken;
      user.value = {
        userId: response.userId,
        email: response.email,
        fullName: response.fullName
      };

      // Store tokens in localStorage for persistence
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      localStorage.setItem('user', JSON.stringify(user.value));

      return true;
    } catch (err: any) {
      // Handle rate limit errors with detailed message
      if (err.response?.status === 429) {
        const retryAfter = err.response?.data?.retryAfter || 'a few moments';
        error.value = `Too many registration attempts. Please try again after ${retryAfter}.`;
      } else {
        error.value =
          err.response?.data?.message ||
          'Registration failed. Please try again.';
      }
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  // Refresh access token using refresh token
  async function refreshAccessToken() {
    try {
      if (!refreshToken.value) {
        throw new Error('No refresh token available');
      }

      const response = await authService.refreshToken(refreshToken.value);

      accessToken.value = response.accessToken;
      refreshToken.value = response.refreshToken;

      // Update stored tokens
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);

      return true;
    } catch (err) {
      // If refresh fails, logout user
      logout();
      return false;
    }
  }

  function logout() {
    user.value = null;
    accessToken.value = null;
    refreshToken.value = null;

    // Clear all stored data
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }

  return {
    user,
    accessToken,
    refreshToken,
    isLoading,
    error,
    isAuthenticated,
    checkAuth,
    login,
    register,
    refreshAccessToken,
    logout
  };
});
