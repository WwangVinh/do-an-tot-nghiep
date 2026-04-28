

import { z } from 'zod'

export function normalizeApiBase(url: string): string {
  let u = url.trim().replace(/\/+$/, '')
  if (u.endsWith('/api')) u = u.slice(0, -4).replace(/\/+$/, '')
  return u || 'https://localhost:7033' // Đã thay đổi
}

const envSchema = z.object({
  VITE_API_BASE_URL: z.preprocess(
    (v) => {
      if (v === undefined || v === null) return undefined
      const s = String(v).trim()
      return s === '' ? undefined : s
    },
    z.string().optional().default('https://localhost:7033'), // Đã thay đổi
  ),
})

const parsed = envSchema.parse(import.meta.env)

export const env = {
  VITE_API_BASE_URL: normalizeApiBase(parsed.VITE_API_BASE_URL ?? 'https://localhost:7033'), // Đã thay đổi
}


// import { z } from 'zod'

// export function normalizeApiBase(url: string): string {
//   let u = url.trim().replace(/\/+$/, '')
//   if (u.endsWith('/api')) u = u.slice(0, -4).replace(/\/+$/, '')
//   return u || 'https://spinulose-polishedly-enrique.ngrok-free.dev'
// }

// const envSchema = z.object({
//   VITE_API_BASE_URL: z.preprocess(
//     (v) => {
//       if (v === undefined || v === null) return undefined
//       const s = String(v).trim()
//       return s === '' ? undefined : s
//     },
//     z.string().optional().default('https://spinulose-polishedly-enrique.ngrok-free.dev'),
//   ),
// })

// const parsed = envSchema.parse(import.meta.env)

// export const env = {
//   VITE_API_BASE_URL: normalizeApiBase(parsed.VITE_API_BASE_URL ?? 'https://spinulose-polishedly-enrique.ngrok-free.dev'),
// }