import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Plus, RefreshCcw, Save, X } from 'lucide-react'

import type { AdminCarListItem, FetchAdminCarsParams } from '../../services/cars/cars'
import { fetchAdminCars } from '../../services/cars/cars'
import type { CreateAdminOrderInput, FetchAdminOrdersParams, ShowroomPickerItem } from '../../services/orders/orders'
import { createAdminOrder, fetchAdminOrders, fetchShowroomsByCar } from '../../services/orders/orders'
import type { AdminUserListItem, FetchAdminUsersParams } from '../../services/users/users'
import { fetchAdminUsers } from '../../services/users/users'
import { OrderDetailModal } from './OrderDetailModal'

const PAYMENT_METHOD_OPTIONS = [
  { value: 'COD', label: 'COD (thanh toán khi nhận hàng)' },
  { value: 'BANK_TRANSFER', label: 'Chuyển khoản ngân hàng' },
  { value: 'MOMO', label: 'Ví MoMo' },
  { value: 'ZALOPAY', label: 'Ví ZaloPay' },
  { value: 'VNPAY', label: 'VNPay' },
  { value: 'CARD', label: 'Thẻ (Online)' },
] as const

const ORDER_STATUS_OPTIONS = [
  { value: 'Pending', label: 'Chờ xử lý' },
  { value: 'Confirmed', label: 'Đã xác nhận' },
  { value: 'Shipping', label: 'Đang giao' },
  { value: 'Completed', label: 'Hoàn tất' },
  { value: 'Cancelled', label: 'Đã hủy' },
  { value: 'Returned', label: 'Trả hàng' },
] as const

const PAYMENT_STATUS_OPTIONS = [
  { value: 'Unpaid',    label: 'Chưa thanh toán' },
  { value: 'Deposited', label: 'Đã cọc'          },
  { value: 'Paid',      label: 'Đã thanh toán đủ' },
  { value: 'Refunded',  label: 'Đã hoàn tiền'    },
  { value: 'Failed',    label: 'Thất bại'         },
] as const

function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const anyErr = err as { message?: unknown; response?: { data?: unknown } }
    const data = anyErr.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (typeof anyErr.message === 'string' && anyErr.message.trim()) return anyErr.message
  }
  return 'Có lỗi xảy ra'
}

function emptyForm(): CreateAdminOrderInput {
  return {
    userId: null,
    phone: null,
    carId: 0,
    quantity: 1,
    shippingAddress: '',
    paymentMethod: 'COD',
    status: 'Pending',
    paymentStatus: 'Unpaid',
    promotionId: null,
    showroomId: null,
  }
}

function fmtMoney(v: unknown): string {
  const n = typeof v === 'number' ? v : Number(v)
  if (!Number.isFinite(n)) return '0'
  return n.toLocaleString('vi-VN')
}

// ─── Badge trạng thái ───
function StatusBadge({ value }: { value: string | null | undefined }) {
  const map: Record<string, string> = {
    Pending: 'bg-yellow-100 text-yellow-700',
    Confirmed: 'bg-blue-100 text-blue-700',
    Shipping: 'bg-indigo-100 text-indigo-700',
    Completed: 'bg-green-100 text-green-700',
    Cancelled: 'bg-red-100 text-red-700',
    Returned: 'bg-orange-100 text-orange-700',
  }
  const cls = map[value ?? ''] ?? 'bg-slate-100 text-slate-600'
  return (
    <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-semibold ${cls}`}>
      {value ?? '—'}
    </span>
  )
}

function PaymentBadge({ value }: { value: string | null | undefined }) {
  const map: Record<string, string> = {
    Unpaid: 'bg-rose-100 text-rose-700',
    Deposited: 'bg-amber-100 text-amber-700',
    Paid: 'bg-emerald-100 text-emerald-700',
    Refunded: 'bg-slate-100 text-slate-600',
    Failed: 'bg-red-100 text-red-700',
  }
  const cls = map[value ?? ''] ?? 'bg-slate-100 text-slate-600'
  return (
    <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-semibold ${cls}`}>
      {value ?? '—'}
    </span>
  )
}

