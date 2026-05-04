import { http } from '../http/http'

export type NotificationItem = {
  notificationId: number
  title: string
  content: string
  type?: string | null
  actionUrl?: string | null
  isRead: boolean
  createdAt: string
}

export type FetchNotificationsParams = {
  userId?: number
  showroomId?: number
  userRole?: string
}

export async function fetchNotifications(params?: FetchNotificationsParams): Promise<NotificationItem[]> {
  const res = await http.get('/api/Notifications', { params })
  const data = res.data
  // BE trả về { Success: true, Data: [...] }
  const arr = Array.isArray(data) ? data : Array.isArray(data?.Data) ? data.Data : Array.isArray(data?.data) ? data.data : []
  return arr.map((n: any) => ({
    notificationId: n.NotificationId ?? n.notificationId,
    title: n.Title ?? n.title,
    content: n.Content ?? n.content,
    type: n.NotificationType ?? n.notificationType ?? n.type,
    actionUrl: n.ActionUrl ?? n.actionUrl,
    isRead: n.IsRead ?? n.isRead ?? false,
    createdAt: n.CreatedAt ?? n.createdAt,
  }))
}

export async function markNotificationRead(id: number): Promise<{ message?: string }> {
  const res = await http.put(`/api/Notifications/${id}/read`)
  return res.data ?? {}
}