import { http } from '../http/http'

export type BookingStatus = 'Scheduled' | 'Confirmed' | 'Completed' | 'Cancelled' | (string & {})

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

  const totalCount = d.TotalCount ?? d.totalCount ?? d.TotalItems ?? d.totalItems ?? 0
  const data = d.Data ?? d.data ?? []

  return {
    totalCount: Number(totalCount ?? 0),
    data: Array.isArray(data) ? (data as AdminBookingListItem[]) : [],
  }
}

export type AdminBookingDetail = {
  bookingId: number
  customerName?: string | null
  phone?: string | null
  bookingDate?: string | null
  bookingTime?: string | null
  status?: string | null
  note?: string | null // BE đang dùng Note để lưu "kết quả" dạng log
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

export type UpdateAdminBookingInput = {
  status?: BookingStatus | string | null
  result?: string | null
}

export async function updateAdminBooking(bookingId: number, input: UpdateAdminBookingInput): Promise<{ message?: string }> {
  const payload = {
    status: input.status ?? null,
    result: input.result ?? null,
  }
  const res = await http.put(`/api/admin/bookings/update/${bookingId}`, payload)
  const d = res.data ?? {}
  return { message: d.message ?? d.Message ?? undefined }
}

