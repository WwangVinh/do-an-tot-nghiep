import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { X } from 'lucide-react'
import {
  fetchOrderDetail,
  updateOrderStatus,
  addPaymentTransaction,
  type AdminOrderDetail,
} from '../../services/orders/orders'

// ─── Constants ────────────────────────────────────────────────────────────────

const STATUS_FLOW: Record<string, { next: string[]; label: string }> = {
  Pending:   { next: ['Confirmed', 'Cancelled'], label: 'Chờ xử lý'   },
  Confirmed: { next: ['Completed', 'Cancelled'], label: 'Đã xác nhận' },
  Completed: { next: [],                          label: 'Hoàn tất'    },
  Cancelled: { next: [],                          label: 'Đã hủy'      },
}

const STATUS_COLOR: Record<string, string> = {
  Pending:   'text-yellow-600',
  Confirmed: 'text-blue-600',
  Completed: 'text-emerald-600',
  Cancelled: 'text-red-500',
}

const PAYMENT_STATUS_COLOR: Record<string, string> = {
  Unpaid:    'text-rose-600',
  Deposited: 'text-amber-600',
  Paid:      'text-emerald-600',
}

const PAYMENT_STATUS_LABEL: Record<string, string> = {
  Unpaid: 'Chưa thanh toán', Deposited: 'Đã cọc', Paid: 'Đã thanh toán đủ',
}

const PAYMENT_METHODS = ['Tiền mặt', 'Chuyển khoản', 'Thẻ tín dụng', 'MoMo', 'ZaloPay', 'VNPay']

const TIMELINE_STEPS = ['Pending', 'Confirmed', 'Completed']
const STATUS_LABEL: Record<string, string> = {
  Pending: 'Chờ xử lý', Confirmed: 'Đã xác nhận', Completed: 'Hoàn tất', Cancelled: 'Đã hủy',
}

function fmtMoney(n: number | null | undefined) {
  if (!n && n !== 0) return '—'
  return new Intl.NumberFormat('vi-VN').format(n) + ' đ'
}
function fmtDate(s: string | null | undefined) {
  if (!s) return '—'
  return new Date(s).toLocaleString('vi-VN')
}
function getErrorMessage(err: unknown): string {
  const e = err as { response?: { data?: { message?: string } }; message?: string }
  return e?.response?.data?.message ?? e?.message ?? 'Có lỗi xảy ra'
}

// ─── Timeline ─────────────────────────────────────────────────────────────────

function OrderTimeline({ status }: { status: string | null | undefined }) {
  const isCancelled = status === 'Cancelled'
  const currentIdx  = TIMELINE_STEPS.indexOf(status ?? '')

  if (isCancelled) {
    return (
      <div className="mb-5 flex items-center gap-2 rounded-lg border border-red-200 bg-red-50 px-4 py-2.5">
        <span className="text-red-500 font-semibold text-sm">🚫 Đơn hàng đã bị hủy</span>
      </div>
    )
  }

  return (
    <div className="mb-5 flex items-start">
      {TIMELINE_STEPS.map((step, idx) => {
        const done    = idx < currentIdx
        const current = idx === currentIdx
        return (
          <div key={step} className="flex-1 flex flex-col items-center">
            <div className="flex items-center w-full">
              {idx > 0 && <div className={`flex-1 h-0.5 ${done || current ? 'bg-blue-400' : 'bg-slate-200'}`} />}
              <div className={`w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold border-2 flex-shrink-0 ${
                current ? 'border-blue-500 bg-blue-500 text-white' :
                done    ? 'border-blue-400 bg-blue-400 text-white' :
                          'border-slate-300 bg-white text-slate-400'
              }`}>
                {done
                  ? <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7"/></svg>
                  : idx + 1
                }
              </div>
              {idx < TIMELINE_STEPS.length - 1 && <div className={`flex-1 h-0.5 ${done ? 'bg-blue-400' : 'bg-slate-200'}`} />}
            </div>
            <span className={`mt-1.5 text-xs font-medium ${current ? 'text-blue-600' : done ? 'text-blue-400' : 'text-slate-400'}`}>
              {STATUS_LABEL[step]}
            </span>
          </div>
        )
      })}
    </div>
  )
}

