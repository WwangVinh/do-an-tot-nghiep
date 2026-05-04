import { useEffect, useId, useMemo, useRef, useState } from 'react'
import toast from 'react-hot-toast'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'

import { env } from '../lib/env'
import { http } from '../services/http/http'

export type QuoteRegisterMode = 'installment' | 'full'

export type QuoteRegisterCardValues = {
  mode: QuoteRegisterMode
  fullName: string
  phone: string
  carId: string
  carName: string
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

type ShowroomDto = {
  showroomId: number
  showroomName: string
}

const carsApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

async function getCarOptions(): Promise<QuoteRegisterCarOption[]> {
  const res = await carsApi.get<PagedCarsResponse>('Cars', {
    params: { page: 1, pageSize: 200, inStockOnly: false },
  })
  const list = Array.isArray(res.data?.data) ? res.data.data : []
  return list
    .map((c) => ({ id: String(c.carId), label: c.name }))
    .filter((c) => c.label.trim().length > 0)
}

async function getCarShowrooms(carId: string): Promise<ShowroomDto[]> {
  const res = await carsApi.get(`Cars/${carId}`)
  const data = res.data?.data
  return (data?.showroomDetails as ShowroomDto[]) || []
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
  submitLabel = 'TIẾP TỤC',
  defaultMode = 'installment',
  cars = [],
  onSubmit,
}: QuoteRegisterCardProps) {
  const fullNameId = useId()
  const phoneId = useId()
  const modeInstallmentId = useId()
  const modeFullId = useId()
  const carSelectId = useId()

  const [submitting, setSubmitting] = useState(false)
  const [selectedShowroomId, setSelectedShowroomId] = useState('')
  const [showroomOpen, setShowroomOpen] = useState(false)

  const { data: carsApiOptions = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'quote-register'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const resolvedCars = useMemo(
    () => (carsApiOptions.length > 0 ? carsApiOptions : cars),
    [cars, carsApiOptions],
  )

  const rootCarRef = useRef<HTMLDivElement | null>(null)
  const [carOpen, setCarOpen] = useState(false)
  const [carQuery, setCarQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<QuoteRegisterCarOption | null>(null)
  const [carTouched, setCarTouched] = useState(false)

  // Fetch showrooms khi chọn xe
  const { data: showrooms = [], isFetching: isShowroomsLoading } = useQuery({
    queryKey: ['car-showrooms', selectedCar?.id],
    queryFn: () => getCarShowrooms(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  // Reset showroom khi đổi xe
  useEffect(() => {
    setSelectedShowroomId('')
    setShowroomOpen(false)
  }, [selectedCar?.id])

  useEffect(() => {
    if (resolvedCars.length === 1 && !selectedCar) {
      setSelectedCar(resolvedCars[0])
      setCarQuery(resolvedCars[0].label)
    }
  }, [resolvedCars, selectedCar])

  const filteredCars = useMemo(() => {
    const q = carQuery.trim().toLowerCase()
    if (!q) return resolvedCars
    return resolvedCars.filter((c) => c.label.toLowerCase().includes(q))
  }, [resolvedCars, carQuery])

  useEffect(() => {
    function onPointerDown(e: MouseEvent) {
      if (rootCarRef.current && e.target instanceof Node && !rootCarRef.current.contains(e.target))
        setCarOpen(false)
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
            setCarOpen(false)

            if (!selectedCar?.id) {
              toast.error('Vui lòng chọn dòng xe bạn quan tâm.')
              return
            }
            if (!selectedShowroomId) {
              toast.error('Vui lòng chọn showroom.')
              return
            }

            const fd = new FormData(e.currentTarget)
            const fullName = String(fd.get('fullName') ?? '').trim()
            const phone = String(fd.get('phone') ?? '').trim()
            const mode = String(fd.get('mode') ?? defaultMode) as QuoteRegisterMode

            onSubmit?.({ mode, fullName, phone, carId: selectedCar.id, carName: selectedCar.label })

            const now = new Date()
            const bookingDate = now.toISOString().split('T')[0]
            const bookingTime = now.toTimeString().slice(0, 5)
            const modeLabel = mode === 'installment' ? 'Trả góp' : 'Trả thẳng'
            const showroomName = showrooms.find(s => String(s.showroomId) === selectedShowroomId)?.showroomName ?? ''
            const note = `Tư vấn báo giá xe ${selectedCar.label} - ${modeLabel} - ${showroomName}`

            const normalizedPhone = phone.startsWith('+84')
              ? '0' + phone.slice(3)
              : phone

            try {
              setSubmitting(true)
              await http.post('/api/Bookings/create', {
                carId: Number(selectedCar.id),
                showroomId: Number(selectedShowroomId),
                customerName: fullName,
                phone: normalizedPhone,
                bookingDate,
                bookingTime,
                timeSpan: '30 phút',
                note,
                userId: 0,
              })
              toast.success('Đăng ký thành công! Nhân viên sẽ liên hệ tư vấn cho bạn sớm nhất.')
              setSelectedCar(null)
              setCarQuery('')
              setCarTouched(false)
              setSelectedShowroomId('')
              ;(e.target as HTMLFormElement).reset()
            } catch {
              toast.error('Có lỗi xảy ra, vui lòng thử lại hoặc liên hệ hotline.')
            } finally {
              setSubmitting(false)
            }
          }}
        >
          {/* Hình thức */}
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

          {/* Họ tên */}
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

          {/* Điện thoại */}
          <div>
            <label htmlFor={phoneId} className="sr-only">Điện thoại</label>
            <input
              id={phoneId}
              name="phone"
              required
              inputMode="tel"
              pattern="^(\+84|0)[0-9]{9,10}$"
              maxLength={13}
              placeholder="Số điện thoại (VD: 09... hoặc +84...)"
              className="h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm outline-none transition focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40"
              autoComplete="tel"
            />
          </div>

          {/* Chọn xe */}
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
                  className="absolute bottom-full mb-2 z-50 max-h-64 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 text-sm shadow-lg"
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

          {/* Chọn Showroom — hiện ra sau khi chọn xe, dropdown mở lên trên */}
          {selectedCar && (
            <div className="relative">
              <button
                type="button"
                onClick={() => !isShowroomsLoading && setShowroomOpen(v => !v)}
                className={[
                  'h-11 w-full rounded-lg border bg-white px-3 pr-11 text-sm shadow-sm outline-none transition text-left',
                  !selectedShowroomId ? 'text-slate-400' : 'text-slate-900',
                  'border-slate-200 focus-visible:border-rose-300 focus-visible:ring-2 focus-visible:ring-rose-400/40',
                  isShowroomsLoading ? 'opacity-60 cursor-not-allowed' : '',
                ].join(' ')}
              >
                {isShowroomsLoading
                  ? 'Đang tải showroom...'
                  : selectedShowroomId
                  ? showrooms.find(s => String(s.showroomId) === selectedShowroomId)?.showroomName ?? '== Chọn showroom =='
                  : '== Chọn showroom =='}
                <span className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400">
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                    <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                  </svg>
                </span>
              </button>

              {showroomOpen && (
                <div className="absolute bottom-full mb-2 z-50 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 text-sm shadow-lg max-h-48">
                  {showrooms.map((s) => {
                    const isSelected = String(s.showroomId) === selectedShowroomId
                    return (
                      <button
                        key={s.showroomId}
                        type="button"
                        className={[
                          'flex w-full items-center justify-between px-3 py-2 text-left',
                          isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50',
                        ].join(' ')}
                        onClick={() => { setSelectedShowroomId(String(s.showroomId)); setShowroomOpen(false) }}
                      >
                        <span className="truncate">{s.showroomName}</span>
                        {isSelected && <span className="ml-3 text-[11px] font-semibold text-rose-500">Đã chọn</span>}
                      </button>
                    )
                  })}
                </div>
              )}
            </div>
          )}

          {/* Submit */}
          <button
            type="submit"
            disabled={submitting}
            className="inline-flex h-11 w-full cursor-pointer items-center justify-center rounded-lg bg-rose-500 px-4 text-sm font-extrabold tracking-wide text-white shadow-sm transition hover:bg-rose-600 focus:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40 disabled:opacity-60"
          >
            {submitting ? 'Đang gửi...' : submitLabel}
          </button>
        </form>
      </div>
    </section>
  )
}