import { http } from '../http/http'

export type Showroom = {
  showroomId: number
  name: string
  province: string
  district: string
  streetAddress: string
  fullAddress?: string | null
  hotline?: string | null
}

export type ShowroomCreateInput = {
  name: string
  province: string
  district: string
  streetAddress: string
  hotline?: string | null
}

export type ShowroomCar = {
  carId: number
  name: string
  price: number
  mainImageUrl?: string | null

  brandName?: string | null
  segmentName?: string | null
  fuelTypeName?: string | null
  transmissionName?: string | null
  colorName?: string | null
  modelYear?: number | null
  origin?: string | null

  quantity: number
  displayStatus: string
}

type ApiOk<T> = {
  message?: string
  Message?: string
  data?: T
  Data?: T
}

function normalizeShowroom(raw: any): Showroom {
  return {
    showroomId: Number(raw?.ShowroomId ?? raw?.showroomId ?? 0),
    name: String(raw?.Name ?? raw?.name ?? ''),
    province: String(raw?.Province ?? raw?.province ?? ''),
    district: String(raw?.District ?? raw?.district ?? ''),
    streetAddress: String(raw?.StreetAddress ?? raw?.streetAddress ?? ''),
    fullAddress: (raw?.FullAddress ?? raw?.fullAddress ?? null) as string | null,
    hotline: (raw?.Hotline ?? raw?.hotline ?? null) as string | null,
  }
}

export async function fetchAdminShowrooms(): Promise<Showroom[]> {
  const res = await http.get('/api/admin/showrooms')
  const d = res.data
  const arr = Array.isArray(d) ? d : Array.isArray(d?.data) ? d.data : Array.isArray(d?.Data) ? d.Data : []
  return (arr as any[]).map(normalizeShowroom).filter((x) => x.showroomId > 0 || x.name.trim())
}

export async function createAdminShowroom(input: ShowroomCreateInput): Promise<{ message?: string }> {
  const payload = {
    Name: input.name,
    Province: input.province,
    District: input.district,
    StreetAddress: input.streetAddress,
    Hotline: input.hotline ?? null,
  }
  const res = await http.post<ApiOk<unknown>>('/api/admin/showrooms', payload)
  return { message: res.data?.message ?? res.data?.Message }
}

function normalizeShowroomCar(raw: any): ShowroomCar {
  return {
    carId: Number(raw?.CarId ?? raw?.carId ?? 0),
    name: String(raw?.Name ?? raw?.name ?? ''),
    price: Number(raw?.Price ?? raw?.price ?? 0),
    mainImageUrl: (raw?.MainImageUrl ?? raw?.mainImageUrl ?? null) as string | null,

    brandName: (raw?.BrandName ?? raw?.brandName ?? null) as string | null,
    segmentName: (raw?.SegmentName ?? raw?.segmentName ?? null) as string | null,
    fuelTypeName: (raw?.FuelTypeName ?? raw?.fuelTypeName ?? null) as string | null,
    transmissionName: (raw?.TransmissionName ?? raw?.transmissionName ?? null) as string | null,
    colorName: (raw?.ColorName ?? raw?.colorName ?? null) as string | null,
    modelYear: (raw?.ModelYear ?? raw?.modelYear ?? null) as number | null,
    origin: (raw?.Origin ?? raw?.origin ?? null) as string | null,

    quantity: Number(raw?.Quantity ?? raw?.quantity ?? 0),
    displayStatus: String(raw?.DisplayStatus ?? raw?.displayStatus ?? ''),
  }
}

export async function fetchAdminShowroomCars(showroomId: number): Promise<{
  message?: string
  showroomInfo?: Showroom
  inventory: ShowroomCar[]
}> {
  const res = await http.get(`/api/admin/showrooms/${showroomId}/cars`)
  const d = res.data ?? {}
  const inventoryRaw = d.inventory ?? d.Inventory ?? d.data ?? d.Data ?? []
  const inventory = Array.isArray(inventoryRaw) ? (inventoryRaw as any[]).map(normalizeShowroomCar) : []
  const showroomInfoRaw = d.showroomInfo ?? d.showroomInfoRaw ?? d.showroom ?? d.ShowroomInfo ?? null
  const showroomInfo = showroomInfoRaw ? normalizeShowroom(showroomInfoRaw) : undefined
  return {
    message: d.message ?? d.Message,
    showroomInfo,
    inventory,
  }
}

