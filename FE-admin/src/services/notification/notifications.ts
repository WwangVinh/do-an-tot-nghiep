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
  return Array.isArray(data) ? data : Array.isArray(data?.data) ? data.data : []
}

export async function markNotificationRead(id: number): Promise<{ message?: string }> {
  const res = await http.put(`/api/Notifications/${id}/read`)
  return res.data ?? {}
}