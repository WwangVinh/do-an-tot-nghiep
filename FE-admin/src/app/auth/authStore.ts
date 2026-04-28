export type AdminUser = {
  userId: number | null
  username: string | null
  role: string | null
  showroomId: number | null
}

export type AuthState = {
  token: string
  user: AdminUser | null
}

type Listener = () => void

export const AUTH_TOKEN_STORAGE_KEY = 'admin_token'
export const AUTH_USER_STORAGE_KEY = 'admin_user'

let state: AuthState = {
  token: '',
  user: null,
}

const listeners = new Set<Listener>()

function notify() {
  for (const l of listeners) l()
}

function safeParseJson(raw: string | null): unknown {
  if (!raw) return null
  try {
    return JSON.parse(raw)
  } catch {
    return null
  }
}

function normalizeToken(t: unknown): string {
  if (typeof t !== 'string') return ''
  const trimmed = t.trim()
  if (!trimmed) return ''
  return trimmed.startsWith('Bearer ') ? trimmed : `Bearer ${trimmed}`
}

function normalizeUser(u: unknown): AdminUser | null {
  if (!u || typeof u !== 'object') return null
  const anyU = u as Record<string, unknown>
  const userId = typeof anyU.userId === 'number' ? anyU.userId : null
  const username = typeof anyU.username === 'string' ? anyU.username : null
  const role = typeof anyU.role === 'string' ? anyU.role : null
  const showroomId = typeof anyU.showroomId === 'number' ? anyU.showroomId : null

  if (userId === null && username === null && role === null && showroomId === null) return null
  return { userId, username, role, showroomId }
}

export function initAuth() {
  let token = ''
  let user: AdminUser | null = null

  try {
    token = normalizeToken(localStorage.getItem(AUTH_TOKEN_STORAGE_KEY))
    user = normalizeUser(safeParseJson(localStorage.getItem(AUTH_USER_STORAGE_KEY)))
  } catch {
    // ignore
  }

  state = { token, user }
  notify()
}

export function getAuth(): AuthState {
  return state
}

export function getAuthToken(): string {
  return state.token
}

export function isAuthenticated(): boolean {
  return Boolean(state.token.trim())
}

export function setAuth(next: AuthState) {
  const token = normalizeToken(next.token)
  const user = next.user ? normalizeUser(next.user) : null

  state = { token, user }

  try {
    if (token) localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, token)
    else localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY)

    if (user) localStorage.setItem(AUTH_USER_STORAGE_KEY, JSON.stringify(user))
    else localStorage.removeItem(AUTH_USER_STORAGE_KEY)
  } catch {
    // ignore
  }

  notify()
}

export function clearAuth() {
  state = { token: '', user: null }
  try {
    localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY)
    localStorage.removeItem(AUTH_USER_STORAGE_KEY)
  } catch {
    // ignore
  }
  notify()
}

export function subscribeAuth(listener: Listener) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

