import { useEffect, useId, useMemo, useRef, useState } from 'react'
import toast from 'react-hot-toast'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'

import { env } from '../lib/env'

export type QuoteRegisterMode = 'installment' | 'full'

export type QuoteRegisterCardValues = {
  mode: QuoteRegisterMode
  fullName: string
  phone: string
  carId: string
  showroomId: string
  showroomName: string
}

export type QuoteRegisterCarOption = {
  id: string
  label: string
}

export type QuoteRegisterShowroomOption = {
  id: string
  label: string
}

type CustomerCarListDto = {
  carId: number
  name: string
}

type PagedCarsResponse = {
  data: CustomerCarListDto[]
}

type CarDetailDto = {
  carId: number
  name: string
  showrooms: string // tên showroom dạng string, VD: "Hà Nội"
}

type PagedCarDetailResponse = {
  data: CarDetailDto[]
}

const carsApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

const bookingsApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

type BookingCreatePayload = {
  carId: number
  showroomId: number
  customerName: string
  phone: string
  bookingDate: string
  bookingTime?: string
  timeSpan?: string
  note?: string
}

async function getCarOptions(): Promise<QuoteRegisterCarOption[]> {
  const res = await carsApi.get<PagedCarsResponse>('Cars', {
    params: { page: 1, pageSize: 200, inStockOnly: false },
  })
  const list = Array.isArray(res.data?.data) ? res.data.data : []
  return list
    .map((c) => ({ id: String(c.carId), label: c.name }))
    .filter((c) => c.label.trim().length > 0)
}

async function getShowroomsByCarId(carId: string): Promise<QuoteRegisterShowroomOption[]> {
  const res = await carsApi.get<PagedCarDetailResponse>('Cars', {
    params: { page: 1, pageSize: 200, inStockOnly: false, carId },
  })
  const list = Array.isArray(res.data?.data) ? res.data.data : []

  // field `showrooms` là string tên showroom — dùng index làm id tạm
  const seen = new Set<string>()
  const options: QuoteRegisterShowroomOption[] = []
  list.forEach((c, idx) => {
    const name = c.showrooms?.trim()
    if (name && !seen.has(name)) {
      seen.add(name)
      options.push({ id: String(idx + 1), label: `Showroom ${name}` })
    }
  })
  return options
}

export type QuoteRegisterCardProps = {
  className?: string
  title?: string
  submitLabel?: string
  defaultMode?: QuoteRegisterMode
  cars?: QuoteRegisterCarOption[]
  onSubmit?: (values: QuoteRegisterCardValues) => void
}

