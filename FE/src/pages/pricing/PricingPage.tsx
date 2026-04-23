import { useQuery } from '@tanstack/react-query'
import { Car, LoaderCircle, Phone, Sparkles, Tag } from 'lucide-react'
import axios from 'axios'

import { CarPricingBlock } from '../../components/pricing/CarPricingBlock'
import heroPricingCarImage from '../../assets/images/cars/vinfast-vf7-rtqsur7.webp'
import { env } from '../../lib/env'

type PricingVersion = {
  name: string
  priceVnd: number
}

type PricingCar = {
  carId: number
  name: string
  imageUrl?: string | null
  versions: PricingVersion[]
}

type PricingCarsResponse = {
  data: PricingCar[]
}

const pricingApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

function getPricingCarImageSrc(car: PricingCar) {
  const raw = car.imageUrl?.trim()
  return raw ? new URL(raw, env.VITE_API_BASE_URL).toString() : ''
}

async function getPricingCars(): Promise<PricingCar[]> {
  const response = await pricingApi.get<PricingCarsResponse>('Pricing/cars')
  const cars = Array.isArray(response.data?.data) ? response.data.data : []
  return cars
}

const pricingHeroHighlights = [
  { Icon: Car, label: 'Đầy đủ phiên bản' },
  { Icon: Sparkles, label: 'Ưu đãi cập nhật' },
  { Icon: Phone, label: 'Tư vấn & báo giá' },
] as const

