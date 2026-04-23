import type { NewsPost } from '../types'

export type NewsSidebarProps = {
  title?: string
  items: NewsPost[]
}

export function NewsSidebar({ title = 'Bài viết mới', items }: NewsSidebarProps) {
  return (
    <aside className="overflow-hidden rounded-2xl border border-slate-200/90 bg-white shadow-[0_1px_2px_rgba(15,23,42,0.06)]">
      <div className="border-b border-slate-100 px-5 py-4 sm:px-6 sm:py-5">
        <h2 className="text-xs font-bold uppercase tracking-[0.12em] text-slate-900">{title}</h2>
        <div className="mt-2 h-0.5 w-10 rounded-full bg-blue-700" aria-hidden />
      </div>

      <ul className="divide-y divide-slate-100">
        {items.map((it) => (
          <li key={it.id} className="px-4 py-4 sm:px-5">
            {it.href ? (
              <a href={it.href} className="group flex gap-3">
                <div className="relative h-14 w-14 shrink-0 overflow-hidden rounded-lg bg-slate-100 ring-1 ring-slate-200/80">
                  {it.imageSrc ? (
                    <img
                      src={it.imageSrc}
                      alt=""
                      className="h-full w-full object-cover transition duration-300 group-hover:scale-105"
                      loading="lazy"
                      decoding="async"
                    />
                  ) : (
                    <div className="h-full w-full bg-gradient-to-br from-slate-200 to-slate-100" aria-hidden />
                  )}
                </div>
                <div className="min-w-0 flex-1">
                  {it.dateText ? (
                    <p className="text-[11px] font-medium uppercase tracking-wide text-slate-400">{it.dateText}</p>
                  ) : null}
                  <p className="mt-0.5 text-sm font-semibold leading-snug text-slate-900 transition group-hover:text-blue-800">
                    {it.title}
                  </p>
                </div>
              </a>
            ) : (
              <div className="flex gap-3">
                <div className="h-14 w-14 shrink-0 rounded-lg bg-slate-100 ring-1 ring-slate-200/80" />
                <div className="min-w-0">
                  {it.dateText ? (
                    <div className="text-[11px] font-medium uppercase tracking-wide text-slate-400">{it.dateText}</div>
                  ) : null}
                  <p className="mt-0.5 text-sm font-semibold leading-snug text-slate-900">{it.title}</p>
                </div>
              </div>
            )}
          </li>
        ))}
      </ul>
    </aside>
  )
}