// ─── Tab: Chi tiết ────────────────────────────────────────────────────────────

function TabDetail({ order }: { order: AdminOrderDetail }) {
  return (
    <div className="space-y-5">
      {/* Grid thông tin */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Khách hàng</p>
          <p className="font-semibold text-slate-800">{order.fullName}</p>
        </div>
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Xe</p>
          <p className="font-semibold text-slate-800">{order.carName ?? '—'}</p>
        </div>
        <div>
          <p className="text-xs text-slate-400 mb-0.5">SĐT</p>
          <p className="font-medium text-slate-700">{order.phone}</p>
        </div>
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Showroom</p>
          <p className="font-medium text-slate-700">{order.showroomName ?? '—'}</p>
        </div>
        {order.email && (
          <div>
            <p className="text-xs text-slate-400 mb-0.5">Email</p>
            <p className="font-medium text-slate-700">{order.email}</p>
          </div>
        )}
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Nhân viên phụ trách</p>
          <p className="font-medium text-slate-700">{order.staffName ?? '—'}</p>
        </div>
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Ngày đặt</p>
          <p className="font-medium text-slate-700">{fmtDate(order.orderDate)}</p>
        </div>
        <div>
          <p className="text-xs text-slate-400 mb-0.5">Cập nhật lần cuối</p>
          <p className="font-medium text-slate-700">{fmtDate(order.lastUpdated)}</p>
        </div>
      </div>

      {/* Ghi chú */}
      {order.customerNote && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 px-3 py-2.5">
          <p className="text-xs font-semibold text-amber-600 mb-0.5">Ghi chú của khách</p>
          <p className="text-sm text-amber-800">{order.customerNote}</p>
        </div>
      )}
      {order.adminNote && (
        <div className="rounded-lg border border-blue-200 bg-blue-50 px-3 py-2.5">
          <p className="text-xs font-semibold text-blue-600 mb-0.5">Ghi chú nội bộ</p>
          <p className="text-sm text-blue-800">{order.adminNote}</p>
        </div>
      )}

      {/* Danh sách items */}
      <div>
        <p className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">Chi tiết đơn hàng</p>
        <div className="divide-y divide-slate-100 rounded-lg border border-slate-200 overflow-hidden">
          {order.items.map((item) => (
            <div key={item.orderItemId} className="flex items-center justify-between px-4 py-2.5">
              <div className="flex items-center gap-2.5">
                <span className={`text-xs rounded-full px-2 py-0.5 font-medium ${
                  item.itemType === 'Car' ? 'bg-blue-100 text-blue-600' : 'bg-purple-100 text-purple-600'
                }`}>
                  {item.itemType === 'Car' ? '🚗 Xe' : '🔧 Phụ kiện'}
                </span>
                <span className="text-sm text-slate-800">{item.name}</span>
              </div>
              <span className="text-sm font-semibold text-slate-700">{fmtMoney(item.price)}</span>
            </div>
          ))}
        </div>

        {/* Tổng */}
        <div className="mt-2 space-y-1 px-1">
          {order.discountAmount > 0 && (
            <>
              <div className="flex justify-between text-sm text-slate-500">
                <span>Tạm tính</span><span>{fmtMoney(order.subtotal)}</span>
              </div>
              <div className="flex justify-between text-sm text-emerald-600 font-medium">
                <span>Giảm giá</span><span>− {fmtMoney(order.discountAmount)}</span>
              </div>
            </>
          )}
          <div className="flex justify-between text-base font-bold text-slate-900 pt-1.5 border-t border-slate-200">
            <span>Tổng thanh toán</span>
            <span className="text-blue-700">{fmtMoney(order.finalAmount)}</span>
          </div>
        </div>
      </div>
    </div>
  )
}

// ─── Tab: Cập nhật trạng thái ─────────────────────────────────────────────────

