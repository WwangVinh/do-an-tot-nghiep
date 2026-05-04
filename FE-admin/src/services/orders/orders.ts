import { http } from '../http/http'

export type OrderPaymentStatus = 'Unpaid' | 'Paid' | 'Refunded' | 'Failed' | string
export type OrderStatus = 'Pending' | 'Processing' | 'Completed' | 'Cancelled' | string

export type AdminOrderListItem = {
  orderId: number
  orderCode?: string | null
  userId?: number | null
  userName?: string | null
  fullName?: string | null
  phone?: string | null
  carId?: number | null
  carName?: string | null
  orderDate?: string | null
  status?: string | null
  paymentStatus?: string | null
  paymentMethod?: string | null
  subtotal?: number | null
  discountAmount?: number | null
  finalAmount?: number | null
  totalAmount?: number | null
  shippingAddress?: string | null
  showroomId?: number | null
  showroomName?: string | null
  staffName?: string | null
  adminNote?: string | null
}

export type AdminOrdersListResponse = {
  totalItems: number
  currentPage: number
  pageSize: number
  totalPages: number
  data: AdminOrderListItem[]
}

export type FetchAdminOrdersParams = {
  search?: string
  status?: OrderStatus | ''
  paymentStatus?: OrderPaymentStatus | ''
  page?: number
  pageSize?: number
}

export type CreateAdminOrderInput = {
  userId?: number | null
  phone?: string | null
  carId: number
  quantity: number
  shippingAddress?: string | null
  paymentMethod?: string | null
  status?: OrderStatus | null
  paymentStatus?: OrderPaymentStatus | null
  promotionId?: number | null
  showroomId?: number | null
}

export type ShowroomPickerItem = {
  showroomId: number
  name: string
  province: string
  district: string
  fullAddress: string
  hotline: string | null
}

export type OrderItemDetail = {
  orderItemId: number
  itemType: string
  name: string
  quantity: number
  price: number
}

export type PaymentTransaction = {
  transactionId: number
  amount: number | null
  paymentMethod: string | null
  status: string | null
  transactionDate: string | null
}

export type AdminOrderDetail = {
  orderId: number
  orderCode: string | null
  fullName: string
  phone: string
  email: string | null
  customerNote: string | null
  adminNote: string | null
  status: string | null
  paymentStatus: string | null
  subtotal: number
  discountAmount: number
  finalAmount: number
  orderDate: string | null
  lastUpdated: string | null
  staffName: string | null
  carName: string | null
  showroomId: number | null
  showroomName: string | null
  items: OrderItemDetail[]
  payments: PaymentTransaction[]
}

// ─── Helpers ─────────────────────────────────────────────────────────────────

type ApiOk<T> = { message?: string; data?: T }

function normalizePaged<T>(raw: unknown): {
  totalItems: number
  currentPage: number
  pageSize: number
  totalPages: number
  data: T[]
} {
  const r = raw as {
    TotalItems?: unknown; totalItems?: unknown
    CurrentPage?: unknown; currentPage?: unknown
    PageSize?: unknown; pageSize?: unknown
    TotalPages?: unknown; totalPages?: unknown
    Data?: unknown; data?: unknown
  } | null | undefined

  const dataArr = r?.Data ?? r?.data
  return {
    totalItems:  Number(r?.TotalItems  ?? r?.totalItems  ?? 0),
    currentPage: Number(r?.CurrentPage ?? r?.currentPage ?? 1),
    pageSize:    Number(r?.PageSize    ?? r?.pageSize    ?? 10),
    totalPages:  Number(r?.TotalPages  ?? r?.totalPages  ?? 1),
    data: Array.isArray(dataArr) ? (dataArr as T[]) : [],
  }
}

// ─── API Functions ────────────────────────────────────────────────────────────

export async function fetchAdminOrders(
  params: FetchAdminOrdersParams,
): Promise<AdminOrdersListResponse> {
  const res = await http.get('/api/admin/orders', { params })
  const d = res.data

  if (Array.isArray(d)) {
    return { totalItems: d.length, currentPage: 1, pageSize: d.length, totalPages: 1, data: d as AdminOrderListItem[] }
  }

  // BE trả flat { success, totalItems, currentPage, totalPages, data }
  // hoặc nested { data: { totalItems, ... } }
  const payload = (d?.data && typeof d.data === 'object' && !Array.isArray(d.data)) ? d.data : d
  return normalizePaged<AdminOrderListItem>(payload ?? {})
}

export async function createAdminOrder(
  input: CreateAdminOrderInput,
): Promise<{ message?: string; data?: unknown }> {
  const res = await http.post<ApiOk<unknown>>('/api/admin/orders', {
    userId:          input.userId ?? null,
    phone:           input.phone ?? null,
    carId:           input.carId,
    quantity:        input.quantity,
    shippingAddress: input.shippingAddress ?? null,
    paymentMethod:   input.paymentMethod ?? null,
    status:          input.status ?? null,
    paymentStatus:   input.paymentStatus ?? null,
    promotionId:     input.promotionId ?? null,
    showroomId:      input.showroomId ?? null,
  })
  return { message: res.data?.message, data: res.data?.data }
}

export async function fetchShowroomsByCar(carId: number): Promise<ShowroomPickerItem[]> {
  const res = await http.get(`/api/public/cars/${carId}/showrooms`)
  const d = res.data
  return d?.data ?? d ?? []
}

export async function fetchOrderDetail(orderId: number): Promise<AdminOrderDetail> {
  const res = await http.get(`/api/admin/orders/${orderId}`)
  return res.data?.data ?? res.data
}

export async function updateOrderStatus(
  orderId: number,
  status: string,
  adminNote: string,
): Promise<{ message?: string }> {
  const res = await http.put(`/api/admin/orders/${orderId}/status`, { status, adminNote })
  return res.data
}

export async function addPaymentTransaction(
  orderId: number,
  payload: { amount: number; paymentMethod: string; status: string },
): Promise<{ message?: string }> {
  const res = await http.post(`/api/admin/orders/${orderId}/payments`, payload)
  return res.data
}