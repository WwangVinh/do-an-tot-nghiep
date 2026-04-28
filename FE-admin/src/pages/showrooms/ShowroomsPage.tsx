import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Plus, RefreshCcw, Save, X } from 'lucide-react'

import { env } from '../../lib/env'
import type { Showroom, ShowroomCar, ShowroomCreateInput } from '../../services/showrooms/showrooms'
import { createAdminShowroom, fetchAdminShowroomCars, fetchAdminShowrooms } from '../../services/showrooms/showrooms'

function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const anyErr = err as { message?: unknown; response?: { data?: unknown } }
    const data = anyErr.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (data && typeof data === 'object' && 'Message' in data) {
      const msg = (data as { Message?: unknown }).Message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (typeof anyErr.message === 'string' && anyErr.message.trim()) return anyErr.message
  }
  return 'Có lỗi xảy ra'
}

function emptyForm(): ShowroomCreateInput {
  return { name: '', province: '', district: '', streetAddress: '', hotline: '' }
}

function renderAddress(s: Showroom) {
  const full = (s.fullAddress ?? '').trim()
  if (full) return full
  return [s.streetAddress, s.district, s.province].filter((x) => (x ?? '').trim()).join(', ')
}

function resolveImageUrl(raw: string | null | undefined): string {
  const s = (raw ?? '').trim()
  if (!s) return ''
  if (s.startsWith('http://') || s.startsWith('https://') || s.startsWith('data:')) return s
  if (s.startsWith('/')) return `${env.VITE_API_BASE_URL}${s}`
  return s
}

