import { useParams, Link, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { fetchPublicArticleDetail } from '../../services/articles/articles'

export function NewsDetailPage() {
  const { articleId } = useParams<{ articleId: string }>()
  const navigate = useNavigate()

  const { data: article, isLoading } = useQuery({
    queryKey: ['public-article', articleId],
    queryFn: () => fetchPublicArticleDetail(Number(articleId)),
    enabled: !!articleId,
  })

  // ✅ Hàm quay lại — nếu có history thì navigate(-1), không thì fallback về list
  const handleGoBack = () => {
    if (window.history.length > 1) {
      navigate(-1)
    } else {
      navigate('/tin-tuc-uu-dai')
    }
  }

  if (isLoading) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <div className="h-8 w-8 animate-spin rounded-full border-2 border-slate-200 border-t-slate-800" />
          <p className="text-sm text-slate-400">Đang tải bài viết...</p>
        </div>
      </div>
    )
  }

  if (!article) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4">
        <p className="text-lg font-medium text-slate-500">Không tìm thấy bài viết.</p>
        <Link
          to="/tin-tuc-uu-dai"
          className="text-sm text-slate-400 underline underline-offset-4 hover:text-slate-700"
        >
          ← Quay lại danh sách
        </Link>
      </div>
    )
  }

  const formattedDate = article.createdAt
    ? new Date(article.createdAt).toLocaleDateString('vi-VN', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
      })
    : ''

  const isHtml = /<[a-z][\s\S]*>/i.test(article.content)

  return (
    <main className="w-full bg-white">

      {/* Hero thumbnail */}
      {article.thumbnail && (
        <div className="relative h-[42vh] w-full overflow-hidden bg-slate-100 sm:h-[52vh]">
          <img
            src={article.thumbnail}
            alt={article.title}
            className="h-full w-full object-cover object-center"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-black/10 to-transparent" />
        </div>
      )}

      {/* ✅ Nội dung chính — mở rộng từ max-w-2xl lên max-w-4xl */}
      <div className="mx-auto max-w-6xl px-5 py-10 sm:px-6 lg:px-8 lg:py-14">

        {/* ✅ Nút quay lại trang trước — sticky-ish ở đầu, dễ thấy */}
        <button
          type="button"
          onClick={handleGoBack}
          className="group mb-6 inline-flex items-center gap-2 rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 shadow-sm transition hover:border-slate-300 hover:bg-slate-50 hover:text-slate-900 hover:shadow"
          aria-label="Quay lại trang trước"
        >
          <svg
            className="h-4 w-4 transition-transform group-hover:-translate-x-0.5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          Quay lại
        </button>

        {/* Breadcrumb */}
        <nav className="mb-6 flex items-center gap-2 text-xs text-slate-400">
          <Link to="/" className="hover:text-slate-600">Trang chủ</Link>
          <span>/</span>
          <Link to="/tin-tuc-uu-dai" className="hover:text-slate-600">Tin tức</Link>
          <span>/</span>
          <span className="text-slate-500 line-clamp-1">{article.title}</span>
        </nav>

        {/* Tiêu đề */}
        <h1 className="text-2xl font-extrabold leading-snug tracking-tight text-slate-900 sm:text-3xl lg:text-4xl">
          {article.title}
        </h1>

        {/* Meta */}
        <div className="mt-4 flex items-center gap-3">
          <div className="h-px flex-1 bg-slate-100" />
          <time className="shrink-0 text-xs font-medium uppercase tracking-widest text-slate-400">
            {formattedDate}
          </time>
          <div className="h-px flex-1 bg-slate-100" />
        </div>

        {/* Accent */}
        <div className="mt-6 mb-8 h-1 w-12 rounded-full bg-rose-500" />

        {/* Nội dung bài viết */}
        {isHtml ? (
          <>
            <style>{`
              .article-body { color: #334155; font-size: 1rem; }
              .article-body p { margin-bottom: 1rem; line-height: 1.8; }
              .article-body h2 { font-size: 1.35rem; font-weight: 700; color: #0f172a; margin: 1.75rem 0 0.75rem; }
              .article-body h3 { font-size: 1.15rem; font-weight: 700; color: #0f172a; margin: 1.5rem 0 0.5rem; }
              .article-body strong, .article-body b { font-weight: 700; color: #0f172a; }
              .article-body em, .article-body i { font-style: italic; }
              .article-body ul { list-style: disc; padding-left: 1.5rem; margin-bottom: 1rem; }
              .article-body ol { list-style: decimal; padding-left: 1.5rem; margin-bottom: 1rem; }
              .article-body li { margin-bottom: 0.35rem; line-height: 1.7; }
              .article-body a { color: #2563eb; text-decoration: none; }
              .article-body a:hover { text-decoration: underline; }
              .article-body img { border-radius: 0.75rem; max-width: 100%; margin: 1rem 0; box-shadow: 0 1px 4px rgba(0,0,0,0.08); }
              .article-body blockquote { border-left: 3px solid #e11d48; padding-left: 1rem; color: #475569; margin: 1rem 0; font-style: italic; }
              .article-body table { width: 100%; border-collapse: collapse; margin: 1rem 0; font-size: 0.95rem; }
              .article-body th { background: #f8fafc; font-weight: 600; color: #1e293b; border: 1px solid #cbd5e1; padding: 10px 14px; text-align: left; }
              .article-body td { border: 1px solid #cbd5e1; padding: 10px 14px; color: #334155; vertical-align: top; }
              .article-body tr:nth-child(even) td { background: #f8fafc; }
            `}</style>
            <div
              className="article-body"
              dangerouslySetInnerHTML={{ __html: article.content }}
            />
          </>
        ) : (
          <div className="space-y-4">
            {article.content.split('\n\n').filter(Boolean).map((para, i) => (
              <p key={i} className="leading-relaxed text-slate-700">
                {para}
              </p>
            ))}
          </div>
        )}

        {/* Xe liên quan — grid rộng hơn (4 cột trên desktop vì container đã to hơn) */}
        {article.relatedCars.length > 0 && (
          <div className="mt-12">
            <div className="mb-1 flex items-center gap-3">
              <div className="h-px flex-1 bg-slate-100" />
              <span className="text-xs font-semibold uppercase tracking-widest text-slate-400">
                Xe liên quan
              </span>
              <div className="h-px flex-1 bg-slate-100" />
            </div>
            <h2 className="mt-4 mb-5 text-lg font-bold text-slate-800">
              Các mẫu xe được đề cập trong bài
            </h2>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
              {article.relatedCars.map((car) => (
                <Link
                  key={car.carId}
                  to={`/san-pham/xe/${car.carId}`}
                  className="group overflow-hidden rounded-2xl border border-slate-100 bg-slate-50 transition hover:border-slate-200 hover:shadow-md"
                >
                  {car.imageUrl ? (
                    <img
                      src={car.imageUrl.startsWith('http') ? car.imageUrl : `https://localhost:7033${car.imageUrl}`}
                      alt={car.name}
                      className="h-28 w-full object-cover transition duration-300 group-hover:scale-105"
                      onError={(e) => { (e.currentTarget as HTMLImageElement).style.display = 'none' }}
                    />
                  ) : (
                    <div className="h-28 w-full bg-slate-200" />
                  )}
                  <div className="p-3">
                    <p className="text-sm font-semibold text-slate-800 line-clamp-1">{car.name}</p>
                    {car.price != null && (
                      <p className="mt-0.5 text-xs font-medium text-rose-600">
                        {(car.price / 1_000_000_000).toFixed(2).replace(/\.?0+$/, '')} tỷ đồng
                      </p>
                    )}
                  </div>
                </Link>
              ))}
            </div>
          </div>
        )}

        {/* ✅ Back footer — 2 lựa chọn: quay lại trang trước HOẶC về list */}
        <div className="mt-12 flex flex-col-reverse items-stretch gap-3 border-t border-slate-100 pt-6 sm:flex-row sm:items-center sm:justify-between">
          <button
            type="button"
            onClick={handleGoBack}
            className="inline-flex items-center justify-center gap-2 text-sm font-medium text-slate-500 transition hover:text-slate-800"
          >
            ← Quay lại trang trước
          </button>
          <Link
            to="/tin-tuc-uu-dai"
            className="inline-flex items-center justify-center gap-2 rounded-full bg-slate-900 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800"
          >
            Xem tất cả bài viết →
          </Link>
        </div>
      </div>
    </main>
  )
}