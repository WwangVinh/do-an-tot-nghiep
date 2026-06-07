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

const LOAN_TERM_OPTIONS = [
  { value: 12, label: '12 tháng (1 năm)' },
  { value: 24, label: '24 tháng (2 năm)' },
  { value: 36, label: '36 tháng (3 năm)' },
  { value: 48, label: '48 tháng (4 năm)' },
  { value: 60, label: '60 tháng (5 năm)' },
  { value: 72, label: '72 tháng (6 năm)' },
  { value: 84, label: '84 tháng (7 năm)' },
  { value: 96, label: '96 tháng (8 năm)' },
] as const

const inputClassName =
  `h-[52px] w-full rounded-xl border border-slate-200 bg-white px-4 text-[15px] text-slate-900 shadow-[inset_0_1px_0_rgba(255,255,255,0.9)] placeholder:text-slate-400 outline-none transition ` +
  `hover:border-slate-300 focus:border-[#243A5E] focus:ring-2 focus:ring-[#243A5E]/20`

type CarOption = { id: string; label: string; image?: string; price?: string }

type CustomerCarListDto = {
  carId: number
  name: string
  imageUrl?: string
  price?: number | string
}
type PagedCarsResponse = { data: CustomerCarListDto[] }

type ShowroomDto = {
  showroomId: number
  name?: string
  showroomName?: string
  fullAddress?: string
}

// ✨ Type cho phiên bản và màu xe
type PricingVersionDto = {
  pricingVersionId: number
  versionName: string
  priceVnd: number
  isActive?: boolean
}

type CarColorDto = {
  carColorId: number
  colorName: string
  hexCode?: string
  imageUrl?: string
  isActive?: boolean
}

type ConsultRequestCreatePayload = {
  carId: number
  showroomId: number
  customerName: string
  phone: string
  requestType: 'Installment'
  customerNote?: string
  monthlyIncome: number
  downPayment: number
  loanTermMonths: number
  carPricingVersionId?: number
  carColorId?: number
}

const carsApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

const consultApi = axios.create({
  baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
  timeout: 20_000,
})

async function getCarOptions(): Promise<CarOption[]> {
  const res = await carsApi.get<PagedCarsResponse>('Cars', {
    params: { page: 1, pageSize: 200, inStockOnly: false },
  })
  const list = Array.isArray(res.data?.data) ? res.data.data : []

  return list
    .map((c) => {
      const fullImageUrl = c.imageUrl
        ? new URL(c.imageUrl, env.VITE_API_BASE_URL).toString()
        : undefined

      const formattedPrice = typeof c.price === 'number'
        ? new Intl.NumberFormat('vi-VN').format(c.price)
        : c.price

      return {
        id: String(c.carId),
        label: c.name,
        image: fullImageUrl,
        price: formattedPrice ? String(formattedPrice) : undefined,
      }
    })
    .filter((c) => c.label.trim().length > 0)
}

async function getCarDetails(carId: string) {
  const res = await carsApi.get(`Cars/${carId}`)
  const data = res.data?.data
  return {
    imageUrl: data?.imageUrl ? new URL(data.imageUrl, env.VITE_API_BASE_URL).toString() : null,
  }
}

async function getCarShowrooms(carId: string): Promise<ShowroomDto[]> {
  try {
    const res = await carsApi.get(`public/cars/${carId}/showrooms`, {
      headers: { 'ngrok-skip-browser-warning': 'true' },
    })
    const data = res.data?.data ?? res.data
    if (Array.isArray(data) && data.length > 0) return data
  } catch { /* fallback */ }

  try {
    const res = await carsApi.get(`Cars/${carId}`)
    const data = res.data?.data ?? res.data
    const showroomDetails = data?.showroomDetails ?? data?.ShowroomDetails ?? []
    if (Array.isArray(showroomDetails) && showroomDetails.length > 0) return showroomDetails
  } catch { /* silent */ }

  try {
    const res = await carsApi.get('public/orders/showrooms', {
      headers: { 'ngrok-skip-browser-warning': 'true' },
    })
    return res.data?.data ?? res.data ?? []
  } catch { return [] }
}

