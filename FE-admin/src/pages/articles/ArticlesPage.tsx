import { useMemo, useState, useEffect, useRef } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Pencil, Plus, RefreshCcw, Save, Trash2, X, Image as ImageIcon, Search, ChevronDown, Eye } from 'lucide-react'

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

// ── Rich Text Editor (dùng contentEditable + toolbar đơn giản, không cần thư viện) ──
function RichEditor({ value, onChange }: { value: string; onChange: (v: string) => void }) {
  const editorRef = useRef<HTMLDivElement>(null)
  const [preview, setPreview] = useState(false)

  // Set nội dung ban đầu khi mở edit
  useEffect(() => {
    if (editorRef.current && editorRef.current.innerHTML !== value) {
      editorRef.current.innerHTML = value
    }
  }, []) // chỉ chạy 1 lần khi mount

  const savedRange = useRef<Range | null>(null)

  function saveSelection() {
    const sel = window.getSelection()
    if (sel && sel.rangeCount > 0) {
      savedRange.current = sel.getRangeAt(0).cloneRange()
    }
  }

  function restoreSelection() {
    const sel = window.getSelection()
    if (sel && savedRange.current) {
      sel.removeAllRanges()
      sel.addRange(savedRange.current)
    }
  }

  function exec(cmd: string, val?: string) {
    document.execCommand(cmd, false, val)
    editorRef.current?.focus()
  }

  function handleInput() {
    onChange(editorRef.current?.innerHTML ?? '')
  }

  const tools = [
    { label: 'B', cmd: 'bold', title: 'In đậm', style: 'font-bold' },
    { label: 'I', cmd: 'italic', title: 'In nghiêng', style: 'italic' },
    { label: 'U', cmd: 'underline', title: 'Gạch chân', style: 'underline' },
  ]

  function insertHeading() { exec('formatBlock', '<h2>') }
  function insertPara() { exec('formatBlock', '<p>') }
  function insertUL() { exec('insertUnorderedList') }
  function insertOL() { exec('insertOrderedList') }

  function insertLink() {
    saveSelection()
    const sel = window.getSelection()
    const selectedText = sel && sel.rangeCount > 0 ? sel.getRangeAt(0).toString().trim() : ''

    const url = prompt('Nhập URL link:')
    if (!url) return

    // Nếu đang select sẵn text thì dùng text đó, không thì hỏi tên hiển thị
    const defaultLabel = selectedText || ''
    const label = prompt('Tên hiển thị (để trống = dùng URL):', defaultLabel) ?? defaultLabel
    const displayText = label.trim() || url

    restoreSelection()

    // Insert HTML thay vì createLink để kiểm soát được label
    exec('insertHTML', `<a href="${url}" target="_blank" rel="noopener noreferrer">${displayText}</a>`)
    onChange(editorRef.current?.innerHTML ?? '')
  }

  function insertImage() {
    saveSelection()
    const url = prompt('Nhập URL ảnh:')
    if (url) {
      restoreSelection()
      exec('insertImage', url)
      onChange(editorRef.current?.innerHTML ?? '')
    }
  }

  const [tableDialog, setTableDialog] = useState(false)
  const [tableRows, setTableRows] = useState(3)
  const [tableCols, setTableCols] = useState(3)

  function insertTable() {
    saveSelection()
    setTableDialog(true)
  }

  function confirmInsertTable() {
    const rows = Math.max(1, Math.min(20, tableRows))
    const cols = Math.max(1, Math.min(10, tableCols))

    const headerCells = Array.from({ length: cols }, (_, i) =>
      `<th style="border:1px solid #cbd5e1;padding:8px 12px;background:#f8fafc;font-weight:600;text-align:left">Tiêu đề ${i + 1}</th>`
    ).join('')

    const bodyRow = Array.from({ length: cols }, () =>
      `<td style="border:1px solid #cbd5e1;padding:8px 12px">&nbsp;</td>`
    ).join('')
    const bodyRows = Array.from({ length: rows }, () => `<tr>${bodyRow}</tr>`).join('')

    const tableHtml = `<table style="border-collapse:collapse;width:100%;margin:12px 0"><thead><tr>${headerCells}</tr></thead><tbody>${bodyRows}</tbody></table><p><br></p>`

    restoreSelection()
    exec('insertHTML', tableHtml)
    onChange(editorRef.current?.innerHTML ?? '')
    setTableDialog(false)
  }

  return (
    <div className="relative rounded-xl border border-slate-200 dark:border-zinc-700 overflow-hidden">
      {/* Toolbar */}
      <div className="flex flex-wrap items-center gap-1 border-b border-slate-200 bg-slate-50 px-2 py-1.5 dark:border-zinc-700 dark:bg-zinc-900">
        {tools.map((t) => (
          <button
            key={t.cmd}
            type="button"
            title={t.title}
            onMouseDown={(e) => { e.preventDefault(); exec(t.cmd) }}
            className={`h-7 w-7 rounded text-sm ${t.style} flex items-center justify-center text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700`}
          >
            {t.label}
          </button>
        ))}
        <div className="mx-1 h-5 w-px bg-slate-200 dark:bg-zinc-700" />
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertHeading() }}
          className="rounded px-2 py-1 text-xs font-semibold text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700">
          H2
        </button>
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertPara() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700">
          ¶
        </button>
        <div className="mx-1 h-5 w-px bg-slate-200 dark:bg-zinc-700" />
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertUL() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700" title="Danh sách chấm">
          • List
        </button>
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertOL() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700" title="Danh sách số">
          1. List
        </button>
        <div className="mx-1 h-5 w-px bg-slate-200 dark:bg-zinc-700" />
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertLink() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700">
          Link
        </button>
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertImage() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700">
          Img
        </button>
        <button type="button" onMouseDown={(e) => { e.preventDefault(); insertTable() }}
          className="rounded px-2 py-1 text-xs text-slate-600 hover:bg-slate-200 dark:text-zinc-300 dark:hover:bg-zinc-700" title="Chèn bảng">
          ⊞ Bảng
        </button>
        <div className="ml-auto">
          <button type="button" onClick={() => setPreview((v) => !v)}
            className={`flex items-center gap-1 rounded px-2 py-1 text-xs font-medium transition ${preview ? 'bg-indigo-100 text-indigo-700 dark:bg-indigo-900/40 dark:text-indigo-300' : 'text-slate-500 hover:bg-slate-200 dark:hover:bg-zinc-700'}`}>
            <Eye size={12} /> {preview ? 'Editor' : 'Preview'}
          </button>
        </div>
      </div>

      {/* Editor / Preview */}
      {preview ? (
        <div
          className="prose prose-sm max-w-none min-h-[200px] p-4 dark:prose-invert prose-headings:font-bold prose-p:text-slate-700 dark:prose-p:text-zinc-300"
          dangerouslySetInnerHTML={{ __html: value }}
        />
      ) : (
        <div
          ref={editorRef}
          contentEditable
          suppressContentEditableWarning
          onInput={handleInput}
          className="min-h-[200px] p-4 text-sm leading-relaxed text-slate-800 outline-none dark:text-zinc-100
            [&_h2]:text-lg [&_h2]:font-bold [&_h2]:mb-2 [&_h2]:mt-4
            [&_p]:mb-3 [&_ul]:list-disc [&_ul]:pl-5 [&_ol]:list-decimal [&_ol]:pl-5
            [&_a]:text-indigo-600 [&_a]:underline [&_img]:rounded-lg [&_img]:max-w-full
            [&_table]:w-full [&_table]:border-collapse [&_table]:my-3
            [&_td]:border [&_td]:border-slate-300 [&_td]:p-2
            [&_th]:border [&_th]:border-slate-300 [&_th]:p-2 [&_th]:bg-slate-50 [&_th]:font-semibold"
        />
      )}

      {/* Table dialog */}
      {tableDialog && (
        <div className="absolute inset-0 z-10 flex items-center justify-center bg-black/30 rounded-xl">
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-xl dark:border-zinc-700 dark:bg-zinc-900 w-64">
            <p className="mb-4 text-sm font-semibold text-slate-800 dark:text-zinc-100">Chèn bảng</p>
            <div className="space-y-3">
              <div>
                <label className="text-xs text-slate-500 dark:text-zinc-400">Số hàng</label>
                <input
                  type="number" min={1} max={20} value={tableRows}
                  onChange={(e) => setTableRows(Number(e.target.value))}
                  className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100"
                />
              </div>
              <div>
                <label className="text-xs text-slate-500 dark:text-zinc-400">Số cột</label>
                <input
                  type="number" min={1} max={10} value={tableCols}
                  onChange={(e) => setTableCols(Number(e.target.value))}
                  className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100"
                />
              </div>
            </div>
            <div className="mt-4 flex gap-2">
              <button
                type="button"
                onClick={() => setTableDialog(false)}
                className="flex-1 rounded-lg border border-slate-200 py-2 text-xs text-slate-600 hover:bg-slate-50 dark:border-zinc-700 dark:text-zinc-300"
              >
                Hủy
              </button>
              <button
                type="button"
                onClick={confirmInsertTable}
                className="flex-1 rounded-lg bg-indigo-600 py-2 text-xs font-medium text-white hover:bg-indigo-700"
              >
                Chèn
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

// ── Car selector ────────────────────────────────────────────────────────────
function CarSelector({ selected, onChange }: { selected: number[]; onChange: (ids: number[]) => void }) {
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

  return (
    <div>
      <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Xe liên quan</label>
      <p className="mt-0.5 mb-2 text-xs text-slate-400">Bài viết sẽ hiển thị ở trang chi tiết các xe được chọn.</p>

      {selectedCars.length > 0 && (
        <div className="mb-2 flex flex-wrap gap-1.5">
          {selectedCars.map((c) => (
            <span key={c.carId}
              className="inline-flex items-center gap-1 rounded-full bg-indigo-50 px-2.5 py-1 text-xs font-medium text-indigo-700 dark:bg-indigo-950/50 dark:text-indigo-300">
              {c.name}
              <button type="button" onClick={() => toggle(c.carId)}
                className="ml-0.5 rounded-full p-0.5 hover:bg-indigo-100 dark:hover:bg-indigo-900">
                <X size={10} />
              </button>
            </span>
          ))}
        </div>
      )}

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
            <div className="p-2 border-b border-slate-100 dark:border-zinc-800">
              <div className="flex items-center gap-2 rounded-md border border-slate-200 bg-slate-50 px-3 py-1.5 dark:border-zinc-700 dark:bg-zinc-800">
                <Search size={13} className="text-slate-400" />
                <input autoFocus value={carSearch} onChange={(e) => setCarSearch(e.target.value)}
                  placeholder="Tìm xe theo tên, hãng..."
                  className="flex-1 bg-transparent text-sm outline-none placeholder:text-slate-400 dark:text-zinc-100" />
                {carSearch && (
                  <button type="button" onClick={() => setCarSearch('')}>
                    <X size={12} className="text-slate-400 hover:text-slate-600" />
                  </button>
                )}
              </div>
            </div>
            <div className="max-h-52 overflow-y-auto py-1">
              {carsQ.isLoading && <div className="px-3 py-2 text-sm text-slate-400">Đang tải...</div>}
              {!carsQ.isLoading && filtered.length === 0 && <div className="px-3 py-2 text-sm text-slate-400">Không tìm thấy xe.</div>}
              {filtered.map((car) => {
                const isSelected = selected.includes(car.carId)
                return (
                  <button key={car.carId} type="button" onClick={() => toggle(car.carId)}
                    className={`flex w-full items-center gap-3 px-3 py-2 text-left text-sm transition-colors ${
                      isSelected ? 'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-300' : 'text-slate-700 hover:bg-slate-50 dark:text-zinc-300 dark:hover:bg-zinc-800'
                    }`}>
                    <span className={`flex h-4 w-4 shrink-0 items-center justify-center rounded border text-[10px] ${
                      isSelected ? 'border-indigo-600 bg-indigo-600 text-white' : 'border-slate-300 dark:border-zinc-600'
                    }`}>
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

// ── Main page ───────────────────────────────────────────────────────────────
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
    setForm({ title: '', content: '<p></p>', thumbnail: '', isPublished: 'true', relatedCarIds: [] })
    setDialogOpen(true)
  }

  async function openEdit(a: ArticleResponseDto) {
    try {
      const detail = await getArticleDetail(a.articleId)
      setEditingId(detail.articleId)
      setForm({
        title: detail.title || '',
        content: detail.content || '<p></p>',
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
    const stripped = form.content.replace(/<[^>]*>/g, '').trim()
    if (!stripped) return toast.error('Vui lòng nhập nội dung')

    const dto: ArticleSubmitDto = {
      title: form.title.trim(),
      content: form.content,
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

      {/* Filter */}
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

        {/* Table */}
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
                      <button
                        onClick={() => { if (confirm('Xóa bài viết này?')) deleteM.mutate(a.articleId) }}
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

        {/* Pagination */}
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

      {/* ── MODAL ── */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div className="absolute inset-0 bg-black/60 backdrop-blur-sm" onClick={closeDialog} />

          <div className="relative flex flex-col w-full max-w-4xl max-h-[90vh] rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            {/* Modal header */}
            <div className="flex items-center justify-between border-b px-6 py-4 dark:border-zinc-800">
              <div>
                <h2 className="text-base font-semibold text-slate-900 dark:text-zinc-50">
                  {editingId ? `Chỉnh sửa bài viết #${editingId}` : 'Đăng bài viết mới'}
                </h2>
                <p className="mt-0.5 text-xs text-slate-400">Soạn nội dung và xuất bản lên trang tin tức</p>
              </div>
              <button onClick={closeDialog} disabled={submitting}
                className="rounded-lg p-2 text-slate-400 hover:bg-slate-100 hover:text-slate-600 dark:hover:bg-zinc-900">
                <X size={18} />
              </button>
            </div>

            <div className="grid grid-cols-1 gap-0 lg:grid-cols-[1fr_320px] overflow-y-auto flex-1">
              {/* Cột trái: nội dung chính */}
              <div className="space-y-5 p-6 lg:border-r lg:dark:border-zinc-800">
                {/* Tiêu đề */}
                <div>
                  <label className="text-xs font-semibold uppercase tracking-wider text-slate-500 dark:text-zinc-400">
                    Tiêu đề bài viết *
                  </label>
                  <input
                    value={form.title}
                    onChange={(e) => setForm({ ...form, title: e.target.value })}
                    className="mt-2 w-full rounded-xl border border-slate-200 px-4 py-3 text-base font-semibold text-slate-900 outline-none placeholder:font-normal placeholder:text-slate-300 focus:border-indigo-400 focus:ring-2 focus:ring-indigo-500/20 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-50"
                    placeholder="Nhập tiêu đề hấp dẫn..."
                  />
                </div>

                {/* Nội dung rich text */}
                <div>
                  <label className="text-xs font-semibold uppercase tracking-wider text-slate-500 dark:text-zinc-400">
                    Nội dung *
                  </label>
                  <div className="mt-2">
                    <RichEditor
                      value={form.content}
                      onChange={(v) => setForm({ ...form, content: v })}
                    />
                  </div>
                </div>
              </div>

              {/* Cột phải: sidebar settings */}
              <div className="space-y-5 p-6 bg-slate-50/50 dark:bg-zinc-900/30">
                {/* Trạng thái */}
                <div>
                  <label className="text-xs font-semibold uppercase tracking-wider text-slate-500 dark:text-zinc-400">
                    Trạng thái
                  </label>
                  <div className="mt-2 flex gap-2">
                    {[
                      { val: 'true', label: 'Xuất bản', color: 'emerald' },
                      { val: 'false', label: 'Lưu nháp', color: 'amber' },
                    ].map((opt) => (
                      <button
                        key={opt.val}
                        type="button"
                        onClick={() => setForm({ ...form, isPublished: opt.val })}
                        className={`flex-1 rounded-lg border py-2 text-sm font-medium transition ${
                          form.isPublished === opt.val
                            ? opt.color === 'emerald'
                              ? 'border-emerald-500 bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300'
                              : 'border-amber-400 bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-300'
                            : 'border-slate-200 bg-white text-slate-500 hover:bg-slate-50 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-400'
                        }`}
                      >
                        {opt.label}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Thumbnail */}
                <div>
                  <label className="text-xs font-semibold uppercase tracking-wider text-slate-500 dark:text-zinc-400">
                    Ảnh thumbnail
                  </label>
                  <input
                    value={form.thumbnail}
                    onChange={(e) => setForm({ ...form, thumbnail: e.target.value })}
                    className="mt-2 w-full rounded-xl border border-slate-200 px-3 py-2.5 text-sm outline-none focus:border-indigo-400 focus:ring-2 focus:ring-indigo-500/20 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-100"
                    placeholder="https://..."
                  />
                  {form.thumbnail ? (
                    <div className="mt-2 overflow-hidden rounded-xl border border-slate-200 dark:border-zinc-700">
                      <img src={form.thumbnail} alt="preview" className="h-28 w-full object-cover" />
                    </div>
                  ) : (
                    <div className="mt-2 flex h-28 items-center justify-center rounded-xl border border-dashed border-slate-200 bg-slate-50 dark:border-zinc-700 dark:bg-zinc-900">
                      <span className="text-xs text-slate-400">Chưa có ảnh</span>
                    </div>
                  )}
                </div>

                {/* Xe liên quan */}
                <CarSelector
                  selected={form.relatedCarIds}
                  onChange={(ids) => setForm({ ...form, relatedCarIds: ids })}
                />
              </div>
            </div>

            {/* Modal footer */}
            <div className="flex items-center justify-end gap-3 border-t px-6 py-4 dark:border-zinc-800">
              <button onClick={closeDialog} disabled={submitting}
                className="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-50 disabled:opacity-60 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-300">
                Hủy
              </button>
              <button onClick={submitForm} disabled={submitting}
                className="inline-flex items-center gap-2 rounded-xl bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-indigo-700 disabled:opacity-60">
                <Save size={15} />
                {submitting ? 'Đang lưu...' : 'Lưu bài viết'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}