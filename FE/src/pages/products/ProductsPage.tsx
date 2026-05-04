import { useEffect, useMemo, useRef, useState } from 'react'

import { CarListSection, type CarListItem } from '../../components/cars/CarListSection'
import vf8Img from '../../assets/images/cars/vinfast-vf8-98yirhq.webp'
import axios from 'axios'
import { env } from '../../lib/env'
import {
  ProductFilters,
  type BodyType,
  type CarCondition,
  type FuelType,
  type ProductFilterState,
  type Transmission,
} from './components/ProductFilters'

type CustomerCarListDto = {
  carId: number
  name: string
  brand: string | null
  model?: string | null
  year: number | null
  condition: string | null
  price: number | null
  imageUrl: string | null
  status: string | null
  mileage: number | null
  fuelType: string | null
  transmission: string | null
  bodyStyle: string | null
  totalQuantity: number
  showrooms: string
}

type PagedCarsResponse = {
  totalItems: number
  currentPage: number
  pageSize: number
  totalPages: number
  data: CustomerCarListDto[]
}

function useDebouncedValue<T>(value: T, delayMs: number) {
  const [debounced, setDebounced] = useState(value)

  useEffect(() => {
    const t = window.setTimeout(() => setDebounced(value), delayMs)
    return () => window.clearTimeout(t)
  }, [delayMs, value])

  return debounced
}

function fuelTypeToApiValue(v: FuelType) {
  switch (v) {
    case 'dien':
      return 'Điện'
    case 'xang':
      return 'Xăng'
    case 'dau':
      return 'Dầu'
    case 'hybrid':
      return 'Hybrid'
  }
}

function conditionToApiValue(v: CarCondition) {
  switch (v) {
    case 'moi':
      return 'New'
    case 'cu':
      return 'Used'
  }
}

function transmissionToApiValue(v: Transmission) {
  switch (v) {
    case 'so-tu-dong':
      return 'Số tự động'
    case 'so-san':
      return 'Số sàn'
  }
}

function bodyStyleToApiValue(v: BodyType) {
  switch (v) {
    case 'sedan':
      return 'Sedan'
    case 'suv':
      return 'SUV'
    case 'hatchback':
      return 'Hatchback'
    case 'pickup':
      return 'Bán tải'
    case 'mpv':
      return 'MPV'
  }
}

function formatVnd(price: number | null | undefined) {
  if (!Number.isFinite(price ?? NaN)) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN').format(price as number) + ' ₫'
}

