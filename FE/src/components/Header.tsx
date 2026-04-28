import { NavLink } from 'react-router-dom'

const navItems = [
  { to: '/', label: 'Trang chủ' },
  { to: '/san-pham', label: 'Sản phẩm' },
  { to: '/bang-gia-xe', label: 'Bảng giá xe' },
  { to: '/tra-gop', label: 'Trả góp' },
  { to: '/tin-tuc-uu-dai', label: 'Tin tức - Ưu đãi' },
  { to: '/lien-he', label: 'Liên hệ' },
] as const

function Logo() {
  return (
    <NavLink
      to="/"
      className="inline-flex items-center gap-3 text-slate-900"
      aria-label="Trang chủ"
    >
      <svg
        width="42"
        height="28"
        viewBox="0 0 42 28"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        className="shrink-0"
        aria-hidden="true"
      >
        <path
          d="M6 4C10 8 12.5 12.5 21 24C29.5 12.5 32 8 36 4"
          stroke="#111827"
          strokeWidth="3"
          strokeLinecap="round"
          strokeLinejoin="round"
        />
        <path
          d="M12 4C14.5 8 16.5 12.5 21 20C25.5 12.5 27.5 8 30 4"
          stroke="#111827"
          strokeWidth="3"
          strokeLinecap="round"
          strokeLinejoin="round"
          opacity="0.45"
        />
      </svg>

      <div className="leading-tight">
        <div className="text-xl font-bold tracking-widest text-slate-900">
          CMC AUTOMOTIVE
        </div>
      </div>
    </NavLink>
  )
}

export function Header() {
  return (
    <header className="sticky top-0 z-50 bg-white text-slate-900 shadow-sm shadow-slate-900/5 ring-1 ring-slate-900/5">
      <div className="mx-auto flex w-full max-w-screen-2xl items-center justify-between px-6 py-6">
        <Logo />

        <nav className="hidden items-center gap-7 text-sm font-semibold md:flex md:text-base">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                [
                  'transition-colors hover:text-slate-900',
                  isActive ? 'font-bold text-slate-900' : 'text-slate-600',
                ].join(' ')
              }
              end={item.to === '/'}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </div>
    </header>
  )
}

