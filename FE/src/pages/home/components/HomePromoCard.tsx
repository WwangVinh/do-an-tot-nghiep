import { CheckCircle2, Mail, Phone } from 'lucide-react'

export type HomePromoModel = {
  name: string
  priceText: string
  discountText: string
}

export type HomePromoCardProps = {
  className?: string
  title?: string
  highlightTitle?: string
  dealerLine?: string
  promoBannerText?: string
  models?: HomePromoModel[]
  benefitsTitle?: string
  benefits?: string[]
  hotlineLabel?: string
  hotlineSubLabel?: string
  hotlineHref?: string
  quoteLabel?: string
  quoteSubLabel?: string
  quoteHref?: string
}

const DEFAULT_MODELS: HomePromoModel[] = [
  { name: 'VF3', priceText: 'Giá từ 281 triệu', discountText: 'Giảm 10 triệu tiền mặt' },
  { name: 'VF5', priceText: 'Giá từ 497 triệu', discountText: 'Giảm 15 triệu tiền mặt' },
  { name: 'VF6', priceText: 'Giá từ 647 triệu', discountText: 'Giảm 15 triệu tiền mặt' },
  { name: 'VF7', priceText: 'Giá từ 751 triệu', discountText: 'Giảm 20 triệu tiền mặt' },
  { name: 'VF8', priceText: 'Giá từ 917 triệu', discountText: 'Giảm 35 triệu tiền mặt' },
  { name: 'VF9', priceText: 'Giá từ 1.349 triệu', discountText: 'Giảm 90 triệu tiền mặt' },
]

const DEFAULT_BENEFITS = [
  'Miễn phí 100% thuế trước bạ',
  'Miễn phí sạc xe điện đến 30/6/2027',
  'Trả góp 0 đồng - nhận xe ngay',
  'Hỗ trợ vay trả góp 100%',
  'Bảo hành 7-10 năm',
]

export function HomePromoCard({
  className,
  title = 'CMC AUTOMOVITE',
  highlightTitle = 'ƯU ĐÃI QUÀ TẶNG HẤP DẪN',
  dealerLine = 'Đại lý phân phối chính hãng toàn quốc',
  promoBannerText = 'SIÊU KM: GIẢM 6 - 10% TẤT CẢ CÁC DÒNG XE + MUA XE 0Đ NHẬN XE NGAY',
  models = DEFAULT_MODELS,
  benefitsTitle = 'Quyền lợi ưu tiên:',
  benefits = DEFAULT_BENEFITS,
  hotlineLabel = 'Hotline 0333 436 743',
  hotlineSubLabel = 'Quý khách vui lòng gọi để có giá xe tốt nhất',
  hotlineHref = 'tel:0333436743',
  quoteLabel = 'NHẬN BÁO GIÁ',
  quoteSubLabel = 'Hoặc đăng ký nhận báo giá xe lăn bánh tốt nhất',
  quoteHref = '#quote-register',
}: HomePromoCardProps) {
  return (
    <section className={['w-full bg-slate-50 py-10 sm:py-12', className ?? ''].join(' ')}>
      <div className="mx-auto w-full max-w-screen-2xl px-6">
        <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-[0_20px_50px_rgba(15,23,42,0.10)]">
          <div className="px-6 pb-4 pt-7 text-center sm:px-10">
            <div className="text-xl font-extrabold tracking-wide text-slate-800 sm:text-2xl">{title}</div>
            <div className="mt-1 text-sm font-extrabold text-rose-600 sm:text-base">{highlightTitle}</div>
            <div className="mt-2 text-sm text-slate-500">{dealerLine}</div>
          </div>

          <div className="px-6 sm:px-10">
            <div className="rounded-xl bg-slate-100 px-4 py-3 text-center text-xs font-semibold tracking-wide text-slate-700 sm:text-sm">
              <span className="mr-2" aria-hidden="true">🔥</span>
              {promoBannerText}
              <span className="ml-2" aria-hidden="true">🔥</span>
            </div>

            <div className="mt-5 grid grid-cols-1 gap-3 md:grid-cols-2">
              {models.map((m) => (
                <div key={m.name} className="flex flex-wrap items-center gap-x-2 gap-y-1 rounded-lg border border-slate-200 bg-white px-4 py-3 text-sm">
                  <span className="font-semibold text-slate-800">{m.name}:</span>
                  <span className="font-semibold text-rose-600">{m.priceText}</span>
                  <span className="text-slate-500">- {m.discountText}</span>
                </div>
              ))}
            </div>

            <div className="mt-6 rounded-xl border border-slate-200 bg-slate-50 px-5 py-4">
              <div className="text-sm font-semibold text-slate-800">{benefitsTitle}</div>
              <ul className="mt-3 grid grid-cols-1 gap-2 sm:grid-cols-2">
                {benefits.map((b) => (
                  <li key={b} className="flex items-start gap-2 text-sm text-slate-600">
                    <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0 text-rose-500" aria-hidden="true" />
                    <span>{b}</span>
                  </li>
                ))}
              </ul>
            </div>
          </div>

          <div className="mt-7 grid grid-cols-1 gap-4 px-6 pb-7 sm:px-10 md:grid-cols-2">
            <a href={hotlineHref}
              className="group flex w-full flex-col items-center justify-center gap-2 rounded-xl bg-rose-600 px-5 py-5 text-white shadow-sm transition hover:bg-rose-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40">
              <Phone className="h-5 w-5 shrink-0" aria-hidden="true" />
              <div className="text-center">
                <div className="text-sm font-extrabold tracking-wide">{hotlineLabel}</div>
                <div className="mt-1 text-xs text-white/90">{hotlineSubLabel}</div>
              </div>
            </a>

            <a href={quoteHref}
              className="group flex w-full flex-col items-center justify-center gap-2 rounded-xl bg-slate-800 px-5 py-5 text-white shadow-sm transition hover:bg-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-slate-900/30">
              <Mail className="h-5 w-5 shrink-0" aria-hidden="true" />
              <div className="text-center">
                <div className="text-sm font-extrabold tracking-wide">{quoteLabel}</div>
                <div className="mt-1 text-xs text-white/90">{quoteSubLabel}</div>
              </div>
            </a>
          </div>
        </div>
      </div>
    </section>
  )
}