export function ProductsPage() {
  const api = useMemo(() => {
    return axios.create({
      baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
      timeout: 20_000,
    })
  }, [])

  const [filters, setFilters] = useState<ProductFilterState>({
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

  const debouncedFilters = useDebouncedValue(filters, 350)

  const [page, setPage] = useState(1)
  const pageSize = 12
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string>('')
  const [totalItems, setTotalItems] = useState(0)
  const [cars, setCars] = useState<CustomerCarListDto[]>([])
  const [optionSeed, setOptionSeed] = useState<CustomerCarListDto[]>([])

  const prevFilterKeyRef = useRef<string>('')

  // Seed options once (brands/models/years) from the API
  useEffect(() => {
    let cancelled = false
    const controller = new AbortController()

    async function loadSeed() {
      try {
        const res = await api.get<PagedCarsResponse>('Cars', {
          signal: controller.signal,
          params: { page: 1, pageSize: 100, inStockOnly: false },
        })
        const data = res.data?.data ?? []
        if (!cancelled) setOptionSeed(data)
      } catch {
        if (!cancelled) setOptionSeed([])
      }
    }

    loadSeed()
    return () => {
      cancelled = true
      controller.abort()
    }
  }, [api])

  // Fetch list when filters/page changes
  useEffect(() => {
    let cancelled = false
    const controller = new AbortController()

    async function load() {
      const filterKey = JSON.stringify(debouncedFilters)
      const prevKey = prevFilterKeyRef.current
      const filtersChanged = prevKey !== '' && prevKey !== filterKey

      // If filters changed, force page=1 first, then fetch.
      if (filtersChanged && page !== 1) {
        setPage(1)
        return
      }

      setLoading(true)
      setError('')

      const params: Record<string, unknown> = {
        page,
        pageSize,
        inStockOnly: false,
        sort: debouncedFilters.sort,
      }

      if (debouncedFilters.brand) params.brand = debouncedFilters.brand
      if (debouncedFilters.fuelType) params.fuelType = fuelTypeToApiValue(debouncedFilters.fuelType)
      if (debouncedFilters.condition) params.condition = conditionToApiValue(debouncedFilters.condition)
      if (debouncedFilters.transmission) params.transmission = transmissionToApiValue(debouncedFilters.transmission)
      if (debouncedFilters.bodyType) params.bodyStyle = bodyStyleToApiValue(debouncedFilters.bodyType)
      if (debouncedFilters.yearMin !== '') params.minYear = debouncedFilters.yearMin
      if (debouncedFilters.yearMax !== '') params.maxYear = debouncedFilters.yearMax
      if (debouncedFilters.priceMinMillions !== '') params.minPrice = debouncedFilters.priceMinMillions * 1_000_000
      if (debouncedFilters.priceMaxMillions !== '') params.maxPrice = debouncedFilters.priceMaxMillions * 1_000_000

      // Backend chưa có param model riêng: dùng search theo model/name
      if (debouncedFilters.model) params.search = debouncedFilters.model

      if (debouncedFilters.searchText) params.search = debouncedFilters.searchText

      try {
        const res = await api.get<PagedCarsResponse>('Cars', { signal: controller.signal, params })
        const payload = res.data
        const next = payload?.data ?? []
        if (!cancelled) {
          prevFilterKeyRef.current = filterKey
          setTotalItems(payload?.totalItems ?? 0)
          setCars((prev) => (page === 1 ? next : [...prev, ...next]))
        }
      } catch (e) {
        const message = e instanceof Error ? e.message : 'Không thể tải danh sách xe'
        if (!cancelled) {
          prevFilterKeyRef.current = filterKey
          setCars(page === 1 ? [] : cars)
          setTotalItems(0)
          setError(message)
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => {
      cancelled = true
      controller.abort()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [api, debouncedFilters, page])

  const filterOptions = useMemo(() => {
    const source = optionSeed.length ? optionSeed : cars
    const brands = source.map((i) => (i.brand ?? '').trim()).filter(Boolean)
    const years = source.map((i) => i.year ?? 0).filter(Boolean)
    const seats: number[] = [] // Backend hiện chưa có field "seats"
    const pricesMillions = source
      .map((i) => (i.price ?? 0) / 1_000_000)
      .filter((x) => Number.isFinite(x) && x > 0)

    const models = source
      .filter((i) => (filters.brand ? (i.brand ?? '') === filters.brand : true))
      .map((i) => (i.model ?? '').trim())
      .filter(Boolean)

    const priceRangeMillions =
      pricesMillions.length > 0
        ? { min: Math.min(...pricesMillions), max: Math.max(...pricesMillions) }
        : undefined

    return { brands, models, years, seats, priceRangeMillions }
  }, [cars, filters.brand, optionSeed])

  const displayItems: CarListItem[] = useMemo(() => {
    return (cars ?? []).map((c) => {
      const raw = (c.imageUrl ?? '').trim()
      const imageSrc = raw ? new URL(raw, env.VITE_API_BASE_URL).toString() : vf8Img
     return {
        id: String(c.carId),
        name: c.name,
        priceText: formatVnd(c.price),
        imageSrc,
        imageAlt: c.name,
        // Thêm 2 dòng này:
        year: c.year ?? null,
        condition: c.condition ?? null,
      }
    })
  }, [cars])

  return (
    <main className="w-full bg-slate-50">
      <ProductFilters
        headingTitle="Sản phẩm"
        headingDescription="Danh mục các dòng xe đang mở bán. Chọn mẫu xe để nhận ưu đãi hoặc gọi chốt giá."
        value={filters}
        onChange={(next) => {
          const normalized = next.brand ? next : { ...next, model: '' }
          setFilters(normalized)
        }}
        options={filterOptions}
        carNames={optionSeed.map((c) => c.name).filter(Boolean)}
        resultCount={displayItems.length}
        totalCount={totalItems}
      />

      {error ? (
        <div className="mx-auto w-full max-w-screen-2xl px-6 pb-4">
          <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div>
        </div>
      ) : null}

      <CarListSection title="Tất cả sản phẩm" items={displayItems} />

      <div className="mx-auto w-full max-w-screen-2xl px-6 pb-12">
        <div className="flex items-center justify-center">
          {loading ? (
            <div className="text-sm font-medium text-slate-600">Đang tải...</div>
          ) : displayItems.length < totalItems ? (
            <button
              type="button"
              onClick={() => setPage((p) => p + 1)}
              className="inline-flex h-10 items-center justify-center rounded-xl bg-slate-900 px-5 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-rose-500/40"
            >
              Xem thêm
            </button>
          ) : (
            <div className="text-xs font-medium text-slate-500">Đã hiển thị hết</div>
          )}
        </div>
      </div>
    </main>
  )
}