// ✨ Lấy phiên bản từ /Cars/{id} (BE đã trả pricingVersions trong response)
async function getCarPricingVersions(carId: string): Promise<PricingVersionDto[]> {
  try {
    const res = await carsApi.get(`Cars/${carId}`)
    const data = res.data?.data ?? res.data
    const versions =
      data?.pricingVersions ??
      data?.PricingVersions ??
      data?.carPricingVersions ??
      data?.CarPricingVersions ??
      []
    if (Array.isArray(versions)) {
      return versions
        .filter((v: any) => v && (v.isActive === undefined || v.isActive === true))
        .map((v: any) => ({
          pricingVersionId: v.pricingVersionId ?? v.PricingVersionId,
          versionName: v.versionName ?? v.VersionName ?? '',
          priceVnd: v.priceVnd ?? v.PriceVnd ?? 0,
          isActive: v.isActive ?? v.IsActive ?? true,
        }))
        .sort((a, b) => a.priceVnd - b.priceVnd)
    }
  } catch { /* silent */ }
  return []
}

// ✨ Lấy danh sách màu từ /Cars/{id}
async function getCarColors(carId: string): Promise<CarColorDto[]> {
  try {
    const res = await carsApi.get(`Cars/${carId}`)
    const data = res.data?.data ?? res.data
    const colors =
      data?.colors ??
      data?.Colors ??
      data?.carColors ??
      data?.CarColors ??
      []
    if (Array.isArray(colors)) {
      return colors
        .filter((c: any) => c && (c.isActive === undefined || c.isActive === true))
        .map((c: any) => ({
          carColorId: c.carColorId ?? c.CarColorId,
          colorName: c.colorName ?? c.ColorName ?? '',
          hexCode: c.hexCode ?? c.HexCode,
          imageUrl: c.imageUrl ?? c.ImageUrl,
          isActive: c.isActive ?? c.IsActive ?? true,
        }))
    }
  } catch { /* silent */ }
  return []
}

function getShowroomName(s: ShowroomDto): string {
  return s.name ?? s.showroomName ?? `Showroom #${s.showroomId}`
}

function parseMoney(input: string): number {
  const digits = input.replace(/[^\d]/g, '')
  return digits ? Number(digits) : 0
}

function formatMoney(value: number | string): string {
  const num = typeof value === 'number' ? value : parseMoney(value)
  if (!num) return ''
  return new Intl.NumberFormat('vi-VN').format(num)
}

