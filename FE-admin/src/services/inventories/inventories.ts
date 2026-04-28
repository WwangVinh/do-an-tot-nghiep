import { http } from '../http/http'

export type AdminCarInventoryDetail = {
  showroomId: number
  showroomName: string
  province: string
  quantity: number
  displayStatus: string
}

export type AdminCarInventoriesByCarResponse = {
  carId: number
  totalQuantity: number
  details: AdminCarInventoryDetail[]
}

export async function fetchAdminInventoriesByCarId(carId: number): Promise<AdminCarInventoriesByCarResponse> {
  const res = await http.get(`/api/admin/CarInventories/car/${carId}`)
  const d = res.data ?? {}

  const detailsRaw = d.Details ?? d.details
  const details = Array.isArray(detailsRaw) ? (detailsRaw as any[]) : []

  return {
    carId: Number(d.CarId ?? d.carId ?? carId),
    totalQuantity: Number(d.TotalQuantity ?? d.totalQuantity ?? 0),
    details: details.map((x) => ({
      showroomId: Number(x.ShowroomId ?? x.showroomId ?? 0),
      showroomName: String(x.ShowroomName ?? x.showroomName ?? ''),
      province: String(x.Province ?? x.province ?? ''),
      quantity: Number(x.Quantity ?? x.quantity ?? 0),
      displayStatus: String(x.DisplayStatus ?? x.displayStatus ?? ''),
    })),
  }
}

