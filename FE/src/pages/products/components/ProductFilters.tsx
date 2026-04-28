import { useMemo, useState } from 'react'

export type FuelType = 'xang' | 'dau' | 'dien' | 'hybrid'
export type CarCondition = 'moi' | 'cu'
export type Transmission = 'so-tu-dong' | 'so-san'
export type BodyType = 'sedan' | 'suv' | 'hatchback' | 'pickup' | 'mpv'

export type SortKey = 'price_desc' | 'price_asc' | 'year_desc' | 'year_asc'

export type ProductFilterState = {
  searchText: string 
  fuelType: FuelType | ''
  condition: CarCondition | ''
  brand: string
  model: string
  yearMin: number | ''
  yearMax: number | ''
  priceMinMillions: number | ''
  priceMaxMillions: number | ''
  transmission: Transmission | ''
  bodyType: BodyType | ''
  seats: number | ''
  sort: SortKey
}

export type ProductFilterOptions = {
  brands: string[]
  models: string[]
  years: number[]
  seats: number[]
  priceRangeMillions?: { min: number; max: number }
}

export type ProductFiltersProps = {
  value: ProductFilterState
  onChange: (next: ProductFilterState) => void
  options: ProductFilterOptions
  resultCount: number
  totalCount: number
  carNames?: string[] 
  /** Tiêu đề trang — nằm trên nền gradient, phía trên card trắng */
  headingTitle?: string
  headingDescription?: string
}

const fuelTypeOptions: { value: FuelType; label: string }[] = [
  { value: 'xang', label: 'Xăng' },
  { value: 'dau', label: 'Dầu' },
  { value: 'dien', label: 'Điện' },
  { value: 'hybrid', label: 'Hybrid' },
]

const conditionOptions: { value: CarCondition; label: string }[] = [
  { value: 'moi', label: 'Mới' },
  { value: 'cu', label: 'Cũ' },
]

const transmissionOptions: { value: Transmission; label: string }[] = [
  { value: 'so-tu-dong', label: 'Số tự động' },
  { value: 'so-san', label: 'Số sàn' },
]

const bodyTypeOptions: { value: BodyType; label: string }[] = [
  { value: 'sedan', label: 'Sedan' },
  { value: 'suv', label: 'SUV' },
  { value: 'hatchback', label: 'Hatchback' },
  { value: 'pickup', label: 'Pickup' },
  { value: 'mpv', label: 'MPV' },
]

const sortOptions: { value: SortKey; label: string }[] = [
  { value: 'price_desc', label: 'Giá: cao → thấp' },
  { value: 'price_asc', label: 'Giá: thấp → cao' },
  { value: 'year_desc', label: 'Năm SX: mới → cũ' },
  { value: 'year_asc', label: 'Năm SX: cũ → mới' },
]

function Label({ children }: { children: React.ReactNode }) {
  return <div className="text-xs font-semibold text-slate-700">{children}</div>
}

function Select({
  value,
  onChange,
  children,
  className = '',
}: {
  value: string | number
  onChange: (next: string) => void
  children: React.ReactNode
  className?: string
}) {
  return (
    <select
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className={[
        'h-9 w-full min-w-0 rounded-lg border border-slate-200/90 bg-white px-3 text-sm text-slate-900 shadow-sm outline-none transition',
        'hover:border-slate-300 focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-500/30',
        className,
      ].join(' ')}
    >
      {children}
    </select>
  )
}

