import axios from 'axios'
import type { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios'

import { useAuthStore } from '../stores/auth.store'
import { useErrorStore } from '../stores/error.store'

const api: AxiosInstance = axios.create({
  baseURL: '/api/v1',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore()
    const accessToken = authStore.accessToken

    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`
    }

    return config
  },
  (error) => Promise.reject(error)
)

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const errorStore = useErrorStore()

    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

    if (error.code === 'ERR_NETWORK' || error.code === 'ECONNABORTED' || error.code === 'ERR_CONNECTION_REFUSED') {
      errorStore.setError(
        'Ошибка соединения с сервером',
        'Проверьте подключение к интернету',
        undefined,
        false
      )
    } else if (error.response?.status === 500) {
      errorStore.setError(
        'Внутренняя ошибка сервера',
        'В данный момент сервер не отвечает. Мы уже работаем над устранением проблемы',
        500,
        true
      )
    } else if (error.response?.status === 403) {
      errorStore.setError(
        'Ошибка доступа',
        'У Вас нет доступа к этому ресурсу',
        403,
        false
      )
    } else if (error.response?.status === 401 && !originalRequest._retry) {
      const isAuthEndpoint =
        originalRequest.url?.includes('/auth/login') ||
        originalRequest.url?.includes('/auth/register') ||
        originalRequest.url?.includes('/auth/refresh');

      if (isAuthEndpoint) {
        return Promise.reject(error);
      }

      originalRequest._retry = true;
      const authStore = useAuthStore();

      try {
        await authStore.refresh();
        originalRequest.headers.Authorization = `Bearer ${authStore.accessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        errorStore.setError(
          'Ошибка авторизации',
          'Вам нужно заново зайти в свой аккаунт',
          401,
          false
        );
        authStore.logout();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }
  }
)

export default api
