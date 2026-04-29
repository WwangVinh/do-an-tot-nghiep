import { useEffect, useMemo, useState } from 'react'

import { useMutation, useQuery } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { useNavigate } from 'react-router-dom'

import { http } from '../../services/http/http'
import type { CarCondition, CarStatus } from '../../services/cars/cars'
import { env } from '../../lib/env'

type SpecRow = { category: string; specName: string; specValue: string }
type PricingRow = { versionName: string; priceVnd: string; sortOrder: string; isActive: boolean }
type InventoryRow = { showroomId: string; quantity: string; displayStatus: string }
type GalleryMetaRow = { title: string; description: string; imageType: string; isMainImage: boolean }

type GalleryType = 'Color' | 'Overview' | 'Exterior' | 'Interior' | 'Safety' | 'Performance' | 'Other'
type GalleryGroupState = { files: File[]; metas: GalleryMetaRow[] }
type GalleryErrorsByType = Partial<Record<GalleryType, Record<number, string>>>

type ExistingGalleryImage = {
  carImageId: number
  title: string
  description: string
  imageUrl: string
  imageType: GalleryType
}

function toAbsoluteImageUrl(input: string): string {
  const s = String(input ?? '').trim()
  if (!s) return ''
  if (s.startsWith('http://') || s.startsWith('https://')) return s
  if (s.startsWith('/')) return `${env.VITE_API_BASE_URL}${s}`
  return `${env.VITE_API_BASE_URL}/${s.replace(/^\/+/, '')}`
}

type AdminShowroom = {
  showroomId: number
  name: string
  fullAddress?: string | null
  province?: string | null
  district?: string | null
  streetAddress?: string | null
}

const GALLERY_GROUPS: Array<{
  type: GalleryType
  label: string
  hint?: string
  requireTitle?: boolean
  defaultTitlePlaceholder?: string
}> = [
  {
    type: 'Color',
    label: 'Màu xe',
    hint: 'ImageType = Color. Mỗi ảnh tương ứng 1 màu; Title = tên màu.',
    requireTitle: true,
    defaultTitlePlaceholder: 'Tên màu (VD: Đỏ đô)',
  },
  { type: 'Overview', label: 'Tổng quan', hint: 'ImageType = Overview.' },
  { type: 'Exterior', label: 'Ngoại thất', hint: 'ImageType = Exterior.' },
  { type: 'Interior', label: 'Nội thất', hint: 'ImageType = Interior.' },
  { type: 'Safety', label: 'An toàn', hint: 'ImageType = Safety.' },
  { type: 'Performance', label: 'Vận hành', hint: 'ImageType = Performance.' },
  { type: 'Other', label: 'Khác', hint: 'ImageType = Other.' },
]

function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const anyErr = err as { message?: unknown; response?: { data?: unknown } }
    const data = anyErr.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (typeof anyErr.message === 'string' && anyErr.message.trim()) return anyErr.message
  }
  return 'Tạo xe thất bại'
}

