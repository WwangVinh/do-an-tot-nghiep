import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { Bell, CheckCheck, RefreshCcw } from 'lucide-react'

import { useAuth } from '../../app/auth/useAuth'
import { fetchNotifications, markNotificationRead, type NotificationItem } from '../../services/notification/notifications'

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

const TYPE_LABEL: Record<string, string> = {
  Booking: 'Đặt lịch',
  TechCheck: 'Kỹ thuật',
  SystemAlert: 'Cảnh báo',
  System: 'Hệ thống',
  BANNER: 'Banner',
  BANNER_UPDATE: 'Banner',
}

const TYPE_CLASS: Record<string, string> = {
  Booking: 'bg-blue-50 text-blue-700 dark:bg-blue-950/40 dark:text-blue-300',
  TechCheck: 'bg-orange-50 text-orange-700 dark:bg-orange-950/40 dark:text-orange-300',
  SystemAlert: 'bg-rose-50 text-rose-700 dark:bg-rose-950/40 dark:text-rose-300',
  System: 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300',
  BANNER: 'bg-purple-50 text-purple-700 dark:bg-purple-950/40 dark:text-purple-300',
  BANNER_UPDATE: 'bg-purple-50 text-purple-700 dark:bg-purple-950/40 dark:text-purple-300',
}

export function NotificationsPage() {
  const { user } = useAuth()
  const qc = useQueryClient()
  const [filterRead, setFilterRead] = useState<'all' | 'unread' | 'read'>('all')

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
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const all: NotificationItem[] = listQ.data ?? []
  const rows = all.filter((n) => {
    if (filterRead === 'unread') return !n.isRead
    if (filterRead === 'read') return n.isRead
    return true
  })

  const unreadCount = all.filter((n) => !n.isRead).length

  async function markAllRead() {
    const unread = all.filter((n) => !n.isRead)
    await Promise.all(unread.map((n) => readM.mutateAsync(n.notificationId)))
    toast.success('Đã đánh dấu tất cả là đã đọc')
  }

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Thông báo</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">
            Thông báo hệ thống dành cho chi nhánh và vai trò của bạn.
            {unreadCount > 0 && (
              <span className="ml-2 inline-flex items-center rounded-full bg-rose-50 px-2 py-0.5 text-xs font-medium text-rose-700 dark:bg-rose-950/40 dark:text-rose-300">
                {unreadCount} chưa đọc
              </span>
            )}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <button onClick={() => listQ.refetch()} disabled={listQ.isFetching}
            className="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-sm font-medium hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900">
            <RefreshCcw size={16} /> Tải lại
          </button>
          {unreadCount > 0 && (
            <button onClick={markAllRead} disabled={readM.isPending}
              className="inline-flex h-10 items-center gap-2 rounded-md bg-slate-900 px-3 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100">
              <CheckCheck size={16} /> Đọc tất cả
            </button>
          )}
        </div>
      </div>

      <div className="mt-6 rounded-lg border border-slate-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        {/* Filter tabs */}
        <div className="flex border-b border-slate-200 dark:border-zinc-800">
          {(['all', 'unread', 'read'] as const).map((v) => {
            const label = v === 'all' ? 'Tất cả' : v === 'unread' ? 'Chưa đọc' : 'Đã đọc'
            const count = v === 'all' ? all.length : v === 'unread' ? all.filter((n) => !n.isRead).length : all.filter((n) => n.isRead).length
            return (
              <button key={v} type="button" onClick={() => setFilterRead(v)}
                className={[
                  'flex items-center gap-1.5 px-4 py-3 text-sm font-medium transition-colors border-b-2',
                  filterRead === v
                    ? 'border-indigo-600 text-indigo-600 dark:border-indigo-400 dark:text-indigo-400'
                    : 'border-transparent text-slate-500 hover:text-slate-700 dark:text-zinc-400 dark:hover:text-zinc-200',
                ].join(' ')}>
                {label}
                <span className="rounded-full bg-slate-100 px-1.5 py-0.5 text-xs dark:bg-zinc-800">{count}</span>
              </button>
            )
          })}
        </div>

        {/* List */}
        <div className="divide-y divide-slate-100 dark:divide-zinc-900">
          {listQ.isLoading && (
            <div className="py-8 text-center text-sm text-slate-500">Đang tải...</div>
          )}
          {listQ.isError && (
            <div className="py-8 text-center text-sm text-rose-600">{getErrorMessage(listQ.error)}</div>
          )}
          {!listQ.isLoading && rows.length === 0 && (
            <div className="flex flex-col items-center gap-3 py-12 text-slate-400 dark:text-zinc-500">
              <Bell size={32} className="opacity-30" />
              <div className="text-sm">Không có thông báo nào.</div>
            </div>
          )}

          {rows.map((n) => {
            const typeLabel = TYPE_LABEL[n.type ?? ''] ?? n.type ?? ''
            const typeCls = TYPE_CLASS[n.type ?? ''] ?? 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300'
            return (
              <div key={n.notificationId}
                className={[
                  'flex items-start gap-4 px-4 py-4 transition-colors',
                  n.isRead ? 'opacity-60' : 'bg-indigo-50/30 dark:bg-indigo-950/10',
                ].join(' ')}>

                {/* Dot unread */}
                <div className="mt-1.5 flex-shrink-0">
                  <div className={[
                    'h-2 w-2 rounded-full',
                    n.isRead ? 'bg-slate-300 dark:bg-zinc-700' : 'bg-indigo-500',
                  ].join(' ')} />
                </div>

                <div className="flex-1 min-w-0">
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="text-sm font-semibold text-slate-900 dark:text-zinc-50">{n.title}</span>
                    {typeLabel && (
                      <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${typeCls}`}>
                        {typeLabel}
                      </span>
                    )}
                  </div>
                  <p className="mt-1 text-sm text-slate-600 dark:text-zinc-300">{n.content}</p>
                  <div className="mt-1.5 flex items-center gap-3">
                    <span className="text-xs text-slate-400 dark:text-zinc-500">
                      {n.createdAt ?? ''}
                    </span>
                    {n.actionUrl && (
                      <a href={n.actionUrl}
                        onClick={() => { if (!n.isRead) readM.mutate(n.notificationId) }}
                        className="text-xs font-medium text-indigo-600 hover:underline dark:text-indigo-400">
                        Xem chi tiết →
                      </a>
                    )}
                  </div>
                </div>
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}