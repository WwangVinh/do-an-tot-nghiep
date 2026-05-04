import { Mail, Phone } from 'lucide-react'

export type HomeCommitmentFeature = {
  title: string
  description: string
}

export type HomeCommitmentSectionProps = {
  className?: string
  title?: string
  subtitle?: string
  topNote?: string
  hotlineLabel?: string
  hotlineSubLabel?: string
  hotlineHref?: string
  quoteLabel?: string
  quoteSubLabel?: string
  quoteHref?: string
  imageSrc: string
  imageAlt?: string
  imageCaption?: string
  cardCtaLabel?: string
  cardCtaHref?: string
  featuresTitle?: string
  features?: HomeCommitmentFeature[]
}

const DEFAULT_FEATURES: HomeCommitmentFeature[] = [
  {
    title: 'TƯ VẤN TẬN TÌNH',
    description: 'Đội ngũ tư vấn được đào tạo chuyên nghiệp, giàu kinh nghiệm luôn sẵn lòng giúp quý khách tìm được chiếc xe ưng ý.',
  },
  {
    title: 'GIÁ ƯU ĐÃI - GIAO XE SỚM',
    description: 'Đại lý luôn cam kết mang lại mức giá ưu đãi nhất cho quý khách và thời gian giao xe sớm tại khu vực Miền Bắc.',
  },
  {
    title: 'BẢO HÀNH TIÊU CHUẨN TOÀN CẦU',
    description: 'Cung cấp phụ tùng, hỗ trợ kỹ thuật và bảo hiểm giúp quý khách an tâm tận hưởng xe; luôn được chăm sóc kĩ lưỡng.',
  },
]

export function HomeCommitmentSection({
  className,
  title = 'CAM KẾT GIÁ XE VÀ CHƯƠNG TRÌNH KHUYẾN MẠI TỐT NHẤT',
  subtitle = 'TRONG HỆ THỐNG XE TẠI VIỆT NAM',
  topNote = `Hỗ Trợ Đăng Kí, Đăng Kiểm, Giao xe tận nhà. Hỗ Trợ Lái Thử Xe Tại Nhà Bất Kì Thời Gian Nào. Hỗ Trợ Làm Thủ tục\nBiển Đẹp, Hỗ Trợ Kỹ Thuật và Bảo Hiểm 24/7`,
  hotlineLabel = 'Hotline 0333 436 743',
  hotlineSubLabel = 'Quý khách vui lòng gọi để có giá xe tốt nhất',
  hotlineHref = 'tel:0333436743',
  quoteLabel = 'NHẬN BÁO GIÁ',
  quoteSubLabel = 'Hoặc đăng ký nhận báo giá xe lăn bánh tốt nhất',
  quoteHref = '#quote-register',
  imageSrc,
  imageAlt = 'Phòng kinh doanh VinFast',
  imageCaption = 'PHÒNG KINH DOANH TÊN CÔNG TY',
  cardCtaLabel = 'Gọi ngay: 0333 436 743 (24/24)',
  cardCtaHref = 'tel:0333436743',
  featuresTitle,
  features = DEFAULT_FEATURES,
}: HomeCommitmentSectionProps) {
  return (
    <section className={['w-full bg-white py-12 sm:py-14', className ?? ''].join(' ')}>
      <div className="mx-auto w-full max-w-screen-2xl px-6">
        <div className="text-center">
          <div className="text-lg font-extrabold tracking-wide text-slate-800 sm:text-xl md:text-2xl">{title}</div>
          <div className="mt-1 text-xs font-semibold tracking-[0.22em] text-slate-500 sm:text-sm">{subtitle}</div>
          {topNote ? (
            <div className="mx-auto mt-4 max-w-4xl whitespace-pre-line text-[11px] leading-relaxed text-slate-500 sm:text-xs">
              {topNote}
            </div>
          ) : null}
        </div>

        <div className="mt-9 grid grid-cols-1 gap-5 md:grid-cols-2">
          <a
            href={hotlineHref}
            className="group flex w-full flex-col items-center justify-center gap-3 rounded-2xl bg-rose-600 px-8 py-7 text-white shadow-[0_16px_40px_rgba(15,23,42,0.12)] transition hover:bg-rose-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40"
          >
            <Phone className="h-5 w-5 shrink-0" aria-hidden="true" />
            <div className="text-center">
              <div className="text-sm font-extrabold tracking-wide">{hotlineLabel}</div>
              <div className="mt-1 text-xs text-white/90">{hotlineSubLabel}</div>
            </div>
          </a>

          <a
            href={quoteHref}
            className="group flex w-full flex-col items-center justify-center gap-3 rounded-2xl bg-slate-800 px-8 py-7 text-white shadow-[0_16px_40px_rgba(15,23,42,0.12)] transition hover:bg-slate-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-slate-900/30"
          >
            <Mail className="h-5 w-5 shrink-0" aria-hidden="true" />
            <div className="text-center">
              <div className="text-sm font-extrabold tracking-wide">{quoteLabel}</div>
              <div className="mt-1 text-xs text-white/90">{quoteSubLabel}</div>
            </div>
          </a>
        </div>

        <div className="mt-10 grid grid-cols-1 gap-10 md:grid-cols-2 md:items-start">
          <div className="w-full">
            <div className="flex flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-[0_20px_50px_rgba(15,23,42,0.10)]">
              <div className="p-4">
                <div className="relative h-[260px] w-full overflow-hidden rounded-xl bg-slate-100 sm:h-[300px]">
                  <img src={imageSrc} alt={imageAlt} className="absolute inset-0 h-full w-full object-cover" loading="lazy" />
                </div>
              </div>
              <div className="px-6 pb-6 pt-1">
                <div className="text-center text-xs font-extrabold tracking-wide text-slate-800 sm:text-sm">
                  {imageCaption}
                </div>
                <div className="mt-4 flex justify-center">
                  <a
                    href={cardCtaHref}
                    className="inline-flex items-center justify-center rounded-full bg-rose-600 px-7 py-3 text-xs font-semibold text-white shadow-sm transition hover:bg-rose-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40 sm:text-sm"
                  >
                    {cardCtaLabel}
                  </a>
                </div>
              </div>
            </div>
          </div>

          <div className="flex min-w-0 flex-col md:pt-4">
            <div>
              {featuresTitle ? <div className="text-sm font-semibold text-slate-700">{featuresTitle}</div> : null}
              <div className="mt-2 md:pr-6">
                <div className="space-y-7">
                  {features.map((f) => (
                    <div key={f.title}>
                      <div className="text-sm font-extrabold tracking-wide text-slate-900 sm:text-base">{f.title}</div>
                      <div className="mt-2 max-w-2xl text-sm leading-6 text-slate-500 sm:leading-7">{f.description}</div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}