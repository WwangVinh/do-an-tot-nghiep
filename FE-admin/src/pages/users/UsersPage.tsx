import { useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Pencil, Plus, RefreshCcw, Save, Shield, Trash2, X } from 'lucide-react'

import { useAuth } from '../../app/auth/useAuth'
import type { Showroom } from '../../services/showrooms/showrooms'
import { fetchAdminShowrooms } from '../../services/showrooms/showrooms'
import type { AdminUserListItem, CreateAdminStaffInput, FetchAdminUsersParams, UpdateAdminStaffInput } from '../../services/users/users'
import { createAdminStaff, fetchAdminUsers, setAdminUserStatus, updateAdminStaff } from '../../services/users/users'

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

type DialogMode = 'create' | 'edit'

function emptyCreateForm(): CreateAdminStaffInput {
  return {
    username: '',
    password: '',
    fullName: '',
    email: '',
    phone: '',
    role: 'ShowroomSales',
    showroomId: 0,
  }
}

function emptyEditForm(): UpdateAdminStaffInput {
  return {
    fullName: '',
    email: '',
    phone: '',
    role: 'ShowroomSales',
    showroomId: 0,
    status: 'Active',
  }
}

export function UsersPage() {
  const qc = useQueryClient()
  const auth = useAuth()

  const isAdmin = (auth.user?.role ?? '').toLowerCase() === 'admin'

  const [userType, setUserType] = useState<'Staff' | 'Customer'>('Staff')
  const [isDeleted, setIsDeleted] = useState(false)
  const [search, setSearch] = useState('')
  const [filterShowroomId, setFilterShowroomId] = useState<number | null>(null)
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(12)

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogMode, setDialogMode] = useState<DialogMode>('create')
  const [editing, setEditing] = useState<AdminUserListItem | null>(null)
  const [createForm, setCreateForm] = useState<CreateAdminStaffInput>(() => emptyCreateForm())
  const [editForm, setEditForm] = useState<UpdateAdminStaffInput>(() => emptyEditForm())

  const params = useMemo<FetchAdminUsersParams>(() => {
    return {
      userType,
      isDeleted,
      search: search.trim() || undefined,
      page,
      pageSize,
      filterShowroomId: isAdmin ? (filterShowroomId ?? undefined) : undefined,
    }
  }, [filterShowroomId, isAdmin, isDeleted, page, pageSize, search, userType])

  const qKey = useMemo(() => ['admin-users', params], [params])

  const listQ = useQuery({
    queryKey: qKey,
    queryFn: () => fetchAdminUsers(params),
  })

  const showroomsQ = useQuery({
    queryKey: ['admin-showrooms'],
    queryFn: () => fetchAdminShowrooms(),
    enabled: dialogOpen || isAdmin,
  })

  const createM = useMutation({
    mutationFn: (input: CreateAdminStaffInput) => createAdminStaff(input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Tạo người dùng thành công')
      setDialogOpen(false)
      setCreateForm(emptyCreateForm())
      await qc.invalidateQueries({ queryKey: ['admin-users'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const updateM = useMutation({
    mutationFn: (vars: { userId: number; input: UpdateAdminStaffInput }) => updateAdminStaff(vars.userId, vars.input),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Cập nhật người dùng thành công')
      setDialogOpen(false)
      setEditing(null)
      setEditForm(emptyEditForm())
      await qc.invalidateQueries({ queryKey: ['admin-users'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const statusM = useMutation({
    mutationFn: (vars: { userId: number; action: string }) => setAdminUserStatus(vars.userId, vars.action),
    onSuccess: async (res) => {
      toast.success(res.message ?? 'Cập nhật trạng thái thành công')
      await qc.invalidateQueries({ queryKey: ['admin-users'] })
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const submitting = createM.isPending || updateM.isPending || statusM.isPending
  const rows = listQ.data?.data ?? []
  const totalPages = listQ.data?.totalPages ?? 1
  const showrooms: Showroom[] = showroomsQ.data ?? []

  function openCreate() {
    setDialogMode('create')
    setEditing(null)
    setCreateForm(() => ({
      ...emptyCreateForm(),
      role: 'ShowroomSales',
      showroomId: isAdmin ? 0 : Number(auth.user?.showroomId ?? 0),
    }))
    setDialogOpen(true)
  }

  function openEdit(u: AdminUserListItem) {
    setDialogMode('edit')
    setEditing(u)
    setEditForm({
      fullName: (u.fullName ?? '').toString(),
      email: u.email ?? '',
      phone: u.phone ?? '',
      role:
        (u.role as any) === 'ShowroomManager'
          ? 'ShowroomManager'
          : (u.role as any) === 'SalesManager'
            ? 'SalesManager'
            : (u.role as any) === 'Technician'
              ? 'Technician'
              : (u.role as any) === 'Sales'
                ? 'Sales'
                : 'ShowroomSales',
      showroomId: Number(u.showroomId ?? 0),
      status: (u.status as any) === 'Inactive' ? 'Inactive' : 'Active',
    })
    setDialogOpen(true)
  }

  function closeDialog() {
    if (submitting) return
    setDialogOpen(false)
    setEditing(null)
    setCreateForm(emptyCreateForm())
    setEditForm(emptyEditForm())
  }

  function submitCreate() {
    const username = createForm.username.trim()
    const password = createForm.password.trim()
    const fullName = createForm.fullName.trim()
    const showroomId = Number(createForm.showroomId)

    if (!username) return toast.error('Vui lòng nhập Username')
    if (!password) return toast.error('Vui lòng nhập Password')
    if (!fullName) return toast.error('Vui lòng nhập Họ tên')
    if (!Number.isFinite(showroomId) || showroomId <= 0) return toast.error('Vui lòng chọn Showroom')

    createM.mutate({
      username,
      password,
      fullName,
      email: (createForm.email ?? '').trim() || null,
      phone: (createForm.phone ?? '').trim() || null,
      role: createForm.role,
      showroomId,
    })
  }

  function submitEdit() {
    if (!editing?.userId) return toast.error('Không xác định được người dùng')
    const fullName = editForm.fullName.trim()
    const showroomId = Number(editForm.showroomId)
    if (!fullName) return toast.error('Vui lòng nhập Họ tên')
    if (!Number.isFinite(showroomId) || showroomId <= 0) return toast.error('Vui lòng chọn Showroom')

    updateM.mutate({
      userId: editing.userId,
      input: {
        fullName,
        email: (editForm.email ?? '').trim() || null,
        phone: (editForm.phone ?? '').trim() || null,
        role: editForm.role,
        showroomId,
        status: editForm.status ?? null,
      },
    })
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Người dùng</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">Quản lý người dùng: xem danh sách, thêm nhân sự, chỉnh sửa thông tin.</p>
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

          {userType === 'Staff' ? (
            <button
              type="button"
              className="inline-flex h-10 items-center gap-2 rounded-md bg-slate-900 px-3 text-sm font-medium text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
              onClick={openCreate}
            >
              <Plus size={16} />
              Thêm nhân sự
            </button>
          ) : null}
        </div>
      </div>

      <div className="mt-6 rounded-lg border border-slate-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-12 md:items-end">
          <div className="md:col-span-3">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Loại</label>
            <select
              value={userType}
              onChange={(e) => {
                setUserType(e.target.value === 'Customer' ? 'Customer' : 'Staff')
                setPage(1)
              }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="Staff">Nhân sự</option>
              <option value="Customer">Khách hàng</option>
            </select>
          </div>

          <div className="md:col-span-2">
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Thùng rác</label>
            <select
              value={isDeleted ? '1' : '0'}
              onChange={(e) => {
                setIsDeleted(e.target.value === '1')
                setPage(1)
              }}
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
            >
              <option value="0">Đang hoạt động</option>
              <option value="1">Đã xóa</option>
            </select>
          </div>

          {isAdmin && userType === 'Staff' ? (
            <div className="md:col-span-3">
              <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Chi nhánh</label>
              <select
                value={String(filterShowroomId ?? 0)}
                onChange={(e) => {
                  const v = Number(e.target.value)
                  setFilterShowroomId(v > 0 ? v : null)
                  setPage(1)
                }}
                className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
              >
                <option value="0">Tất cả</option>
                {showrooms.map((s) => (
                  <option key={s.showroomId} value={s.showroomId}>
                    {s.name} (mã {s.showroomId})
                  </option>
                ))}
              </select>
            </div>
          ) : null}

          <div className={isAdmin && userType === 'Staff' ? 'md:col-span-4' : 'md:col-span-7'}>
            <label className="text-xs font-medium text-slate-600 dark:text-zinc-300">Tìm kiếm</label>
            <input
              value={search}
              onChange={(e) => {
                setSearch(e.target.value)
                setPage(1)
              }}
              placeholder="Tên đăng nhập / Email / Họ tên..."
              className="mt-1 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
            />
          </div>
        </div>

        <div className="mt-4 overflow-auto">
          <table className="min-w-[1100px] w-full text-left text-sm">
            <thead className="text-xs uppercase text-slate-500 dark:text-zinc-400">
              <tr className="border-b border-slate-200 dark:border-zinc-800">
                <th className="py-3 pr-3">ID</th>
                <th className="py-3 pr-3">Tài khoản</th>
                <th className="py-3 pr-3">Họ tên</th>
                <th className="py-3 pr-3">Liên hệ</th>
                <th className="py-3 pr-3">Vai trò</th>
                <th className="py-3 pr-3">Trạng thái</th>
                <th className="py-3 pr-3">Chi nhánh</th>
                <th className="py-3 pr-3">Ngày tạo</th>
                <th className="py-3 pr-3">Hành động</th>
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

              {rows.map((u) => {
                const isInactive = (u.status ?? '').toString() === 'Inactive'
                const canEdit = userType === 'Staff' && !isDeleted
                return (
                  <tr key={u.userId} className="border-b border-slate-100 dark:border-zinc-900">
                    <td className="py-3 pr-3 text-slate-600 dark:text-zinc-300">{u.userId}</td>
                    <td className="py-3 pr-3">
                      <div className="font-medium text-slate-900 dark:text-zinc-50">{u.username ?? '—'}</div>
                      <div className="text-xs text-slate-500 dark:text-zinc-400">{u.email ?? '—'}</div>
                    </td>
                    <td className="py-3 pr-3">{u.fullName ?? '—'}</td>
                    <td className="py-3 pr-3 text-xs text-slate-600 dark:text-zinc-300">
                      <div>{u.phone ?? '—'}</div>
                    </td>
                    <td className="py-3 pr-3">{u.role ?? '—'}</td>
                    <td className="py-3 pr-3">
                      <span
                        className={[
                          'inline-flex items-center rounded-full px-2 py-1 text-xs font-medium',
                          isInactive
                            ? 'bg-slate-100 text-slate-700 dark:bg-zinc-800 dark:text-zinc-200'
                            : 'bg-emerald-50 text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-300',
                        ].join(' ')}
                      >
                        {u.status === 'Active' ? 'Đang hoạt động' : u.status === 'Inactive' ? 'Đã khóa' : (u.status ?? '—')}
                      </span>
                    </td>
                    <td className="py-3 pr-3 text-xs text-slate-600 dark:text-zinc-300">{u.showroomId ?? '—'}</td>
                    <td className="py-3 pr-3 text-xs text-slate-600 dark:text-zinc-300">
                      {u.createdAt ? new Date(u.createdAt).toLocaleString() : '—'}
                    </td>
                    <td className="py-3 pr-3">
                      <div className="flex flex-wrap items-center gap-2">
                        {canEdit ? (
                          <button
                            type="button"
                            onClick={() => openEdit(u)}
                            className="inline-flex h-9 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-xs font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                            title="Chỉnh sửa thông tin"
                          >
                            <Pencil size={14} />
                            Sửa
                          </button>
                        ) : null}

                        {!isDeleted ? (
                          <button
                            type="button"
                            onClick={() => statusM.mutate({ userId: u.userId, action: isInactive ? 'Activate' : 'Deactivate' })}
                            disabled={statusM.isPending}
                            className="inline-flex h-9 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
                            title={isInactive ? 'Mở khóa tài khoản' : 'Khóa tài khoản'}
                          >
                            <Shield size={14} />
                            {isInactive ? 'Mở khóa' : 'Khóa'}
                          </button>
                        ) : null}

                        {isAdmin && !isDeleted ? (
                          <button
                            type="button"
                            onClick={() => statusM.mutate({ userId: u.userId, action: 'Delete' })}
                            disabled={statusM.isPending}
                            className="inline-flex h-9 items-center gap-2 rounded-md border border-rose-200 bg-rose-50 px-3 text-xs font-semibold text-rose-700 hover:bg-rose-100 disabled:opacity-60 dark:border-rose-900/40 dark:bg-rose-950/30 dark:text-rose-300 dark:hover:bg-rose-950/50"
                            title="Xóa mềm"
                          >
                            <Trash2 size={14} />
                            Xóa
                          </button>
                        ) : null}
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
            Trang {listQ.data?.currentPage ?? page}/{totalPages} · {listQ.data?.totalItems ?? rows.length} người dùng
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
              {[12, 20, 50].map((n) => (
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

      {dialogOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950">
            <div className="flex items-center justify-between gap-3 border-b border-slate-200 px-5 py-4 dark:border-zinc-800">
              <div>
                <div className="text-lg font-semibold text-slate-900 dark:text-zinc-50">
                  {dialogMode === 'create' ? 'Thêm nhân sự' : `Chỉnh sửa #${editing?.userId ?? ''}`}
                </div>
                <div className="mt-1 text-sm text-slate-500 dark:text-zinc-300">
                      {dialogMode === 'create' ? 'Tạo tài khoản nhân sự (Nhân viên/Quản lý).' : 'Cập nhật thông tin tài khoản.'}
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
              {dialogMode === 'create' ? (
                <>
                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tên đăng nhập *</label>
                    <input
                      value={createForm.username}
                      onChange={(e) => setCreateForm((f) => ({ ...f, username: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="vd: sales01"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Mật khẩu *</label>
                    <input
                      type="password"
                      value={createForm.password}
                      onChange={(e) => setCreateForm((f) => ({ ...f, password: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="••••••••"
                    />
                  </div>

                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Họ tên *</label>
                    <input
                      value={createForm.fullName}
                      onChange={(e) => setCreateForm((f) => ({ ...f, fullName: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="Nguyễn Văn A"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Email (không bắt buộc)</label>
                    <input
                      value={createForm.email ?? ''}
                      onChange={(e) => setCreateForm((f) => ({ ...f, email: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="a@email.com"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Số điện thoại (không bắt buộc)</label>
                    <input
                      value={createForm.phone ?? ''}
                      onChange={(e) => setCreateForm((f) => ({ ...f, phone: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="09xxxxxxxx"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Vai trò *</label>
                    <select
                      value={createForm.role}
                      onChange={(e) =>
                        setCreateForm((f) => ({
                          ...f,
                          role:
                            e.target.value === 'ShowroomManager'
                              ? 'ShowroomManager'
                              : e.target.value === 'SalesManager'
                                ? 'SalesManager'
                                : e.target.value === 'Technician'
                                  ? 'Technician'
                                  : e.target.value === 'Sales'
                                    ? 'Sales'
                                    : 'ShowroomSales',
                        }))
                      }
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    >
                      <option value="ShowroomSales">Nhân viên bán hàng (Showroom)</option>
                      <option value="ShowroomManager">Quản lý chi nhánh (Showroom)</option>
                      <option value="Sales">Sale</option>
                      <option value="SalesManager">Quản lý sale</option>
                      <option value="Technician">Nhân viên kỹ thuật</option>
                    </select>
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Chi nhánh *</label>
                    <select
                      value={String(createForm.showroomId ?? 0)}
                      onChange={(e) => setCreateForm((f) => ({ ...f, showroomId: Number(e.target.value) }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      disabled={!isAdmin}
                      title={!isAdmin ? 'Quản lý chỉ tạo nhân viên trong chi nhánh của mình' : undefined}
                    >
                      <option value="0">
                        {showroomsQ.isLoading ? 'Đang tải danh sách chi nhánh...' : showroomsQ.isError ? 'Không tải được chi nhánh' : '-- Chọn chi nhánh --'}
                      </option>
                      {showrooms.map((s) => (
                        <option key={s.showroomId} value={s.showroomId}>
                          {s.name} (mã {s.showroomId})
                        </option>
                      ))}
                    </select>
                  </div>
                </>
              ) : (
                <>
                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Họ tên *</label>
                    <input
                      value={editForm.fullName}
                      onChange={(e) => setEditForm((f) => ({ ...f, fullName: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="Nguyễn Văn A"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Email</label>
                    <input
                      value={editForm.email ?? ''}
                      onChange={(e) => setEditForm((f) => ({ ...f, email: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="a@email.com"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Phone</label>
                    <input
                      value={editForm.phone ?? ''}
                      onChange={(e) => setEditForm((f) => ({ ...f, phone: e.target.value }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      placeholder="09xxxxxxxx"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Vai trò *</label>
                    <select
                      value={editForm.role}
                      onChange={(e) =>
                        setEditForm((f) => ({
                          ...f,
                          role:
                            e.target.value === 'ShowroomManager'
                              ? 'ShowroomManager'
                              : e.target.value === 'SalesManager'
                                ? 'SalesManager'
                                : e.target.value === 'Technician'
                                  ? 'Technician'
                                  : e.target.value === 'Sales'
                                    ? 'Sales'
                                    : 'ShowroomSales',
                        }))
                      }
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      disabled={!isAdmin}
                      title={!isAdmin ? 'Quản lý không được đổi vai trò' : undefined}
                    >
                      <option value="ShowroomSales">Nhân viên bán hàng (Showroom)</option>
                      <option value="ShowroomManager">Quản lý chi nhánh (Showroom)</option>
                      <option value="Sales">Sale</option>
                      <option value="SalesManager">Quản lý sale</option>
                      <option value="Technician">Nhân viên kỹ thuật</option>
                    </select>
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Chi nhánh *</label>
                    <select
                      value={String(editForm.showroomId ?? 0)}
                      onChange={(e) => setEditForm((f) => ({ ...f, showroomId: Number(e.target.value) }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                      disabled={!isAdmin}
                      title={!isAdmin ? 'Quản lý chỉ chỉnh sửa trong chi nhánh của mình' : undefined}
                    >
                      <option value="0">
                        {showroomsQ.isLoading ? 'Đang tải danh sách chi nhánh...' : showroomsQ.isError ? 'Không tải được chi nhánh' : '-- Chọn chi nhánh --'}
                      </option>
                      {showrooms.map((s) => (
                        <option key={s.showroomId} value={s.showroomId}>
                          {s.name} (mã {s.showroomId})
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái</label>
                    <select
                      value={String(editForm.status ?? 'Active')}
                      onChange={(e) => setEditForm((f) => ({ ...f, status: e.target.value === 'Inactive' ? 'Inactive' : 'Active' }))}
                      className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-indigo-500/30 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50"
                    >
                      <option value="Active">Đang hoạt động</option>
                      <option value="Inactive">Đã khóa</option>
                    </select>
                  </div>
                </>
              )}
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
                onClick={dialogMode === 'create' ? submitCreate : submitEdit}
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

