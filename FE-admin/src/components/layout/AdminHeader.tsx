import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Bell, CheckCheck, Languages, Menu, Search, Settings, X } from 'lucide-react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { IconButton } from './IconButton'
import { ThemeToggle } from './ThemeToggle'
import { useAuth } from '../../app/auth/useAuth'
import { isInRole } from '../../app/auth/roles'
import { fetchNotifications, markNotificationRead } from '../../services/notification/notifications'

const ROUTE_ROLES: Record<string, string[]> = {
  '/bookings':      ['Admin', 'Manager', 'Sales', 'ShowroomSales', 'Technician'],
  '/consignments':  ['Admin', 'Manager', 'Sales', 'ShowroomSales'],
  '/users':         ['Admin', 'Manager'],
  '/cars':          ['Admin', 'Manager', 'Sales', 'ShowroomSales', 'Technician'],
  '/orders':        ['Admin', 'Manager', 'Sales', 'ShowroomSales'],
  '/showrooms':     ['Admin'],
  '/inventories':   ['Admin', 'Manager', 'Sales', 'ShowroomSales', 'Technician'],
  '/articles':      ['Admin', 'Manager', 'Marketing', 'Content'],
  '/promotions':    ['Admin', 'Manager', 'Marketing'],
  '/notifications': ['Admin', 'Manager', 'Sales', 'ShowroomSales', 'Technician', 'Marketing', 'Content'],
  '/banners':       ['Admin', 'Manager', 'Marketing', 'Content'],
  '/reviews':       ['Admin', 'Manager', 'Content'],
}

const TYPE_LABEL: Record<string, string> = {
  Booking: 'Đặt lịch',
  TechCheck: 'Kỹ thuật',
  SystemAlert: 'Cảnh báo',
  System: 'Hệ thống',
  Promotion: 'Khuyến mãi',
  BANNER: 'Banner',
  BANNER_UPDATE: 'Banner',
  Consignment: 'Ký gửi xe', // Đã thêm label cho thông báo Ký gửi
}

const TYPE_CLASS: Record<string, string> = {
  Booking: 'bg-blue-50 text-blue-700 dark:bg-blue-950/40 dark:text-blue-300',
  TechCheck: 'bg-orange-50 text-orange-700 dark:bg-orange-950/40 dark:text-orange-300',
  SystemAlert: 'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-300',
  System: 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300',
  Promotion: 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300',
  BANNER: 'bg-purple-50 text-purple-700 dark:bg-purple-950/40 dark:text-purple-300',
  BANNER_UPDATE: 'bg-purple-50 text-purple-700 dark:bg-purple-950/40 dark:text-purple-300',
  Consignment: 'bg-fuchsia-50 text-fuchsia-700 dark:bg-fuchsia-950/40 dark:text-fuchsia-300', // Đã thêm màu cho thông báo Ký gửi
}

// Map role tiếng Anh → tiếng Việt hiển thị
const ROLE_LABEL: Record<string, string> = {
  Admin: 'Quản trị viên',
  Manager: 'Quản lý',
  Sales: 'Nhân viên Sales',
  ShowroomSales: 'Sales Showroom',
  Technician: 'Kỹ thuật viên',
  Marketing: 'Marketing',
  Content: 'Content',
}