export function InstallmentRegisterSection() {
  const nameId = useId()
  const phoneId = useId()
  const carSelectId = useId()
  const showroomSelectId = useId()
  const versionId = useId()
  const colorId = useId()
  const incomeId = useId()
  const downPaymentId = useId()
  const loanTermId = useId()

  const { data: carOptionsApi = [], isLoading: isCarsLoading } = useQuery({
    queryKey: ['cars', 'select-options', 'installment'],
    queryFn: getCarOptions,
    staleTime: 5 * 60_000,
  })

  const carOptions = useMemo<CarOption[]>(() => {
    if (carOptionsApi.length > 0) return carOptionsApi
    return INSTALLMENT_CAR_OPTIONS
  }, [carOptionsApi])

  const rootCarRef = useRef<HTMLDivElement | null>(null)
  const [carOpen, setCarOpen] = useState(false)
  const [carQuery, setCarQuery] = useState('')
  const [selectedCar, setSelectedCar] = useState<CarOption | null>(null)
  const [carTouched, setCarTouched] = useState(false)

  const filteredCars = useMemo(() => {
    const q = carQuery.trim().toLowerCase()
    if (!q) return carOptions
    return carOptions.filter((c) => c.label.toLowerCase().includes(q))
  }, [carOptions, carQuery])

  useEffect(() => {
    function onPointerDown(e: MouseEvent) {
      if (rootCarRef.current && e.target instanceof Node && !rootCarRef.current.contains(e.target))
        setCarOpen(false)
    }
    document.addEventListener('mousedown', onPointerDown)
    return () => document.removeEventListener('mousedown', onPointerDown)
  }, [])

  const { data: carDetails, isFetching: isCarDetailsLoading } = useQuery({
    queryKey: ['car-details-installment', selectedCar?.id],
    queryFn: () => getCarDetails(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  const carImageUrl = useMemo(() => {
    if (carDetails?.imageUrl) return carDetails.imageUrl
    if (selectedCar?.image) return selectedCar.image
    return '/default-car.jpg'
  }, [carDetails, selectedCar])

  const rootShowroomRef = useRef<HTMLDivElement | null>(null)
  const [showroomOpen, setShowroomOpen] = useState(false)
  const [selectedShowroomId, setSelectedShowroomId] = useState('')

  const { data: showrooms = [], isFetching: isShowroomsLoading } = useQuery({
    queryKey: ['car-showrooms-installment', selectedCar?.id],
    queryFn: () => getCarShowrooms(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  // ✨ Query phiên bản & màu xe theo xe đã chọn
  const { data: pricingVersions = [], isFetching: isVersionsLoading } = useQuery({
    queryKey: ['car-pricing-versions', selectedCar?.id],
    queryFn: () => getCarPricingVersions(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  const { data: carColors = [], isFetching: isColorsLoading } = useQuery({
    queryKey: ['car-colors', selectedCar?.id],
    queryFn: () => getCarColors(selectedCar!.id),
    enabled: !!selectedCar?.id,
    staleTime: 5 * 60_000,
  })

  const [selectedVersionId, setSelectedVersionId] = useState<number | ''>('')
  const [selectedColorId, setSelectedColorId] = useState<number | ''>('')

  // Reset khi đổi xe
  useEffect(() => {
    setSelectedShowroomId('')
    setShowroomOpen(false)
    setSelectedVersionId('')
    setSelectedColorId('')
  }, [selectedCar?.id])

  // Auto-select phiên bản rẻ nhất + màu đầu khi xe vừa load xong
  useEffect(() => {
    if (pricingVersions.length > 0 && !selectedVersionId) {
      setSelectedVersionId(pricingVersions[0].pricingVersionId)
    }
  }, [pricingVersions, selectedVersionId])

  useEffect(() => {
    if (carColors.length > 0 && !selectedColorId) {
      setSelectedColorId(carColors[0].carColorId)
    }
  }, [carColors, selectedColorId])

  useEffect(() => {
    function onPointerDown(e: MouseEvent) {
      if (rootShowroomRef.current && e.target instanceof Node && !rootShowroomRef.current.contains(e.target))
        setShowroomOpen(false)
    }
    document.addEventListener('mousedown', onPointerDown)
    return () => document.removeEventListener('mousedown', onPointerDown)
  }, [])

  const [monthlyIncomeInput, setMonthlyIncomeInput] = useState('')
  const [downPaymentInput, setDownPaymentInput] = useState('')
  const [loanTermMonths, setLoanTermMonths] = useState<number | ''>('')

  const [isSubmitting, setIsSubmitting] = useState(false)
  const [touched, setTouched] = useState(false)

  const selectedShowroomName = useMemo(
    () => showrooms.find((s) => String(s.showroomId) === selectedShowroomId),
    [showrooms, selectedShowroomId],
  )

  const selectedVersion = useMemo(
    () => pricingVersions.find((v) => v.pricingVersionId === selectedVersionId),
    [pricingVersions, selectedVersionId],
  )

  const selectedColor = useMemo(
    () => carColors.find((c) => c.carColorId === selectedColorId),
    [carColors, selectedColorId],
  )

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

          {/* ── Cột trái ── */}
          <div className="relative min-w-0 pl-5 before:absolute before:left-0 before:top-0 before:bottom-0 before:w-1 before:rounded-full before:bg-gradient-to-b before:from-[#243A5E] before:via-[#4a6494] before:to-[#8B1D1D] before:content-[''] before:shadow-[2px_0_12px_rgba(36,58,94,0.2)] sm:pl-7">
            <p className="inline-flex items-center gap-2 rounded-full border border-white/80 bg-white/95 px-3.5 py-2 text-[10px] font-semibold uppercase tracking-[0.2em] text-[#243A5E] shadow-[0_2px_12px_rgba(36,58,94,0.08)] backdrop-blur-sm sm:text-[11px]">
              <span className="flex h-2 w-2 animate-pulse rounded-full bg-[#8B1D1D]" aria-hidden />
              CMC AUTOMOTIVE <span className="font-normal text-slate-300">•</span> Đại lý phân phối toàn quốc
            </p>

            <h1 className="mt-6 text-[1.85rem] font-extrabold leading-[1.15] tracking-tight sm:text-4xl lg:text-[2.45rem]">
              <span style={{ color: NAVY }}>Mua xe </span>
              <span className="bg-gradient-to-r from-[#243A5E] via-[#1e5080] to-[#8B1D1D] bg-clip-text text-transparent">
                trả góp
              </span>
            </h1>
            <div className="mt-3 h-1 w-16 rounded-full bg-gradient-to-r from-[#243A5E] to-[#8B1D1D] shadow-sm sm:w-20" aria-hidden />

            <p className="mt-5 max-w-xl text-[15px] leading-relaxed text-slate-600 sm:text-base">
              Tư vấn gói vay phù hợp, thủ tục rõ ràng — đồng hành cùng bạn từ đăng ký đến khi nhận xe.
            </p>

            <div className="mt-6 flex flex-wrap gap-3 sm:gap-4">
              {[
                { value: '85%', label: 'Tối đa vay', color: FORM_TITLE_RED },
                { value: '8', label: 'Năm vay', color: NAVY },
                { value: '24h', label: 'Duyệt hồ sơ', color: NAVY },
              ].map(({ value, label, color }) => (
                <div key={label} className="flex min-w-[5.5rem] flex-1 flex-col rounded-xl border border-[#243A5E]/15 bg-gradient-to-br from-white to-slate-50/80 px-3 py-2.5 text-center shadow-[0_4px_14px_rgba(36,58,94,0.08)] sm:min-w-[6rem] sm:px-4 sm:py-3">
                  <span className="text-lg font-extrabold tabular-nums sm:text-xl" style={{ color }}>{value}</span>
                  <span className="mt-0.5 text-[10px] font-medium uppercase tracking-wide text-slate-500 sm:text-[11px]">{label}</span>
                </div>
              ))}
            </div>

            <ul className="mt-7 flex flex-wrap gap-3">
              {featurePills.map(({ Icon, label, highlight }) => (
                <li key={label} className={[
                  'inline-flex items-center gap-2.5 rounded-full px-4 py-2.5 text-[13px] font-semibold transition sm:text-sm',
                  highlight
                    ? 'border-2 border-[#243A5E]/35 bg-gradient-to-r from-[#243A5E]/[0.08] to-[#8B1D1D]/[0.06] text-[#243A5E] shadow-[0_4px_16px_rgba(36,58,94,0.12)] ring-1 ring-[#243A5E]/10'
                    : 'border border-slate-200 bg-white/95 text-slate-700 shadow-sm hover:border-slate-300 hover:shadow-md',
                ].join(' ')}>
                  <Icon className="h-4 w-4 shrink-0 text-[#243A5E]" strokeWidth={2} aria-hidden />
                  {label}
                </li>
              ))}
            </ul>

            {selectedCar && (
              <div className="mt-8 overflow-hidden rounded-2xl border border-slate-200 bg-white p-4 shadow-[0_4px_20px_rgba(36,58,94,0.05)] transition-all duration-300">
                <p className="text-xs font-bold uppercase tracking-wider text-slate-400 mb-2">Xe đang chọn lựa:</p>
                <div className="flex flex-col sm:flex-row items-center gap-5">
                  <div className="relative h-28 w-44 shrink-0 overflow-hidden rounded-xl bg-slate-50 border border-slate-100 flex items-center justify-center">
                    {isCarDetailsLoading ? (
                      <div className="flex items-center justify-center text-xs text-slate-400 animate-pulse">Đang tải ảnh...</div>
                    ) : (
                      <img
                        src={carImageUrl}
                        alt={selectedCar.label}
                        className="h-full w-full object-contain p-2 transition-all duration-300"
                        onError={(e) => {
                          e.currentTarget.src = '/default-car.jpg'
                        }}
                      />
                    )}
                  </div>
                  <div className="text-center sm:text-left">
                    <h3 className="text-lg font-bold text-[#243A5E]">{selectedCar.label}</h3>
                    {selectedVersion && (
                      <p className="mt-1 text-sm font-semibold text-[#8B1D1D]">
                        Bản {selectedVersion.versionName}: {formatMoney(selectedVersion.priceVnd)} ₫
                      </p>
                    )}
                    {selectedColor && (
                      <p className="mt-1 flex items-center justify-center gap-1.5 text-xs text-slate-600 sm:justify-start">
                        Màu:
                        {selectedColor.hexCode && (
                          <span
                            className="inline-block h-3.5 w-3.5 rounded-full border border-slate-300"
                            style={{ backgroundColor: selectedColor.hexCode }}
                          />
                        )}
                        <span className="font-medium">{selectedColor.colorName}</span>
                      </p>
                    )}
                  </div>
                </div>
              </div>
            )}

            <div className="mt-9 max-w-[34rem] space-y-4 text-[15px] leading-[1.8] text-slate-600 sm:text-base">
              <p>
                Là trả trước một phần tiền mua xe, phần còn thiếu sẽ vay ngân hàng rồi hàng tháng trả dần cho ngân hàng cả gốc và lãi theo phương thức{' '}
                <strong className="font-semibold text-slate-800">trừ lùi</strong> trong suốt thời gian trả góp.
              </p>
              <p>
                Hỗ trợ tư vấn mua xe đại lý phân phối toàn quốc trả góp tới{' '}
                <strong className="font-semibold text-[#8B1D1D]">85% giá trị xe</strong>, thời gian vay tối đa{' '}
                <strong className="font-semibold text-[#243A5E]">8 năm</strong>. Thủ tục đơn giản nhanh gọn, thời gian thẩm duyệt trong vòng{' '}
                <strong className="rounded-md bg-[#243A5E]/[0.1] px-1.5 py-0.5 font-semibold text-[#243A5E]">24h</strong>
                , kể cả khách hàng ở tỉnh, bao đậu hồ sơ khó. Vui lòng liên hệ để được tư vấn chính xác.
              </p>
            </div>
          </div>

          {/* ── Cột phải: Form ── */}
          <div className="relative flex min-w-0 w-full justify-center lg:justify-end">
            <div
              className="pointer-events-none absolute left-1/2 top-1/2 -z-0 h-[min(100%,380px)] w-[min(100%,340px)] -translate-x-1/2 -translate-y-1/2 rounded-[2rem] bg-gradient-to-br from-[#243A5E]/25 via-[#8B1D1D]/10 to-transparent opacity-90 blur-2xl lg:left-auto lg:right-0 lg:translate-x-0"
              aria-hidden
            />

            <div className="relative z-10 w-full max-w-[min(100%,480px)] rounded-2xl border border-white/90 bg-white/95 shadow-[0_8px_40px_rgba(15,23,42,0.1),0_0_0_1px_rgba(36,58,94,0.06)] backdrop-blur-sm lg:max-w-none">
              <div className="relative overflow-hidden rounded-t-2xl">
                <div className="h-[6px] w-full bg-gradient-to-r from-[#243A5E] from-[5%] via-[#5a7199] via-50% to-[#8B1D1D] to-[98%]" aria-hidden />
                <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-white/50 to-transparent" aria-hidden />
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
                  Điền thông tin — chúng tôi gọi lại tư vấn gói vay phù hợp trong thời gian sớm nhất.
                </p>

                <form
                  className="mt-8 space-y-4"
                  onSubmit={async (e) => {
                    e.preventDefault()
                    setTouched(true)
                    setCarTouched(true)
                    setCarOpen(false)
                    setShowroomOpen(false)

                    if (!selectedCar?.id) {
                      toast.error('Vui lòng chọn xe muốn mua.')
                      return
                    }
                    if (!selectedShowroomId) {
                      toast.error('Vui lòng chọn showroom.')
                      return
                    }

                    const numericCarId = Number(selectedCar.id)
                    if (!Number.isFinite(numericCarId)) {
                      toast.error('Mã xe không hợp lệ. Vui lòng chọn xe từ danh sách.')
                      return
                    }

                    const monthlyIncome = parseMoney(monthlyIncomeInput)
                    const downPayment = parseMoney(downPaymentInput)

                    if (!monthlyIncome || monthlyIncome <= 0) {
                      toast.error('Vui lòng nhập thu nhập hàng tháng để tư vấn gói vay phù hợp.')
                      return
                    }
                    if (downPayment < 0) {
                      toast.error('Số tiền trả trước không hợp lệ.')
                      return
                    }
                    if (!loanTermMonths || loanTermMonths <= 0) {
                      toast.error('Vui lòng chọn kỳ hạn vay mong muốn.')
                      return
                    }

                    const form = e.currentTarget
                    const fd = new FormData(form)
                    const customerName = String(fd.get('fullName') ?? '').trim()
                    const rawPhone = String(fd.get('phone') ?? '').trim()

                    const normalizedPhone = rawPhone.startsWith('+84')
                      ? '0' + rawPhone.slice(3)
                      : rawPhone

                    const showroomNameStr = selectedShowroomName ? getShowroomName(selectedShowroomName) : ''

                    // Build note có thêm thông tin phiên bản + màu để Sales tiện theo dõi
                    const noteParts = [`Quan tâm xe ${selectedCar.label}`]
                    if (selectedVersion) noteParts.push(`bản ${selectedVersion.versionName}`)
                    if (selectedColor) noteParts.push(`màu ${selectedColor.colorName}`)
                    noteParts.push(`tại ${showroomNameStr}`)

                    const payload: ConsultRequestCreatePayload = {
                      carId: numericCarId,
                      showroomId: Number(selectedShowroomId),
                      customerName,
                      phone: normalizedPhone,
                      requestType: 'Installment',
                      customerNote: noteParts.join(' - '),
                      monthlyIncome,
                      downPayment,
                      loanTermMonths: Number(loanTermMonths),
                      carPricingVersionId: selectedVersionId || undefined,
                      carColorId: selectedColorId || undefined,
                    }

                    try {
                      setIsSubmitting(true)
                      await consultApi.post('consult-requests', payload)
                      toast.success('Đã gửi yêu cầu mua trả góp! Nhân viên sẽ liên hệ tư vấn gói vay sớm.')
                      form.reset()
                      setSelectedCar(null)
                      setCarQuery('')
                      setCarTouched(false)
                      setTouched(false)
                      setSelectedShowroomId('')
                      setSelectedVersionId('')
                      setSelectedColorId('')
                      setMonthlyIncomeInput('')
                      setDownPaymentInput('')
                      setLoanTermMonths('')
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
                  {/* Họ tên */}
                  <div>
                    <label htmlFor={nameId} className="sr-only">Họ và tên</label>
                    <input
                      id={nameId}
                      name="fullName"
                      required
                      placeholder="Họ và tên"
                      autoComplete="name"
                      className={inputClassName}
                    />
                  </div>

                  {/* SĐT */}
                  <div>
                    <label htmlFor={phoneId} className="sr-only">Số điện thoại</label>
                    <input
                      id={phoneId}
                      name="phone"
                      required
                      inputMode="tel"
                      pattern="^(\+84|0)[0-9]{9,10}$"
                      maxLength={13}
                      placeholder="Số điện thoại (VD: 09... hoặc +84...)"
                      autoComplete="tel"
                      className={inputClassName}
                    />
                  </div>

                  {/* Chọn xe */}
                  <div ref={rootCarRef} className="relative">
                    <label htmlFor={carSelectId} className="sr-only">Xe muốn mua</label>
                    <input type="hidden" name="carId" value={selectedCar?.id ?? ''} />
                    <input
                      id={carSelectId}
                      name="carSearch"
                      role="combobox"
                      aria-expanded={carOpen}
                      aria-controls={`${carSelectId}-listbox`}
                      placeholder={isCarsLoading ? 'Đang tải danh sách xe...' : 'Chọn xe muốn mua'}
                      value={carOpen ? carQuery : selectedCar?.label ?? ''}
                      onFocus={() => { setCarOpen(true); setCarQuery(selectedCar?.label ?? '') }}
                      onChange={(e) => { setCarOpen(true); setCarQuery(e.target.value); setCarTouched(true) }}
                      onBlur={() => setCarTouched(true)}
                      className={[
                        inputClassName,
                        'cursor-pointer pr-11 text-slate-800',
                        carTouched && !selectedCar?.id ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                      ].join(' ')}
                      autoComplete="off"
                    />
                    <button
                      type="button"
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 rounded-lg p-1 text-slate-400 hover:text-slate-600"
                      aria-label={carOpen ? 'Đóng danh sách xe' : 'Mở danh sách xe'}
                      onClick={() => { setCarTouched(true); setCarOpen((v) => !v); if (!carOpen) setCarQuery(selectedCar?.label ?? '') }}
                    >
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
                        <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                      </svg>
                    </button>

                    {carOpen && (
                      <div
                        id={`${carSelectId}-listbox`}
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
                                onMouseDown={(e) => {
                                  e.preventDefault()
                                  setSelectedCar(c)
                                  setCarQuery(c.label)
                                  setCarOpen(false)
                                  setCarTouched(true)
                                }}
                                className={[
                                  'flex w-full items-center justify-between px-4 py-2.5 text-left',
                                  isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50',
                                ].join(' ')}
                              >
                                <div className="h-10 w-14 shrink-0 overflow-hidden rounded-md bg-slate-100 border border-slate-200 flex items-center justify-center">
                                  <img
                                    src={c.image || '/default-car.jpg'}
                                    alt={c.label}
                                    className="h-full w-full object-contain p-0.5 mix-blend-multiply"
                                    onError={(e) => {
                                      e.currentTarget.src = '/default-car.jpg'
                                    }}
                                  />
                                </div>

                                <div className="flex flex-1 flex-col ml-3">
                                  <span className={`font-semibold ${isSelected ? 'text-[#243A5E]' : 'text-slate-800'}`}>
                                    {c.label}
                                  </span>
                                  {c.price && <span className="text-[12px] text-slate-500">Từ {c.price} ₫</span>}
                                </div>

                                {isSelected && (
                                  <svg className="h-5 w-5 shrink-0 text-[#8B1D1D]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                  </svg>
                                )}
                              </button>
                            )
                          })
                        )}
                      </div>
                    )}
                  </div>

                  {/* ✨ Chọn phiên bản — hiện sau khi chọn xe */}
                  {selectedCar && (
                    <div className="relative">
                      <label htmlFor={versionId} className="sr-only">Phiên bản</label>
                      <select
                        id={versionId}
                        value={selectedVersionId}
                        onChange={(e) => setSelectedVersionId(e.target.value ? Number(e.target.value) : '')}
                        disabled={isVersionsLoading}
                        className={[
                          inputClassName,
                          'appearance-none pr-11 cursor-pointer',
                          !selectedVersionId ? 'text-slate-400' : 'text-slate-900',
                          isVersionsLoading ? 'cursor-not-allowed opacity-60' : '',
                        ].join(' ')}
                      >
                        <option value="">
                          {isVersionsLoading
                            ? 'Đang tải phiên bản...'
                            : pricingVersions.length === 0
                            ? 'Chưa có phiên bản'
                            : 'Chọn phiên bản'}
                        </option>
                        {pricingVersions.map((v) => (
                          <option key={v.pricingVersionId} value={v.pricingVersionId}>
                            {v.versionName} — {formatMoney(v.priceVnd)} ₫
                          </option>
                        ))}
                      </select>
                      <svg className="pointer-events-none absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400" width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
                        <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                      </svg>
                    </div>
                  )}

                  {/* ✨ Chọn màu — hiện sau khi chọn xe */}
                  {selectedCar && (
                    <div className="relative">
                      <label htmlFor={colorId} className="sr-only">Màu xe</label>
                      <select
                        id={colorId}
                        value={selectedColorId}
                        onChange={(e) => setSelectedColorId(e.target.value ? Number(e.target.value) : '')}
                        disabled={isColorsLoading}
                        className={[
                          inputClassName,
                          'appearance-none pr-11 cursor-pointer',
                          !selectedColorId ? 'text-slate-400' : 'text-slate-900',
                          isColorsLoading ? 'cursor-not-allowed opacity-60' : '',
                        ].join(' ')}
                      >
                        <option value="">
                          {isColorsLoading
                            ? 'Đang tải màu xe...'
                            : carColors.length === 0
                            ? 'Chưa có màu'
                            : 'Chọn màu xe'}
                        </option>
                        {carColors.map((c) => (
                          <option key={c.carColorId} value={c.carColorId}>
                            {c.colorName}
                          </option>
                        ))}
                      </select>
                      <svg className="pointer-events-none absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400" width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
                        <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                      </svg>
                    </div>
                  )}

                  {/* Chọn showroom — hiện sau khi chọn xe */}
                  {selectedCar && (
                    <div ref={rootShowroomRef} className="relative">
                      <label htmlFor={showroomSelectId} className="sr-only">Showroom</label>
                      <input type="hidden" name="showroomId" value={selectedShowroomId} />

                      <button
                        id={showroomSelectId}
                        type="button"
                        disabled={isShowroomsLoading}
                        onClick={() => !isShowroomsLoading && setShowroomOpen((v) => !v)}
                        className={[
                          inputClassName,
                          'flex items-center justify-between pr-11 text-left',
                          !selectedShowroomId ? 'text-slate-400' : 'text-slate-900',
                          touched && !selectedShowroomId ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                          isShowroomsLoading ? 'cursor-not-allowed opacity-60' : 'cursor-pointer',
                        ].join(' ')}
                      >
                        <span className="truncate">
                          {isShowroomsLoading
                            ? 'Đang tải showroom...'
                            : selectedShowroomName
                            ? getShowroomName(selectedShowroomName)
                            : 'Chọn showroom nhận xe'}
                        </span>
                        <svg className="absolute right-3.5 top-1/2 -translate-y-1/2 shrink-0 text-slate-400" width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
                          <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                        </svg>
                      </button>

                      {showroomOpen && (
                        <div className="absolute z-20 mt-2 max-h-56 w-full overflow-auto rounded-xl border border-slate-200 bg-white py-1 text-sm shadow-lg">
                          {showrooms.length === 0 ? (
                            <div className="px-4 py-2 text-slate-500">Không có showroom nào.</div>
                          ) : (
                            showrooms.map((s) => {
                              const isSelected = String(s.showroomId) === selectedShowroomId
                              return (
                                <button
                                  key={s.showroomId}
                                  type="button"
                                  onMouseDown={(e) => {
                                    e.preventDefault()
                                    setSelectedShowroomId(String(s.showroomId))
                                    setShowroomOpen(false)
                                  }}
                                  className={[
                                    'flex w-full flex-col px-4 py-2.5 text-left transition',
                                    isSelected ? 'bg-slate-100 text-slate-900' : 'text-slate-700 hover:bg-slate-50',
                                  ].join(' ')}
                                >
                                  <div className="flex items-center justify-between gap-2">
                                    <span className="truncate font-medium">{getShowroomName(s)}</span>
                                    {isSelected && <span className="shrink-0 text-[11px] font-semibold text-[#243A5E]">Đã chọn</span>}
                                  </div>
                                  {s.fullAddress && (
                                    <span className="mt-0.5 truncate text-xs text-slate-400">{s.fullAddress}</span>
                                  )}
                                </button>
                              )
                            })
                          )}
                        </div>
                      )}
                    </div>
                  )}

                  {/* Thông tin tài chính */}
                  <div className="pt-2">
                    <div className="mb-3 flex items-center gap-2 px-1">
                      <div className="h-px flex-1 bg-gradient-to-r from-transparent via-slate-300 to-transparent" aria-hidden />
                      <span className="text-[11px] font-bold uppercase tracking-wider text-slate-500">
                        Thông tin tài chính
                      </span>
                      <div className="h-px flex-1 bg-gradient-to-r from-transparent via-slate-300 to-transparent" aria-hidden />
                    </div>

                    <div className="relative">
                      <label htmlFor={incomeId} className="sr-only">Thu nhập hàng tháng</label>
                      <input
                        id={incomeId}
                        type="text"
                        inputMode="numeric"
                        placeholder="Thu nhập hàng tháng (VND)"
                        value={monthlyIncomeInput}
                        onChange={(e) => {
                          const num = parseMoney(e.target.value)
                          setMonthlyIncomeInput(num ? formatMoney(num) : '')
                        }}
                        className={[
                          inputClassName,
                          'pr-14',
                          touched && !parseMoney(monthlyIncomeInput) ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                        ].join(' ')}
                      />
                      <span className="pointer-events-none absolute right-4 top-1/2 -translate-y-1/2 text-sm font-semibold text-slate-400">₫</span>
                    </div>
                  </div>

                  <div className="relative">
                    <label htmlFor={downPaymentId} className="sr-only">Số tiền trả trước</label>
                    <input
                      id={downPaymentId}
                      type="text"
                      inputMode="numeric"
                      placeholder="Số tiền trả trước dự kiến (VND)"
                      value={downPaymentInput}
                      onChange={(e) => {
                        const num = parseMoney(e.target.value)
                        setDownPaymentInput(num ? formatMoney(num) : '')
                      }}
                      className={[
                        inputClassName,
                        'pr-14',
                        touched && parseMoney(downPaymentInput) <= 0 ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                      ].join(' ')}
                    />
                    <span className="pointer-events-none absolute right-4 top-1/2 -translate-y-1/2 text-sm font-semibold text-slate-400">₫</span>
                  </div>

                  <div className="relative">
                    <label htmlFor={loanTermId} className="sr-only">Kỳ hạn vay</label>
                    <select
                      id={loanTermId}
                      value={loanTermMonths}
                      onChange={(e) => setLoanTermMonths(e.target.value ? Number(e.target.value) : '')}
                      className={[
                        inputClassName,
                        'appearance-none pr-11 cursor-pointer',
                        !loanTermMonths ? 'text-slate-400' : 'text-slate-900',
                        touched && !loanTermMonths ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-400/20' : '',
                      ].join(' ')}
                    >
                      <option value="">Chọn kỳ hạn vay mong muốn</option>
                      {LOAN_TERM_OPTIONS.map((opt) => (
                        <option key={opt.value} value={opt.value}>{opt.label}</option>
                      ))}
                    </select>
                    <svg className="pointer-events-none absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400" width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden>
                      <path d="M6 9l6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
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