import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Link } from 'react-router-dom'

import { env } from '../../lib/env'
import type { AdminCarListItem, CarStatus, FetchAdminCarsParams } from '../../services/cars/cars'
import { fetchAdminCars, restoreCar, softDeleteCar } from '../../services/cars/cars'

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

export function CarsListPage() {
  const qc = useQueryClient()
  const [search, setSearch] = useState('')
  const [brand, setBrand] = useState('')
  const [status, setStatus] = useState<CarStatus | ''>('')
  const [isDeleted, setIsDeleted] = useState<boolean | ''>('')
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  const params = useMemo<FetchAdminCarsParams>(() => {
    return {
      search: search.trim() || undefined,
      brand: brand.trim() || undefined,
      status: status || undefined,
      isDeleted: isDeleted === '' ? undefined : isDeleted,
      page,
      pageSize,
    }
  }, [brand, isDeleted, page, pageSize, search, status])

  const qKey = useMemo(() => ['admin-cars', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminCars(params),
  })

  const deleteM = useMutation({
    mutationFn: (carId: number) => softDeleteCar(carId),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Đã đưa xe vào thùng rác')
      await qc.invalidateQueries({ queryKey: ['admin-cars'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const restoreM = useMutation({
    mutationFn: (carId: number) => restoreCar(carId),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Đã khôi phục xe')
      await qc.invalidateQueries({ queryKey: ['admin-cars'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const rows = listQ.data?.data ?? []
  const totalPages = listQ.data?.totalPages ?? 1

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
          <h1 className="text-xl font-semibold">Danh sách xe</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý xe (bảng `Cars`).</p>
        </div>
        <Link
          to="/cars/new"
          className="rounded-md bg-slate-900 px-3 py-2 text-sm font-medium text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
        >
          + Thêm xe
        </Link>
      </div>

      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-5">
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

          <div className="md:col-span-2">
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

          <div className="md:col-span-2">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Thùng rác</label>
            <select
              value={String(isDeleted)}
              onChange={(e) => {
                const v = e.target.value
                setIsDeleted(v === '' ? '' : v === 'true')
                setPage(1)
              }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="">Tất cả</option>
              <option value="false">Đang hoạt động</option>
              <option value="true">Đã xoá (soft)</option>
            </select>
          </div>
        </div>

        <div className="mt-4 overflow-auto">
          <table className="min-w-[980px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3">Xe</th>
                <th className="py-3 pr-3">Hãng</th>
                <th className="py-3 pr-3">Năm</th>
                <th className="py-3 pr-3">Giá</th>
                <th className="py-3 pr-3">Tồn</th>
                <th className="py-3 pr-3">Showrooms</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3 text-right">Thao tác</th>
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

              {rows.map((r: AdminCarListItem) => {
                const img = resolveImageUrl(r.imageUrl ?? '')
                return (
                  <tr key={r.carId} className="border-b border-slate-100 dark:border-zinc-900">
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
                    <td className="py-3 pr-3">{Number(r.price ?? 0).toLocaleString('vi-VN')}</td>
                    <td className="py-3 pr-3">{r.totalQuantity ?? 0}</td>
                    <td className="py-3 pr-3">
                      <div className="max-w-[220px] truncate" title={r.showrooms ?? ''}>
                        {r.showrooms ?? '-'}
                      </div>
                    </td>
                    <td className="py-3 pr-3">{renderStatusBadge(r.status ?? '')}</td>
                    <td className="py-3 pr-3 text-right">
                      <div className="inline-flex items-center gap-2">
                        <Link
                          to={`/cars/${r.carId}/edit`}
                          className="rounded-md border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 hover:bg-slate-100 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                        >
                          Sửa
                        </Link>
                        {r.isDeleted ? (
                          <button
                            type="button"
                            disabled={restoreM.isPending}
                            onClick={() => restoreM.mutate(r.carId)}
                            className="rounded-md border border-emerald-200 bg-emerald-50 px-2.5 py-1.5 text-xs text-emerald-700 hover:bg-emerald-100 disabled:opacity-60 dark:border-emerald-900/40 dark:bg-emerald-950/30 dark:text-emerald-200 dark:hover:bg-emerald-950/50"
                          >
                            Khôi phục
                          </button>
                        ) : (
                          <button
                            type="button"
                            disabled={deleteM.isPending}
                            onClick={() => deleteM.mutate(r.carId)}
                            className="rounded-md border border-rose-200 bg-rose-50 px-2.5 py-1.5 text-xs text-rose-700 hover:bg-rose-100 disabled:opacity-60 dark:border-rose-900/40 dark:bg-rose-950/30 dark:text-rose-200 dark:hover:bg-rose-950/50"
                          >
                            Xoá
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
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