export function OrdersPage() {
  const qc = useQueryClient()
  const [searchInput, setSearchInput] = useState('')   // giá trị input tức thời
  const [search, setSearch] = useState('')              // giá trị debounced gửi lên BE
  const [status, setStatus] = useState('')
  const [paymentStatus, setPaymentStatus] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  // Debounce search 400ms — tránh gọi API mỗi keystroke
  useEffect(() => {
    const timer = setTimeout(() => {
      setSearch(searchInput.trim())
      setPage(1)
    }, 400)
    return () => clearTimeout(timer)
  }, [searchInput])

  const [dialogOpen, setDialogOpen] = useState(false)
  const [form, setForm] = useState<CreateAdminOrderInput>(() => emptyForm())
  const [carSearch, setCarSearch] = useState('')
  const [userMode, setUserMode] = useState<'select' | 'phone'>('select')
  const [userSearch, setUserSearch] = useState('')

  // ✅ Showroom state trong form
  const [showroomList, setShowroomList] = useState<ShowroomPickerItem[]>([])
  const [showroomLoading, setShowroomLoading] = useState(false)

  // ✅ Detail modal
  const [selectedOrderId, setSelectedOrderId] = useState<number | null>(null)

  const params = useMemo<FetchAdminOrdersParams>(() => ({
    search: search.trim() || undefined,
    status: status.trim() ? status.trim() : undefined,
    paymentStatus: paymentStatus.trim() ? paymentStatus.trim() : undefined,
    page,
    pageSize,
  }), [page, pageSize, paymentStatus, search, status])

  const qKey = useMemo(() => ['admin-orders', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminOrders(params),
  })

  const carParams = useMemo<FetchAdminCarsParams>(() => ({
    search: carSearch.trim() || undefined,
    page: 1,
    pageSize: 50,
    isDeleted: false,
  }), [carSearch])

  const carsQ = useQuery({
    queryKey: ['admin-cars', carParams],
    queryFn: () => fetchAdminCars(carParams),
    enabled: dialogOpen,
  })

  const userParams = useMemo<FetchAdminUsersParams>(() => ({
    userType: 'Customer',
    isDeleted: false,
    search: userSearch.trim() || undefined,
    page: 1,
    pageSize: 50,
  }), [userSearch])

  const usersQ = useQuery({
    queryKey: ['admin-users', userParams],
    queryFn: () => fetchAdminUsers(userParams),
    enabled: dialogOpen && userMode === 'select',
  })

  // ✅ Load showroom khi carId thay đổi
  useEffect(() => {
    const carId = Number(form.carId)
    if (!dialogOpen || !carId || carId <= 0) {
      setShowroomList([])
      setForm(f => ({ ...f, showroomId: null }))
      return
    }
    setShowroomLoading(true)
    fetchShowroomsByCar(carId)
      .then(data => {
        setShowroomList(data)
        // Nếu showroom hiện tại không còn trong list thì reset
        setForm(f => ({
          ...f,
          showroomId: data.some(s => s.showroomId === f.showroomId) ? f.showroomId : null,
        }))
      })
      .catch(() => setShowroomList([]))
      .finally(() => setShowroomLoading(false))
  }, [form.carId, dialogOpen])

  const createM = useMutation({
    mutationFn: (input: CreateAdminOrderInput) => createAdminOrder(input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Tạo đơn hàng thành công')
      setDialogOpen(false)
      setForm(emptyForm())
      await qc.invalidateQueries({ queryKey: ['admin-orders'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const submitting = createM.isPending
  const rows = listQ.data?.data ?? []
  const totalPages = listQ.data?.totalPages ?? 1
  const carOptions: AdminCarListItem[] = carsQ.data?.data ?? []
  const userOptions: AdminUserListItem[] = usersQ.data?.data ?? []

  function openCreate() {
    setForm(emptyForm())
    setCarSearch('')
    setUserMode('select')
    setUserSearch('')
    setShowroomList([])
    setDialogOpen(true)
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
    setForm(emptyForm())
    setCarSearch('')
    setUserMode('select')
    setUserSearch('')
    setShowroomList([])
  }

  function submit() {
    const carId = Number(form.carId)
    const quantity = Number(form.quantity)
    const userId = !form.userId ? null : Number(form.userId)
    const promotionId = !form.promotionId ? null : Number(form.promotionId)
    const showroomId = !form.showroomId ? null : Number(form.showroomId)

    if (!Number.isFinite(carId) || carId <= 0) { toast.error('Vui lòng chọn xe'); return }
    if (!Number.isFinite(quantity) || quantity <= 0) { toast.error('Vui lòng nhập Quantity hợp lệ'); return }

    const payload: CreateAdminOrderInput = {
      userId: userId !== null && !Number.isFinite(userId) ? null : userId,
      phone: (form.phone ?? '').toString().trim() || null,
      carId,
      quantity,
      shippingAddress: (form.shippingAddress ?? '').trim() || null,
      paymentMethod: (form.paymentMethod ?? '').trim() || null,
      status: (form.status ?? '').toString().trim() || null,
      paymentStatus: (form.paymentStatus ?? '').toString().trim() || null,
      promotionId: promotionId !== null && !Number.isFinite(promotionId) ? null : promotionId,
      showroomId, // ✅
    }

    if (payload.userId && payload.phone) payload.phone = null
    if (!payload.userId && !payload.phone) payload.phone = null

    createM.mutate(payload)
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Đơn hàng</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý đơn hàng (bảng `Orders`, `OrderItems`).</p>
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            className="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
            onClick={() => listQ.refetch()}
            disabled={listQ.isFetching}
          >
            <RefreshCcw size={16} />
            Tải lại
          </button>
          <button
            type="button"
            className="inline-flex h-10 items-center gap-2 rounded-md bg-slate-900 px-3 text-sm font-medium text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
            onClick={openCreate}
          >
            <Plus size={16} />
            Thêm đơn hàng
          </button>
        </div>
      </div>

      {/* ── Filters ── */}
      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-6">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <div className="relative mt-1">
              <input
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                placeholder="Mã đơn, tên khách, số điện thoại..."
                className="w-full rounded-md border border-slate-200 bg-white pl-3 pr-8 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
              />
              {searchInput && (
                <button
                  type="button"
                  onClick={() => { setSearchInput(''); setSearch(''); setPage(1) }}
                  className="absolute right-2 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"
                >
                  <X size={14} />
                </button>
              )}
            </div>
          </div>
          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái đơn</label>
            <select
              value={status}
              onChange={(e) => { setStatus(e.target.value); setPage(1) }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả trạng thái</option>
              {ORDER_STATUS_OPTIONS.map(o => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Thanh toán</label>
            <select
              value={paymentStatus}
              onChange={(e) => { setPaymentStatus(e.target.value); setPage(1) }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả</option>
              {PAYMENT_STATUS_OPTIONS.map(o => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>

        {/* ── Active filter chips ── */}
        {(search || status || paymentStatus) && (
          <div className="mt-3 flex flex-wrap items-center gap-2">
            <span className="text-xs text-slate-500 dark:text-zinc-400">Đang lọc:</span>
            {search && (
              <span className="inline-flex items-center gap-1 rounded-full bg-slate-100 px-2.5 py-1 text-xs font-medium text-slate-700 dark:bg-zinc-800 dark:text-zinc-200">
                🔍 "{search}"
                <button onClick={() => { setSearchInput(''); setSearch(''); setPage(1) }} className="hover:text-red-500 ml-0.5"><X size={11} /></button>
              </span>
            )}
            {status && (
              <span className="inline-flex items-center gap-1 rounded-full bg-blue-100 px-2.5 py-1 text-xs font-medium text-blue-700 dark:bg-blue-900/30 dark:text-blue-300">
                {ORDER_STATUS_OPTIONS.find(o => o.value === status)?.label ?? status}
                <button onClick={() => { setStatus(''); setPage(1) }} className="hover:text-red-500 ml-0.5"><X size={11} /></button>
              </span>
            )}
            {paymentStatus && (
              <span className="inline-flex items-center gap-1 rounded-full bg-amber-100 px-2.5 py-1 text-xs font-medium text-amber-700 dark:bg-amber-900/30 dark:text-amber-300">
                {PAYMENT_STATUS_OPTIONS.find(o => o.value === paymentStatus)?.label ?? paymentStatus}
                <button onClick={() => { setPaymentStatus(''); setPage(1) }} className="hover:text-red-500 ml-0.5"><X size={11} /></button>
              </span>
            )}
            <button
              onClick={() => { setSearchInput(''); setSearch(''); setStatus(''); setPaymentStatus(''); setPage(1) }}
              className="text-xs text-slate-400 underline hover:text-slate-600 dark:hover:text-zinc-200"
            >
              Xóa tất cả
            </button>
          </div>
        )}
        <div className={`mt-4 overflow-auto transition-opacity duration-150 ${listQ.isFetching ? 'opacity-60' : 'opacity-100'}`}>
          <table className="min-w-[1200px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3">Mã đơn</th>
                <th className="py-3 pr-3">Khách hàng</th>
                <th className="py-3 pr-3">Xe</th>
                <th className="py-3 pr-3">Showroom</th>
                <th className="py-3 pr-3">Ngày đặt</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3">Thanh toán</th>
                <th className="py-3 pr-3 text-right">Tổng tiền</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading && Array.from({ length: 6 }).map((_, i) => (
                <tr key={i} className="border-b border-slate-100 dark:border-zinc-900">
                  {[120, 130, 110, 100, 90, 70, 70, 80].map((w, j) => (
                    <td key={j} className="py-3 pr-3">
                      <div className={`h-3.5 rounded animate-pulse bg-slate-100 dark:bg-zinc-800`} style={{ width: w }} />
                      {j < 3 && <div className="mt-1.5 h-2.5 w-16 rounded animate-pulse bg-slate-50 dark:bg-zinc-900" />}
                    </td>
                  ))}
                </tr>
              ))}
              {listQ.isError && (
                <tr><td colSpan={8} className="py-4 text-sm text-rose-600">{getErrorMessage(listQ.error)}</td></tr>
              )}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 && (
                <tr><td colSpan={8} className="py-6 text-center text-sm text-slate-400">Không có dữ liệu.</td></tr>
              )}

              {rows.map((r) => (
                <tr
                  key={r.orderId}
                  onClick={() => setSelectedOrderId(r.orderId)}
                  className="border-b border-slate-100 hover:bg-slate-50 dark:border-zinc-900 dark:hover:bg-zinc-900/50 transition-colors cursor-pointer"
                >
                  {/* Mã đơn */}
                  <td className="py-3 pr-3">
                    <div className="font-semibold text-slate-900 dark:text-zinc-50">{r.orderCode ?? `#${r.orderId}`}</div>
                    <div className="text-xs text-slate-400">ID: {r.orderId}</div>
                  </td>

                  {/* Khách */}
                  <td className="py-3 pr-3">
                    {/* BE trả về fullName + phone (khách vãng lai) hoặc userName (user hệ thống) */}
                    <div className="font-medium text-slate-900 dark:text-zinc-50">
                      {r.fullName ?? r.userName ?? <span className="text-slate-400 italic text-xs">Chưa có tên</span>}
                    </div>
                    {r.phone && (
                      <div className="text-xs text-slate-500 dark:text-zinc-400 mt-0.5">
                        📞 {r.phone}
                      </div>
                    )}
                    {!r.phone && r.userId && (
                      <div className="text-xs text-slate-400">User #{r.userId}</div>
                    )}
                  </td>

                  {/* Xe */}
                  <td className="py-3 pr-3">
                    <div className="font-medium">{r.carName ?? '—'}</div>
                    {r.carId && <div className="text-xs text-slate-400">#{r.carId}</div>}
                  </td>

                  {/* ✅ Showroom */}
                  <td className="py-3 pr-3">
                    {r.showroomName && r.showroomName !== 'Chưa chọn showroom' ? (
                      <div className="flex items-start gap-1.5">
                        <svg className="mt-0.5 w-3.5 h-3.5 flex-shrink-0 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                        </svg>
                        <div>
                          <div className="text-sm">{r.showroomName}</div>
                          {r.staffName && r.staffName !== 'Chưa phân công' && (
                            <div className="text-xs text-slate-400 mt-0.5">👤 {r.staffName}</div>
                          )}
                        </div>
                      </div>
                    ) : (
                      <span className="text-xs text-slate-400 italic">Chưa chọn</span>
                    )}
                  </td>

                  {/* Ngày */}
                  <td className="py-3 pr-3 text-xs text-slate-500 dark:text-zinc-400">
                    {r.orderDate ? new Date(r.orderDate).toLocaleString('vi-VN') : '—'}
                  </td>

                  {/* Trạng thái */}
                  <td className="py-3 pr-3"><StatusBadge value={r.status} /></td>

                  {/* Thanh toán */}
                  <td className="py-3 pr-3"><PaymentBadge value={r.paymentStatus} /></td>

                  {/* Tổng */}
                  <td className="py-3 pr-3 text-right font-semibold text-slate-900 dark:text-zinc-50">
                    {fmtMoney(r.finalAmount ?? r.totalAmount ?? 0)}
                    <span className="ml-0.5 text-xs font-normal text-slate-400">đ</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* ── Pagination ── */}
        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {listQ.data?.currentPage ?? page}/{totalPages} · {listQ.data?.totalItems ?? rows.length} đơn hàng
          </div>
          <div className="flex items-center gap-2">
            <select
              value={String(pageSize)}
              onChange={(e) => { setPageSize(Number(e.target.value)); setPage(1) }}
              className="rounded-md border border-slate-200 bg-white px-2 py-1.5 text-xs text-slate-700 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >
              {[10, 20, 50].map((n) => <option key={n} value={n}>{n}/trang</option>)}
            </select>
            <button type="button" disabled={page <= 1} onClick={() => setPage(p => Math.max(1, p - 1))}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >← Trước</button>
            <button type="button" disabled={page >= totalPages} onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >Sau →</button>
          </div>
        </div>
      </div>

      {/* ── Dialog tạo đơn ── */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            {/* Header */}
            <div className="flex items-center justify-between gap-3 border-b border-slate-200 px-5 py-4 dark:border-zinc-800">
              <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">Thêm mới đơn hàng</div>
              <button type="button" onClick={closeDialog} disabled={submitting}
                className="rounded-md p-2 text-slate-600 hover:bg-slate-100 dark:text-zinc-300 dark:hover:bg-zinc-900"
              >
                <X size={18} />
              </button>
            </div>

            <div className="grid gap-4 overflow-y-auto max-h-[70vh] p-5 md:grid-cols-2">

              {/* Chọn xe */}
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Chọn xe <span className="text-red-500">*</span></label>
                <div className="mt-2 grid gap-2 md:grid-cols-[1fr_1fr]">
                  <input
                    value={carSearch}
                    onChange={(e) => setCarSearch(e.target.value)}
                    className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    placeholder="Tìm theo tên xe / hãng..."
                  />
                  <select
                    value={String(form.carId ?? 0)}
                    onChange={(e) => setForm(f => ({ ...f, carId: Number(e.target.value), showroomId: null }))}
                    className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  >
                    <option value="0">
                      {carsQ.isLoading ? 'Đang tải...' : carsQ.isError ? 'Lỗi tải xe' : '-- Chọn xe --'}
                    </option>
                    {carOptions.map((c) => (
                      <option key={c.carId} value={c.carId}>
                        {c.brand} {c.name} ({c.year}) · #{c.carId}
                      </option>
                    ))}
                  </select>
                </div>
                <p className="mt-1.5 text-xs text-slate-400">Tìm kiếm để lọc, rồi chọn xe trong dropdown.</p>
              </div>

              {/* ✅ Chọn Showroom — chỉ hiện khi đã chọn xe */}
              {Number(form.carId) > 0 && (
                <div className="md:col-span-2">
                  <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">
                    Showroom nhận xe
                    <span className="ml-1.5 text-xs font-normal text-slate-400">(chỉ hiển thị showroom còn xe trong kho)</span>
                  </label>
                  {showroomLoading ? (
                    <div className="mt-2 flex items-center gap-2 text-xs text-slate-400 py-2">
                      <svg className="animate-spin w-3.5 h-3.5" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"/>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                      </svg>
                      Đang tải showroom...
                    </div>
                  ) : showroomList.length === 0 ? (
                    <div className="mt-2 rounded-lg border border-amber-200 bg-amber-50 px-3 py-2 text-xs text-amber-700">
                      ⚠️ Không có showroom nào còn xe này trong kho.
                    </div>
                  ) : (
                    <div className="mt-2 space-y-2 max-h-48 overflow-y-auto pr-1">
                      {/* Option "chưa chọn" */}
                      <div
                        onClick={() => setForm(f => ({ ...f, showroomId: null }))}
                        className={`flex items-center gap-3 px-3 py-2.5 rounded-lg border cursor-pointer transition-all select-none text-sm ${
                          !form.showroomId
                            ? 'border-slate-300 bg-slate-50 text-slate-500'
                            : 'border-slate-200 hover:border-slate-300 text-slate-400'
                        }`}
                      >
                        <div className={`w-4 h-4 rounded-full border-2 flex-shrink-0 ${!form.showroomId ? 'border-slate-400' : 'border-slate-300'}`} />
                        <span className="italic">Chưa chọn showroom</span>
                      </div>

                      {showroomList.map((s) => {
                        const isSelected = form.showroomId === s.showroomId
                        return (
                          <div
                            key={s.showroomId}
                            onClick={() => setForm(f => ({ ...f, showroomId: s.showroomId }))}
                            className={`flex items-start gap-3 px-3 py-2.5 rounded-lg border cursor-pointer transition-all select-none ${
                              isSelected
                                ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                                : 'border-slate-200 hover:border-slate-300 hover:bg-slate-50 dark:border-zinc-800 dark:hover:bg-zinc-900'
                            }`}
                          >
                            {/* Radio */}
                            <div className={`mt-0.5 w-4 h-4 rounded-full border-2 flex items-center justify-center flex-shrink-0 ${
                              isSelected ? 'border-white bg-white' : 'border-slate-400'
                            }`}>
                              {isSelected && <div className="w-1.5 h-1.5 rounded-full bg-slate-900 dark:bg-zinc-900" />}
                            </div>

                            {/* Info */}
                            <div className="flex-1 min-w-0">
                              <div className="flex items-center justify-between gap-2">
                                <span className="font-semibold text-sm leading-tight">{s.name}</span>
                                <span className={`flex-shrink-0 text-xs px-1.5 py-0.5 rounded-full font-medium ${
                                  isSelected ? 'bg-white/20 text-white dark:bg-zinc-900/20 dark:text-zinc-900' : 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300'
                                }`}>
                                  {s.province}
                                </span>
                              </div>
                              <p className={`text-xs mt-0.5 truncate ${isSelected ? 'text-white/80 dark:text-zinc-900/80' : 'text-slate-500 dark:text-zinc-400'}`}>
                                {s.fullAddress}
                              </p>
                              {s.hotline && (
                                <p className={`text-xs mt-0.5 font-medium ${isSelected ? 'text-white/90 dark:text-zinc-900' : 'text-slate-600 dark:text-zinc-300'}`}>
                                  📞 {s.hotline}
                                </p>
                              )}
                            </div>
                          </div>
                        )
                      })}
                    </div>
                  )}
                </div>
              )}

              {/* Số lượng */}
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Số lượng <span className="text-red-500">*</span></label>
                <input
                  type="number"
                  value={String(form.quantity ?? 1)}
                  onChange={(e) => setForm(f => ({ ...f, quantity: Number(e.target.value) }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="1"
                />
              </div>

              {/* Mã khuyến mãi */}
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Mã khuyến mãi (không bắt buộc)</label>
                <input
                  type="number"
                  value={form.promotionId === null || form.promotionId === undefined ? '' : String(form.promotionId)}
                  onChange={(e) => setForm(f => ({ ...f, promotionId: e.target.value === '' ? null : Number(e.target.value) }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Ví dụ: 3"
                />
              </div>

              {/* Người dùng */}
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Người dùng (không bắt buộc)</label>
                <div className="mt-2 flex items-center gap-2">
                  <button type="button"
                    onClick={() => { setUserMode('select'); setForm(f => ({ ...f, phone: null })) }}
                    className={`h-10 rounded-lg border px-3 text-sm font-medium ${userMode === 'select' ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900' : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200'}`}
                  >Chọn người dùng</button>
                  <button type="button"
                    onClick={() => { setUserMode('phone'); setUserSearch(''); setForm(f => ({ ...f, userId: null })) }}
                    className={`h-10 rounded-lg border px-3 text-sm font-medium ${userMode === 'phone' ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900' : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200'}`}
                  >Nhập SĐT</button>
                </div>

                {userMode === 'select' ? (
                  <div className="mt-2 grid gap-2 md:grid-cols-[1fr_1fr]">
                    <input
                      value={userSearch}
                      onChange={(e) => setUserSearch(e.target.value)}
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="Tìm theo tên / SĐT / email..."
                    />
                    <select
                      value={form.userId === null || form.userId === undefined ? '' : String(form.userId)}
                      onChange={(e) => setForm(f => ({ ...f, userId: e.target.value === '' ? null : Number(e.target.value), phone: null }))}
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    >
                      <option value="">{usersQ.isLoading ? 'Đang tải...' : '-- Chọn người dùng --'}</option>
                      {userOptions.map((u) => (
                        <option key={u.userId} value={u.userId}>
                          {u.fullName ?? u.username ?? 'User'} · {u.phone ?? '—'} · #{u.userId}
                        </option>
                      ))}
                    </select>
                  </div>
                ) : (
                  <div className="mt-2">
                    <input
                      value={(form.phone ?? '').toString()}
                      onChange={(e) => setForm(f => ({ ...f, phone: e.target.value, userId: null }))}
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="09xxxxxxxx"
                    />
                    <p className="mt-1.5 text-xs text-slate-400">Hệ thống tự tìm người dùng theo SĐT. Nếu không có sẽ báo lỗi.</p>
                  </div>
                )}
              </div>

              {/* Địa chỉ giao hàng */}
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Địa chỉ giao hàng (không bắt buộc)</label>
                <input
                  value={form.shippingAddress ?? ''}
                  onChange={(e) => setForm(f => ({ ...f, shippingAddress: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Số nhà, đường, phường/xã..."
                />
              </div>

              {/* Phương thức thanh toán */}
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Phương thức thanh toán</label>
                <select
                  value={(form.paymentMethod ?? '').toString()}
                  onChange={(e) => setForm(f => ({ ...f, paymentMethod: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn phương thức --</option>
                  {PAYMENT_METHOD_OPTIONS.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
                </select>
              </div>

              {/* Trạng thái đơn */}
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái đơn hàng</label>
                <select
                  value={(form.status ?? '').toString()}
                  onChange={(e) => setForm(f => ({ ...f, status: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn trạng thái --</option>
                  {ORDER_STATUS_OPTIONS.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
                </select>
              </div>

              {/* Trạng thái thanh toán */}
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái thanh toán</label>
                <select
                  value={(form.paymentStatus ?? '').toString()}
                  onChange={(e) => setForm(f => ({ ...f, paymentStatus: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn trạng thái --</option>
                  {PAYMENT_STATUS_OPTIONS.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
                </select>
              </div>
            </div>

            {/* Footer */}
            <div className="flex flex-col-reverse gap-2 border-t border-slate-200 px-5 py-4 dark:border-zinc-800 sm:flex-row sm:justify-end">
              <button type="button" onClick={closeDialog} disabled={submitting}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-lg border border-slate-200 bg-white px-4 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
              >
                <X size={16} />Hủy
              </button>
              <button type="button" onClick={submit} disabled={submitting}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-lg bg-slate-900 px-4 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
              >
                <Save size={16} />
                {submitting ? 'Đang lưu...' : 'Lưu'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* ── Order Detail Modal ── */}
      {selectedOrderId !== null && (
        <OrderDetailModal
          orderId={selectedOrderId}
          onClose={() => setSelectedOrderId(null)}
        />
      )}
    </div>
  )
}