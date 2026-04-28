import { http } from '../http/http'

export type UserStatus = 'Active' | 'Inactive' | string
export type UserRole = 'Admin' | 'ShowroomManager' | 'ShowroomSales' | 'Customer' | string

export type AdminUserListItem = {
  userId: number
  username?: string | null
  email?: string | null
  fullName?: string | null
  phone?: string | null
  role?: UserRole | null
  status?: UserStatus | null
  showroomId?: number | null
  avatarUrl?: string | null
  createdAt?: string | null
  deletedAt?: string | null
}

export type AdminUsersListResponse = {
  totalItems: number
  currentPage: number
  pageSize: number
  totalPages: number
  data: AdminUserListItem[]
}

export type FetchAdminUsersParams = {
  userType?: 'Staff' | 'Customer' | string
  isDeleted?: boolean
  search?: string
  page?: number
  pageSize?: number
  filterShowroomId?: number | null
}

export type CreateAdminStaffInput = {
  username: string
  password: string
  fullName: string
  email?: string | null
  phone?: string | null
  role: 'ShowroomManager' | 'ShowroomSales' | 'SalesManager' | 'Sales' | 'Technician'
  showroomId: number
}

export type UpdateAdminStaffInput = {
  fullName: string
  email?: string | null
  phone?: string | null
  role: 'ShowroomManager' | 'ShowroomSales' | 'SalesManager' | 'Sales' | 'Technician'
  showroomId: number
  status?: 'Active' | 'Inactive' | null
}

type ApiOk<T> = { message?: string; Message?: string; data?: T; Data?: T }

function normalizeUser(raw: any): AdminUserListItem {
  return {
    userId: Number(raw?.UserId ?? raw?.userId ?? 0),
    username: (raw?.Username ?? raw?.username ?? null) as string | null,
    email: (raw?.Email ?? raw?.email ?? null) as string | null,
    fullName: (raw?.FullName ?? raw?.fullName ?? null) as string | null,
    phone: (raw?.Phone ?? raw?.phone ?? null) as string | null,
    role: (raw?.Role ?? raw?.role ?? null) as UserRole | null,
    status: (raw?.Status ?? raw?.status ?? null) as UserStatus | null,
    showroomId: (raw?.ShowroomId ?? raw?.showroomId ?? null) as number | null,
    avatarUrl: (raw?.AvatarUrl ?? raw?.avatarUrl ?? null) as string | null,
    createdAt: (raw?.CreatedAt ?? raw?.createdAt ?? null) as string | null,
    deletedAt: (raw?.DeletedAt ?? raw?.deletedAt ?? null) as string | null,
  }
}

export async function fetchAdminUsers(params: FetchAdminUsersParams): Promise<AdminUsersListResponse> {
  const res = await http.get('/api/admin/users', {
    params: {
      userType: params.userType ?? 'Staff',
      isDeleted: params.isDeleted ?? false,
      search: params.search?.trim() || undefined,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 12,
      filterShowroomId: params.filterShowroomId ?? undefined,
    },
  })

  const d = res.data ?? {}
  const dataRaw = d.Data ?? d.data ?? []
  const total = d.TotalCount ?? d.totalCount ?? d.TotalItems ?? d.totalItems ?? 0

  const currentPage = Number(params.page ?? 1)
  const pageSize = Number(params.pageSize ?? 12)
  const totalItems = Number(total ?? 0)
  const totalPages = Math.max(1, Math.ceil(totalItems / Math.max(1, pageSize)))

  return {
    totalItems,
    currentPage,
    pageSize,
    totalPages,
    data: Array.isArray(dataRaw) ? (dataRaw as any[]).map(normalizeUser) : [],
  }
}

export async function createAdminStaff(input: CreateAdminStaffInput): Promise<{ message?: string }> {
  const payload = {
    Username: input.username,
    Password: input.password,
    FullName: input.fullName,
    Email: input.email ?? null,
    Phone: input.phone ?? null,
    Role: input.role,
    ShowroomId: input.showroomId,
  }
  const res = await http.post<ApiOk<unknown>>('/api/admin/users/staff', payload)
  return { message: res.data?.message ?? res.data?.Message }
}

export async function updateAdminStaff(userId: number, input: UpdateAdminStaffInput): Promise<{ message?: string }> {
  const payload = {
    FullName: input.fullName,
    Email: input.email ?? null,
    Phone: input.phone ?? null,
    Role: input.role,
    ShowroomId: input.showroomId,
    Status: input.status ?? null,
  }
  const res = await http.put<ApiOk<unknown>>(`/api/admin/users/${userId}`, payload)
  return { message: res.data?.message ?? res.data?.Message }
}

export async function setAdminUserStatus(
  userId: number,
  action: 'Activate' | 'Deactivate' | 'Delete' | 'hard_delete' | string,
): Promise<{ message?: string }> {
  const res = await http.put<ApiOk<unknown>>(`/api/admin/users/${userId}/status`, null, { params: { action } })
  return { message: res.data?.message ?? res.data?.Message }
}