export function CarsNewPage({ mode = 'create', carId }: { mode?: 'create' | 'edit'; carId?: number }) {
  const navigate = useNavigate()

  const [name, setName] = useState('')
  const [brand, setBrand] = useState('')
  const [year, setYear] = useState(new Date().getFullYear().toString())
  const [model, setModel] = useState('')
  const [color, setColor] = useState('')
  const [fuelType, setFuelType] = useState<'Xăng' | 'Điện' | 'Dầu' | 'Hybrid' | ''>('')
  const [mileage, setMileage] = useState('0')
  const [description, setDescription] = useState('')
  const [transmission, setTransmission] = useState<'Số sàn' | 'Số tự động' | ''>('')
  const [bodyStyle, setBodyStyle] = useState<
    'Sedan' | 'SUV' | 'Hatchback' | 'Crossover' | 'MPV' | 'Bán tải' | 'Coupe' | ''
  >('')

  const [condition, setCondition] = useState<CarCondition>('New')
  const [status, setStatus] = useState<CarStatus>('Available')

  const [showroomId, setShowroomId] = useState('1')
  const [quantity, setQuantity] = useState('1')
  const [displayStatus, setDisplayStatus] = useState<'OnDisplay' | 'Pending' | 'Out of stock'>('OnDisplay')

  const [price, setPrice] = useState('')

  // Tính năng
  const [featureIds, setFeatureIds] = useState<string[]>([]) 
  const [token, setToken] = useState<string>(() => localStorage.getItem('admin_token') ?? '')
  const [isFeatureOpen, setIsFeatureOpen] = useState(false)
  const [isCreatingFeature, setIsCreatingFeature] = useState(false)
  const [newFeatureName, setNewFeatureName] = useState('')
  const [newFeatureIcon, setNewFeatureIcon] = useState<File | null>(null)
  const [isSubmittingFeature, setIsSubmittingFeature] = useState(false)
  const [availableFeatures, setAvailableFeatures] = useState<{id: string, name: string}[]>([])

  const [mainImage, setMainImage] = useState<File | null>(null)
  const [existingMainImageUrl, setExistingMainImageUrl] = useState<string>('')
  const [existingGallery, setExistingGallery] = useState<Record<GalleryType, ExistingGalleryImage[]>>(() => ({
    Color: [], Overview: [], Exterior: [], Interior: [], Safety: [], Performance: [], Other: [],
  }))
  const [gallery, setGallery] = useState<Record<GalleryType, GalleryGroupState>>(() => ({
    Color: { files: [], metas: [] }, Overview: { files: [], metas: [] }, Exterior: { files: [], metas: [] },
    Interior: { files: [], metas: [] }, Safety: { files: [], metas: [] }, Performance: { files: [], metas: [] }, Other: { files: [], metas: [] },
  }))
  const [specs, setSpecs] = useState<SpecRow[]>([{ category: '', specName: '', specValue: '' }])
  const [pricing, setPricing] = useState<PricingRow[]>([])
  const [inventories, setInventories] = useState<InventoryRow[]>([])
  const [galleryErrors, setGalleryErrors] = useState<GalleryErrorsByType>({})

  // 1. Tự động nhập Giá: Lấy bản rẻ nhất
  useEffect(() => {
    if (pricing.length > 0) {
      const validPrices = pricing.map((p) => Number(p.priceVnd)).filter((p) => !isNaN(p) && p > 0)
      if (validPrices.length > 0) setPrice(Math.min(...validPrices).toString())
    }
  }, [pricing])

  // 2. Tự động nhập Màu: Ghép tên màu từ ảnh
  useEffect(() => {
    const colorGroup = gallery['Color']
    if (colorGroup && colorGroup.files.length > 0) {
      const colorNames = colorGroup.metas.map((m) => m.title?.trim()).filter(Boolean)
      setColor(Array.from(new Set(colorNames)).join(', '))
    }
  }, [gallery])

  const [featureSearch, setFeatureSearch] = useState('')
  // Tải danh sách Features
  useEffect(() => {
    async function loadFeatures() {
      try {
        const res = await http.get('/api/admin/features') 
        const rawData = res.data?.data || res.data || []
        const mapped = rawData.map((item: any) => ({
          id: String(item.id || item.Id || item.featureId),
          name: item.name || item.featureName || item.FeatureName || item.title 
        }))
        setAvailableFeatures(mapped)
      } catch (err) {
        console.error('Không tải được danh sách tính năng từ BE:', err)
      }
    }
    loadFeatures()
  }, [])

  const ui = useMemo(() => {
    const card = 'rounded-xl border border-slate-200/70 bg-white shadow-sm shadow-slate-900/5 ring-1 ring-slate-900/5 dark:border-zinc-800/80 dark:bg-zinc-950 dark:shadow-none dark:ring-0'
    const cardBody = 'p-4 sm:p-5'
    const cardHeader = 'flex items-start justify-between gap-3 border-b border-slate-200/70 bg-gradient-to-r from-indigo-50/60 to-transparent px-4 py-3 sm:px-5 dark:border-zinc-800/80 dark:from-indigo-950/20'
    const cardTitle = 'text-sm font-semibold text-slate-900 dark:text-zinc-100'
    const cardSub = 'mt-0.5 text-xs text-slate-500 dark:text-zinc-400'
    const label = 'text-xs font-medium text-slate-700 dark:text-zinc-200'
    const control = 'mt-2 w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 shadow-sm shadow-slate-900/5 outline-none placeholder:text-slate-400 focus:border-indigo-200 focus:ring-4 focus:ring-indigo-500/10 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:shadow-none dark:placeholder:text-zinc-500 dark:focus:border-indigo-900/40 dark:focus:ring-indigo-900/30'
    const controlMono = control + ' font-mono'
    const controlError = 'mt-2 w-full rounded-lg border border-red-400 bg-white px-3 py-2 text-sm text-slate-900 shadow-sm shadow-slate-900/5 outline-none placeholder:text-slate-400 focus:border-red-400 focus:ring-4 focus:ring-red-500/10 dark:border-red-900/60 dark:bg-zinc-950 dark:text-zinc-100 dark:shadow-none dark:placeholder:text-zinc-500 dark:focus:border-red-900/60 dark:focus:ring-red-900/30'
    const textarea = control + ' min-h-24 resize-y'
    const textareaError = controlError + ' min-h-20 resize-y'
    const btn = 'inline-flex items-center justify-center gap-2 rounded-lg border px-3 py-2 text-sm font-medium shadow-sm shadow-slate-900/5 outline-none transition hover:shadow-md focus:ring-4 focus:ring-slate-900/10 disabled:cursor-not-allowed disabled:opacity-60 dark:shadow-none dark:hover:shadow-none dark:focus:ring-zinc-700/25'
    const btnPrimary = 'inline-flex items-center justify-center gap-2 rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white shadow-sm shadow-slate-900/10 outline-none ring-1 ring-indigo-200/60 transition hover:bg-slate-800 focus:ring-4 focus:ring-indigo-500/15 disabled:cursor-not-allowed disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:ring-indigo-900/30 dark:hover:bg-zinc-100 dark:focus:ring-indigo-900/30'
    const btnGhost = 'inline-flex items-center justify-center gap-2 rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm shadow-slate-900/5 outline-none transition hover:bg-slate-50 focus:ring-4 focus:ring-slate-900/10 disabled:cursor-not-allowed disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900/60 dark:shadow-none dark:focus:ring-zinc-700/25'
    const btnDanger = 'inline-flex items-center justify-center rounded-lg border border-red-200 bg-white px-2 py-2 text-sm font-medium text-red-700 shadow-sm shadow-slate-900/5 outline-none transition hover:bg-red-50 focus:ring-4 focus:ring-red-500/10 dark:border-red-900/40 dark:bg-zinc-950 dark:text-red-300 dark:shadow-none dark:hover:bg-red-950/30 dark:focus:ring-red-900/30'
    const badge = 'inline-flex items-center rounded-md border border-indigo-200/70 bg-indigo-50/60 px-2 py-1 text-xs font-medium text-indigo-800 dark:border-indigo-900/40 dark:bg-indigo-950/20 dark:text-indigo-200'
    return { card, cardBody, cardHeader, cardTitle, cardSub, label, control, controlMono, controlError, textarea, textareaError, btn, btnPrimary, btnGhost, btnDanger, badge }
  }, [])

  const showroomsQ = useQuery({
    queryKey: ['admin-showrooms'],
    queryFn: async (): Promise<AdminShowroom[]> => {
      const res = await http.get('/api/admin/showrooms')
      const raw = res.data
      const arr = Array.isArray(raw) ? raw : Array.isArray(raw?.data) ? raw.data : []
      return (arr as any[]).map((s) => ({
        showroomId: Number(s?.ShowroomId ?? s?.showroomId ?? 0),
        name: String(s?.Name ?? s?.name ?? ''),
      }))
    },
    staleTime: 60_000,
  })

  const detailQ = useQuery({
    queryKey: ['admin-car-detail', carId],
    enabled: mode === 'edit' && Number.isFinite(carId) && (carId ?? 0) > 0,
    queryFn: async () => {
      const res = await http.get(`/api/admin/cars/${carId}`)
      return res.data as any
    },
    staleTime: 30_000,
  })

  const showroomOptions = useMemo(() => {
    const items = (showroomsQ.data ?? []).filter((s) => s.showroomId > 0 && s.name.trim())
    items.sort((a, b) => a.name.localeCompare(b.name))
    return items
  }, [showroomsQ.data])

  useEffect(() => {
    if (!showroomOptions.length) return
    const current = Number(showroomId || 0)
    const exists = showroomOptions.some((s) => s.showroomId === current)
    if (!exists) setShowroomId(String(showroomOptions[0].showroomId))
  }, [showroomId, showroomOptions])

  useEffect(() => {
    if (mode !== 'edit') return
    const d = detailQ.data
    if (!d) return

    setName(String(d?.Name ?? d?.name ?? ''))
    setBrand(String(d?.Brand ?? d?.brand ?? ''))
    setYear(String(d?.Year ?? d?.year ?? new Date().getFullYear()))
    setModel(String(d?.Model ?? d?.model ?? ''))
    setColor(String(d?.Color ?? d?.color ?? ''))
    setPrice(String(d?.Price ?? d?.price ?? ''))
    setFuelType((d?.FuelType ?? d?.fuelType ?? '') as any)
    setMileage(String(d?.Mileage ?? d?.mileage ?? 0))
    setDescription(String(d?.Description ?? d?.description ?? ''))
    setTransmission((d?.Transmission ?? d?.transmission ?? '') as any)
    setBodyStyle((d?.BodyStyle ?? d?.bodyStyle ?? '') as any)
    setCondition(((d?.Condition ?? d?.condition) === 'Used' ? 'Used' : 'New') as any)
    setStatus(((d?.Status ?? d?.status) ?? 'Available') as any)

    const mainUrl = String(d?.ImageUrl ?? d?.imageUrl ?? '')
    setExistingMainImageUrl(toAbsoluteImageUrl(mainUrl))

    const showroomDetails = (d?.ShowroomDetails ?? d?.showroomDetails ?? []) as any[]
    if (Array.isArray(showroomDetails) && showroomDetails.length) {
      const mapped = showroomDetails.map((x) => ({
          showroomId: String(x?.ShowroomId ?? x?.showroomId ?? ''),
          quantity: String(x?.Quantity ?? x?.quantity ?? 0),
          displayStatus: String(x?.DisplayStatus ?? x?.displayStatus ?? 'OnDisplay'),
        })).filter((x) => Number(x.showroomId) > 0)
      setInventories(mapped)
      if (mapped[0]) {
        setShowroomId(mapped[0].showroomId)
        setQuantity(mapped[0].quantity)
      }
    }

    const features = (d?.Features ?? d?.features ?? []) as any[]
    if (Array.isArray(features) && features.length) {
      const ids = features.map((f) => Number(f?.FeatureId ?? f?.featureId ?? 0)).filter((n) => Number.isFinite(n) && n > 0)
      if (ids.length) setFeatureIds(ids.map(String))
    }

    const specsGroups = (d?.Specifications ?? d?.specifications ?? []) as any[]
    if (Array.isArray(specsGroups) && specsGroups.length) {
      const rows: SpecRow[] = []
      for (const g of specsGroups) {
        const cat = String(g?.Category ?? g?.category ?? '')
        const items = (g?.Items ?? g?.items ?? []) as any[]
        for (const it of items) {
          const specName = String(it?.SpecName ?? it?.specName ?? it?.Name ?? it?.name ?? '')
          const specValue = String(it?.SpecValue ?? it?.specValue ?? it?.Value ?? it?.value ?? '')
          if (cat && specName && specValue) rows.push({ category: cat, specName, specValue })
        }
      }
      if (rows.length) setSpecs(rows)
    }

    const pvs = (d?.PricingVersions ?? d?.pricingVersions ?? []) as any[]
    if (Array.isArray(pvs) && pvs.length) {
      setPricing(pvs.map((p) => ({
          versionName: String(p?.VersionName ?? p?.versionName ?? ''),
          priceVnd: String(p?.PriceVnd ?? p?.priceVnd ?? 0),
          sortOrder: String(p?.SortOrder ?? p?.sortOrder ?? ''),
          isActive: Boolean(p?.IsActive ?? p?.isActive ?? true),
        })))
    }

    // Existing gallery images
    const galleryGroups = (d?.GalleryImages ?? d?.galleryImages ?? []) as any[]
    if (Array.isArray(galleryGroups)) {
      const next: Record<GalleryType, ExistingGalleryImage[]> = { Color: [], Overview: [], Exterior: [], Interior: [], Safety: [], Performance: [], Other: [] }
      const allowedTypes = new Set<GalleryType>(['Color', 'Overview', 'Exterior', 'Interior', 'Safety', 'Performance', 'Other'])
      for (const g of galleryGroups) {
        const rawType = String(g?.Category ?? g?.category ?? '')
        const imageType = (allowedTypes.has(rawType as GalleryType) ? (rawType as GalleryType) : 'Other') as GalleryType
        const images = (g?.Images ?? g?.images ?? []) as any[]
        if (!Array.isArray(images)) continue

        for (const im of images) {
          const idNum = Number(im?.CarImageId ?? im?.carImageId ?? 0)
          const imageUrl = String(im?.ImageUrl ?? im?.imageUrl ?? '')
          if (!idNum || !imageUrl) continue
          next[imageType].push({
            carImageId: idNum, title: String(im?.Title ?? im?.title ?? ''),
            description: String(im?.Description ?? im?.description ?? ''),
            imageUrl: toAbsoluteImageUrl(imageUrl), imageType,
          })
        }
      }
      setExistingGallery(next) 
    }
  }, [detailQ.data, mode])

  // Phục hồi API Update/Delete cho ảnh cũ
  const updateExistingImageM = useMutation({
    mutationFn: async (payload: { imageId: number; title: string; description: string }) => {
      const fd = new FormData()
      fd.append('Title', payload.title ?? ''); fd.append('Description', payload.description ?? '')
      const res = await http.put(`/api/admin/cars/images/${payload.imageId}/details`, fd)
      return res.data as any
    },
    onSuccess: () => toast.success('Đã cập nhật thông tin ảnh'),
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const deleteExistingImageM = useMutation({
    mutationFn: async (imageId: number) => {
      const res = await http.delete(`/api/admin/cars/delete-image/${imageId}`)
      return res.data as any
    },
    onSuccess: () => toast.success('Đã xoá ảnh'),
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const flattenedGallery = useMemo(() => {
    const files: File[] = []; const metas: GalleryMetaRow[] = []
    for (const g of GALLERY_GROUPS) {
      const st = gallery[g.type]
      if (!st || !st.files) continue
      for (let i = 0; i < st.files.length; i++) {
        if (!st.files[i]) continue
        files.push(st.files[i])
        const m = st.metas[i] ?? { title: '', description: '', imageType: g.type, isMainImage: false }
        metas.push({ ...m, imageType: g.type })
      }
    }
    return { files, metas }
  }, [gallery])

  const galleryMetasJson = useMemo(() => {
    const payload = flattenedGallery.metas.map((m) => {
      let finalDescription = m.description?.trim() || null;
      if (m.imageType === 'Color' && !finalDescription) finalDescription = 'Màu xe';
      return { title: m.title?.trim() || null, description: finalDescription, imageType: m.imageType || null, isMainImage: m.isMainImage || false };
    })
    return JSON.stringify(payload)
  }, [flattenedGallery.metas])

  const specificationsJson = useMemo(() => {
    const payload = specs.filter((s) => s.category.trim() && s.specName.trim() && s.specValue.trim()).map((s) => ({ category: s.category.trim(), specName: s.specName.trim(), specValue: s.specValue.trim() }))
    return JSON.stringify(payload)
  }, [specs])

  const pricingVersionsJson = useMemo(() => {
    const payload = pricing.filter((p) => p.versionName.trim()).map((p, idx) => ({ versionName: p.versionName.trim(), priceVnd: Number(p.priceVnd || 0), sortOrder: Number(p.sortOrder || idx + 1), isActive: p.isActive }))
    return JSON.stringify(payload)
  }, [pricing])

  const inventoriesJson = useMemo(() => {
    const payload = inventories.filter((i) => Number(i.showroomId) > 0).map((i) => ({ showroomId: Number(i.showroomId), quantity: Number(i.quantity || 0), displayStatus: i.displayStatus || null }))
    return JSON.stringify(payload)
  }, [inventories])

  function setMetaFor(type: GalleryType, idx: number, patch: Partial<GalleryMetaRow>) {
    setGallery((prev) => {
      const g = prev[type]
      const nextMetas = g.metas.map((m, i) => (i === idx ? { ...m, ...patch, imageType: type } : m))
      return { ...prev, [type]: { ...g, metas: nextMetas } }
    })
  }

  function removeGalleryFile(type: GalleryType, idxToRemove: number) {
    setGallery((prev) => {
      const g = prev[type]
      const nextFiles = g.files.filter((_, i) => i !== idxToRemove)
      const nextMetas = g.metas.filter((_, i) => i !== idxToRemove)
      return { ...prev, [type]: { files: nextFiles, metas: nextMetas } }
    })
  }

  function setMainImageFor(type: GalleryType, idx: number, isMain: boolean) {
    setGallery((prev) => {
      const next: Record<GalleryType, GalleryGroupState> = { ...prev }
      for (const k of Object.keys(next) as GalleryType[]) {
        const g = next[k]
        next[k] = { ...g, metas: g.metas.map((m) => ({ ...m, isMainImage: false })) }
      }
      if (isMain) {
        const g = next[type]
        next[type] = { ...g, metas: g.metas.map((m, i) => (i === idx ? { ...m, isMainImage: true, imageType: type } : m)) }
      }
      return next
    })
  }

  // Khôi phục Hàm tạo tính năng nhanh ngay tại trang
  async function handleCreateFeature() {
    if (!newFeatureName.trim()) return
    setIsSubmittingFeature(true)
    try {
      const fd = new FormData()
      fd.append('FeatureName', newFeatureName)
      if (newFeatureIcon) fd.append('Icon', newFeatureIcon)

      const headers: Record<string, string> = {}
      if (token.trim()) headers.Authorization = token.trim().startsWith('Bearer ') ? token.trim() : `Bearer ${token.trim()}`

      await http.post('/api/admin/Features', fd, { headers })

      const listRes = await http.get('/api/admin/Features')
      const rawData = listRes.data?.data || listRes.data || []
      const mapped = rawData.map((item: any) => ({
          id: String(item.id || item.Id || item.featureId), 
          name: item.name || item.featureName || item.FeatureName || item.title 
        }))
      setAvailableFeatures(mapped)

      const created = mapped.find((x: any) => x.name.toLowerCase() === newFeatureName.trim().toLowerCase())
      if (created && !featureIds.includes(created.id)) {
        setFeatureIds((prev) => [...prev, created.id])
      }

      setNewFeatureName(''); setNewFeatureIcon(null); setIsCreatingFeature(false)
      toast.success('Đã tạo tính năng mới')
    } catch (err: any) {
      toast.error('Lỗi khi tạo tính năng: ' + getErrorMessage(err))
    } finally {
      setIsSubmittingFeature(false)
    }
  }


  // Hàm xóa tính năng
  async function handleDeleteFeature(id: string, e: React.MouseEvent) {
    e.stopPropagation(); // Ngăn việc click vào nút xóa lại làm trigger checkbox của label
    
    if (!window.confirm('Ní có chắc muốn xóa tính năng này không? Nó sẽ mất khỏi hệ thống luôn á!')) return;

    try {
      const headers: Record<string, string> = {}
      if (token.trim()) headers.Authorization = token.trim().startsWith('Bearer ') ? token.trim() : `Bearer ${token.trim()}`

      // Giả định endpoint là DELETE /api/admin/features/{id}
      await http.delete(`/api/admin/features/${id}`, { headers })

      // Cập nhật lại list hiển thị tại chỗ
      setAvailableFeatures((prev) => prev.filter((f) => f.id !== id))
      // Nếu tính năng đang được chọn thì bỏ chọn luôn
      setFeatureIds((prev) => prev.filter((fid) => fid !== id))
      
      toast.success('Đã tiễn tính năng lên đường!')
    } catch (err: any) {
      toast.error('Lỗi xóa tính năng: ' + getErrorMessage(err))
    }
  }
  
  const createM = useMutation({
    mutationFn: async () => {
      setGalleryErrors({})
      const fd = new FormData()

      fd.append('Name', name); fd.append('Brand', brand); fd.append('Year', year)
      fd.append('Model', model); fd.append('Color', color); fd.append('Description', description.trim())
      if (price.trim()) fd.append('Price', price)
      if (fuelType) fd.append('FuelType', fuelType)
      if (mileage.trim()) fd.append('Mileage', mileage)
      if (transmission) fd.append('Transmission', transmission)
      if (bodyStyle) fd.append('BodyStyle', bodyStyle)
      fd.append('Condition', condition); fd.append('Status', status)

      fd.append('ShowroomId', showroomId); fd.append('Quantity', quantity)
      fd.append('InventoriesJson', inventoriesJson); fd.append('DisplayStatus', displayStatus)

      if (featureIds.length > 0) fd.append('FeatureIds', featureIds.join(','))

      fd.append('SpecificationsJson', specificationsJson)
      fd.append('PricingVersionsJson', pricingVersionsJson)

      if (mainImage) fd.append('ImageFile', mainImage)

      for (const f of flattenedGallery.files) fd.append('GalleryFiles', f)
      fd.append('GalleryMetasJson', galleryMetasJson)

      const url = mode === 'edit' ? `/api/admin/cars/full/${carId}` : '/api/admin/cars/full'
      const method = mode === 'edit' ? 'put' : 'post'
      const res = await http.request({ url, method, data: fd, headers: { 'Content-Type': 'multipart/form-data' } })
      return res.data as { message?: string; data?: unknown }
    },
    onSuccess: (res) => {
      toast.success(res?.message ?? (mode === 'edit' ? 'Cập nhật xe OK' : 'Tạo xe OK'))
      navigate('/cars')
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <div className="w-full bg-slate-50/60 px-4 py-6 text-slate-900 dark:bg-zinc-950/10 dark:text-zinc-100">
      <div className="w-full max-w-none">
        <div className="sticky top-[64px] z-20 -mx-4 mb-6 border-b border-slate-200/70 bg-gradient-to-r from-indigo-50/70 via-slate-50/70 to-sky-50/40 px-4 py-4 backdrop-blur dark:border-zinc-800/80 dark:from-indigo-950/25 dark:via-zinc-950/40 dark:to-sky-950/10">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <h1 className="mt-1 text-xl font-semibold tracking-tight sm:text-2xl">
                {mode === 'edit' ? `Sửa xe #${carId}` : 'Thêm xe mới'}
              </h1>
              <div className="mt-1 text-sm text-slate-600 dark:text-zinc-300">
                {mode === 'edit' ? 'Dữ liệu được load sẵn từ backend. Chỉnh xong bấm Lưu.' : 'Nhập thông tin cơ bản, tồn kho, thông số, pricing versions và ảnh.'}
              </div>
            </div>

            <div className="flex flex-wrap items-center gap-2">
              <button type="button" disabled={createM.isPending} onClick={() => navigate('/cars')} className={ui.btnGhost}>Huỷ</button>
              <button type="button" disabled={createM.isPending} onClick={() => createM.mutate()} className={ui.btnPrimary}>
                {createM.isPending ? (mode === 'edit' ? 'Đang lưu...' : 'Đang tạo...') : mode === 'edit' ? 'Lưu thay đổi' : 'Tạo xe'}
              </button>
            </div>
          </div>
        </div>

        {mode === 'edit' && detailQ.isLoading ? <div className="mb-4 text-sm text-slate-600 dark:text-zinc-300">Đang tải...</div> : null}

        <div className="grid grid-cols-1 gap-6 pb-10">
          <section className={ui.card}>
            <div className={ui.cardHeader}>
              <div><h2 className={ui.cardTitle}>Thông tin chính</h2><div className={ui.cardSub}>Dữ liệu bảng Cars + ảnh chính + mô tả.</div></div>
              <span className={ui.badge}>Cars</span>
            </div>

            <div className={ui.cardBody}>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div><label className={ui.label}>Tên xe</label><input value={name} onChange={(e) => setName(e.target.value)} placeholder="VD: VinFast VF 7 Plus" className={ui.control} /></div>
                <div><label className={ui.label}>Hãng</label><input value={brand} onChange={(e) => setBrand(e.target.value)} placeholder="VD: VinFast" className={ui.control} /></div>
                <div><label className={ui.label}>Năm</label><input value={year} onChange={(e) => setYear(e.target.value)} inputMode="numeric" placeholder="VD: 2026" className={ui.control} /></div>
                <div><label className={ui.label}>Model</label><input value={model} onChange={(e) => setModel(e.target.value)} placeholder="VD: VF7" className={ui.control} /></div>
                <div><label className={ui.label}>Màu</label><input value={color} onChange={(e) => setColor(e.target.value)} placeholder="VD: Trắng" className={ui.control} /></div>
                <div><label className={ui.label}>ODO (km)</label><input value={mileage} onChange={(e) => setMileage(e.target.value)} inputMode="numeric" placeholder="VD: 0" className={ui.control} /></div>
                <div>
                  <label className={ui.label}>Nhiên liệu</label>
                  <select value={fuelType} onChange={(e) => setFuelType(e.target.value as any)} className={ui.control}>
                    <option value="">--</option><option value="Xăng">Xăng</option><option value="Điện">Điện</option><option value="Dầu">Dầu</option><option value="Hybrid">Hybrid</option>
                  </select>
                </div>
                <div>
                  <label className={ui.label}>Hộp số</label>
                  <select value={transmission} onChange={(e) => setTransmission(e.target.value as any)} className={ui.control}>
                    <option value="">--</option><option value="Số sàn">Số sàn</option><option value="Số tự động">Số tự động</option>
                  </select>
                </div>
                <div>
                  <label className={ui.label}>Kiểu dáng</label>
                  <select value={bodyStyle} onChange={(e) => setBodyStyle(e.target.value as any)} className={ui.control}>
                    <option value="">--</option><option value="Sedan">Sedan</option><option value="SUV">SUV</option><option value="Hatchback">Hatchback</option><option value="Crossover">Crossover</option><option value="MPV">MPV</option><option value="Bán tải">Bán tải</option><option value="Coupe">Coupe</option>
                  </select>
                </div>
                <div>
                  <label className={ui.label}>Giá (tuỳ chọn)</label>
                  <input value={price} onChange={(e) => setPrice(e.target.value)} placeholder="VD: 950000000" inputMode="numeric" className={ui.control} />
                </div>
                <div>
                  <label className={ui.label}>Tình trạng</label>
                  <select value={condition} onChange={(e) => setCondition(e.target.value as CarCondition)} className={ui.control}>
                    <option value="New">Xe mới</option><option value="Used">Xe cũ</option>
                  </select>
                </div>
                <div>
                  <label className={ui.label}>Trạng thái</label>
                  <select value={status} onChange={(e) => setStatus(e.target.value as CarStatus)} className={ui.control}>
                    <option value="Available">Available</option><option value="PendingApproval">PendingApproval</option><option value="Draft">Draft</option><option value="COMING_SOON">COMING_SOON</option><option value="Out_of_stock">Out_of_stock</option><option value="Rejected">Rejected</option>
                  </select>
                </div>
              </div>

              <div className="mt-4">
                <label className={ui.label}>Mô tả</label>
                <textarea value={description} onChange={(e) => setDescription(e.target.value)} className={ui.textarea} />
              </div>

              <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label className={ui.label}>Ảnh chính</label>
                  <input type="file" accept="image/*" onChange={(e) => setMainImage(e.target.files?.[0] ?? null)} className={ui.control} />
                  {mode === 'edit' && !mainImage && existingMainImageUrl ? (
                    <div className="mt-2">
                      <div className="text-xs text-slate-500 dark:text-zinc-400">Ảnh hiện tại:</div>
                      <img src={existingMainImageUrl} alt="Current main" className="mt-2 h-28 w-auto max-w-full rounded-lg border border-slate-200/70 object-cover dark:border-zinc-800/80" />
                    </div>
                  ) : null}
                  {mainImage && <div className="mt-2 text-xs text-slate-500 dark:text-zinc-400">Đã chọn: {mainImage.name}</div>}
                </div>
                
                {/* Đã gộp Tính năng lên chung row, XÓA duplicate Ảnh chính */}
                <div>
                  <div className="flex items-center justify-between">
                    <label className={ui.label}>Tính năng (FeatureIds)</label>
                    {!isCreatingFeature && (
                      <button type="button" onClick={() => setIsCreatingFeature(true)} className="text-xs font-semibold text-indigo-600 hover:text-indigo-800 hover:underline dark:text-indigo-400">
                        + Tạo tính năng mới
                      </button>
                    )}
                  </div>

                  {isCreatingFeature ? (
                    <div className="mt-2 rounded-lg border border-indigo-200 bg-indigo-50/50 p-3 shadow-sm dark:border-indigo-900/40 dark:bg-indigo-950/20">
                      <div className="text-xs font-bold text-indigo-800 dark:text-indigo-300 mb-2">Tạo Tính Năng Nhanh</div>
                      <div className="space-y-2">
                        <div>
                          <input value={newFeatureName} onChange={(e) => setNewFeatureName(e.target.value)} placeholder="Nhập tên tính năng (VD: Cửa hít, HUD...)" className={ui.control} autoFocus />
                        </div>
                        <div>
                          <label className="text-[10px] text-slate-600 dark:text-zinc-400 font-medium">Icon (tuỳ chọn):</label>
                          <input type="file" accept="image/*" onChange={(e) => setNewFeatureIcon(e.target.files?.[0] || null)} className="mt-1 w-full text-[10px] text-slate-700 file:mr-2 file:rounded file:border-0 file:bg-indigo-100 file:px-2 file:py-1 file:text-indigo-700 file:font-semibold dark:file:bg-indigo-900/50 dark:file:text-indigo-300" />
                        </div>
                        <div className="flex items-center gap-2 pt-1">
                          <button type="button" onClick={handleCreateFeature} disabled={isSubmittingFeature || !newFeatureName.trim()} className={ui.btnPrimary + " py-1.5 px-3 text-xs"}>
                            {isSubmittingFeature ? 'Đang lưu...' : 'Lưu & Chọn'}
                          </button>
                          <button type="button" onClick={() => { setIsCreatingFeature(false); setNewFeatureName(''); setNewFeatureIcon(null) }} className={ui.btnGhost + " py-1.5 px-3 text-xs"}>
                            Huỷ
                          </button>
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className="relative mt-2">
                      <div className={ui.control + " cursor-pointer flex justify-between items-center select-none"} onClick={() => setIsFeatureOpen(!isFeatureOpen)}>
                        <span className="truncate pr-4 text-slate-600 dark:text-zinc-400">
                          {featureIds.length > 0 ? featureIds.map(id => availableFeatures.find(f => f.id === id)?.name || id).join(', ') : 'Bấm để xổ ra chọn tính năng...'}
                        </span>
                        <span className="text-xs text-slate-400">{isFeatureOpen ? '▲' : '▼'}</span>
                      </div>

                      {isFeatureOpen && (
                        <div className="absolute z-10 mt-1 w-full rounded-lg border border-slate-200 bg-white shadow-lg dark:border-zinc-800 dark:bg-zinc-950">
                          {/* Ô search */}
                          <div className="p-2 border-b border-slate-100 dark:border-zinc-800">
                            <div className="flex items-center gap-2 rounded-md border border-slate-200 bg-slate-50 px-3 py-1.5 dark:border-zinc-700 dark:bg-zinc-800">
                              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/></svg>
                              <input
                                autoFocus
                                value={featureSearch}
                                onChange={(e) => setFeatureSearch(e.target.value)}
                                placeholder="Tìm tính năng..."
                                className="flex-1 bg-transparent text-sm outline-none placeholder:text-slate-400 dark:text-zinc-100 dark:placeholder:text-zinc-500"
                                onClick={(e) => e.stopPropagation()}
                              />
                              {featureSearch && (
                                <button type="button" onClick={() => setFeatureSearch('')}>
                                  <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="text-slate-400 hover:text-slate-600"><path d="M18 6 6 18M6 6l12 12"/></svg>
                                </button>
                              )}
                            </div>
                          </div>

                          {/* Danh sách */}
                          <div className="max-h-48 overflow-auto py-1">
                            {availableFeatures
                              .filter((f) => f.name.toLowerCase().includes(featureSearch.toLowerCase()))
                              .map((f) => (
                                <div key={f.id} className="flex items-center justify-between px-3 py-2 border-b border-slate-100 dark:border-zinc-800 hover:bg-slate-50 dark:hover:bg-zinc-900">
                                  <label className="flex flex-1 cursor-pointer items-center gap-3 text-sm text-slate-800 dark:text-zinc-200">
                                    <input
                                      type="checkbox"
                                      checked={featureIds.includes(f.id)}
                                      onChange={(e) => {
                                        if (e.target.checked) setFeatureIds((prev) => [...prev, f.id])
                                        else setFeatureIds((prev) => prev.filter((id) => id !== f.id))
                                      }}
                                      className="h-4 w-4 rounded border-slate-300 text-indigo-600 focus:ring-indigo-500 dark:border-zinc-700 dark:bg-zinc-900"
                                    />
                                    <span className="truncate">{f.name}</span>
                                  </label>
                                  <button
                                    type="button"
                                    onClick={(e) => handleDeleteFeature(f.id, e)}
                                    className="ml-2 flex items-center justify-center rounded-md p-1.5 text-red-500 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-900/30"
                                    title="Xóa tính năng"
                                  >
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                                      <path d="M3 6h18m-2 0v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6m3 0V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/>
                                    </svg>
                                  </button>
                                </div>
                              ))}
                            {availableFeatures.filter((f) => f.name.toLowerCase().includes(featureSearch.toLowerCase())).length === 0 && (
                              <div className="px-3 py-2 text-sm text-slate-400 italic">Không tìm thấy tính năng.</div>
                            )}
                          </div>

                          <div className="border-t border-slate-100 px-3 py-2 dark:border-zinc-800">
                            <button type="button" onClick={() => { setIsFeatureOpen(false); setFeatureSearch('') }}
                              className="w-full rounded-md bg-slate-900 py-1.5 text-xs font-medium text-white hover:bg-slate-700 dark:bg-white dark:text-zinc-900">
                              Xong
                            </button>
                          </div>
                        </div>
                      )}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </section>

          <section className={ui.card}>
            <div className={ui.cardHeader}>
              <div><h2 className={ui.cardTitle}>Tồn kho</h2><div className={ui.cardSub}>Thiết lập kho mặc định hoặc thêm nhiều showroom.</div></div>
              <span className={ui.badge}>CarInventories</span>
            </div>

            <div className={ui.cardBody}>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
                <div>
                  <label className={ui.label}>Showroom mặc định</label>
                  <select value={showroomId} onChange={(e) => setShowroomId(e.target.value)} className={ui.control}>
                    {showroomsQ.isLoading ? <option value={showroomId}>Đang tải...</option> : null}
                    {!showroomsQ.isLoading && showroomOptions.length === 0 ? (<option value={showroomId}>Không có showroom</option>) : null}
                    {showroomOptions.map((s) => (<option key={s.showroomId} value={String(s.showroomId)}>{s.name}</option>))}
                  </select>
                </div>
                <div><label className={ui.label}>Số lượng mặc định</label><input value={quantity} onChange={(e) => setQuantity(e.target.value)} inputMode="numeric" className={ui.control} /></div>
                <div>
                  <label className={ui.label}>DisplayStatus mặc định</label>
                  <select value={displayStatus} onChange={(e) => setDisplayStatus(e.target.value as any)} className={ui.control}>
                    <option value="OnDisplay">OnDisplay</option><option value="Pending">Pending</option><option value="Out of stock">Out of stock</option>
                  </select>
                </div>
              </div>

              <div className="mt-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <div className="text-sm font-semibold text-slate-900 dark:text-zinc-100">Danh sách kho (tuỳ chọn)</div>
                  <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">Nếu để trống, backend có thể dùng kho mặc định.</div>
                </div>
                <button type="button" onClick={() => setInventories((p) => [...p, { showroomId, quantity, displayStatus }])} className={ui.btn}>+ Thêm kho</button>
              </div>

              {inventories.length > 0 ? (
                <div className="mt-4 space-y-2">
                  {inventories.map((row, idx) => (
                    <div key={idx} className="grid grid-cols-1 gap-2 rounded-lg border border-slate-200/70 bg-white p-3 md:grid-cols-12 dark:border-zinc-800/80 dark:bg-zinc-950">
                      <select value={row.showroomId} onChange={(e) => setInventories((p) => p.map((x, i) => (i === idx ? { ...x, showroomId: e.target.value } : x)))} className={'md:col-span-4 ' + ui.control}>
                        {showroomsQ.isLoading ? <option value={row.showroomId}>Đang tải...</option> : <option value="">-- Chọn showroom --</option>}
                        {showroomOptions.map((s) => (<option key={s.showroomId} value={String(s.showroomId)}>{s.name}</option>))}
                      </select>
                      <input value={row.quantity} onChange={(e) => setInventories((p) => p.map((x, i) => (i === idx ? { ...x, quantity: e.target.value } : x)))} placeholder="Quantity" inputMode="numeric" className={'md:col-span-3 ' + ui.control} />
                      <input value={row.displayStatus} onChange={(e) => setInventories((p) => p.map((x, i) => (i === idx ? { ...x, displayStatus: e.target.value } : x)))} placeholder="DisplayStatus" className={'md:col-span-4 ' + ui.control} />
                      <button type="button" onClick={() => setInventories((p) => p.filter((_, i) => i !== idx))} className={'md:col-span-1 ' + ui.btnGhost}>X</button>
                    </div>
                  ))}
                </div>
              ) : null}
            </div>
          </section>

          <section className={ui.card}>
            <div className={ui.cardHeader}>
              <div><h2 className={ui.cardTitle}>Thông số</h2><div className={ui.cardSub}>Nhập theo từng dòng (category / name / value).</div></div>
              <span className={ui.badge}>CarSpecifications</span>
            </div>

            <div className={ui.cardBody}>
              <div className="space-y-2">
                {specs.map((s, idx) => {
                  const isSameCategoryAsPrev = idx > 0 && s.category === specs[idx - 1].category && s.category.trim() !== '';
                  return (
                    <div key={idx} className="flex flex-col">
                      <div className="grid grid-cols-1 gap-2 md:grid-cols-12">
                        <div className="md:col-span-3">
                          {!isSameCategoryAsPrev ? (<input value={s.category} onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, category: e.target.value } : x)))} placeholder="Category (VD: Động cơ)" className={'w-full ' + ui.control} />) : null}
                        </div>
                        <input value={s.specName} onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, specName: e.target.value } : x)))} placeholder="Tên thông số" className={'md:col-span-4 ' + ui.control} />
                        <input value={s.specValue} onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, specValue: e.target.value } : x)))} placeholder="Giá trị" className={'md:col-span-4 ' + ui.control} />
                        <button type="button" onClick={() => setSpecs((p) => p.filter((_, i) => i !== idx))} className={'md:col-span-1 ' + ui.btnGhost}>X</button>
                      </div>

                      {(idx === specs.length - 1 || specs[idx + 1].category !== s.category) && s.category.trim() !== '' && (
                        <div className="grid grid-cols-1 md:grid-cols-12 mt-2 mb-4">
                          <div className="md:col-span-3"></div>
                          <div className="md:col-span-9 pl-1">
                            <button type="button" onClick={() => { setSpecs((prev) => { const next = [...prev]; next.splice(idx + 1, 0, { category: s.category, specName: '', specValue: '' }); return next }) }} className="inline-flex items-center rounded-full bg-indigo-50 px-3 py-1.5 text-xs font-medium text-indigo-700 transition-colors hover:bg-indigo-100 dark:bg-indigo-900/30 dark:text-indigo-300 dark:hover:bg-indigo-900/50">
                              + Thêm dòng cùng nhóm "{s.category}"
                            </button>
                          </div>
                        </div>
                      )}
                    </div>
                  )
                })}
              </div>
              <button type="button" onClick={() => setSpecs((p) => [...p, { category: '', specName: '', specValue: '' }])} className={'mt-4 ' + ui.btn}>+ Thêm thông số</button>
            </div>
          </section>

          <section className={ui.card}>
            <div className={ui.cardHeader}>
              <div><h2 className={ui.cardTitle}>Phiên bản / Giá</h2><div className={ui.cardSub}>Backend sẽ lấy giá thấp nhất để set vào Cars (nếu có).</div></div>
              <span className={ui.badge}>CarPricingVersions</span>
            </div>

            <div className={ui.cardBody}>
              <div className="space-y-2">
                {pricing.map((p, idx) => (
                  <div key={idx} className="grid grid-cols-1 gap-2 rounded-lg border border-slate-200/70 bg-white p-3 md:grid-cols-12 dark:border-zinc-800/80 dark:bg-zinc-950">
                    <input value={p.versionName} onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, versionName: e.target.value } : x)))} placeholder="VersionName" className={'md:col-span-4 ' + ui.control} />
                    <input value={p.priceVnd} onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, priceVnd: e.target.value } : x)))} placeholder="PriceVnd" inputMode="numeric" className={'md:col-span-3 ' + ui.control} />
                    <input value={p.sortOrder} onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, sortOrder: e.target.value } : x)))} placeholder="SortOrder" inputMode="numeric" className={'md:col-span-2 ' + ui.control} />
                    <label className="md:col-span-2 flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-800 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-200 mt-2">
                      <input type="checkbox" checked={p.isActive} onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, isActive: e.target.checked } : x)))} /> Active
                    </label>
                    <button type="button" onClick={() => setPricing((prev) => prev.filter((_, i) => i !== idx))} className={'md:col-span-1 mt-2 ' + ui.btnGhost}>X</button>
                  </div>
                ))}
              </div>
              <button type="button" onClick={() => setPricing((p) => [...p, { versionName: '', priceVnd: '', sortOrder: '', isActive: true }])} className={'mt-4 ' + ui.btn}>+ Thêm phiên bản</button>
            </div>
          </section>

          <section className={ui.card}>
            <div className={ui.cardHeader}>
              <div><h2 className={ui.cardTitle}>Ảnh phụ</h2><div className={ui.cardSub}>Thêm chi tiết mô tả cho từng hình ảnh.</div></div>
              <span className={ui.badge}>CarImages</span>
            </div>

            <div className={ui.cardBody}>
              <div className="space-y-6">
                {GALLERY_GROUPS.map((group) => {
                  const st = gallery[group.type]
                  const errs = galleryErrors[group.type] ?? {}
                  const existed = existingGallery[group.type] ?? []
                  return (
                    <div key={group.type} className="rounded-xl border border-slate-200/70 bg-white p-4 shadow-sm shadow-slate-900/5 dark:border-zinc-800/80 dark:bg-zinc-950 dark:shadow-none">
                      <div className="flex flex-col gap-1 md:flex-row md:items-center md:justify-between">
                        <div className="text-sm font-semibold text-slate-900 dark:text-zinc-100">{group.label} <span className="text-xs font-normal text-slate-500">(ImageType: {group.type})</span></div>
                        {group.hint ? <div className="text-xs text-slate-500 dark:text-zinc-400">{group.hint}</div> : null}
                      </div>

                      {/* Hiển thị ảnh cũ để có thể Sửa/Xóa (chỉ khi Edit) */}
                      {mode === 'edit' && existed.length > 0 ? (
                        <div className="mt-4 space-y-3">
                          <div className="text-xs font-medium text-slate-700 dark:text-zinc-200">Ảnh đang có trên Server</div>
                          {existed.map((img, idx) => (
                            <div key={img.carImageId} className="rounded-lg border border-slate-200/70 bg-slate-50/70 p-3 dark:border-zinc-800/80 dark:bg-zinc-900/20">
                              <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                                <div className="flex min-w-0 flex-1 gap-3">
                                  <img src={img.imageUrl} alt={img.title} className="h-20 w-28 flex-none rounded-md border border-slate-200/70 object-cover dark:border-zinc-800/80" />
                                  <div className="min-w-0 flex-1">
                                    <div className="mt-1 grid grid-cols-1 gap-2 md:grid-cols-12">
                                      <input value={img.title} onChange={(e) => setExistingGallery((prev) => ({ ...prev, [group.type]: prev[group.type].map((x, i) => i === idx ? { ...x, title: e.target.value } : x) }))} placeholder={group.defaultTitlePlaceholder ?? 'Title'} className={'md:col-span-4 ' + ui.control} />
                                      <textarea value={img.description} onChange={(e) => setExistingGallery((prev) => ({ ...prev, [group.type]: prev[group.type].map((x, i) => i === idx ? { ...x, description: e.target.value } : x) }))} placeholder="Description" className={'md:col-span-8 ' + ui.textarea} />
                                    </div>
                                  </div>
                                </div>
                                <div className="flex flex-wrap items-center justify-end gap-2">
                                  <button type="button" disabled={updateExistingImageM.isPending} onClick={() => updateExistingImageM.mutate({ imageId: img.carImageId, title: img.title, description: img.description })} className={ui.btnGhost}>Lưu info</button>
                                  <button type="button" disabled={deleteExistingImageM.isPending} onClick={() => { deleteExistingImageM.mutate(img.carImageId, { onSuccess: () => { setExistingGallery((prev) => ({ ...prev, [group.type]: prev[group.type].filter((x) => x.carImageId !== img.carImageId) })) } }) }} className={ui.btnDanger}>Xoá</button>
                                </div>
                              </div>
                            </div>
                          ))}
                        </div>
                      ) : null}

                      {/* Khối Thêm Ảnh Mới */}
                      <div className="mt-4">
                        <button type="button" onClick={() => { setGallery((prev) => ({ ...prev, [group.type]: { files: [...prev[group.type].files, null as any], metas: [...prev[group.type].metas, { title: group.type === 'Other' ? 'Khác' : '', description: group.type === 'Other' ? 'Khác' : '', imageType: group.type, isMainImage: false }] } })) }} className={ui.btnGhost}>
                          + Thêm 1 ô ảnh mới
                        </button>
                      </div>

                      {st.files.length > 0 ? (
                        <div className="mt-4 space-y-3">
                          {st.files.map((f, idx) => (
                            <div key={idx} className="rounded-lg border border-slate-200/70 bg-slate-50/70 p-3 dark:border-zinc-800/80 dark:bg-zinc-900/20">
                              <div className="flex items-center justify-between mb-2 pb-2 border-b border-slate-200/70 dark:border-zinc-800/80">
                                <div className="text-sm font-medium text-indigo-700 dark:text-indigo-400 truncate pr-2">
                                  {f ? f.name : (<input type="file" accept="image/*" onChange={(e) => { const file = e.target.files?.[0]; if (file) { setGallery((prev) => { const nextFiles = [...prev[group.type].files]; nextFiles[idx] = file; return { ...prev, [group.type]: { ...prev[group.type], files: nextFiles } } }) } }} className="w-full text-xs text-slate-900 dark:text-zinc-200" />)}
                                </div>
                                <button type="button" onClick={() => removeGalleryFile(group.type, idx)} className={ui.btnDanger + " py-1 px-2 text-xs"}>Xoá ô này</button>
                              </div>

                              <div className="mt-2 grid grid-cols-1 gap-2 md:grid-cols-12">
                                {group.type === 'Other' ? (
                                  <div className="md:col-span-11 rounded-lg border border-slate-200/70 bg-slate-100/50 px-3 py-2 text-sm text-slate-500 italic dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-400">Tự động lưu Tiêu đề & Mô tả là "Khác"</div>
                                ) : (
                                  <>
                                    <input value={st.metas[idx]?.title ?? ''} onChange={(e) => setMetaFor(group.type, idx, { title: e.target.value })} placeholder={group.defaultTitlePlaceholder ?? 'Title (tuỳ chọn)'} className={(errs[idx] ? ui.controlError : ui.control) + (group.type === 'Color' ? ' md:col-span-7' : ' md:col-span-3')} />
                                    <div className={"rounded-lg border border-slate-200/70 bg-slate-100/50 px-3 py-2 text-sm text-slate-700 flex items-center dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-300" + (group.type === 'Color' ? ' md:col-span-4' : ' md:col-span-3')}>{group.type}</div>
                                    {group.type !== 'Color' && (<textarea value={st.metas[idx]?.description ?? ''} onChange={(e) => setMetaFor(group.type, idx, { description: e.target.value })} placeholder="Description (hỗ trợ xuống dòng)" className={(errs[idx] ? ui.textareaError : ui.textarea) + ' md:col-span-5'} />)}
                                  </>
                                )}
                                <label className="md:col-span-1 flex items-center justify-center rounded-lg border border-slate-200/70 px-2 py-2 text-sm text-slate-800 cursor-pointer hover:bg-slate-50 transition-colors dark:border-zinc-800 dark:text-zinc-200 dark:hover:bg-zinc-900 mt-2">
                                  <input type="checkbox" checked={st.metas[idx]?.isMainImage ?? false} onChange={(e) => setMainImageFor(group.type, idx, e.target.checked)} title="Là ảnh chính" className="mr-1" />
                                  <span className="text-xs md:hidden">Main</span>
                                </label>
                              </div>
                              {errs[idx] ? <div className="mt-2 text-xs text-red-600">{errs[idx]}</div> : null}
                            </div>
                          ))}
                        </div>
                      ) : (
                        <div className="mt-3 text-xs text-slate-500 dark:text-zinc-400">Chưa có ảnh mới nào được thêm.</div>
                      )}
                    </div>
                  )
                })}
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  )
}