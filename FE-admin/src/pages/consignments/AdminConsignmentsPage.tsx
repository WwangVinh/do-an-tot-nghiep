import { useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'

import {
  fetchAdminConsignments,
  fetchAdminConsignmentDetail,
  updateConsignmentStatus,
  type AdminConsignmentDetail,
  type AdminConsignmentListItem,
  type ConsignmentStatus,
  type FetchAdminConsignmentsParams,
} from '../../services/consignments/consignments'
import { isInRole, type AdminRole } from '../../app/auth/roles'
import { useAuth } from '../../app/auth/useAuth'

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
  return d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
}

function formatCurrency(val: number | null | undefined): string {
  if (!val) return '-'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const STATUS_LABEL: Record<string, string> = {
  Pending:    'Chờ xử lý',
  Appraising: 'Đang thẩm định',
  Approved:   'Đã duyệt nhận',
  Rejected:   'Từ chối',
  Completed:  'Đã hoàn thành',
}

const STATUS_CLASS: Record<string, string> = {
  Pending:    'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-200',
  Appraising: 'bg-orange-50 text-orange-700 dark:bg-orange-950/40 dark:text-orange-200',
  Approved:   'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-200',
  Rejected:   'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-200',
  Completed:  'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-200',
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

interface ActionButtonsProps {
  data: AdminConsignmentDetail
  userRole: AdminRole | null
  consignmentId: number
  onSuccess: () => void
}

function ActionButtons({ data, userRole, consignmentId, onSuccess }: ActionButtonsProps) {
  const navigate = useNavigate()
  const [note, setNote] = useState('')
  const [feedback, setFeedback] = useState<{ ok: boolean; msg: string } | null>(null)
  const [isUpdating, setIsUpdating] = useState(false)

  async function handleUpdateStatus(newStatus: ConsignmentStatus) {
    setFeedback(null)
    setIsUpdating(true)
    try {
      const r = await updateConsignmentStatus(consignmentId, newStatus, note)
      setFeedback({ ok: true, msg: r.message ?? 'Cập nhật trạng thái thành công' })
      setNote('')
      onSuccess()

      if (newStatus === 'Approved') {
        const params = new URLSearchParams({
          brand:     data.brand              ?? '',
          model:     data.model              ?? '',
          year:      String(data.year        ?? ''),
          mileage:   String(data.mileage     ?? '0'),
          condition: 'Used',
        })
        navigate(`/cars/new?${params.toString()}`)
      }
    } catch (e) {
      setFeedback({ ok: false, msg: getErrorMessage(e) })
    } finally {
      setIsUpdating(false)
    }
  }

  const status = data.status ?? ''
  const canAction = isInRole(userRole, ['Admin', 'Manager', 'Sales', 'ShowroomSales'])

  if (!canAction) return null

  return (
    <div className="space-y-4">
      {feedback && (
        <div className={`rounded-md px-3 py-2 text-sm ${feedback.ok ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/30 dark:text-emerald-300' : 'bg-rose-50 text-rose-700 dark:bg-rose-950/30 dark:text-rose-300'}`}>
          {feedback.msg}
        </div>
      )}

      {status === 'Pending' && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Xử lý yêu cầu mới</div>
          <button
            type="button"
            disabled={isUpdating}
            onClick={() => handleUpdateStatus('Appraising')}
            className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            Tiến hành thẩm định xe →
          </button>
        </div>
      )}

      {status === 'Appraising' && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Quyết định nhận ký gửi</div>
          <textarea
            value={note}
            onChange={(e) => setNote(e.target.value)}
            rows={2}
            placeholder="Ghi chú về quyết định duyệt/từ chối (vd: Xe bị ngập nước)..."
            className="w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          <div className="mt-2 flex flex-wrap gap-2">
            <button
              type="button"
              disabled={isUpdating}
              onClick={() => handleUpdateStatus('Approved')}
              className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-medium text-white hover:bg-emerald-700 disabled:opacity-50"
            >
              ✓ Duyệt nhận ký gửi
            </button>
            <button
              type="button"
              disabled={isUpdating || !note.trim()}
              onClick={() => handleUpdateStatus('Rejected')}
              className="rounded-md bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700 disabled:opacity-50"
            >
              ✗ Từ chối nhận
            </button>
          </div>
          <p className="mt-2 text-xs text-slate-400 dark:text-zinc-500">
            * Khi duyệt nhận, hệ thống sẽ tự chuyển sang trang tạo xe với thông tin được điền sẵn.
          </p>
        </div>
      )}

      {status === 'Approved' && (
        <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
          <div className="mb-2 text-xs font-medium text-slate-600 dark:text-zinc-300">Cập nhật sau ký gửi</div>
          <div className="flex flex-wrap gap-2">
            <button
              type="button"
              disabled={isUpdating}
              onClick={() => handleUpdateStatus('Completed')}
              className="rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
            >
              Đã hoàn thành (Bán xong/Giao lại xe)
            </button>
            <button
              type="button"
              onClick={() => {
                const params = new URLSearchParams({
                  brand:   data.brand          ?? '',
                  model:   data.model          ?? '',
                  year:    String(data.year    ?? ''),
                  mileage: String(data.mileage ?? '0'),
                  condition: 'Used',
                })
                navigate(`/cars/new?${params.toString()}`)
              }}
              className="rounded-md border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
            >
              Tạo xe từ hồ sơ này →
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

export function AdminConsignmentsPage() {
  const { user } = useAuth()
  const userRole = (user?.role ?? null) as AdminRole | null

  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<ConsignmentStatus | ''>('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [selectedId, setSelectedId] = useState<number | null>(null)

  const qc = useQueryClient()

  const params = useMemo<FetchAdminConsignmentsParams>(
    () => ({ search: search.trim() || undefined, status: status || undefined, page, pageSize }),
    [page, pageSize, search, status],
  )

  const listQ = useQuery({
    queryKey: ['admin-consignments', params],
    queryFn: () => fetchAdminConsignments(params),
  })

  const detailQ = useQuery({
    queryKey: ['admin-consignment-detail', selectedId],
    queryFn: () => fetchAdminConsignmentDetail(selectedId as number),
    enabled: typeof selectedId === 'number',
  })

  const rows = listQ.data?.data ?? []
  const totalCount = listQ.data?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / Math.max(1, pageSize)))

  function refreshAll() {
    qc.invalidateQueries({ queryKey: ['admin-consignments'] })
    qc.invalidateQueries({ queryKey: ['admin-consignment-detail', selectedId] })
  }

  return (
    <div>
      <h1 className="text-xl font-semibold">Quản lý Ký gửi xe</h1>
      <p className="mt-1 text-sm text-slate-500 dark:text-zinc-400">
        Danh sách yêu cầu ký gửi xe từ khách hàng.
      </p>

      <div className="mt-4 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-8">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1) }}
              placeholder="Tên khách, SĐT, Hãng xe..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            />
          </div>
          <div className="md:col-span-4">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
            <select
              value={status}
              onChange={(e) => { setStatus(e.target.value as ConsignmentStatus | ''); setPage(1) }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả</option>
              {Object.entries(STATUS_LABEL).map(([v, l]) => (
                <option key={v} value={v}>{l}</option>
              ))}
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
                <th className="py-3 pr-3">Thông tin Xe</th>
                <th className="py-3 pr-3">Giá kỳ vọng</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3">Ngày gửi</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading && (
                <tr><td colSpan={7} className="py-4 text-sm text-slate-500">Đang tải...</td></tr>
              )}
              {listQ.isError && (
                <tr><td colSpan={7} className="py-4 text-sm text-rose-600">{getErrorMessage(listQ.error)}</td></tr>
              )}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 && (
                <tr><td colSpan={7} className="py-4 text-sm text-slate-500">Không có dữ liệu.</td></tr>
              )}
              {rows.map((r: AdminConsignmentListItem) => (
                <tr
                  key={r.consignmentId}
                  className="cursor-pointer border-b border-slate-100 hover:bg-slate-50 dark:border-zinc-900 dark:hover:bg-zinc-900/40"
                  onClick={() => setSelectedId(r.consignmentId)}
                >
                  <td className="py-3 pr-3 font-medium">#{r.consignmentId}</td>
                  <td className="py-3 pr-3">{r.guestName ?? '-'}</td>
                  <td className="py-3 pr-3">{r.guestPhone ?? '-'}</td>
                  <td className="py-3 pr-3">
                    <span className="font-semibold">{r.brand}</span> {r.model} {r.year ? `(${r.year})` : ''}
                  </td>
                  <td className="py-3 pr-3 font-medium text-emerald-600 dark:text-emerald-400">
                    {formatCurrency(r.expectedPrice)}
                  </td>
                  <td className="py-3 pr-3"><StatusBadge status={r.status} /></td>
                  <td className="py-3 pr-3 text-slate-500 dark:text-zinc-400">{formatDateVN(r.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {page}/{totalPages} · {totalCount} yêu cầu
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

      {selectedId !== null && (
        <div className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/40 p-4 md:p-8">
          <div className="w-full max-w-3xl rounded-lg border border-slate-200 bg-white shadow-lg dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-zinc-800">
              <div>
                <div className="text-sm font-semibold">Chi tiết Ký gửi #{selectedId}</div>
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
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Thông tin Khách hàng</div>
                      <div className="mt-1 text-sm font-semibold">{detailQ.data.guestName ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Số điện thoại</div>
                      <div className="mt-1 text-sm">{detailQ.data.guestPhone ?? '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Email</div>
                      <div className="mt-1 text-sm">{detailQ.data.guestEmail || '-'}</div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Ngày gửi form</div>
                      <div className="mt-1 text-sm">{formatDateVN(detailQ.data.createdAt)}</div>
                    </div>

                    <div className="rounded-md border border-slate-200 p-3 dark:border-zinc-800">
                      <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Thông tin Xe Ký Gửi</div>
                      <div className="mt-1 text-sm font-semibold text-rose-600 dark:text-rose-400">
                        {detailQ.data.brand} {detailQ.data.model}
                      </div>
                      <div className="flex gap-6 mt-2">
                        <div>
                          <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Năm SX</div>
                          <div className="mt-1 text-sm">{detailQ.data.year ?? '-'}</div>
                        </div>
                        <div>
                          <div className="text-xs font-medium text-slate-500 dark:text-zinc-400">Odo (Số km)</div>
                          <div className="mt-1 text-sm">{detailQ.data.mileage ? `${detailQ.data.mileage} km` : '-'}</div>
                        </div>
                      </div>
                      <div className="mt-2 text-xs font-medium text-slate-500 dark:text-zinc-400">Giá kỳ vọng</div>
                      <div className="mt-1 text-sm font-semibold text-emerald-600 dark:text-emerald-400">
                        {formatCurrency(detailQ.data.expectedPrice)}
                      </div>
                    </div>
                  </div>

                  {detailQ.data.conditionDescription && (
                    <div className="rounded-md border border-slate-200 p-3 bg-slate-50 dark:bg-zinc-900/50 dark:border-zinc-800">
                      <div className="mb-1 text-xs font-medium text-slate-500 dark:text-zinc-400">Mô tả tình trạng xe (Khách điền)</div>
                      <p className="whitespace-pre-wrap text-sm text-slate-700 dark:text-zinc-300">
                        {detailQ.data.conditionDescription}
                      </p>
                    </div>
                  )}

                  <ActionButtons
                    data={detailQ.data}
                    userRole={userRole}
                    consignmentId={selectedId}
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