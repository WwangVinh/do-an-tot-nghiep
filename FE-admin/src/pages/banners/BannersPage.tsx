import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Plus, RefreshCcw, Save, UploadCloud, X } from 'lucide-react'

import { env } from '../../lib/env'
import type { Banner, BannerUpsertInput } from '../../services/banners/banners'
import { createBanner, fetchBanners, updateBanner, uploadBannerImage } from '../../services/banners/banners'

function resolveImageUrl(raw: string | null | undefined): string {
  const s = (raw ?? '').trim()
  if (!s) return ''
  if (s.startsWith('http://') || s.startsWith('https://') || s.startsWith('data:')) return s
  if (s.startsWith('/')) return `${env.VITE_API_BASE_URL}${s}`
  return s
}

function toDateTimeLocalValue(iso: string | null | undefined): string {
  if (!iso) return ''
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return ''
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}

function fromDateTimeLocalValue(v: string): string | null {
  const s = v.trim()
  if (!s) return null
  const d = new Date(s)
  if (Number.isNaN(d.getTime())) return null
  return d.toISOString()
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

function emptyForm(): BannerUpsertInput {
  return {
    bannerName: '',
    imageUrl: '',
    linkUrl: '',
    position: 0,
    isActive: true,
    startDate: null,
    endDate: null,
  }
}

export function BannersPage() {
  const qc = useQueryClient()
  const [onlyActive, setOnlyActive] = useState<boolean | undefined>(undefined)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editing, setEditing] = useState<Banner | null>(null)
  const [form, setForm] = useState<BannerUpsertInput>(() => emptyForm())
  const [imageFile, setImageFile] = useState<File | null>(null)

  const qKey = useMemo(() => ['banners', { onlyActive }], [onlyActive])

  const bannersQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchBanners({ isActive: onlyActive }),
  })

  const createM = useMutation({
    mutationFn: (input: BannerUpsertInput) => createBanner(input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Tạo banner thành công')
      setDialogOpen(false)
      setEditing(null)
      setForm(emptyForm())
      await qc.invalidateQueries({ queryKey: ['banners'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const updateM = useMutation({
    mutationFn: (vars: { bannerId: number; input: BannerUpsertInput }) => updateBanner(vars.bannerId, vars.input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Cập nhật banner thành công')
      setDialogOpen(false)
      setEditing(null)
      setForm(emptyForm())
      await qc.invalidateQueries({ queryKey: ['banners'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const submitting = createM.isPending || updateM.isPending
  const rows = bannersQ.data ?? []

  function openCreate() {
    setEditing(null)
    setForm(emptyForm())
    setImageFile(null)
    setDialogOpen(true)
  }

  function openEdit(b: Banner) {
    setEditing(b)
    setForm({
      bannerName: b.bannerName ?? '',
      imageUrl: b.imageUrl ?? '',
      linkUrl: b.linkUrl ?? '',
      position: b.position ?? 0,
      isActive: Boolean(b.isActive),
      startDate: b.startDate ?? null,
      endDate: b.endDate ?? null,
    })
    setImageFile(null)
    setDialogOpen(true)
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
    setEditing(null)
    setForm(emptyForm())
    setImageFile(null)
  }

  async function onUploadImage() {
    if (!imageFile) {
      toast.error('Vui lòng chọn file ảnh')
      return
    }
    try {
      const res = await uploadBannerImage(imageFile, form.bannerName?.trim() || undefined)
      setForm((f) => ({ ...f, imageUrl: res.imageUrl }))
      toast.success(res.message ?? 'Upload ảnh thành công')
    } catch (e: unknown) {
      toast.error(getErrorMessage(e))
    }
  }

  function submit() {
    const bannerName = form.bannerName.trim()
    const imageUrl = form.imageUrl.trim()
    const linkUrl = form.linkUrl?.trim() ?? ''

    if (!bannerName) {
      toast.error('Vui lòng nhập tên banner')
      return
    }
    if (!imageUrl) {
      toast.error('Vui lòng nhập ImageUrl')
      return
    }

    const input: BannerUpsertInput = {
      bannerName,
      imageUrl,
      linkUrl: linkUrl ? linkUrl : null,
      position: Number.isFinite(form.position) ? Number(form.position) : 0,
      isActive: Boolean(form.isActive),
      startDate: form.startDate ?? null,
      endDate: form.endDate ?? null,
    }

    if (!editing) createM.mutate(input)
    else updateM.mutate({ bannerId: editing.bannerId, input })
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Banner</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý banner (bảng `Banners`).</p>
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            className="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
            onClick={() => bannersQ.refetch()}
            disabled={bannersQ.isFetching}
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
            Thêm banner
          </button>
        </div>
      </div>

      <div className="mt-4 flex flex-wrap items-center gap-3 text-sm">
        <div className="text-slate-600 dark:text-zinc-300">Lọc:</div>
        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => setOnlyActive(undefined)}
            className={[
              'h-9 rounded-md border px-3',
              onlyActive === undefined
                ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
            ].join(' ')}
          >
            Tất cả
          </button>
          <button
            type="button"
            onClick={() => setOnlyActive(true)}
            className={[
              'h-9 rounded-md border px-3',
              onlyActive === true
                ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
            ].join(' ')}
          >
            Đang active
          </button>
          <button
            type="button"
            onClick={() => setOnlyActive(false)}
            className={[
              'h-9 rounded-md border px-3',
              onlyActive === false
                ? 'border-slate-900 bg-slate-900 text-white dark:border-white dark:bg-white dark:text-zinc-900'
                : 'border-slate-200 bg-white text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
            ].join(' ')}
          >
            Inactive
          </button>
        </div>
      </div>

      <div className="mt-6 overflow-hidden rounded-lg border border-slate-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <div className="border-b border-slate-200 px-4 py-3 text-sm text-slate-600 dark:border-zinc-800 dark:text-zinc-300">
          {bannersQ.isLoading ? 'Đang tải...' : `Tổng: ${rows.length} banner`}
          {bannersQ.isFetching && !bannersQ.isLoading ? ' (đang tải lại...)' : ''}
        </div>

        {bannersQ.isError ? (
          <div className="p-4 text-sm text-red-600 dark:text-red-400">Không thể tải danh sách banner.</div>
        ) : (
          <div className="w-full overflow-x-auto">
            <table className="min-w-[900px] w-full text-left text-sm">
              <thead className="bg-slate-50 text-slate-600 dark:bg-zinc-900/50 dark:text-zinc-300">
                <tr>
                  <th className="px-4 py-3 font-medium">ID</th>
                  <th className="px-4 py-3 font-medium">Tên</th>
                  <th className="px-4 py-3 font-medium">Ảnh</th>
                  <th className="px-4 py-3 font-medium">Link</th>
                  <th className="px-4 py-3 font-medium">Vị trí</th>
                  <th className="px-4 py-3 font-medium">Trạng thái</th>
                  <th className="px-4 py-3 font-medium">Thời gian</th>
                  <th className="px-4 py-3 font-medium">Hành động</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((b) => (
                  <tr key={b.bannerId} className="border-t border-slate-100 dark:border-zinc-900">
                    <td className="px-4 py-3 text-slate-600 dark:text-zinc-300">{b.bannerId}</td>
                    <td className="px-4 py-3 font-medium text-slate-900 dark:text-zinc-50">{b.bannerName}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <div className="h-10 w-16 overflow-hidden rounded-md border border-slate-200 bg-slate-50 dark:border-zinc-800 dark:bg-zinc-900">
                          {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
                          <img
                            src={resolveImageUrl(b.imageUrl)}
                            alt={b.bannerName}
                            className="h-full w-full object-cover"
                            onError={(e) => {
                              ;(e.currentTarget as HTMLImageElement).style.display = 'none'
                            }}
                          />
                        </div>
                        <div className="max-w-[260px] truncate text-xs text-slate-600 dark:text-zinc-300">{b.imageUrl}</div>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      {b.linkUrl ? (
                        <a
                          href={b.linkUrl}
                          target="_blank"
                          rel="noreferrer"
                          className="max-w-[240px] truncate text-indigo-600 hover:underline dark:text-indigo-400"
                        >
                          {b.linkUrl}
                        </a>
                      ) : (
                        <span className="text-slate-400 dark:text-zinc-500">—</span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-slate-700 dark:text-zinc-200">{b.position}</td>
                    <td className="px-4 py-3">
                      <span
                        className={[
                          'inline-flex items-center rounded-full px-2 py-1 text-xs font-medium',
                          b.isActive
                            ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-300'
                            : 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300',
                        ].join(' ')}
                      >
                        {b.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-xs text-slate-600 dark:text-zinc-300">
                      <div>
                        <div>
                          <span className="text-slate-400 dark:text-zinc-500">Start:</span>{' '}
                          {b.startDate ? new Date(b.startDate).toLocaleString() : '—'}
                        </div>
                        <div className="mt-1">
                          <span className="text-slate-400 dark:text-zinc-500">End:</span>{' '}
                          {b.endDate ? new Date(b.endDate).toLocaleString() : '—'}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <button
                        type="button"
                        onClick={() => openEdit(b)}
                        className="h-9 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                      >
                        Sửa
                      </button>
                    </td>
                  </tr>
                ))}

                {!bannersQ.isLoading && rows.length === 0 ? (
                  <tr>
                    <td colSpan={8} className="px-4 py-10 text-center text-sm text-slate-500 dark:text-zinc-300">
                      Chưa có banner nào.
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
                <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">
                  {editing ? `Cập nhật banner #${editing.bannerId}` : 'Thêm mới banner'}
                </div>
                <div className="mt-1 text-sm text-slate-500 dark:text-zinc-300">
                  Nhập đầy đủ `BannerName` và `ImageUrl`.
                </div>
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
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tên banner</label>
                <input
                  value={form.bannerName}
                  onChange={(e) => setForm((f) => ({ ...f, bannerName: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="Ví dụ: Khuyến mãi mùa hè"
                />
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Ảnh banner</label>
                <div className="mt-2 grid gap-2 md:grid-cols-[1fr_auto]">
                  <input
                    type="file"
                    accept="image/*"
                    onChange={(e) => setImageFile(e.currentTarget.files?.item(0) ?? null)}
                    className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none file:mr-3 file:rounded-md file:border-0 file:bg-slate-900 file:px-3 file:py-2 file:text-sm file:font-medium file:text-white hover:file:bg-slate-800 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50 dark:file:bg-white dark:file:text-zinc-900 dark:hover:file:bg-zinc-100"
                  />
                  <button
                    type="button"
                    onClick={() => void onUploadImage()}
                    disabled={submitting || !imageFile}
                    className="inline-flex h-11 items-center justify-center gap-2 rounded-lg bg-slate-900 px-4 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
                    title="Upload ảnh lên server"
                  >
                    <UploadCloud size={16} />
                    Upload
                  </button>
                </div>
                <div className="mt-2 text-xs text-slate-500 dark:text-zinc-300">
                  Sau khi upload, hệ thống sẽ tự điền `ImageUrl` (đường dẫn `/uploads/...`).
                </div>

                <div className="mt-3 grid gap-3 md:grid-cols-2">
                  <div>
                    <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">ImageUrl</label>
                    <input
                      value={form.imageUrl}
                      readOnly
                      className="mt-1 h-10 w-full rounded-lg border border-slate-200 bg-slate-50 px-3 text-xs text-slate-700 outline-none dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-200"
                      placeholder="/uploads/Banners/..."
                    />
                  </div>
                  <div className="flex items-end">
                    <div className="h-24 w-full overflow-hidden rounded-lg border border-slate-200 bg-slate-50 dark:border-zinc-800 dark:bg-zinc-900">
                      {form.imageUrl ? (
                        <img src={resolveImageUrl(form.imageUrl)} alt={form.bannerName} className="h-full w-full object-cover" />
                      ) : (
                        <div className="flex h-full items-center justify-center text-xs text-slate-500 dark:text-zinc-300">
                          Chưa có ảnh
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">LinkUrl (optional)</label>
                <input
                  value={form.linkUrl ?? ''}
                  onChange={(e) => setForm((f) => ({ ...f, linkUrl: e.target.value }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                  placeholder="https://..."
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Vị trí</label>
                <input
                  value={String(form.position ?? 0)}
                  onChange={(e) => setForm((f) => ({ ...f, position: Number(e.target.value) }))}
                  type="number"
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                />
              </div>

              <div className="flex items-end gap-3">
                <label className="flex items-center gap-2 text-sm text-slate-700 dark:text-zinc-200">
                  <input
                    type="checkbox"
                    checked={form.isActive}
                    onChange={(e) => setForm((f) => ({ ...f, isActive: e.target.checked }))}
                    className="h-4 w-4 rounded border-slate-300 text-indigo-600 focus:ring-indigo-500/30 dark:border-zinc-700 dark:bg-zinc-900"
                  />
                  Active
                </label>
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">StartDate (optional)</label>
                <input
                  type="datetime-local"
                  value={toDateTimeLocalValue(form.startDate)}
                  onChange={(e) => setForm((f) => ({ ...f, startDate: fromDateTimeLocalValue(e.target.value) }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">EndDate (optional)</label>
                <input
                  type="datetime-local"
                  value={toDateTimeLocalValue(form.endDate)}
                  onChange={(e) => setForm((f) => ({ ...f, endDate: fromDateTimeLocalValue(e.target.value) }))}
                  className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
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
    </div>
  )
}

