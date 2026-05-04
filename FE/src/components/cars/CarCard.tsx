import { Phone } from 'lucide-react'
import { Link } from 'react-router-dom'

export type CarCardAction = {
  label: string
  href?: string
  onClick?: () => void
  variant?: 'primary' | 'secondary'
}

export type CarCardProps = {
  name: string
  priceText?: string
  imageSrc?: string
  imageAlt?: string
  to?: string
  actions?: [CarCardAction, CarCardAction?]
  className?: string
  year?: number | null
  condition?: string | null
}

export function CarCard({
  name,
  priceText,
  imageSrc,
  imageAlt,
  to,
  actions,
  className,
  year,
  condition,
}: CarCardProps) {
  const [a1, a2] = actions ?? [
    { label: 'Nhận ưu đãi', variant: 'primary' },
    { label: 'Gọi chốt giá', variant: 'secondary', href: 'tel:0333436743' },
  ]

  const isUsed = condition?.toLowerCase() === 'used'

  const renderAction = (a?: CarCardAction, key?: string) => {
    if (!a) return null
    const isPrimary = (a.variant ?? 'secondary') === 'primary'
    const base = 'inline-flex h-9 items-center justify-center gap-2 rounded-lg px-3 text-xs font-semibold shadow-sm transition outline-none focus-visible:ring-2 focus-visible:ring-rose-500/50'
    const cls = isPrimary
      ? `${base} bg-rose-50 text-rose-600 ring-1 ring-rose-200 hover:bg-rose-100`
      : `${base} bg-white text-slate-700 ring-1 ring-slate-200 hover:bg-slate-50`
    const content = (
      <>
        {!isPrimary && <Phone className="h-4 w-4" aria-hidden="true" />}
        <span className="truncate">{a.label}</span>
      </>
    )
    return a.href ? (
      <a key={key} href={a.href} className={cls}>{content}</a>
    ) : (
      <button key={key} type="button" onClick={a.onClick} className={cls}>{content}</button>
    )
  }

  const preview = (
    <>
      <div className="bg-slate-50">
        <div className="relative aspect-[16/10] w-full overflow-hidden">
          {imageSrc ? (
            <img
              src={imageSrc}
              alt={imageAlt ?? name}
              className="absolute inset-0 h-full w-full object-contain p-4 transition-transform duration-300 ease-out group-hover:scale-110"
              loading="lazy"
              decoding="async"
              draggable={false}
            />
          ) : (
            <div className="absolute inset-0 grid place-items-center">
              <div className="h-14 w-14 rounded-full bg-slate-200" aria-hidden="true" />
            </div>
          )}
        </div>
      </div>

      <div className="px-5 pt-4">
        <div className="flex items-center gap-2">
          <div className="text-base font-semibold text-slate-800 sm:text-lg">{name}</div>
          {isUsed && (
            <span className="shrink-0 rounded-md bg-amber-50 px-1.5 py-0.5 text-[10px] font-semibold uppercase tracking-wide text-amber-700 ring-1 ring-amber-200">
              Cũ
            </span>
          )}
        </div>
        {year && (
          <div className="mt-0.5 text-xs text-slate-400">{year}</div>
        )}
        {priceText ? (
          <div className="mt-1 text-sm">
            <span className="text-slate-500">Giá từ: </span>
            <span className="font-semibold text-rose-600 sm:text-base">{priceText}</span>
          </div>
        ) : (
          <div className="mt-1 text-sm text-slate-500">Liên hệ để nhận báo giá</div>
        )}
      </div>
    </>
  )

  return (
    <article
      className={[
        'group',
        'overflow-hidden rounded-2xl border border-slate-200 bg-white',
        'shadow-[0_18px_40px_rgba(15,23,42,0.08)] transition-all duration-300 ease-out hover:-translate-y-0.5 hover:shadow-[0_28px_70px_rgba(15,23,42,0.18)]',
        className ?? '',
      ].join(' ')}
    >
      {to ? (
        <Link to={to} className="block rounded-t-2xl outline-none focus-visible:ring-2 focus-visible:ring-rose-500 focus-visible:ring-offset-2" aria-label={`Xem chi tiết ${name}`}>
          {preview}
        </Link>
      ) : (
        preview
      )}
      <div className="px-5 pb-4 pt-2">
        <div className="grid grid-cols-2 gap-2">
          {renderAction(a1, 'a1')}
          {renderAction(a2, 'a2')}
        </div>
      </div>
    </article>
  )
}