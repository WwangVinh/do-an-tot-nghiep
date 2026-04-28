import { useMemo, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ChevronDown, ChevronRight, RefreshCcw } from 'lucide-react'

import { env } from '../../lib/env'
import type { AdminCarListItem, CarStatus, FetchAdminCarsParams } from '../../services/cars/cars'
import { fetchAdminCars } from '../../services/cars/cars'
import { fetchAdminInventoriesByCarId } from '../../services/inventories/inventories'

function resolveImageUrl(raw: string | null | undefined): string {
  const s = (raw ?? '').trim()
  if (!s) return ''
  if (s.startsWith('http://') || s.startsWith('https://') || s.startsWith('data:')) return s
  if (s.startsWith('/')) return `${env.VITE_API_BASE_URL}${s}`
  return s
}

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

export function InventoriesPage() {
  const [search, setSearch] = useState('')
  const [brand, setBrand] = useState('')
  const [status, setStatus] = useState<CarStatus | ''>('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [expandedCarId, setExpandedCarId] = useState<number | null>(null)

  const params = useMemo<FetchAdminCarsParams>(() => {
    return {
      search: search.trim() || undefined,
      brand: brand.trim() || undefined,
      status: status || undefined,
      isDeleted: false,
      page,
      pageSize,
    }
  }, [brand, page, pageSize, search, status])

  const qKey = useMemo(() => ['admin-inventories', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminCars(params),
  })

  const rows = listQ.data?.data ?? []
  const totalPages = listQ.data?.totalPages ?? 1

  const invQ = useQuery({
    queryKey: ['admin-car-inventory', expandedCarId],
    queryFn: () => fetchAdminInventoriesByCarId(expandedCarId as number),
    enabled: typeof expandedCarId === 'number' && expandedCarId > 0,
  })

  function renderStatusBadge(s: string | null | undefined) {
    const v = (s ?? '').trim()
    if (!v) return null
    const cls =
      v === 'Available'
        ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-200'
        : v === 'PendingApproval'
          ? 'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-200'
          : v === 'Draft'
            ? 'bg-slate-100 text-slate-700 dark:bg-zinc-900 dark:text-zinc-200'
            : v === 'Out_of_stock'
              ? 'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-200'
              : 'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-200'
    return <span className={['inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium', cls].join(' ')}>{v}</span>
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Tồn kho</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Danh sách tồn kho của xe (tổng tồn theo xe).</p>
        </div>

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
              placeholder="Tên xe..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>

          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Hãng</label>
            <input
              value={brand}
              onChange={(e) => {
                setBrand(e.target.value)
                setPage(1)
              }}
              placeholder="TOYOTA..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>

          <div className="md:col-span-3">
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
              <option value="Available">Available</option>
              <option value="PendingApproval">PendingApproval</option>
              <option value="Draft">Draft</option>
              <option value="COMING_SOON">COMING_SOON</option>
              <option value="Out_of_stock">Out_of_stock</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>
        </div>

        <div className="mt-4 overflow-auto">
          <table className="min-w-[980px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3" />
                <th className="py-3 pr-3">Xe</th>
                <th className="py-3 pr-3">Hãng</th>
                <th className="py-3 pr-3">Năm</th>
                <th className="py-3 pr-3 text-right">Giá</th>
                <th className="py-3 pr-3 text-right">Tồn</th>
                <th className="py-3 pr-3">Trạng thái</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading ? (
                <tr>
                  <td colSpan={7} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Đang tải...
                  </td>
                </tr>
              ) : null}
              {listQ.isError ? (
                <tr>
                  <td colSpan={7} className="py-4 text-sm text-rose-600">
                    {getErrorMessage(listQ.error)}
                  </td>
                </tr>
              ) : null}
              {!listQ.isLoading && !listQ.isError && rows.length === 0 ? (
                <tr>
                  <td colSpan={7} className="py-4 text-sm text-slate-600 dark:text-zinc-300">
                    Không có dữ liệu.
                  </td>
                </tr>
              ) : null}

              {rows.map((r: AdminCarListItem) => {
                const img = resolveImageUrl(r.imageUrl ?? '')
                const qty = Number(r.totalQuantity ?? 0)
                const qtyCls =
                  qty <= 0
                    ? 'text-rose-700 dark:text-rose-200'
                    : qty <= 3
                      ? 'text-amber-700 dark:text-amber-200'
                      : 'text-emerald-700 dark:text-emerald-200'
                const expanded = expandedCarId === r.carId
                return (
                  <>
                    <tr key={r.carId} className="border-b border-slate-100 dark:border-zinc-900">
                      <td className="py-3 pr-3">
                        <button
                          type="button"
                          onClick={() => setExpandedCarId((cur) => (cur === r.carId ? null : r.carId))}
                          className="inline-flex h-8 w-8 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                          aria-label={expanded ? 'Thu gọn' : 'Xem chi tiết theo showroom'}
                          title={expanded ? 'Thu gọn' : 'Xem chi tiết theo showroom'}
                        >
                          {expanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
                        </button>
                      </td>
                      <td className="py-3 pr-3">
                        <div className="flex items-center gap-3">
                          <div className="h-10 w-14 overflow-hidden rounded-md border border-slate-200 bg-slate-50 dark:border-zinc-800 dark:bg-zinc-900">
                            {img ? <img src={img} alt={r.name} className="h-full w-full object-cover" /> : null}
                          </div>
                          <div>
                            <div className="font-medium">{r.name}</div>
                            <div className="text-xs text-slate-500 dark:text-zinc-400">#{r.carId}</div>
                          </div>
                        </div>
                      </td>
                      <td className="py-3 pr-3">{r.brand}</td>
                      <td className="py-3 pr-3">{r.year}</td>
                      <td className="py-3 pr-3 text-right">{Number(r.price ?? 0).toLocaleString('vi-VN')}</td>
                      <td className={['py-3 pr-3 text-right font-semibold', qtyCls].join(' ')}>{qty}</td>
                      <td className="py-3 pr-3">{renderStatusBadge(r.status ?? '')}</td>
                    </tr>

                    {expanded ? (
                      <tr className="border-b border-slate-100 bg-slate-50/40 dark:border-zinc-900 dark:bg-zinc-950/40">
                        <td colSpan={7} className="py-3 pr-3">
                          <div className="rounded-lg border border-slate-200 bg-white p-3 dark:border-zinc-800 dark:bg-zinc-950">
                            <div className="flex items-center justify-between gap-3">
                              <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">
                                Chi tiết tồn kho theo showroom
                              </div>
                              <div className="text-xs text-slate-500 dark:text-zinc-400">
                                Tổng: {invQ.data?.totalQuantity ?? qty}
                              </div>
                            </div>

                            {invQ.isLoading ? (
                              <div className="mt-3 text-sm text-slate-600 dark:text-zinc-300">Đang tải chi tiết...</div>
                            ) : null}
                            {invQ.isError ? (
                              <div className="mt-3 text-sm text-rose-600">{getErrorMessage(invQ.error)}</div>
                            ) : null}

                            {!invQ.isLoading && !invQ.isError ? (
                              invQ.data?.details?.length ? (
                                <div className="mt-3 overflow-auto">
                                  <table className="min-w-[760px] w-full text-left text-sm">
                                    <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
                                      <tr className="border-b border-slate-200 dark:border-zinc-800">
                                        <th className="py-2 pr-3">Showroom</th>
                                        <th className="py-2 pr-3">Tỉnh/TP</th>
                                        <th className="py-2 pr-3 ">Số lượng</th>
                                        <th className="py-2 pr-3">Trạng thái </th>
                                      </tr>
                                    </thead>
                                    <tbody className="text-slate-700 dark:text-zinc-200">
                                      {invQ.data.details.map((d) => (
                                        <tr key={`${r.carId}-${d.showroomId}`} className="border-b border-slate-100 dark:border-zinc-900">
                                          <td className="py-2 pr-3">
                                            <div className="font-medium">{d.showroomName || `Showroom #${d.showroomId}`}</div>
                                            <div className="text-xs text-slate-500 dark:text-zinc-400">ID: {d.showroomId}</div>
                                          </td>
                                          <td className="py-2 pr-3">{d.province || '—'}</td>
                                          <td className="py-2 pr-3 font-semibold">{Number(d.quantity ?? 0)}</td>
                                          <td className="py-2 pr-3">{d.displayStatus || '—'}</td>
                                        </tr>
                                      ))}
                                    </tbody>
                                  </table>
                                </div>
                              ) : (
                                <div className="mt-3 text-sm text-slate-600 dark:text-zinc-300">Chưa có tồn kho theo showroom.</div>
                              )
                            ) : null}
                          </div>
                        </td>
                      </tr>
                    ) : null}
                  </>
                )
              })}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="text-xs text-slate-500 dark:text-zinc-400">
            Trang {listQ.data?.currentPage ?? page}/{totalPages} · {listQ.data?.totalItems ?? 0} xe
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
    </div>
  )
}

