import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import type { AdminBookingDetail, AdminBookingListItem, BookingStatus, FetchAdminBookingsParams } from '../../services/bookings/bookings'
import { fetchAdminBookingDetail, fetchAdminBookings, updateAdminBooking } from '../../services/bookings/bookings'

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

function formatDateVN(raw: string | null | undefined): string {
  const s = (raw ?? '').trim()
  if (!s) return '-'
  // Backend có thể trả DateOnly dạng '2026-04-05T00:00:00' hoặc '2026-04-05'
  const d = new Date(s)
  if (Number.isNaN(d.getTime())) return s
  return d.toLocaleDateString('vi-VN')
}

export function BookingsPage() {
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<BookingStatus | ''>('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [selectedId, setSelectedId] = useState<number | null>(null)
  const [editStatus, setEditStatus] = useState<BookingStatus | ''>('')
  const [editResult, setEditResult] = useState('')

  const qc = useQueryClient()

  const params = useMemo<FetchAdminBookingsParams>(() => {
    return {
      search: search.trim() || undefined,
      status: status || undefined,
      page,
      pageSize,
    }
  }, [page, pageSize, search, status])

  const qKey = useMemo(() => ['admin-bookings', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminBookings(params),
  })

  const detailQ = useQuery({
    queryKey: ['admin-booking-detail', selectedId],
    queryFn: () => fetchAdminBookingDetail(selectedId as number),
    enabled: typeof selectedId === 'number',
  })

  const updateM = useMutation({
    mutationFn: async (payload: { bookingId: number; status?: string | null; result?: string | null }) => {
      return updateAdminBooking(payload.bookingId, { status: payload.status ?? null, result: payload.result ?? null })
    },
    onSuccess: async () => {
      await Promise.all([
        qc.invalidateQueries({ queryKey: ['admin-bookings'] }),
        qc.invalidateQueries({ queryKey: ['admin-booking-detail', selectedId] }),
      ])
    },
  })

  const rows = listQ.data?.data ?? []
  const totalCount = listQ.data?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / Math.max(1, pageSize)))

  useEffect(() => {
    const d = detailQ.data as AdminBookingDetail | undefined
    if (!d) return
    setEditStatus((d.status ?? '') as any)
    setEditResult(d.note ?? '')
  }, [detailQ.data])

  function renderStatusBadge(s: string | null | undefined) {
    const v = (s ?? '').trim()
    if (!v) return null
    const cls =
      v === 'Confirmed'
        ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-200'
        : v === 'Scheduled'
          ? 'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-200'
          : v === 'Completed'
            ? 'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-200'
            : v === 'Cancelled'
              ? 'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-200'
              : 'bg-slate-100 text-slate-700 dark:bg-zinc-900 dark:text-zinc-200'
    return <span className={['inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium', cls].join(' ')}>{v}</span>
  }

  return (
    <div>
      <h1 className="text-xl font-semibold">Đặt lịch</h1>
      <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Danh sách các đặt lịch của khách hàng (bảng `Bookings`).</p>

      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-8">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input
              value={search}
              onChange={(e) => {
                setSearch(e.target.value)
                setPage(1)
              }}
              placeholder="Tên khách / SĐT..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>

          <div className="md:col-span-4">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
            <select
              value={status}
              onChange={(e) => {
                setStatus(e.target.value as any)
                setPage(1)
              }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả</option>
              <option value="Scheduled">Scheduled</option>
              <option value="Confirmed">Confirmed</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>
        </div>

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
              {listQ.isLoading ? (
                <tr>
                  <td colSpan={9} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Đang tải...
                  </td>
                </tr>
              ) : null}
              {listQ.isError ? (
                <tr>
                  <td colSpan={9} className="py-4 text-sm text-rose-600">
                    {getErrorMessage(listQ.error)}
                  </td>
                </tr>
              ) : null}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 ? (
                <tr>
                  <td colSpan={9} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Không có dữ liệu.
                  </td>
                </tr>
              ) : null}

              {rows.map((r: AdminBookingListItem) => (
                <tr
                  key={r.bookingId}
                  className="cursor-pointer border-b border-slate-100 hover:bg-slate-50 dark:border-zinc-900 dark:hover:bg-zinc-900/40"
                  onClick={() => setSelectedId(r.bookingId)}
                  title="Bấm để xem chi tiết"
                >
                  <td className="py-3 pr-3">
                    <div className="font-medium">#{r.bookingId}</div>
                  </td>
                  <td className="py-3 pr-3">{r.customerName ?? '-'}</td>
                  <td className="py-3 pr-3">{r.phone ?? '-'}</td>
                  <td className="py-3 pr-3">{formatDateVN(r.bookingDate)}</td>
                  <td className="py-3 pr-3">{r.bookingTime ?? '-'}</td>
                  <td className="py-3 pr-3">
                    <div className="max-w-[240px] truncate" title={r.carName ?? ''}>
                      {r.carName ?? '-'}
                    </div>
                  </td>
                  <td className="py-3 pr-3">
                    <div className="max-w-[220px] truncate" title={r.showroomName ?? ''}>
                      {r.showroomName ?? '-'}
                    </div>
                  </td>
                  <td className="py-3 pr-3">{renderStatusBadge(r.status)}</td>
                  <td className="py-3 pr-3">{r.createdAt ?? '-'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {page}/{totalPages} · {totalCount} lịch hẹn
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

      {selectedId !== null ? (
        <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/40 p-4 md:p-8">
          <div className="w-full max-w-3xl rounded-lg border border-slate-200 bg-white shadow-lg dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-zinc-800">
              <div>
                <div className="text-sm font-semibold">Chi tiết đặt lịch</div>
                <div className="text-xs text-slate-500 dark:text-zinc-400">#{selectedId}</div>
              </div>
              <button
                type="button"
                onClick={() => {
                  setSelectedId(null)
                }}
                className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
              >
                Đóng
              </button>
            </div>

            <div className="px-4 py-4">
              {detailQ.isLoading ? <div className="text-sm text-slate-600 dark:text-zinc-300">Đang tải...</div> : null}
              {detailQ.isError ? <div className="text-sm text-rose-600">{getErrorMessage(detailQ.error)}</div> : null}

              {!detailQ.isLoading && !detailQ.isError && detailQ.data ? (
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                    <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Khách hàng</div>
                    <div className="mt-1 text-sm font-semibold">{detailQ.data.customerName ?? '-'}</div>
                    <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">SĐT</div>
                    <div className="mt-1 text-sm">{detailQ.data.phone ?? '-'}</div>
                    <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Ngày / Giờ</div>
                    <div className="mt-1 text-sm">
                      {formatDateVN(detailQ.data.bookingDate)} · {detailQ.data.bookingTime ?? '-'}
                    </div>
                  </div>

                  <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                    <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Xe</div>
                    <div className="mt-1 text-sm font-semibold">{detailQ.data.carDetails?.name ?? '-'}</div>
                    <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Showroom</div>
                    <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.name ?? '-'}</div>
                    <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Tỉnh/Thành</div>
                    <div className="mt-1 text-sm">{detailQ.data.showroomDetails?.province ?? '-'}</div>
                  </div>

                  <div className="md:col-span-2 rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                    <div className="grid grid-cols-1 gap-3 md:grid-cols-2 md:items-end">
                      <div>
                        <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
                        <select
                          value={editStatus}
                          onChange={(e) => setEditStatus(e.target.value as any)}
                          className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
                        >
                          <option value="Scheduled">Scheduled</option>
                          <option value="Confirmed">Confirmed</option>
                          <option value="Completed">Completed</option>
                          <option value="Cancelled">Cancelled</option>
                        </select>
                        <div className="mt-2 text-xs text-slate-500 dark:text-zinc-400">Hiện tại: {renderStatusBadge(detailQ.data.status)}</div>
                      </div>

                      <div className="md:text-right">
                        <button
                          type="button"
                          disabled={updateM.isPending}
                          onClick={() =>
                            updateM.mutate({
                              bookingId: selectedId,
                              status: editStatus || null,
                              result: editResult.trim() || null,
                            })
                          }
                          className="w-full md:w-auto rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
                        >
                          {updateM.isPending ? 'Đang lưu...' : 'Lưu cập nhật'}
                        </button>
                        {updateM.isSuccess ? (
                          <div className="mt-2 text-xs text-emerald-600">Đã lưu.</div>
                        ) : updateM.isError ? (
                          <div className="mt-2 text-xs text-rose-600">{getErrorMessage(updateM.error)}</div>
                        ) : null}
                      </div>
                    </div>

                    <div className="mt-3">
                      <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Kết quả</label>
                      <textarea
                        value={editResult}
                        onChange={(e) => setEditResult(e.target.value)}
                        rows={6}
                        placeholder="Nhập kết quả xử lý / ghi chú sau khi làm việc với khách..."
                        className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
                      />
                      <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">
                        Trường này sẽ được lưu trong `Note` của booking (dạng log).
                      </div>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>
          </div>
        </div>
      ) : null}
    </div>
  )
}

