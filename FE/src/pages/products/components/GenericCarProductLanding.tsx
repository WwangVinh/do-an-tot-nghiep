import { Check, Download } from 'lucide-react'
import { useCallback } from 'react'
import { Link } from 'react-router-dom'
import toast from 'react-hot-toast'

import banner2 from '../../../assets/images/banner2.jpg'
import { BannerCarousel } from '../../../components/BannerCarousel'
import type { CarProductMeta } from '../carProductCatalog'

const NAVY = '#1a3a5a'
const BLUE_BTN = '#1e4a7a'

const genericHighlights = [
  'Xe điện VinFast — chính sách bảo hành & hỗ trợ theo hãng',
  'Tư vấn trả góp, giao xe và thủ tục đăng ký tận nơi',
  'Catalogue đầy đủ phiên bản & màu sắc tại showroom',
  'Đặt lịch lái thử & nhận ưu đãi mới nhất trong 24h',
]

function SectionHeading({
  tag,
  title,
  intro,
}: {
  tag: string
  title: string
  intro: string
}) {
  return (
    <div className="grid gap-6 lg:grid-cols-[auto_1fr] lg:gap-10">
      <div
        className="flex h-12 min-w-[3.5rem] items-center justify-center rounded bg-red-600 px-3 text-xs font-bold uppercase tracking-wider text-white sm:text-sm"
        aria-hidden
      >
        {tag}
      </div>
      <div className="space-y-4">
        <h2 className="text-2xl font-bold tracking-tight sm:text-3xl" style={{ color: NAVY }}>
          {title}
        </h2>
        <p className="max-w-3xl whitespace-pre-line text-[15px] leading-relaxed text-slate-700 sm:text-base">{intro}</p>
      </div>
    </div>
  )
}

const exteriorBullets = [
  'Thiết kế nhận diện thương hiệu VinFast — hiện đại, tối ưu khí động học',
  'Cụm đèn LED đặc trưng, tăng khả năng quan sát trong nhiều điều kiện',
  'Mâm hợp kim và đường gân thân xe tạo vẻ khỏe khoắn (tùy phiên bản)',
]

const interiorBullets = [
  'Khoang lái tối giản, tập trung vào người dùng và trải nghiệm công nghệ',
  'Hàng ghế rộng rãi, tối ưu sự thoải mái cho gia đình (tùy dòng xe)',
  'Kết nối điện thoại và tiện nghi giải trí theo trang bị từng phiên bản',
]

const safetyBullets = [
  'Các hệ thống an toàn chủ động/bị động theo tiêu chuẩn của hãng',
  'ABS/EBD/ESC và hỗ trợ phanh, hỗ trợ khởi hành ngang dốc (tùy phiên bản)',
  'ADAS có thể bao gồm cảnh báo va chạm, hỗ trợ giữ làn, ga tự động thích ứng (tùy phiên bản)',
]

const performanceBullets = [
  'Động cơ điện phản hồi nhanh, vận hành êm ái trong đô thị',
  'Hệ thống treo và khung gầm được tinh chỉnh cho độ ổn định',
  'Hỗ trợ sạc AC/DC theo tiêu chuẩn trạm sạc của hãng (tùy mẫu xe)',
]