function Range({
  min,
  max,
  step = 1,
  value,
  onChange,
  disabled,
  fillColor = '#0ea5e9',
}: {
  min: number
  max: number
  step?: number
  value: number
  onChange: (next: number) => void
  disabled?: boolean
  fillColor?: string
}) {
  const pct = (() => {
    if (max <= min) return 0
    const clamped = Math.min(max, Math.max(min, value))
    return ((clamped - min) / (max - min)) * 100
  })()

  return (
    <input
      type="range"
      min={min}
      max={max}
      step={step}
      value={value}
      disabled={disabled}
      onChange={(e) => {
        const n = Number(e.target.value)
        onChange(Number.isFinite(n) ? n : min)
      }}
      style={{
        background: `linear-gradient(to right, ${fillColor} 0%, ${fillColor} ${pct}%, rgb(226 232 240) ${pct}%, rgb(226 232 240) 100%)`,
      }}
      className={[
        'h-2 w-full cursor-pointer appearance-none rounded-full',
        'disabled:cursor-not-allowed disabled:opacity-60',
        '[&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:rounded-full',
        '[&::-webkit-slider-thumb]:bg-rose-600 [&::-webkit-slider-thumb]:shadow-sm [&::-webkit-slider-thumb]:ring-2 [&::-webkit-slider-thumb]:ring-white',
        '[&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4 [&::-moz-range-thumb]:rounded-full [&::-moz-range-thumb]:border-2 [&::-moz-range-thumb]:border-white [&::-moz-range-thumb]:bg-rose-600',
      ].join(' ')}
    />
  )
}

function DualThumbRange({
  min,
  max,
  step = 1,
  valueMin,
  valueMax,
  onChange,
  disabled,
  fillColor = '#0ea5e9',
}: {
  min: number
  max: number
  step?: number
  valueMin: number
  valueMax: number
  onChange: (next: { min: number; max: number }) => void
  disabled?: boolean
  fillColor?: string
}) {
  const [activeThumb, setActiveThumb] = useState<'min' | 'max'>('min')
  const safeMin = Math.min(valueMin, valueMax)
  const safeMax = Math.max(valueMin, valueMax)
  const pct = (v: number) => {
    if (max <= min) return 0
    const clamped = Math.min(max, Math.max(min, v))
    return ((clamped - min) / (max - min)) * 100
  }
  const leftPct = pct(safeMin)
  const rightPct = pct(safeMax)

  const sliderCls = [
    'absolute inset-0 h-5 w-full appearance-none bg-transparent',
    'disabled:cursor-not-allowed disabled:opacity-60',
    '[&::-webkit-slider-runnable-track]:h-2 [&::-webkit-slider-runnable-track]:bg-transparent',
    // Center thumb vertically on 8px track (16px thumb => -4px offset)
    '[&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:mt-[-4px] [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:rounded-full',
    '[&::-webkit-slider-thumb]:bg-rose-600 [&::-webkit-slider-thumb]:shadow-sm [&::-webkit-slider-thumb]:ring-2 [&::-webkit-slider-thumb]:ring-white',
    '[&::-moz-range-track]:h-2 [&::-moz-range-track]:bg-transparent',
    '[&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4 [&::-moz-range-thumb]:rounded-full [&::-moz-range-thumb]:border-2 [&::-moz-range-thumb]:border-white [&::-moz-range-thumb]:bg-rose-600',
  ].join(' ')

  return (
    <div className="relative h-5 w-full">
      <div className="absolute left-0 right-0 top-1/2 h-2 -translate-y-1/2 rounded-full bg-slate-200" />
      <div
        className="absolute top-1/2 h-2 -translate-y-1/2 rounded-full opacity-80"
        style={{
          left: `${leftPct}%`,
          width: `${Math.max(0, rightPct - leftPct)}%`,
          backgroundColor: fillColor,
        }}
      />

      <input
        type="range"
        min={min}
        max={max}
        step={step}
        value={safeMin}
        disabled={disabled}
        onPointerDown={() => setActiveThumb('min')}
        onChange={(e) => {
          const n = Number(e.target.value)
          const nextMin = Number.isFinite(n) ? n : min
          onChange({ min: Math.min(nextMin, safeMax), max: safeMax })
        }}
        className={sliderCls}
        style={{ zIndex: activeThumb === 'min' ? 20 : 10 }}
      />
      <input
        type="range"
        min={min}
        max={max}
        step={step}
        value={safeMax}
        disabled={disabled}
        onPointerDown={() => setActiveThumb('max')}
        onChange={(e) => {
          const n = Number(e.target.value)
          const nextMax = Number.isFinite(n) ? n : max
          onChange({ min: safeMin, max: Math.max(nextMax, safeMin) })
        }}
        className={sliderCls}
        style={{ zIndex: activeThumb === 'max' ? 20 : 10 }}
      />
    </div>
  )
}

