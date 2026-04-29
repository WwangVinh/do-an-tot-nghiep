import { http } from '../http/http'

export type BookingStatus =
  | 'Pending'
  | 'Consulted'
  | 'PendingTechCheck'
  | 'TechApproved'
  | 'Confirmed'
  | 'Completed'
  | 'NoShow'
  | 'Cancelled'
  | (string & {})

export type AdminBookingListItem = {
  bookingId: number
  customerName?: string | null
  phone?: string | null
  bookingDate?: string | null
  bookingTime?: string | null
  status?: string | null
  carName?: string | null
  showroomName?: string | null
  createdAt?: string | null
}

export type AdminBookingsListResponse = {
  totalCount: number
  data: AdminBookingListItem[]
}

export type FetchAdminBookingsParams = {
  page?: number
  pageSize?: number
  search?: string
  status?: string
}

export async function fetchAdminBookings(params: FetchAdminBookingsParams): Promise<AdminBookingsListResponse> {
  const res = await http.get('/api/admin/bookings/list', { params })
  const d = res.data ?? {}
  return {
    totalCount: Number(d.TotalCount ?? d.totalCount ?? 0),
    data: Array.isArray(d.Data ?? d.data) ? (d.Data ?? d.data) : [],
  }
}

export type AdminBookingDetail = {
  bookingId: number
  customerName?: string | null
  phone?: string | null
  bookingDate?: string | null
  bookingTime?: string | null
  status?: string | null
  note?: string | null
  carDetails?: { carId?: number | null; name?: string | null; imageUrl?: string | null } | null
  showroomDetails?: { showroomId?: number | null; name?: string | null; province?: string | null } | null
  createdAt?: string | null
  updatedAt?: string | null
}

export async function fetchAdminBookingDetail(bookingId: number): Promise<AdminBookingDetail> {
  const res = await http.get(`/api/admin/bookings/detail/${bookingId}`)
  const d = res.data ?? {}
  return {
    bookingId: Number(d.BookingId ?? d.bookingId ?? bookingId),
    customerName: d.CustomerName ?? d.customerName ?? null,
    phone: d.Phone ?? d.phone ?? null,
    bookingDate: d.BookingDate ?? d.bookingDate ?? null,
    bookingTime: d.BookingTime ?? d.bookingTime ?? null,
    status: d.Status ?? d.status ?? null,
    note: d.Note ?? d.note ?? null,
    carDetails: d.CarDetails ?? d.carDetails ?? null,
    showroomDetails: d.ShowroomDetails ?? d.showroomDetails ?? null,
    createdAt: d.CreatedAt ?? d.createdAt ?? null,
    updatedAt: d.UpdatedAt ?? d.updatedAt ?? null,
  }
}

// BƯỚC 1
export async function consultBooking(bookingId: number, consultNote: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/consult`, { consultNote })
  return { message: res.data?.message ?? res.data?.Message }
}

// BƯỚC 2
export async function sendToTechCheck(bookingId: number, techNote?: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/send-tech`, { techNote: techNote ?? null })
  return { message: res.data?.message ?? res.data?.Message }
}

// BƯỚC 3
export async function submitTechResult(bookingId: number, isApproved: boolean, techNote: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/tech-result`, { isApproved, techNote })
  return { message: res.data?.message ?? res.data?.Message }
}

// BƯỚC 4
export async function confirmBooking(bookingId: number): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/confirm`)
  return { message: res.data?.message ?? res.data?.Message }
}

// BƯỚC 5a
export async function completeBooking(bookingId: number, resultNote?: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/complete`, { resultNote: resultNote ?? null })
  return { message: res.data?.message ?? res.data?.Message }
}

// BƯỚC 5b
export async function markNoShow(bookingId: number, reason?: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/no-show`, { reason: reason ?? null })
  return { message: res.data?.message ?? res.data?.Message }
}

// HỦY LỊCH
export async function cancelBookingByAdmin(bookingId: number, cancelReason: string): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/bookings/${bookingId}/cancel`, { cancelReason })
  return { message: res.data?.message ?? res.data?.Message }
}

// DANH SÁCH CHỜ KỸ THUẬT
export async function fetchPendingTechCheck(): Promise<AdminBookingListItem[]> {
  const res = await http.get('/api/admin/bookings/pending-tech')
  return Array.isArray(res.data) ? res.data : []
}

// THỐNG KÊ
export async function fetchBookingStats(): Promise<Record<string, number>> {
  const res = await http.get('/api/admin/bookings/stats')
  return res.data ?? {}
}