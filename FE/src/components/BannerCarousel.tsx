import { ChevronLeft, ChevronRight } from 'lucide-react'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

export type BannerSlide = {
  src: string
  alt: string
  href?: string
  /** Caption title (optional) */
  title?: string | null
  /** Caption description (optional) */
  description?: string | null
}

export type BannerCarouselProps = {
  slides: BannerSlide[]
  intervalMs?: number
  className?: string
  /** Đồng bộ với hàng thumbnail / state bên ngoài (cả hai cùng lúc = controlled). */
  activeIndex?: number
  onActiveIndexChange?: (index: number) => void
  /** Ẩn dải chấm dưới cùng (vd. khi đã có thumbnail bên dưới). */
  hideDots?: boolean
}

export function BannerCarousel({
  slides,
  intervalMs = 4000,
  className,
  activeIndex: controlledIndex,
  onActiveIndexChange,
  hideDots = false,
}: BannerCarouselProps) {
  const [internalIndex, setInternalIndex] = useState(0)
  const [isPaused, setIsPaused] = useState(false)

  const isControlled =
    controlledIndex !== undefined && onActiveIndexChange !== undefined

  const safeSlides = useMemo(() => slides.filter(Boolean), [slides])
  const count = safeSlides.length

  const indexRef = useRef(0)

  const normalizedIndex = useMemo(() => {
    const raw = isControlled ? controlledIndex! : internalIndex
    if (count === 0) return 0
    return ((raw % count) + count) % count
  }, [controlledIndex, internalIndex, isControlled, count])

  indexRef.current = normalizedIndex

  const setSlideIndex = useCallback(
    (next: number) => {
      if (count === 0) return
      const n = ((next % count) + count) % count
      if (isControlled) {
        onActiveIndexChange!(n)
      } else {
        setInternalIndex(n)
      }
    },
    [count, isControlled, onActiveIndexChange],
  )

  useEffect(() => {
    if (count <= 1) return
    if (isPaused) return
    if (!Number.isFinite(intervalMs) || intervalMs <= 500) return

    const t = window.setInterval(() => {
      const cur = indexRef.current
      const next = (cur + 1) % count
      if (isControlled) {
        onActiveIndexChange!(next)
      } else {
        setInternalIndex(next)
      }
    }, intervalMs)

    return () => window.clearInterval(t)
  }, [count, intervalMs, isPaused, isControlled, onActiveIndexChange])

  if (count === 0) return null

  const goPrev = () => setSlideIndex(indexRef.current - 1)
  const goNext = () => setSlideIndex(indexRef.current + 1)

  return (
    <section
      className={className}
      aria-roledescription="carousel"
      aria-label="Banner"
      onMouseEnter={() => setIsPaused(true)}
      onMouseLeave={() => setIsPaused(false)}
    >
      <div className="relative overflow-hidden bg-white/5">
        <div className="relative aspect-[16/9] w-full md:aspect-[21/9]">
          {safeSlides.map((slide, idx) => {
            const isActive = idx === normalizedIndex
            const content = (
              <img
                src={slide.src}
                alt={slide.alt}
                className={[
                  'absolute inset-0 h-full w-full object-cover',
                  'transition-opacity duration-500 ease-out',
                  isActive ? 'opacity-100' : 'opacity-0',
                ].join(' ')}
                loading={idx === 0 ? 'eager' : 'lazy'}
                decoding="async"
                draggable={false}
              />
            )

            return slide.href ? (
              <a
                key={slide.src + idx}
                href={slide.href}
                className={isActive ? 'pointer-events-auto' : 'pointer-events-none'}
                aria-hidden={!isActive}
                tabIndex={isActive ? 0 : -1}
              >
                {content}
              </a>
            ) : (
              <div
                key={slide.src + idx}
                aria-hidden={!isActive}
                className={isActive ? 'pointer-events-auto' : 'pointer-events-none'}
              >
                {content}
              </div>
            )
          })}
        </div>

        {count > 1 && (
          <>
            <button
              type="button"
              onClick={goPrev}
              className="absolute left-3 top-1/2 -translate-y-1/2 rounded-full bg-black/35 p-2 text-white backdrop-blur transition hover:bg-black/50 focus:outline-none focus-visible:ring-2 focus-visible:ring-white/80"
              aria-label="Ảnh trước"
            >
              <ChevronLeft className="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              type="button"
              onClick={goNext}
              className="absolute right-3 top-1/2 -translate-y-1/2 rounded-full bg-black/35 p-2 text-white backdrop-blur transition hover:bg-black/50 focus:outline-none focus-visible:ring-2 focus-visible:ring-white/80"
              aria-label="Ảnh tiếp theo"
            >
              <ChevronRight className="h-5 w-5" aria-hidden="true" />
            </button>

            {!hideDots && (
              <div className="absolute bottom-3 left-1/2 flex -translate-x-1/2 items-center gap-2">
                {safeSlides.map((_, idx) => {
                  const isActive = idx === normalizedIndex
                  return (
                    <button
                      key={idx}
                      type="button"
                      onClick={() => setSlideIndex(idx)}
                      className={[
                        'h-2.5 w-2.5 rounded-full transition',
                        isActive ? 'bg-white' : 'bg-white/45 hover:bg-white/70',
                      ].join(' ')}
                      aria-label={`Chuyển đến ảnh ${idx + 1}`}
                      aria-current={isActive ? 'true' : undefined}
                    />
                  )
                })}
              </div>
            )}
          </>
        )}
      </div>
    </section>
  )
}

