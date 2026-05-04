import { http } from '../http/http'

export type ConsignmentStatus =
  | 'Pending'       // Chờ xử lý / Mới gửi
  | 'Appraising'    // Đang thẩm định xe (Thay cho Evaluating)
  | 'Approved'      // Đã duyệt / Nhận ký gửi
  | 'Rejected'      // Từ chối ký gửi
  | 'Completed'     // Hoàn thành
  | (string & {})

export type AdminConsignmentListItem = {
  consignmentId: number
  guestName?: string | null
  guestPhone?: string | null
  brand?: string | null
  model?: string | null
  year?: number | null
  expectedPrice?: number | null
  status?: string | null
  createdAt?: string | null
}

export type AdminConsignmentsListResponse = {
  totalCount: number
  data: AdminConsignmentListItem[]
}

export type FetchAdminConsignmentsParams = {
  page?: number
  pageSize?: number
  search?: string
  status?: string
}

export async function fetchAdminConsignments(params: FetchAdminConsignmentsParams): Promise<AdminConsignmentsListResponse> {
  const res = await http.get('/api/Consignments/admin', { params })
  const d = res.data ?? {}
  // Đọc từ cấu trúc Swagger { success: true, data: [...] }
  const dataArr = Array.isArray(d.data) ? d.data : Array.isArray(d.Data) ? d.Data : []
  return {
    totalCount: Number(d.totalCount ?? d.TotalCount ?? dataArr.length),
    data: dataArr,
  }
}

export type AdminConsignmentDetail = AdminConsignmentListItem & {
  guestEmail?: string | null
  mileage?: number | null
  conditionDescription?: string | null
  note?: string | null // Ghi chú của admin
}

export async function fetchAdminConsignmentDetail(id: number): Promise<AdminConsignmentDetail> {
  const res = await http.get(`/api/Consignments/admin/${id}`)
  const d = res.data ?? {}
  return d.data ?? d.Data ?? d // Trả về data tuỳ theo cách backend wrap response
}

// Hàm cập nhật trạng thái (Dựa theo PUT /api/Consignments/admin/{id})
export async function updateConsignmentStatus(id: number, status: ConsignmentStatus, note?: string): Promise<{ message?: string }> {
  const res = await http.put(`/api/Consignments/admin/${id}`, { status, note })
  return { message: res.data?.message ?? res.data?.Message ?? 'Cập nhật thành công' }
}