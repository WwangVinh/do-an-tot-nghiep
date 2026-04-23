import { BannerCarousel } from '../../components/BannerCarousel'
import { BannerQuickActions } from '../../components/BannerQuickActions'
import { QuoteRegisterSection } from '../../components/QuoteRegisterSection'
import { CarListSection, type CarListItem } from '../../components/cars/CarListSection'
import { HomeCommitmentSection } from './components/HomeCommitmentSection'
import { HomePromoCard } from './components/HomePromoCard'
import { useEffect, useState } from 'react'
import axios from 'axios'
import banner1 from '../../assets/images/banner1.jpg'
import banner2 from '../../assets/images/banner2.jpg'
import banner3 from '../../assets/images/banner3.jpg'
import banner4 from '../../assets/images/banner4.jpg'
import vf8Img from '../../assets/images/cars/vinfast-vf8-98yirhq.webp'
import { HomeQuoteRegisterBar } from './components/HomeQuoteRegisterBar'
import { env } from '../../lib/env'

type LatestCarDto = {
  carId: number
  name: string
  price: number
  imageUrl: string | null
}

export function HomePage() {
  const [latestCars, setLatestCars] = useState<CarListItem[]>([])
  const [bestSellerCars, setBestSellerCars] = useState<CarListItem[]>([])
  const bestSellerDisplayCars = bestSellerCars.length ? bestSellerCars : latestCars

  useEffect(() => {
    let cancelled = false
    const api = axios.create({
      baseURL: new URL('/api/', env.VITE_API_BASE_URL).toString(),
      timeout: 20_000,
    })

    async function load() {
      try {
        const [latestRes, bestRes] = await Promise.all([
          api.get<{ data: LatestCarDto[] }>('Cars/latest', { params: { limit: 6 } }),
          api.get<{ data: LatestCarDto[] }>('Cars/best-sellers', { params: { limit: 6 } }),
        ])

        const mapDtoToItem = (c: LatestCarDto): CarListItem => {
          const raw = (c.imageUrl ?? '').trim()
          const imageSrc = raw ? new URL(raw, env.VITE_API_BASE_URL).toString() : vf8Img
          const priceText = Number.isFinite(c.price) ? new Intl.NumberFormat('vi-VN').format(c.price) + ' ₫' : 'Liên hệ'
          return { id: String(c.carId), name: c.name, priceText, imageSrc }
        }

        const latestItems = latestRes.data?.data?.map(mapDtoToItem) ?? []
        const bestItems = bestRes.data?.data?.map(mapDtoToItem) ?? []

        if (!cancelled) {
          setLatestCars(latestItems)
          setBestSellerCars(bestItems)
        }
      } catch {
        if (!cancelled) {
          setLatestCars([])
          setBestSellerCars([])
        }
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <main className="w-full p-0">
      <div className="relative">
        <BannerCarousel
          className="m-0"
          intervalMs={2500}
          slides={[
            { src: banner1, alt: 'Banner 1' },
            { src: banner2, alt: 'Banner 2' },
            { src: banner3, alt: 'Banner 3' },
            { src: banner4, alt: 'Banner 4' },
          ]}
        />

        <div className="absolute inset-x-0 bottom-3 z-10 px-0">
          <BannerQuickActions />
        </div>
      </div>

      <HomeQuoteRegisterBar
        onSubmit={(values) => {
          // TODO: hook API later
          console.log('Quote register', values)
        }}
      />

      <HomePromoCard />
      <CarListSection title="Các dòng xe mới nhất" items={latestCars} />
      <CarListSection title="Các dòng xe bán chạy nhất" items={bestSellerDisplayCars} />

      <HomeCommitmentSection imageSrc={banner2} />

      <QuoteRegisterSection
        backgroundImageSrc={banner2}
        cardProps={{
          cars: [
            ...latestCars.map((c) => ({
              id: c.id,
              label: c.name,
            })),
            ...bestSellerDisplayCars.map((c) => ({ id: c.id, label: c.name })),
          ],
          onSubmit: (values) => {
            // TODO: hook API later
            console.log('Quote register', values)
          },
        }}
      />
    </main>
  )
}

