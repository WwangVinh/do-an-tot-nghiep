import axios from 'axios'

import { env } from '../../lib/env'

export const http = axios.create({
  baseURL: env.VITE_API_BASE_URL,
  timeout: 20_000,
})

