import { http } from '../http/http'

export type OrderPaymentStatus = 'Unpaid' | 'Paid' | 'Refunded' | 'Failed' | string
export type OrderStatus = 'Pending' | 'Processing' | 'Completed' | 'Cancelled' | string

export type AdminOrderListItem = {
  orderId: number
  orderCode?: string | null
  userId?: number | null
  userName?: string | null
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
}

type ApiOk<T> = { message?: string; data?: T }

function normalizePaged<T>(
  raw: unknown,
): { totalItems: number; currentPage: number; pageSize: number; totalPages: number; data: T[] } {
  const r = raw as
    | {
        TotalItems?: unknown
        totalItems?: unknown
        CurrentPage?: unknown
        currentPage?: unknown
        PageSize?: unknown
        pageSize?: unknown
        TotalPages?: unknown
        totalPages?: unknown
        Data?: unknown
        data?: unknown
      }
    | null
    | undefined
  const totalItems = r?.TotalItems ?? r?.totalItems
  const currentPage = r?.CurrentPage ?? r?.currentPage
  const pageSize = r?.PageSize ?? r?.pageSize
  const totalPages = r?.TotalPages ?? r?.totalPages
  const data = r?.Data ?? r?.data
  return {
    totalItems: Number(totalItems ?? 0),
    currentPage: Number(currentPage ?? 1),
    pageSize: Number(pageSize ?? 10),
    totalPages: Number(totalPages ?? 1),
    data: Array.isArray(data) ? (data as T[]) : [],
  }
}

export async function fetchAdminOrders(params: FetchAdminOrdersParams): Promise<AdminOrdersListResponse> {
  // Convention: /api/admin/orders
  // Backend có thể trả về 1) PagedResponse {TotalItems, Data...} hoặc 2) array orders trực tiếp.
  const res = await http.get('/api/admin/orders', { params })
  const d = res.data
  if (Array.isArray(d)) {
    return {
      totalItems: d.length,
      currentPage: 1,
      pageSize: d.length,
      totalPages: 1,
      data: d as AdminOrderListItem[],
    }
  }
  return normalizePaged<AdminOrderListItem>(d ?? {})
}

export async function createAdminOrder(input: CreateAdminOrderInput): Promise<{ message?: string; data?: unknown }> {
  const payload = {
    userId: input.userId ?? null,
    phone: input.phone ?? null,
    carId: input.carId,
    quantity: input.quantity,
    shippingAddress: input.shippingAddress ?? null,
    paymentMethod: input.paymentMethod ?? null,
    status: input.status ?? null,
    paymentStatus: input.paymentStatus ?? null,
    promotionId: input.promotionId ?? null,
  }

  const res = await http.post<ApiOk<unknown>>('/api/admin/orders', payload)
  return { message: res.data?.message, data: res.data?.data }
}