export function GenericCarProductLanding({ name, imageSrc, priceText }: CarProductMeta) {
  const onBrochure = useCallback(() => {
    toast.success('Brochure sẽ được gửi qua email khi có kết nối hệ thống.')
  }, [])
  const stickyHeaderOffsetClass = 'top-[80px]'

  return (
    <div className="w-full bg-white text-slate-900">
      <section className="border-b border-slate-100 bg-white">
        <div className="mx-auto w-full max-w-screen-2xl px-5 py-10 sm:px-6 lg:px-8 lg:py-14">
          <div className="grid gap-10 lg:grid-cols-12 lg:items-start lg:gap-8 xl:gap-12">
            <div className="lg:col-span-5">
              <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                <img
                  src={imageSrc}
                  alt={name}
                  className="aspect-[4/3] w-full object-contain p-6 sm:aspect-[16/10]"
                  loading="eager"
                />
              </div>
              <p className="mt-4 text-sm text-slate-600">
                Giá từ{' '}
                <span className="font-semibold text-rose-600">{priceText}</span> — tham khảo bảng giá niêm yết
                và ưu đãi tháng.
              </p>
            </div>

            <div className="lg:col-span-4">
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl" style={{ color: NAVY }}>
                {name}
              </h1>
              <ul className="mt-6 space-y-3">
                {genericHighlights.map((line) => (
                  <li key={line} className="flex gap-3 text-[15px] leading-relaxed text-slate-800 sm:text-base">
                    <span className="mt-0.5 shrink-0 text-red-600">
                      <Check className="h-5 w-5" strokeWidth={2.5} aria-hidden />
                    </span>
                    <span>{line}</span>
                  </li>
                ))}
              </ul>
            </div>

            <div className="flex flex-col gap-4 lg:col-span-3">
              <div
                className="rounded-lg px-5 py-6 text-center text-white shadow-lg"
                style={{
                  background: 'linear-gradient(145deg, #9f1239 0%, #7f1d1d 55%, #450a0a 100%)',
                }}
              >
                <p className="text-lg font-extrabold uppercase tracking-[0.12em] sm:text-xl">
                  Nhận báo giá
                </p>
                <p className="mt-2 text-sm text-white/90">
                  Để lại thông tin — chúng tôi gửi báo giá & ưu đãi trong 24h.
                </p>
                <a
                  href="tel:1900123456"
                  className="mt-5 inline-flex h-11 w-full items-center justify-center rounded-md bg-white text-sm font-bold uppercase tracking-wide text-red-900 transition hover:bg-red-50"
                >
                  Gọi hotline
                </a>
              </div>
              <Link
                to="/lai-thu"
                className="inline-flex h-12 w-full items-center justify-center rounded-md text-sm font-bold uppercase tracking-wide text-white transition hover:opacity-95"
                style={{ backgroundColor: BLUE_BTN }}
              >
                Đăng ký lái thử
              </Link>
              <button
                type="button"
                onClick={onBrochure}
                className="inline-flex h-12 w-full items-center justify-center gap-2 rounded-md text-sm font-bold uppercase tracking-wide text-white transition hover:opacity-95"
                style={{ backgroundColor: BLUE_BTN }}
              >
                <Download className="h-4 w-4" aria-hidden />
                Tải brochure
              </button>
            </div>
          </div>
        </div>
      </section>

      <nav
        className={`sticky ${stickyHeaderOffsetClass} z-40 border-b border-white/10 shadow-sm`}
        style={{ backgroundColor: NAVY }}
        aria-label={`Mục lục ${name}`}
      >
        <div className="mx-auto flex max-w-screen-2xl flex-wrap items-center justify-center gap-1 px-3 py-2 sm:gap-2 sm:px-6 lg:px-8">
          {[
            ['#car-tong-quan', 'Tổng quan'],
            ['#car-ngoai-that', 'Ngoại thất'],
            ['#car-noi-that', 'Nội thất'],
            ['#car-an-toan', 'An toàn'],
            ['#car-van-hanh', 'Vận hành'],
            ['#car-thong-so', 'Thông số'],
            ['#car-hinh-anh', 'Hình ảnh'],
          ].map(([href, label]) => (
            <a
              key={href}
              href={href}
              className="rounded px-3 py-2 text-[11px] font-semibold uppercase tracking-wide text-white/95 transition hover:bg-white/10 sm:text-xs"
            >
              {label}
            </a>
          ))}
          <Link
            to="/san-pham"
            className="rounded px-3 py-2 text-[11px] font-semibold uppercase tracking-wide text-white/95 transition hover:bg-white/10 sm:text-xs"
          >
            ← Danh sách xe
          </Link>
        </div>
      </nav>

      <section id="car-tong-quan" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-6 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="01"
            title={`Tổng quan ${name}`}
            intro={`Quý khách quan tâm tới ${name} vui lòng liên hệ showroom để được tư vấn phiên bản, màu sắc, thời gian giao xe và chương trình ưu đãi. Đội ngũ tư vấn sẽ phản hồi nhanh theo nhu cầu của Quý khách.`}
          />
          <div className="max-w-lg overflow-hidden rounded-lg border border-slate-200 bg-slate-100/90">
            <dl className="divide-y divide-slate-200/90">
              <div className="grid gap-1 px-4 py-3.5 sm:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)] sm:gap-6 sm:px-6">
                <dt className="text-sm font-semibold text-slate-800">Giá từ (tham khảo)</dt>
                <dd className="text-sm text-slate-700">{priceText}</dd>
              </div>
              <div className="grid gap-1 px-4 py-3.5 sm:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)] sm:gap-6 sm:px-6">
                <dt className="text-sm font-semibold text-slate-800">Trạng thái</dt>
                <dd className="text-sm text-slate-700">Đang mở bán — liên hệ báo giá & lái thử</dd>
              </div>
            </dl>
          </div>
        </div>
      </section>

      <section id="car-ngoai-that" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="02"
            title="Ngoại thất"
            intro="Thiết kế hiện đại và các chi tiết nhận diện thương hiệu giúp chiếc xe nổi bật trên mọi cung đường."
          />
          <div className="grid gap-8 lg:grid-cols-12 lg:items-start">
            <div className="lg:col-span-7">
              <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                <img src={imageSrc} alt={`${name} — ngoại thất`} className="aspect-[16/9] w-full object-contain p-6" loading="lazy" />
              </div>
            </div>
            <div className="lg:col-span-5">
              <h3 className="text-lg font-bold" style={{ color: NAVY }}>
                Điểm nhấn ngoại thất
              </h3>
              <ul className="mt-4 list-disc space-y-2 pl-5 text-[15px] leading-relaxed text-slate-700 sm:text-base">
                {exteriorBullets.map((b) => (
                  <li key={b}>{b}</li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      </section>

      <section id="car-noi-that" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="03"
            title="Nội thất"
            intro="Không gian tối ưu trải nghiệm người dùng với các tiện nghi và kết nối thông minh theo từng phiên bản."
          />
          <div className="grid gap-8 lg:grid-cols-12 lg:items-start">
            <div className="lg:col-span-7">
              <div className="overflow-hidden rounded-lg bg-slate-100 ring-1 ring-black/5">
                <img src={banner2} alt={`${name} — nội thất`} className="aspect-[16/9] w-full object-cover" loading="lazy" />
              </div>
            </div>
            <div className="lg:col-span-5">
              <h3 className="text-lg font-bold" style={{ color: NAVY }}>
                Tiện nghi nổi bật
              </h3>
              <ul className="mt-4 list-disc space-y-2 pl-5 text-[15px] leading-relaxed text-slate-700 sm:text-base">
                {interiorBullets.map((b) => (
                  <li key={b}>{b}</li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      </section>

      <section id="car-an-toan" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="04"
            title="An toàn"
            intro="Tập trung bảo vệ người lái và hành khách với các hệ thống hỗ trợ chủ động và trang bị an toàn tiêu chuẩn."
          />
          <div className="max-w-4xl">
            <ul className="list-disc space-y-2 pl-5 text-[15px] leading-relaxed text-slate-700 sm:text-base">
              {safetyBullets.map((b) => (
                <li key={b}>{b}</li>
              ))}
            </ul>
            <p className="mt-4 text-xs text-slate-500">
              Trang bị có thể thay đổi theo phiên bản và thời điểm mở bán.
            </p>
          </div>
        </div>
      </section>

      <section id="car-van-hanh" className="scroll-mt-28 border-b border-slate-100 bg-slate-50/80">
        <div className="mx-auto max-w-screen-2xl space-y-10 px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <SectionHeading
            tag="05"
            title="Vận hành"
            intro="Vận hành êm, phản hồi nhanh và tối ưu cho di chuyển hằng ngày; phù hợp cả đô thị và đường trường."
          />
          <div className="max-w-4xl">
            <ul className="list-disc space-y-2 pl-5 text-[15px] leading-relaxed text-slate-700 sm:text-base">
              {performanceBullets.map((b) => (
                <li key={b}>{b}</li>
              ))}
            </ul>
          </div>
        </div>
      </section>

      <section id="car-thong-so" className="scroll-mt-28 border-b border-slate-100 bg-white">
        <div className="mx-auto max-w-screen-2xl px-5 py-14 sm:px-6 lg:px-8 lg:py-16">
          <div
            className="overflow-hidden rounded-t-lg px-4 py-3 text-center text-sm font-bold uppercase tracking-wider text-white sm:text-base"
            style={{ background: `linear-gradient(90deg, ${NAVY}, #2d5a87)` }}
          >
            Thông số
          </div>
          <div className="overflow-hidden rounded-b-lg border border-t-0 border-slate-200 bg-slate-100/90">
            <dl className="divide-y divide-slate-200/90">
              <div className="grid gap-1 px-4 py-3.5 sm:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)] sm:gap-6 sm:px-6">
                <dt className="text-sm font-semibold text-slate-800">Tên xe</dt>
                <dd className="text-sm text-slate-700">{name}</dd>
              </div>
              <div className="grid gap-1 px-4 py-3.5 sm:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)] sm:gap-6 sm:px-6">
                <dt className="text-sm font-semibold text-slate-800">Giá từ (tham khảo)</dt>
                <dd className="text-sm text-slate-700">{priceText}</dd>
              </div>
              <div className="grid gap-1 px-4 py-3.5 sm:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)] sm:gap-6 sm:px-6">
                <dt className="text-sm font-semibold text-slate-800">Tư vấn phiên bản</dt>
                <dd className="text-sm text-slate-700">Liên hệ showroom để nhận catalogue & cấu hình chi tiết</dd>
              </div>
            </dl>
            <p className="border-t border-slate-200/90 px-4 py-3 text-xs text-slate-500 sm:px-6">
              Thông số tham khảo; số liệu chính thức theo catalogue và hợp đồng mua bán tại thời điểm giao xe.
            </p>
          </div>
        </div>
      </section>

      <section id="car-hinh-anh" className="scroll-mt-28 bg-slate-50/80 pb-16 pt-12 sm:pb-20 sm:pt-14">
        <div className="mx-auto max-w-screen-2xl px-5 sm:px-6 lg:px-8">
          <h2 className="text-center text-2xl font-bold tracking-tight sm:text-3xl" style={{ color: NAVY }}>
            Hình ảnh {name}
          </h2>
          <div className="mt-8 overflow-hidden rounded-lg bg-white shadow-md ring-1 ring-black/5">
            <BannerCarousel
              slides={[
                { src: imageSrc, alt: `${name} — ảnh 1` },
                { src: banner2, alt: `${name} — showroom` },
              ]}
              intervalMs={5000}
            />
          </div>
        </div>
      </section>
    </div>
  )
}
