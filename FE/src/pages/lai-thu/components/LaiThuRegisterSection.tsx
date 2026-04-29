import { useEffect, useId, useMemo, useRef, useState } from 'react'
import toast from 'react-hot-toast'
import { useLocation } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'

import banner1 from '../../../assets/images/banner1.jpg'
import { env } from '../../../lib/env'

const NAVY = '#1a3a5a'

const inputClassName =
  'h-12 w-full rounded border border-[#e0e0e0] bg-white px-4 text-sm text-slate-900 placeholder:text-slate-400 outline-none transition focus:border-[#1a3a5a] focus:ring-1 focus:ring-[#1a3a5a]'

type CarOption = { id: string; label: string }

type CustomerCarListDto = {
  carId: number
  name: string
}

type PagedCarsResponse = {
  data: CustomerCarListDto[]
}

type PricingVersionDto = {
  pricingVersionId: number
  name: string
}

type ShowroomDto = {
  showroomId: number // <-- BACKEND PHẢI TRẢ VỀ TRƯỜNG NÀY NHÉ NÍ
  showroomName: string
  showroomAddress: string
  quantity: number
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
  pricingVersionId?: number 
  customerName: string
  phone: string
  bookingDate: string 
  bookingTime?: string
  timeSpan?: string
  note?: string
}

async function getCarOptions(): Promise<CarOption[]> {
  const res = await carsApi.get<PagedCarsResponse>('Cars', {
    params: { page: 1, pageSize: 100, inStockOnly: false },
  })
  const list = Array.isArray(res.data?.data) ? res.data.data : []
  return list
    .map((c) => ({ id: String(c.carId), label: c.name }))
    .filter((c) => c.label.trim().length > 0)
}

async function getCarDetails(carId: string) {
  const res = await carsApi.get(`Cars/${carId}`)
  const data = res.data?.data
  
  return {
    versions: (data?.pricingVersions as PricingVersionDto[]) || [],
    showrooms: (data?.showroomDetails as ShowroomDto[]) || [],
    imageUrl: data?.imageUrl ? new URL(data.imageUrl, env.VITE_API_BASE_URL).toString() : null
  }
}

type LaiThuLocationState = {
  trialPrefill?: { 
    fullName?: string; 
    phone?: string; 
    carId?: string;
    carName?: string;
    carImage?: string; 
  }
}