function TabUpdateStatus({ order, onDone }: { order: AdminOrderDetail; onDone: () => void }) {
  const qc = useQueryClient()
  const nextSteps = STATUS_FLOW[order.status ?? '']?.next ?? []
  const [selected, setSelected] = useState(nextSteps[0] ?? '')
  const [note, setNote]         = useState(order.adminNote ?? '')

  const mut = useMutation({
    mutationFn: () => updateOrderStatus(order.orderId, selected, note),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Cập nhật thành công')
      await qc.invalidateQueries({ queryKey: ['admin-orders'] })
      await qc.invalidateQueries({ queryKey: ['order-detail', order.orderId] })
      onDone()
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  if (nextSteps.length === 0) {
    return (
      <div className="py-10 text-center text-slate-400">
        <p className="text-3xl mb-2">{order.status === 'Completed' ? '✅' : '🚫'}</p>
        <p className="text-sm font-medium">
          {order.status === 'Completed' ? 'Đơn hàng đã hoàn tất' : 'Đơn hàng đã bị hủy'}
        </p>
        <p className="text-xs mt-1">Không thể thay đổi trạng thái</p>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <div>
        <p className="text-sm font-semibold text-slate-700 mb-2">Chuyển sang trạng thái</p>
        <div className="flex flex-col gap-2">
          {nextSteps.map((s) => (
            <label key={s} className={`flex items-center gap-3 rounded-lg border-2 px-4 py-3 cursor-pointer transition-all ${
              selected === s ? 'border-blue-500 bg-blue-50' : 'border-slate-200 hover:border-slate-300'
            }`}>
              <input
                type="radio"
                name="status"
                value={s}
                checked={selected === s}
                onChange={() => setSelected(s)}
                className="accent-blue-500"
              />
              <span className={`text-sm font-semibold ${selected === s ? 'text-blue-700' : 'text-slate-600'}`}>
                {STATUS_LABEL[s] ?? s}
              </span>
              {s === 'Cancelled' && (
                <span className="ml-auto text-xs text-red-400">⚠️ Không thể hoàn tác</span>
              )}
            </label>
          ))}
        </div>
      </div>

      <div>
        <p className="text-sm font-semibold text-slate-700 mb-1.5">
          Ghi nhận kết quả xử lý
          <span className="ml-1 text-xs font-normal text-slate-400">(nhân viên xem, khách không thấy)</span>
        </p>
        <textarea
          rows={3}
          value={note}
          onChange={(e) => setNote(e.target.value)}
          placeholder="Ví dụ: Đã liên hệ xác nhận, dự kiến giao xe ngày 10/5..."
          className="w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:border-blue-400 resize-none"
        />
      </div>

      <button
        type="button"
        disabled={mut.isPending || !selected}
        onClick={() => mut.mutate()}
        className="w-full rounded-lg bg-blue-600 py-2.5 text-sm font-bold text-white hover:bg-blue-700 disabled:opacity-40 transition"
      >
        {mut.isPending ? 'Đang cập nhật...' : `Xác nhận đã ${STATUS_LABEL[selected] ?? selected} →`}
      </button>
    </div>
  )
}

// ─── Tab: Thanh toán ──────────────────────────────────────────────────────────

function TabPayment({ order, onDone }: { order: AdminOrderDetail; onDone: () => void }) {
  const qc = useQueryClient()
  const [amount, setAmount] = useState('')
  const [method, setMethod] = useState('Chuyển khoản')
  const [txStatus, setTxStatus] = useState('Success')

  const totalPaid = order.payments
    .filter((p) => p.status === 'Success')
    .reduce((s, p) => s + (p.amount ?? 0), 0)
  const remaining = Math.max(0, order.finalAmount - totalPaid)

  const mut = useMutation({
    mutationFn: () => addPaymentTransaction(order.orderId, {
      amount: Number(amount), paymentMethod: method, status: txStatus,
    }),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Ghi nhận thanh toán thành công')
      setAmount('')
      await qc.invalidateQueries({ queryKey: ['admin-orders'] })
      await qc.invalidateQueries({ queryKey: ['order-detail', order.orderId] })
      onDone()
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <div className="space-y-4">
      {/* Tóm tắt */}
      <div className="grid grid-cols-3 gap-3 text-center">
        {[
          { label: 'Tổng đơn',     value: fmtMoney(order.finalAmount), cls: 'text-slate-800' },
          { label: 'Đã thanh toán', value: fmtMoney(totalPaid),         cls: 'text-emerald-600' },
          { label: 'Còn lại',       value: fmtMoney(remaining),         cls: remaining > 0 ? 'text-rose-600' : 'text-emerald-600' },
        ].map(({ label, value, cls }) => (
          <div key={label} className="rounded-lg border border-slate-100 bg-slate-50 p-3">
            <p className="text-xs text-slate-400 mb-1">{label}</p>
            <p className={`text-sm font-black ${cls}`}>{value}</p>
          </div>
        ))}
      </div>

      {/* Lịch sử */}
      {order.payments.length > 0 && (
        <div>
          <p className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">Lịch sử thanh toán</p>
          <div className="divide-y divide-slate-100 rounded-lg border border-slate-200 max-h-32 overflow-y-auto">
            {order.payments.map((p) => (
              <div key={p.transactionId} className="flex items-center justify-between px-4 py-2.5">
                <div>
                  <p className="text-sm font-semibold text-slate-800">{fmtMoney(p.amount)}</p>
                  <p className="text-xs text-slate-400">{p.paymentMethod} · {fmtDate(p.transactionDate)}</p>
                </div>
                <span className={`text-xs font-semibold rounded-full px-2 py-0.5 ${
                  p.status === 'Success' ? 'bg-emerald-100 text-emerald-700' : 'bg-rose-100 text-rose-700'
                }`}>
                  {p.status === 'Success' ? 'Thành công' : p.status}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Form */}
      <div className="space-y-3">
        <p className="text-sm font-semibold text-slate-700">Ghi nhận thanh toán mới</p>

        <div>
          <label className="text-xs text-slate-500 mb-1 block">Số tiền <span className="text-red-500">*</span></label>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            placeholder="Nhập số tiền..."
            className="w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm focus:outline-none focus:border-blue-400"
          />
          {/* Shortcut */}
          <div className="mt-2 flex gap-2 flex-wrap">
            {[
              { label: 'Cọc 10%',  val: Math.round(order.finalAmount * 0.1) },
              { label: 'Cọc 30%',  val: Math.round(order.finalAmount * 0.3) },
              { label: 'Còn lại',  val: remaining },
              { label: 'Toàn bộ',  val: order.finalAmount },
            ].map(({ label, val }) => (
              <button key={label} type="button"
                onClick={() => setAmount(String(val))}
                className="rounded border border-slate-200 bg-slate-50 px-2.5 py-1 text-xs text-slate-600 hover:bg-slate-100 transition"
              >
                {label} ({new Intl.NumberFormat('vi-VN', { notation: 'compact' }).format(val)}đ)
              </button>
            ))}
          </div>
        </div>

        <div>
          <label className="text-xs text-slate-500 mb-1 block">Phương thức</label>
          <div className="flex flex-wrap gap-2">
            {PAYMENT_METHODS.map((m) => (
              <button key={m} type="button" onClick={() => setMethod(m)}
                className={`rounded-lg border px-3 py-1.5 text-xs font-medium transition ${
                  method === m ? 'border-blue-500 bg-blue-50 text-blue-700' : 'border-slate-200 text-slate-600 hover:border-slate-300'
                }`}
              >
                {m}
              </button>
            ))}
          </div>
        </div>

        <div className="flex gap-2">
          {[{ v: 'Success', label: '✅ Thành công' }, { v: 'Failed', label: '❌ Thất bại' }].map(({ v, label }) => (
            <button key={v} type="button" onClick={() => setTxStatus(v)}
              className={`flex-1 rounded-lg border py-2 text-xs font-semibold transition ${
                txStatus === v
                  ? v === 'Success' ? 'border-emerald-400 bg-emerald-50 text-emerald-700' : 'border-red-400 bg-red-50 text-red-700'
                  : 'border-slate-200 text-slate-500 hover:border-slate-300'
              }`}
            >
              {label}
            </button>
          ))}
        </div>

        <button
          type="button"
          disabled={mut.isPending || !amount || Number(amount) <= 0}
          onClick={() => mut.mutate()}
          className="w-full rounded-lg bg-emerald-600 py-2.5 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-40 transition"
        >
          {mut.isPending ? 'Đang ghi nhận...' : '💰 Xác nhận thanh toán'}
        </button>
      </div>
    </div>
  )
}

// ─── Main Modal ───────────────────────────────────────────────────────────────

type Tab = 'detail' | 'status' | 'payment'

interface Props {
  orderId: number
  onClose: () => void
}

export function OrderDetailModal({ orderId, onClose }: Props) {
  const [tab, setTab] = useState<Tab>('detail')

  const detailQ = useQuery({
    queryKey: ['order-detail', orderId],
    queryFn:  () => fetchOrderDetail(orderId),
  })

  const order = detailQ.data

  const TABS: { key: Tab; label: string }[] = [
    { key: 'detail',  label: '📋 Chi tiết'   },
    { key: 'status',  label: '🔄 Trạng thái' },
    { key: 'payment', label: '💳 Thanh toán' },
  ]

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-lg overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl flex flex-col max-h-[90vh]">

        {/* ── Header — style giống trang Booking ── */}
        <div className="flex items-start justify-between gap-3 px-6 py-5 border-b border-slate-100">
          <div>
            <h2 className="text-lg font-bold text-slate-900">
              Chi tiết đơn hàng {order?.orderCode ? `· ${order.orderCode}` : `#${orderId}`}
            </h2>
            {order && (
              <div className="flex items-center gap-3 mt-1">
                <span className={`text-sm font-semibold ${STATUS_COLOR[order.status ?? ''] ?? 'text-slate-500'}`}>
                  {STATUS_LABEL[order.status ?? ''] ?? order.status}
                </span>
                <span className="text-slate-300">·</span>
                <span className={`text-sm font-medium ${PAYMENT_STATUS_COLOR[order.paymentStatus ?? ''] ?? 'text-slate-400'}`}>
                  {PAYMENT_STATUS_LABEL[order.paymentStatus ?? ''] ?? order.paymentStatus}
                </span>
              </div>
            )}
          </div>
          <button type="button" onClick={onClose}
            className="rounded-lg p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition flex-shrink-0"
          >
            <X size={18} />
          </button>
        </div>

        {/* ── Body ── */}
        <div className="overflow-y-auto flex-1 px-6 py-4">
          {detailQ.isLoading && (
            <div className="space-y-3">
              {[80, 60, 90, 50].map((w, i) => (
                <div key={i} className={`h-4 rounded animate-pulse bg-slate-100`} style={{ width: `${w}%` }} />
              ))}
            </div>
          )}

          {detailQ.isError && (
            <div className="rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">
              Không thể tải chi tiết đơn hàng.
            </div>
          )}

          {order && (
            <>
              {/* Timeline */}
              <OrderTimeline status={order.status} />

              {/* Tabs */}
              <div className="flex border-b border-slate-100 mb-5">
                {TABS.map((t) => (
                  <button key={t.key} type="button" onClick={() => setTab(t.key)}
                    className={`relative pb-3 pr-5 text-sm font-semibold transition ${
                      tab === t.key ? 'text-blue-600' : 'text-slate-400 hover:text-slate-600'
                    }`}
                  >
                    {t.label}
                    {tab === t.key && (
                      <span className="absolute bottom-0 left-0 right-5 h-0.5 bg-blue-500 rounded-full" />
                    )}
                  </button>
                ))}
              </div>

              {tab === 'detail'  && <TabDetail order={order} />}
              {tab === 'status'  && <TabUpdateStatus order={order} onDone={() => setTab('detail')} />}
              {tab === 'payment' && <TabPayment order={order} onDone={() => setTab('detail')} />}
            </>
          )}
        </div>
      </div>
    </div>
  )
}