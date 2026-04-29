import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Pencil, Plus, RefreshCcw, Save, Trash2, X, Ticket } from 'lucide-react'

// Import đúng đường dẫn từ thư mục service mới tạo
import type { Promotion } from '../../services/promotions/promotions'
import { 
  fetchAdminPromotions, 
  createAdminPromotion, 
  updateAdminPromotion, 
  deleteAdminPromotion
} from '../../services/promotions/promotions'

// Hàm bắt lỗi
function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const anyErr = err as { message?: unknown; response?: { data?: unknown } }
    const data = anyErr.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
  }
  return 'Có lỗi xảy ra'
}

export function PromotionsPage() {
  const qc = useQueryClient()
  
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [search, setSearch] = useState('')

  const [form, setForm] = useState({
    code: '',
    description: '',
    discountPercentage: 0,
    startDate: '',
    endDate: '',
    status: 'Active',
    carId: 0,
    maxUsage: 100,
  })

  // 1. Load danh sách
  const listQ = useQuery({
    queryKey: ['admin-promotions'],
    queryFn: fetchAdminPromotions,
  })

  /// 2. Các mutation (Thêm/Sửa/Xóa)
  const submitM = useMutation({
    mutationFn: (data: Promotion) => 
      editingId ? updateAdminPromotion(editingId, data) : createAdminPromotion(data),
    onSuccess: (res) => {
      // res.message lấy từ API trả về, nếu API không có thì xài text mặc định
      toast.success(res.message || (editingId ? 'Cập nhật thành công!' : 'Thêm mã mới thành công!'))
      setDialogOpen(false)
      qc.invalidateQueries({ queryKey: ['admin-promotions'] })
    },
    onError: (e) => toast.error(getErrorMessage(e))
  })

  const deleteM = useMutation({
    mutationFn: deleteAdminPromotion,
    onSuccess: (res) => {
      toast.success(res.message || 'Đã xóa mã khuyến mãi!')
      qc.invalidateQueries({ queryKey: ['admin-promotions'] })
    },
    onError: (e) => toast.error(getErrorMessage(e))
  })

  // Logic lọc và format dữ liệu
  const rawData = listQ.data ?? []
  const rows = useMemo(() => {
    return rawData.filter(p => 
      p.code?.toLowerCase().includes(search.toLowerCase()) || 
      p.description?.toLowerCase().includes(search.toLowerCase())
    )
  }, [rawData, search])

  const formatForInput = (dateStr: string) => {
    if (!dateStr) return ''
    const d = new Date(dateStr)
    return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16)
  }

  const openCreate = () => {
    setEditingId(null)
    setForm({
      code: '', description: '', discountPercentage: 10,
      startDate: formatForInput(new Date().toISOString()),
      endDate: formatForInput(new Date(Date.now() + 7 * 86400000).toISOString()),
      status: 'Active', carId: 0, maxUsage: 100,
    })
    setDialogOpen(true)
  }

  const openEdit = (p: Promotion) => {
    const id = p.promotionId ?? p.id
    if (!id) return
    setEditingId(id)
    setForm({
      code: p.code, description: p.description, discountPercentage: p.discountPercentage,
      startDate: formatForInput(p.startDate), endDate: formatForInput(p.endDate),
      status: p.status, carId: p.carId || 0, maxUsage: p.maxUsage,
    })
    setDialogOpen(true)
  }

  const handleSave = () => {
    if (!form.code.trim()) return toast.error('Vui lòng nhập Mã')
    submitM.mutate({
      ...form,
      code: form.code.toUpperCase(),
      startDate: new Date(form.startDate).toISOString(),
      endDate: new Date(form.endDate).toISOString(),
      carId: Number(form.carId) === 0 ? null : Number(form.carId),
    })
  }

  const submitting = submitM.isPending || deleteM.isPending

  return (
    <div className="p-2">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-800 dark:text-white">Khuyến mãi</h1>
          <p className="text-slate-500 text-sm mt-1">Quản lý mã giảm giá và voucher hệ thống.</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => listQ.refetch()} className="flex items-center gap-2 bg-white dark:bg-zinc-900 border dark:border-zinc-800 px-4 py-2 rounded-lg text-sm font-medium hover:bg-slate-50 text-slate-700 dark:text-zinc-200 transition-colors">
            <RefreshCcw size={16} className={listQ.isFetching ? 'animate-spin' : ''} /> Tải lại
          </button>
          <button onClick={openCreate} className="flex items-center gap-2 bg-slate-900 dark:bg-white text-white dark:text-slate-900 px-4 py-2 rounded-lg text-sm font-bold hover:opacity-90 transition-all shadow-lg shadow-indigo-200 dark:shadow-none">
            <Plus size={18} /> Thêm mã mới
          </button>
        </div>
      </div>

      <div className="bg-white dark:bg-zinc-950 border dark:border-zinc-800 rounded-xl p-4 mb-4 flex items-center shadow-sm">
        <input
          value={search} onChange={(e) => setSearch(e.target.value)}
          placeholder="Tìm theo mã hoặc mô tả..."
          className="w-full md:w-1/3 rounded-lg border dark:border-zinc-800 dark:bg-zinc-900 px-4 py-2 text-sm outline-none focus:ring-2 focus:ring-indigo-500/20"
        />
      </div>

      <div className="bg-white dark:bg-zinc-950 border dark:border-zinc-800 rounded-xl overflow-hidden shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm min-w-[900px]">
            <thead className="bg-slate-50 dark:bg-zinc-900/50 border-b dark:border-zinc-800 text-slate-500 uppercase text-[11px] font-bold">
              <tr>
                <th className="px-4 py-3 w-12">ID</th>
                <th className="px-4 py-3 w-40">Mã KM</th>
                <th className="px-4 py-3">Mô tả</th>
                <th className="px-4 py-3 w-24">% Giảm</th>
                <th className="px-4 py-3 w-40">Hiệu lực</th>
                <th className="px-4 py-3 w-32">Trạng thái</th>
                <th className="px-4 py-3 w-28 text-right">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y dark:divide-zinc-900">
              {listQ.isLoading ? (
                <tr><td colSpan={7} className="px-4 py-8 text-center text-slate-400">Đang tải dữ liệu...</td></tr>
              ) : rows.length === 0 ? (
                <tr><td colSpan={7} className="px-4 py-8 text-center text-slate-400">Không có mã khuyến mãi nào.</td></tr>
              ) : rows.map((p) => {
                const id = p.promotionId ?? p.id
                const isExpired = new Date(p.endDate) < new Date()
                return (
                  <tr key={id} className="hover:bg-slate-50/50 dark:hover:bg-zinc-900/30 transition-colors">
                    <td className="px-4 py-4 text-slate-400 font-mono text-xs">#{id}</td>
                    <td className="px-4 py-4 font-bold text-indigo-600 dark:text-indigo-400 flex items-center gap-2">
                      <Ticket size={14} /> {p.code}
                    </td>
                    <td className="px-4 py-4 text-slate-600 dark:text-zinc-300">{p.description}</td>
                    <td className="px-4 py-4 font-semibold text-rose-500">{p.discountPercentage}%</td>
                    <td className="px-4 py-4 text-[11px] leading-tight text-slate-500">
                      <div className="text-emerald-600">Bắt đầu: {new Date(p.startDate).toLocaleDateString('vi-VN')}</div>
                      <div className="text-rose-500">Kết thúc: {new Date(p.endDate).toLocaleDateString('vi-VN')}</div>
                    </td>
                    <td className="px-4 py-4">
                      <span className={`px-2 py-1 rounded-full text-[10px] font-bold uppercase ${
                          isExpired ? 'bg-slate-100 text-slate-500' :
                          p.status === 'Active' ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'
                      }`}>
                        {isExpired ? 'Hết hạn' : (p.status === 'Active' ? 'Hoạt động' : 'Tạm dừng')}
                      </span>
                    </td>
                    <td className="px-4 py-4 text-right flex justify-end gap-1">
                      <button onClick={() => openEdit(p)} className="p-1.5 hover:bg-indigo-50 text-indigo-600 rounded-md transition-colors"><Pencil size={16}/></button>
                      <button onClick={() => confirm('Xóa mã này hả ní?') && id && deleteM.mutate(id)} className="p-1.5 hover:bg-rose-50 text-rose-600 rounded-md transition-colors"><Trash2 size={16}/></button>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>

      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
          <div className="w-full max-w-xl bg-white dark:bg-zinc-950 rounded-2xl shadow-2xl border dark:border-zinc-800 overflow-hidden animate-in fade-in zoom-in-95 duration-200">
            <div className="px-6 py-4 border-b dark:border-zinc-800 flex justify-between items-center bg-slate-50 dark:bg-zinc-900/50">
              <h2 className="font-bold">{editingId ? 'Sửa Khuyến mãi' : 'Tạo mã mới'}</h2>
              <button onClick={() => setDialogOpen(false)} disabled={submitting} className="p-2 hover:bg-slate-200 dark:hover:bg-zinc-800 rounded-full transition-colors"><X size={18}/></button>
            </div>
            
            <div className="p-6 grid grid-cols-2 gap-4">
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Mã khuyến mãi *</label>
                <input value={form.code} onChange={e => setForm({...form, code: e.target.value.toUpperCase()})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-indigo-500/20 font-bold text-indigo-600 uppercase" placeholder="VD: SUMMER2026" />
              </div>
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">% Giảm giá *</label>
                <input type="number" value={form.discountPercentage} onChange={e => setForm({...form, discountPercentage: Number(e.target.value)})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-indigo-500/20" />
              </div>
              <div className="col-span-2">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Mô tả</label>
                <input value={form.description} onChange={e => setForm({...form, description: e.target.value})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-indigo-500/20" />
              </div>
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Bắt đầu</label>
                <input type="datetime-local" value={form.startDate} onChange={e => setForm({...form, startDate: e.target.value})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none" />
              </div>
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Kết thúc</label>
                <input type="datetime-local" value={form.endDate} onChange={e => setForm({...form, endDate: e.target.value})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none" />
              </div>
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Số lượt dùng tối đa (0 = Vô hạn)</label>
                <input type="number" min="0" value={form.maxUsage} onChange={e => setForm({...form, maxUsage: Number(e.target.value)})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none" />
              </div>
              <div className="col-span-1">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">ID Xe áp dụng (0 = Tất cả)</label>
                <input type="number" min="0" value={form.carId} onChange={e => setForm({...form, carId: Number(e.target.value)})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none" />
              </div>
              <div className="col-span-2">
                <label className="text-[11px] font-bold uppercase text-slate-500 block mb-1">Trạng thái</label>
                <select value={form.status} onChange={e => setForm({...form, status: e.target.value})} className="w-full rounded-lg border dark:bg-zinc-900 dark:border-zinc-800 px-3 py-2 text-sm outline-none">
                  <option value="Active">Hoạt động</option>
                  <option value="Inactive">Tạm khóa</option>
                </select>
              </div>
            </div>
            
            <div className="px-6 py-4 border-t dark:border-zinc-800 flex justify-end gap-3 bg-slate-50 dark:bg-zinc-900/50">
              <button onClick={() => setDialogOpen(false)} disabled={submitting} className="px-5 py-2 rounded-lg border font-semibold hover:bg-slate-100 dark:border-zinc-700 dark:hover:bg-zinc-800 transition-colors">
                Hủy
              </button>
              <button
                onClick={handleSave} disabled={submitting}
                className="px-6 py-2 bg-indigo-600 text-white rounded-lg font-bold hover:bg-indigo-700 disabled:opacity-50 transition-all flex items-center gap-2"
              >
                <Save size={16} /> {submitting ? 'Đang lưu...' : 'Lưu thông tin'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}