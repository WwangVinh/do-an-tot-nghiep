import { useQuery } from '@tanstack/react-query'
import { NewsListItem } from './components/NewsListItem'
import { NewsSidebar } from './components/NewsSidebar'
import type { NewsPost } from './types'
import { fetchPublicArticles } from '../../services/articles/articles'
import banner4 from '../../assets/images/banner4.jpg'

export function NewsPage() {
  const { data: articleData, isLoading } = useQuery({
    queryKey: ['public-articles'],
    queryFn: () => fetchPublicArticles({ page: 1, pageSize: 20 }),
  })

  const rawArticles = articleData?.data ?? []

  const posts: NewsPost[] = rawArticles.map((a) => ({
    id: a.articleId.toString(),
    title: a.title,
    excerpt: 'Bấm vào xem chi tiết để cập nhật thêm thông tin về bài viết này...',
    dateText: a.createdAt ? new Date(a.createdAt).toLocaleDateString('vi-VN') : '',
    imageSrc: a.thumbnail || 'https://images.unsplash.com/photo-1503328427499-d92d1fa3af8a?q=80&w=800&auto=format&fit=crop',
    href: `/tin-tuc/${a.articleId}`,
  }))

  const latest = posts.slice(0, 5)

  return (
    <main className="w-full bg-[#f4f6f9]">
      <div className="mx-auto w-full max-w-screen-2xl px-4 pb-14 pt-8 sm:px-6 lg:px-8 lg:pb-16 lg:pt-10">

        {/* Banner Hero */}
        <section className="relative mb-10 overflow-hidden rounded-2xl shadow-[0_8px_32px_-6px_rgba(15,23,42,0.05),0_2px_12px_-2px_rgba(15,23,42,0.05)] ring-1 ring-inset ring-white/15">
          <div className="absolute inset-0">
            <img
              src={banner4}
              alt=""
              className="h-full w-full object-cover object-[center_35%]"
              loading="eager"
              decoding="async"
            />
            <div className="absolute inset-0 bg-gradient-to-r from-slate-950/95 via-slate-900/80 to-slate-800/50" />
            <div className="absolute inset-0 bg-gradient-to-t from-slate-950/40 to-transparent" />
          </div>
          <div className="relative px-6 py-10 sm:px-10 sm:py-12 md:py-14">
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-white/70">Danh mục</p>
            <h1 className="mt-2 max-w-2xl text-3xl font-extrabold tracking-tight text-white sm:text-4xl md:text-[2.35rem] md:leading-tight">
              Tin tức &amp; sự kiện
            </h1>
            <p className="mt-4 max-w-xl text-sm leading-relaxed text-white/85 sm:text-base">
              Cập nhật ưu đãi, hoạt động và thông tin mới nhất từ CMC Automotive.
            </p>
            <div className="mt-6 h-px w-16 bg-gradient-to-r from-rose-500 to-blue-500 opacity-90" aria-hidden />
          </div>
        </section>

        {/* Nội dung tin tức */}
        <div className="grid grid-cols-1 gap-10 lg:grid-cols-12 lg:gap-12">

          {/* Cột trái: Danh sách bài viết */}
          <section className="lg:col-span-8">
            <div className="flex flex-col gap-7">
              {isLoading ? (
                <div className="py-20 text-center font-medium text-slate-500">Đang tải tin tức mới nhất...</div>
              ) : posts.length === 0 ? (
                <div className="py-20 text-center font-medium text-slate-500">Chưa có bài viết nào được xuất bản.</div>
              ) : (
                posts.map((p) => (
                  <NewsListItem key={p.id} post={p} />
                ))
              )}
            </div>
          </section>

          {/* Cột phải: Sidebar bài mới */}
          <div className="lg:col-span-4">
            <div className="lg:sticky lg:top-24">
              <NewsSidebar items={latest} />
            </div>
          </div>

        </div>
      </div>
    </main>
  )
}