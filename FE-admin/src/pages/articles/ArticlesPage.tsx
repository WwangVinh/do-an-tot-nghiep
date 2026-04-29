import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Pencil, Plus, RefreshCcw, Save, Trash2, X, Image as ImageIcon, Search, ChevronDown } from 'lucide-react'

import { http } from '../../services/http/http'
import { fetchAdminCars } from '../../services/cars/cars'

interface ArticleResponseDto {
  articleId: number
  title: string
  thumbnail: string | null
  isPublished: boolean
  createdAt: string
}

interface ArticleSubmitDto {
  title: string
  content: string
  thumbnail: string | null
  isPublished: boolean
  relatedCarIds: number[]
}

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

async function fetchArticles(params: { search?: string; page: number; pageSize: number; isPublished?: boolean }) {
  const res = await http.get('/api/admin/Articles', { params })
  return res.data
}

async function getArticleDetail(id: number) {
  const res = await http.get(`/api/admin/Articles/${id}`)
  return res.data
}

async function createArticle(dto: ArticleSubmitDto) {
  const res = await http.post('/api/admin/Articles', dto)
  return res.data
}

async function updateArticle(id: number, dto: ArticleSubmitDto) {
  const res = await http.put(`/api/admin/Articles/${id}`, dto)
  return res.data
}

async function deleteArticle(id: number) {
  const res = await http.delete(`/api/admin/Articles/${id}`)
  return res.data
}