export function PricingPage() {
  const { data: pricingCars = [], isLoading, isError } = useQuery({
    queryKey: ['pricing-cars'],
    queryFn: getPricingCars,
  })

  return (
    <main className="relative w-full overflow-hidden bg-[#e8edf3] py-10 sm:py-14">
      {/* Nền trang: gradient dọc + mesh + lưới chấm nhẹ */}
      <div
        className="pointer-events-none absolute inset-0 bg-[linear-gradient(180deg,#f4f7fb_0%,#e8edf3_38%,#eef2f7_72%,#e4eaf2_100%)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_110%_60%_at_50%_-8%,rgba(13,52,88,0.16),transparent_58%)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_70%_50%_at_100%_15%,rgba(176,30,64,0.09),transparent_52%)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_55%_45%_at_0%_85%,rgba(13,52,88,0.1),transparent_55%)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-0 opacity-[0.45] [background-image:radial-gradient(rgba(51,65,85,0.14)_1px,transparent_1px)] [background-size:22px_22px] [mask-image:linear-gradient(180deg,transparent,black_12%,black_88%,transparent)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-white/70 to-transparent"
        aria-hidden
      />

      <div className="relative z-10 mx-auto w-full max-w-screen-2xl px-6">
        <section
          className="relative mb-10 overflow-hidden rounded-2xl border border-[#1D3D63]/18 bg-gradient-to-br from-white via-[#f3f6fb] to-[#e8eef6] px-6 py-10 shadow-[0_12px_40px_-8px_rgba(13,52,88,0.18),0_4px_16px_-4px_rgba(139,21,56,0.08),inset_0_1px_0_rgba(255,255,255,0.9)] ring-1 ring-[#1D3D63]/10 sm:mb-12 sm:px-10 sm:py-12"
          aria-labelledby="pricing-page-title"
        >
          {/* Nền trong thẻ: mesh + vệt sáng + viền sáng trong */}
          <div
            className="pointer-events-none absolute inset-0 rounded-2xl bg-[linear-gradient(125deg,rgba(255,255,255,0.95)_0%,transparent_42%,rgba(226,236,248,0.55)_100%)]"
            aria-hidden
          />
          <div
            className="pointer-events-none absolute -right-24 top-1/2 h-[min(340px,75vw)] w-[min(380px,90vw)] -translate-y-1/2 bg-[radial-gradient(ellipse_68%_58%_at_75%_45%,rgba(13,52,88,0.22),transparent_72%)]"
            aria-hidden
          />
          <div
            className="pointer-events-none absolute -left-20 bottom-0 h-52 w-80 bg-[radial-gradient(circle_at_center,rgba(176,30,64,0.16),transparent_68%)]"
            aria-hidden
          />
          <div
            className="pointer-events-none absolute left-1/2 top-0 h-[45%] w-[85%] -translate-x-1/2 bg-[radial-gradient(ellipse_90%_100%_at_50%_0%,rgba(255,255,255,0.75),transparent_72%)]"
            aria-hidden
          />
          <div
            className="pointer-events-none absolute inset-0 rounded-2xl opacity-[0.35] [background-image:radial-gradient(rgba(100,116,139,0.12)_1px,transparent_1px)] [background-size:18px_18px] [mask-image:linear-gradient(165deg,black_20%,transparent_75%)]"
            aria-hidden
          />
          <div
            className="pointer-events-none absolute inset-0 rounded-2xl ring-1 ring-inset ring-white/60"
            aria-hidden
          />

          <div className="relative grid grid-cols-1 items-center gap-8 lg:grid-cols-[minmax(0,1.08fr)_minmax(0,0.92fr)] lg:gap-10 xl:gap-12">
            <div className="relative min-w-0 text-center sm:text-left sm:pl-7 sm:before:absolute sm:before:left-0 sm:before:top-1 sm:before:bottom-1 sm:before:w-[5px] sm:before:rounded-full sm:before:bg-gradient-to-b sm:before:from-[#0d3458] sm:before:via-[#1D3D63] sm:before:to-[#b01e40] sm:before:shadow-[3px_0_18px_rgba(13,52,88,0.35)] sm:before:content-['']">
              <div className="inline-flex items-center gap-2 rounded-full border border-[#1D3D63]/35 bg-gradient-to-r from-[#1D3D63]/[0.12] to-[#b01e40]/[0.1] px-3.5 py-1.5 shadow-[0_4px_16px_rgba(13,52,88,0.12)] ring-1 ring-white/60 backdrop-blur-sm transition hover:border-[#1D3D63]/50 hover:shadow-[0_6px_20px_rgba(13,52,88,0.18)]">
                <span
                  className="flex h-2 w-2 shrink-0 animate-pulse rounded-full bg-[#b01e40] shadow-[0_0_8px_rgba(176,30,64,0.85)]"
                  aria-hidden
                />
                <Tag className="h-3.5 w-3.5 shrink-0 text-[#b01e40]" aria-hidden />
                <span className="text-[10px] font-extrabold uppercase tracking-[0.26em] text-[#0d3458] sm:text-[11px]">
                  Niêm yết 
                </span>
              </div>

              <h1
                id="pricing-page-title"
                className="mt-5 text-[1.85rem] font-extrabold leading-[1.12] tracking-tight sm:text-4xl md:text-[2.45rem] md:leading-[1.1]"
              >
                <span className="text-[#0d3458]">Bảng giá xe</span>
              </h1>

              <div
                className="mx-auto mt-3 h-1.5 w-20 rounded-full bg-gradient-to-r from-[#0d3458] via-[#b01e40] to-[#0d3458] shadow-[0_2px_12px_rgba(176,30,64,0.35)] sm:mx-0 sm:w-24"
                aria-hidden
              />

              <p className="mx-auto mt-5 max-w-xl text-sm font-medium leading-relaxed text-[#2d4a63] sm:mx-0 sm:text-base sm:leading-relaxed">
                Giá niêm yết theo phiên bản. Liên hệ để nhận ưu đãi và khuyến mãi mới nhất.
              </p>

              <ul className="mx-auto mt-6 flex max-w-xl flex-wrap justify-center gap-2.5 sm:mx-0 sm:justify-start sm:gap-3">
                {pricingHeroHighlights.map(({ Icon, label }) => (
                  <li
                    key={label}
                    className="inline-flex items-center gap-2 rounded-xl border border-[#1D3D63]/28 bg-gradient-to-br from-white to-[#eef3fa] px-3 py-2 text-left text-[11px] font-bold text-[#0d3458] shadow-[0_4px_16px_rgba(13,52,88,0.1)] ring-1 ring-[#1D3D63]/10 sm:text-xs"
                  >
                    <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-gradient-to-br from-[#1D3D63]/18 to-[#b01e40]/15 text-[#b01e40] shadow-inner ring-1 ring-[#1D3D63]/15">
                      <Icon className="h-3.5 w-3.5" aria-hidden />
                    </span>
                    {label}
                  </li>
                ))}
              </ul>
            </div>

            <div className="relative mx-auto w-full max-w-md lg:mx-0 lg:max-w-none">
              <div
                className="relative aspect-[5/3] overflow-hidden rounded-2xl shadow-[0_20px_48px_-10px_rgba(13,52,88,0.4)] ring-2 ring-[#1D3D63]/35 ring-offset-2 ring-offset-[#eef3fa] sm:aspect-[16/10]"
                aria-hidden
              >
                <img
                  src={heroPricingCarImage}
                  alt=""
                  className="h-full w-full object-cover object-center"
                  loading="eager"
                  decoding="async"
                />
                <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-[#0d3458]/70 via-[#0d3458]/15 to-transparent" />
                <div className="pointer-events-none absolute inset-0 bg-[linear-gradient(115deg,rgba(255,255,255,0.25)_0%,transparent_42%)]" />
                <div className="absolute bottom-3 left-3 right-3 flex items-end justify-between gap-2 sm:bottom-4 sm:left-4 sm:right-4">
                  <p className="max-w-[70%] text-[10px] font-extrabold uppercase tracking-[0.2em] text-white drop-shadow-[0_2px_8px_rgba(0,0,0,0.45)] sm:text-[11px]">
                    Đa dạng dòng xe
                  </p>
                </div>
              </div>
              <div
                className="pointer-events-none absolute -right-3 -top-3 hidden h-28 w-28 rounded-full bg-[radial-gradient(circle_at_center,rgba(176,30,64,0.45),transparent_68%)] blur-2xl sm:block"
                aria-hidden
              />
              <div
                className="pointer-events-none absolute -bottom-4 -left-4 hidden h-32 w-32 rounded-full bg-[radial-gradient(circle_at_center,rgba(13,52,88,0.35),transparent_70%)] blur-2xl sm:block"
                aria-hidden
              />
            </div>
          </div>
        </section>

        {isError ? (
          <div className="mb-8 rounded-2xl border border-rose-200 bg-rose-50/90 px-4 py-3 text-sm font-medium text-rose-700 shadow-sm sm:px-5">
            Không thể tải bảng giá. Vui lòng thử lại sau.
          </div>
        ) : null}

        {isLoading ? (
          <div className="flex min-h-40 items-center justify-center rounded-2xl border border-[#1D3D63]/12 bg-white/70 px-6 py-12 shadow-sm">
            <div className="inline-flex items-center gap-3 text-sm font-semibold text-[#0d3458]">
              <LoaderCircle className="h-5 w-5 animate-spin" aria-hidden />
              Đang tải bảng giá...
            </div>
          </div>
        ) : null}

        {!isLoading && !isError && pricingCars.length === 0 ? (
          <div className="rounded-2xl border border-[#1D3D63]/12 bg-white/70 px-6 py-10 text-center text-sm font-semibold text-slate-700 shadow-sm">
            Chưa có dữ liệu bảng giá.
          </div>
        ) : null}

        {!isLoading && !isError && pricingCars.length > 0 ? (
          <div className="flex flex-col gap-10 sm:gap-12">
            {pricingCars.map((car) => (
              <CarPricingBlock
                key={car.carId}
                name={car.name}
                imageSrc={getPricingCarImageSrc(car)}
                versions={[...car.versions]}
              />
            ))}
          </div>
        ) : null}
      </div>
    </main>
  )
}
