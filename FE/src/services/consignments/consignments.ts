import { http } from '../http/http'

export type ConsignmentCreatePayload = {
  guestName: string
  guestPhone: string
  guestEmail?: string | null
  brand: string
  model: string
  year: number
  mileage: number
  conditionDescription?: string | null
  expectedPrice: number
}

export type ConsignmentCreateResponse = {
  success: boolean
  message: string
}

export async function createConsignment(
  payload: ConsignmentCreatePayload,
): Promise<ConsignmentCreateResponse> {
  const res = await http.post<ConsignmentCreateResponse>('/api/Consignments', payload)
  return res.data
}