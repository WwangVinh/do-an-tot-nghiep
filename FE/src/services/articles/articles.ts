import { http } from '../http/http'

export type ArticleResponseDto = {
  articleId: number
  title: string
  thumbnail: string | null
  isPublished: boolean
  createdAt: string
}

type ApiOk<T> = {
  message?: string
  data?: T
  totalCount?: number
}

export type ArticlePublicListResult = {
  data: ArticleResponseDto[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
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

export async function fetchPublicArticles(params: {
  search?: string
  page?: number
  pageSize?: number
}): Promise<ArticlePublicListResult> {
  const queryParams: Record<string, unknown> = {
    page: params.page ?? 1,
    pageSize: params.pageSize ?? 10,
  }
  if (params.search?.trim()) queryParams.search = params.search.trim()

  const res = await http.get<ArticlePublicListResult>('/api/public/articles', {
    params: queryParams,
  })

  return res.data
}
export type ArticleDetailResponseDto = {
  articleId: number
  title: string
  content: string
  thumbnail: string | null
  createdAt: string
  isPublished: boolean
  relatedCars: {
    carId: number
    name: string
    price: number | null
    imageUrl: string | null
  }[]
}

export async function fetchPublicArticleDetail(id: number): Promise<ArticleDetailResponseDto> {
  const res = await http.get<ArticleDetailResponseDto>(`/api/public/articles/${id}`)
  return res.data
}