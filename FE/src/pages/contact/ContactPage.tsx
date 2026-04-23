import { useMemo, useState } from 'react'
import { useQuery } from '@tanstack/react-query'

import { http } from '../../services/http/http'

import { ContactInfoSection, type ShowroomListItem } from './components/ContactInfoSection'
import { ContactMap } from './components/ContactMap'

async function getShowrooms(): Promise<ShowroomListItem[]> {
  const res = await http.get<ShowroomListItem[]>('/api/showrooms')
  return Array.isArray(res.data) ? res.data : []
}

export function ContactPage() {
  const { data: showrooms = [], isLoading: isShowroomsLoading } = useQuery({
    queryKey: ['showrooms', 'contact'],
    queryFn: getShowrooms,
    staleTime: 5 * 60_000,
  })

  const [selectedShowroomId, setSelectedShowroomId] = useState<number | null>(null)

  const selectedShowroom = useMemo(() => {
    if (showrooms.length === 0) return null
    const byId = selectedShowroomId ? showrooms.find((s) => s.showroomId === selectedShowroomId) : undefined
    return byId ?? showrooms[0] ?? null
  }, [selectedShowroomId, showrooms])

  return (
    <main className="w-full bg-white">
      <ContactMap showroom={selectedShowroom} />
      <ContactInfoSection
        showrooms={showrooms}
        isShowroomsLoading={isShowroomsLoading}
        selectedShowroomId={selectedShowroom?.showroomId ?? null}
        onSelectShowroomId={setSelectedShowroomId}
      />
    </main>
  )
}
