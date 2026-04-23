import { Megaphone } from 'lucide-react'
import type { ReactNode } from 'react'

export type CarPriceVersion = {
  name: string
  /** Giá niêm yết (VND, số nguyên) */
  priceVnd: number
}

export type CarPricingBlockProps = {
  name: string
  imageSrc: string
  imageAlt?: string
  versions: CarPriceVersion[]
  /** Tiêu đề phần bảng giá (mặc định: giảm giá tiền mặt) */
  discountTitle?: string
  /** Thay thế toàn bộ nội dung thanh thông báo phía dưới */
  notice?: ReactNode
  className?: string
}

const NAVY = '#0d3458'
const BURGUNDY = '#b01e40'

function formatVndDots(n: number): string {
  return `${n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.')} VND`
}

function DefaultNotice() {
  return (
    <p className="text-sm leading-relaxed text-slate-800 sm:text-[15px]">
      Giá trên là giá công bố của Hãng. Để được mua xe{' '}
      <span className="font-bold text-[#b01e40]">
        Giá tốt nhất + Khuyến Mãi nhiều nhất
      </span>{' '}
      hãy gọi ngay cho Phòng Bán Hàng.
    </p>
  )
}

export function CarPricingBlock({
  name,
  imageSrc,
  imageAlt,
  versions,
  discountTitle = 'GIẢM GIÁ TIỀN MẶT HẤP DẪN',
  notice,
  className,
}: CarPricingBlockProps) {
  return (
    <section
      className={[
        'group/card relative w-full overflow-hidden rounded-2xl border border-[#1D3D63]/15 bg-gradient-to-br from-white via-[#fafbfd] to-[#f0f4fa]',
        'shadow-[0_16px_40px_-12px_rgba(13,52,88,0.14),0_4px_14px_-4px_rgba(13,52,88,0.08)]',
        'ring-1 ring-[#1D3D63]/[0.07] transition-shadow duration-300 hover:shadow-[0_22px_48px_-12px_rgba(13,52,88,0.2)]',
        className ?? '',
      ].join(' ')}
    >
      {/* Vạch màu trên cùng */}
      <div
        className="h-1 w-full bg-gradient-to-r from-[#0d3458] via-[#1D3D63] to-[#b01e40]"
        aria-hidden
      />

      <div className="grid gap-8 p-6 sm:p-8 lg:grid-cols-2 lg:items-start lg:gap-10 xl:gap-12">
        {/* Cột ảnh */}
        <div className="flex flex-col items-center">
          <div className="relative text-center">
            <h2
              className="text-xl font-extrabold tracking-tight sm:text-2xl"
              style={{ color: NAVY }}
            >
              {name}
            </h2>
            <div
              className="mx-auto mt-2 h-1 w-14 rounded-full bg-gradient-to-r from-[#0d3458] to-[#b01e40] opacity-90"
              aria-hidden
            />
          </div>

          <div className="relative mt-5 w-full max-w-[min(100%,28rem)]">
            <div
              className="pointer-events-none absolute -inset-3 rounded-[1.35rem] bg-[radial-gradient(ellipse_80%_70%_at_50%_100%,rgba(13,52,88,0.08),transparent_65%)]"
              aria-hidden
            />
            <div className="relative overflow-hidden rounded-2xl border border-[#1D3D63]/12 bg-gradient-to-b from-[#f8fafc] to-white p-4 shadow-inner ring-1 ring-white/80 sm:p-5">
              <img
                src={imageSrc}
                alt={imageAlt ?? name}
                className="mx-auto h-auto w-full object-contain drop-shadow-[0_16px_32px_rgba(13,52,88,0.18)] transition duration-300 group-hover/card:drop-shadow-[0_20px_40px_rgba(13,52,88,0.22)]"
                loading="lazy"
              />
            </div>
          </div>
        </div>

        {/* Bảng giá */}
        <div className="flex min-w-0 flex-col">
          <h3
            className="text-center text-[13px] font-extrabold uppercase tracking-[0.12em] sm:text-sm sm:tracking-[0.14em]"
            style={{ color: NAVY }}
          >
            {discountTitle}
          </h3>

          <div className="mt-4 overflow-hidden rounded-xl border border-[#1D3D63]/18 shadow-[0_4px_20px_rgba(13,52,88,0.08)]">
            <table className="w-full border-collapse text-sm sm:text-base">
              <thead>
                <tr className="bg-gradient-to-r from-[#0d3458] to-[#1a4d7a] text-white shadow-sm">
                  <th
                    scope="col"
                    className="border-r border-white/20 px-3 py-3 text-left text-[11px] font-bold uppercase tracking-wider sm:px-4 sm:text-xs"
                  >
                    Tên phiên bản
                  </th>
                  <th
                    scope="col"
                    className="px-3 py-3 text-right text-[11px] font-bold uppercase tracking-wider sm:px-4 sm:text-xs"
                  >
                    Giá
                  </th>
                </tr>
              </thead>
              <tbody>
                {versions.map((v, i) => (
                  <tr
                    key={`${v.name}-${i}`}
                    className={[
                      'border-t border-[#1D3D63]/10 transition-colors',
                      i % 2 === 0 ? 'bg-white' : 'bg-[#f7f9fc]',
                      'hover:bg-[#eef3fa]/90',
                    ].join(' ')}
                  >
                    <td className="border-r border-[#1D3D63]/10 px-3 py-3.5 text-left font-semibold text-slate-900 sm:px-4">
                      {v.name}
                    </td>
                    <td
                      className="whitespace-nowrap px-3 py-3.5 text-right text-base font-extrabold tabular-nums sm:px-4 sm:text-[17px]"
                      style={{ color: BURGUNDY }}
                    >
                      {formatVndDots(v.priceVnd)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Footer */}
      <div className="relative border-t border-[#1D3D63]/10 bg-gradient-to-r from-[#f4f7fb] via-[#eef3fa] to-[#f8fafc] px-4 py-4 sm:px-7 sm:py-5">
        <div
          className="absolute left-0 top-0 bottom-0 w-1 rounded-r bg-gradient-to-b from-amber-500 via-orange-500 to-[#b01e40]"
          aria-hidden
        />
        <div className="flex gap-3 pl-4 sm:pl-5">
          <span
            className="mt-0.5 flex size-10 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br from-[#0d3458] to-[#1D3D63] text-white shadow-md ring-1 ring-white/30"
            aria-hidden
          >
            <Megaphone className="size-5" strokeWidth={2.25} />
          </span>
          <div className="min-w-0 pt-0.5">{notice ?? <DefaultNotice />}</div>
        </div>
      </div>
    </section>
  )
}
