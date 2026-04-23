import { Send } from 'lucide-react'
import { useId } from 'react'

export type HomeQuoteRegisterBarValues = {
  fullName: string
  phone: string
}

export type HomeQuoteRegisterBarProps = {
  className?: string
  title?: string
  subtitle?: string
  submitLabel?: string
  onSubmit?: (values: HomeQuoteRegisterBarValues) => void
}

export function HomeQuoteRegisterBar({
  className,
  title = 'Đăng ký nhận báo giá xe',
  subtitle = 'Ưu đãi trong tháng: Miễn Phí 100% Thuế Trước Bạ',
  submitLabel = 'Đăng ký',
  onSubmit,
}: HomeQuoteRegisterBarProps) {
  const nameId = useId()
  const phoneId = useId()

  return (
    <section className={['w-full bg-slate-800', className ?? ''].join(' ')}>
      <div className="mx-auto flex w-full max-w-screen-2xl flex-col gap-5 px-6 py-7 sm:px-8 md:flex-row md:items-center md:justify-between md:gap-8 md:py-8">
        <div className="min-w-0">
          <div className="text-2xl font-semibold text-white md:text-3xl lg:text-4xl">{title}</div>
          <div className="mt-1 text-lg text-slate-200 md:text-xl">
            Ưu đãi trong tháng:{' '}
            <span className="font-semibold text-rose-400">{subtitle.replace(/^Ưu đãi trong tháng:\s*/i, '')}</span>
          </div>
        </div>

        <form
          className="flex w-full flex-col gap-3 sm:flex-row md:w-auto md:justify-end"
          onSubmit={(e) => {
            e.preventDefault()
            const fd = new FormData(e.currentTarget)
            const values = {
              fullName: String(fd.get('fullName') ?? ''),
              phone: String(fd.get('phone') ?? ''),
            }
            onSubmit?.(values)
          }}
          aria-label="Đăng ký nhận báo giá"
        >
          <div className="flex w-full flex-col gap-3 sm:flex-row">
            <div className="w-full sm:w-64">
              <label htmlFor={nameId} className="sr-only">
                Họ tên (Bắt buộc)
              </label>
              <input
                id={nameId}
                name="fullName"
                required
                placeholder="Họ tên (Bắt buộc)"
                className="h-10 w-full rounded-lg border border-white/10 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:ring-2 focus-visible:ring-rose-400/70"
                autoComplete="name"
              />
            </div>

            <div className="w-full sm:w-64">
              <label htmlFor={phoneId} className="sr-only">
                Điện thoại (Bắt buộc)
              </label>
              <input
                id={phoneId}
                name="phone"
                required
                inputMode="tel"
                placeholder="Điện thoại (Bắt buộc)"
                className="h-10 w-full rounded-lg border border-white/10 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:ring-2 focus-visible:ring-rose-400/70"
                autoComplete="tel"
              />
            </div>
          </div>

          <button
            type="submit"
            className="inline-flex h-10 w-full min-w-28 items-center justify-center gap-2 whitespace-nowrap rounded-lg bg-rose-500 px-5 text-sm font-semibold text-white shadow-sm transition hover:bg-rose-600 focus:outline-none focus-visible:ring-2 focus-visible:ring-white/80 sm:w-auto"
          >
            <Send className="h-4 w-4" aria-hidden="true" />
            {submitLabel}
          </button>
        </form>
      </div>
    </section>
  )
}

