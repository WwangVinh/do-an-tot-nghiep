import { CreditCard, Clock, ShieldCheck } from 'lucide-react'
import { useEffect, useId, useMemo, useRef, useState } from 'react'
import toast from 'react-hot-toast'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'

import { INSTALLMENT_CAR_OPTIONS } from '../installmentCarOptions'
import { env } from '../../../lib/env'

const NAVY = '#243A5E'
const FORM_TITLE_RED = '#8B1D1D'

const featurePills = [
  { Icon: CreditCard, label: 'Vay tới 85% giá trị xe', highlight: true },
  { Icon: Clock, label: 'Tối đa 8 năm', highlight: false },
  { Icon: ShieldCheck, label: 'Duyệt hồ sơ nhanh', highlight: false },
] as const

const inputClassName =
  `h-[52px] w-full rounded-xl border border-slate-200 bg-white px-4 text-[15px] text-slate-900 shadow-[inset_0_1px_0_rgba(255,255,255,0.9)] placeholder:text-slate-400 outline-none transition ` +
  `hover:border-slate-300 focus:border-[#243A5E] focus:ring-2 focus:ring-[#243A5E]/20`

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

export function InstallmentRegisterSection() {
  const nameId = useId()
  const phoneId = useId()
  const carId = useId()

  const { data: carOptionsApi = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'installment'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const rootRef = useRef<HTMLDivElement | null>(null)
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<CarOption | null>(null)
  const [touched, setTouched] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const carOptions = useMemo<CarOption[]>(() => {
    if (carOptionsApi.length > 0) return carOptionsApi
    return INSTALLMENT_CAR_OPTIONS
  }, [carOptionsApi])

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

  const todayISO = useMemo(() => new Date().toISOString().slice(0, 10), [])
  const SHOWROOM_ID = 1

  return (
    <section className="relative overflow-hidden bg-[#f0f2f7]">
      <div
        className="pointer-events-none absolute inset-0"
        style={{
          background:
            'radial-gradient(800px 400px at 90% 0%, rgba(36,58,94,0.11), transparent 50%), radial-gradient(500px 320px at 0% 100%, rgba(139,29,29,0.07), transparent 55%)',
        }}
        aria-hidden
      />
      <div
        className="pointer-events-none absolute bottom-0 right-0 h-[min(70%,520px)] w-[min(55vw,420px)] translate-x-1/4 translate-y-1/4 rounded-full bg-[#243A5E]/[0.07] blur-3xl"
        aria-hidden
      />

      <div className="relative mx-auto w-full max-w-screen-2xl px-5 py-14 sm:px-6 sm:py-16 lg:px-8 lg:py-[4.5rem]">
        <div className="grid min-w-0 grid-cols-1 gap-12 lg:grid-cols-[minmax(0,1.12fr)_minmax(0,0.88fr)] lg:items-center lg:gap-x-12 xl:gap-x-16">
          <div className="relative min-w-0 pl-5 before:absolute before:left-0 before:top-0 before:bottom-0 before:w-1 before:rounded-full before:bg-gradient-to-b before:from-[#243A5E] before:via-[#4a6494] before:to-[#8B1D1D] before:content-[''] before:shadow-[2px_0_12px_rgba(36,58,94,0.2)] sm:pl-7">
            <p className="inline-flex items-center gap-2 rounded-full border border-white/80 bg-white/95 px-3.5 py-2 text-[10px] font-semibold uppercase tracking-[0.2em] text-[#243A5E] shadow-[0_2px_12px_rgba(36,58,94,0.08)] backdrop-blur-sm sm:text-[11px]">
              <span className="flex h-2 w-2 animate-pulse rounded-full bg-[#8B1D1D]" aria-hidden />
              Tên công ty <span className="font-normal text-slate-300">•</span> Đại lý phân phối toàn quốc
            </p>

            <h1 className="mt-6 text-[1.85rem] font-extrabold leading-[1.15] tracking-tight sm:text-4xl lg:text-[2.45rem]">
              <span style={{ color: NAVY }}>Mua xe </span>
              <span className="bg-gradient-to-r from-[#243A5E] via-[#1e5080] to-[#8B1D1D] bg-clip-text text-transparent">
                trả góp
              </span>
            </h1>
            <div
              className="mt-3 h-1 w-16 rounded-full bg-gradient-to-r from-[#243A5E] to-[#8B1D1D] shadow-sm sm:w-20"
              aria-hidden
            />

            <p className="mt-5 max-w-xl text-[15px] leading-relaxed text-slate-600 sm:text-base">
              Tư vấn gói vay phù hợp, thủ tục rõ ràng — đồng hành cùng bạn từ đăng ký đến khi nhận
              xe.
            </p>

            <div className="mt-6 flex flex-wrap gap-3 sm:gap-4">
              <div className="flex min-w-[5.5rem] flex-1 flex-col rounded-xl border border-[#243A5E]/15 bg-gradient-to-br from-white to-slate-50/80 px-3 py-2.5 text-center shadow-[0_4px_14px_rgba(36,58,94,0.08)] sm:min-w-[6rem] sm:px-4 sm:py-3">
                <span className="text-lg font-extrabold tabular-nums text-[#8B1D1D] sm:text-xl">85%</span>
                <span className="mt-0.5 text-[10px] font-medium uppercase tracking-wide text-slate-500 sm:text-[11px]">
                  Tối đa vay
                </span>
              </div>
              <div className="flex min-w-[5.5rem] flex-1 flex-col rounded-xl border border-[#243A5E]/15 bg-gradient-to-br from-white to-slate-50/80 px-3 py-2.5 text-center shadow-[0_4px_14px_rgba(36,58,94,0.08)] sm:min-w-[6rem] sm:px-4 sm:py-3">
                <span className="text-lg font-extrabold tabular-nums text-[#243A5E] sm:text-xl">8</span>
                <span className="mt-0.5 text-[10px] font-medium uppercase tracking-wide text-slate-500 sm:text-[11px]">
                  Năm vay
                </span>
              </div>
              <div className="flex min-w-[5.5rem] flex-1 flex-col rounded-xl border border-[#243A5E]/15 bg-gradient-to-br from-white to-slate-50/80 px-3 py-2.5 text-center shadow-[0_4px_14px_rgba(36,58,94,0.08)] sm:min-w-[6rem] sm:px-4 sm:py-3">
                <span className="text-lg font-extrabold tabular-nums text-[#243A5E] sm:text-xl">24h</span>
                <span className="mt-0.5 text-[10px] font-medium uppercase tracking-wide text-slate-500 sm:text-[11px]">
                  Duyệt hồ sơ
                </span>
              </div>
            </div>

            <ul className="mt-7 flex flex-wrap gap-3">
              {featurePills.map(({ Icon, label, highlight }) => (
                <li
                  key={label}
                  className={[
                    'inline-flex items-center gap-2.5 rounded-full px-4 py-2.5 text-[13px] font-semibold transition sm:text-sm',
                    highlight
                      ? 'border-2 border-[#243A5E]/35 bg-gradient-to-r from-[#243A5E]/[0.08] to-[#8B1D1D]/[0.06] text-[#243A5E] shadow-[0_4px_16px_rgba(36,58,94,0.12)] ring-1 ring-[#243A5E]/10'
                      : 'border border-slate-200 bg-white/95 text-slate-700 shadow-sm hover:border-slate-300 hover:shadow-md',
                  ].join(' ')}
                >
                  <Icon className="h-4 w-4 shrink-0 text-[#243A5E]" strokeWidth={2} aria-hidden />
                  {label}
                </li>
              ))}
            </ul>

            <div className="mt-9 max-w-[34rem] space-y-4 text-[15px] leading-[1.8] text-slate-600 sm:text-base">
              <p>
                Là trả trước một phần tiền mua xe, phần còn thiếu sẽ vay ngân hàng rồi hàng tháng
                trả dần cho ngân hàng cả gốc và lãi theo phương thức{' '}
                <strong className="font-semibold text-slate-800">trừ lùi</strong> trong suốt thời gian trả góp.
              </p>
              <p>
                Hỗ trợ tư vấn mua xe đại lý phân phối toàn quốc trả góp tới{' '}
                <strong className="font-semibold text-[#8B1D1D]">85% giá trị xe</strong>, thời gian vay tối đa{' '}
                <strong className="font-semibold text-[#243A5E]">8 năm</strong>. Thủ tục đơn giản nhanh gọn, thời
                gian thẩm duyệt trong vòng{' '}
                <strong className="rounded-md bg-[#243A5E]/[0.1] px-1.5 py-0.5 font-semibold text-[#243A5E]">
                  24h
                </strong>
                , kể cả khách hàng ở tỉnh, bao đậu hồ sơ khó. Vui lòng liên hệ để được tư vấn chính
                xác.
              </p>
            </div>
          </div>

          <div className="relative flex min-w-0 w-full justify-center lg:justify-end">
            <div
              className="pointer-events-none absolute left-1/2 top-1/2 -z-0 h-[min(100%,380px)] w-[min(100%,340px)] -translate-x-1/2 -translate-y-1/2 rounded-[2rem] bg-gradient-to-br from-[#243A5E]/25 via-[#8B1D1D]/10 to-transparent opacity-90 blur-2xl lg:left-auto lg:right-0 lg:translate-x-0"
              aria-hidden
            />

            <div className="relative z-10 w-full max-w-[min(100%,480px)] overflow-hidden rounded-2xl border border-white/90 bg-white/95 shadow-[0_8px_40px_rgba(15,23,42,0.1),0_0_0_1px_rgba(36,58,94,0.06)] backdrop-blur-sm lg:max-w-none">
              <div className="relative">
                <div
                  className="h-[6px] w-full bg-gradient-to-r from-[#243A5E] from-[5%] via-[#5a7199] via-50% to-[#8B1D1D] to-[98%]"
                  aria-hidden
                />
                <div
                  className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-white/50 to-transparent"
                  aria-hidden
                />
              </div>

              <div className="p-8 sm:p-9 lg:p-10">
                <div className="flex justify-center">
                  <span className="inline-flex items-center rounded-full border border-[#243A5E]/20 bg-[#243A5E]/[0.06] px-3 py-1 text-[11px] font-bold uppercase tracking-[0.15em] text-[#243A5E]">
                    Ưu đãi trả góp
                  </span>
                </div>
                <h2
                  className="mt-4 text-center text-xl font-extrabold uppercase leading-tight tracking-[0.05em] text-balance sm:text-2xl lg:text-[1.65rem] xl:text-[1.85rem] xl:tracking-[0.06em]"
                  style={{ color: FORM_TITLE_RED }}
                >
                  Đăng ký mua trả góp
                </h2>
                <p className="mx-auto mt-3 text-center text-sm leading-relaxed text-slate-500">
                  Điền thông tin — chúng tôi gọi lại tư vấn trong thời gian sớm nhất.
                </p>

                <form
                  className="mt-8 space-y-4"
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
                      toast.error('Mã xe không hợp lệ. Vui lòng chọn xe từ danh sách tải từ hệ thống.')
                      return
                    }

                    const form = e.currentTarget
                    const fd = new FormData(form)
                    const customerName = String(fd.get('fullName') ?? '').trim()
                    const phone = String(fd.get('phone') ?? '').trim()

                    const payload: BookingCreatePayload = {
                      carId: numericCarId,
                      showroomId: SHOWROOM_ID,
                      customerName,
                      phone,
                      bookingDate: todayISO,
                      note: 'Yêu cầu tư vấn mua trả góp',
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
                      name="carSearch"
                      role="combobox"
                      aria-expanded={open}
                      aria-controls={`${carId}-listbox`}
                      required={false}
                      placeholder={isCarsLoading ? 'Đang tải danh sách xe...' : 'Xe muốn mua'}
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
                        'cursor-pointer pr-11 text-slate-800',
                        touched && !selectedCar?.id ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                      ].join(' ')}
                      autoComplete="off"
                    />

                    <button
                      type="button"
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded-lg p-1 text-slate-400 hover:text-slate-600"
                      aria-label={open ? 'Đóng danh sách xe' : 'Mở danh sách xe'}
                      onClick={() => {
                        setTouched(true)
                        setOpen((v) => !v)
                        if (!open) setQuery(selectedCar?.label ?? '')
                      }}
                    >
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
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
                        className="absolute z-20 mt-2 max-h-64 w-full overflow-auto rounded-xl border border-slate-200 bg-white py-1 text-sm shadow-lg"
                      >
                        {isCarsLoading ? (
                          <div className="px-4 py-2 text-slate-500">Đang tải...</div>
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
                                  <span className="ml-3 text-[11px] font-semibold text-[#243A5E]">Đã chọn</span>
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
                    className="group mt-2 flex h-[52px] w-full cursor-pointer items-center justify-center rounded-xl bg-gradient-to-b from-[#2f4f7a] to-[#243A5E] text-[13px] font-bold uppercase tracking-[0.12em] text-white shadow-[0_4px_0_#1a2d4a,0_8px_24px_rgba(36,58,94,0.35)] transition hover:from-[#355887] hover:to-[#2a4570] hover:shadow-[0_4px_0_#1a2d4a,0_12px_28px_rgba(36,58,94,0.4)] focus:outline-none focus-visible:ring-2 focus-visible:ring-[#243A5E] focus-visible:ring-offset-2 active:translate-y-0.5 active:shadow-[0_2px_0_#1a2d4a,0_4px_16px_rgba(36,58,94,0.3)] disabled:cursor-not-allowed disabled:opacity-70 sm:text-sm"
                  >
                    {isSubmitting ? 'Đang gửi...' : 'Gửi yêu cầu ngay'}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}
