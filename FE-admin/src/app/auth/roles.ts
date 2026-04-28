export type AdminRole =
  | 'Admin'
  | 'ShowroomManager'
  | 'ShowroomSales'
  | 'SalesManager'
  | 'Sales'
  | 'Technician'

export function normalizeRole(role: unknown): AdminRole | null {
  if (typeof role !== 'string') return null
  const r = role.trim()
  if (!r) return null

  const lower = r.toLowerCase()
  if (lower === 'admin') return 'Admin'
  if (lower === 'showroommanager') return 'ShowroomManager'
  if (lower === 'showroomsales') return 'ShowroomSales'
  if (lower === 'salesmanager') return 'SalesManager'
  if (lower === 'sales' || lower === 'sale') return 'Sales'
  if (lower === 'technician' || lower === 'tech') return 'Technician'
  return null
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

