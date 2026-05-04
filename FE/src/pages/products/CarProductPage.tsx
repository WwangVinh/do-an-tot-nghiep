import axios from 'axios'
import { useEffect, useMemo, useState } from 'react'
import { Link, Navigate, useParams, useLocation } from 'react-router-dom'

import { NotFoundPage } from '../NotFoundPage'
import { env } from '../../lib/env'
import {
  CAR_PRODUCT_CATALOG,
  type CarProductColorGalleryItem,
  type CarProductMeta,
  type CarProductSectionSplit,
  type CarProductGallerySlide,
} from './carProductCatalog'
import { CarProductLanding } from './components/CarProductLanding'
import { CarRollingCostCalculator } from './components/CarRollingCostCalculator'

// ─────────────────────────────────────────────
// TYPES
// ─────────────────────────────────────────────
type PricingVersionDto = {
  pricingVersionId: number
  name: string
  priceVnd: number
  sortOrder: number
}
type CarSpecGroupDto = {
  category: string
  items: { specName: string; specValue: string }[]
}
type GalleryGroupDto = {
  category: string
  images: { title: string | null; description: string | null; imageUrl: string }[]
}
type CustomerCarDetailDto = {
  carId: number
  name: string
  brand: string | null
  model: string | null
  year: number | null
  price: number | null
  color: string | null
  mileage: number | null
  fuelType: string | null
  transmission: string | null
  bodyStyle: string | null
  totalQuantity: number
  description: string | null
  imageUrl: string | null
  condition: string | null
  status: string | null
  pricingVersions: PricingVersionDto[]
  specifications: CarSpecGroupDto[]
  galleryImages: GalleryGroupDto[]
  images360: string[]
  features: { featureId: number; featureName: string; icon: string }[]
}
type ApiEnvelope<T> = { message?: string; data: T }
interface Accessory {
  accessoryId: number
  name: string
  price: number
}
interface Review {
  reviewId: number
  fullName: string
  rating: number
  comment: string
  createdAt: string
}

// ✅ Showroom
interface ShowroomPickerDto {
  showroomId: number
  name: string
  province: string
  district: string
  fullAddress: string
  hotline: string | null
}

// ─────────────────────────────────────────────
// HELPERS
// ─────────────────────────────────────────────
function formatVnd(price: number | null | undefined) {
  if (!Number.isFinite(price ?? NaN)) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN').format(price as number) + ' ₫'
}
function toAbsoluteUrl(path: string) {
  const raw = (path ?? '').trim()
  return raw ? new URL(raw, env.VITE_API_BASE_URL).toString() : ''
}
function findGalleryGroup(groups: GalleryGroupDto[] | undefined, category: string) {
  const g = (groups ?? []).find(
    (x) => x.category?.trim().toLowerCase() === category.trim().toLowerCase(),
  )
  return g?.images ?? []
}
function combinedImageDescriptions(images: { description?: string | null }[]) {
  return images
    .map((i) => (i.description ?? '').replace(/\r\n/g, '\n').trim())
    .filter(Boolean)
    .join('\n\n')
}
function toSlides(
  images: { title?: string | null; description?: string | null; imageUrl?: string }[],
  fallbackName: string,
): CarProductGallerySlide[] {
  return images
    .map((img) => {
      const src = toAbsoluteUrl(img.imageUrl ?? '')
      const title = (img.title ?? '').replace(/\r\n/g, '\n').trim()
      const description = (img.description ?? '').replace(/\r\n/g, '\n').trim()
      return { src, alt: title || fallbackName, title: title || null, description: description || null }
    })
    .filter((s) => s.src.length > 0)
}
function buildSectionSplit(
  groups: GalleryGroupDto[] | undefined,
  category: string,
  fallbackIntro: string,
  carName: string,
): CarProductSectionSplit | undefined {
  const images = findGalleryGroup(groups, category)
  const slides = toSlides(images, carName)
  const bodyText = combinedImageDescriptions(images)
  if (!slides.length && !bodyText.trim()) return undefined
  return { intro: fallbackIntro, slides, bodyText }
}

// ─────────────────────────────────────────────
// BREADCRUMBS
// ─────────────────────────────────────────────
function Breadcrumbs({ carName }: { carName: string }) {
  return (
    <nav className="mx-auto w-full max-w-screen-2xl px-6 py-3 text-sm">
      <ol className="flex items-center gap-1.5 text-slate-400 flex-wrap">
        <li>
          <Link to="/" className="hover:text-slate-700 transition">Trang chủ</Link>
        </li>
        <li className="text-slate-300">/</li>
        <li>
          <Link to="/products" className="hover:text-slate-700 transition">Tất cả sản phẩm</Link>
        </li>
        <li className="text-slate-300">/</li>
        <li className="text-slate-800 font-semibold truncate max-w-xs">{carName}</li>
      </ol>
    </nav>
  )
}

// ─────────────────────────────────────────────
// SECTION NAV
// ─────────────────────────────────────────────
type ActiveSection = 'overview' | 'reviews' | 'rolling-cost'

function SectionNav({
  active,
  onChange,
  onOpenOrder,
}: {
  active: ActiveSection
  onChange: (s: ActiveSection) => void
  onOpenOrder: () => void
}) {
  return (
    <div className="sticky top-0 z-30 bg-white border-b border-slate-200 shadow-sm">
      <div className="mx-auto w-full max-w-screen-2xl px-6 flex items-center">
        <button
          onClick={() => onChange('overview')}
          className={`relative px-5 py-4 text-sm font-semibold transition-all ${
            active === 'overview' ? 'text-[#0A2540]' : 'text-slate-400 hover:text-slate-700'
          }`}
        >
          Tổng quan
          {active === 'overview' && (
            <span className="absolute bottom-0 left-0 right-0 h-0.5 bg-[#0A2540] rounded-full" />
          )}
        </button>

        <button
          onClick={() => onChange('reviews')}
          className={`relative px-5 py-4 text-sm font-semibold transition-all ${
            active === 'reviews' ? 'text-[#0A2540]' : 'text-slate-400 hover:text-slate-700'
          }`}
        >
          Đánh giá
          {active === 'reviews' && (
            <span className="absolute bottom-0 left-0 right-0 h-0.5 bg-[#0A2540] rounded-full" />
          )}
        </button>

        <button
          onClick={() => onChange('rolling-cost')}
          className={`relative px-5 py-4 text-sm font-semibold transition-all ${
            active === 'rolling-cost' ? 'text-[#0A2540]' : 'text-slate-400 hover:text-slate-700'
          }`}
        >
          Giá lăn bánh
          {active === 'rolling-cost' && (
            <span className="absolute bottom-0 left-0 right-0 h-0.5 bg-[#0A2540] rounded-full" />
          )}
        </button>

        <div className="ml-auto py-3">
          <button
            onClick={onOpenOrder}
            className="flex items-center gap-2 bg-gradient-to-r from-red-600 to-red-500 hover:from-red-700 hover:to-red-600 text-white text-sm font-bold px-6 py-2.5 rounded-lg transition-all shadow-md ring-2 ring-red-400/40 hover:ring-red-500/60 hover:shadow-lg"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z"
              />
            </svg>
            Đặt mua ngay
          </button>
        </div>
      </div>
    </div>
  )
}

// ─────────────────────────────────────────────
// STEP INDICATOR — 5 bước (thêm Showroom ở bước 2)
// ─────────────────────────────────────────────
const ORDER_STEPS = [
  { step: 1, label: 'Thông tin' },
  { step: 2, label: 'Showroom' },
  { step: 3, label: 'Phụ kiện' },
  { step: 4, label: 'Xác nhận' },
  { step: 5, label: 'Thanh toán' },
]

