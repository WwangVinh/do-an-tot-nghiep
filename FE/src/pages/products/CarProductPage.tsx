import axios from 'axios'
import { useEffect, useMemo, useState } from 'react'
import { Navigate, useParams } from 'react-router-dom'

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
      const alt = title || fallbackName
      return { src, alt, title: title || null, description: description || null }
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

function CarApiLandingPage({ carId }: { carId: number }) {
  const api = useMemo(() => {
    return axios.create({
      // Nối trực tiếp link ngrok với tiền tố /api
      baseURL: `${env.VITE_API_BASE_URL}/api`,
      timeout: 20_000,
      headers: {
        'ngrok-skip-browser-warning': 'true',
        'Content-Type': 'application/json'
      }
    })
  }, [])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string>('')
  const [meta, setMeta] = useState<(CarProductMeta & { features?: any[] }) | null>(null)

  useEffect(() => {
    let cancelled = false
    const controller = new AbortController()

    async function load() {
      setLoading(true)
      setError('')
      setMeta(null)
      try {
        const res = await api.get<ApiEnvelope<CustomerCarDetailDto>>(`Cars/${carId}`, { signal: controller.signal })
        const payload = res.data?.data ?? null
        if (cancelled) return

        if (!payload) {
          setMeta(null)
          return
        }

        const heroImage = (payload.imageUrl ?? '').trim()
        const imageSrc = heroImage ? new URL(heroImage, env.VITE_API_BASE_URL).toString() : ''

        const priceText =
          payload.pricingVersions?.length
            ? formatVnd(Math.min(...payload.pricingVersions.map((v) => v.priceVnd)))
            : formatVnd(payload.price)

        const specsRows =
          (payload.specifications ?? [])
            .flatMap((g) =>
              (g.items ?? []).map((i) => ({
                category: g.category ?? '',
                label: i.specName ?? '',
                value: i.specValue ?? '',
              })),
            )
            .filter((r) => (r.label || r.value) && r.label.trim() !== '')

        const carName = payload.name ?? ''

        const galleryAll: CarProductGallerySlide[] = (payload.galleryImages ?? [])
          .flatMap((g) => g.images ?? [])
          .map((img) => {
            const src = toAbsoluteUrl(img.imageUrl ?? '')
            const title = (img.title ?? '').replace(/\r\n/g, '\n').trim()
            const description = (img.description ?? '').replace(/\r\n/g, '\n').trim()
            return {
              src,
              alt: title || carName,
              title: title || null,
              description: description || null,
            }
          })
          .filter((i) => i.src.length > 0)

        const colorImages = findGalleryGroup(payload.galleryImages, 'Color')
        const colorGallery: CarProductColorGalleryItem[] = colorImages
          .map((img, idx) => {
            const imageSrc = toAbsoluteUrl(img.imageUrl ?? '')
            const label = (img.title ?? '').trim() || `Màu ${idx + 1}`
            return {
              id: `color-${idx}-${label}`,
              label,
              imageSrc,
            }
          })
          .filter((c) => c.imageSrc.length > 0)

        const pricingRows =
          (payload.pricingVersions ?? [])
            .slice()
            .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
            .map((v) => ({
              name: v.name?.trim() || 'Phiên bản',
              priceText: formatVnd(v.priceVnd),
            }))

        const overviewSplit = buildSectionSplit(
          payload.galleryImages,
          'Overview',
          (payload.description ?? '').trim() ||
            'Thông tin tổng quan được cập nhật theo hình ảnh và mô tả từ showroom.',
          carName,
        )
        const exteriorSplit = buildSectionSplit(
          payload.galleryImages,
          'Exterior',
          'Ngoại thất: hình ảnh và mô tả theo từng chi tiết.',
          carName,
        )
        const interiorSplit = buildSectionSplit(
          payload.galleryImages,
          'Interior',
          'Nội thất: hình ảnh và trang bị theo dữ liệu cập nhật.',
          carName,
        )
        const safetySplit = buildSectionSplit(
          payload.galleryImages,
          'Safety',
          'An toàn: hình ảnh minh họa và mô tả tính năng.',
          carName,
        )
        const performanceSplit = buildSectionSplit(
          payload.galleryImages,
          'Performance',
          'Vận hành: hình ảnh và mô tả hiệu năng.',
          carName,
        )

        setMeta({
          name: carName,
          imageSrc,
          priceText,
          features: payload.features ?? [],
          content: {
            overviewIntro: payload.description ?? '',
            overviewHighlights: [],
            exteriorIntro: '',
            exteriorBullets: [],
            interiorIntro: '',
            interiorBullets: [],
            safetyIntro: '',
            safetyBullets: [],
            performanceIntro: '',
            performanceBullets: [],
            specsTitle: 'Thông số kỹ thuật',
            specsRows,
            gallery: galleryAll,
            galleryAll,
            pricingRows,
            colorGallery,
            overviewSplit,
            exteriorSplit,
            interiorSplit,
            safetySplit,
            performanceSplit,
          },
        })
      } catch (e) {
        const message = e instanceof Error ? e.message : 'Không thể tải chi tiết xe'
        if (!cancelled) setError(message)
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => {
      cancelled = true
      controller.abort()
    }
  }, [api, carId])

  if (loading) {
    return (
      <main className="w-full bg-white">
        <div className="mx-auto w-full max-w-screen-2xl px-6 py-12 text-sm font-medium text-slate-600">Đang tải...</div>
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
      <CarProductLanding {...meta} />
    </main>
  )
}

export function CarProductPage() {
  const { carId } = useParams<{ carId: string }>()

  if (!carId) {
    return <Navigate to="/san-pham" replace />
  }

  // Nếu carId là số => trang chi tiết xe từ API (DB)
  if (/^\d+$/.test(carId)) {
    return <CarApiLandingPage carId={Number(carId)} />
  }

  const meta = CAR_PRODUCT_CATALOG[carId]
  if (!meta) {
    return <NotFoundPage />
  }

  return (
    <main className="w-full bg-white">
      <CarProductLanding {...meta} />
    </main>
  )
}