// ── Car selector component ─────────────────────────────────────────────────
function CarSelector({
  selected,
  onChange,
}: {
  selected: number[]
  onChange: (ids: number[]) => void
}) {
  const [carSearch, setCarSearch] = useState('')
  const [open, setOpen] = useState(false)

  const carsQ = useQuery({
    queryKey: ['admin-cars-select'],
    queryFn: () => fetchAdminCars({ page: 1, pageSize: 500 }),
    staleTime: 5 * 60_000,
  })

  const allCars = carsQ.data?.data ?? []

  const filtered = useMemo(() => {
    const q = carSearch.trim().toLowerCase()
    if (!q) return allCars
    return allCars.filter(
      (c) => c.name.toLowerCase().includes(q) || c.brand.toLowerCase().includes(q) || String(c.carId).includes(q)
    )
  }, [allCars, carSearch])

  const selectedCars = allCars.filter((c) => selected.includes(c.carId))

  function toggle(carId: number) {
    onChange(selected.includes(carId) ? selected.filter((id) => id !== carId) : [...selected, carId])
  }

  function remove(carId: number) {
    onChange(selected.filter((id) => id !== carId))
  }

  return (
    <div>
      <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Xe liên quan</label>
      <p className="mt-0.5 mb-2 text-xs text-slate-400">Bài viết sẽ hiển thị ở trang chi tiết các xe được chọn.</p>

      {/* Tags xe đã chọn */}
      {selectedCars.length > 0 && (
        <div className="mb-2 flex flex-wrap gap-1.5">
          {selectedCars.map((c) => (
            <span key={c.carId}
              className="inline-flex items-center gap-1 rounded-full bg-indigo-50 px-2.5 py-1 text-xs font-medium text-indigo-700 dark:bg-indigo-950/50 dark:text-indigo-300">
              {c.name}
              <button type="button" onClick={() => remove(c.carId)}
                className="ml-0.5 rounded-full hover:bg-indigo-100 dark:hover:bg-indigo-900 p-0.5">
                <X size={10} />
              </button>
            </span>
          ))}
        </div>
      )}

      {/* Dropdown tìm và thêm xe */}
      <div className="relative">
        <button type="button" onClick={() => setOpen((v) => !v)}
          className="flex w-full items-center justify-between rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-500 hover:bg-slate-50 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-400 dark:hover:bg-zinc-800">
          <span className="flex items-center gap-2">
            <Plus size={14} />
            {selected.length === 0 ? 'Thêm xe liên quan...' : `Thêm xe khác (đã chọn ${selected.length})`}
          </span>
          <ChevronDown size={14} className={`transition-transform ${open ? 'rotate-180' : ''}`} />
        </button>

        {open && (
          <div className="absolute z-20 mt-1 w-full rounded-xl border border-slate-200 bg-white shadow-xl dark:border-zinc-700 dark:bg-zinc-900">
            {/* Search */}
            <div className="p-2 border-b border-slate-100 dark:border-zinc-800">
              <div className="flex items-center gap-2 rounded-md border border-slate-200 bg-slate-50 px-3 py-1.5 dark:border-zinc-700 dark:bg-zinc-800">
                <Search size={13} className="text-slate-400" />
                <input
                  autoFocus
                  value={carSearch}
                  onChange={(e) => setCarSearch(e.target.value)}
                  placeholder="Tìm xe theo tên, hãng..."
                  className="flex-1 bg-transparent text-sm outline-none placeholder:text-slate-400 dark:text-zinc-100 dark:placeholder:text-zinc-500"
                />
                {carSearch && (
                  <button type="button" onClick={() => setCarSearch('')}>
                    <X size={12} className="text-slate-400 hover:text-slate-600" />
                  </button>
                )}
              </div>
            </div>

            {/* List */}
            <div className="max-h-52 overflow-y-auto py-1">
              {carsQ.isLoading && <div className="px-3 py-2 text-sm text-slate-400">Đang tải...</div>}
              {!carsQ.isLoading && filtered.length === 0 && (
                <div className="px-3 py-2 text-sm text-slate-400">Không tìm thấy xe.</div>
              )}
              {filtered.map((car) => {
                const isSelected = selected.includes(car.carId)
                return (
                  <button key={car.carId} type="button" onClick={() => toggle(car.carId)}
                    className={[
                      'flex w-full items-center gap-3 px-3 py-2 text-left text-sm transition-colors',
                      isSelected
                        ? 'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-300'
                        : 'text-slate-700 hover:bg-slate-50 dark:text-zinc-300 dark:hover:bg-zinc-800',
                    ].join(' ')}>
                    <span className={[
                      'flex h-4 w-4 flex-shrink-0 items-center justify-center rounded border text-[10px]',
                      isSelected
                        ? 'border-indigo-600 bg-indigo-600 text-white'
                        : 'border-slate-300 dark:border-zinc-600',
                    ].join(' ')}>
                      {isSelected && '✓'}
                    </span>
                    <span className="flex-1 truncate">
                      <span className="font-medium">{car.name}</span>
                      <span className="ml-1.5 text-xs opacity-50">#{car.carId} · {car.brand}</span>
                    </span>
                  </button>
                )
              })}
            </div>

            <div className="border-t border-slate-100 px-3 py-2 dark:border-zinc-800">
              <button type="button" onClick={() => setOpen(false)}
                className="w-full rounded-md bg-slate-900 py-1.5 text-xs font-medium text-white hover:bg-slate-700 dark:bg-white dark:text-zinc-900">
                Xong
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}

// ── Main page ──────────────────────────────────────────────────────────────
export function ArticlesPage() {
  const qc = useQueryClient()

  const [search, setSearch] = useState('')
  const [isPublishedFilter, setIsPublishedFilter] = useState<string>('')
  const [page, setPage] = useState(1)
  const [pageSize] = useState(12)

  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)

  const [form, setForm] = useState({
    title: '',
    content: '',
    thumbnail: '',
    isPublished: 'true',
    relatedCarIds: [] as number[],
  })

  const params = useMemo(() => ({
    search: search.trim() || undefined,
    page,
    pageSize,
    isPublished: isPublishedFilter === '' ? undefined : isPublishedFilter === 'true',
  }), [search, page, pageSize, isPublishedFilter])

  const listQ = useQuery({
    queryKey: ['admin-articles', params],
    queryFn: () => fetchArticles(params),
  })

  const createM = useMutation({
    mutationFn: createArticle,
    onSuccess: async (res) => {
      toast.success(res.message || 'Tạo bài viết thành công')
      closeDialog()
      await qc.invalidateQueries({ queryKey: ['admin-articles'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const updateM = useMutation({
    mutationFn: (vars: { id: number; dto: ArticleSubmitDto }) => updateArticle(vars.id, vars.dto),
    onSuccess: async (res) => {
      toast.success(res.message || 'Cập nhật thành công')
      closeDialog()
      await qc.invalidateQueries({ queryKey: ['admin-articles'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const deleteM = useMutation({
    mutationFn: deleteArticle,
    onSuccess: async (res) => {
      toast.success(res.message || 'Đã xóa bài viết')
      await qc.invalidateQueries({ queryKey: ['admin-articles'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const rows: ArticleResponseDto[] = listQ.data?.data ?? []
  const totalItems = listQ.data?.totalCount ?? 0
  const totalPages = Math.ceil(totalItems / pageSize) || 1
  const submitting = createM.isPending || updateM.isPending

  function openCreate() {
    setEditingId(null)
    setForm({ title: '', content: '', thumbnail: '', isPublished: 'true', relatedCarIds: [] })
    setDialogOpen(true)
  }

  async function openEdit(a: ArticleResponseDto) {
    try {
      const detail = await getArticleDetail(a.articleId)
      setEditingId(detail.articleId)
      setForm({
        title: detail.title || '',
        content: detail.content || '',
        thumbnail: detail.thumbnail || '',
        isPublished: detail.isPublished ? 'true' : 'false',
        relatedCarIds: detail.relatedCars ? detail.relatedCars.map((c: any) => c.carId) : [],
      })
      setDialogOpen(true)
    } catch {
      toast.error('Không thể lấy chi tiết bài viết')
    }
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
  }

  function submitForm() {
    if (!form.title.trim()) return toast.error('Vui lòng nhập tiêu đề')
    if (!form.content.trim()) return toast.error('Vui lòng nhập nội dung')

    const dto: ArticleSubmitDto = {
      title: form.title.trim(),
      content: form.content.trim(),
      thumbnail: form.thumbnail.trim() || null,
      isPublished: form.isPublished === 'true',
      relatedCarIds: form.relatedCarIds,
    }

    if (editingId) updateM.mutate({ id: editingId, dto })
    else createM.mutate(dto)
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Bài viết</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý tin tức, bài viết đánh giá xe, khuyến mãi.</p>
        </div>
        <div className="flex items-center gap-2">
          <button onClick={() => listQ.refetch()} disabled={listQ.isFetching}
            className="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900">
            <RefreshCcw size={16} /> Tải lại
          </button>
          <button onClick={openCreate}
            className="inline-flex h-10 items-center gap-2 rounded-md bg-slate-900 px-3 text-sm font-medium text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100">
            <Plus size={16} /> Viết bài mới
          </button>
        </div>
      </div>

      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Trạng thái</label>
            <select value={isPublishedFilter} onChange={(e) => { setIsPublishedFilter(e.target.value); setPage(1) }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100">
              <option value="">Tất cả</option>
              <option value="true">Đã xuất bản</option>
              <option value="false">Bản nháp</option>
            </select>
          </div>
          <div className="md:col-span-9">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input value={search} onChange={(e) => { setSearch(e.target.value); setPage(1) }}
              placeholder="Nhập tiêu đề bài viết..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100" />
          </div>
        </div>

        <div className="mt-4 overflow-auto">
          <table className="min-w-[900px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3 w-16">ID</th>
                <th className="py-3 pr-3 w-24">Ảnh</th>
                <th className="py-3 pr-3">Tiêu đề</th>
                <th className="py-3 pr-3 w-36">Trạng thái</th>
                <th className="py-3 pr-3 w-40">Ngày tạo</th>
                <th className="py-3 pr-3 w-40">Hành động</th>
              </tr>
            </thead>
            <tbody className="text-slate-700 dark:text-zinc-200">
              {listQ.isLoading && <tr><td colSpan={6} className="py-4 text-center text-slate-500">Đang tải...</td></tr>}
              {!listQ.isLoading && rows.length === 0 && <tr><td colSpan={6} className="py-4 text-center text-slate-500">Không có bài viết nào.</td></tr>}
              {rows.map((a) => (
                <tr key={a.articleId} className="border-b border-slate-100 dark:border-zinc-900">
                  <td className="py-3 pr-3 text-slate-500">#{a.articleId}</td>
                  <td className="py-3 pr-3">
                    {a.thumbnail
                      ? <img src={a.thumbnail} alt="thumb" className="h-10 w-16 rounded object-cover border border-slate-200 dark:border-zinc-800" />
                      : <div className="flex h-10 w-16 items-center justify-center rounded bg-slate-100 text-slate-400 dark:bg-zinc-900"><ImageIcon size={18} /></div>
                    }
                  </td>
                  <td className="py-3 pr-3 font-medium">{a.title}</td>
                  <td className="py-3 pr-3">
                    <span className={`inline-flex items-center rounded-full px-2 py-1 text-xs font-medium ${
                      a.isPublished
                        ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-300'
                        : 'bg-amber-50 text-amber-700 dark:bg-amber-500/10 dark:text-amber-400'
                    }`}>
                      {a.isPublished ? 'Đã xuất bản' : 'Bản nháp'}
                    </span>
                  </td>
                  <td className="py-3 pr-3 text-xs text-slate-500">{new Date(a.createdAt).toLocaleString('vi-VN')}</td>
                  <td className="py-3 pr-3">
                    <div className="flex items-center gap-2">
                      <button onClick={() => openEdit(a)}
                        className="inline-flex h-8 items-center gap-1.5 rounded border border-slate-200 bg-white px-2 text-xs font-medium hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:hover:bg-zinc-900">
                        <Pencil size={14} /> Sửa
                      </button>
                      <button onClick={() => { if (confirm('Xóa bài viết này?')) deleteM.mutate(a.articleId) }}
                        disabled={deleteM.isPending}
                        className="inline-flex h-8 items-center gap-1.5 rounded border border-rose-200 bg-rose-50 px-2 text-xs font-semibold text-rose-700 hover:bg-rose-100 disabled:opacity-60 dark:border-rose-900/40 dark:bg-rose-950/30 dark:text-rose-300">
                        <Trash2 size={14} /> Xóa
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <div className="text-xs text-slate-500">Trang {page}/{totalPages} · {totalItems} bài viết</div>
          <div className="flex gap-2">
            <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)}
              className="rounded border px-2.5 py-1 text-xs disabled:opacity-50 hover:bg-slate-50 dark:border-zinc-800 dark:hover:bg-zinc-900">← Trước</button>
            <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}
              className="rounded border px-2.5 py-1 text-xs disabled:opacity-50 hover:bg-slate-50 dark:border-zinc-800 dark:hover:bg-zinc-900">Sau →</button>
          </div>
        </div>
      </div>

      {/* MODAL */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          {/* Backdrop mờ */}
          <div
            className="absolute inset-0 bg-black/60 backdrop-blur-sm"
            onClick={closeDialog}
          />

          <div className="relative w-full max-w-3xl overflow-hidden rounded-2xl bg-white shadow-2xl dark:bg-zinc-950 border border-slate-200 dark:border-zinc-800 flex flex-col max-h-[90vh]">
            <div className="flex items-center justify-between border-b px-5 py-4 dark:border-zinc-800">
              <h2 className="text-lg font-semibold">{editingId ? `Sửa Bài Viết #${editingId}` : 'Đăng Bài Viết Mới'}</h2>
              <button onClick={closeDialog} disabled={submitting}
                className="p-2 rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-600 dark:hover:bg-zinc-900">
                <X size={18} />
              </button>
            </div>

            <div className="flex-1 overflow-auto p-5 space-y-4">
              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tiêu đề *</label>
                <input value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })}
                  className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm dark:bg-zinc-900 dark:border-zinc-700 outline-none focus:ring-2 focus:ring-indigo-500/30"
                  placeholder="Nhập tiêu đề..." />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700 dark:text-zinc-200 flex justify-between">
                  <span>Nội dung *</span>
                  <span className="text-xs text-slate-400 font-normal">Hỗ trợ text hoặc HTML</span>
                </label>
                <textarea rows={6} value={form.content} onChange={(e) => setForm({ ...form, content: e.target.value })}
                  className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm dark:bg-zinc-900 dark:border-zinc-700 outline-none focus:ring-2 focus:ring-indigo-500/30 font-mono"
                  placeholder="<p>Nhập nội dung...</p>" />
              </div>

              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Link ảnh thumbnail</label>
                  <input value={form.thumbnail} onChange={(e) => setForm({ ...form, thumbnail: e.target.value })}
                    className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm dark:bg-zinc-900 dark:border-zinc-700 outline-none"
                    placeholder="https://..." />
                  {form.thumbnail && (
                    <img src={form.thumbnail} alt="preview" className="mt-2 h-20 w-full rounded-lg object-cover border dark:border-zinc-700" />
                  )}
                </div>
                <div>
                  <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái</label>
                  <select value={form.isPublished} onChange={(e) => setForm({ ...form, isPublished: e.target.value })}
                    className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm dark:bg-zinc-900 dark:border-zinc-700 outline-none">
                    <option value="true">Xuất bản luôn</option>
                    <option value="false">Lưu nháp</option>
                  </select>
                </div>
              </div>

              <CarSelector
                selected={form.relatedCarIds}
                onChange={(ids) => setForm({ ...form, relatedCarIds: ids })}
              />
            </div>

            <div className="border-t p-4 flex justify-end gap-2 dark:border-zinc-800 bg-slate-50/80 dark:bg-zinc-900/50">
              <button onClick={closeDialog} disabled={submitting}
                className="px-4 py-2 text-sm border rounded-lg hover:bg-slate-100 dark:border-zinc-700 dark:hover:bg-zinc-800 dark:text-zinc-200">
                Hủy
              </button>
              <button onClick={submitForm} disabled={submitting}
                className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50 flex items-center gap-2">
                <Save size={16} /> {submitting ? 'Đang lưu...' : 'Lưu bài viết'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}