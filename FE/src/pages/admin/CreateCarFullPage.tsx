import { useMemo, useState } from 'react'

import { http } from '../../services/http/http'

type SpecRow = { category: string; specName: string; specValue: string }
type PricingRow = { versionName: string; priceVnd: string; sortOrder: string; isActive: boolean }
type InventoryRow = { showroomId: string; quantity: string; displayStatus: string }
type GalleryMetaRow = { title: string; description: string; imageType: string; isMainImage: boolean }

type CarCondition = 'New' | 'Used'
type CarStatus = 'Available' | 'PendingApproval' | 'Draft' | 'COMING_SOON' | 'Out_of_stock'

type GalleryType = 'Color' | 'Overview' | 'Exterior' | 'Interior' | 'Safety' | 'Performance' | 'Other'
type GalleryGroupState = { files: File[]; metas: GalleryMetaRow[] }
type GalleryErrorsByType = Partial<Record<GalleryType, Record<number, string>>>

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

export function CreateCarFullPage() {
  const [token, setToken] = useState(() => localStorage.getItem('admin_token') ?? '')

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

  const [price, setPrice] = useState('') // optional: nếu có pricing versions thì backend sẽ override Cars.Price = min

  const [featureIds, setFeatureIds] = useState('')

  const [mainImage, setMainImage] = useState<File | null>(null)
  const [gallery, setGallery] = useState<Record<GalleryType, GalleryGroupState>>(() => ({
    Color: { files: [], metas: [] },
    Overview: { files: [], metas: [] },
    Exterior: { files: [], metas: [] },
    Interior: { files: [], metas: [] },
    Safety: { files: [], metas: [] },
    Performance: { files: [], metas: [] },
    Other: { files: [], metas: [] },
  }))
  const [specs, setSpecs] = useState<SpecRow[]>([{ category: '', specName: '', specValue: '' }])
  const [pricing, setPricing] = useState<PricingRow[]>([])
  const [inventories, setInventories] = useState<InventoryRow[]>([])

  const [submitting, setSubmitting] = useState(false)
  const [result, setResult] = useState<{ ok: boolean; message: string; data?: unknown } | null>(null)
  const [galleryErrors, setGalleryErrors] = useState<GalleryErrorsByType>({})

  const flattenedGallery = useMemo(() => {
    const files: File[] = []
    const metas: GalleryMetaRow[] = []
    for (const g of GALLERY_GROUPS) {
      const st = gallery[g.type]
      for (let i = 0; i < st.files.length; i++) {
        files.push(st.files[i])
        const m = st.metas[i] ?? { title: '', description: '', imageType: g.type, isMainImage: false }
        metas.push({ ...m, imageType: g.type })
      }
    }
    return { files, metas }
  }, [gallery])

  const galleryMetasJson = useMemo(() => {
    const payload = flattenedGallery.metas.map((m) => ({
      title: m.title || null,
      description: m.description || null,
      imageType: m.imageType || null,
      isMainImage: m.isMainImage || false,
    }))
    return JSON.stringify(payload)
  }, [flattenedGallery.metas])

  const specificationsJson = useMemo(() => {
    const payload = specs
      .filter((s) => s.category.trim() && s.specName.trim() && s.specValue.trim())
      .map((s) => ({
        category: s.category.trim(),
        specName: s.specName.trim(),
        specValue: s.specValue.trim(),
      }))
    return JSON.stringify(payload)
  }, [specs])

  const pricingVersionsJson = useMemo(() => {
    const payload = pricing
      .filter((p) => p.versionName.trim())
      .map((p, idx) => ({
        versionName: p.versionName.trim(),
        priceVnd: Number(p.priceVnd || 0),
        sortOrder: Number(p.sortOrder || idx + 1),
        isActive: p.isActive,
      }))
    return JSON.stringify(payload)
  }, [pricing])

  const inventoriesJson = useMemo(() => {
    const payload = inventories
      .filter((i) => Number(i.showroomId) > 0)
      .map((i) => ({
        showroomId: Number(i.showroomId),
        quantity: Number(i.quantity || 0),
        displayStatus: i.displayStatus || null,
      }))
    return JSON.stringify(payload)
  }, [inventories])

  function syncGalleryMetasByType(type: GalleryType, nextFiles: File[]) {
    setGallery((prev) => {
      const prevGroup = prev[type]
      const nextMetas: GalleryMetaRow[] = []
      for (let i = 0; i < nextFiles.length; i++) {
        const old = prevGroup.metas[i]
        nextMetas.push(
          old ?? {
            title: '',
            description: '',
            imageType: type,
            isMainImage: false,
          },
        )
      }
      return { ...prev, [type]: { files: nextFiles, metas: nextMetas } }
    })
  }

  function setMetaFor(type: GalleryType, idx: number, patch: Partial<GalleryMetaRow>) {
    setGallery((prev) => {
      const g = prev[type]
      const nextMetas = g.metas.map((m, i) => (i === idx ? { ...m, ...patch, imageType: type } : m))
      return { ...prev, [type]: { ...g, metas: nextMetas } }
    })
  }

  function setMainImageFor(type: GalleryType, idx: number, isMain: boolean) {
    setGallery((prev) => {
      const next: Record<GalleryType, GalleryGroupState> = { ...prev }
      // Ensure only 1 "main" across all groups
      for (const k of Object.keys(next) as GalleryType[]) {
        const g = next[k]
        next[k] = {
          ...g,
          metas: g.metas.map((m) => ({ ...m, isMainImage: false })),
        }
      }
      if (isMain) {
        const g = next[type]
        next[type] = {
          ...g,
          metas: g.metas.map((m, i) => (i === idx ? { ...m, isMainImage: true, imageType: type } : m)),
        }
      }
      return next
    })
  }

  async function onSubmit() {
    setSubmitting(true)
    setResult(null)
    setGalleryErrors({})
    try {
      localStorage.setItem('admin_token', token)

      // Validate ảnh phụ:
      // - Color: bắt buộc Title + Description
      // - Nhóm khác: bắt buộc Description (Title tuỳ chọn)
      if (flattenedGallery.files.length > 0) {
        const errsByType: GalleryErrorsByType = {}
        for (const gg of GALLERY_GROUPS) {
          const st = gallery[gg.type]
          if (!st.files.length) continue
          const errs: Record<number, string> = {}
          for (let i = 0; i < st.files.length; i++) {
            const meta = st.metas[i]
            const missingDesc = !meta?.description?.trim()
            const missingTitle = gg.requireTitle ? !meta?.title?.trim() : false
            if (missingTitle && missingDesc) errs[i] = 'Bắt buộc nhập Title và Description'
            else if (missingTitle) errs[i] = 'Bắt buộc nhập Title'
            else if (missingDesc) errs[i] = 'Bắt buộc nhập Description'
          }
          if (Object.keys(errs).length) errsByType[gg.type] = errs
        }
        if (Object.keys(errsByType).length > 0) {
          setGalleryErrors(errsByType)
          setResult({ ok: false, message: 'Ảnh phụ thiếu meta bắt buộc. Vui lòng nhập đủ.' })
          return
        }
      }

      const fd = new FormData()

      fd.append('Name', name)
      fd.append('Brand', brand)
      fd.append('Year', year)
      fd.append('Model', model)
      fd.append('Color', color)
      if (price.trim()) fd.append('Price', price)
      if (fuelType) fd.append('FuelType', fuelType)
      if (mileage.trim()) fd.append('Mileage', mileage)
      fd.append('Description', description.trim())
      if (transmission) fd.append('Transmission', transmission)
      if (bodyStyle) fd.append('BodyStyle', bodyStyle)
      fd.append('Condition', condition)
      fd.append('Status', status)

      fd.append('ShowroomId', showroomId)
      fd.append('Quantity', quantity)
      fd.append('InventoriesJson', inventoriesJson)

      if (featureIds.trim()) fd.append('FeatureIds', featureIds)

      fd.append('SpecificationsJson', specificationsJson)
      fd.append('PricingVersionsJson', pricingVersionsJson)

      if (mainImage) fd.append('ImageFile', mainImage)

      for (const f of flattenedGallery.files) fd.append('GalleryFiles', f)
      fd.append('GalleryMetasJson', galleryMetasJson)

      const headers: Record<string, string> = {}
      if (token.trim()) headers.Authorization = token.trim().startsWith('Bearer ') ? token.trim() : `Bearer ${token.trim()}`

      const res = await http.post('/api/admin/cars/full', fd, { headers })
      setResult({ ok: true, message: res.data?.message ?? 'OK', data: res.data?.data })
    } catch (e: any) {
      const message =
        e?.response?.data?.message ??
        (typeof e?.response?.data === 'string' ? e.response.data : null) ??
        e?.message ??
        'Tạo xe thất bại'
      setResult({ ok: false, message })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="mx-auto w-full max-w-5xl px-4 py-8">
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-zinc-50">Thêm xe mới</h1>
      </div>

      <div className="mb-6 rounded-lg border bg-white p-4 text-zinc-900">
        <div className="text-sm font-medium text-zinc-800">Token Admin (tuỳ chọn)</div>
        <input
          value={token}
          onChange={(e) => setToken(e.target.value)}
          placeholder="Bearer eyJhbGciOi..."
          className="mt-2 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
        />
        <div className="mt-1 text-xs text-zinc-500">
          Nếu backend bật authorize cho route này, dán token vào để test nhanh.
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6">
        <section className="rounded-lg border bg-white p-4 text-zinc-900">
          <h2 className="text-base font-semibold text-zinc-900">Thông tin chính (Cars)</h2>

          <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
            <div>
              <label className="text-sm font-medium text-zinc-700">Tên xe</label>
              <input
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Hãng</label>
              <input
                value={brand}
                onChange={(e) => setBrand(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700">Năm</label>
              <input
                value={year}
                onChange={(e) => setYear(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Model</label>
              <input
                value={model}
                onChange={(e) => setModel(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700">Màu</label>
              <input
                value={color}
                onChange={(e) => setColor(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">ODO (km)</label>
              <input
                value={mileage}
                onChange={(e) => setMileage(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700">Nhiên liệu</label>
              <select
                value={fuelType}
                onChange={(e) => setFuelType(e.target.value as any)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              >
                <option value="">--</option>
                <option value="Xăng">Xăng</option>
                <option value="Điện">Điện</option>
                <option value="Dầu">Dầu</option>
                <option value="Hybrid">Hybrid</option>
              </select>
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Hộp số</label>
              <select
                value={transmission}
                onChange={(e) => setTransmission(e.target.value as any)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              >
                <option value="">--</option>
                <option value="Số sàn">Số sàn</option>
                <option value="Số tự động">Số tự động</option>
              </select>
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700">Kiểu dáng</label>
              <select
                value={bodyStyle}
                onChange={(e) => setBodyStyle(e.target.value as any)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              >
                <option value="">--</option>
                <option value="Sedan">Sedan</option>
                <option value="SUV">SUV</option>
                <option value="Hatchback">Hatchback</option>
                <option value="Crossover">Crossover</option>
                <option value="MPV">MPV</option>
                <option value="Bán tải">Bán tải</option>
                <option value="Coupe">Coupe</option>
              </select>
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Giá (tuỳ chọn)</label>
              <input
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                placeholder="VD: 950000000"
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
              <div className="mt-1 text-xs text-zinc-500">Nếu có pricing versions, backend sẽ set lại giá thấp nhất.</div>
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700">Tình trạng</label>
              <select
                value={condition}
                onChange={(e) => setCondition(e.target.value as any)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              >
                <option value="New">Xe mới</option>
                <option value="Used">Xe cũ</option>
              </select>
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Trạng thái</label>
              <select value={status} onChange={(e) => setStatus(e.target.value as any)} className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900">
                <option value="Available">Available</option>
                <option value="PendingApproval">PendingApproval</option>
                <option value="Draft">Draft</option>
                <option value="COMING_SOON">COMING_SOON</option>
                <option value="Out_of_stock">Out_of_stock</option>
              </select>
              <div className="mt-1 text-xs text-zinc-500">Sales tạo sẽ bị ép về PendingApproval ở backend.</div>
            </div>
          </div>

          <div className="mt-4">
            <label className="text-sm font-medium text-zinc-700">Mô tả</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="mt-1 min-h-24 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
            />
          </div>

          {/* <div className="mt-4">
            <label className="text-sm font-medium text-zinc-700">Thông tin khác</label>
            <textarea
              value={otherInfo}
              onChange={(e) => setOtherInfo(e.target.value)}
              placeholder="Nhập thêm các thông tin/ghi chú khác (hỗ trợ xuống dòng)."
              className="mt-1 min-h-24 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
            />
            <div className="mt-1 text-xs text-zinc-500">Sẽ được gộp vào Description khi submit (ngăn cách bằng dấu ---).</div>
          </div> */}

          <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
            <div>
              <label className="text-sm font-medium text-zinc-700">Ảnh chính</label>
              <input
                type="file"
                accept="image/*"
                onChange={(e) => setMainImage(e.target.files?.[0] ?? null)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">FeatureIds (comma-separated)</label>
              <input
                value={featureIds}
                onChange={(e) => setFeatureIds(e.target.value)}
                placeholder="VD: 1,2,3"
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 placeholder:text-zinc-400"
              />
            </div>
          </div>
        </section>

        <section className="rounded-lg border bg-white p-4 text-zinc-900">
          <h2 className="text-base font-semibold">Tồn kho (CarInventories)</h2>
          <div className="mt-2 text-sm text-zinc-600">
            Nếu không nhập danh sách kho, backend sẽ dùng <b>ShowroomId + Quantity</b> bên dưới.
          </div>

          <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-3">
            <div>
              <label className="text-sm font-medium text-zinc-700">ShowroomId (default)</label>
              <input
                value={showroomId}
                onChange={(e) => setShowroomId(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">Quantity (default)</label>
              <input
                value={quantity}
                onChange={(e) => setQuantity(e.target.value)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              />
            </div>
            <div>
              <label className="text-sm font-medium text-zinc-700">DisplayStatus (default)</label>
              <select
                value={displayStatus}
                onChange={(e) => setDisplayStatus(e.target.value as any)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900"
              >
                <option value="OnDisplay">OnDisplay</option>
                <option value="Pending">Pending</option>
                <option value="Out of stock">Out of stock</option>
              </select>
            </div>
          </div>

          <div className="mt-4 flex items-center justify-between">
            <div className="text-sm font-medium">Danh sách kho (tuỳ chọn)</div>
            <button
              type="button"
              onClick={() => setInventories((p) => [...p, { showroomId: showroomId, quantity: quantity, displayStatus }])}
              className="rounded-md border px-3 py-1.5 text-sm hover:bg-neutral-50"
            >
              + Thêm kho
            </button>
          </div>

          {inventories.length > 0 ? (
            <div className="mt-3 space-y-2">
              {inventories.map((row, idx) => (
                <div key={idx} className="grid grid-cols-1 gap-2 md:grid-cols-12">
                  <input
                    value={row.showroomId}
                    onChange={(e) =>
                      setInventories((p) => p.map((x, i) => (i === idx ? { ...x, showroomId: e.target.value } : x)))
                    }
                    placeholder="ShowroomId"
                    className="md:col-span-3 rounded-md border px-3 py-2 text-sm"
                  />
                  <input
                    value={row.quantity}
                    onChange={(e) => setInventories((p) => p.map((x, i) => (i === idx ? { ...x, quantity: e.target.value } : x)))}
                    placeholder="Quantity"
                    className="md:col-span-3 rounded-md border px-3 py-2 text-sm"
                  />
                  <input
                    value={row.displayStatus}
                    onChange={(e) =>
                      setInventories((p) => p.map((x, i) => (i === idx ? { ...x, displayStatus: e.target.value } : x)))
                    }
                    placeholder="DisplayStatus"
                    className="md:col-span-5 rounded-md border px-3 py-2 text-sm"
                  />
                  <button
                    type="button"
                    onClick={() => setInventories((p) => p.filter((_, i) => i !== idx))}
                    className="md:col-span-1 rounded-md border px-2 py-2 text-sm hover:bg-neutral-50"
                    aria-label="Remove inventory"
                  >
                    X
                  </button>
                </div>
              ))}
            </div>
          ) : null}
        </section>

        <section className="rounded-lg border bg-white p-4 text-zinc-900">
          <h2 className="text-base font-semibold">Thông số (CarSpecifications)</h2>
          <div className="mt-3 space-y-2">
            {specs.map((s, idx) => (
              <div key={idx} className="grid grid-cols-1 gap-2 md:grid-cols-12">
                <input
                  value={s.category}
                  onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, category: e.target.value } : x)))}
                  placeholder="Category"
                  className="md:col-span-3 rounded-md border px-3 py-2 text-sm"
                />
                <input
                  value={s.specName}
                  onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, specName: e.target.value } : x)))}
                  placeholder="SpecName"
                  className="md:col-span-4 rounded-md border px-3 py-2 text-sm"
                />
                <input
                  value={s.specValue}
                  onChange={(e) => setSpecs((p) => p.map((x, i) => (i === idx ? { ...x, specValue: e.target.value } : x)))}
                  placeholder="SpecValue"
                  className="md:col-span-4 rounded-md border px-3 py-2 text-sm"
                />
                <button
                  type="button"
                  onClick={() => setSpecs((p) => p.filter((_, i) => i !== idx))}
                  className="md:col-span-1 rounded-md border px-2 py-2 text-sm hover:bg-neutral-50"
                  aria-label="Remove spec"
                >
                  X
                </button>
              </div>
            ))}
          </div>

          <button
            type="button"
            onClick={() => setSpecs((p) => [...p, { category: '', specName: '', specValue: '' }])}
            className="mt-3 rounded-md border px-3 py-1.5 text-sm hover:bg-neutral-50"
          >
            + Thêm thông số
          </button>
        </section>

        <section className="rounded-lg border bg-white p-4 text-zinc-900">
          <h2 className="text-base font-semibold">Phiên bản / Giá (CarPricingVersions)</h2>

          <div className="mt-3 space-y-2">
            {pricing.map((p, idx) => (
              <div key={idx} className="grid grid-cols-1 gap-2 md:grid-cols-12">
                <input
                  value={p.versionName}
                  onChange={(e) =>
                    setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, versionName: e.target.value } : x)))
                  }
                  placeholder="VersionName"
                  className="md:col-span-4 rounded-md border px-3 py-2 text-sm"
                />
                <input
                  value={p.priceVnd}
                  onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, priceVnd: e.target.value } : x)))}
                  placeholder="PriceVnd"
                  className="md:col-span-3 rounded-md border px-3 py-2 text-sm"
                />
                <input
                  value={p.sortOrder}
                  onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, sortOrder: e.target.value } : x)))}
                  placeholder="SortOrder"
                  className="md:col-span-2 rounded-md border px-3 py-2 text-sm"
                />
                <label className="md:col-span-2 flex items-center gap-2 rounded-md border px-3 py-2 text-sm text-zinc-800">
                  <input
                    type="checkbox"
                    checked={p.isActive}
                    onChange={(e) => setPricing((prev) => prev.map((x, i) => (i === idx ? { ...x, isActive: e.target.checked } : x)))}
                  />
                  Active
                </label>
                <button
                  type="button"
                  onClick={() => setPricing((prev) => prev.filter((_, i) => i !== idx))}
                  className="md:col-span-1 rounded-md border px-2 py-2 text-sm hover:bg-neutral-50"
                  aria-label="Remove pricing"
                >
                  X
                </button>
              </div>
            ))}
          </div>

          <button
            type="button"
            onClick={() => setPricing((p) => [...p, { versionName: '', priceVnd: '', sortOrder: '', isActive: true }])}
            className="mt-3 rounded-md border px-3 py-1.5 text-sm hover:bg-neutral-50"
          >
            + Thêm phiên bản
          </button>
        </section>

        <section className="rounded-lg border bg-white p-4 text-zinc-900">
          <h2 className="text-base font-semibold">Ảnh phụ (CarImages)</h2>

          <div className="mt-2 text-xs text-neutral-500">
            Meta sẽ map theo thứ tự file trong từng nhóm, và khi submit sẽ ghép theo thứ tự nhóm: {GALLERY_GROUPS.map((g) => g.type).join(' → ')}.
          </div>

          <div className="mt-4 space-y-6">
            {GALLERY_GROUPS.map((group) => {
              const st = gallery[group.type]
              const errs = galleryErrors[group.type] ?? {}
              return (
                <div key={group.type} className="rounded-md border p-3">
                  <div className="flex flex-col gap-1 md:flex-row md:items-center md:justify-between">
                    <div className="text-sm font-semibold text-zinc-900">
                      {group.label}{' '}
                      <span className="text-xs font-normal text-zinc-500">(ImageType: {group.type})</span>
                    </div>
                    {group.hint ? <div className="text-xs text-zinc-500">{group.hint}</div> : null}
                  </div>

                  <div className="mt-3 grid grid-cols-1 gap-4 md:grid-cols-2">
                    <div>
                      <label className="text-sm font-medium text-zinc-700">Chọn ảnh</label>
                      <input
                        type="file"
                        multiple
                        accept="image/*"
                        onChange={(e) => {
                          const files = Array.from(e.target.files ?? [])
                          syncGalleryMetasByType(group.type, files)
                        }}
                        className="mt-1 w-full rounded-md border px-3 py-2 text-sm"
                      />
                      <div className="mt-1 text-xs text-neutral-500">Meta ảnh sẽ map theo thứ tự file đã chọn trong nhóm này.</div>
                    </div>
                  </div>

                  {st.files.length > 0 ? (
                    <div className="mt-4 space-y-3">
                      {st.files.map((f, idx) => (
                        <div key={idx} className="rounded-md border p-3">
                          <div className="text-sm font-medium">{f.name}</div>
                          <div className="mt-2 grid grid-cols-1 gap-2 md:grid-cols-12">
                            <input
                              value={st.metas[idx]?.title ?? ''}
                              onChange={(e) => setMetaFor(group.type, idx, { title: e.target.value })}
                              placeholder={group.defaultTitlePlaceholder ?? 'Title (tuỳ chọn)'}
                              className={
                                (errs[idx] ? 'border-red-400 ' : '') +
                                'md:col-span-3 rounded-md border bg-white px-3 py-2 text-sm text-zinc-900'
                              }
                            />

                            <div className="md:col-span-3 rounded-md border bg-neutral-50 px-3 py-2 text-sm text-zinc-700">
                              {group.type}
                            </div>

                            <textarea
                              value={st.metas[idx]?.description ?? ''}
                              onChange={(e) => setMetaFor(group.type, idx, { description: e.target.value })}
                              placeholder="Description (hỗ trợ xuống dòng)"
                              className={
                                (errs[idx] ? 'border-red-400 ' : '') +
                                'md:col-span-5 min-h-20 rounded-md border bg-white px-3 py-2 text-sm text-zinc-900'
                              }
                            />

                            <label className="md:col-span-1 flex items-center justify-center rounded-md border px-2 py-2 text-sm text-zinc-800">
                              <input
                                type="checkbox"
                                checked={st.metas[idx]?.isMainImage ?? false}
                                onChange={(e) => setMainImageFor(group.type, idx, e.target.checked)}
                                title="IsMainImage"
                              />
                            </label>
                          </div>

                          {errs[idx] ? <div className="mt-2 text-xs text-red-600">{errs[idx]}</div> : null}
                          <div className="mt-1 text-xs text-neutral-500">Tick ô cuối nếu đây là ảnh “main” trong bảng CarImages.</div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <div className="mt-3 text-xs text-zinc-500">Chưa chọn ảnh cho nhóm này.</div>
                  )}
                </div>
              )
            })}
          </div>
        </section>

        <div className="flex items-center gap-3">
          <button
            type="button"
            disabled={submitting}
            onClick={onSubmit}
            className="rounded-md bg-neutral-900 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
          >
            {submitting ? 'Đang tạo...' : 'Tạo xe'}
          </button>
          {result ? (
            <div className={result.ok ? 'text-sm text-green-700' : 'text-sm text-red-700'}>{result.message}</div>
          ) : null}
        </div>
      </div>
    </div>
  )
}

