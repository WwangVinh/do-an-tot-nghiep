import { http } from '../http/http'

export type ArticleResponseDto = {
  articleId: number
  title: string
  thumbnail: string | null
  isPublished: boolean
  createdAt: string
}

export type RelatedCarDto = {
  carId: number
  name: string
  price?: number
  imageUrl?: string
}

export type ArticleDetailResponseDto = {
  articleId: number
  title: string
  content: string
  thumbnail: string | null
  isPublished: boolean
  createdAt: string
  relatedCars: RelatedCarDto[]
}

export type ArticleSubmitDto = {
  title: string
  content: string
  thumbnail: string | null
  isPublished: boolean
  relatedCarIds: number[]
}

type ApiOk<T> = {
  message?: string
  data?: T
  totalCount?: number
}

export async function fetchAdminArticles(params: {
  search?: string
  page: number
  pageSize: number
  isPublished?: boolean | string
}): Promise<{ data: ArticleResponseDto[]; totalCount: number }> {
  const queryParams: any = { ...params }
  if (queryParams.isPublished === '') delete queryParams.isPublished

  const res = await http.get<ApiOk<ArticleResponseDto[]>>('/api/admin/Articles', {
    params: queryParams,
  })
  return {
    data: res.data?.data ?? [],
    totalCount: res.data?.totalCount ?? 0,
  }
}

export async function getAdminArticleDetail(id: number): Promise<ArticleDetailResponseDto> {
  const res = await http.get<ArticleDetailResponseDto>(`/api/admin/Articles/${id}`)
  return res.data
}

export async function createAdminArticle(input: ArticleSubmitDto): Promise<{ message?: string }> {
  const res = await http.post<ApiOk<any>>('/api/admin/Articles', input)
  return { message: res.data?.message }
}

export async function updateAdminArticle(id: number, input: ArticleSubmitDto): Promise<{ message?: string }> {
  const res = await http.put<ApiOk<any>>(`/api/admin/Articles/${id}`, input)
  return { message: res.data?.message }
}

export async function deleteAdminArticle(id: number): Promise<{ message?: string }> {
  const res = await http.delete<ApiOk<any>>(`/api/admin/Articles/${id}`)
  return { message: res.data?.message }
}