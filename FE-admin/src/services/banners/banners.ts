import { http } from '../http/http'

export type Banner = {
  bannerId: number
  imageUrl: string
  linkUrl?: string | null
  position: number
  isActive: boolean
  bannerName: string
  startDate?: string | null
  endDate?: string | null
}

export type BannerUpsertInput = {
  bannerName: string
  imageUrl: string
  linkUrl?: string | null
  position: number
  isActive: boolean
  startDate?: string | null
  endDate?: string | null
}

type ApiOk<T> = {
  message?: string
  data?: T
}

export async function fetchBanners(params?: { isActive?: boolean }): Promise<Banner[]> {
  const res = await http.get<Banner[]>('/api/admin/banners', {
    params: params?.isActive === undefined ? undefined : { isActive: params.isActive },
  })
  return res.data ?? []
}

export async function createBanner(input: BannerUpsertInput): Promise<{ message?: string; data?: Banner }> {
  const res = await http.post<ApiOk<Banner>>('/api/admin/banners', input)
  return { message: res.data?.message, data: res.data?.data }
}

export async function updateBanner(
  bannerId: number,
  input: BannerUpsertInput,
): Promise<{ message?: string; data?: Banner }> {
  const res = await http.put<ApiOk<Banner>>(`/api/admin/banners/${bannerId}`, input)
  return { message: res.data?.message, data: res.data?.data }
}

export async function uploadBannerImage(file: File, bannerName?: string): Promise<{ message?: string; imageUrl: string }> {
  const fd = new FormData()
  fd.append('file', file)
  if (bannerName) fd.append('bannerName', bannerName)

  const res = await http.post<{ message?: string; imageUrl?: string }>('/api/admin/banners/upload-image', fd, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })

  const imageUrl = res.data?.imageUrl?.trim()
  if (!imageUrl) throw new Error('Upload thành công nhưng không nhận được imageUrl')
  return { message: res.data?.message, imageUrl }
}

