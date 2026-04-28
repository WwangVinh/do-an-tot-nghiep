import { http } from '../http/http'

export type CarStatus =
  | 'Available'
  | 'PendingApproval'
  | 'Draft'
  | 'COMING_SOON'
  | 'Out_of_stock'
  | 'Rejected'

export type CarCondition = 'New' | 'Used'

export type AdminCarListItem = {
  carId: number
  name: string
  brand: string
  year: number
  price: number
  imageUrl?: string | null
  condition?: string | null
  status?: string | null
  isDeleted?: boolean | null
  totalQuantity?: number | null
  showrooms?: string | null
  bodyStyle?: string | null
  createdAt?: string | null
  updatedAt?: string | null
}

export type AdminCarsListResponse = {
  totalItems: number
  currentPage: number
  pageSize: number
  totalPages: number
  data: AdminCarListItem[]
}

export type AdminCarDetail = {
  carId: number
  name: string
  brand: string
  model?: string | null
  year: number
  price: number
  color?: string | null
  mileage?: number | null
  fuelType?: string | null
  transmission?: string | null
  bodyStyle?: string | null
  description?: string | null
  imageUrl?: string | null
  condition?: string | null
  status?: string | null
  isDeleted?: boolean | null
  totalQuantity?: number | null
  createdAt?: string | null
  updatedAt?: string | null
  // Other fields exist (inventories/specs/features/gallery/360) but not required for basic upsert UI.
}

export type CarUpsertInput = {
  name: string
  brand: string
  model?: string
  year: number
  price: number
  color?: string
  mileage?: number
  fuelType?: string
  transmission?: string
  bodyStyle?: string
  description?: string
  condition: CarCondition
  status?: CarStatus
  showroomId: number
  quantity: number
  featureIds?: string
  specifications?: string
  imageFile?: File | null
}

export type FetchAdminCarsParams = {
  search?: string
  brand?: string
  color?: string
  minPrice?: number
  maxPrice?: number
  status?: CarStatus | ''
  transmission?: string
  bodyStyle?: string
  fuelType?: string
  location?: string
  isDeleted?: boolean | ''
  page?: number
  pageSize?: number
}

function toFormData(input: CarUpsertInput): FormData {
  const fd = new FormData()
  fd.append('Name', input.name)
  fd.append('Brand', input.brand)
  if (input.model) fd.append('Model', input.model)
  fd.append('Year', String(input.year))
  fd.append('Price', String(input.price ?? 0))
  if (input.color) fd.append('Color', input.color)
  if (input.fuelType) fd.append('FuelType', input.fuelType)
  if (input.mileage !== undefined && input.mileage !== null) fd.append('Mileage', String(input.mileage))
  if (input.description) fd.append('Description', input.description)
  if (input.transmission) fd.append('Transmission', input.transmission)
  if (input.bodyStyle) fd.append('BodyStyle', input.bodyStyle)

  fd.append('Condition', input.condition)
  if (input.status) fd.append('Status', input.status)

  fd.append('ShowroomId', String(input.showroomId ?? 0))
  fd.append('Quantity', String(input.quantity ?? 0))

  if (input.featureIds) fd.append('FeatureIds', input.featureIds)
  if (input.specifications) fd.append('Specifications', input.specifications)
  if (input.imageFile) fd.append('ImageFile', input.imageFile)
  return fd
}

export async function fetchAdminCars(params: FetchAdminCarsParams): Promise<AdminCarsListResponse> {
  const res = await http.get('/api/admin/cars', { params })
  const d = res.data ?? {}
  // Backend có thể trả về PascalCase (C#) hoặc camelCase (JS). Hỗ trợ cả hai.
  const totalItems = d.TotalItems ?? d.totalItems
  const currentPage = d.CurrentPage ?? d.currentPage
  const pageSize = d.PageSize ?? d.pageSize
  const totalPages = d.TotalPages ?? d.totalPages
  const data = d.Data ?? d.data
  return {
    totalItems: Number(totalItems ?? 0),
    currentPage: Number(currentPage ?? 1),
    pageSize: Number(pageSize ?? 10),
    totalPages: Number(totalPages ?? 1),
    data: Array.isArray(data) ? (data as AdminCarListItem[]) : [],
  }
}

export async function fetchAdminCarDetail(carId: number): Promise<AdminCarDetail> {
  const res = await http.get(`/api/admin/cars/${carId}`)
  return res.data as AdminCarDetail
}

export async function createCar(input: CarUpsertInput): Promise<{ message?: string; data?: unknown }> {
  const res = await http.post('/api/admin/cars', toFormData(input), {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return res.data ?? {}
}

export async function updateCar(carId: number, input: CarUpsertInput): Promise<{ message?: string; data?: unknown }> {
  const res = await http.put(`/api/admin/cars/${carId}`, toFormData(input), {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return res.data ?? {}
}

export async function softDeleteCar(carId: number): Promise<{ message?: string }> {
  const res = await http.delete(`/api/admin/cars/${carId}/SoftDeleteCar`)
  return res.data ?? {}
}

export async function restoreCar(carId: number): Promise<{ message?: string }> {
  const res = await http.put(`/api/admin/cars/${carId}/restore`)
  return res.data ?? {}
}

