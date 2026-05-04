import { QuoteRegisterCard, type QuoteRegisterCardProps } from './QuoteRegisterCard'

export type QuoteRegisterSectionProps = {
  className?: string
  backgroundImageSrc: string
  backgroundAlt?: string
  cardProps: QuoteRegisterCardProps
}

export function QuoteRegisterSection({
  className,
  backgroundImageSrc,
  backgroundAlt = '',
  cardProps,
}: QuoteRegisterSectionProps) {
  return (
    <section id="quote-register" className={['w-full', className ?? ''].join(' ')} aria-label="Quote register section">
      <div className="relative min-h-[360px] overflow-hidden bg-slate-900 sm:min-h-[420px] lg:min-h-[520px]">
        <div className="absolute inset-0">
          <img
            src={backgroundImageSrc}
            alt={backgroundAlt}
            className="h-full w-full object-cover"
            loading="lazy"
            decoding="async"
            draggable={false}
          />
          <div className="absolute inset-0 bg-black/45" />
        </div>

        <div className="relative mx-auto w-full max-w-screen-2xl px-4 py-14 sm:px-6 sm:py-16 lg:px-8 lg:py-24">
          <div className="grid grid-cols-1 items-center gap-6 lg:grid-cols-12">
            <div className="hidden lg:col-span-6 lg:block" aria-hidden="true" />
            <div className="lg:col-span-6 lg:flex lg:justify-end">
              <div className="w-full max-w-[380px]">
                <QuoteRegisterCard {...cardProps} />
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}