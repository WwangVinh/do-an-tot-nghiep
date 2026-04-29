import { useQuery } from '@tanstack/react-query'
import { fetchAdminArticles } from '../../services/articles/articles' // Sử dụng service đã tạo
import { Link } from 'react-router-dom'
import { CalendarDays, ArrowRight } from 'lucide-react'

// Hàm helper để tạo đoạn trích dẫn (excerpt) từ nội dung HTML
function createExcerpt(html: string, limit: number = 160): string {
  if (!html) return ''
  const text = html.replace(/<[^>]*>/g, '') // Xóa các thẻ HTML
  return text.length > limit ? text.substring(0, limit) + '...' : text
}

export function NewsPage() {
  // 1. Gọi API lấy danh sách bài viết từ Database
  const { data: articleData, isLoading } = useQuery({
    queryKey: ['public-articles'],
    queryFn: () => fetchAdminArticles({ page: 1, pageSize: 15, isPublished: true }), // Chỉ lấy bài đã xuất bản
  })

  const articles = articleData?.data ?? []

  if (isLoading) return <div className="py-20 text-center">Đang tải tin tức mới nhất...</div>

  return (
    <div className="container mx-auto px-4 py-12">
      <h1 className="mb-8 text-3xl font-bold">Tin tức VinFast</h1>
      
      <div className="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
        {articles.map((post) => (
          <article key={post.articleId} className="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-all hover:shadow-md dark:border-zinc-800 dark:bg-zinc-950">
            {/* 2. Hiển thị Thumbnail từ Database */}
            <div className="aspect-video overflow-hidden">
              <img
                src={post.thumbnail || '/assets/images/default-news.jpg'} 
                alt={post.title}
                className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
              />
            </div>

            <div className="p-6">
              <div className="mb-3 flex items-center gap-2 text-xs text-slate-500">
                <CalendarDays size={14} />
                {/* 3. Format ngày tạo từ DB */}
                {post.createdAt ? new Date(post.createdAt).toLocaleDateString('vi-VN') : '—'}
              </div>

              <h2 className="mb-3 line-clamp-2 text-xl font-bold text-slate-900 group-hover:text-indigo-600 dark:text-zinc-50">
                {post.title}
              </h2>

              {/* 4. Hiển thị đoạn trích dẫn được xử lý từ nội dung HTML */}
              <p className="mb-4 line-clamp-3 text-sm text-slate-600 dark:text-zinc-400">
                {/* Ở đây ní cần lấy detail nếu muốn có content, hoặc sửa API list để trả về 1 ít content */}
                {/* Tạm thời tui để title làm excerpt nếu list không trả về content */}
                Bấm vào để xem chi tiết bài viết về {post.title}.
              </p>

              <Link
                to={`/news/${post.articleId}`}
                className="inline-flex items-center gap-2 text-sm font-semibold text-indigo-600 hover:text-indigo-700"
              >
                Xem chi tiết <ArrowRight size={16} />
              </Link>
            </div>
          </article>
        ))}
      </div>

      {articles.length === 0 && (
        <div className="py-20 text-center text-slate-500">Hệ thống đang cập nhật tin tức mới...</div>
      )}
    </div>
  )
}