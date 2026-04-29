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

export async function fetchAdminArticles(params: {
  search?: string
  page: number
  pageSize: number
  isPublished?: boolean | string
}): Promise<{ data: ArticleResponseDto[]; totalCount: number }> {
  // Lọc bỏ param rỗng
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