function NotificationPopover() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  const params = {
    showroomId: user?.showroomId ?? undefined,
    userRole: user?.role ?? undefined,
  }

  const listQ = useQuery({
    queryKey: ['admin-notifications', params],
    queryFn: () => fetchNotifications(params),
    refetchInterval: 60_000,
  })

  const readM = useMutation({
    mutationFn: markNotificationRead,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-notifications'] }),
  })

  const all = listQ.data ?? []
  const unreadCount = all.filter((n) => !n.isRead).length

  useEffect(() => {
    function handler(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false)
    }
    if (open) document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [open])

  async function markAllRead() {
    const unread = all.filter((n) => !n.isRead)
    await Promise.all(unread.map((n) => readM.mutateAsync(n.notificationId)))
  }

  function toFrontendPath(url: string | null | undefined): string {
    if (!url) return '/notifications'
    return url.replace(/^\/admin/, '')
  }

  function canNavigateTo(path: string): boolean {
    const matchedRoute = Object.keys(ROUTE_ROLES).find((r) => path.startsWith(r))
    if (!matchedRoute) return true
    return isInRole(user?.role, ROUTE_ROLES[matchedRoute] as any)
  }

  function handleClickItem(n: typeof all[0]) {
    if (!n.isRead) readM.mutate(n.notificationId)
    if (n.actionUrl) {
      const path = toFrontendPath(n.actionUrl)
      setOpen(false)
      if (canNavigateTo(path)) navigate(path)
    }
  }

  return (
    <div ref={ref} className="relative">
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        className="relative inline-flex h-9 w-9 items-center justify-center rounded-lg border border-slate-200 bg-white text-slate-600 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300 dark:hover:bg-zinc-900"
        aria-label="Thông báo"
      >
        <Bell size={18} />
        {unreadCount > 0 && (
          <span className="absolute -right-1 -top-1 flex h-4 w-4 items-center justify-center rounded-full bg-rose-500 text-[10px] font-bold text-white">
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      {open && (
        <div className="absolute right-0 top-11 z-50 w-80 rounded-xl border border-slate-200 bg-white shadow-2xl dark:border-zinc-800 dark:bg-zinc-950 sm:w-96">
          <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-zinc-800">
            <div className="flex items-center gap-2">
              <span className="text-sm font-semibold text-slate-900 dark:text-zinc-100">Thông báo</span>
              {unreadCount > 0 && (
                <span className="rounded-full bg-rose-100 px-1.5 py-0.5 text-xs font-medium text-rose-700 dark:bg-rose-950/50 dark:text-rose-300">
                  {unreadCount} mới
                </span>
              )}
            </div>
            <div className="flex items-center gap-1">
              {unreadCount > 0 && (
                <button type="button" onClick={markAllRead} title="Đọc tất cả"
                  className="rounded-md p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600 dark:hover:bg-zinc-900 dark:hover:text-zinc-200">
                  <CheckCheck size={15} />
                </button>
              )}
              <button type="button" onClick={() => setOpen(false)}
                className="rounded-md p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600 dark:hover:bg-zinc-900 dark:hover:text-zinc-200">
                <X size={15} />
              </button>
            </div>
          </div>

          <div className="max-h-96 overflow-y-auto divide-y divide-slate-100 dark:divide-zinc-900">
            {listQ.isLoading && <div className="py-6 text-center text-sm text-slate-400">Đang tải...</div>}
            {!listQ.isLoading && all.length === 0 && (
              <div className="flex flex-col items-center gap-2 py-8 text-slate-400 dark:text-zinc-500">
                <Bell size={24} className="opacity-30" />
                <div className="text-sm">Chưa có thông báo nào</div>
              </div>
            )}
            {all.slice(0, 20).map((n) => {
              const typeLabel = TYPE_LABEL[n.type ?? ''] ?? n.type ?? ''
              const typeCls = TYPE_CLASS[n.type ?? ''] ?? 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300'
              return (
                <div key={n.notificationId}
                  className={['flex gap-3 px-4 py-3 transition-colors',
                    n.isRead ? 'opacity-60' : 'bg-indigo-50/40 dark:bg-indigo-950/10',
                    n.actionUrl ? 'cursor-pointer hover:bg-slate-50 dark:hover:bg-zinc-900/50' : '',
                  ].join(' ')}
                  onClick={() => n.actionUrl && handleClickItem(n)}
                >
                  <div className="mt-1.5 flex-shrink-0">
                    <div className={['h-2 w-2 rounded-full',
                      n.isRead ? 'bg-slate-300 dark:bg-zinc-700' : 'bg-indigo-500'].join(' ')} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex flex-wrap items-center gap-1.5">
                      <span className="text-xs font-semibold text-slate-900 dark:text-zinc-100 truncate">{n.title}</span>
                      {typeLabel && (
                        <span className={`inline-flex items-center rounded-full px-1.5 py-0.5 text-[10px] font-medium ${typeCls}`}>
                          {typeLabel}
                        </span>
                      )}
                    </div>
                    <p className="mt-0.5 text-xs text-slate-500 dark:text-zinc-400 line-clamp-2">{n.content}</p>
                    <div className="mt-1 flex items-center gap-2">
                      <span className="text-[10px] text-slate-400">{n.createdAt ?? ''}</span>
                      {n.actionUrl && (
                        <span className="text-[10px] font-medium text-indigo-600 dark:text-indigo-400">Xem chi tiết →</span>
                      )}
                    </div>
                  </div>
                </div>
              )
            })}
          </div>

          <div className="border-t border-slate-200 px-4 py-2.5 dark:border-zinc-800">
            <button type="button"
              onClick={() => { setOpen(false); navigate('/notifications') }}
              className="w-full rounded-lg py-1.5 text-xs font-medium text-indigo-600 hover:bg-indigo-50 dark:text-indigo-400 dark:hover:bg-indigo-950/20">
              Xem tất cả thông báo
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

export function AdminHeader({ onToggleSidebar }: { onToggleSidebar: () => void }) {
  const { user } = useAuth()

  const displayName = user?.username ?? 'Admin'
  const displayRole = ROLE_LABEL[user?.role ?? ''] ?? user?.role ?? 'Admin Panel'
  const initial = displayName.charAt(0).toUpperCase()

  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/80 backdrop-blur dark:border-zinc-800 dark:bg-zinc-950/80">
      <div className="w-full px-4 py-3">
        <div className="flex items-center gap-3 md:grid md:grid-cols-[280px_1fr] md:gap-6">
          <div className="flex items-center gap-3">
            <IconButton className="md:hidden" onClick={onToggleSidebar} aria-label="Toggle sidebar">
              <Menu size={18} />
            </IconButton>

            <div className="flex items-center gap-2">
              <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-500" />
              <div className="leading-tight">
                <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">CMC AUTOMOTIVE</div>
                <div className="text-xs text-slate-500 dark:text-zinc-400">{displayRole}</div>
              </div>
            </div>
          </div>

          <div className="flex flex-1 items-center gap-3">
            <div className="relative w-full max-w-2xl">
              <Search size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                placeholder="Search"
                className="h-10 w-full rounded-lg border border-slate-200 bg-slate-50 pl-9 pr-3 text-sm text-slate-900 placeholder:text-slate-400 outline-none focus:bg-white focus:ring-2 focus:ring-slate-200 dark:border-zinc-800 dark:bg-zinc-900/40 dark:text-zinc-100 dark:placeholder:text-zinc-500 dark:focus:bg-zinc-950 dark:focus:ring-zinc-800"
              />
            </div>

            <div className="ml-auto flex items-center gap-2">
              <div className="hidden md:block">
                <ThemeToggle />
              </div>

              <IconButton aria-label="Language">
                <Languages size={18} />
              </IconButton>

              <NotificationPopover />

              <IconButton aria-label="Settings">
                <Settings size={18} />
              </IconButton>

              <button
                type="button"
                className="ml-1 inline-flex items-center gap-2 rounded-full border border-slate-200 bg-white px-2 py-1.5 text-sm text-slate-700 hover:bg-slate-100 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
              >
                <span className="inline-flex h-7 w-7 items-center justify-center rounded-full bg-gradient-to-br from-indigo-500 to-purple-500 text-xs font-semibold text-white">
                  {initial}
                </span>
                <span className="hidden pr-1 md:block">{displayName}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="px-4 pb-3 md:hidden">
        <ThemeToggle />
      </div>
    </header>
  )
}