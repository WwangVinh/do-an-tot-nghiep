export type AdminRole =
  | 'Admin'
  | 'Manager'
  | 'ShowroomSales'
  | 'Technician'
  | 'Sales'
  | 'Marketing'
  | 'Content'

export function normalizeRole(role: unknown): AdminRole | null {
  if (typeof role !== 'string') return null
  const r = role.trim().toLowerCase()
  if (!r) return null

  switch (r) {
    case 'admin':
      return 'Admin'
    case 'manager':
      return 'Manager'
    case 'showroomsales':
      return 'ShowroomSales'
    case 'technician':
      return 'Technician'
    case 'sales':
      return 'Sales'
    case 'marketing':
      return 'Marketing'
    case 'content':
      return 'Content'
    default:
      return null
  }
}

export function isInRole(userRole: unknown, allowed: readonly AdminRole[]): boolean {
  if (!allowed.length) return true
  const r = normalizeRole(userRole)
  if (!r) return false
  return allowed.includes(r)
}

export function isAdminRole(userRole: unknown): boolean {
  return normalizeRole(userRole) === 'Admin'
}