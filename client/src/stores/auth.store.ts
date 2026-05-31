import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { AxiosError } from 'axios'

import api from '../services/api'

const STORAGE_KEYS = {
  ACCESS_TOKEN: 'docanai_access_token',
  REFRESH_TOKEN: 'docanai_refresh_token',
  USER_INFO: 'docanai_user_info'
}

interface UserInfo {
  username: string
  userType: string
}

interface LoginCredentials {
  username: string
  password: string
}

interface RegisterCredentials {
  username: string
  password: string
  userType: string
}

interface AuthResponse {
  accessToken: string
  refreshToken: string
  username: string
  userType: string
}

export const useAuthStore = defineStore('auth', () => {
  const userInfo = ref<UserInfo | null>(null)
  const accessToken = ref<string | null>(null)
  const refreshToken = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const initialized = ref(false)

  const clearLocalStorage = () => {
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.USER_INFO)
  }

  const saveToLocalStorage = () => {
    if (accessToken.value && refreshToken.value) {
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, accessToken.value)
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken.value)

      if (userInfo.value) localStorage.setItem(STORAGE_KEYS.USER_INFO, JSON.stringify(userInfo.value))
    }
  }

  const initialize = () => {
    const storedAccessToken = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)
    const storedRefreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN)
    const storedUser = localStorage.getItem(STORAGE_KEYS.USER_INFO)

    accessToken.value = storedAccessToken || null
    refreshToken.value = storedRefreshToken || null

    if (storedUser && storedUser !== 'undefined') {
      try {
        userInfo.value = JSON.parse(storedUser)
      } catch (err) {
        userInfo.value = null
      }
    }
  }

  initialize()

  const checkAuth = async (): Promise<boolean> => {
    if (initialized.value && userInfo.value) return true

    if (!accessToken.value || !refreshToken.value) {
      initialized.value = true
      return false
    }

    loading.value = true

    try {
      const response = await api.get<UserInfo>('/auth/me')

      userInfo.value = response.data

      saveToLocalStorage()
      initialized.value = true

      return true
    } catch (err) {
      userInfo.value = null
      accessToken.value = null
      refreshToken.value = null

      clearLocalStorage()
      initialized.value = true

      return false
    } finally {
      loading.value = false
    }
  }

  const login = async (credentials: LoginCredentials) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post<AuthResponse>('/auth/login', credentials)

      userInfo.value = { username: response.data.username, userType: response.data.userType }
      accessToken.value = response.data.accessToken
      refreshToken.value = response.data.refreshToken

      saveToLocalStorage()
      initialized.value = true
    } catch (err) {
      const axiosError = err as AxiosError<{ message: string }>
      error.value = axiosError.response?.data?.message || 'Неверный логин или пароль'

      throw error.value
    } finally {
      loading.value = false
    }
  }

  const register = async (credentials: RegisterCredentials) => {
    loading.value = true
    error.value = null

    try {
      const response = await api.post<AuthResponse>('/auth/register', credentials)

      userInfo.value = { username: response.data.username, userType: response.data.userType }
      accessToken.value = response.data.accessToken
      refreshToken.value = response.data.refreshToken

      saveToLocalStorage()
      initialized.value = true
    } catch (err) {
      const axiosError = err as AxiosError<{ message: string }>
      error.value = axiosError.response?.data?.message || 'Ошибка регистрации'

      throw error.value
    } finally {
      loading.value = false
    }
  }

  const refresh = async () => {
    loading.value = true
    error.value = null

    if (!refreshToken.value) throw new Error('No refresh token')
    
    try {
      const response = await api.post<AuthResponse>('/auth/refresh', { refreshToken: refreshToken.value })

      userInfo.value = { username: response.data.username, userType: response.data.userType }
      accessToken.value = response.data.accessToken
      refreshToken.value = response.data.refreshToken

      saveToLocalStorage()
      initialized.value = true
    } catch (err) {
      const axiosError = err as AxiosError<{ message: string }>
      error.value = axiosError.response?.data?.message || 'Ошибка авторизации'

      throw error.value
    } finally {
      loading.value = false
    }
  }

  const logout = () => {
    userInfo.value = null
    accessToken.value = null
    refreshToken.value = null
    clearLocalStorage()
    initialized.value = false
  }

  return {
    userInfo,
    accessToken,
    refreshToken,
    loading,
    error,
    checkAuth,
    login,
    register,
    refresh,
    logout
  }
})
