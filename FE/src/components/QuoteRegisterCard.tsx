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
}

export type QuoteRegisterCarOption = {
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
  bookingDate: string // YYYY-MM-DD (DateOnly)
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
  const carId = useId()

  const { data: carsApiOptions = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'quote-register'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  // Ưu tiên danh sách từ API (đầy đủ). Nếu API rỗng/lỗi thì fallback qua props.
  const resolvedCars = useMemo(
    () => (carsApiOptions.length > 0 ? carsApiOptions : cars),
    [cars, carsApiOptions],
  )

  const rootRef = useRef<HTMLDivElement | null>(null)
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<QuoteRegisterCarOption | null>(null)
  const [touched, setTouched] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const filteredCars = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return resolvedCars
    return resolvedCars.filter((c) => c.label.toLowerCase().includes(q))
  }, [resolvedCars, query])

  const todayISO = useMemo(() => new Date().toISOString().slice(0, 10), [])
  const SHOWROOM_ID = 1

  useEffect(() => {
    function onPointerDown(e: MouseEvent) {
      const el = rootRef.current
      if (!el) return
      if (e.target instanceof Node && !el.contains(e.target)) setOpen(false)
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
            setTouched(true)
            setOpen(false)

            if (!selectedCar?.id) {
              toast.error('Vui lòng chọn dòng xe.')
              return
            }

            const fd = new FormData(e.currentTarget)
            const values: QuoteRegisterCardValues = {
              mode: (String(fd.get('mode') ?? defaultMode) as QuoteRegisterMode) || defaultMode,
              fullName: String(fd.get('fullName') ?? '').trim(),
              phone: String(fd.get('phone') ?? '').trim(),
              carId: String(fd.get('carId') ?? ''),
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
              showroomId: SHOWROOM_ID,
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
              setQuery('')
              setTouched(false)
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

          <div>
            <label htmlFor={fullNameId} className="sr-only">
              Họ và tên
            </label>
            <input
              id={fullNameId}
              name="fullName"
              required
              placeholder="Họ và tên"
              className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40"
              autoComplete="name"
            />
          </div>

          <div>
            <label htmlFor={phoneId} className="sr-only">
              Điện thoại
            </label>
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

          <div>
            <div ref={rootRef} className="relative">
              <label htmlFor={carId} className="sr-only">
                Chọn dòng xe
              </label>

              <input type="hidden" name="carId" value={selectedCar?.id ?? ''} />

              <input
                id={carId}
                name="carSearch"
                role="combobox"
                aria-expanded={open}
                aria-controls={`${carId}-listbox`}
                placeholder={isCarsLoading ? 'Đang tải danh sách xe...' : '== Chọn dòng xe =='}
                value={open ? query : selectedCar?.label ?? ''}
                onFocus={() => {
                  setOpen(true)
                  setQuery(selectedCar?.label ?? '')
                }}
                onChange={(e) => {
                  setOpen(true)
                  setQuery(e.target.value)
                  setTouched(true)
                }}
                onBlur={() => setTouched(true)}
                className={[
                  'h-11 w-full rounded-lg border bg-white px-3 pr-11 text-sm shadow-sm outline-none transition',
                  'text-slate-900 placeholder:text-slate-400',
                  touched && !selectedCar?.id
                    ? 'border-rose-300 focus-visible:border-rose-400 focus-visible:ring-2 focus-visible:ring-rose-400/40'
                    : 'border-slate-200 focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40',
                ].join(' ')}
                autoComplete="off"
              />

              <button
                type="button"
                className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded p-1 text-slate-400 hover:text-slate-600"
                aria-label={open ? 'Đóng danh sách xe' : 'Mở danh sách xe'}
                onClick={() => {
                  setTouched(true)
                  setOpen((v) => !v)
                  if (!open) setQuery(selectedCar?.label ?? '')
                }}
              >
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                  <path
                    d="M6 9l6 6 6-6"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>

              {open ? (
                <div
                  id={`${carId}-listbox`}
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
                          onClick={() => {
                            setSelectedCar(c)
                            setQuery(c.label)
                            setOpen(false)
                            setTouched(true)
                          }}
                        >
                          <span className="truncate">{c.label}</span>
                          {isSelected ? (
                            <span className="ml-3 text-[11px] font-semibold text-rose-500">Đã chọn</span>
                          ) : null}
                        </button>
                      )
                    })
                  )}
                </div>
              ) : null}
            </div>
          </div>

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