function uniqSortedStrings(xs: string[]) {
  return Array.from(new Set(xs.filter(Boolean))).sort((a, b) => a.localeCompare(b, 'vi'))
}

function Pill({
  active,
  onClick,
  children,
  size = 'md',
}: {
  active?: boolean
  onClick?: () => void
  children: React.ReactNode
  size?: 'sm' | 'md'
}) {
  const sizeCls = size === 'sm' ? 'h-8 px-3 text-xs' : 'h-9 px-4 text-sm'
  return (
    <button
      type="button"
      onClick={onClick}
      className={[
        'inline-flex items-center justify-center rounded-full font-semibold transition',
        sizeCls,
        'shadow-sm ring-1',
        active
          ? 'bg-rose-600 text-white ring-rose-600 hover:bg-rose-700'
          : 'bg-slate-50 text-slate-700 ring-slate-200/90 hover:bg-white hover:ring-slate-300',
        'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40',
      ].join(' ')}
    >
      {children}
    </button>
  )
}

function Chip({
  label,
  onRemove,
}: {
  label: string
  onRemove?: () => void
}) {
  return (
    <span className="inline-flex items-center gap-2 rounded-full bg-slate-50 px-3 py-1 text-xs font-semibold text-slate-700 ring-1 ring-slate-200">
      <span className="max-w-[220px] truncate">{label}</span>
      {onRemove ? (
        <button
          type="button"
          onClick={onRemove}
          className="grid h-5 w-5 place-items-center rounded-full bg-white text-slate-600 ring-1 ring-slate-200 transition hover:bg-slate-50 hover:text-slate-900 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40"
          aria-label={`Bỏ lọc: ${label}`}
        >
          ×
        </button>
      ) : null}
    </span>
  )
}

function IconButton({
  label,
  title,
  onClick,
  pressed,
  children,
}: {
  label: string
  title?: string
  onClick: () => void
  pressed?: boolean
  children: React.ReactNode
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      aria-label={label}
      title={title ?? label}
      aria-pressed={pressed}
      className={[
        'inline-flex h-9 w-9 items-center justify-center rounded-lg text-slate-600 transition',
        'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40',
        pressed
          ? 'bg-white text-rose-600 shadow-sm ring-1 ring-slate-200/90'
          : 'hover:bg-white/90 hover:text-slate-900',
      ].join(' ')}
    >
      <span className="grid h-5 w-5 place-items-center" aria-hidden="true">
        {children}
      </span>
    </button>
  )
}

function IconTune({ active }: { active?: boolean }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" className="h-5 w-5">
      <path
        d="M4 6h10M18 6h2M8 6v0"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <path
        d="M4 12h6M14 12h6"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        opacity="0.9"
      />
      <path
        d="M4 18h14M20 18h0"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        opacity="0.8"
      />
      <circle
        cx="16"
        cy="6"
        r="2"
        stroke="currentColor"
        strokeWidth="2"
        fill={active ? 'currentColor' : 'none'}
        opacity={active ? 1 : 0.95}
      />
      <circle cx="10" cy="12" r="2" stroke="currentColor" strokeWidth="2" fill="none" opacity="0.95" />
      <circle cx="18" cy="18" r="2" stroke="currentColor" strokeWidth="2" fill="none" opacity="0.95" />
    </svg>
  )
}

function IconEraser() {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" className="h-5 w-5">
      <path
        d="M4.5 16.5 14.9 6.1a2.4 2.4 0 0 1 3.4 0l1.6 1.6a2.4 2.4 0 0 1 0 3.4L12.9 19.1"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M12.9 19.1H7.8a2 2 0 0 1-1.4-.6l-1.9-1.9a2 2 0 0 1 0-2.8"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        opacity="0.9"
      />
      <path d="M13 19h7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" opacity="0.7" />
    </svg>
  )
}

