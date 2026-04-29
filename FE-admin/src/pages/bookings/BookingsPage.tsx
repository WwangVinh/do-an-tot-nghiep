import { useMemo, useState } from 'react'
import { useQuery, useQueryClient } from '@tanstack/react-query'

import {
  cancelBookingByAdmin,
  completeBooking,
  confirmBooking,
  consultBooking,
  fetchAdminBookingDetail,
  fetchAdminBookings,
  fetchBookingStats,
  markNoShow,
  sendToTechCheck,
  submitTechResult,
  type AdminBookingDetail,
  type AdminBookingListItem,
  type BookingStatus,
  type FetchAdminBookingsParams,
} from '../../services/bookings/bookings'
import { isInRole, type AdminRole } from '../../app/auth/roles'
import { useAuth } from '../../app/auth/useAuth'

// ─── helpers ────────────────────────────────────────────────────────────────

function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const e = err as { message?: unknown; response?: { data?: unknown } }
    const data = e.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (typeof e.message === 'string' && e.message.trim()) return e.message
  }
  return 'Có lỗi xảy ra'
}

function formatDateVN(raw: string | null | undefined): string {
  const s = (raw ?? '').trim()
  if (!s) return '-'
  const d = new Date(s)
  if (Number.isNaN(d.getTime())) return s
  return d.toLocaleDateString('vi-VN')
}

const STATUS_LABEL: Record<string, string> = {
  Scheduled: 'Chờ xử lý',
  Pending: 'Chờ tư vấn',
  Consulted: 'Đã tư vấn',
  PendingTechCheck: 'Đang kiểm tra xe',
  TechApproved: 'Xe sẵn sàng',
  Confirmed: 'Đã xác nhận',
  Completed: 'Hoàn thành',
  NoShow: 'Không đến',
  Cancelled: 'Đã hủy',
}

const STATUS_CLASS: Record<string, string> = {
  Scheduled: 'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-200',
  Pending: 'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-200',
  Consulted: 'bg-blue-50 text-blue-700 dark:bg-blue-950/40 dark:text-blue-200',
  PendingTechCheck: 'bg-orange-50 text-orange-700 dark:bg-orange-950/40 dark:text-orange-200',
  TechApproved: 'bg-teal-50 text-teal-700 dark:bg-teal-950/40 dark:text-teal-200',
  Confirmed: 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-200',
  Completed: 'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-200',
  NoShow: 'bg-slate-100 text-slate-600 dark:bg-zinc-900 dark:text-zinc-400',
  Cancelled: 'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-200',
}

