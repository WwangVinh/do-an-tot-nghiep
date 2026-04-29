import { http } from '../http/http' // 👈 Sửa lại đường dẫn này cho khớp với file banners.ts của ní nha

export type Promotion = {
  promotionId?: number
  id?: number
  code: string
  description: string
  discountPercentage: number
  startDate: string
  endDate: string
  status: string
  carId: number | null
  maxUsage: number
}

type ApiOk<T> = {
  message?: string
  data?: T
}

export async function fetchAdminPromotions(): Promise<Promotion[]> {
  const res = await http.get<Promotion[]>('/api/admin/Promotions')
  return res.data ?? []
}

export async function createAdminPromotion(input: Promotion): Promise<{ message?: string; data?: Promotion }> {
  const res = await http.post<ApiOk<Promotion>>('/api/admin/Promotions', input)
  return { message: res.data?.message, data: res.data?.data }
}

export async function updateAdminPromotion(
  id: number,
  input: Promotion,
): Promise<{ message?: string; data?: Promotion }> {
  const res = await http.put<ApiOk<Promotion>>(`/api/admin/Promotions/${id}`, input)
  return { message: res.data?.message, data: res.data?.data }
}

export async function deleteAdminPromotion(id: number): Promise<{ message?: string }> {
  const res = await http.delete<ApiOk<any>>(`/api/admin/Promotions/${id}`)
  return { message: res.data?.message }
}