export function ProductFilters({
  value,
  onChange,
  options,
  resultCount,
  totalCount,
  carNames = [],
  headingTitle,
  headingDescription,
}: ProductFiltersProps) {
  const [showAll, setShowAll] = useState(false)

  const [searchInput, setSearchInput] = useState(value.searchText)
  const [showSuggestions, setShowSuggestions] = useState(false)

  const [activeIndex, setActiveIndex] = useState(-1)

  const suggestions = useMemo(() => {
    const q = searchInput.trim().toLowerCase()
    if (!q || q.length < 1) return []
    return carNames
      .filter((n) => n.toLowerCase().includes(q))
      .slice(0, 8)
  }, [carNames, searchInput])

  const brands = useMemo(() => uniqSortedStrings(options.brands), [options.brands])
  const models = useMemo(() => uniqSortedStrings(options.models), [options.models])
  const years = useMemo(() => Array.from(new Set(options.years)).sort((a, b) => b - a), [options.years])
  const yearBounds = useMemo(() => {
    if (!years.length) return { min: 0, max: 0, has: false }
    const min = Math.min(...years)
    const max = Math.max(...years)
    return { min, max, has: true }
  }, [years])
  const seatsBounds = { min: 0, max: 32, has: true as const }
  const priceBounds = useMemo(() => {
    const r = options.priceRangeMillions
    if (!r) return { min: 0, max: 0, has: false }
    const min = Math.max(0, Math.floor(r.min))
    const max = Math.max(min, Math.ceil(r.max))
    return { min, max, has: max > 0 }
  }, [options.priceRangeMillions])

  const clear = () => {
    onChange({
      searchText: '', 
      fuelType: '',
      condition: '',
      brand: '',
      model: '',
      yearMin: '',
      yearMax: '',
      priceMinMillions: '',
      priceMaxMillions: '',
      transmission: '',
      bodyType: '',
      seats: '',
      sort: 'price_desc',
    })
  }

  const set = (patch: Partial<ProductFilterState>) => onChange({ ...value, ...patch })

  const activeChips = (() => {
    const chips: { key: string; label: string; onRemove: () => void }[] = []
    if (value.fuelType) {
      const label = fuelTypeOptions.find((x) => x.value === value.fuelType)?.label ?? value.fuelType
      chips.push({ key: 'fuelType', label: `Nhiên liệu: ${label}`, onRemove: () => set({ fuelType: '' }) })
    }
    if (value.condition) {
      const label = conditionOptions.find((x) => x.value === value.condition)?.label ?? value.condition
      chips.push({ key: 'condition', label: `Tình trạng: ${label}`, onRemove: () => set({ condition: '' }) })
    }
    if (value.brand) chips.push({ key: 'brand', label: `Hãng: ${value.brand}`, onRemove: () => set({ brand: '', model: '' }) })
    if (value.model) chips.push({ key: 'model', label: `Model: ${value.model}`, onRemove: () => set({ model: '' }) })
    if (value.yearMin !== '' || value.yearMax !== '') {
      const label =
        value.yearMin !== '' && value.yearMax !== ''
          ? `${value.yearMin}–${value.yearMax}`
          : value.yearMin !== ''
            ? `Từ ${value.yearMin}`
            : `Đến ${value.yearMax}`
      chips.push({ key: 'year', label: `Năm SX: ${label}`, onRemove: () => set({ yearMin: '', yearMax: '' }) })
    }
    if (value.priceMinMillions !== '' || value.priceMaxMillions !== '') {
      const label =
        value.priceMinMillions !== '' && value.priceMaxMillions !== ''
          ? `${value.priceMinMillions}–${value.priceMaxMillions} triệu`
          : value.priceMinMillions !== ''
            ? `Từ ${value.priceMinMillions} triệu`
            : `Đến ${value.priceMaxMillions} triệu`
      chips.push({
        key: 'price',
        label: `Giá: ${label}`,
        onRemove: () => set({ priceMinMillions: '', priceMaxMillions: '' }),
      })
    }
    if (value.transmission) {
      const label = transmissionOptions.find((x) => x.value === value.transmission)?.label ?? value.transmission
      chips.push({ key: 'transmission', label: `Hộp số: ${label}`, onRemove: () => set({ transmission: '' }) })
    }
    if (value.bodyType) {
      const label = bodyTypeOptions.find((x) => x.value === value.bodyType)?.label ?? value.bodyType
      chips.push({ key: 'bodyType', label: `Kiểu dáng: ${label}`, onRemove: () => set({ bodyType: '' }) })
    }
    if (value.seats !== '') {
      chips.push({ key: 'seats', label: `Số chỗ: ${value.seats}`, onRemove: () => set({ seats: '' }) })
    }
    return chips
  })()

  return (
    <section className="w-full bg-gradient-to-b from-white to-[#e8e8e8] pb-6 pt-8 sm:pb-8 sm:pt-10">
      <div className="mx-auto w-full max-w-screen-2xl px-6">
        {headingTitle ? (
          <div className="mb-12 max-w-3xl sm:mb-14">
            <h1 className="text-3xl font-semibold tracking-tight text-slate-900 sm:text-4xl">{headingTitle}</h1>
            {headingDescription ? (
              <p className="mt-2 max-w-2xl text-sm leading-relaxed text-slate-600 sm:text-base">
                {headingDescription}
              </p>
            ) : null}
          </div>
        ) : null}

        <div className="overflow-hidden rounded-[20px] border border-slate-200/80 bg-white shadow-[0_12px_40px_rgba(15,23,42,0.07)] ring-1 ring-slate-900/[0.04]">
          <div className="px-4 py-4 sm:px-6 sm:py-5">
            <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 pb-4">
              <h2 className="text-base font-semibold tracking-tight text-slate-900">Bộ lọc</h2>
              <div className="inline-flex items-center gap-2 rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600 ring-1 ring-slate-200/80">
                <span className="tabular-nums text-slate-900">{resultCount}</span>
                <span className="text-slate-300">/</span>
                <span className="tabular-nums">{totalCount}</span>
                <span className="hidden sm:inline">sản phẩm</span>
              </div>
            </div>

            {/* ── Tìm kiếm theo tên xe ── */}
            <div className="mt-4">
              <div className="relative flex gap-2">
                <div className="relative flex-1">
                  <svg
                    className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"
                    viewBox="0 0 24 24" fill="none" aria-hidden
                  >
                    <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="2" />
                    <path d="M16.5 16.5l4 4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                  </svg>
                  <input
                    type="text"
                    placeholder="Tìm theo tên xe... (Enter để tìm)"
                    value={searchInput}
                    onChange={(e) => {
                      setSearchInput(e.target.value)
                      setShowSuggestions(true)
                      if (e.target.value === '') set({ searchText: '' })
                    }}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        set({ searchText: searchInput })
                        setShowSuggestions(false)
                      }
                      if (e.key === 'Escape') setShowSuggestions(false)
                    }}
                    onFocus={() => { if (searchInput) setShowSuggestions(true) }}
                    onBlur={() => setTimeout(() => setShowSuggestions(false), 150)}
                    className="h-10 w-full rounded-lg border border-slate-200 bg-white pl-9 pr-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40"
                  />

                  {/* Dropdown gợi ý */}
                  {showSuggestions && suggestions.length > 0 && (
                    <div className="absolute z-30 mt-1 w-full overflow-hidden rounded-lg border border-slate-200 bg-white shadow-lg">
                      {suggestions.map((name, idx) => (
                          <button
                            key={name}
                            type="button"
                            onMouseDown={() => {
                              setSearchInput(name)
                              set({ searchText: name })
                              setShowSuggestions(false)
                              setActiveIndex(-1)
                            }}
                            onMouseEnter={() => setActiveIndex(idx)}
                            className={[
                              'flex w-full items-center gap-2 px-4 py-2.5 text-left text-sm',
                              idx === activeIndex ? 'bg-rose-50 text-rose-600' : 'text-slate-700 hover:bg-slate-50',
                            ].join(' ')}
                          >
                          <svg className="h-3.5 w-3.5 shrink-0 text-slate-400" viewBox="0 0 24 24" fill="none">
                            <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="2" />
                            <path d="M16.5 16.5l4 4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                          </svg>
                          <span className="truncate">{name}</span>
                        </button>
                      ))}
                    </div>
                  )}
                </div>

                <button
                  type="button"
                  onClick={() => {
                    set({ searchText: searchInput })
                    setShowSuggestions(false)
                  }}
                  className="inline-flex h-10 items-center justify-center rounded-lg bg-rose-500 px-4 text-sm font-semibold text-white shadow-sm transition hover:bg-rose-600 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40"
                >
                  Tìm
                </button>
              </div>
            </div>


            {/* Nhãn cùng hàng với control — gọn chiều cao; xl: một dải + cụm phải cố định */}
            <div className="mt-4 flex flex-col gap-4 xl:flex-row xl:items-center xl:justify-between xl:gap-6">
              <div className="flex min-w-0 flex-1 flex-col gap-3 lg:flex-row lg:flex-wrap lg:items-center lg:gap-x-5 lg:gap-y-3">
                <div className="flex min-w-0 flex-wrap items-center gap-2 sm:gap-3">
                  <span className="shrink-0 text-[11px] font-bold uppercase tracking-wide text-slate-400">
                    Nhiên liệu
                  </span>
                  <div className="flex min-w-0 flex-wrap gap-1.5">
                    <Pill size="sm" active={value.fuelType === ''} onClick={() => set({ fuelType: '' })}>
                      Tất cả
                    </Pill>
                    {fuelTypeOptions.map((o) => (
                      <Pill
                        key={o.value}
                        size="sm"
                        active={value.fuelType === o.value}
                        onClick={() => set({ fuelType: value.fuelType === o.value ? '' : o.value })}
                      >
                        {o.label}
                      </Pill>
                    ))}
                  </div>
                </div>

                <div className="hidden h-7 w-px shrink-0 bg-slate-200 lg:block" aria-hidden="true" />

                <div className="flex min-w-0 flex-wrap items-center gap-2 sm:gap-3">
                  <span className="shrink-0 text-[11px] font-bold uppercase tracking-wide text-slate-400">
                    Tình trạng
                  </span>
                  <div className="flex flex-wrap gap-1.5">
                    <Pill size="sm" active={value.condition === ''} onClick={() => set({ condition: '' })}>
                      Tất cả
                    </Pill>
                    {conditionOptions.map((o) => (
                      <Pill
                        key={o.value}
                        size="sm"
                        active={value.condition === o.value}
                        onClick={() => set({ condition: value.condition === o.value ? '' : o.value })}
                      >
                        {o.label}
                      </Pill>
                    ))}
                  </div>
                </div>
              </div>

              <div className="flex w-full min-w-0 shrink-0 flex-col gap-3 sm:flex-row sm:items-center sm:justify-end sm:gap-3 xl:w-auto xl:justify-end">
                <div className="flex min-w-0 flex-1 items-center gap-2 sm:min-w-[220px] sm:max-w-[280px] sm:flex-initial">
                  <span className="shrink-0 text-[11px] font-bold uppercase tracking-wide text-slate-400">
                    Sắp xếp
                  </span>
                  <Select
                    className="min-w-0 flex-1 sm:min-w-[200px]"
                    value={value.sort}
                    onChange={(v) => set({ sort: v as SortKey })}
                  >
                    {sortOptions.map((o) => (
                      <option key={o.value} value={o.value}>
                        {o.label}
                      </option>
                    ))}
                  </Select>
                </div>

                <div
                  className="flex shrink-0 justify-end gap-0.5 rounded-xl bg-slate-100/90 p-1 ring-1 ring-slate-200/80"
                  role="group"
                  aria-label="Thao tác lọc"
                >
                  <IconButton
                    label={showAll ? 'Đóng lọc nâng cao' : 'Mở lọc nâng cao'}
                    title={showAll ? 'Đóng nâng cao' : 'Lọc nâng cao'}
                    pressed={showAll}
                    onClick={() => setShowAll((v) => !v)}
                  >
                    <IconTune active={showAll} />
                  </IconButton>
                  <IconButton label="Xoá lọc" onClick={clear}>
                    <IconEraser />
                  </IconButton>
                </div>
              </div>
            </div>

            <div className="mt-4 rounded-xl bg-slate-50/90 px-3 py-2.5 ring-1 ring-slate-100 sm:px-4">
              {activeChips.length ? (
                <div className="flex flex-wrap items-center gap-2">
                  {activeChips.map((c) => (
                    <Chip key={c.key} label={c.label} onRemove={c.onRemove} />
                  ))}
                </div>
              ) : (
                <p className="text-xs leading-relaxed text-slate-500">
                  Chọn bộ lọc để thu hẹp danh sách sản phẩm.
                </p>
              )}
            </div>
          </div>

          {showAll ? (
            <div className="border-t border-slate-100 bg-gradient-to-b from-slate-50/80 to-slate-100/50 px-4 py-5 sm:px-6 sm:py-6">
              <p className="mb-4 text-[11px] font-bold uppercase tracking-wide text-slate-400">Lọc nâng cao</p>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
                {/* Row 1 */}
                <div className="flex flex-col gap-2 lg:order-1">
                  <Label>Hãng xe</Label>
                  <Select
                    value={value.brand}
                    onChange={(v) => {
                      if (v === value.brand) return
                      set({ brand: v, model: '' })
                    }}
                  >
                    <option value="">Tất cả</option>
                    {brands.map((b) => (
                      <option key={b} value={b}>
                        {b}
                      </option>
                    ))}
                  </Select>
                </div>

                <div className="flex flex-col gap-2 lg:order-2">
                  <Label>Dòng xe / Model</Label>
                  <Select value={value.model} onChange={(v) => set({ model: v })}>
                    <option value="">Tất cả</option>
                    {models.map((m) => (
                      <option key={m} value={m}>
                        {m}
                      </option>
                    ))}
                  </Select>
                </div>

                <div className="flex flex-col gap-2 lg:order-3">
                  <Label>Hộp số</Label>
                  <Select
                    value={value.transmission}
                    onChange={(v) => set({ transmission: v as Transmission | '' })}
                  >
                    <option value="">Tất cả</option>
                    {transmissionOptions.map((o) => (
                      <option key={o.value} value={o.value}>
                        {o.label}
                      </option>
                    ))}
                  </Select>
                </div>

                <div className="flex flex-col gap-2 lg:order-4">
                  <Label>Kiểu dáng</Label>
                  <Select value={value.bodyType} onChange={(v) => set({ bodyType: v as BodyType | '' })}>
                    <option value="">Tất cả</option>
                    {bodyTypeOptions.map((o) => (
                      <option key={o.value} value={o.value}>
                        {o.label}
                      </option>
                    ))}
                  </Select>
                </div>

                {/* Row 2 */}
                <div className="flex flex-col gap-2 lg:order-5">
                  <Label>Khoảng giá (triệu)</Label>
                  {priceBounds.has ? (
                    <div className="rounded-xl bg-white/70 p-3 ring-1 ring-slate-200/80">
                      <div className="flex items-center justify-between gap-3 text-xs font-semibold text-slate-700">
                        <span>
                          {value.priceMinMillions === '' ? priceBounds.min : value.priceMinMillions} –{' '}
                          {value.priceMaxMillions === '' ? priceBounds.max : value.priceMaxMillions} triệu
                        </span>
                        <button
                          type="button"
                          onClick={() => set({ priceMinMillions: '', priceMaxMillions: '' })}
                          className="text-slate-500 hover:text-slate-900"
                        >
                          Reset
                        </button>
                      </div>
                      <div className="mt-3">
                        <DualThumbRange
                          min={priceBounds.min}
                          max={priceBounds.max}
                          step={10}
                          valueMin={value.priceMinMillions === '' ? priceBounds.min : value.priceMinMillions}
                          valueMax={value.priceMaxMillions === '' ? priceBounds.max : value.priceMaxMillions}
                          onChange={({ min, max }) => {
                            const normalizedMin = min === priceBounds.min && max === priceBounds.max ? '' : min
                            const normalizedMax = min === priceBounds.min && max === priceBounds.max ? '' : max
                            set({ priceMinMillions: normalizedMin, priceMaxMillions: normalizedMax })
                          }}
                        />
                        <div className="mt-1 flex items-center justify-between text-[11px] font-semibold text-slate-400">
                          <span>{priceBounds.min} triệu</span>
                          <span>{priceBounds.max} triệu</span>
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className="rounded-xl bg-white/70 p-3 text-xs text-slate-500 ring-1 ring-slate-200/80">
                      Chưa có dữ liệu giá để tạo thanh kéo.
                    </div>
                  )}
                </div>

                <div className="flex flex-col gap-2 lg:order-6">
                  <Label>Năm sản xuất</Label>
                  {yearBounds.has ? (
                    <div className="rounded-xl bg-white/70 p-3 ring-1 ring-slate-200/80">
                      <div className="flex items-center justify-between gap-3 text-xs font-semibold text-slate-700">
                        <span>
                          {value.yearMin === '' ? yearBounds.min : value.yearMin} –{' '}
                          {value.yearMax === '' ? yearBounds.max : value.yearMax}
                        </span>
                        <button
                          type="button"
                          onClick={() => set({ yearMin: '', yearMax: '' })}
                          className="text-slate-500 hover:text-slate-900"
                        >
                          Reset
                        </button>
                      </div>
                      <div className="mt-3">
                        <DualThumbRange
                          min={yearBounds.min}
                          max={yearBounds.max}
                          step={1}
                          valueMin={value.yearMin === '' ? yearBounds.min : value.yearMin}
                          valueMax={value.yearMax === '' ? yearBounds.max : value.yearMax}
                          onChange={({ min, max }) => {
                            const normalizedMin = min === yearBounds.min && max === yearBounds.max ? '' : min
                            const normalizedMax = min === yearBounds.min && max === yearBounds.max ? '' : max
                            set({ yearMin: normalizedMin, yearMax: normalizedMax })
                          }}
                        />
                        <div className="mt-1 flex items-center justify-between text-[11px] font-semibold text-slate-400">
                          <span>{yearBounds.min}</span>
                          <span>{yearBounds.max}</span>
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className="rounded-xl bg-white/70 p-3 text-xs text-slate-500 ring-1 ring-slate-200/80">
                      Chưa có dữ liệu năm sản xuất để tạo thanh kéo.
                    </div>
                  )}
                </div>

                <div className="flex flex-col gap-2 lg:order-7">
                  <Label>Số chỗ ngồi</Label>
                  <div className="rounded-xl bg-white/70 p-3 ring-1 ring-slate-200/80">
                    <div className="flex items-center justify-between gap-3 text-xs font-semibold text-slate-700">
                      <span>{value.seats === '' ? 'Tất cả' : `${value.seats} chỗ`}</span>
                      <button
                        type="button"
                        onClick={() => set({ seats: '' })}
                        className="text-slate-500 hover:text-slate-900"
                      >
                        Reset
                      </button>
                    </div>
                    <div className="mt-3">
                      <Range
                        min={seatsBounds.min}
                        max={seatsBounds.max}
                        step={1}
                        value={value.seats === '' ? seatsBounds.min : value.seats}
                        onChange={(n) => set({ seats: n })}
                      />
                      <div className="mt-1 flex items-center justify-between text-[11px] font-semibold text-slate-400">
                        <span>{seatsBounds.min} chỗ</span>
                        <span>{seatsBounds.max} chỗ</span>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="hidden lg:block lg:order-8" aria-hidden="true" />
              </div>
            </div>
          ) : null}
        </div>
      </div>
    </section>
  )
}