function StatusBadge({ status }: { status: string | null | undefined }) {
  const v = (status ?? '').trim()
  if (!v) return null
  const cls = STATUS_CLASS[v] ?? 'bg-slate-100 text-slate-700 dark:bg-zinc-900 dark:text-zinc-200'
  return (
    <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${cls}`}>
      {STATUS_LABEL[v] ?? v}
    </span>
  )
}

// Sales, ShowroomSales, Manager, Admin đều có thể thực hiện workflow chính
const SALES_SIDE: readonly AdminRole[] = ['Admin', 'Manager', 'Sales', 'ShowroomSales']
// Kỹ thuật + Admin duyệt xe
const TECH_SIDE: readonly AdminRole[] = ['Admin', 'Technician']

// ─── Stats bar (Manager + Admin) ─────────────────────────────────────────────
// Hiện số lượng theo trạng thái; Cancelled/NoShow được highlight để cảnh báo

interface StatsBarProps {
  onFilterByStatus: (s: BookingStatus) => void
}

function StatsBar({ onFilterByStatus }: StatsBarProps) {
  const statsQ = useQuery({
    queryKey: ['admin-booking-stats'],
    queryFn: fetchBookingStats,
    refetchInterval: 60_000,
  })

  if (!statsQ.data) return null

  const stats = statsQ.data
  const cancelled = stats['Cancelled'] ?? 0
  const noShow = stats['NoShow'] ?? 0
  const hasAlerts = cancelled > 0 || noShow > 0

  return (
    <div className="mt-4 space-y-2">
      {hasAlerts && (
        <div className="flex flex-wrap gap-2">
          {cancelled > 0 && (
            <button
              type="button"
              onClick={() => onFilterByStatus('Cancelled')}
              className="inline-flex items-center gap-1.5 rounded-md border border-rose-200 bg-rose-50 px-3 py-1.5 text-xs font-medium text-rose-700 hover:bg-rose-100 dark:border-rose-900/40 dark:bg-rose-950/30 dark:text-rose-300"
            >
              ⚠ {cancelled} lịch bị hủy
            </button>
          )}
          {noShow > 0 && (
            <button
              type="button"
              onClick={() => onFilterByStatus('NoShow')}
              className="inline-flex items-center gap-1.5 rounded-md border border-amber-200 bg-amber-50 px-3 py-1.5 text-xs font-medium text-amber-700 hover:bg-amber-100 dark:border-amber-900/40 dark:bg-amber-950/30 dark:text-amber-300"
            >
              ⚠ {noShow} khách không đến
            </button>
          )}
        </div>
      )}

      <div className="flex flex-wrap gap-2">
        {Object.entries(STATUS_LABEL).map(([s, label]) => {
          const count = stats[s] ?? 0
          if (count === 0) return null
          return (
            <button
              key={s}
              type="button"
              onClick={() => onFilterByStatus(s as BookingStatus)}
              className="inline-flex items-center gap-1 rounded-md border border-slate-200 bg-white px-2.5 py-1 text-xs text-slate-600 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300"
            >
              <span className="font-semibold">{count}</span>
              <span>{label}</span>
            </button>
          )
        })}
      </div>
    </div>
  )
}

// ─── Action buttons ───────────────────────────────────────────────────────────
// Render theo trạng thái + role hiện tại

interface ActionButtonsProps {
  booking: AdminBookingDetail
  userRole: AdminRole | null
  bookingId: number
  onSuccess: () => void
}

function ActionButtons({ booking, userRole, bookingId, onSuccess }: ActionButtonsProps) {
  const [consultNote, setConsultNote] = useState('')
  const [techNote, setTechNote] = useState('')
  const [techResultNote, setTechResultNote] = useState('')
  const [completeNote, setCompleteNote] = useState('')
  const [noShowReason, setNoShowReason] = useState('')
  const [cancelReason, setCancelReason] = useState('')
  const [feedback, setFeedback] = useState<{ ok: boolean; msg: string } | null>(null)

  async function run(fn: () => Promise<{ message?: string }>) {
    setFeedback(null)
    try {
      const r = await fn()
      setFeedback({ ok: true, msg: r.message ?? 'Thành công' })
      onSuccess()
    } catch (e) {
      setFeedback({ ok: false, msg: getErrorMessage(e) })
    }
  }

  const status = booking.status ?? ''

  // Chỉ SALES_SIDE mới được hủy; không thể hủy Completed
  const canCancel = isInRole(userRole, SALES_SIDE) && status !== 'Completed' && status !== 'Cancelled'

  return (
    <div className="space-y-4">
      {feedback && (
        <div className={`rounded-md px-3 py-2 text-sm ${feedback.ok ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/30 dark:text-emerald-300' : 'bg-rose-50 text-rose-700 dark:bg-rose-950/30 dark:text-rose-300'}`}>
          {feedback.msg}
        </div>
      )}

      {/* BƯỚC 1: Pending → Consulted (Sales side) */}
      {(status === 'Pending' || status === 'Scheduled') && isInRole(userRole, SALES_SIDE) && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Ghi nhận kết quả tư vấn</div>
          <textarea
            value={consultNote}
            onChange={(e) => setConsultNote(e.target.value)}
            rows={3}
            placeholder="Nội dung tư vấn với khách..."
            className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          <button
            type="button"
            disabled={!consultNote.trim()}
            onClick={() => run(() => consultBooking(bookingId, consultNote))}
            className="mt-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            Xác nhận đã tư vấn →
          </button>
        </div>
      )}

      {/* BƯỚC 2: Consulted → PendingTechCheck (Sales side) */}
      {status === 'Consulted' && isInRole(userRole, SALES_SIDE) && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Gửi xe qua bộ phận kỹ thuật</div>
          <textarea
            value={techNote}
            onChange={(e) => setTechNote(e.target.value)}
            rows={2}
            placeholder="Ghi chú cho kỹ thuật (tuỳ chọn)..."
            className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          <button
            type="button"
            onClick={() => run(() => sendToTechCheck(bookingId, techNote || undefined))}
            className="mt-2 rounded-md bg-orange-500 px-4 py-2 text-sm font-medium text-white hover:bg-orange-600"
          >
            Gửi kỹ thuật kiểm tra →
          </button>
        </div>
      )}

      {/* BƯỚC 3a: PendingTechCheck → TechApproved hoặc → Consulted (Technician/Admin) */}
      {status === 'PendingTechCheck' && isInRole(userRole, TECH_SIDE) && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Kết quả kiểm tra kỹ thuật</div>
          <textarea
            value={techResultNote}
            onChange={(e) => setTechResultNote(e.target.value)}
            rows={2}
            placeholder="Ghi chú kết quả kiểm tra (bắt buộc)..."
            className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          <div className="mt-2 flex flex-wrap gap-2">
            <button
              type="button"
              disabled={!techResultNote.trim()}
              onClick={() => run(() => submitTechResult(bookingId, true, techResultNote))}
              className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-medium text-white hover:bg-emerald-700 disabled:opacity-50"
            >
              ✓ Xe đạt — Duyệt
            </button>
            <button
              type="button"
              disabled={!techResultNote.trim()}
              onClick={() => run(() => submitTechResult(bookingId, false, techResultNote))}
              className="rounded-md bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700 disabled:opacity-50"
            >
              ✗ Không đạt — Trả lại Sales
            </button>
          </div>
        </div>
      )}

      {/* BƯỚC 3b: Sales thấy "đang chờ kỹ thuật" (readonly) */}
      {status === 'PendingTechCheck' && isInRole(userRole, ['Admin', 'Manager', 'Sales', 'ShowroomSales']) && !isInRole(userRole, ['Technician']) && (
        <div className="rounded-md border border-orange-200 bg-orange-50 p-3 dark:border-orange-900/40 dark:bg-orange-950/20">
          <div className="text-sm text-orange-700 dark:text-orange-300">
            Xe đang được bộ phận kỹ thuật kiểm tra. Vui lòng chờ kết quả...
          </div>
        </div>
      )}

      {/* BƯỚC 4: TechApproved → Confirmed (Sales side) */}
      {status === 'TechApproved' && isInRole(userRole, SALES_SIDE) && (
        <div className="rounded-md border border-teal-200 bg-teal-50 p-3 dark:border-teal-900/40 dark:bg-teal-950/20">
          <div className="mb-2 text-sm text-teal-700 dark:text-teal-300">
            Xe đã qua kiểm tra kỹ thuật. Xác nhận lịch với khách để tiến hành lái thử.
          </div>
          <button
            type="button"
            onClick={() => run(() => confirmBooking(bookingId))}
            className="rounded-md bg-teal-600 px-4 py-2 text-sm font-medium text-white hover:bg-teal-700"
          >
            Xác nhận lịch lái thử →
          </button>
        </div>
      )}

      {/* BƯỚC 5: Confirmed → Completed hoặc NoShow (Sales side) */}
      {status === 'Confirmed' && isInRole(userRole, SALES_SIDE) && (
        <div className="space-y-3 rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="text-xs font-medium text-slate-600 dark:text-zinc-300">Kết quả sau buổi lái thử</div>

          <div>
            <textarea
              value={completeNote}
              onChange={(e) => setCompleteNote(e.target.value)}
              rows={2}
              placeholder="Ghi chú kết quả lái thử (tuỳ chọn)..."
              className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            />
            <button
              type="button"
              onClick={() => run(() => completeBooking(bookingId, completeNote || undefined))}
              className="mt-2 rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
            >
              ✓ Khách đã lái thử xong
            </button>
          </div>

          <div className="border-t border-slate-200 pt-3 dark:border-zinc-800">
            <textarea
              value={noShowReason}
              onChange={(e) => setNoShowReason(e.target.value)}
              rows={2}
              placeholder="Lý do khách không đến (tuỳ chọn)..."
              className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            />
            <button
              type="button"
              onClick={() => run(() => markNoShow(bookingId, noShowReason || undefined))}
              className="mt-2 rounded-md bg-slate-500 px-4 py-2 text-sm font-medium text-white hover:bg-slate-600"
            >
              ✗ Khách không đến
            </button>
          </div>
        </div>
      )}

      {/* HỦY LỊCH — bất kỳ bước nào trừ Completed */}
      {canCancel && (
        <div className="rounded-md border border-rose-200 p-3 dark:border-rose-900/40">
          <div className="mb-2 text-xs font-medium text-rose-600 dark:text-rose-400">Hủy lịch hẹn</div>
          <textarea
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            rows={2}
            placeholder="Lý do hủy (bắt buộc)..."
            className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          <button
            type="button"
            disabled={!cancelReason.trim()}
            onClick={() => run(() => cancelBookingByAdmin(bookingId, cancelReason))}
            className="mt-2 rounded-md bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700 disabled:opacity-50"
          >
            Hủy lịch hẹn
          </button>
        </div>
      )}
    </div>
  )
}

