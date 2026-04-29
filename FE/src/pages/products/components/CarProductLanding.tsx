import { Download, Gift } from 'lucide-react'
import { useCallback, useEffect, useId, useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'

import banner2 from '../../../assets/images/banner2.jpg'
import banner3 from '../../../assets/images/banner3.jpg'
import { BannerCarousel } from '../../../components/BannerCarousel'
import type { CarProductMeta } from '../carProductCatalog'

const NAVY = '#1a3a5a'
const BLUE_BTN = '#1e4a7a'
const PROMO_HEADER_BG = '#24486a'
const PROMO_HIGHLIGHT = '#e31a27'

const trialFormInputClassName =
  'h-11 w-full rounded-md border border-[#e0e0e0] bg-white px-3.5 text-sm text-slate-900 shadow-sm placeholder:text-slate-400 outline-none transition placeholder:font-normal focus:border-[#24486a] focus:shadow-md focus:ring-2 focus:ring-[#24486a]/20'

function SectionHeading({
  tag,
  title,
  intro,
  introClassName,
}: {
  tag: string
  title: string
  intro: string
  introClassName?: string
}) {
  return (
    <div className="grid gap-6 lg:grid-cols-[auto_1fr] lg:gap-10">
      <div
        className="flex h-12 min-w-[3.5rem] items-center justify-center rounded bg-red-600 px-3 text-xs font-bold uppercase tracking-wider text-white sm:text-sm"
        aria-hidden
      >
        {tag}
      </div>
      <div className="space-y-4">
        <h2 className="text-2xl font-bold tracking-tight sm:text-3xl" style={{ color: NAVY }}>
          {title}
        </h2>
        <p
          className={[
            'whitespace-pre-line text-[15px] leading-relaxed text-slate-700 sm:text-base',
            introClassName ?? 'max-w-3xl',
          ].join(' ')}
        >
          {intro}
        </p>
      </div>
    </div>
  )
}

function OverviewGalleryBlock({
  slides,
  fallbackSingleSrc,
  fallbackAlt,
}: {
  slides: { src: string; alt: string; title?: string | null; description?: string | null }[]
  fallbackSingleSrc?: string
  fallbackAlt: string
}) {
  const carouselSlides =
    slides.length > 0
      ? slides
      : fallbackSingleSrc
        ? [{ src: fallbackSingleSrc, alt: fallbackAlt }]
        : []

  const [activeIndex, setActiveIndex] = useState(0)

  useEffect(() => {
    setActiveIndex(0)
  }, [carouselSlides])

  const activeSlide = carouselSlides[activeIndex]
  const activeDesc = (activeSlide?.description ?? '').trim()

  return (
    <div className="mt-8 grid gap-8 lg:grid-cols-12 lg:items-start">
      <div className="lg:col-span-7">
        {carouselSlides.length > 0 ? (
          <div className="overflow-hidden rounded-lg bg-white shadow-md ring-1 ring-black/5">
            <BannerCarousel
              slides={carouselSlides}
              intervalMs={5000}
              activeIndex={activeIndex}
              onActiveIndexChange={setActiveIndex}
              hideDots={carouselSlides.length > 1}
            />
          </div>
        ) : (
          <p className="rounded-lg border border-dashed border-slate-200 bg-slate-50 px-4 py-8 text-center text-sm text-slate-500">
            Chưa có hình ảnh cho mục này.
          </p>
        )}
      </div>
      <div className="lg:col-span-5">
        {activeDesc ? (
          <div className="whitespace-pre-line text-[15px] leading-relaxed text-slate-700 sm:text-base">{activeDesc}</div>
        ) : (
          <p className="text-[15px] text-slate-400 sm:text-base">—</p>
        )}
      </div>
    </div>
  )
}

function BulletList({ items }: { items: string[] }) {
  if (!items?.length) return null
  return (
    <ul className="mt-4 list-disc space-y-2 pl-5 text-[15px] leading-relaxed text-slate-700 sm:text-base">
      {items.map((b) => (
        <li key={b}>{b}</li>
      ))}
    </ul>
  )
}

const FALLBACK_HEX_SWATCHES = [
  { id: 'red', label: 'Đỏ', hex: '#b91c1c' },
  { id: 'white', label: 'Trắng', hex: '#ffffff' },
  { id: 'silver', label: 'Bạc', hex: '#d1d5db' },
  { id: 'blue', label: 'Xanh', hex: '#1d4ed8' },
  { id: 'black', label: 'Đen', hex: '#111827' },
  { id: 'brown', label: 'Nâu', hex: '#92400e' },
] as const

function DetailGallerySplitBlock({
  slides,
  fallbackSingleSrc,
  fallbackAlt,
  title,
  tag,
}: {
  slides: { src: string; alt: string; title?: string | null; description?: string | null }[]
  fallbackSingleSrc?: string
  fallbackAlt: string
  title: string
  tag: string
}) {
  const carouselSlides =
    slides.length > 0
      ? slides
      : fallbackSingleSrc
        ? [{ src: fallbackSingleSrc, alt: fallbackAlt }]
        : []

  const [activeIndex, setActiveIndex] = useState(0)

  useEffect(() => {
    setActiveIndex(0)
  }, [carouselSlides])

  const activeSlide = carouselSlides[activeIndex]
  const activeTitle = (activeSlide?.title ?? '').trim()
  const activeDesc = (activeSlide?.description ?? '').trim()

  return (
    <div className="space-y-8">
      <SectionHeading tag={tag} title={title} intro={activeTitle || '—'} />
      <div className="grid gap-8 lg:grid-cols-12 lg:items-start">
        <div className="lg:col-span-7">
          {carouselSlides.length > 0 ? (
            <div className="overflow-hidden rounded-lg bg-white shadow-md ring-1 ring-black/5">
              <BannerCarousel
                slides={carouselSlides}
                intervalMs={5000}
                activeIndex={activeIndex}
                onActiveIndexChange={setActiveIndex}
                hideDots={carouselSlides.length > 1}
              />
            </div>
          ) : (
            <p className="rounded-lg border border-dashed border-slate-200 bg-slate-50 px-4 py-8 text-center text-sm text-slate-500">
              Chưa có hình ảnh cho mục này.
            </p>
          )}
        </div>
        <div className="lg:col-span-5">
          {activeDesc ? (
            <div className="whitespace-pre-line text-[15px] leading-relaxed text-slate-700 sm:text-base">{activeDesc}</div>
          ) : (
            <p className="text-[15px] text-slate-400 sm:text-base">—</p>
          )}
        </div>
      </div>
    </div>
  )
}

export function CarProductLanding({ name, imageSrc, priceText, content, features }: CarProductMeta) {
  const navigate = useNavigate()
  const trialNameFieldId = useId()
  const trialPhoneFieldId = useId()
  const [trialFullName, setTrialFullName] = useState('')
  const [trialPhone, setTrialPhone] = useState('')
  const stickyHeaderOffsetClass = 'top-[80px]'

  const onBrochure = useCallback(() => {
    toast.success('Brochure sẽ được gửi qua email khi có kết nối hệ thống.')
  }, [])

  const apiColorList = content.colorGallery
  const isApiColorSource = apiColorList !== undefined

  const colorSwatches = useMemo(() => {
    if (isApiColorSource) {
      if (!apiColorList?.length) return []
      return apiColorList.map((c) => ({ kind: 'image' as const, id: c.id, label: c.label, imageSrc: c.imageSrc }))
    }
    return FALLBACK_HEX_SWATCHES.map((c) => ({ kind: 'hex' as const, ...c }))
  }, [apiColorList, isApiColorSource])

  const [selectedColorId, setSelectedColorId] = useState(() => colorSwatches[0]?.id ?? '')

  useEffect(() => {
    const first = colorSwatches[0]?.id ?? ''
    setSelectedColorId((prev) => (colorSwatches.some((c) => c.id === prev) ? prev : first))
  }, [colorSwatches])

  const heroImageSrc = useMemo(() => {
    if (isApiColorSource && apiColorList?.length && selectedColorId) {
      const hit = apiColorList.find((c) => c.id === selectedColorId) ?? apiColorList[0]
      if (hit?.imageSrc) return hit.imageSrc
    }
    return imageSrc
  }, [apiColorList, isApiColorSource, selectedColorId, imageSrc])

  const pricingTableRows = useMemo(() => {
    if (content.pricingRows?.length) return content.pricingRows
    return [{ name, priceText }]
  }, [content.pricingRows, name, priceText])

  const gallerySlides = useMemo(() => {
    const all = content.galleryAll?.length ? content.galleryAll : content.gallery
    if (all?.length) return all
    return [
      { src: imageSrc, alt: `${name} — ảnh 1` },
      { src: banner2, alt: `${name} — ảnh 2 (demo)` },
      { src: banner3, alt: `${name} — ảnh 3 (demo)` },
    ]
  }, [content.galleryAll, content.gallery, imageSrc, name])

  const [galleryHeroIndex, setGalleryHeroIndex] = useState(0)

  useEffect(() => {
    setGalleryHeroIndex(0)
  }, [gallerySlides])

  return (
    <div className="w-full bg-white text-slate-900">
      {/* Hero */}
      <section className="border-b border-slate-100 bg-white">
        {/* Giữ nguyên Hero Section của bạn */}
        <div className="mx-auto w-full max-w-screen-2xl px-5 py-10 sm:px-6 lg:px-8 lg:py-14">
          <div className="grid gap-10 lg:grid-cols-12 lg:items-start lg:gap-8 xl:gap-12">
            <div className="lg:col-span-5">
              <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                <img
                  src={heroImageSrc}
                  alt={name}
                  className="aspect-[4/3] w-full object-contain p-6 sm:aspect-[16/10]"
                  loading="eager"
                />
              </div>

              {colorSwatches.length > 0 ? (
                <div className="mt-5 flex flex-wrap items-center justify-center gap-3 sm:gap-4">
                  {colorSwatches.map((c) => {
                    const isActive = c.id === selectedColorId
                    if (c.kind === 'image') {
                      return (
                        <button
                          key={c.id}
                          type="button"
                          onClick={() => setSelectedColorId(c.id)}
                          className={[
                            'h-11 w-11 overflow-hidden rounded-full transition cursor-pointer',
                            isActive ? 'ring-4 ring-slate-900/20' : 'ring-1 ring-black/10 hover:ring-2 hover:ring-slate-900/10',
                          ].join(' ')}
                          aria-label={`Chọn màu ${c.label}`}
                          aria-pressed={isActive}
                        >
                          <img src={c.imageSrc} alt="" className="h-full w-full object-cover" />
                        </button>
                      )
                    }
                    const isLight = c.hex.toLowerCase() === '#ffffff'
                    return (
                      <button
                        key={c.id}
                        type="button"
                        onClick={() => setSelectedColorId(c.id)}
                        className={[
                          'h-11 w-11 rounded-full transition cursor-pointer',
                          isActive ? 'ring-4 ring-slate-900/10' : 'ring-1 ring-black/10 hover:ring-2 hover:ring-slate-900/10 ',
                        ].join(' ')}
                        style={{
                          backgroundColor: c.hex,
                          boxShadow: isLight ? 'inset 0 0 0 1px rgba(15, 23, 42, 0.12)' : undefined,
                        }}
                        aria-label={`Chọn màu ${c.label}`}
                        aria-pressed={isActive}
                      />
                    )
                  })}
                </div>
              ) : null}

              <div className="mt-6 overflow-hidden rounded-md border border-slate-200 bg-white">
                <table className="w-full table-fixed">
                  <thead>
                    <tr style={{ backgroundColor: NAVY }}>
                      <th className="px-5 py-3 text-left text-sm font-bold uppercase tracking-wider text-white">
                        Tên phiên bản
                      </th>
                      <th className="px-5 py-3 text-right text-sm font-bold uppercase tracking-wider text-white">Giá</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pricingTableRows.map((row, idx) => (
                      <tr key={`${row.name}-${idx}`} className="border-t border-slate-200">
                        <td className="px-5 py-4 text-base font-semibold text-slate-700">{row.name}</td>
                        <td className="px-5 py-4 text-right text-base font-semibold text-slate-700">{row.priceText}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
            <div className="flex flex-col gap-6 lg:col-span-7">
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl" style={{ color: NAVY }}>
                {name}
              </h1>
              <div className="grid grid-cols-1 gap-10 lg:grid-cols-7 lg:items-start lg:gap-8 xl:gap-12">
                <div className="lg:col-span-4">
                  <div className="rounded-md border border-dashed border-[#e31a27] bg-[#f8f9fa] p-3 sm:p-4">
                    <div className="text-center text-base font-extrabold uppercase tracking-wide text-white sm:text-lg">
                      <div className="rounded-sm py-2.5 shadow-sm" style={{ backgroundColor: PROMO_HEADER_BG }}>
                        Chương trình khuyến mãi
                      </div>
                    </div>
                    <ul className="mt-3 rounded-sm bg-[#f0f2f4] px-3 py-1 sm:px-4">
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Giảm trực tiếp{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        6-10%
                      </span>{' '}
                      giá trị xe
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Ưu đãi thu{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        xe xăng
                      </span>{' '}
                      sang{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        xe điện
                      </span>{' '}
                      giảm thêm{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        3%
                      </span>{' '}
                      giá trị xe
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Ưu đãi cho đối tượng{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        Công An, Quân Đội
                      </span>{' '}
                      giảm thêm{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        5%
                      </span>{' '}
                      giá trị xe
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Miễn{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        100%
                      </span>{' '}
                      Thuế trước bạ
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Ưu đãi sạc pin{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        miễn phí
                      </span>{' '}
                      đến{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        10/02/2029
                      </span>
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Xe có sẵn, nhiều màu để lựa chọn và sẵn sàng{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        giao ngay
                      </span>{' '}
                      theo yêu cầu của khách hàng
                    </span>
                  </li>
                  <li className="flex gap-3 border-b border-dashed border-sky-300 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Đăng ký lái thử{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        miễn phí
                      </span>{' '}
                      24/7
                    </span>
                  </li>
                  <li className="flex gap-3 py-3 text-[15px] leading-relaxed text-slate-900 sm:text-base">
                    <span className="mt-0.5 shrink-0" style={{ color: PROMO_HIGHLIGHT }}>
                      <Gift className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>
                      Bảo hiểm thân vỏ trong 2 năm hoặc quy đổi{' '}
                      <span className="font-bold" style={{ color: PROMO_HIGHLIGHT }}>
                        15 triệu
                      </span>{' '}
                      tiền mặt
                    </span>
                  </li>
                    </ul>
                  </div>
                </div>

                <div className="flex flex-col gap-4 lg:col-span-3">
              <div
                className="rounded-lg px-5 py-6 text-center text-white shadow-lg"
                style={{
                  background: 'linear-gradient(145deg, #9f1239 0%, #7f1d1d 55%, #450a0a 100%)',
                }}
              >
                <p className="text-lg font-extrabold uppercase tracking-[0.12em] sm:text-xl">Nhận báo giá</p>
                <p className="mt-2 text-sm text-white/90">Để lại thông tin — chúng tôi gửi báo giá & ưu đãi trong 24h.</p>
                <a
                  href="tel:1900123456"
                  className="mt-5 inline-flex h-11 w-full items-center justify-center rounded-md bg-white text-sm font-bold uppercase tracking-wide text-red-900 transition hover:bg-red-50"
                >
                  Gọi hotline
                </a>
              </div>
              <button
                type="button"
                onClick={onBrochure}
                className="inline-flex h-12 w-full items-center justify-center gap-2 rounded-md text-sm font-bold uppercase tracking-wide text-white transition hover:opacity-95"
                style={{ backgroundColor: BLUE_BTN }}
              >
                <Download className="h-4 w-4" aria-hidden />
                Tải brochure
              </button>
              <div className="rounded-lg border border-slate-200/90 bg-white p-4 shadow-[0_2px_16px_rgba(15,23,42,0.06)] ring-1 ring-slate-900/[0.04] sm:p-5">
                <p
                  className="mb-4 border-b border-slate-100 pb-3 text-center text-[13px] font-bold uppercase tracking-[0.08em] sm:text-sm"
                  style={{ color: NAVY }}
                >
                  Đăng ký lái thử
                </p>
                <form
                  className="flex flex-col gap-4"
                  onSubmit={(e) => {
                    e.preventDefault()
                    const full = trialFullName.trim()
                    const phone = trialPhone.trim()
                    if (!full || !phone) {
                      toast.error('Vui lòng nhập họ tên và số điện thoại.')
                      return
                    }
                    if (!/^\d{10}$/.test(phone)) {
                      toast.error('Số điện thoại phải gồm đúng 10 chữ số.')
                      return
                    }
                    const carIdFromUrl = window.location.pathname.split('/').pop() ?? ''
                    navigate('/lai-thu', {
                      state: { trialPrefill: { fullName: full, phone, carId: carIdFromUrl,carImage: heroImageSrc } },
                    })
                  }}
                >
                  <div className="space-y-2">
                    <label htmlFor={trialNameFieldId} className="block text-left text-[11px] font-semibold uppercase tracking-wide text-slate-600 sm:text-xs">
                      Họ và tên
                    </label>
                    <input
                      id={trialNameFieldId}
                      name="trialFullName"
                      value={trialFullName}
                      onChange={(ev) => setTrialFullName(ev.target.value)}
                      placeholder="Nhập họ và tên"
                      autoComplete="name"
                      className={trialFormInputClassName}
                    />
                  </div>
                  <div className="space-y-2">
                    <label htmlFor={trialPhoneFieldId} className="block text-left text-[11px] font-semibold uppercase tracking-wide text-slate-600 sm:text-xs">
                      Số điện thoại
                    </label>
                    <input
                      id={trialPhoneFieldId}
                      name="trialPhone"
                      value={trialPhone}
                      onChange={(ev) => setTrialPhone(ev.target.value.replace(/\D/g, '').slice(0, 10))}
                      placeholder="Nhập số điện thoại (10 số)"
                      inputMode="numeric"
                      autoComplete="tel"
                      maxLength={10}
                      className={trialFormInputClassName}
                    />
                  </div>
                  <button
                    type="submit"
                    className="mt-1 inline-flex h-12 w-full items-center justify-center rounded-md text-sm font-bold uppercase tracking-[0.12em] text-white shadow-sm transition hover:brightness-110 active:brightness-95 cursor-pointer"
                    style={{ backgroundColor: PROMO_HEADER_BG }}
                  >
                    Đăng ký lái thử
                  </button>
                </form>
              </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Sticky sub-nav: ĐÃ CHÈN "Tính năng" VÀO TRƯỚC "Ngoại thất" */}
      <nav
        className={`sticky ${stickyHeaderOffsetClass} z-40 border-b border-white/10 shadow-sm`}
        style={{ backgroundColor: NAVY }}
        aria-label={`Mục lục ${name}`}
      >
        <div className="mx-auto flex max-w-screen-2xl flex-wrap items-center justify-center gap-1 px-3 py-2 sm:gap-2 sm:px-6 lg:px-8">
          {[
            ['#car-hinh-anh', 'Hình ảnh'],
            ['#car-tong-quan', 'Tổng quan'],
            ['#car-tinh-nang', 'Tính năng'],
            ['#car-ngoai-that', 'Ngoại thất'],
            ['#car-noi-that', 'Nội thất'],
            ['#car-an-toan', 'An toàn'],
            ['#car-van-hanh', 'Vận hành'],
            ['#car-thong-so', 'Thông số'],
          ].map(([href, label]) => (
            <a
              key={href}
              href={href}
              className="rounded px-3 py-2 text-[11px] font-semibold uppercase tracking-wide text-white/95 transition hover:bg-white/10 sm:text-xs"
            >
              {label}
            </a>
          ))}
          <Link
            to="/san-pham"
            className="rounded px-3 py-2 text-[11px] font-semibold uppercase tracking-wide text-white/95 transition hover:bg-white/10 sm:text-xs"
          >
            ← Danh sách xe
          </Link>
        </div>
      </nav>

      {/* Hình ảnh */}
      <section id="car-hinh-anh" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80 pb-14 pt-10 sm:pb-16 sm:pt-12">
        <div className="mx-auto max-w-screen-2xl px-5 sm:px-6 lg:px-8">
          <h2 className="text-center text-2xl font-bold tracking-tight sm:text-3xl" style={{ color: NAVY }}>
            Hình ảnh {name}
          </h2>
          <div className="mt-8 overflow-hidden rounded-lg bg-white shadow-md ring-1 ring-black/5">
            <BannerCarousel
              slides={gallerySlides}
              intervalMs={5000}
              activeIndex={galleryHeroIndex}
              onActiveIndexChange={setGalleryHeroIndex}
              hideDots={gallerySlides.length > 1}
            />
          </div>
          {gallerySlides.length > 1 ? (
            <div className="mt-4 flex justify-center sm:mt-5">
              <div
                className="inline-flex max-w-full gap-2 overflow-x-auto [-ms-overflow-style:none] sm:gap-3 [scrollbar-width:thin] px-1 py-1"
                role="tablist"
                aria-label={`Ảnh xem nhanh — ${name}`}
              >
                {gallerySlides.map((slide, idx) => {
                  const active = idx === galleryHeroIndex
                  return (
                    <button
                      key={`${slide.src}-${idx}`}
                      type="button"
                      role="tab"
                      aria-selected={active}
                      onClick={() => setGalleryHeroIndex(idx)}
                      className={[
                        'group relative shrink-0 overflow-hidden rounded-md bg-white shadow-sm ring-1 ring-black/10 transition cursor-pointer',
                        active
                          ? 'ring-2 ring-sky-600 ring-offset-2 ring-offset-slate-50'
                          : 'opacity-[0.88] hover:opacity-100 hover:ring-slate-400',
                      ].join(' ')}
                    >
                      <img
                        src={slide.src}
                        alt=""
                        className="h-14 w-[5.25rem] object-cover sm:h-16 sm:w-28 "
                        loading="lazy"
                        decoding="async"
                        draggable={false}
                      />
                      <span className="sr-only">
                        Ảnh {idx + 1}/{gallerySlides.length}
                      </span>
                    </button>
                  )
                })}
              </div>
            </div>
          ) : null}
        </div>
      </section>

      {/* Tổng quan */}
      <section id="car-tong-quan" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-8 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          {content.overviewSplit ? (
            <>
              <SectionHeading
                tag="01"
                title={`Tổng quan ${name}`}
                intro={content.overviewSplit.intro || content.overviewIntro}
                introClassName="max-w-none"
              />
              <OverviewGalleryBlock
                slides={content.overviewSplit.slides}
                fallbackSingleSrc={imageSrc}
                fallbackAlt={`${name} — tổng quan`}
              />
            </>
          ) : (
            <>
              <SectionHeading
                tag="01"
                title={`Tổng quan ${name}`}
                intro={content.overviewIntro}
                introClassName="max-w-none"
              />
            </>
          )}
        </div>
      </section>

      {/* TÍNH NĂNG */}
      <section id="car-tinh-nang" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-8 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading 
            tag="02" 
            title="Tính năng" 
            intro="Các tính năng tiện ích được trang bị sẵn trên phiên bản này." 
          />
          {features && features.length > 0 ? (
            <div className="mt-8 relative w-full">
              {/* Container cuộn ngang, ẩn thanh cuộn mặc định của trình duyệt */}
              <div className="flex gap-4 sm:gap-6 overflow-x-auto pb-6 snap-x snap-mandatory [-ms-overflow-style:none] [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
                {features.map((feature: any) => (
                  <div 
                    key={feature.featureId} 
                    // shrink-0 để không bị ép nhỏ lại, w-[140px] hoặc w-[160px] tuỳ chỉnh độ rộng thẻ
                    className="shrink-0 w-[140px] sm:w-[160px] snap-start flex flex-col items-center justify-start p-5 border border-slate-200 rounded-xl bg-white text-center hover:shadow-md transition hover:-translate-y-1"
                  >
                    <div className="h-16 w-16 mb-4 flex shrink-0 items-center justify-center rounded-full bg-slate-50 border border-slate-100 p-3 shadow-sm">
                      <img
                        src={
                          feature.icon?.startsWith('http') || feature.icon?.startsWith('/')
                            ? feature.icon
                            : `https://localhost:7033${feature.icon}`
                        }
                        alt={feature.featureName}
                        className="w-full h-full object-contain"
                        onError={(e) => { e.currentTarget.src = '/fallback-icon.png' }}
                      />
                    </div>
                    <span className="text-[13px] sm:text-sm font-semibold text-slate-700 leading-snug">
                      {feature.featureName}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <p className="text-sm text-slate-500 mt-4 rounded-lg border border-dashed border-slate-300 bg-white px-4 py-8 text-center">
              Chưa có thông tin về tính năng cho phiên bản này.
            </p>
          )}
        </div>
      </section>

      {/* Ngoại thất (Cập nhật Tag thành 03) */}
      <section id="car-ngoai-that" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          {content.exteriorSplit ? (
            <DetailGallerySplitBlock
              tag="03"
              title="Ngoại thất"
              slides={content.exteriorSplit.slides}
              fallbackSingleSrc={imageSrc}
              fallbackAlt={`${name} — ngoại thất`}
            />
          ) : (
            <>
              <SectionHeading tag="03" title="Ngoại thất" intro={content.exteriorIntro} />
              <div className="grid gap-8 lg:grid-cols-12 lg:items-start">
                <div className="lg:col-span-7">
                  <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                    <img
                      src={imageSrc}
                      alt={`${name} — ngoại thất`}
                      className="aspect-[16/9] w-full object-contain p-6"
                      loading="lazy"
                    />
                  </div>
                </div>
                <div className="lg:col-span-5">
                  <h3 className="text-lg font-bold" style={{ color: NAVY }}>
                    Điểm nhấn ngoại thất
                  </h3>
                  <BulletList items={content.exteriorBullets} />
                </div>
              </div>
            </>
          )}
        </div>
      </section>

      {/* Nội thất (Cập nhật Tag thành 04) */}
      <section id="car-noi-that" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          {content.interiorSplit ? (
            <DetailGallerySplitBlock
              tag="04"
              title="Nội thất"
              slides={content.interiorSplit.slides}
              fallbackSingleSrc={imageSrc}
              fallbackAlt={`${name} — nội thất`}
            />
          ) : (
            <>
              <SectionHeading tag="04" title="Nội thất" intro={content.interiorIntro} />
              <div className="grid gap-8 lg:grid-cols-12 lg:items-start">
                <div className="lg:col-span-7">
                  <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                    <img
                      src={banner2}
                      alt={`${name} — nội thất (demo)`}
                      className="aspect-[16/9] w-full object-cover"
                      loading="lazy"
                    />
                  </div>
                </div>
                <div className="lg:col-span-5">
                  <h3 className="text-lg font-bold" style={{ color: NAVY }}>
                    Tiện nghi nổi bật
                  </h3>
                  <BulletList items={content.interiorBullets} />
                </div>
              </div>
            </>
          )}
        </div>
      </section>

      {/* An toàn (Cập nhật Tag thành 05) */}
      <section id="car-an-toan" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          {content.safetySplit ? (
            <DetailGallerySplitBlock
              tag="05"
              title="An toàn"
              slides={content.safetySplit.slides}
              fallbackSingleSrc={imageSrc}
              fallbackAlt={`${name} — an toàn`}
            />
          ) : (
            <>
              <SectionHeading tag="05" title="An toàn" intro={content.safetyIntro} />
              <div className="max-w-4xl">
                <BulletList items={content.safetyBullets} />
                <p className="mt-4 text-xs text-slate-500">Trang bị có thể thay đổi theo phiên bản và thời điểm mở bán.</p>
              </div>
            </>
          )}
        </div>
      </section>

      {/* Vận hành (Cập nhật Tag thành 06) */}
      <section id="car-van-hanh" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          {content.performanceSplit ? (
            <DetailGallerySplitBlock
              tag="06"
              title="Vận hành"
              slides={content.performanceSplit.slides}
              fallbackSingleSrc={imageSrc}
              fallbackAlt={`${name} — vận hành`}
            />
          ) : (
            <>
              <SectionHeading tag="06" title="Vận hành" intro={content.performanceIntro} />
              <div className="max-w-4xl">
                <BulletList items={content.performanceBullets} />
              </div>
            </>
          )}
        </div>
      </section>

      {/* Thông số (Cập nhật Tag thành 07) */}
      <section id="car-thong-so" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="07"
            title={content.specsTitle ?? 'Thông số kỹ thuật'}
            intro="Bảng thông số tham khảo theo catalogue; chi tiết chính xác theo phiên bản và thời điểm giao xe."
          />
          <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-slate-100/90">
                  <th className="px-6 py-3 text-left text-xs font-bold uppercase tracking-wider text-slate-700 w-[22%]">Danh mục</th>
                  <th className="px-6 py-3 text-left text-xs font-bold uppercase tracking-wider text-slate-700 w-[38%]">Thông số</th>
                  <th className="px-6 py-3 text-left text-xs font-bold uppercase tracking-wider text-slate-700">Giá trị</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200/90">
                {(() => {
                  const rows: React.ReactNode[] = []
                  let i = 0
                  while (i < content.specsRows.length) {
                    const category = (content.specsRows[i].category ?? '').trim()
                    let count = 1
                    while (
                      i + count < content.specsRows.length &&
                      (content.specsRows[i + count].category ?? '').trim() === category
                    ) count++

                    for (let j = 0; j < count; j++) {
                      const row = content.specsRows[i + j]
                      const isFirst = j === 0
                      const isLast = j === count - 1
                      rows.push(
                        <tr
                          key={`${category}-${row.label}-${i + j}`}
                          className={isLast && i + count < content.specsRows.length ? '' : ''}
                        >
                          {isFirst ? (
                            <td
                              rowSpan={count}
                              className="px-6 py-3 text-sm font-semibold text-slate-700 bg-slate-50 border-r border-slate-200 align-top"
                            >
                              {category || '—'}
                            </td>
                          ) : null}
                          <td className="px-6 py-3 text-sm font-semibold text-slate-800">{row.label}</td>
                          <td className="px-6 py-3 text-sm text-slate-700">{row.value}</td>
                        </tr>
                      )
                    }
                    i += count
                  }
                  return rows
                })()}
              </tbody>
            </table>

            <p className="border-t border-slate-200/90 bg-slate-50 px-4 py-3 text-xs text-slate-500 sm:px-6">
              Thông số tham khảo; số liệu chính thức theo catalogue và hợp đồng mua bán tại thời điểm giao xe.
            </p>
          </div>
        </div>
      </section>
    </div>
  )
}