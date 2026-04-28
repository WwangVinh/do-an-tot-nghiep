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
                'group flex flex-col items-center justify-center gap-3',
                'rounded-xl px-2 py-3',
                'transition duration-300 ease-out',
              ].join(' ')}
            >
              {/* === ICON: LIQUID GLASS === */}
              <div
                className={[
                  'grid h-16 w-16 place-items-center rounded-full',
                  'bg-white/10 backdrop-blur-md text-white',
                  'border border-white/20',
                  'shadow-[inset_0_1px_1px_rgba(255,255,255,0.4),0_8px_16px_-4px_rgba(0,0,0,0.2)]',
                  'transition-all duration-300 ease-out',
                  'group-hover:-translate-y-1 group-hover:scale-105',
                  'group-hover:bg-white/25 group-hover:border-white/40',
                  'group-hover:text-rose-400', // Đổi icon sang màu đỏ nhẹ khi hover
                ].join(' ')}
              >
                <item.Icon className="h-7 w-7 transition-colors duration-200" aria-hidden="true" />
              </div>

              {/* === LABEL: LIQUID GLASS === */}
              <div className="text-center">
                <span className="
                  inline-block rounded-full
                  bg-white/10 backdrop-blur-lg
                  px-4 py-1
                  text-[10px] sm:text-xs font-bold text-white
                  border border-white/20
                  shadow-[inset_0_1px_1px_rgba(255,255,255,0.4)]
                  transition-all duration-300
                  group-hover:bg-white/20 group-hover:border-white/40
                ">
                  {item.label}
                </span>
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