import { ArrowUpRight } from 'lucide-react'

import type { NewsPost } from '../types'

export type NewsListItemProps = {
  post: NewsPost
}

export function NewsListItem({ post }: NewsListItemProps) {
  const className = [
    'group block overflow-hidden rounded-2xl border border-slate-200/90 bg-white',
    'shadow-[0_1px_2px_rgba(15,23,42,0.06)] transition-all duration-300',
    'hover:border-slate-300 hover:shadow-[0_12px_40px_-12px_rgba(15,23,42,0.15)]',
  ].join(' ')

  const content = (
    <div className="flex flex-col sm:flex-row sm:min-h-[168px]">
      <div className="relative aspect-[16/10] w-full shrink-0 overflow-hidden bg-slate-100 sm:aspect-auto sm:w-[min(42%,280px)] sm:min-h-[168px]">
        {post.imageSrc ? (
          <img
            src={post.imageSrc}
            alt={post.title}
            className="h-full w-full object-cover transition duration-500 group-hover:scale-[1.03]"
            loading="lazy"
            decoding="async"
          />
        ) : (
          <div className="h-full min-h-[140px] w-full bg-gradient-to-br from-slate-200 to-slate-100" aria-hidden />
        )}
        <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-black/10 to-transparent opacity-0 transition group-hover:opacity-100" />
      </div>

      <div className="flex min-w-0 flex-1 flex-col justify-center gap-3 px-5 py-5 sm:px-7 sm:py-6">
        <h2 className="text-[17px] font-bold leading-snug tracking-tight text-slate-900 sm:text-lg">
          {post.title}
        </h2>
        {post.excerpt ? (
          <p className="line-clamp-3 text-sm leading-relaxed text-slate-600">{post.excerpt}</p>
        ) : null}
        <div className="mt-auto flex items-center pt-1">
          <span className="inline-flex items-center gap-1.5 rounded-full bg-blue-50 px-3 py-1.5 text-xs font-semibold text-blue-800 ring-1 ring-blue-100 transition group-hover:bg-blue-100 group-hover:ring-blue-200">
            Xem chi tiết
            <ArrowUpRight className="h-3.5 w-3.5 shrink-0 transition group-hover:translate-x-0.5 group-hover:-translate-y-0.5" aria-hidden />
          </span>
        </div>
      </div>
    </div>
  )

  return post.href ? (
    <a href={post.href} className={className}>
      {content}
    </a>
  ) : (
    <div className={className}>{content}</div>
  )
}