function StepIndicator({ current }: { current: number }) {
  return (
    <div className="flex items-center justify-center mb-6">
      {ORDER_STEPS.map((s, idx) => (
        <div key={s.step} className="flex items-center">
          <div className="flex flex-col items-center">
            <div
              className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold border-2 transition-all ${
                current === s.step
                  ? 'bg-[#0A2540] border-[#0A2540] text-white'
                  : current > s.step
                  ? 'bg-green-500 border-green-500 text-white'
                  : 'bg-white border-slate-300 text-slate-400'
              }`}
            >
              {current > s.step ? (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                </svg>
              ) : (
                s.step
              )}
            </div>
            <span className={`text-xs mt-1 font-medium ${current >= s.step ? 'text-[#0A2540]' : 'text-slate-400'}`}>
              {s.label}
            </span>
          </div>
          {idx < ORDER_STEPS.length - 1 && (
            <div className={`w-8 h-0.5 mb-5 mx-1 transition-all ${current > s.step ? 'bg-green-500' : 'bg-slate-200'}`} />
          )}
        </div>
      ))}
    </div>
  )
}

// ─────────────────────────────────────────────
// ROLLING COST INLINE
// ─────────────────────────────────────────────
const PROVINCES_RC = [
  { label: 'Hà Nội',            rate: 0.12, phiBien: 1_000_000 },
  { label: 'TP. Hồ Chí Minh',  rate: 0.10, phiBien: 1_000_000 },
  { label: 'Đà Nẵng',          rate: 0.10, phiBien:   500_000 },
  { label: 'Hải Phòng',        rate: 0.10, phiBien:   500_000 },
  { label: 'Cần Thơ',          rate: 0.10, phiBien:   500_000 },
  { label: 'Các tỉnh khác',    rate: 0.10, phiBien:   500_000 },
]

function RollingCostToggle({ basePrice }: { basePrice: number }) {
  const [open, setOpen] = useState(false)
  return (
    <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
      <div className="flex items-center justify-between px-4 py-3 bg-slate-50">
        <div className="flex items-center gap-2">
          <svg className="w-4 h-4 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 11h.01M12 11h.01M15 11h.01M4 19h16a2 2 0 002-2V7a2 2 0 00-2-2H4a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
          <span className="text-xs font-bold text-slate-600 uppercase tracking-wider">Ước tính giá lăn bánh</span>
        </div>
        <button
          onClick={() => setOpen(v => !v)}
          className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none ${open ? 'bg-[#0A2540]' : 'bg-slate-300'}`}
        >
          <span className={`inline-block h-4 w-4 transform rounded-full bg-white shadow transition-transform ${open ? 'translate-x-6' : 'translate-x-1'}`} />
        </button>
      </div>
      {open && <RollingCostInline basePrice={basePrice} />}
    </div>
  )
}

function RollingCostInline({ basePrice }: { basePrice: number }) {
  const [prov, setProv] = useState(PROVINCES_RC[0])
  const truocBa  = basePrice * prov.rate
  const phiBien  = prov.phiBien
  const duongBo  = 1_560_000
  const kiemDinh = 560_000
  const baoHiem  = 479_000
  const total    = basePrice + truocBa + phiBien + duongBo + kiemDinh + baoHiem
  const fmtRC = (n: number) => new Intl.NumberFormat('vi-VN').format(Math.round(n))

  return (
    <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
      <div className="bg-slate-50 px-4 py-2.5 flex items-center justify-between">
        <span className="font-bold text-slate-600 text-xs uppercase tracking-wider">Ước tính giá lăn bánh</span>
        <select
          value={prov.label}
          onChange={(e) => setProv(PROVINCES_RC.find(p => p.label === e.target.value) ?? PROVINCES_RC[0])}
          className="text-xs border border-slate-200 rounded-lg px-2 py-1 text-slate-700 bg-white focus:outline-none"
        >
          {PROVINCES_RC.map(p => <option key={p.label} value={p.label}>{p.label}</option>)}
        </select>
      </div>
      <div className="px-4 py-2 space-y-1.5">
        {[
          [`Lệ phí trước bạ (${(prov.rate*100).toFixed(0)}%)`, truocBa],
          ['Phí đăng ký + biển số', phiBien],
          ['Phí đường bộ (1 năm)', duongBo],
          ['Phí kiểm định', kiemDinh],
          ['Bảo hiểm TNDS (1 năm)', baoHiem],
        ].map(([label, val]) => (
          <div key={label as string} className="flex justify-between text-slate-500">
            <span>{label}</span>
            <span className="font-medium text-slate-700">+ {fmtRC(val as number)} đ</span>
          </div>
        ))}
      </div>
      <div className="border-t border-amber-200 bg-amber-50 px-4 py-3 flex justify-between items-center">
        <span className="text-xs font-bold text-amber-700 uppercase tracking-wider">Tổng lăn bánh ước tính</span>
        <span className="font-black text-[#0A2540] text-base">~ {fmtRC(total)} đ</span>
      </div>
    </div>
  )
}

// ─────────────────────────────────────────────
// ORDER MODAL — 5 bước (thêm bước 2: chọn Showroom)
// ─────────────────────────────────────────────
function OrderModal({
  open,
  onClose,
  carId,
  carName,
  pricingVersions,
  externalState,
}: {
  open: boolean
  onClose: () => void
  carId: number
  carName: string
  pricingVersions: PricingVersionDto[]
  externalState: any
}) {
  const {
    step, setStep,
    formData, setFormData,
    selectedVersion, setSelectedVersion,
    discountPercent, setDiscountPercent,
    discountAmount, setDiscountAmount,
    promoMessage, setPromoMessage,
    selectedAccessories, setSelectedAccessories,
    selectedShowroom, setSelectedShowroom,
    payosUrl, setPayosUrl,
    createdOrderCode, setCreatedOrderCode,
    createdOrderId,
    paymentStatus, setPaymentStatus,
  } = externalState

  const [accessoriesList, setAccessoriesList] = useState<Accessory[]>([])
  const [accessoriesLoading, setAccessoriesLoading] = useState(false)
  const [showroomList, setShowroomList] = useState<ShowroomPickerDto[]>([])
  const [showroomLoading, setShowroomLoading] = useState(false)
  const [showroomFilter, setShowroomFilter] = useState('')
  const [loading, setLoading] = useState(false)
  const [depositPercent, setDepositPercent] = useState<number | null>(null)
  const [checkingPayment, setCheckingPayment] = useState(false)
  const [isExpanded, setIsExpanded] = useState(false)
  const [includeRolling, setIncludeRolling] = useState(false)
  const [rollingProvince, setRollingProvince] = useState(PROVINCES_RC[0])
  const [payosExpiry, setPayosExpiry] = useState<number | null>(null)
  const [timeLeft, setTimeLeft] = useState<number>(0)

  // Countdown timer
  useEffect(() => {
    if (!payosExpiry) return
    const interval = setInterval(() => {
      const left = Math.max(0, Math.floor((payosExpiry - Date.now()) / 1000))
      setTimeLeft(left)
      if (left === 0) clearInterval(interval)
    }, 1000)
    return () => clearInterval(interval)
  }, [payosExpiry])

  const formatTime = (s: number) => `${String(Math.floor(s/60)).padStart(2,'0')}:${String(s%60).padStart(2,'0')}`

  const checkPaymentStatus = async () => {
    if (!createdOrderCode || !formData?.phone) return
    setCheckingPayment(true)
    try {
      if (createdOrderId) {
        await axios.post(
          `${env.VITE_API_BASE_URL}/api/Checkout/${createdOrderId}/confirm`,
          {},
          { headers: { 'ngrok-skip-browser-warning': 'true' } }
        ).catch(() => {})
      }
      const res = await axios.get(
        `${env.VITE_API_BASE_URL}/api/public/orders/lookup?phone=${encodeURIComponent(formData.phone)}&code=${encodeURIComponent(createdOrderCode)}`,
        { headers: { 'ngrok-skip-browser-warning': 'true' } }
      )
      const status = res.data?.paymentStatus ?? res.data?.data?.paymentStatus
      if (status === 'Paid' || status === 'paid' || status === 'Deposited') {
        setPaymentStatus('paid')
      }
    } catch {
      // silent
    } finally {
      setCheckingPayment(false)
    }
  }

  useEffect(() => {
    if (step !== 5 || paymentStatus !== 'pending' || !createdOrderCode) return
    const interval = setInterval(checkPaymentStatus, 5000)
    return () => clearInterval(interval)
  }, [step, paymentStatus, createdOrderCode])

  // Load data khi mở modal
  useEffect(() => {
    if (!open) return

    // Load % đặt cọc
    axios
      .get(`${env.VITE_API_BASE_URL}/api/admin/SystemSettings/public/DepositPercentage`, {
        headers: { 'ngrok-skip-browser-warning': 'true' },
      })
      .then((r) => setDepositPercent(Number(r.data?.value ?? 10)))
      .catch(() => setDepositPercent(10))

    // Load phụ kiện
    setAccessoriesLoading(true)
    axios
      .get(`${env.VITE_API_BASE_URL}/api/public/cars/${carId}/accessories`, {
        headers: { 'ngrok-skip-browser-warning': 'true' },
      })
      .then((r) => setAccessoriesList(r.data?.data ?? r.data ?? []))
      .catch(() => setAccessoriesList([]))
      .finally(() => setAccessoriesLoading(false))

    // ✅ Load showroom có xe này trong kho
    setShowroomLoading(true)
    axios
      .get(`${env.VITE_API_BASE_URL}/api/public/cars/${carId}/showrooms`, {
        headers: { 'ngrok-skip-browser-warning': 'true' },
      })
      .then((r) => setShowroomList(r.data?.data ?? r.data ?? []))
      .catch(() => {
        // fallback: lấy tất cả showroom nếu API theo carId chưa có
        axios
          .get(`${env.VITE_API_BASE_URL}/api/public/orders/showrooms`, {
            headers: { 'ngrok-skip-browser-warning': 'true' },
          })
          .then((r2) => setShowroomList(r2.data?.data ?? r2.data ?? []))
          .catch(() => setShowroomList([]))
      })
      .finally(() => setShowroomLoading(false))
  }, [open])

  useEffect(() => {
    if (selectedVersion) {
      setDiscountAmount((selectedVersion.priceVnd * discountPercent) / 100)
    }
  }, [selectedVersion, discountPercent])

  const handleApplyPromo = async () => {
    if (!formData.promotionCode.trim()) return
    try {
      const res = await axios.get(
        `${env.VITE_API_BASE_URL}/api/public/orders/check-promotion?code=${formData.promotionCode}&carId=${carId}`,
      )
      setDiscountPercent(res.data.discountPercentage)
      setPromoMessage({ type: 'success', text: `Giảm ${res.data.discountPercentage}% — áp dụng thành công!` })
    } catch (err: any) {
      setDiscountPercent(0)
      setDiscountAmount(0)
      setPromoMessage({ type: 'error', text: err.response?.data?.message || 'Mã không hợp lệ hoặc đã hết hạn' })
    }
  }

  const toggleAccessory = (acc: Accessory) => {
    setSelectedAccessories((prev: Accessory[]) =>
      prev.find((a) => a.accessoryId === acc.accessoryId)
        ? prev.filter((a) => a.accessoryId !== acc.accessoryId)
        : [...prev, acc],
    )
  }

  const handleCheckout = async () => {
    setLoading(true)
    try {
      const orderPayload = {
        fullName: formData.fullName,
        phone: formData.phone,
        email: formData.email,
        customerNote: formData.customerNote,
        promotionCode: formData.promotionCode,
        carId,
        pricingVersionId: selectedVersion?.pricingVersionId ?? null,
        showroomId: selectedShowroom?.showroomId ?? null, // ✅ gửi showroomId
        rollingFees: includeRolling ? Math.round(rollingFees) : 0,
        accessoryIds: selectedAccessories.map((a: Accessory) => a.accessoryId),
      }
      const orderRes = await axios.post(
        `${env.VITE_API_BASE_URL}/api/public/orders/checkout`,
        orderPayload,
        { headers: { 'ngrok-skip-browser-warning': 'true' } },
      )

      const orderId =
        orderRes.data?.data?.orderId ??
        orderRes.data?.data?.id ??
        orderRes.data?.orderId ??
        orderRes.data?.id
      const newOrderCode =
        orderRes.data?.data?.orderCode ??
        orderRes.data?.orderCode ?? ''

      if (!orderId) throw new Error('Không lấy được mã đơn hàng từ server')

      const payRes = await axios.post(
        `${env.VITE_API_BASE_URL}/api/Checkout/${orderId}/pay`,
        {},
        { headers: { 'ngrok-skip-browser-warning': 'true' } },
      )

      const checkoutUrl =
        payRes.data?.checkoutUrl ??
        payRes.data?.data?.checkoutUrl ??
        payRes.data?.paymentUrl ??
        payRes.data?.data?.paymentUrl

      if (checkoutUrl) {
        setPayosUrl(checkoutUrl)
        setCreatedOrderCode(newOrderCode)
        const externalSetOrderId = externalState.setCreatedOrderId
        if (externalSetOrderId) externalSetOrderId(orderId)
        setPayosExpiry(Date.now() + 30 * 60 * 1000)
        setTimeLeft(30 * 60)
        setStep(5)
      } else {
        throw new Error('Không nhận được link thanh toán từ PayOS')
      }
    } catch (err: any) {
      alert('Lỗi thanh toán: ' + (err.response?.data?.message || err.message))
    } finally {
      setLoading(false)
    }
  }

  const carPriceAfterDiscount = (selectedVersion?.priceVnd || 0) - discountAmount
  const accessoriesTotal = selectedAccessories.reduce((s: number, a: Accessory) => s + a.price, 0)
  const finalTotal = carPriceAfterDiscount + accessoriesTotal

  const rollingFees = includeRolling ? (
    (selectedVersion?.priceVnd || 0) * rollingProvince.rate +
    rollingProvince.phiBien + 1_560_000 + 560_000 + 479_000
  ) : 0
  const grandTotal = finalTotal + rollingFees

  // Lọc showroom theo tỉnh
  const filteredShowrooms = showroomFilter
    ? showroomList.filter(s => s.province === showroomFilter)
    : showroomList
  const uniqueProvinces = Array.from(new Set(showroomList.map(s => s.province))).sort()

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <div className={`relative bg-white rounded-2xl shadow-2xl overflow-hidden transition-all duration-300 ${isExpanded ? 'w-full h-full max-w-none rounded-none' : 'w-full max-w-2xl'}`}>
        {/* Header modal */}
        <div className="bg-[#0A2540] px-6 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            {step === 5 && (
              <button
                onClick={() => { setStep(4); setPayosExpiry(null); setTimeLeft(0); setPaymentStatus('pending') }}
                className="text-slate-400 hover:text-white transition w-8 h-8 flex items-center justify-center rounded-full hover:bg-white/10 flex-shrink-0"
                title="Quay lại"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                </svg>
              </button>
            )}
            <div>
              <h2 className="text-white font-bold text-base tracking-wide">ĐẶT MUA XE</h2>
              <p className="text-slate-400 text-xs mt-0.5 truncate max-w-xs">{carName}</p>
            </div>
          </div>
          <div className="flex items-center gap-1">
            <button
              onClick={() => setIsExpanded((v: boolean) => !v)}
              className="text-slate-400 hover:text-white transition w-8 h-8 flex items-center justify-center rounded-full hover:bg-white/10"
              title={isExpanded ? 'Thu nhỏ' : 'Phóng to'}
            >
              {isExpanded ? (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 9L4 4m0 0h5m-5 0v5M15 9l5-5m0 0h-5m5 0v5M9 15l-5 5m0 0h5m-5 0v-5M15 15l5 5m0 0h-5m5 0v-5" />
                </svg>
              ) : (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 8V4m0 0h4M4 4l5 5M20 8V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5M20 16v4m0 0h-4m4 0l-5-5" />
                </svg>
              )}
            </button>
            <button
              onClick={onClose}
              className="text-slate-400 hover:text-white transition w-8 h-8 flex items-center justify-center rounded-full hover:bg-white/10"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        {/* Body */}
        <div className={`px-6 pt-6 pb-5 overflow-y-auto ${isExpanded ? 'h-[calc(100vh-64px)]' : 'max-h-[80vh]'}`}>
          <StepIndicator current={step} />

          {/* ─── BƯỚC 1: Thông tin & mã giảm giá ─── */}
          {step === 1 && (
            <div className="space-y-4">
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Chọn phiên bản <span className="text-red-500">*</span>
                </label>
                <select
                  className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
                  value={selectedVersion?.pricingVersionId ?? ''}
                  onChange={(e) =>
                    setSelectedVersion(pricingVersions.find((v) => v.pricingVersionId === Number(e.target.value)) ?? null)
                  }
                >
                  {pricingVersions.map((v) => (
                    <option key={v.pricingVersionId} value={v.pricingVersionId}>
                      {v.name} — {new Intl.NumberFormat('vi-VN').format(v.priceVnd)} đ
                    </option>
                  ))}
                </select>
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                    Họ và tên <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text" placeholder="Nguyễn Văn A"
                    className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
                    value={formData.fullName}
                    onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                  />
                </div>
                <div>
                  <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                    Số điện thoại <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="tel" placeholder="0987 654 321"
                    className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
                    value={formData.phone}
                    onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">Email</label>
                <input
                  type="email" placeholder="email@example.com"
                  className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">Ghi chú</label>
                <textarea
                  rows={2} placeholder="Yêu cầu thêm (nếu có)..."
                  className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540] resize-none"
                  value={formData.customerNote}
                  onChange={(e) => setFormData({ ...formData, customerNote: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Mã khuyến mãi
                  <span className="ml-1 text-slate-400 normal-case font-normal">(chỉ áp dụng cho giá xe)</span>
                </label>
                <div className="flex gap-2">
                  <input
                    type="text" placeholder="VD: SALE2026"
                    className="flex-1 border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white uppercase focus:outline-none focus:border-[#0A2540]"
                    value={formData.promotionCode}
                    onChange={(e) => setFormData({ ...formData, promotionCode: e.target.value })}
                    onKeyDown={(e) => { if (e.key === 'Enter') handleApplyPromo() }}
                  />
                  <button
                    className="bg-slate-800 hover:bg-slate-900 text-white px-4 rounded-lg text-xs font-bold transition"
                    onClick={handleApplyPromo}
                  >
                    ÁP DỤNG
                  </button>
                </div>
                {promoMessage.text && (
                  <p className={`text-xs mt-1.5 font-medium ${promoMessage.type === 'error' ? 'text-red-500' : 'text-green-600'}`}>
                    {promoMessage.type === 'success' ? '✓ ' : '✗ '}{promoMessage.text}
                  </p>
                )}
              </div>

              <div className="bg-slate-50 border border-slate-100 rounded-xl p-4 text-sm space-y-1.5">
                <div className="flex justify-between text-slate-600">
                  <span>Giá xe ({selectedVersion?.name}):</span>
                  <span>{new Intl.NumberFormat('vi-VN').format(selectedVersion?.priceVnd || 0)} đ</span>
                </div>
                {discountAmount > 0 && (
                  <div className="flex justify-between text-green-600 font-semibold">
                    <span>Giảm giá ({discountPercent}%):</span>
                    <span>− {new Intl.NumberFormat('vi-VN').format(discountAmount)} đ</span>
                  </div>
                )}
                <div className="flex justify-between font-bold text-[#0A2540] text-sm pt-2 border-t border-slate-200">
                  <span>Tạm tính:</span>
                  <span>{new Intl.NumberFormat('vi-VN').format(carPriceAfterDiscount)} đ</span>
                </div>
              </div>

              <button
                disabled={!formData.fullName.trim() || !formData.phone.trim()}
                className="w-full bg-[#0A2540] disabled:opacity-40 disabled:cursor-not-allowed text-white font-bold py-3 rounded-xl hover:bg-[#0d345c] transition-all"
                onClick={() => setStep(2)}
              >
                Tiếp tục → Chọn showroom
              </button>
            </div>
          )}

          {/* ─── BƯỚC 2: Chọn Showroom ─── */}
          {step === 2 && (
            <div className="space-y-4">
              <div>
                <p className="text-sm font-bold text-slate-800 mb-1">Chọn showroom nhận xe</p>
                <p className="text-xs text-slate-400 mb-4">
                  Chỉ hiển thị showroom hiện có <span className="font-semibold text-[#0A2540]">{carName}</span> trong kho.
                  Bạn có thể đến trực tiếp để xem xe và hoàn tất thủ tục.
                </p>
              </div>

              {/* Lọc theo tỉnh */}
              {uniqueProvinces.length > 1 && (
                <div className="flex gap-2 flex-wrap">
                  <button
                    onClick={() => setShowroomFilter('')}
                    className={`px-3 py-1.5 rounded-lg text-xs font-semibold border transition-all ${
                      showroomFilter === ''
                        ? 'bg-[#0A2540] text-white border-[#0A2540]'
                        : 'bg-white text-slate-600 border-slate-200 hover:border-slate-300'
                    }`}
                  >
                    Tất cả
                  </button>
                  {uniqueProvinces.map(p => (
                    <button
                      key={p}
                      onClick={() => setShowroomFilter(p)}
                      className={`px-3 py-1.5 rounded-lg text-xs font-semibold border transition-all ${
                        showroomFilter === p
                          ? 'bg-[#0A2540] text-white border-[#0A2540]'
                          : 'bg-white text-slate-600 border-slate-200 hover:border-slate-300'
                      }`}
                    >
                      {p}
                    </button>
                  ))}
                </div>
              )}

              {/* Danh sách showroom */}
              <div className="space-y-3 max-h-80 overflow-y-auto pr-1">
                {showroomLoading ? (
                  <div className="flex items-center justify-center py-10 text-slate-400 text-sm gap-2">
                    <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z" />
                    </svg>
                    Đang tải danh sách showroom...
                  </div>
                ) : filteredShowrooms.length === 0 ? (
                  <div className="text-sm text-slate-400 text-center py-8 bg-slate-50 rounded-xl border border-slate-100">
                    <div className="text-2xl mb-2">🏪</div>
                    Không tìm thấy showroom phù hợp.
                  </div>
                ) : (
                  filteredShowrooms.map((showroom) => {
                    const isSelected = selectedShowroom?.showroomId === showroom.showroomId
                    return (
                      <div
                        key={showroom.showroomId}
                        onClick={() => setSelectedShowroom(showroom)}
                        className={`flex items-start gap-4 px-4 py-4 rounded-xl border cursor-pointer transition-all select-none ${
                          isSelected
                            ? 'border-[#0A2540] bg-blue-50 shadow-sm ring-1 ring-[#0A2540]/20'
                            : 'border-slate-200 hover:border-slate-300 hover:bg-slate-50'
                        }`}
                      >
                        {/* Radio circle */}
                        <div className={`mt-0.5 w-5 h-5 rounded-full border-2 flex items-center justify-center flex-shrink-0 transition-all ${
                          isSelected ? 'border-[#0A2540] bg-[#0A2540]' : 'border-slate-300'
                        }`}>
                          {isSelected && <div className="w-2 h-2 rounded-full bg-white" />}
                        </div>

                        {/* Nội dung showroom */}
                        <div className="flex-1 min-w-0">
                          <div className="flex items-start justify-between gap-2">
                            <p className="font-bold text-slate-900 text-sm leading-tight">{showroom.name}</p>
                            <span className="flex-shrink-0 text-xs font-semibold text-[#0A2540] bg-blue-100 px-2 py-0.5 rounded-full">
                              {showroom.province}
                            </span>
                          </div>
                          <p className="text-xs text-slate-500 mt-1 flex items-start gap-1.5">
                            <svg className="w-3.5 h-3.5 mt-0.5 flex-shrink-0 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                            </svg>
                            <span>{showroom.fullAddress}</span>
                          </p>
                          {showroom.hotline && (
                            <p className="text-xs text-slate-500 mt-1 flex items-center gap-1.5">
                              <svg className="w-3.5 h-3.5 flex-shrink-0 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                              </svg>
                              <span className="font-medium text-[#0A2540]">{showroom.hotline}</span>
                            </p>
                          )}
                        </div>
                      </div>
                    )
                  })
                )}
              </div>

              {/* Selected showroom summary */}
              {selectedShowroom && (
                <div className="bg-green-50 border border-green-200 rounded-xl px-4 py-3 flex items-center gap-3">
                  <div className="w-8 h-8 rounded-full bg-green-500 flex items-center justify-center flex-shrink-0">
                    <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                    </svg>
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="text-xs text-green-600 font-semibold">Đã chọn</p>
                    <p className="text-sm font-bold text-slate-800 truncate">{selectedShowroom.name}</p>
                    <p className="text-xs text-slate-500 truncate">{selectedShowroom.district}, {selectedShowroom.province}</p>
                  </div>
                </div>
              )}

              <div className="flex gap-3">
                <button
                  className="flex-1 bg-slate-100 text-slate-700 font-semibold py-3 rounded-xl hover:bg-slate-200 transition text-sm"
                  onClick={() => setStep(1)}
                >
                  ← Quay lại
                </button>
                <button
                  disabled={!selectedShowroom}
                  className="flex-[2] bg-[#0A2540] disabled:opacity-40 disabled:cursor-not-allowed text-white font-bold py-3 rounded-xl hover:bg-[#0d345c] transition text-sm"
                  onClick={() => setStep(3)}
                >
                  Tiếp tục → Chọn phụ kiện
                </button>
              </div>
            </div>
          )}

          {/* ─── BƯỚC 3: Phụ kiện ─── */}
          {step === 3 && (
            <div className="space-y-4">
              <p className="text-xs text-slate-500 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
                ⚠️ Phụ kiện <strong>không áp dụng mã giảm giá</strong>. Tổng = (giá xe − giảm giá) + phụ kiện.
              </p>

              <div className="space-y-2 max-h-52 overflow-y-auto pr-1">
                {accessoriesLoading ? (
                  <div className="flex items-center justify-center py-8 text-slate-400 text-sm gap-2">
                    <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z" />
                    </svg>
                    Đang tải phụ kiện...
                  </div>
                ) : accessoriesList.length === 0 ? (
                  <div className="text-sm text-slate-400 text-center py-6 bg-slate-50 rounded-xl border border-slate-100">
                    Hiện chưa có phụ kiện nào.
                  </div>
                ) : (
                  accessoriesList.map((acc) => {
                    const isSelected = selectedAccessories.some((a: Accessory) => a.accessoryId === acc.accessoryId)
                    return (
                      <div
                        key={acc.accessoryId}
                        onClick={() => toggleAccessory(acc)}
                        className={`flex items-center justify-between px-4 py-3 rounded-xl border cursor-pointer transition-all select-none ${
                          isSelected
                            ? 'border-[#0A2540] bg-blue-50 shadow-sm'
                            : 'border-slate-200 hover:border-slate-300 hover:bg-slate-50'
                        }`}
                      >
                        <div className="flex items-center gap-3">
                          <div className={`w-4 h-4 rounded border-2 flex items-center justify-center flex-shrink-0 transition-all ${isSelected ? 'bg-[#0A2540] border-[#0A2540]' : 'border-slate-300'}`}>
                            {isSelected && (
                              <svg className="w-2.5 h-2.5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                              </svg>
                            )}
                          </div>
                          <span className="text-sm font-medium text-slate-800">{acc.name}</span>
                        </div>
                        <span className="text-sm font-bold text-slate-700 flex-shrink-0 ml-3">
                          +{new Intl.NumberFormat('vi-VN').format(acc.price)} đ
                        </span>
                      </div>
                    )
                  })
                )}
              </div>

              <div className="bg-[#0A2540] text-white rounded-xl p-4 space-y-1.5 text-sm">
                <div className="flex justify-between opacity-75">
                  <span>Xe (sau giảm giá):</span>
                  <span>{new Intl.NumberFormat('vi-VN').format(carPriceAfterDiscount)} đ</span>
                </div>
                <div className="flex justify-between opacity-75">
                  <span>Phụ kiện ({selectedAccessories.length} món):</span>
                  <span>+ {new Intl.NumberFormat('vi-VN').format(accessoriesTotal)} đ</span>
                </div>
                <div className="flex justify-between text-base font-bold pt-2 border-t border-white/20">
                  <span>Tổng cộng:</span>
                  <span className="text-amber-400">{new Intl.NumberFormat('vi-VN').format(finalTotal)} đ</span>
                </div>
              </div>

              <div className="flex gap-3">
                <button
                  className="flex-1 bg-slate-100 text-slate-700 font-semibold py-3 rounded-xl hover:bg-slate-200 transition text-sm"
                  onClick={() => setStep(2)}
                >
                  ← Quay lại
                </button>
                <button
                  className="flex-[2] bg-[#0A2540] text-white font-bold py-3 rounded-xl hover:bg-[#0d345c] transition text-sm"
                  onClick={() => setStep(4)}
                >
                  Xem lại & Thanh toán →
                </button>
              </div>
            </div>
          )}

          {/* ─── BƯỚC 4: Xác nhận ─── */}
          {step === 4 && (
            <div className="space-y-4">
              {/* Thông tin khách */}
              <div className="bg-slate-50 border border-slate-100 rounded-xl p-4 text-sm">
                <p className="font-bold text-slate-800 mb-3">Thông tin đặt mua</p>
                <div className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1.5 text-sm">
                  <span className="text-slate-400 text-xs font-semibold uppercase">Họ tên</span>
                  <span className="font-medium text-slate-800">{formData.fullName}</span>
                  <span className="text-slate-400 text-xs font-semibold uppercase">Điện thoại</span>
                  <span className="font-medium text-slate-800">{formData.phone}</span>
                  {formData.email && (
                    <>
                      <span className="text-slate-400 text-xs font-semibold uppercase">Email</span>
                      <span className="font-medium text-slate-800">{formData.email}</span>
                    </>
                  )}
                  <span className="text-slate-400 text-xs font-semibold uppercase">Phiên bản</span>
                  <span className="font-medium text-slate-800">{selectedVersion?.name}</span>
                  {formData.customerNote && (
                    <>
                      <span className="text-slate-400 text-xs font-semibold uppercase">Ghi chú</span>
                      <span className="font-medium text-slate-800">{formData.customerNote}</span>
                    </>
                  )}
                </div>
              </div>

              {/* Showroom đã chọn */}
              {selectedShowroom && (
                <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
                  <div className="bg-slate-50 px-4 py-2.5 flex items-center justify-between">
                    <span className="font-bold text-slate-600 text-xs uppercase tracking-wider flex items-center gap-1.5">
                      <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                      </svg>
                      Showroom nhận xe
                    </span>
                    <button
                      onClick={() => setStep(2)}
                      className="text-xs text-[#0A2540] font-semibold hover:underline"
                    >
                      Đổi
                    </button>
                  </div>
                  <div className="px-4 py-3 space-y-1">
                    <p className="font-bold text-slate-900">{selectedShowroom.name}</p>
                    <p className="text-xs text-slate-500 flex items-start gap-1.5">
                      <svg className="w-3.5 h-3.5 mt-0.5 flex-shrink-0 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      {selectedShowroom.fullAddress}
                    </p>
                    {selectedShowroom.hotline && (
                      <p className="text-xs text-slate-500 flex items-center gap-1.5">
                        <svg className="w-3.5 h-3.5 flex-shrink-0 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                        </svg>
                        <span className="font-medium text-[#0A2540]">{selectedShowroom.hotline}</span>
                      </p>
                    )}
                  </div>
                </div>
              )}

              {/* Chi tiết đơn hàng */}
              <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
                <div className="bg-slate-50 px-4 py-2.5 font-bold text-slate-600 text-xs uppercase tracking-wider">
                  Chi tiết đơn hàng
                </div>
                <div className="px-4 py-3 space-y-2">
                  <div className="flex justify-between text-slate-600">
                    <span>Xe {selectedVersion?.name}:</span>
                    <span>{new Intl.NumberFormat('vi-VN').format(selectedVersion?.priceVnd || 0)} đ</span>
                  </div>
                  {discountAmount > 0 && (
                    <div className="flex justify-between text-green-600 font-semibold">
                      <span>Giảm giá ({discountPercent}%):</span>
                      <span>− {new Intl.NumberFormat('vi-VN').format(discountAmount)} đ</span>
                    </div>
                  )}
                  {selectedAccessories.map((a: Accessory) => (
                    <div key={a.accessoryId} className="flex justify-between text-slate-500">
                      <span>+ {a.name}:</span>
                      <span>{new Intl.NumberFormat('vi-VN').format(a.price)} đ</span>
                    </div>
                  ))}
                  {rollingFees > 0 && (
                    <div className="flex justify-between text-blue-600 font-semibold">
                      <span>Phí lăn bánh (showroom nộp hộ):</span>
                      <span>+ {new Intl.NumberFormat('vi-VN').format(Math.round(rollingFees))} đ</span>
                    </div>
                  )}
                  <div className="flex justify-between font-bold text-[#0A2540] text-base pt-2 border-t border-slate-200">
                    <span>TỔNG THANH TOÁN:</span>
                    <span>{new Intl.NumberFormat('vi-VN').format(Math.round(grandTotal))} đ</span>
                  </div>
                </div>
              </div>

              {depositPercent !== null && (
                <div className="bg-amber-50 border border-amber-200 rounded-xl p-4 text-sm">
                  <div className="flex justify-between text-amber-700 font-semibold mb-1">
                    <span>Tỷ lệ đặt cọc:</span>
                    <span>{depositPercent}% tổng giá trị đơn</span>
                  </div>
                  <div className="flex justify-between text-amber-900 font-black text-base">
                    <span>Số tiền cần đặt cọc (ước tính):</span>
                    <span>~ {new Intl.NumberFormat('vi-VN').format(Math.round(grandTotal * depositPercent / 100))} đ</span>
                  </div>
                  <p className="text-xs text-amber-600 mt-1.5">⚠️ Số tiền chính xác do hệ thống xác nhận khi tạo đơn. Phần còn lại thanh toán khi nhận xe.</p>
                </div>
              )}

              {/* Toggle phí lăn bánh */}
              <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
                <div className="flex items-center justify-between px-4 py-3 bg-slate-50">
                  <div>
                    <p className="text-xs font-bold text-slate-700">Nhờ showroom nộp phí hộ</p>
                    <p className="text-xs text-slate-400 mt-0.5">Cộng thêm phí trước bạ, biển số, đường bộ vào đơn</p>
                  </div>
                  <button
                    onClick={() => setIncludeRolling((v: boolean) => !v)}
                    className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none ${includeRolling ? 'bg-[#0A2540]' : 'bg-slate-300'}`}
                  >
                    <span className={`inline-block h-4 w-4 transform rounded-full bg-white shadow transition-transform ${includeRolling ? 'translate-x-6' : 'translate-x-1'}`} />
                  </button>
                </div>
                {includeRolling && (
                  <div className="px-4 pb-3 pt-2 space-y-2 border-t border-slate-100">
                    <div className="flex items-center justify-between">
                      <span className="text-slate-500 text-xs">Tỉnh/thành đăng ký:</span>
                      <select
                        value={rollingProvince.label}
                        onChange={(e) => setRollingProvince(PROVINCES_RC.find(p => p.label === e.target.value) ?? PROVINCES_RC[0])}
                        className="text-xs border border-slate-200 rounded-lg px-2 py-1 text-slate-700 bg-white focus:outline-none"
                      >
                        {PROVINCES_RC.map(p => <option key={p.label} value={p.label}>{p.label}</option>)}
                      </select>
                    </div>
                    {[
                      [`Trước bạ (${(rollingProvince.rate*100).toFixed(0)}%)`, (selectedVersion?.priceVnd||0)*rollingProvince.rate],
                      ['Biển số + đăng ký', rollingProvince.phiBien],
                      ['Đường bộ 1 năm', 1_560_000],
                      ['Kiểm định', 560_000],
                      ['Bảo hiểm TNDS', 479_000],
                    ].map(([label, val]) => (
                      <div key={label as string} className="flex justify-between text-slate-500">
                        <span>{label}</span>
                        <span className="font-medium text-slate-700">+ {new Intl.NumberFormat('vi-VN').format(val as number)} đ</span>
                      </div>
                    ))}
                    <div className="flex justify-between font-bold text-[#0A2540] pt-2 border-t border-slate-100">
                      <span>Tổng phí lăn bánh:</span>
                      <span>+ {new Intl.NumberFormat('vi-VN').format(Math.round(rollingFees))} đ</span>
                    </div>
                  </div>
                )}
              </div>

              <div className="flex gap-3">
                <button
                  className="flex-1 bg-slate-100 text-slate-700 font-semibold py-3 rounded-xl hover:bg-slate-200 transition text-sm"
                  onClick={() => setStep(3)}
                >
                  ← Quay lại
                </button>
                <button
                  disabled={loading}
                  className="flex-[2] bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-bold py-3 rounded-xl transition text-sm flex items-center justify-center gap-2"
                  onClick={handleCheckout}
                >
                  {loading ? (
                    <>
                      <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z" />
                      </svg>
                      Đang xử lý...
                    </>
                  ) : (
                    <>
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 9V7a5 5 0 00-10 0v2M5 9h14l1 11H4L5 9z" />
                      </svg>
                      Xác nhận & Thanh toán đặt cọc →
                    </>
                  )}
                </button>
              </div>
            </div>
          )}

          {/* ─── BƯỚC 5: PayOS ─── */}
          {step === 5 && (
            <div className="space-y-4">
              <div className="flex items-center justify-between gap-3">
                {createdOrderCode && (
                  <div className="flex items-center gap-2 bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 flex-1">
                    <span className="text-xs text-slate-500 font-semibold uppercase tracking-wider">Mã đơn</span>
                    <span className="font-black text-[#0A2540] text-sm">{createdOrderCode}</span>
                  </div>
                )}
                {payosExpiry && timeLeft > 0 && (
                  <div className={`flex items-center gap-1.5 px-3 py-2 rounded-xl border text-xs font-bold flex-shrink-0 ${timeLeft < 300 ? 'bg-red-50 border-red-200 text-red-600' : 'bg-amber-50 border-amber-200 text-amber-700'}`}>
                    <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    {formatTime(timeLeft)}
                  </div>
                )}
              </div>

              {payosExpiry && timeLeft === 0 ? (
                <div className="bg-red-50 border border-red-200 rounded-xl p-5 text-center space-y-3">
                  <div className="text-3xl">⏰</div>
                  <p className="text-red-700 font-bold text-sm">Link thanh toán đã hết hạn (30 phút)</p>
                  <p className="text-red-500 text-xs">Vui lòng quay lại và tạo đơn mới.</p>
                  <button
                    onClick={() => { setStep(4); setPayosExpiry(null); setTimeLeft(0) }}
                    className="bg-[#0A2540] text-white text-xs font-bold px-5 py-2 rounded-lg hover:bg-[#0d345c] transition"
                  >
                    ← Quay lại tạo đơn mới
                  </button>
                </div>
              ) : (
                <>
                  {paymentStatus === 'paid' ? (
                    <div className="flex flex-col items-center justify-center gap-5 py-8 text-center">
                      <div className="w-20 h-20 rounded-full bg-green-100 flex items-center justify-center">
                        <svg className="w-10 h-10 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                        </svg>
                      </div>
                      <div>
                        <p className="font-black text-slate-900 text-xl mb-1">Thanh toán thành công! 🎉</p>
                        <p className="text-slate-500 text-sm">Đặt cọc của bạn đã được ghi nhận.</p>
                        {createdOrderCode && (
                          <div className="mt-3 inline-block bg-slate-100 rounded-xl px-4 py-2">
                            <span className="text-xs text-slate-500 font-semibold uppercase tracking-wider block">Mã đơn hàng</span>
                            <span className="font-black text-[#0A2540] text-lg tracking-widest">{createdOrderCode}</span>
                          </div>
                        )}
                        {selectedShowroom && (
                          <div className="mt-3 bg-blue-50 border border-blue-100 rounded-xl px-4 py-3 text-sm text-left">
                            <p className="text-xs text-blue-500 font-semibold mb-1">Showroom nhận xe của bạn</p>
                            <p className="font-bold text-slate-800">{selectedShowroom.name}</p>
                            <p className="text-xs text-slate-500">{selectedShowroom.fullAddress}</p>
                            {selectedShowroom.hotline && (
                              <p className="text-xs text-[#0A2540] font-semibold mt-1">{selectedShowroom.hotline}</p>
                            )}
                          </div>
                        )}
                      </div>
                      <div className="bg-blue-50 border border-blue-100 rounded-xl p-4 text-sm text-blue-700 max-w-xs">
                        Nhân viên sẽ liên hệ xác nhận đơn hàng trong vòng <strong>24 giờ</strong>.
                      </div>
                      <button onClick={onClose} className="bg-[#0A2540] hover:bg-[#0d345c] text-white font-bold px-8 py-3 rounded-xl transition text-sm">
                        Đóng
                      </button>
                    </div>
                  ) : (
                    <>
                      <div className="rounded-xl overflow-hidden border border-slate-200 bg-slate-50" style={{ height: isExpanded ? 'calc(100vh - 280px)' : 440 }}>
                        <iframe
                          src={payosUrl}
                          className="w-full h-full"
                          title="Thanh toán PayOS"
                          allow="payment"
                          onLoad={(e) => {
                            try {
                              const href = (e.target as HTMLIFrameElement).contentWindow?.location?.href ?? ''
                              if (href.includes('payment-success')) checkPaymentStatus()
                            } catch {
                              checkPaymentStatus()
                            }
                          }}
                        />
                      </div>
                      <div className="space-y-1 text-center">
                        <div className="flex items-center gap-2 text-xs text-slate-400 justify-center">
                          <svg className="animate-spin w-3 h-3" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"/>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                          </svg>
                          Đang tự động kiểm tra thanh toán...
                        </div>
                        <p className="text-xs text-amber-600 font-medium">✅ Vui lòng tắt trang này sau khi thanh toán xong.</p>
                      </div>
                    </>
                  )}
                </>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

// ─────────────────────────────────────────────
// CAR REVIEWS
// ─────────────────────────────────────────────
function CarReviews({ carId }: { carId: number }) {
  const [reviews, setReviews] = useState<Review[]>([])
  const [phone, setPhone] = useState('')
  const [checking, setChecking] = useState(false)
  const [checkError, setCheckError] = useState('')
  const [verifiedName, setVerifiedName] = useState<string | null>(null)
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [submitMsg, setSubmitMsg] = useState({ type: '', text: '' })

  const fetchReviews = async () => {
    try {
      const res = await axios.get(`${env.VITE_API_BASE_URL}/api/Reviews/car/${carId}`)
      setReviews(res.data?.data ?? res.data ?? [])
    } catch {
      console.error('Lỗi tải đánh giá')
    }
  }

  useEffect(() => { fetchReviews() }, [carId])

  const handleVerifyPhone = async () => {
    if (!phone.trim()) return
    setChecking(true)
    setCheckError('')
    setVerifiedName(null)
    try {
      const res = await axios.get(
        `${env.VITE_API_BASE_URL}/api/Reviews/verify-phone?phone=${encodeURIComponent(phone.trim())}&carId=${carId}`,
        { headers: { 'ngrok-skip-browser-warning': 'true' } },
      )
      const name = res.data?.fullName ?? res.data?.data?.fullName
      if (name) {
        setVerifiedName(name)
      } else {
        setCheckError('Số điện thoại chưa có lịch sử giao dịch được xác nhận với showroom.')
      }
    } catch (err: any) {
      setCheckError(
        err.response?.data?.message ||
        'Số điện thoại chưa có lịch sử giao dịch được xác nhận với showroom.',
      )
    } finally {
      setChecking(false)
    }
  }

  const handleSubmitReview = async () => {
    if (!verifiedName || !comment.trim()) return
    setSubmitting(true)
    setSubmitMsg({ type: '', text: '' })
    try {
      const res = await axios.post(`${env.VITE_API_BASE_URL}/api/Reviews/submit`, {
        carId,
        fullName: verifiedName,
        phone: phone.trim(),
        orderCode: '',
        rating,
        comment,
      }, { headers: { 'ngrok-skip-browser-warning': 'true' } })
      setSubmitMsg({ type: 'success', text: res.data?.message || 'Đăng đánh giá thành công!' })
      setComment('')
      setRating(5)
      setVerifiedName(null)
      setPhone('')
      setCheckError('')
      fetchReviews()
    } catch (err: any) {
      setSubmitMsg({
        type: 'error',
        text: err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.',
      })
    } finally {
      setSubmitting(false)
    }
  }

  const avgRating = reviews.length
    ? (reviews.reduce((s, r) => s + r.rating, 0) / reviews.length).toFixed(1)
    : null

  return (
    <section className="mx-auto w-full max-w-screen-2xl px-6 py-12">
      {avgRating && (
        <div className="flex items-center gap-4 mb-8 p-5 bg-slate-50 rounded-2xl border border-slate-100 w-fit">
          <div className="text-5xl font-black text-[#0A2540]">{avgRating}</div>
          <div>
            <div className="flex text-amber-400 text-lg mb-0.5">
              {Array.from({ length: Math.round(Number(avgRating)) }).map((_, i) => (
                <span key={i}>★</span>
              ))}
            </div>
            <p className="text-xs text-slate-500">{reviews.length} đánh giá</p>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-10">
        <div className="bg-slate-50 p-6 rounded-2xl border border-slate-100 h-fit">
          <h3 className="text-base font-bold text-slate-900 mb-1">Gửi đánh giá của bạn</h3>
          <p className="text-xs text-slate-400 mb-4 leading-relaxed">
            Chỉ khách hàng đã booking hoặc đặt cọc và được showroom xác nhận mới có thể đánh giá.
          </p>

          {submitMsg.text && (
            <div className={`mb-4 px-4 py-2.5 text-sm rounded-xl border ${
              submitMsg.type === 'success'
                ? 'bg-green-50 text-green-700 border-green-200'
                : 'bg-rose-50 text-rose-700 border-rose-200'
            }`}>
              {submitMsg.text}
            </div>
          )}

          {!verifiedName ? (
            <div className="space-y-3">
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Số điện thoại <span className="text-red-500">*</span>
                </label>
                <div className="flex gap-2">
                  <input
                    type="tel"
                    placeholder="0987 654 321"
                    className="flex-1 bg-white border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 focus:outline-none focus:border-slate-400"
                    value={phone}
                    onChange={(e) => { setPhone(e.target.value); setCheckError('') }}
                    onKeyDown={(e) => { if (e.key === 'Enter') handleVerifyPhone() }}
                  />
                  <button
                    onClick={handleVerifyPhone}
                    disabled={!phone.trim() || checking}
                    className="bg-[#0A2540] disabled:opacity-40 hover:bg-[#0d345c] text-white px-4 rounded-xl text-xs font-bold transition flex-shrink-0 flex items-center gap-1.5"
                  >
                    {checking ? (
                      <>
                        <svg className="animate-spin w-3 h-3" fill="none" viewBox="0 0 24 24">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z" />
                        </svg>
                        ...
                      </>
                    ) : 'XÁC MINH'}
                  </button>
                </div>
                {checkError && (
                  <p className="text-xs mt-1.5 text-red-500 font-medium leading-relaxed">✗ {checkError}</p>
                )}
              </div>
            </div>
          ) : (
            <div className="space-y-3">
              <div className="flex items-center gap-3 bg-green-50 border border-green-200 rounded-xl px-4 py-3">
                <div className="w-8 h-8 rounded-full bg-green-500 flex items-center justify-center flex-shrink-0">
                  <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs text-green-600 font-semibold">Đã xác minh</p>
                  <p className="text-sm font-bold text-slate-800 truncate">{verifiedName}</p>
                </div>
                <button
                  onClick={() => { setVerifiedName(null); setPhone(''); setCheckError('') }}
                  className="text-slate-400 hover:text-slate-600 transition text-xs underline flex-shrink-0"
                >
                  Đổi
                </button>
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1.5">Đánh giá sao</label>
                <div className="flex gap-1">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <button
                      key={star}
                      onClick={() => setRating(star)}
                      className={`text-2xl transition-transform hover:scale-110 leading-none ${star <= rating ? 'text-amber-400' : 'text-slate-200'}`}
                    >
                      ★
                    </button>
                  ))}
                  <span className="ml-2 text-xs text-slate-400 self-center">
                    {['', 'Rất tệ', 'Tệ', 'Trung bình', 'Tốt', 'Xuất sắc'][rating]}
                  </span>
                </div>
              </div>

              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Nhận xét <span className="text-red-500">*</span>
                </label>
                <textarea
                  rows={4}
                  placeholder="Chia sẻ trải nghiệm thực tế của bạn về chiếc xe này..."
                  className="w-full bg-white border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:border-slate-400 resize-none"
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                />
              </div>

              <button
                onClick={handleSubmitReview}
                disabled={!comment.trim() || submitting}
                className="w-full bg-slate-900 disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-bold py-3 rounded-xl hover:bg-slate-800 transition flex items-center justify-center gap-2"
              >
                {submitting ? (
                  <>
                    <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z" />
                    </svg>
                    Đang gửi...
                  </>
                ) : 'GỬI ĐÁNH GIÁ'}
              </button>
            </div>
          )}
        </div>

        <div className="lg:col-span-2">
          <h3 className="text-lg font-bold text-slate-900 mb-5">
            Đánh giá từ khách hàng
            <span className="ml-2 text-sm font-normal text-slate-400">({reviews.length})</span>
          </h3>
          {reviews.length === 0 ? (
            <div className="text-sm text-slate-500 bg-slate-50 border border-slate-100 rounded-2xl p-8 text-center">
              <div className="text-3xl mb-2">💬</div>
              Xe này chưa có đánh giá nào. Hãy là người đầu tiên chia sẻ trải nghiệm!
            </div>
          ) : (
            <div className="space-y-4">
              {reviews.map((r) => (
                <div key={r.reviewId} className="bg-white border border-slate-100 p-5 rounded-2xl shadow-sm">
                  <div className="flex items-center justify-between mb-2">
                    <div className="flex items-center gap-2">
                      <div className="w-8 h-8 rounded-full bg-[#0A2540] flex items-center justify-center text-white text-xs font-bold flex-shrink-0">
                        {r.fullName?.charAt(0)?.toUpperCase() ?? '?'}
                      </div>
                      <span className="font-bold text-slate-900 text-sm">{r.fullName}</span>
                    </div>
                    <span className="text-xs text-slate-400">
                      {new Date(r.createdAt).toLocaleDateString('vi-VN')}
                    </span>
                  </div>
                  <div className="flex text-sm mb-2 ml-10">
                    {Array.from({ length: 5 }).map((_, i) => (
                      <span key={i} className={i < r.rating ? 'text-amber-400' : 'text-slate-200'}>★</span>
                    ))}
                  </div>
                  <p className="text-slate-600 text-sm leading-relaxed ml-10">{r.comment}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  )
}

// ─────────────────────────────────────────────
// CAR API LANDING PAGE (core)
// ─────────────────────────────────────────────
function CarApiLandingPage({ carId }: { carId: number }) {
  const api = useMemo(() => axios.create({
    baseURL: `${env.VITE_API_BASE_URL}/api`,
    timeout: 20_000,
    headers: { 'ngrok-skip-browser-warning': 'true', 'Content-Type': 'application/json' },
  }), [])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string>('')
  const [meta, setMeta] = useState<(CarProductMeta & { features?: any[]; pricingVersions?: PricingVersionDto[] }) | null>(null)
  const [activeSection, setActiveSection] = useState<ActiveSection>('overview')

  const location = useLocation()
  const locationState = location.state as any

  const [orderOpen, setOrderOpen] = useState(!!locationState?.openOrder)

  const [orderStep, setOrderStep] = useState(locationState?.openOrder ? 4 : 1)
  const [orderFormData, setOrderFormData] = useState({
    fullName: locationState?.prefill?.fullName ?? '',
    phone: locationState?.prefill?.phone ?? '',
    email: '',
    customerNote: '',
    promotionCode: ''
  })
  const [orderSelectedVersion, setOrderSelectedVersion] = useState<PricingVersionDto | null>(null)
  const [orderDiscountPercent, setOrderDiscountPercent] = useState(0)
  const [orderDiscountAmount, setOrderDiscountAmount] = useState(0)
  const [orderPromoMessage, setOrderPromoMessage] = useState({ type: '', text: '' })
  const [orderSelectedAccessories, setOrderSelectedAccessories] = useState<Accessory[]>([])
  // ✅ State showroom
  const [orderSelectedShowroom, setOrderSelectedShowroom] = useState<ShowroomPickerDto | null>(null)
  const [orderPayosUrl, setOrderPayosUrl] = useState('')
  const [orderCreatedCode, setOrderCreatedCode] = useState('')
  const [orderCreatedId, setOrderCreatedId] = useState<number | null>(null)
  const [orderPaymentStatus, setOrderPaymentStatus] = useState<'pending'|'paid'|'error'>('pending')

  useEffect(() => {
    let cancelled = false
    const controller = new AbortController()

    async function load() {
      setLoading(true); setError(''); setMeta(null)
      try {
        const res = await api.get<ApiEnvelope<CustomerCarDetailDto>>(`Cars/${carId}`, { signal: controller.signal })
        const payload = res.data?.data ?? null
        if (cancelled || !payload) { if (!cancelled) setMeta(null); return }

        const heroImage = (payload.imageUrl ?? '').trim()
        const imageSrc = heroImage ? new URL(heroImage, env.VITE_API_BASE_URL).toString() : ''
        const priceText = payload.pricingVersions?.length
          ? formatVnd(Math.min(...payload.pricingVersions.map((v) => v.priceVnd)))
          : formatVnd(payload.price)
        const specsRows = (payload.specifications ?? [])
          .flatMap((g) => (g.items ?? []).map((i) => ({ category: g.category ?? '', label: i.specName ?? '', value: i.specValue ?? '' })))
          .filter((r) => (r.label || r.value) && r.label.trim() !== '')
        const carName = payload.name ?? ''
        const galleryAll: CarProductGallerySlide[] = (payload.galleryImages ?? [])
          .flatMap((g) => g.images ?? [])
          .map((img) => {
            const src = toAbsoluteUrl(img.imageUrl ?? '')
            const title = (img.title ?? '').replace(/\r\n/g, '\n').trim()
            const description = (img.description ?? '').replace(/\r\n/g, '\n').trim()
            return { src, alt: title || carName, title: title || null, description: description || null }
          })
          .filter((i) => i.src.length > 0)
        const colorImages = findGalleryGroup(payload.galleryImages, 'Color')
        const colorGallery: CarProductColorGalleryItem[] = colorImages
          .map((img, idx) => ({ id: `color-${idx}`, label: (img.title ?? '').trim() || `Màu ${idx + 1}`, imageSrc: toAbsoluteUrl(img.imageUrl ?? '') }))
          .filter((c) => c.imageSrc.length > 0)
        const pricingRows = (payload.pricingVersions ?? [])
          .slice().sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
          .map((v) => ({ name: v.name?.trim() || 'Phiên bản', priceText: formatVnd(v.priceVnd) }))

        setMeta({
          name: carName, imageSrc, priceText,
          features: payload.features ?? [],
          pricingVersions: payload.pricingVersions ?? [],
          content: {
            overviewIntro: payload.description ?? '', overviewHighlights: [],
            exteriorIntro: '', exteriorBullets: [],
            interiorIntro: '', interiorBullets: [],
            safetyIntro: '', safetyBullets: [],
            performanceIntro: '', performanceBullets: [],
            specsTitle: 'Thông số kỹ thuật', specsRows,
            gallery: galleryAll, galleryAll, pricingRows, colorGallery,
            overviewSplit: buildSectionSplit(payload.galleryImages, 'Overview', (payload.description ?? '').trim() || 'Thông tin tổng quan được cập nhật theo hình ảnh và mô tả từ showroom.', carName),
            exteriorSplit: buildSectionSplit(payload.galleryImages, 'Exterior', 'Ngoại thất: hình ảnh và mô tả theo từng chi tiết.', carName),
            interiorSplit: buildSectionSplit(payload.galleryImages, 'Interior', 'Nội thất: hình ảnh và trang bị theo dữ liệu cập nhật.', carName),
            safetySplit: buildSectionSplit(payload.galleryImages, 'Safety', 'An toàn: hình ảnh minh họa và mô tả tính năng.', carName),
            performanceSplit: buildSectionSplit(payload.galleryImages, 'Performance', 'Vận hành: hình ảnh và mô tả hiệu năng.', carName),
          },
        })
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Không thể tải chi tiết xe')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => { cancelled = true; controller.abort() }
  }, [api, carId])

  if (loading) {
    return (
      <main className="w-full bg-white">
        <div className="mx-auto w-full max-w-screen-2xl px-6 py-12 text-sm text-slate-500">Đang tải...</div>
      </main>
    )
  }
  if (error) {
    return (
      <main className="w-full bg-white">
        <div className="mx-auto w-full max-w-screen-2xl px-6 py-12">
          <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div>
        </div>
      </main>
    )
  }
  if (!meta) return <NotFoundPage />

  return (
    <main className="w-full bg-white">
      <Breadcrumbs carName={meta.name} />

      <SectionNav
        active={activeSection}
        onChange={setActiveSection}
        onOpenOrder={() => setOrderOpen(true)}
      />

      {activeSection === 'overview' && <CarProductLanding {...meta} />}
      {activeSection === 'reviews' && <CarReviews carId={carId} />}
      {activeSection === 'rolling-cost' && <CarRollingCostCalculator basePrice={meta.pricingVersions?.[0]?.priceVnd ?? 0} carName={meta.name} />}

      <OrderModal
        open={orderOpen}
        onClose={() => setOrderOpen(false)}
        carId={carId}
        carName={meta.name}
        pricingVersions={meta.pricingVersions ?? []}
        externalState={{
          step: orderStep, setStep: setOrderStep,
          formData: orderFormData, setFormData: setOrderFormData,
          selectedVersion: orderSelectedVersion ?? meta.pricingVersions?.[0] ?? null,
          setSelectedVersion: setOrderSelectedVersion,
          discountPercent: orderDiscountPercent, setDiscountPercent: setOrderDiscountPercent,
          discountAmount: orderDiscountAmount, setDiscountAmount: setOrderDiscountAmount,
          promoMessage: orderPromoMessage, setPromoMessage: setOrderPromoMessage,
          selectedAccessories: orderSelectedAccessories, setSelectedAccessories: setOrderSelectedAccessories,
          // ✅ Truyền showroom state
          selectedShowroom: orderSelectedShowroom, setSelectedShowroom: setOrderSelectedShowroom,
          payosUrl: orderPayosUrl, setPayosUrl: setOrderPayosUrl,
          createdOrderCode: orderCreatedCode, setCreatedOrderCode: setOrderCreatedCode,
          createdOrderId: orderCreatedId, setCreatedOrderId: setOrderCreatedId,
          paymentStatus: orderPaymentStatus, setPaymentStatus: setOrderPaymentStatus,
        }}
      />
    </main>
  )
}

// ─────────────────────────────────────────────
// EXPORT CHÍNH
// ─────────────────────────────────────────────
export function CarProductPage() {
  const { carId } = useParams<{ carId: string }>()

  if (!carId) return <Navigate to="/san-pham" replace />

  if (/^\d+$/.test(carId)) {
    return <CarApiLandingPage carId={Number(carId)} />
  }

  const meta = CAR_PRODUCT_CATALOG[carId]
  if (!meta) return <NotFoundPage />

  return (
    <main className="w-full bg-white">
      <CarProductLanding {...meta} />
    </main>
  )
}