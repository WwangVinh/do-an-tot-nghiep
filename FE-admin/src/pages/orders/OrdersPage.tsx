import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Plus, RefreshCcw, Save, X } from 'lucide-react'

import type { AdminCarListItem, FetchAdminCarsParams } from '../../services/cars/cars'
import { fetchAdminCars } from '../../services/cars/cars'
import type { CreateAdminOrderInput, FetchAdminOrdersParams } from '../../services/orders/orders'
import { createAdminOrder, fetchAdminOrders } from '../../services/orders/orders'
import type { AdminUserListItem, FetchAdminUsersParams } from '../../services/users/users'
import { fetchAdminUsers } from '../../services/users/users'

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
  { value: 'Unpaid', label: 'Chưa thanh toán' },
  { value: 'Paid', label: 'Đã thanh toán' },
  { value: 'Refunded', label: 'Đã hoàn tiền' },
  { value: 'Failed', label: 'Thất bại' },
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
  }
}

function fmtMoney(v: unknown): string {
  const n = typeof v === 'number' ? v : Number(v)
  if (!Number.isFinite(n)) return '0'
  return n.toLocaleString('vi-VN')
}

export function OrdersPage() {
  const qc = useQueryClient()
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [paymentStatus, setPaymentStatus] = useState('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  const [dialogOpen, setDialogOpen] = useState(false)
  const [form, setForm] = useState<CreateAdminOrderInput>(() => emptyForm())
  const [carSearch, setCarSearch] = useState('')
  const [userMode, setUserMode] = useState<'select' | 'phone'>('select')
  const [userSearch, setUserSearch] = useState('')

  const params = useMemo<FetchAdminOrdersParams>(() => {
    return {
      search: search.trim() || undefined,
      status: status.trim() ? status.trim() : undefined,
      paymentStatus: paymentStatus.trim() ? paymentStatus.trim() : undefined,
      page,
      pageSize,
    }
  }, [page, pageSize, paymentStatus, search, status])

  const qKey = useMemo(() => ['admin-orders', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminOrders(params),
  })

  const carParams = useMemo<FetchAdminCarsParams>(() => {
    return {
      search: carSearch.trim() || undefined,
      page: 1,
      pageSize: 50,
      isDeleted: false,
    }
  }, [carSearch])

  const carsQ = useQuery({
    queryKey: ['admin-cars', carParams],
    queryFn: () => fetchAdminCars(carParams),
    enabled: dialogOpen,
  })

  const userParams = useMemo<FetchAdminUsersParams>(() => {
    return {
      userType: 'Customer',
      isDeleted: false,
      search: userSearch.trim() || undefined,
      page: 1,
      pageSize: 50,
    }
  }, [userSearch])

  const usersQ = useQuery({
    queryKey: ['admin-users', userParams],
    queryFn: () => fetchAdminUsers(userParams),
    enabled: dialogOpen && userMode === 'select',
  })

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
    setDialogOpen(true)
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
    setForm(emptyForm())
    setCarSearch('')
    setUserMode('select')
    setUserSearch('')
  }

  function submit() {
    const carId = Number(form.carId)
    const quantity = Number(form.quantity)

    const userId =
      form.userId === undefined || form.userId === null || String(form.userId).trim() === '' ? null : Number(form.userId)
    const promotionId =
      form.promotionId === undefined || form.promotionId === null || String(form.promotionId).trim() === ''
        ? null
        : Number(form.promotionId)

    if (!Number.isFinite(carId) || carId <= 0) {
      toast.error('Vui lòng chọn xe')
      return
    }
    if (!Number.isFinite(quantity) || quantity <= 0) {
      toast.error('Vui lòng nhập Quantity hợp lệ')
      return
    }

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
            title="Tải lại"
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

      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-6">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input
              value={search}
              onChange={(e) => {
                setSearch(e.target.value)
                setPage(1)
              }}
              placeholder="Mã đơn / Mã người dùng / Mã xe..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>

          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
            <input
              value={status}
              onChange={(e) => {
                setStatus(e.target.value)
                setPage(1)
              }}
              placeholder="Ví dụ: Pending / Completed..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>

          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái thanh toán</label>
            <input
              value={paymentStatus}
              onChange={(e) => {
                setPaymentStatus(e.target.value)
                setPage(1)
              }}
              placeholder="Ví dụ: Unpaid / Paid..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>
        </div>

        <div className="mt-4 overflow-auto">
          <table className="min-w-[1050px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3">Mã</th>
                <th className="py-3 pr-3">User</th>
                <th className="py-3 pr-3">Car</th>
                <th className="py-3 pr-3">Ngày</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3">Thanh toán</th>
                <th className="py-3 pr-3 text-right">Tổng</th>
                <th className="py-3 pr-3">Địa chỉ</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading ? (
                <tr>
                  <td colSpan={8} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Đang tải...
                  </td>
                </tr>
              ) : null}
              {listQ.isError ? (
                <tr>
                  <td colSpan={8} className="py-4 text-sm text-rose-600">
                    {getErrorMessage(listQ.error)}
                  </td>
                </tr>
              ) : null}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 ? (
                <tr>
                  <td colSpan={8} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Không có dữ liệu.
                  </td>
                </tr>
              ) : null}

              {rows.map((r) => (
                <tr key={r.orderId} className="border-b border-slate-100 dark:border-zinc-900">
                  <td className="py-3 pr-3">
                    <div className="font-medium text-slate-900 dark:text-zinc-50">{r.orderCode ?? `#${r.orderId}`}</div>
                    <div className="text-xs text-slate-500 dark:text-zinc-400">ID: {r.orderId}</div>
                  </td>
                  <td className="py-3 pr-3">
                    {r.userName ? (
                      <div>
                        <div className="font-medium">{r.userName}</div>
                        <div className="text-xs text-slate-500 dark:text-zinc-400">#{r.userId ?? '—'}</div>
                      </div>
                    ) : (
                      (r.userId ?? '—')
                    )}
                  </td>
                  <td className="py-3 pr-3">
                    {r.carName ? (
                      <div>
                        <div className="font-medium">{r.carName}</div>
                        <div className="text-xs text-slate-500 dark:text-zinc-400">#{r.carId ?? '—'}</div>
                      </div>
                    ) : (
                      (r.carId ?? '—')
                    )}
                  </td>
                  <td className="py-3 pr-3 text-xs text-slate-600 dark:text-zinc-300">
                    {r.orderDate ? new Date(r.orderDate).toLocaleString() : '—'}
                  </td>
                  <td className="py-3 pr-3">{r.status ?? '—'}</td>
                  <td className="py-3 pr-3">{r.paymentStatus ?? '—'}</td>
                  <td className="py-3 pr-3 text-right">{fmtMoney(r.finalAmount ?? r.totalAmount ?? 0)}</td>
                  <td className="py-3 pr-3">
                    <div className="max-w-[320px] truncate" title={r.shippingAddress ?? ''}>
                      {r.shippingAddress ?? '—'}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {listQ.data?.currentPage ?? page}/{totalPages} · {listQ.data?.totalItems ?? rows.length} đơn hàng
          </div>

          <div className="flex items-center gap-2">
            <select
              value={String(pageSize)}
              onChange={(e) => {
                setPageSize(Number(e.target.value))
                setPage(1)
              }}
              className="rounded-md border border-slate-200 bg-white px-2 py-1.5 text-xs text-slate-700 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >
              {[10, 20, 50].map((n) => (
                <option key={n} value={n}>
                  {n}/trang
                </option>
              ))}
            </select>

            <button
              type="button"
              disabled={page <= 1}
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
            >
              ← Trước
            </button>
            <button
              type="button"
              disabled={page >= totalPages}
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
            >
              Sau →
            </button>
          </div>
        </div>
      </div>

      {dialogOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between gap-3 border-b border-slate-200 px-5 py-4 dark:border-zinc-800">
              <div>
                <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">Thêm mới đơn hàng</div>
              </div>
              <button
                type="button"
                onClick={closeDialog}
                className="rounded-md p-2 text-slate-600 hover:bg-slate-100 dark:text-zinc-300 dark:hover:bg-zinc-900"
                aria-label="Close"
                disabled={submitting}
              >
                <X size={18} />
              </button>
            </div>

            <div className="grid gap-4 p-5 md:grid-cols-2">
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Chọn xe *</label>
                <div className="mt-2 grid gap-2 md:grid-cols-[1fr_1fr]">
                  <input
                    value={carSearch}
                    onChange={(e) => setCarSearch(e.target.value)}
                    className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    placeholder="Tìm theo tên xe / hãng..."
                  />
                  <select
                    value={String(form.carId ?? 0)}
                    onChange={(e) => setForm((f) => ({ ...f, carId: Number(e.target.value) }))}
                    className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  >
                    <option value="0">
                      {carsQ.isLoading
                        ? 'Đang tải danh sách xe...'
                        : carsQ.isError
                          ? 'Không tải được danh sách xe'
                          : '-- Chọn xe --'}
                    </option>
                    {carOptions.map((c) => (
                      <option key={c.carId} value={c.carId}>
                        {c.brand} {c.name} ({c.year}) · Mã {c.carId}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="mt-2 text-xs text-slate-500 dark:text-zinc-300">
                  Gợi ý: dùng ô tìm kiếm để lọc nhanh, rồi chọn xe trong dropdown.
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Số lượng *</label>
                <input
                  type="number"
                  value={String(form.quantity ?? 1)}
                  onChange={(e) => setForm((f) => ({ ...f, quantity: Number(e.target.value) }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="1"
                />
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Người dùng (không bắt buộc)</label>
                <div className="mt-2 flex items-center gap-2">
                  <button
                    type="button"
                    onClick={() => {
                      setUserMode('select')
                      setForm((f) => ({ ...f, phone: null }))
                    }}
                    className={[
                      'h-10 rounded-lg border px-3 text-sm font-medium',
                      userMode === 'select'
                        ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                        : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
                    ].join(' ')}
                  >
                    Chọn người dùng
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setUserMode('phone')
                      setUserSearch('')
                      setForm((f) => ({ ...f, userId: null }))
                    }}
                    className={[
                      'h-10 rounded-lg border px-3 text-sm font-medium',
                      userMode === 'phone'
                        ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                        : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
                    ].join(' ')}
                  >
                    Nhập SĐT
                  </button>
                </div>

                {userMode === 'select' ? (
                  <div className="mt-2 grid gap-2 md:grid-cols-[1fr_1fr]">
                    <input
                      value={userSearch}
                      onChange={(e) => setUserSearch(e.target.value)}
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="Tìm theo tên / SĐT / email..."
                    />
                    <select
                      value={form.userId === null || form.userId === undefined ? '' : String(form.userId)}
                      onChange={(e) =>
                        setForm((f) => ({
                          ...f,
                          userId: e.target.value === '' ? null : Number(e.target.value),
                          phone: null,
                        }))
                      }
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    >
                      <option value="">
                        {usersQ.isLoading
                          ? 'Đang tải danh sách người dùng...'
                          : usersQ.isError
                            ? 'Không tải được người dùng'
                            : '-- Chọn người dùng --'}
                      </option>
                      {userOptions.map((u) => (
                        <option key={u.userId} value={u.userId}>
                          {(u.fullName ?? u.username ?? 'User')} · {u.phone ?? '—'} · Mã {u.userId}
                        </option>
                      ))}
                    </select>
                  </div>
                ) : (
                  <div className="mt-2">
                    <input
                      value={(form.phone ?? '').toString()}
                      onChange={(e) => setForm((f) => ({ ...f, phone: e.target.value, userId: null }))}
                      className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="Nhập số điện thoại (vd: 09xxxxxxx)"
                    />
                    <div className="mt-2 text-xs text-slate-500 dark:text-zinc-300">
                      Hệ thống sẽ tự tìm người dùng theo SĐT. Nếu không tìm thấy sẽ báo lỗi.
                    </div>
                  </div>
                )}
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Mã khuyến mãi (không bắt buộc)</label>
                <input
                  type="number"
                  value={form.promotionId === null || form.promotionId === undefined ? '' : String(form.promotionId)}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, promotionId: e.target.value === '' ? null : Number(e.target.value) }))
                  }
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Ví dụ: 3 (nếu có)"
                />
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Địa chỉ giao hàng (không bắt buộc)</label>
                <input
                  value={form.shippingAddress ?? ''}
                  onChange={(e) => setForm((f) => ({ ...f, shippingAddress: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Số nhà, đường, phường/xã..."
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Phương thức thanh toán (không bắt buộc)</label>
                <select
                  value={(form.paymentMethod ?? '').toString()}
                  onChange={(e) => setForm((f) => ({ ...f, paymentMethod: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn phương thức --</option>
                  {PAYMENT_METHOD_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái đơn hàng (không bắt buộc)</label>
                <select
                  value={(form.status ?? '').toString()}
                  onChange={(e) => setForm((f) => ({ ...f, status: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn trạng thái --</option>
                  {ORDER_STATUS_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái thanh toán (không bắt buộc)</label>
                <select
                  value={(form.paymentStatus ?? '').toString()}
                  onChange={(e) => setForm((f) => ({ ...f, paymentStatus: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                >
                  <option value="">-- Chọn trạng thái --</option>
                  {PAYMENT_STATUS_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="flex flex-col-reverse gap-2 border-t border-slate-200 px-5 py-4 dark:border-zinc-800 sm:flex-row sm:justify-end">
              <button
                type="button"
                onClick={closeDialog}
                disabled={submitting}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-lg border border-slate-200 bg-white px-4 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
              >
                <X size={16} />
                Hủy
              </button>
              <button
                type="button"
                onClick={submit}
                disabled={submitting}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-lg bg-slate-900 px-4 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
              >
                <Save size={16} />
                {submitting ? 'Đang lưu...' : 'Lưu'}
              </button>
            </div>
          </div>
        </div>
      ) : null}
    </div>
  )
}

