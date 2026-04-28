import { useParams } from 'react-router-dom'

import { CarsNewPage } from './CarsNewPage'

export function CarsEditPage() {
  const { id } = useParams()
  const carId = Number(id)
  return <CarsNewPage mode="edit" carId={carId} />
}

