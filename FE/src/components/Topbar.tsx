import { MapPin, Phone } from 'lucide-react'

export function Topbar() {
  return (
    <div className="bg-slate-800 text-white">
      <div className="mx-auto flex h-9 w-full max-w-screen-2xl items-center justify-between px-6 text-xs">
        <div className="flex min-w-0 items-center gap-2">
          <MapPin className="h-4 w-4 shrink-0 opacity-90" aria-hidden="true" />
          <span className="truncate">
            địa chỉ trụ sở chính
          </span>
        </div>

        <a
          className="inline-flex items-center gap-2 font-semibold tracking-wide"
          href="tel:0333436743"
        >
          <Phone className="h-4 w-4 opacity-90" aria-hidden="true" />
          <span>Hotline: 0333 436 743</span>
        </a>
      </div>
    </div>
  )
}

