import { useEffect, useState } from 'react'
import { Clock, Globe, Headphones, Mail, MapPin, Phone } from 'lucide-react'

import { http } from '../services/http/http'

type FooterLink = {
  label: string
  href?: string
}

export type FooterProps = {
  className?: string
  brandTitle?: string
  address?: string
  phone?: string
  email?: string
  website?: string
  supportLinks?: FooterLink[]
  copyrightText?: string
  poweredByText?: string
  /** Cột giữa (ngang hàng với “Hỗ trợ khách hàng”). Đặt rỗng middleColumnTitle và cả intro* / office* / consult* để ẩn cột — chỉ còn 2 cột trên + showroom */
  middleColumnTitle?: string
  introTitle?: string
  introSubtitle?: string
  officeHours?: string
  consultNote?: string
}

type ShowroomListItem = {
  showroomId: number
  name: string
  fullAddress?: string | null
  hotline?: string | null
}

const defaultSupportLinks: FooterLink[] = [
  { label: 'Bảng giá xe', href: '/bang-gia-xe' },
  { label: 'Mua xe trả góp', href: '/tra-gop' },
  { label: 'Đăng ký báo giá xe', href: '/bao-gia' },
  { label: 'Đăng ký lái thử', href: '/lai-thu' },
  { label: 'Liên hệ với chúng tôi', href: '/lien-he' },
]

function telHref(hotline: string): string {
  const digits = hotline.replace(/[^\d+]/g, '')
  return digits ? `tel:${digits}` : ''
}

