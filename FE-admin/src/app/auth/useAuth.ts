import { useSyncExternalStore } from 'react'

import { getAuth, isAuthenticated, subscribeAuth } from './authStore'

export function useAuth() {
  const auth = useSyncExternalStore(subscribeAuth, getAuth, getAuth)
  const authed = useSyncExternalStore(subscribeAuth, isAuthenticated, isAuthenticated)

  return { ...auth, isAuthenticated: authed }
}