// ─── Main page ────────────────────────────────────────────────────────────────

export function BookingsPage() {
  const { user } = useAuth()
  const userRole = (user?.role ?? null) as AdminRole | null

  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<BookingStatus | ''>('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [selectedId, setSelectedId] = useState<number | null>(null)

  const qc = useQueryClient()

  const isTechnician = userRole === 'Technician'
  const isManagerOrAdmin = isInRole(userRole, ['Admin', 'Manager'])

  // Technician chỉ thấy hàng chờ kỹ thuật
  if (isTechnician) {
    return (
      <div>
        <h1 className="text-xl font-semibold">Hàng chờ kiểm tra xe</h1>
        <p className="mt-1 text-sm text-slate-500 dark:text-zinc-400">
          Các xe đang chờ kỹ thuật kiểm tra trước lịch lái thử.
        </p>
        <TechQueueSection userRole={userRole} />
      </div>
    )
  }

 const params = useMemo<FetchAdminBookingsParams>(
    () => ({ search: search.trim() || undefined, status: status || undefined, page, pageSize }),
    [page, pageSize, search, status],
  )

  const listQ = useQuery({
    queryKey: ['admin-bookings', params],
    queryFn: () => fetchAdminBookings(params),
    enabled: !isTechnician,   // ← Technician không fetch danh sách chung
  })

  const detailQ = useQuery({
    queryKey: ['admin-booking-detail', selectedId],
    queryFn: () => fetchAdminBookingDetail(selectedId as number),
    enabled: !isTechnician && typeof selectedId === 'number',
  })

  const rows = listQ.data?.data ?? []
  const totalCount = listQ.data?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / Math.max(1, pageSize)))

  function refreshAll() {
    qc.invalidateQueries({ queryKey: ['admin-bookings'] })
    qc.invalidateQueries({ queryKey: ['admin-booking-detail', selectedId] })
    qc.invalidateQueries({ queryKey: ['admin-booking-stats'] })
  }

  function handleFilterByStatus(s: BookingStatus) {
    setStatus(s)
    setPage(1)
  }

  return (
    <div>
      <h1 className="text-xl font-semibold">Quản lý lịch hẹn</h1>
      <p className="mt-1 text-sm text-slate-500 dark:text-zinc-400">
        Danh sách lịch hẹn lái thử của khách hàng.
        {isInRole(userRole, ['Sales', 'ShowroomSales']) && (
          <span className="ml-1 text-slate-400 dark:text-zinc-500">· Chỉ hiển thị showroom của bạn.</span>
        )}
      </p>

      {/* Cảnh báo + tổng quan trạng thái (Manager / Admin) */}
      {isManagerOrAdmin && (
        <StatsBar onFilterByStatus={handleFilterByStatus} />
      )}

      <div className="mt-4 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        {/* Bộ lọc */}
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-8">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1) }}
              placeholder="Tên khách / SĐT..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            />
          </div>
          <div className="md:col-span-4">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
            <select
              value={status}
              onChange={(e) => { setStatus(e.target.value as BookingStatus | ''); setPage(1) }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả</option>
              {Object.entries(STATUS_LABEL).map(([v, l]) => (
                <option key={v} value={v}>{l}</option>
              ))}
            </select>
          </div>
        </div>

        {/* Bảng danh sách */}
        <div className="mt-4 overflow-auto">
          <table className="min-w-[980px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3">Mã</th>
                <th className="py-3 pr-3">Khách hàng</th>
                <th className="py-3 pr-3">SĐT</th>
                <th className="py-3 pr-3">Ngày hẹn</th>
                <th className="py-3 pr-3">Giờ</th>
                <th className="py-3 pr-3">Xe</th>
                <th className="py-3 pr-3">Showroom</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3">Tạo lúc</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading && (
                <tr><td colSpan={9} className="py-4 text-sm text-slate-500">Đang tải...</td></tr>
              )}
              {listQ.isError && (
                <tr><td colSpan={9} className="py-4 text-sm text-rose-600">{getErrorMessage(listQ.error)}</td></tr>
              )}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 && (
                <tr><td colSpan={9} className="py-4 text-sm text-slate-500">Không có dữ liệu.</td></tr>
              )}
              {rows.map((r: AdminBookingListItem) => (
                <tr
                  key={r.bookingId}
                  className="cursor-pointer border-b border-slate-100 hover:bg-slate-50 dark:border-zinc-900 dark:hover:bg-zinc-900/40"
                  onClick={() => setSelectedId(r.bookingId)}
                >
                  <td className="py-3 pr-3 font-medium">#{r.bookingId}</td>
                  <td className="py-3 pr-3">{r.customerName ?? '-'}</td>
                  <td className="py-3 pr-3">{r.phone ?? '-'}</td>
                  <td className="py-3 pr-3">{formatDateVN(r.bookingDate)}</td>
                  <td className="py-3 pr-3">{r.bookingTime ?? '-'}</td>
                  <td className="py-3 pr-3">
                    <div className="max-w-[240px] truncate">{r.carName ?? '-'}</div>
                  </td>
                  <td className="py-3 pr-3">
                    <div className="max-w-[220px] truncate">{r.showroomName ?? '-'}</div>
                  </td>
                  <td className="py-3 pr-3"><StatusBadge status={r.status} /></td>
                  <td className="py-3 pr-3 text-slate-500 dark:text-zinc-400">{r.createdAt ?? '-'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Phân trang */}
        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {page}/{totalPages} · {totalCount} lịch hẹn
          </div>
          <div className="flex items-center gap-2">
            <select
              value={String(pageSize)}
              onChange={(e) => { setPageSize(Number(e.target.value)); setPage(1) }}
              className="rounded-md border border-slate-200 bg-white px-2 py-1.5 text-xs dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >
              {[10, 20, 50].map((n) => <option key={n} value={n}>{n}/trang</option>)}
            </select>
            <button
              type="button"
              disabled={page <= 1}
              onClick={() => setPage((p) => p - 1)}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs disabled:opacity-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >
              ← Trước
            </button>
            <button
              type="button"
              disabled={page >= totalPages}
              onClick={() => setPage((p) => p + 1)}
              className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs disabled:opacity-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200"
            >
              Sau →
            </button>
          </div>
        </div>
      </div>

      {/* Modal chi tiết */}
      {selectedId !== null && (
        <div className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/40 p-4 md:p-8">
          <div className="w-full max-w-3xl rounded-lg border border-slate-200 bg-white shadow-lg dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-zinc-800">
              <div>
                <div className="text-sm font-semibold">Chi tiết lịch hẹn #{selectedId}</div>
                {detailQ.data && (
                  <div className="mt-0.5"><StatusBadge status={detailQ.data.status} /></div>
                )}
              </div>
              <button
                type="button"
                onClick={() => setSelectedId(null)}
                className="rounded-md border border-slate-200 px-2.5 py-1.5 text-xs hover:bg-slate-100 dark:border-zinc-800 dark:hover:bg-zinc-900"
              >
                Đóng
              </button>
            </div>

            <div className="px-4 py-4">
              {detailQ.isLoading && <div className="text-sm text-slate-500">Đang tải...</div>}
              {detailQ.isError && <div className="text-sm text-rose-600">{getErrorMessage(detailQ.error)}</div>}

              {!detailQ.isLoading && !detailQ.isError && detailQ.data && (
                <div className="space-y-4">
                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Khách hàng</div>
                      <div className="mt-1 text-sm font-semibold">{detailQ.data.customerName ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">SĐT</div>
                      <div className="mt-1 text-sm">{detailQ.data.phone ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Ngày / Giờ hẹn</div>
                      <div className="mt-1 text-sm">
                        {formatDateVN(detailQ.data.bookingDate)} · {detailQ.data.bookingTime ?? '-'}
                      </div>
                    </div>

                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Xe</div>
                      <div className="mt-1 text-sm font-semibold">{detailQ.data.carDetails?.name ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Showroom</div>
                      <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.name ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Tỉnh / Thành</div>
                      <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.province ?? '-'}</div>
                    </div>
                  </div>

                  {detailQ.data.note && (
                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="mb-1 text-xs font-medium text-slate-500 dark:text-zinc-400">Lịch sử xử lý</div>
                      <pre className="whitespace-pre-wrap text-xs leading-relaxed text-slate-700 dark:text-zinc-300">
                        {detailQ.data.note}
                      </pre>
                    </div>
                  )}

                  <ActionButtons
                    booking={detailQ.data}
                    userRole={userRole}
                    bookingId={selectedId}
                    onSuccess={refreshAll}
                  />
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

// ─── Tech queue (Technician only) ─────────────────────────────────────────────

function TechQueueSection({ userRole }: { userRole: AdminRole | null }) {
  const [selectedId, setSelectedId] = useState<number | null>(null)
  const qc = useQueryClient()

  const queueQ = useQuery({
    queryKey: ['admin-bookings-tech-queue'],
    queryFn: async () => {
      const { fetchPendingTechCheck } = await import('../../services/bookings/bookings')
      return fetchPendingTechCheck()
    },
  })

  const detailQ = useQuery({
    queryKey: ['admin-booking-detail', selectedId],
    queryFn: () => fetchAdminBookingDetail(selectedId as number),
    enabled: typeof selectedId === 'number',
  })

  const rows = queueQ.data ?? []

  return (
    <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
      {queueQ.isLoading && <div className="text-sm text-slate-500">Đang tải...</div>}
      {queueQ.isError && <div className="text-sm text-rose-600">{getErrorMessage(queueQ.error)}</div>}
      {!queueQ.isLoading && rows.length === 0 && (
        <div className="text-sm text-slate-500">Không có xe nào đang chờ kiểm tra.</div>
      )}

      <div className="space-y-3">
        {rows.map((r: AdminBookingListItem) => (
          <div
            key={r.bookingId}
            className="cursor-pointer rounded-md border border-slate-200 p-3 hover:bg-slate-50 dark:border-zinc-800 dark:hover:bg-zinc-900/40"
            onClick={() => setSelectedId(r.bookingId)}
          >
            <div className="flex items-center justify-between">
              <div className="text-sm font-semibold">{r.carName ?? '-'}</div>
              <div className="text-xs text-slate-500">{formatDateVN(r.bookingDate)} · {r.bookingTime ?? '-'}</div>
            </div>
            <div className="mt-1 text-xs text-slate-500">
              {r.showroomName ?? '-'} · Khách: {r.customerName ?? '-'} ({r.phone ?? '-'})
            </div>
          </div>
        ))}
      </div>

      {selectedId !== null && (
        <div className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/40 p-4 md:p-8">
          <div className="w-full max-w-2xl rounded-lg border border-slate-200 bg-white shadow-lg dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-zinc-800">
              <div className="text-sm font-semibold">Kiểm tra xe — Lịch #{selectedId}</div>
              <button
                type="button"
                onClick={() => setSelectedId(null)}
                className="rounded-md border border-slate-200 px-2.5 py-1.5 text-xs hover:bg-slate-100 dark:border-zinc-800 dark:hover:bg-zinc-900"
              >
                Đóng
              </button>
            </div>
            <div className="px-4 py-4">
              {detailQ.isLoading && <div className="text-sm text-slate-500">Đang tải...</div>}
              {detailQ.isError && <div className="text-sm text-rose-600">{getErrorMessage(detailQ.error)}</div>}
              {!detailQ.isLoading && !detailQ.isError && detailQ.data && (
                <div className="space-y-4">
                  {/* Info cards */}
                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Khách hàng</div>
                      <div className="mt-1 text-sm font-semibold">{detailQ.data.customerName ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">SĐT</div>
                      <div className="mt-1 text-sm">{detailQ.data.phone ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Ngày / Giờ hẹn</div>
                      <div className="mt-1 text-sm">
                        {formatDateVN(detailQ.data.bookingDate)} · {detailQ.data.bookingTime ?? '-'}
                      </div>
                    </div>
                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Xe</div>
                      <div className="mt-1 text-sm font-semibold">{detailQ.data.carDetails?.name ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Showroom</div>
                      <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.name ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Tỉnh / Thành</div>
                      <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.province ?? '-'}</div>
                    </div>
                  </div>

                  {/* Ghi chú từ Sales */}
                  {detailQ.data.note && (
                    <div className="rounded-md border border-orange-200 bg-orange-50 p-3 dark:border-orange-900/40 dark:bg-orange-950/20">
                      <div className="mb-1 text-xs font-medium text-orange-700 dark:text-orange-300">Ghi chú từ Sales / Lịch sử xử lý</div>
                      <pre className="whitespace-pre-wrap text-xs leading-relaxed text-slate-700 dark:text-zinc-300">
                        {detailQ.data.note}
                      </pre>
                    </div>
                  )}

                  <ActionButtons
                    booking={detailQ.data}
                    userRole={userRole}
                    bookingId={selectedId}
                    onSuccess={() => {
                      qc.invalidateQueries({ queryKey: ['admin-bookings-tech-queue'] })
                      qc.invalidateQueries({ queryKey: ['admin-booking-detail', selectedId] })
                      setSelectedId(null)
                    }}
                  />
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
