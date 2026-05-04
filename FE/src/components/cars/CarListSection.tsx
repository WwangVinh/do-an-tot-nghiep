import { CarCard, type CarCardAction } from './CarCard'

export type CarListItem = {
  id: string
  name: string
  priceText?: string
  imageSrc?: string
  imageAlt?: string
  to?: string
  detailLink?: boolean
  actions?: [CarCardAction, CarCardAction?]
  year?: number | null
  condition?: string | null
}

export type CarListSectionProps = {
  title: string
  items: CarListItem[]
  className?: string
  containerClassName?: string
}

export function CarListSection({ title, items, className, containerClassName }: CarListSectionProps) {
  if (!items?.length) return null

  return (
    <section className={['w-full bg-slate-50 py-10 sm:py-12', className ?? ''].join(' ')}>
      <div className={['mx-auto w-full max-w-screen-2xl px-6', containerClassName ?? ''].join(' ')}>
        <div className="flex flex-col items-center">
          <h2 className="text-center text-2xl font-semibold text-slate-800">{title}</h2>
          <div className="mt-3 h-0.5 w-14 rounded-full bg-rose-500" aria-hidden="true" />
        </div>

        <div className="mt-8 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {items.map((c) => (
            <CarCard
              key={c.id}
              name={c.name}
              priceText={c.priceText}
              imageSrc={c.imageSrc}
              imageAlt={c.imageAlt}
              to={c.detailLink === false ? undefined : (c.to ?? `/san-pham/xe/${c.id}`)}
              actions={c.actions}
              year={c.year}
              condition={c.condition}
            />
          ))}
        </div>
      </div>
    </section>
  )
}