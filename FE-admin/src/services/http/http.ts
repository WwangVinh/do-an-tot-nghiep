import axios from 'axios'

import { env } from '../../lib/env'
import { clearAuth, getAuthToken } from '../../app/auth/authStore'

export const http = axios.create({
  baseURL: env.VITE_API_BASE_URL,
  timeout: 60_000, 
})

http.interceptors.request.use((config) => {
  const token = getAuthToken()
  if (token) {
    config.headers = config.headers ?? {}
    // Allow per-request override (e.g. manual token in some pages)
    if (!config.headers.Authorization) config.headers.Authorization = token
  }
  return config
})

http.interceptors.response.use(
  (res) => res,
  (err) => {
    const status = err?.response?.status
    if (status === 401) {
      // Token hết hạn / không hợp lệ -> clear để buộc login lại
      clearAuth()
    }
    return Promise.reject(err)
  },
)