export function Footer({
  className,
  brandTitle = 'CMC AUTOMOTIVE',
  address = 'Trụ sở: Địa chỉ trụ sở chính',
  phone = 'Hotline: 0333 436 743',
  email = 'cmcautomotive@gmail.com',
  website = 'https://cmcautomotive.vn',
  supportLinks = defaultSupportLinks,
  copyrightText = `Copyright© ${new Date().getFullYear()} - Tên công ty`,
  poweredByText = 'Powered by Readdy',
  middleColumnTitle = 'Tư vấn & giờ phục vụ',
  introTitle = 'Đồng hành tư vấn & phục vụ xe chuyên nghiệp',
  introSubtitle =
    'Hỗ trợ báo giá, thủ tục trả góp, đăng ký lái thử và kết nối bạn với showroom gần nhất.',
  officeHours = 'Giờ làm việc: Thứ 2 – Chủ nhật, 8:00 – 20:00',
  consultNote = 'Tư vấn miễn phí qua hotline và email trong giờ làm việc.',
}: FooterProps) {
  const hasCompanyExtra =
    Boolean(middleColumnTitle?.trim()) ||
    Boolean(introTitle?.trim()) ||
    Boolean(introSubtitle?.trim()) ||
    Boolean(officeHours?.trim()) ||
    Boolean(consultNote?.trim())

  const [showrooms, setShowrooms] = useState<ShowroomListItem[]>([])
  const [showroomsLoad, setShowroomsLoad] = useState<'idle' | 'loading' | 'error'>('idle')

  useEffect(() => {
    let cancelled = false
    setShowroomsLoad('loading')
    http
      .get<ShowroomListItem[]>('/api/Showrooms')
      .then((res) => {
        if (!cancelled) {
          setShowrooms(Array.isArray(res.data) ? res.data : [])
          setShowroomsLoad('idle')
        }
      })
      .catch(() => {
        if (!cancelled) {
          setShowrooms([])
          setShowroomsLoad('error')
        }
      })
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <footer
      className={[
        'w-full bg-gradient-to-b from-slate-900 to-slate-950 text-slate-100',
        className ?? '',
      ].join(' ')}
      aria-label="Footer"
    >
      <div className="mx-auto w-full max-w-screen-2xl px-4 py-16 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 gap-y-10 md:grid-cols-12 md:gap-x-6 md:gap-y-12 lg:gap-x-8 xl:gap-x-10">
          <div className={hasCompanyExtra ? 'md:col-span-4' : 'md:col-span-6'}>
            <div className="flex items-start gap-3">
              <div className="mt-1 h-8 w-8 rounded-full bg-white/10 ring-1 ring-white/10" aria-hidden="true" />
              <div>
                <div className="mt-2 text-lg font-extrabold text-white">{brandTitle}</div>
              </div>
            </div>

            <ul className="mt-5 space-y-3 text-sm text-slate-200">
              <li className="flex items-start gap-3">
                <MapPin className="mt-0.5 h-4 w-4 shrink-0 text-white/80" aria-hidden="true" />
                <span>{address}</span>
              </li>
              <li className="flex items-start gap-3">
                <Phone className="mt-0.5 h-4 w-4 shrink-0 text-white/80" aria-hidden="true" />
                <span>{phone}</span>
              </li>
              <li className="flex items-start gap-3">
                <Mail className="mt-0.5 h-4 w-4 shrink-0 text-white/80" aria-hidden="true" />
                <a className="underline-offset-2 hover:underline" href={`mailto:${email}`}>
                  {email}
                </a>
              </li>
              <li className="flex items-start gap-3">
                <Globe className="mt-0.5 h-4 w-4 shrink-0 text-white/80" aria-hidden="true" />
                <a className="underline-offset-2 hover:underline" href={website} target="_blank" rel="noreferrer">
                  {website}
                </a>
              </li>
            </ul>
          </div>

          {hasCompanyExtra ? (
            <div className="border-t border-white/10 pt-10 md:col-span-4 md:border-t-0 md:pl-5 md:pt-0 lg:pl-8">
              {middleColumnTitle?.trim() ? (
                <div className="text-base font-bold text-white">{middleColumnTitle.trim()}</div>
              ) : null}
              {introTitle?.trim() ? (
                <p
                  className={[
                    'text-sm font-semibold leading-snug text-white',
                    middleColumnTitle?.trim() ? 'mt-3' : 'mt-0',
                  ].join(' ')}
                >
                  {introTitle.trim()}
                </p>
              ) : null}
              {introSubtitle?.trim() ? (
                <p
                  className={[
                    'text-sm leading-relaxed text-slate-400',
                    middleColumnTitle?.trim() || introTitle?.trim() ? 'mt-2' : 'mt-0',
                  ].join(' ')}
                >
                  {introSubtitle.trim()}
                </p>
              ) : null}
              {officeHours?.trim() || consultNote?.trim() ? (
                <div className="mt-4 space-y-3">
                  {officeHours?.trim() ? (
                    <div className="flex items-start gap-2.5 text-sm text-slate-200">
                      <Clock className="mt-0.5 h-4 w-4 shrink-0 text-sky-400/90" aria-hidden="true" />
                      <span>{officeHours.trim()}</span>
                    </div>
                  ) : null}
                  {consultNote?.trim() ? (
                    <div className="flex items-start gap-2.5 text-sm text-slate-200">
                      <Headphones className="mt-0.5 h-4 w-4 shrink-0 text-sky-400/90" aria-hidden="true" />
                      <span>{consultNote.trim()}</span>
                    </div>
                  ) : null}
                </div>
              ) : null}
            </div>
          ) : null}

          <div
            className={[
              'md:pl-0',
              hasCompanyExtra ? 'md:col-span-4 border-t border-white/10 pt-10 md:border-t-0 md:pt-0' : 'md:col-span-6',
            ].join(' ')}
          >
            <div className="md:ml-auto md:mr-9 md:w-max md:max-w-full lg:mr-12">
              <div className="text-base font-bold text-white">Hỗ trợ khách hàng</div>
              <ul className="mt-4 space-y-2 text-sm text-slate-200">
                {supportLinks.map((l) => (
                  <li key={l.label} className="flex items-start gap-2">
                    <span className="mt-1.5 h-1 w-1 shrink-0 rounded-full bg-white/70" aria-hidden="true" />
                    {l.href ? (
                      <a className="hover:text-white" href={l.href}>
                        {l.label}
                      </a>
                    ) : (
                      <span>{l.label}</span>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          </div>

          <div className="border-t border-white/10 pt-10 md:col-span-12 md:border-t-0 md:pt-0">
            <div className="text-base font-bold text-white">Showroom và đại lý phân phối</div>
            <ul className="mt-4 grid grid-cols-1 gap-x-6 gap-y-2 text-sm text-slate-200 sm:grid-cols-2 md:grid-cols-4 md:gap-x-6 lg:gap-x-8 xl:gap-x-10">
              {showroomsLoad === 'loading' ? (
                <li className="col-span-full text-slate-400">Đang tải danh sách…</li>
              ) : showroomsLoad === 'error' ? (
                <li className="col-span-full text-slate-400">Không tải được danh sách showroom.</li>
              ) : showrooms.length === 0 ? (
                <li className="col-span-full text-slate-400">Chưa có showroom.</li>
              ) : (
                showrooms.map((s) => {
                  const tel = s.hotline ? telHref(s.hotline) : ''
                  const title = [s.name, s.fullAddress].filter(Boolean).join(' — ')
                  return (
                    <li key={s.showroomId} className="flex min-w-0 items-start gap-2 break-words">
                      <span className="mt-1.5 h-1 w-1 shrink-0 rounded-full bg-white/70" aria-hidden="true" />
                      {tel ? (
                        <a className="hover:text-white" href={tel} title={title || undefined}>
                          {s.name}
                          {s.hotline ? <span className="block text-xs text-slate-400">{s.hotline}</span> : null}
                        </a>
                      ) : (
                        <span title={title || undefined}>{s.name}</span>
                      )}
                    </li>
                  )
                })
              )}
            </ul>
          </div>
        </div>
      </div>

      <div className="border-t border-white/10">
        <div className="mx-auto flex w-full max-w-screen-2xl flex-col gap-2 px-4 py-4 text-xs text-slate-300 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8">
          <div>{copyrightText}</div>
          <div className="text-slate-400">{poweredByText}</div>
        </div>
      </div>
    </footer>
  )
}
