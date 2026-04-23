import { Car, CreditCard, Package, Tag } from 'lucide-react'

type ActionItem = {
  label: string
  href?: string
  Icon: React.ComponentType<React.SVGProps<SVGSVGElement>>
}

export type BannerQuickActionsProps = {
  items?: ActionItem[]
  className?: string
}

const defaultItems: ActionItem[] = [
  { label: 'Sản phẩm', href: '/san-pham', Icon: Package },
  { label: 'Lái thử', href: '/lai-thu', Icon: Car },
  { label: 'Bảng giá', href: '/bang-gia-xe', Icon: Tag },
  { label: 'Trả góp', href: '/tra-gop', Icon: CreditCard },
]

export function BannerQuickActions({ items = defaultItems, className }: BannerQuickActionsProps) {
  return (
    <div
      className={[
        'pointer-events-none',
        'mx-auto w-full max-w-screen-2xl px-6',
        className ?? '',
      ].join(' ')}
      aria-label="Quick actions"
    >
      <div className="grid grid-cols-4 gap-4 py-2 sm:gap-6">
        {items.slice(0, 4).map((item) => {
          const content = (
            <div
              className={[
                'pointer-events-auto',
                'group flex flex-col items-center justify-center gap-2',
                'rounded-xl px-2 py-3',
                'transition duration-200 ease-out',
                'hover:bg-white/10',
                'focus-within:bg-white/10',
              ].join(' ')}
            >
              <div
                className={[
                  'grid h-16 w-16 place-items-center rounded-full',
                  'bg-white/90 text-rose-600',
                  'shadow-sm ring-1 ring-black/5',
                  'transition duration-200 ease-out',
                  'group-hover:-translate-y-0.5 group-hover:shadow-md',
                  'group-hover:bg-rose-500 group-hover:text-white',
                  'group-focus-visible:-translate-y-0.5 group-focus-visible:shadow-md',
                  'group-focus-visible:bg-rose-500 group-focus-visible:text-white',
                ].join(' ')}
              >
                <item.Icon className="h-7 w-7 transition-colors duration-200" aria-hidden="true" />
              </div>
              <div className="text-center text-base font-semibold text-slate-800">
                {item.label}
              </div>
            </div>
          )

          return item.href ? (
            <a
              key={item.label}
              href={item.href}
              className="outline-none focus-visible:ring-2 focus-visible:ring-rose-500/60 focus-visible:ring-offset-2 focus-visible:ring-offset-transparent"
            >
              {content}
            </a>
          ) : (
            <div key={item.label}>{content}</div>
          )
        })}
      </div>
    </div>
  )
}

