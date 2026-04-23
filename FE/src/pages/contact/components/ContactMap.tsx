import type { ShowroomListItem } from './ContactInfoSection'

function buildEmbedSrc(query: string): string {
  return (
    'https://www.google.com/maps?q=' +
    encodeURIComponent(query) +
    '&output=embed&hl=vi&z=15'
  )
}

export type ContactMapProps = {
  showroom?: ShowroomListItem | null
  className?: string
}

export function ContactMap({ showroom, className }: ContactMapProps) {
  const address =
    showroom?.fullAddress?.trim() ||
    showroom?.streetAddress?.trim() ||
    ''
  const embedSrc = buildEmbedSrc(address || 'Việt Nam')

  return (
    <section
      className={['w-full overflow-hidden bg-slate-200', className ?? ''].join(' ')}
      aria-label="Bản đồ vị trí showroom"
    >
      <div className="relative aspect-[21/9] min-h-[280px] w-full sm:min-h-[320px] md:aspect-[2.4/1] md:min-h-[380px]">
        <iframe
          title={showroom?.name ? `Google Maps — ${showroom.name}` : 'Google Maps'}
          src={embedSrc}
          className="absolute inset-0 h-full w-full border-0"
          loading="lazy"
          referrerPolicy="no-referrer-when-downgrade"
          allowFullScreen
        />
      </div>
    </section>
  )
}