export function ShowroomsPage() {
  const qc = useQueryClient()
  const [dialogOpen, setDialogOpen] = useState(false)
  const [carsDialogOpen, setCarsDialogOpen] = useState(false)
  const [selectedShowroom, setSelectedShowroom] = useState<Showroom | null>(null)
  const [form, setForm] = useState<ShowroomCreateInput>(() => emptyForm())

  const listQ = useQuery({
    queryKey: ['admin-showrooms'],
    queryFn: () => fetchAdminShowrooms(),
  })

  const carsQKey = useMemo(
    () => ['admin-showroom-cars', { showroomId: selectedShowroom?.showroomId ?? 0 }],
    [selectedShowroom?.showroomId],
  )

  const carsQ = useQuery({
    queryKey: carsQKey,
    queryFn: () => fetchAdminShowroomCars(Number(selectedShowroom?.showroomId ?? 0)),
    enabled: Boolean(selectedShowroom?.showroomId) && carsDialogOpen,
  })

  const createM = useMutation({
    mutationFn: (input: ShowroomCreateInput) => createAdminShowroom(input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Tạo showroom thành công')
      setDialogOpen(false)
      setForm(emptyForm())
      await qc.invalidateQueries({ queryKey: ['admin-showrooms'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const submitting = createM.isPending
  const rows = listQ.data ?? []
  const inventory: ShowroomCar[] = carsQ.data?.inventory ?? []

  function openCreate() {
    setForm(emptyForm())
    setDialogOpen(true)
  }

  function openCars(s: Showroom) {
    setSelectedShowroom(s)
    setCarsDialogOpen(true)
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
  }

  function closeCarsDialog() {
    setCarsDialogOpen(false)
    setSelectedShowroom(null)
  }

  function submit() {
    const name = form.name.trim()
    const province = form.province.trim()
    const district = form.district.trim()
    const streetAddress = form.streetAddress.trim()
    const hotline = (form.hotline ?? '').trim()

    if (!name) return toast.error('Vui lòng nhập tên showroom')
    if (!province) return toast.error('Vui lòng nhập Tỉnh/Thành phố')
    if (!district) return toast.error('Vui lòng nhập Quận/Huyện')
    if (!streetAddress) return toast.error('Vui lòng nhập Địa chỉ')

    createM.mutate({
      name,
      province,
      district,
      streetAddress,
      hotline: hotline ? hotline : null,
    })
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Showroom</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý showroom (bảng `Showrooms`).</p>
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
            Thêm showroom
          </button>
        </div>
      </div>

      <div className="mt-6 overflow-hidden rounded-lg border border-slate-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <div className="border-b border-slate-200 px-4 py-3 text-sm text-slate-600 dark:border-zinc-800 dark:text-zinc-300">
          {listQ.isLoading ? 'Đang tải...' : `Tổng: ${rows.length} showroom`}
          {listQ.isFetching && !listQ.isLoading ? ' (đang tải lại...)' : ''}
        </div>

        {listQ.isError ? (
          <div className="p-4 text-sm text-red-600 dark:text-red-400">{getErrorMessage(listQ.error)}</div>
        ) : (
          <div className="w-full overflow-x-auto">
            <table className="min-w-[980px] w-full text-left text-sm">
              <thead className="bg-slate-50 text-slate-600 dark:bg-zinc-900/50 dark:text-zinc-300">
                <tr>
                  <th className="px-4 py-3 font-medium">ID</th>
                  <th className="px-4 py-3 font-medium">Tên</th>
                  <th className="px-4 py-3 font-medium">Địa chỉ</th>
                  <th className="px-4 py-3 font-medium">Tỉnh/TP</th>
                  <th className="px-4 py-3 font-medium">Quận/Huyện</th>
                  <th className="px-4 py-3 font-medium">Hotline</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((s) => (
                  <tr
                    key={s.showroomId}
                    className="cursor-pointer border-t border-slate-100 hover:bg-slate-50/60 dark:border-zinc-900 dark:hover:bg-zinc-900/30"
                    onClick={() => openCars(s)}
                    title="Click để xem xe trong showroom"
                  >
                    <td className="px-4 py-3 text-slate-600 dark:text-zinc-300">{s.showroomId}</td>
                    <td className="px-4 py-3 font-medium text-slate-900 dark:text-zinc-50">{s.name}</td>
                    <td className="px-4 py-3">
                      <div className="max-w-[460px] truncate" title={renderAddress(s)}>
                        {renderAddress(s) || <span className="text-slate-400 dark:text-zinc-500">—</span>}
                      </div>
                    </td>
                    <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{s.province || '—'}</td>
                    <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{s.district || '—'}</td>
                    <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{s.hotline?.trim() ? s.hotline : '—'}</td>
                  </tr>
                ))}

                {!listQ.isLoading && rows.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-4 py-10 text-center text-sm text-slate-500 dark:text-zinc-300">
                      Chưa có showroom nào.
                    </td>
                  </tr>
                ) : null}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {dialogOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between gap-3 border-b border-slate-200 px-5 py-4 dark:border-zinc-800">
              <div>
                <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">Thêm mới showroom</div>
                <div className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Nhập tên, địa chỉ và thông tin liên hệ.</div>
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
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tên showroom</label>
                <input
                  value={form.name}
                  onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Ví dụ: Showroom Quận 1"
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tỉnh/Thành phố</label>
                <input
                  value={form.province}
                  onChange={(e) => setForm((f) => ({ ...f, province: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Hồ Chí Minh"
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Quận/Huyện</label>
                <input
                  value={form.district}
                  onChange={(e) => setForm((f) => ({ ...f, district: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Quận 1"
                />
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Địa chỉ</label>
                <input
                  value={form.streetAddress}
                  onChange={(e) => setForm((f) => ({ ...f, streetAddress: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="123 Nguyễn Huệ"
                />
                <div className="mt-2 text-xs text-slate-500 dark:text-zinc-300">
                  Preview: <span className="text-slate-700 dark:text-zinc-200">{renderAddress({ showroomId: 0, fullAddress: null, ...form } as any) || '—'}</span>
                </div>
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Hotline (optional)</label>
                <input
                  value={form.hotline ?? ''}
                  onChange={(e) => setForm((f) => ({ ...f, hotline: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="0909xxxxxx"
                />
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

      {carsDialogOpen && selectedShowroom ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-5xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between gap-3 border-b border-slate-200 px-5 py-4 dark:border-zinc-800">
              <div>
                <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">
                  Xe trong showroom: {selectedShowroom.name}
                </div>
                <div className="mt-1 text-sm text-slate-500 dark:text-zinc-300">{renderAddress(selectedShowroom)}</div>
              </div>
              <button
                type="button"
                onClick={closeCarsDialog}
                className="rounded-md p-2 text-slate-600 hover:bg-slate-100 dark:text-zinc-300 dark:hover:bg-zinc-900"
                aria-label="Close"
              >
                <X size={18} />
              </button>
            </div>

            <div className="border-b border-slate-200 px-5 py-3 text-sm text-slate-600 dark:border-zinc-800 dark:text-zinc-300">
              {carsQ.isLoading ? 'Đang tải...' : `Tổng: ${inventory.length} xe`}
              {carsQ.isFetching && !carsQ.isLoading ? ' (đang tải lại...)' : ''}
              <button
                type="button"
                className="ml-3 inline-flex h-9 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                onClick={() => carsQ.refetch()}
                disabled={carsQ.isFetching}
                title="Tải lại danh sách xe"
              >
                <RefreshCcw size={16} />
                Tải lại
              </button>
            </div>

            {carsQ.isError ? (
              <div className="p-5 text-sm text-red-600 dark:text-red-400">{getErrorMessage(carsQ.error)}</div>
            ) : (
              <div className="w-full overflow-x-auto">
                <table className="min-w-[1100px] w-full text-left text-sm">
                  <thead className="bg-slate-50 text-slate-600 dark:bg-zinc-900/50 dark:text-zinc-300">
                    <tr>
                      <th className="px-4 py-3 font-medium">Xe</th>
                      <th className="px-4 py-3 font-medium">Giá</th>
                      <th className="px-4 py-3 font-medium">Tồn</th>
                      <th className="px-4 py-3 font-medium">Trạng thái</th>
                      <th className="px-4 py-3 font-medium">Hãng</th>
                      <th className="px-4 py-3 font-medium">Phân khúc</th>
                      <th className="px-4 py-3 font-medium">Nhiên liệu</th>
                      <th className="px-4 py-3 font-medium">Hộp số</th>
                      <th className="px-4 py-3 font-medium">Màu</th>
                      <th className="px-4 py-3 font-medium">Năm</th>
                      <th className="px-4 py-3 font-medium">Xuất xứ</th>
                    </tr>
                  </thead>
                  <tbody>
                    {inventory.map((c) => {
                      const img = resolveImageUrl(c.mainImageUrl)
                      return (
                        <tr key={c.carId} className="border-t border-slate-100 dark:border-zinc-900">
                          <td className="px-4 py-3">
                            <div className="flex items-center gap-3">
                              <div className="h-10 w-14 overflow-hidden rounded-md border border-slate-200 bg-slate-50 dark:border-zinc-800 dark:bg-zinc-900">
                                {img ? <img src={img} alt={c.name} className="h-full w-full object-cover" /> : null}
                              </div>
                              <div>
                                <div className="font-medium text-slate-900 dark:text-zinc-50">{c.name}</div>
                                <div className="text-xs text-slate-500 dark:text-zinc-400">#{c.carId}</div>
                              </div>
                            </div>
                          </td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">
                            {Number(c.price ?? 0).toLocaleString('vi-VN')}
                          </td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.quantity ?? 0}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.displayStatus || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.brandName || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.segmentName || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.fuelTypeName || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.transmissionName || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.colorName || '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.modelYear ?? '—'}</td>
                          <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{c.origin || '—'}</td>
                        </tr>
                      )
                    })}

                    {!carsQ.isLoading && inventory.length === 0 ? (
                      <tr>
                        <td
                          colSpan={11}
                          className="px-4 py-10 text-center text-sm text-slate-500 dark:text-zinc-300"
                        >
                          Showroom này chưa có xe tồn kho.
                        </td>
                      </tr>
                    ) : null}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      ) : null}
    </div>
  )
}

