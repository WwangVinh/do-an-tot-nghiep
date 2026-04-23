import { useEffect, useId, type ReactNode, useMemo, useRef, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'
import toast from 'react-hot-toast'
import { Mail, MapPin, Phone } from 'lucide-react'

import { env } from '../../../lib/env'

const BRAND_BLUE = '#335599'
const BUTTON_BLUE = '#1a3a5a'

const CONTACT = {
  showroomName: 'Tên công ty',
  address: 'Địa chỉ trụ sở chính',
  hotline: '0393.775.683',
  email: 'tencongty@gmail.com',
} as const

export type ShowroomListItem = {
  showroomId: number
  name: string
  province?: string | null
  district?: string | null
  streetAddress?: string | null
  fullAddress?: string | null
  hotline?: string | null
}

const OTHER_CAR: CarOption = { id: 'other', label: 'Khác / Tư vấn chung' }

type CarOption = { id: string; label: string }

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

async function getCarOptions(): Promise<CarOption[]> {
  const res = await carsApi.get<PagedCarsResponse>('Cars', {
    params: { page: 1, pageSize: 200, inStockOnly: false },
  })

  const list = Array.isArray(res.data?.data) ? res.data.data : []
  return list
    .map((c) => ({ id: String(c.carId), label: c.name }))
    .filter((c) => c.label.trim().length > 0)
}

const inputClassName =
  'h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-[#335599]/50 focus-visible:ring-2 focus-visible:ring-[#335599]/25'

function IconBubble({ children }: { children: ReactNode }) {
  return (
    <span
      className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full text-white shadow-sm"
      style={{ backgroundColor: BRAND_BLUE }}
    >
      {children}
    </span>
  )
}

export type ContactInfoSectionProps = {
  showrooms: ShowroomListItem[]
  isShowroomsLoading?: boolean
  selectedShowroomId: number | null
  onSelectShowroomId: (id: number) => void
}

function telHref(hotline: string): string {
  const digits = hotline.replace(/[^\d+]/g, '')
  return digits ? `tel:${digits}` : ''
}

export function ContactInfoSection({
  showrooms,
  isShowroomsLoading = false,
  selectedShowroomId,
  onSelectShowroomId,
}: ContactInfoSectionProps) {
  const nameId = useId()
  const phoneId = useId()
  const carId = useId()
  const todayISO = useMemo(() => new Date().toISOString().slice(0, 10), [])

  const selectedShowroom = useMemo(() => {
    if (showrooms.length === 0) return null
    const byId = selectedShowroomId ? showrooms.find((s) => s.showroomId === selectedShowroomId) : undefined
    return byId ?? showrooms[0] ?? null
  }, [selectedShowroomId, showrooms])

  const { data: carOptionsFromApi = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'contact-info'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const carOptions = useMemo(() => [...carOptionsFromApi, OTHER_CAR], [carOptionsFromApi])

  const rootRef = useRef<HTMLDivElement | null>(null)
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<CarOption | null>(null)
  const [touched, setTouched] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const filteredCars = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return carOptions
    return carOptions.filter((c) => c.label.toLowerCase().includes(q))
  }, [carOptions, query])

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
    <section className="w-full bg-white" aria-labelledby="contact-showroom-heading">
      <div className="mx-auto w-full max-w-screen-2xl px-4 py-12 sm:px-6 lg:px-8 lg:py-16">
        <div className="grid grid-cols-1 gap-12 lg:grid-cols-2 lg:gap-16 xl:gap-20">
          <div>
            <h1
              id="contact-showroom-heading"
              className="text-xl font-bold uppercase tracking-wide text-slate-900 sm:text-2xl"
            >
              {CONTACT.showroomName}
            </h1>

            <div className="mt-6">
              <p className="text-xs font-bold uppercase tracking-wide" style={{ color: BRAND_BLUE }}>
                Danh sách showroom
              </p>

              <div className="mt-3 rounded-xl border border-slate-200/90 bg-white p-4 shadow-[0_8px_30px_-12px_rgba(15,23,42,0.10)]">
                {isShowroomsLoading ? (
                  <div className="text-sm text-slate-500">Đang tải danh sách showroom…</div>
                ) : showrooms.length === 0 ? (
                  <div className="text-sm text-slate-500">Chưa có showroom.</div>
                ) : (
                  <ul className="max-h-72 space-y-2 overflow-auto pr-1">
                    {showrooms.map((s) => {
                      const isActive = s.showroomId === (selectedShowroom?.showroomId ?? -1)
                      return (
                        <li key={s.showroomId}>
                          <button
                            type="button"
                            className={[
                              'w-full rounded-lg border px-3 py-2 text-left text-sm transition',
                              isActive
                                ? 'border-slate-200 bg-slate-50 text-slate-900'
                                : 'border-transparent hover:border-slate-200 hover:bg-slate-50 text-slate-700',
                            ].join(' ')}
                            onClick={() => onSelectShowroomId(s.showroomId)}
                          >
                            <div className="flex items-start justify-between gap-3">
                              <div className="min-w-0">
                                <div className="truncate font-semibold">{s.name}</div>
                                {s.fullAddress ? (
                                  <div className="mt-0.5 line-clamp-2 text-xs leading-relaxed text-slate-500">
                                    {s.fullAddress}
                                  </div>
                                ) : null}
                              </div>
                              {isActive ? (
                                <span className="shrink-0 text-[11px] font-semibold" style={{ color: BRAND_BLUE }}>
                                  Đang xem
                                </span>
                              ) : null}
                            </div>
                          </button>
                        </li>
                      )
                    })}
                  </ul>
                )}
              </div>
            </div>

            {selectedShowroom ? (
              <div className="mt-6 rounded-xl border border-slate-200/90 bg-white p-4 shadow-[0_8px_30px_-12px_rgba(15,23,42,0.10)]">
                <p className="text-xs font-bold uppercase tracking-wide" style={{ color: BRAND_BLUE }}>
                  Thông tin showroom
                </p>
                <div className="mt-2 text-sm font-semibold text-slate-900">{selectedShowroom.name}</div>
                {selectedShowroom.fullAddress ? (
                  <div className="mt-1 text-sm leading-relaxed text-slate-700">{selectedShowroom.fullAddress}</div>
                ) : null}
                {selectedShowroom.hotline ? (
                  <a
                    className="mt-2 inline-block text-sm text-slate-900 underline-offset-2 hover:underline"
                    href={telHref(selectedShowroom.hotline)}
                  >
                    {selectedShowroom.hotline}
                  </a>
                ) : null}
              </div>
            ) : null}
          </div>

          <div>
            <h2
              className="text-lg font-bold uppercase tracking-wide sm:text-xl"
              style={{ color: BRAND_BLUE }}
            >
              Tư vấn nhanh
            </h2>

            <ul className="mt-4 flex flex-col gap-6">
              <li className="flex gap-4">
                <IconBubble>
                  <MapPin className="h-5 w-5" strokeWidth={2} aria-hidden />
                </IconBubble>
                <div className="min-w-0 pt-0.5">
                  <p className="text-xs font-bold uppercase tracking-wide" style={{ color: BRAND_BLUE }}>
                    Trụ sở chính
                  </p>
                  <p className="mt-1 text-sm leading-relaxed text-slate-900">{CONTACT.address}</p>
                </div>
              </li>

              <li className="flex gap-4">
                <IconBubble>
                  <Phone className="h-5 w-5" strokeWidth={2} aria-hidden />
                </IconBubble>
                <div className="min-w-0 pt-0.5">
                  <p className="text-xs font-bold uppercase tracking-wide" style={{ color: BRAND_BLUE }}>
                    Hotline
                  </p>
                  <a
                    href="tel:+84393775683"
                    className="mt-1 inline-block text-sm text-slate-900 underline-offset-2 hover:underline"
                  >
                    {CONTACT.hotline}
                  </a>
                </div>
              </li>

              <li className="flex gap-4">
                <IconBubble>
                  <Mail className="h-5 w-5" strokeWidth={2} aria-hidden />
                </IconBubble>
                <div className="min-w-0 pt-0.5">
                  <p className="text-xs font-bold uppercase tracking-wide" style={{ color: BRAND_BLUE }}>
                    Email
                  </p>
                  <a
                    href={`mailto:${CONTACT.email}`}
                    className="mt-1 inline-block break-all text-sm text-slate-900 underline-offset-2 hover:underline"
                  >
                    {CONTACT.email}
                  </a>
                </div>
              </li>
            </ul>

            <div className="mt-4 rounded-xl border border-slate-200/90 bg-white p-5 shadow-[0_8px_30px_-12px_rgba(15,23,42,0.12)] sm:p-6">
              <form
                className="space-y-3"
                onSubmit={async (e) => {
                  e.preventDefault()
                  setTouched(true)
                  setOpen(false)

                  if (!selectedCar?.id) {
                    toast.error('Vui lòng chọn xe muốn mua.')
                    return
                  }

                  const numericCarId = Number(selectedCar.id)
                  if (!Number.isFinite(numericCarId)) {
                    toast.error('Vui lòng chọn dòng xe cụ thể từ danh sách.')
                    return
                  }

                  const form = e.currentTarget
                  const fd = new FormData(form)
                  const customerName = String(fd.get('fullName') ?? '').trim()
                  const phone = String(fd.get('phone') ?? '').trim()

                  const payload: BookingCreatePayload = {
                    carId: numericCarId,
                    showroomId: selectedShowroom?.showroomId ?? 1,
                    customerName,
                    phone,
                    bookingDate: todayISO,
                    note: 'Tư vấn nhanh (liên hệ)',
                  }

                  try {
                    setIsSubmitting(true)
                    await bookingsApi.post('Bookings/public-create', payload)
                    toast.success('Đã gửi yêu cầu. Chúng tôi sẽ liên hệ sớm.')
                    form.reset()
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
                <div>
                  <label htmlFor={nameId} className="sr-only">
                    Họ và tên
                  </label>
                  <input
                    id={nameId}
                    name="fullName"
                    required
                    placeholder="Họ và tên"
                    autoComplete="name"
                    className={inputClassName}
                  />
                </div>

                <div>
                  <label htmlFor={phoneId} className="sr-only">
                    Số điện thoại
                  </label>
                  <input
                    id={phoneId}
                    name="phone"
                    required
                    inputMode="numeric"
                    pattern="[0-9]{10}"
                    maxLength={10}
                    placeholder="Nhập số điện thoại (10 số)"
                    autoComplete="tel"
                    className={inputClassName}
                  />
                </div>

                <div ref={rootRef} className="relative">
                  <label htmlFor={carId} className="sr-only">
                    Xe muốn mua
                  </label>

                  <input type="hidden" name="carId" value={selectedCar?.id ?? ''} />

                  <input
                    id={carId}
                    role="combobox"
                    aria-expanded={open}
                    aria-controls={`${carId}-listbox`}
                    aria-required="true"
                    placeholder={isCarsLoading ? 'Đang tải danh sách xe…' : 'Gõ để tìm hoặc chọn xe'}
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
                      inputClassName,
                      'pr-11',
                      touched && !selectedCar?.id ? 'border-rose-300 focus-visible:border-rose-400 focus-visible:ring-rose-400/30' : '',
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
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden>
                      <path d="M6 9l6 6 6-6" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  </button>

                  {open ? (
                    <div
                      id={`${carId}-listbox`}
                      role="listbox"
                      className="absolute z-20 mt-2 max-h-64 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 text-sm shadow-lg"
                    >
                      {isCarsLoading ? (
                        <div className="px-4 py-2 text-slate-500">Đang tải…</div>
                      ) : filteredCars.length === 0 ? (
                        <div className="px-4 py-2 text-slate-500">Không tìm thấy xe phù hợp.</div>
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
                                'flex w-full items-center justify-between px-4 py-2 text-left',
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
                                <span className="ml-3 shrink-0 text-[11px] font-semibold" style={{ color: BRAND_BLUE }}>
                                  Đã chọn
                                </span>
                              ) : null}
                            </button>
                          )
                        })
                      )}
                    </div>
                  ) : null}
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="cursor-pointer inline-flex h-12 w-full items-center justify-center rounded-lg px-4 text-sm font-bold uppercase tracking-wide text-white shadow-sm transition hover:opacity-95 focus:outline-none focus-visible:ring-2 focus-visible:ring-[#1a3a5a]/50 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-70"
                  style={{ backgroundColor: BUTTON_BLUE }}
                >
                  {isSubmitting ? 'Đang gửi...' : 'Gửi yêu cầu ngay'}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}
