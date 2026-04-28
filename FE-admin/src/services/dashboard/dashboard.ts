import { http } from '../http/http'

export type DashboardSeriesPoint = { label: string; value: number }
export type DashboardBookingStatus = { status: string; count: number }
export type DashboardRecentOrder = {
  orderId: number
  orderCode?: string | null
  customerName?: string | null
  status?: string | null
  paymentStatus?: string | null
  amount: number
  orderDate?: string | null
}

export type DashboardSummary = {
  days: number
  revenue: number
  orders: number
  bookings: number
  activeUsers: number
  activeCars: number
  inventoryQuantity: number
  revenueByMonth: DashboardSeriesPoint[]
  ordersByWeek: DashboardSeriesPoint[]
  bookingByStatus: DashboardBookingStatus[]
  recentOrders: DashboardRecentOrder[]
}

function toNumber(v: unknown, fallback = 0) {
  const n = typeof v === 'number' ? v : Number(v)
  return Number.isFinite(n) ? n : fallback
}

function normalizeSummary(raw: any): DashboardSummary {
  const revenueByMonthRaw = raw?.RevenueByMonth ?? raw?.revenueByMonth ?? []
  const ordersByWeekRaw = raw?.OrdersByWeek ?? raw?.ordersByWeek ?? []
  const bookingByStatusRaw = raw?.BookingByStatus ?? raw?.bookingByStatus ?? []
  const recentOrdersRaw = raw?.RecentOrders ?? raw?.recentOrders ?? []

  return {
    days: toNumber(raw?.Days ?? raw?.days ?? 30, 30),
    revenue: toNumber(raw?.Revenue ?? raw?.revenue ?? 0),
    orders: toNumber(raw?.Orders ?? raw?.orders ?? 0),
    bookings: toNumber(raw?.Bookings ?? raw?.bookings ?? 0),
    activeUsers: toNumber(raw?.ActiveUsers ?? raw?.activeUsers ?? 0),
    activeCars: toNumber(raw?.ActiveCars ?? raw?.activeCars ?? 0),
    inventoryQuantity: toNumber(raw?.InventoryQuantity ?? raw?.inventoryQuantity ?? 0),
    revenueByMonth: Array.isArray(revenueByMonthRaw)
      ? revenueByMonthRaw.map((p: any) => ({
          label: String(p?.Label ?? p?.label ?? ''),
          value: toNumber(p?.Value ?? p?.value ?? 0),
        }))
      : [],
    ordersByWeek: Array.isArray(ordersByWeekRaw)
      ? ordersByWeekRaw.map((p: any) => ({
          label: String(p?.Label ?? p?.label ?? ''),
          value: toNumber(p?.Value ?? p?.value ?? 0),
        }))
      : [],
    bookingByStatus: Array.isArray(bookingByStatusRaw)
      ? bookingByStatusRaw.map((x: any) => ({
          status: String(x?.Status ?? x?.status ?? 'Unknown'),
          count: toNumber(x?.Count ?? x?.count ?? 0),
        }))
      : [],
    recentOrders: Array.isArray(recentOrdersRaw)
      ? recentOrdersRaw.map((o: any) => ({
          orderId: toNumber(o?.OrderId ?? o?.orderId ?? 0),
          orderCode: (o?.OrderCode ?? o?.orderCode ?? null) as string | null,
          customerName: (o?.CustomerName ?? o?.customerName ?? null) as string | null,
          status: (o?.Status ?? o?.status ?? null) as string | null,
          paymentStatus: (o?.PaymentStatus ?? o?.paymentStatus ?? null) as string | null,
          amount: toNumber(o?.Amount ?? o?.amount ?? 0),
          orderDate: (o?.OrderDate ?? o?.orderDate ?? null) as string | null,
        }))
      : [],
  }
}

export async function fetchDashboardSummary(days = 30): Promise<DashboardSummary> {
  const res = await http.get('/api/admin/dashboard/summary', { params: { days } })
  return normalizeSummary(res.data ?? {})
}