export function QuoteRegisterCard({
  className,
  title = 'NHẬN BÁO GIÁ & ĐĂNG KÝ LÁI THỬ',
  submitLabel = 'GỬI YÊU CẦU NGAY',
  defaultMode = 'installment',
  cars = [],
  onSubmit,
}: QuoteRegisterCardProps) {
  const fullNameId = useId()
  const phoneId = useId()
  const modeInstallmentId = useId()
  const modeFullId = useId()
  const carSelectId = useId()
  const showroomSelectId = useId()

  // ── Cars ──────────────────────────────────────────────
  const { data: carsApiOptions = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'quote-register'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const resolvedCars = useMemo(
    () => (carsApiOptions.length > 0 ? carsApiOptions : cars),
    [cars, carsApiOptions],
  )

  // ── State xe ──────────────────────────────────────────
  const rootCarRef = useRef<HTMLDivElement | null>(null)
  const [carOpen, setCarOpen] = useState(false)
  const [carQuery, setCarQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<QuoteRegisterCarOption | null>(null)
  const [carTouched, setCarTouched] = useState(false)

  // ── Showrooms theo xe đã chọn ─────────────────────────
  const { data: showroomOptions = [], isLoading: isShowroomsLoading } = useQuery({
    queryKey: ['showrooms-by-car', selectedCar?.id],
    queryFn: () => getShowroomsByCarId(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  // ── State showroom ────────────────────────────────────
  const rootShowroomRef = useRef<HTMLDivElement | null>(null)
  const [showroomOpen, setShowroomOpen] = useState(false)
  const [selectedShowroom, setSelectedShowroom] = useState<QuoteRegisterShowroomOption | null>(null)
  const [showroomTouched, setShowroomTouched] = useState(false)

  // Reset showroom khi đổi xe
  useEffect(() => {
    setSelectedShowroom(null)
    setShowroomTouched(false)
    setShowroomOpen(false)
  }, [selectedCar?.id])

  // Auto-select nếu chỉ có 1 showroom
  useEffect(() => {
    if (showroomOptions.length === 1) {
      setSelectedShowroom(showroomOptions[0])
    }
  }, [showroomOptions])

  // ── Submit state ──────────────────────────────────────
  const [isSubmitting, setIsSubmitting] = useState(false)

  const todayISO = useMemo(() => new Date().toISOString().slice(0, 10), [])

  const filteredCars = useMemo(() => {
    const q = carQuery.trim().toLowerCase()
    if (!q) return resolvedCars
    return resolvedCars.filter((c) => c.label.toLowerCase().includes(q))
  }, [resolvedCars, carQuery])

  // Đóng dropdown khi click ra ngoài
  useEffect(() => {
    function onPointerDown(e: MouseEvent) {
      if (rootCarRef.current && e.target instanceof Node && !rootCarRef.current.contains(e.target))
        setCarOpen(false)
      if (rootShowroomRef.current && e.target instanceof Node && !rootShowroomRef.current.contains(e.target))
        setShowroomOpen(false)
    }
    document.addEventListener('mousedown', onPointerDown)
    return () => document.removeEventListener('mousedown', onPointerDown)
  }, [])

  return (
    <section
      className={[
        'w-full rounded-xl bg-white shadow-xl ring-1 ring-black/5',
        className ?? '',
      ].join(' ')}
      aria-label="Nhận báo giá và đăng ký lái thử"
    >
      <div className="p-5 sm:p-6">
        <div className="text-center text-base font-extrabold tracking-wide text-slate-800 sm:text-lg">
          {title}
        </div>

        <form
          className="mt-4 space-y-3"
          onSubmit={async (e) => {
            e.preventDefault()
            setCarTouched(true)
            setShowroomTouched(true)
            setCarOpen(false)
            setShowroomOpen(false)

            if (!selectedCar?.id) {
              toast.error('Vui lòng chọn dòng xe.')
              return
            }
            if (!selectedShowroom?.id) {
              toast.error('Vui lòng chọn showroom.')
              return
            }

            const fd = new FormData(e.currentTarget)
            const values: QuoteRegisterCardValues = {
              mode: (String(fd.get('mode') ?? defaultMode) as QuoteRegisterMode) || defaultMode,
              fullName: String(fd.get('fullName') ?? '').trim(),
              phone: String(fd.get('phone') ?? '').trim(),
              carId: selectedCar.id,
              showroomId: selectedShowroom.id,
              showroomName: selectedShowroom.label,
            }

            const numericCarId = Number(values.carId)
            if (!Number.isFinite(numericCarId)) {
              toast.error('Mã xe không hợp lệ. Vui lòng chọn xe từ danh sách.')
              return
            }

            const note =
              values.mode === 'installment'
                ? 'Yêu cầu báo giá / tư vấn trả góp'
                : 'Yêu cầu báo giá / tư vấn trả thẳng'

            const payload: BookingCreatePayload = {
              carId: numericCarId,
              showroomId: Number(values.showroomId),
              customerName: values.fullName,
              phone: values.phone,
              bookingDate: todayISO,
              note,
            }

            try {
              setIsSubmitting(true)
              await bookingsApi.post('Bookings/public-create', payload)
              toast.success('Đã gửi yêu cầu. Chúng tôi sẽ liên hệ sớm.')
              onSubmit?.(values)
              e.currentTarget.reset()
              setSelectedCar(null)
              setCarQuery('')
              setCarTouched(false)
              setSelectedShowroom(null)
              setShowroomTouched(false)
            } catch (err) {
              if (axios.isAxiosError(err)) {
                const message =
                  (err.response?.data as any)?.message ??
                  (typeof err.response?.data === 'string' ? err.response?.data : null) ??
                  err.message
                toast.error(message || 'Gửi yêu cầu thất bại. Vui lòng thử lại.')
                return
              }
              toast.error('Gửi yêu cầu thất bại. Vui lòng thử lại.')
            } finally {
              setIsSubmitting(false)
            }
          }}
        >
          {/* ── Hình thức ── */}
          <fieldset className="flex items-center justify-center gap-6">
            <legend className="sr-only">Hình thức</legend>
            <label className="inline-flex cursor-pointer items-center gap-2 text-sm font-medium text-slate-700">
              <input
                id={modeInstallmentId}
                type="radio"
                name="mode"
                value="installment"
                defaultChecked={defaultMode === 'installment'}
                className="h-4 w-4 accent-rose-500"
              />
              Trả góp
            </label>
            <label className="inline-flex cursor-pointer items-center gap-2 text-sm font-medium text-slate-700">
              <input
                id={modeFullId}
                type="radio"
                name="mode"
                value="full"
                defaultChecked={defaultMode === 'full'}
                className="h-4 w-4 accent-rose-500"
              />
              Trả thẳng
            </label>
          </fieldset>

          {/* ── Họ tên ── */}
          <div>
            <label htmlFor={fullNameId} className="sr-only">Họ và tên</label>
            <input
              id={fullNameId}
              name="fullName"
              required
              placeholder="Họ và tên"
              className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40"
              autoComplete="name"
            />
          </div>

          {/* ── Điện thoại ── */}
          <div>
            <label htmlFor={phoneId} className="sr-only">Điện thoại</label>
            <input
              id={phoneId}
              name="phone"
              required
              inputMode="tel"
              placeholder="Điện thoại"
              className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40"
              autoComplete="tel"
            />
          </div>

          {/* ── Chọn xe ── */}
          <div>
            <div ref={rootCarRef} className="relative">
              <label htmlFor={carSelectId} className="sr-only">Chọn dòng xe</label>
              <input type="hidden" name="carId" value={selectedCar?.id ?? ''} />
              <input
                id={carSelectId}
                name="carSearch"
                role="combobox"
                aria-expanded={carOpen}
                aria-controls={`${carSelectId}-listbox`}
                placeholder={isCarsLoading ? 'Đang tải danh sách xe...' : '== Chọn dòng xe =='}
                value={carOpen ? carQuery : selectedCar?.label ?? ''}
                onFocus={() => { setCarOpen(true); setCarQuery(selectedCar?.label ?? '') }}
                onChange={(e) => { setCarOpen(true); setCarQuery(e.target.value); setCarTouched(true) }}
                onBlur={() => setCarTouched(true)}
                className={[
                  'h-11 w-full rounded-lg border bg-white px-3 pr-11 text-sm shadow-sm outline-none transition',
                  'text-slate-900 placeholder:text-slate-400',
                  carTouched && !selectedCar?.id
                    ? 'border-rose-300 focus-visible:border-rose-400 focus-visible:ring-2 focus-visible:ring-rose-400/40'
                    : 'border-slate-200 focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40',
                ].join(' ')}
                autoComplete="off"
              />
              <button
                type="button"
                className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded p-1 text-slate-400 hover:text-slate-600"
                aria-label={carOpen ? 'Đóng danh sách xe' : 'Mở danh sách xe'}
                onClick={() => { setCarTouched(true); setCarOpen((v) => !v); if (!carOpen) setCarQuery(selectedCar?.label ?? '') }}
              >
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                  <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                </svg>
              </button>

              {carOpen && (
                <div
                  id={`${carSelectId}-listbox`}
                  role="listbox"
                  className="absolute z-20 mt-2 max-h-64 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 text-sm shadow-lg"
                >
                  {isCarsLoading ? (
                    <div className="px-3 py-2 text-slate-500">Đang tải...</div>
                  ) : filteredCars.length === 0 ? (
                    <div className="px-3 py-2 text-slate-500">Không tìm thấy xe phù hợp.</div>
                  ) : (
                    filteredCars.map((c) => {
                      const isSelected = c.id === selectedCar?.id
                      return (
                        <button
                          key={c.id}
                          type="button"
                          role="option"
                          aria-selected={isSelected}
                          className={[
                            'flex w-full items-center justify-between px-3 py-2 text-left',
                            isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50',
                          ].join(' ')}
                          onClick={() => { setSelectedCar(c); setCarQuery(c.label); setCarOpen(false); setCarTouched(true) }}
                        >
                          <span className="truncate">{c.label}</span>
                          {isSelected && <span className="ml-3 text-[11px] font-semibold text-rose-500">Đã chọn</span>}
                        </button>
                      )
                    })
                  )}
                </div>
              )}
            </div>
          </div>

          {/* ── Chọn showroom (chỉ hiện sau khi chọn xe) ── */}
          {selectedCar && (
            <div>
              <div ref={rootShowroomRef} className="relative">
                <label htmlFor={showroomSelectId} className="sr-only">Chọn showroom</label>
                <input type="hidden" name="showroomId" value={selectedShowroom?.id ?? ''} />
                <input
                  id={showroomSelectId}
                  name="showroomSearch"
                  role="combobox"
                  aria-expanded={showroomOpen}
                  aria-controls={`${showroomSelectId}-listbox`}
                  readOnly
                  placeholder={
                    isShowroomsLoading
                      ? 'Đang tải showroom...'
                      : showroomOptions.length === 0
                      ? 'Không có showroom cho xe này'
                      : '== Chọn showroom =='
                  }
                  value={selectedShowroom?.label ?? ''}
                  onFocus={() => { if (showroomOptions.length > 0) setShowroomOpen(true) }}
                  onBlur={() => setShowroomTouched(true)}
                  className={[
                    'h-11 w-full cursor-pointer rounded-lg border bg-white px-3 pr-11 text-sm shadow-sm outline-none transition',
                    'text-slate-900 placeholder:text-slate-400',
                    showroomTouched && !selectedShowroom?.id
                      ? 'border-rose-300 focus-visible:border-rose-400 focus-visible:ring-2 focus-visible:ring-rose-400/40'
                      : 'border-slate-200 focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40',
                    showroomOptions.length === 0 ? 'opacity-60 cursor-not-allowed' : '',
                  ].join(' ')}
                />
                <button
                  type="button"
                  disabled={showroomOptions.length === 0 || isShowroomsLoading}
                  className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded p-1 text-slate-400 hover:text-slate-600 disabled:opacity-40"
                  aria-label={showroomOpen ? 'Đóng danh sách showroom' : 'Mở danh sách showroom'}
                  onClick={() => { setShowroomTouched(true); setShowroomOpen((v) => !v) }}
                >
                  {isShowroomsLoading ? (
                    <svg className="animate-spin" width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="3" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z" />
                    </svg>
                  ) : (
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                      <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  )}
                </button>

                {showroomOpen && showroomOptions.length > 0 && (
                  <div
                    id={`${showroomSelectId}-listbox`}
                    role="listbox"
                    className="absolute z-20 mt-2 max-h-48 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 text-sm shadow-lg"
                  >
                    {showroomOptions.map((s) => {
                      const isSelected = s.id === selectedShowroom?.id
                      return (
                        <button
                          key={s.id}
                          type="button"
                          role="option"
                          aria-selected={isSelected}
                          className={[
                            'flex w-full items-center justify-between px-3 py-2 text-left',
                            isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50',
                          ].join(' ')}
                          onClick={() => { setSelectedShowroom(s); setShowroomOpen(false); setShowroomTouched(true) }}
                        >
                          <span className="truncate">{s.label}</span>
                          {isSelected && <span className="ml-3 text-[11px] font-semibold text-rose-500">Đã chọn</span>}
                        </button>
                      )
                    })}
                  </div>
                )}
              </div>
            </div>
          )}

          {/* ── Submit ── */}
          <button
            type="submit"
            disabled={isSubmitting}
            className="inline-flex h-11 w-full cursor-pointer items-center justify-center rounded-lg bg-rose-500 px-4 text-sm font-extrabold tracking-wide text-white shadow-sm transition hover:bg-rose-600 focus:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40 disabled:cursor-not-allowed disabled:opacity-70"
          >
            {isSubmitting ? 'Đang gửi...' : submitLabel}
          </button>
        </form>
      </div>
    </section>
  )
}