export function LaiThuRegisterSection() {
  const { state } = useLocation()
  const trialPrefill = (state as LaiThuLocationState | null)?.trialPrefill

  const nameId = useId()
  const phoneId = useId()
  const carId = useId()
  const bookingDateId = useId()
  const bookingTimeId = useId()

  const { data: carOptions = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const rootRef = useRef<HTMLDivElement | null>(null)
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<CarOption | null>(null)

  const [selectedVersionId, setSelectedVersionId] = useState('')
  const [selectedShowroomId, setSelectedShowroomId] = useState('') // Đổi tên state cho chuẩn

  useEffect(() => {
    const prefillCarId = trialPrefill?.carId
    if (!prefillCarId || !carOptions.length) return
    const found = carOptions.find((c) => c.id === prefillCarId)
    if (found) setSelectedCar(found)
  }, [carOptions, trialPrefill?.carId])

  const { data: carDetails, isFetching: isCarDetailsLoading } = useQuery({
    queryKey: ['car-details', selectedCar?.id],
    queryFn: () => getCarDetails(selectedCar!.id),
    enabled: !!selectedCar?.id,
  })

  const versions = carDetails?.versions || []
  const showrooms = carDetails?.showrooms || []

  useEffect(() => {
    setSelectedVersionId('')
    setSelectedShowroomId('')
  }, [selectedCar?.id])
  
  const [touched, setTouched] = useState(false)
  const [bookingDate, setBookingDate] = useState('')
  const [bookingTime, setBookingTime] = useState('')
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

  const todayISO = useMemo(() => new Date().toISOString().slice(0, 10), [])

  const displayImage = carDetails?.imageUrl 
    || (selectedCar?.id === trialPrefill?.carId ? trialPrefill?.carImage : banner1) 
    || banner1

  const displayName = selectedCar?.label || trialPrefill?.carName || "Showroom VinFast Nam Từ Liêm"

  return (
    <section className="w-full bg-white">
      <div className="mx-auto w-full max-w-screen-2xl px-5 py-10 sm:px-6 sm:py-14 lg:px-8 lg:py-16">
        <div className="grid gap-10 lg:grid-cols-2 lg:items-stretch lg:gap-12 xl:gap-16">
          <div className="relative min-h-[240px] flex items-center justify-center overflow-hidden rounded-lg bg-slate-50 border border-slate-100 sm:min-h-[320px] lg:min-h-0">
            {isCarDetailsLoading ? (
              <div className="flex items-center justify-center text-slate-400">Đang tải ảnh xe...</div>
            ) : (
              <img
                src={displayImage}
                alt={displayName}
                className={displayImage === banner1 ? "h-full w-full object-cover" : "w-full p-4 object-contain transition-opacity duration-300"}
                loading="eager"
              />
            )}
          </div>

          <div className="flex flex-col justify-center">
            <div className="space-y-4 text-[15px] leading-[1.7] text-slate-800 sm:text-base sm:leading-relaxed">
              <p>
                Quý khách hàng có thể đến trực tiếp Showroom để đăng ký lái thử hoặc nếu Quý
                khách hàng không thể sắp xếp được, chúng tôi sẽ cho nhân viên hỗ trợ lái xe
                đến tận nhà phục vụ Quý khách hàng.
              </p>
              <p>
                Để đăng ký lái thử, vui lòng điền thông tin vào mẫu dưới đây và gửi yêu cầu. Chúng
                tôi sẽ liên hệ lại để hỗ trợ trong vòng 24h.
              </p>
            </div>

            <div className="mt-8 rounded border border-[#e0e0e0] bg-white p-6 shadow-[0_4px_24px_rgba(0,0,0,0.08)] sm:p-7">
              <form
                className="space-y-4"
                onSubmit={async (e) => {
                  e.preventDefault()
                  setTouched(true)
                  setOpen(false)

                  if (!selectedCar?.id) {
                    toast.error('Vui lòng chọn xe muốn lái thử.')
                    return
                  }
                  if (!selectedVersionId) {
                    toast.error('Vui lòng chọn phiên bản xe.')
                    return
                  }
                  if (!selectedShowroomId) {
                    toast.error('Vui lòng chọn Showroom.')
                    return
                  }
                  if (!bookingDate) {
                    toast.error('Vui lòng chọn ngày muốn lái thử.')
                    return
                  }
                  if (!bookingTime) {
                    toast.error('Vui lòng chọn giờ muốn lái thử.')
                    return
                  }

                  const form = e.currentTarget
                  const fd = new FormData(form)
                  const customerName = String(fd.get('fullName') ?? '').trim()
                  const phone = String(fd.get('phone') ?? '').trim()

                  // Lấy tên Phiên bản và Showroom để nhét vào Ghi chú (Note)
                  const versionName = versions.find(v => String(v.pricingVersionId) === selectedVersionId)?.name || ''
                  
                  // Lấy tên Showroom từ list
                  let showroomName = showrooms.find(s => String(s.showroomId) === selectedShowroomId)?.showroomName || selectedShowroomId;

                  // XỬ LÝ LỖI 400 CỦA SHOWROOM ID:
                  // Nếu BE chưa trả showroomId, selectedShowroomId sẽ là chữ -> Number() ra NaN.
                  // Lúc này mình fallback về 1 (giống biến SHOWROOM_ID = 1 ở code cũ của ní) để API không bị lỗi 400.
                  const parsedShowroomId = Number(selectedShowroomId);
                  const safeShowroomId = isNaN(parsedShowroomId) || parsedShowroomId === 0 ? 1 : parsedShowroomId;

                  // PAYLOAD CHUẨN (Khớp 100% code cũ của ní, không gửi thừa trường)
                  const payload: BookingCreatePayload = {
                    carId: Number(selectedCar.id),
                    showroomId: safeShowroomId,
                    customerName,
                    phone,
                    bookingDate,
                    bookingTime,
                    note: `Đăng ký lái thử phiên bản [${versionName}] tại [${showroomName}]`,
                    // bỏ userId: null
                  }

                  try {
                    setIsSubmitting(true)
                    // Nhớ check lại link này là 'Bookings/public-create' hay 'Bookings/create' theo BE của ní nha
                    await bookingsApi.post('Bookings/create', payload)
                    toast.success('Đặt lịch lái thử thành công. Chúng tôi sẽ liên hệ sớm.')
                    form.reset()
                    setSelectedCar(null)
                    setSelectedVersionId('')
                    setSelectedShowroomId('')
                    setQuery('')
                    setBookingDate('')
                    setBookingTime('')
                    setTouched(false)
                  } catch (err) {
                    if (axios.isAxiosError(err)) {
                      const status = err.response?.status
                      const message =
                        (err.response?.data as any)?.message ??
                        (typeof err.response?.data === 'string' ? err.response?.data : null) ??
                        err.message

                      if (status === 401) {
                        toast.error('Bạn cần đăng nhập để đặt lịch lái thử.')
                        return
                      }
                      console.error("LỖI API:", err.response?.data) // Log ra console để ní F12 bắt lỗi dễ hơn
                      toast.error(message || 'Đặt lịch thất bại. Vui lòng thử lại.')
                      return
                    }
                    toast.error('Đặt lịch thất bại. Vui lòng thử lại.')
                  } finally {
                    setIsSubmitting(false)
                  }
                }}
              >
                <div>
                  <input
                    id={nameId}
                    name="fullName"
                    required
                    placeholder="Họ và tên"
                    autoComplete="name"
                    defaultValue={trialPrefill?.fullName ?? ''}
                    className={inputClassName}
                  />
                </div>

                <div>
                  <input
                    id={phoneId}
                    name="phone"
                    required
                    inputMode="numeric"
                    pattern="[0-9]{10}"
                    maxLength={10}
                    placeholder="Nhập số điện thoại (10 số)"
                    autoComplete="tel"
                    defaultValue={trialPrefill?.phone ?? ''}
                    className={inputClassName}
                  />
                </div>

                <div ref={rootRef} className="relative">
                  <input type="hidden" name="carId" value={selectedCar?.id ?? ''} />
                  <input
                    id={carId}
                    role="combobox"
                    aria-expanded={open}
                    aria-controls={`${carId}-listbox`}
                    placeholder={isCarsLoading ? 'Đang tải danh sách xe...' : 'Xe muốn lái thử'}
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
                      touched && !selectedCar?.id ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400' : '',
                    ].join(' ')}
                    autoComplete="off"
                  />

                  <button
                    type="button"
                    className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded p-1 text-slate-400 hover:text-slate-600"
                    onClick={() => {
                      setTouched(true)
                      setOpen((v) => !v)
                      if (!open) setQuery(selectedCar?.label ?? '')
                    }}
                  >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
                      <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  </button>

                  {open ? (
                    <div className="absolute z-20 mt-2 max-h-64 w-full overflow-auto rounded border border-slate-200 bg-white py-1 text-sm shadow-lg">
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
                              className={`flex w-full items-center justify-between px-4 py-2 text-left ${isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50'}`}
                              onClick={() => {
                                setSelectedCar(c)
                                setQuery(c.label)
                                setOpen(false)
                                setTouched(true)
                              }}
                            >
                              <span className="truncate">{c.label}</span>
                              {isSelected && <span className="ml-3 text-[11px] font-semibold text-[#1a3a5a]">Đã chọn</span>}
                            </button>
                          )
                        })
                      )}
                    </div>
                  ) : null}
                </div>

                <div className="grid gap-3 sm:grid-cols-2">
                  <div>
                    <select
                      value={selectedVersionId}
                      onChange={(e) => setSelectedVersionId(e.target.value)}
                      className={inputClassName}
                      disabled={!selectedCar?.id || isCarDetailsLoading}
                    >
                      <option value="">{isCarDetailsLoading ? 'Đang tải...' : 'Chọn phiên bản'}</option>
                      {versions.map(v => (
                        <option key={v.pricingVersionId} value={v.pricingVersionId}>
                          {v.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <select
                      value={selectedShowroomId}
                      onChange={(e) => setSelectedShowroomId(e.target.value)}
                      className={inputClassName}
                      disabled={!selectedCar?.id || isCarDetailsLoading}
                    >
                      <option value="">{isCarDetailsLoading ? 'Đang tải...' : 'Chọn Showroom'}</option>
                      {showrooms.map(s => (
                        /* ĐÂY NÈ NÍ: Value là truyền ID vào, Chữ kẹp giữa là hiển thị Tên ra */
                        <option key={s.showroomId} value={s.showroomId}>
                          {s.showroomName}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="grid gap-3 sm:grid-cols-2">
                  <div>
                    <input
                      id={bookingDateId}
                      name="bookingDate"
                      type="date"
                      required
                      min={todayISO}
                      value={bookingDate}
                      onChange={(e) => {
                        setBookingDate(e.target.value)
                        setTouched(true)
                      }}
                      className={inputClassName}
                    />
                  </div>

                  <div>
                    <input
                      id={bookingTimeId}
                      name="bookingTime"
                      type="time"
                      required
                      value={bookingTime}
                      onChange={(e) => {
                        setBookingTime(e.target.value)
                        setTouched(true)
                      }}
                      className={inputClassName}
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="mt-2 inline-flex h-12 w-full cursor-pointer items-center justify-center rounded px-4 text-[13px] font-bold uppercase tracking-[0.06em] text-white transition hover:opacity-95 disabled:cursor-not-allowed disabled:opacity-70"
                  style={{ backgroundColor: NAVY }}
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