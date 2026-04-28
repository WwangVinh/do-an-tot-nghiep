import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'

import { useAuth } from '../auth/useAuth'
import type { AdminRole } from '../auth/roles'
import { isInRole } from '../auth/roles'

export function RequireRole({
  allowed,
  children,
}: {
  allowed: readonly AdminRole[]
  children: ReactNode
}) {
  const auth = useAuth()
  const location = useLocation()

  if (!auth.isAuthenticated) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location.pathname + location.search }}
      />
    )
  }

  if (!isInRole(auth.user?.role, allowed)) {
    return <Navigate to="/403" replace />
  }

  return children
}

