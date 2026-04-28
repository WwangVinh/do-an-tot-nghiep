import { useMemo, useState } from 'react'

import type { CarCondition, CarStatus, CarUpsertInput } from '../../services/cars/cars'

function asNumber(v: string): number {
  const n = Number(v)
  return Number.isFinite(n) ? n : 0
}

export function CarUpsertForm({
  initial,
  submitting,
  submitLabel,
  onSubmit,
  onCancel,
}: {
  initial: CarUpsertInput
  submitting?: boolean
  submitLabel: string
  onSubmit: (input: CarUpsertInput) => void
  onCancel?: () => void
}) {
  const [form, setForm] = useState<CarUpsertInput>(initial)
  const [imageFile, setImageFile] = useState<File | null>(initial.imageFile ?? null)

  const canSubmit = useMemo(() => {
    return form.name.trim() && form.brand.trim() && form.year > 0
  }, [form.brand, form.name, form.year])

  return (
    <div className="mt-6">
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tên xe</label>
          <input
            value={form.name}
            onChange={(e) => setForm((p) => ({ ...p, name: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Hãng</label>
          <input
            value={form.brand}
            onChange={(e) => setForm((p) => ({ ...p, brand: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Model</label>
          <input
            value={form.model ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, model: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Năm</label>
          <input
            value={String(form.year ?? '')}
            onChange={(e) => setForm((p) => ({ ...p, year: asNumber(e.target.value) }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Giá</label>
          <input
            value={String(form.price ?? 0)}
            onChange={(e) => setForm((p) => ({ ...p, price: asNumber(e.target.value) }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Màu</label>
          <input
            value={form.color ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, color: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">ODO (km)</label>
          <input
            value={String(form.mileage ?? 0)}
            onChange={(e) => setForm((p) => ({ ...p, mileage: asNumber(e.target.value) }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Nhiên liệu</label>
          <select
            value={form.fuelType ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, fuelType: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          >
            <option value="">--</option>
            <option value="Xăng">Xăng</option>
            <option value="Điện">Điện</option>
            <option value="Dầu">Dầu</option>
            <option value="Hybrid">Hybrid</option>
          </select>
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Hộp số</label>
          <select
            value={form.transmission ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, transmission: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          >
            <option value="">--</option>
            <option value="Số sàn">Số sàn</option>
            <option value="Số tự động">Số tự động</option>
          </select>
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Kiểu dáng</label>
          <select
            value={form.bodyStyle ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, bodyStyle: e.target.value }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
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
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Tình trạng</label>
          <select
            value={form.condition as CarCondition}
            onChange={(e) => setForm((p) => ({ ...p, condition: e.target.value as CarCondition }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          >
            <option value="New">Xe mới</option>
            <option value="Used">Xe cũ</option>
          </select>
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Trạng thái</label>
          <select
            value={(form.status ?? '') as string}
            onChange={(e) => setForm((p) => ({ ...p, status: (e.target.value || undefined) as CarStatus | undefined }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          >
            <option value="">(để backend quyết định)</option>
            <option value="Available">Available</option>
            <option value="PendingApproval">PendingApproval</option>
            <option value="Draft">Draft</option>
            <option value="COMING_SOON">COMING_SOON</option>
            <option value="Out_of_stock">Out_of_stock</option>
          </select>
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">ShowroomId</label>
          <input
            value={String(form.showroomId ?? 1)}
            onChange={(e) => setForm((p) => ({ ...p, showroomId: asNumber(e.target.value) }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Số lượng</label>
          <input
            value={String(form.quantity ?? 1)}
            onChange={(e) => setForm((p) => ({ ...p, quantity: asNumber(e.target.value) }))}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Ảnh chính</label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setImageFile(e.target.files?.[0] ?? null)}
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100"
          />
          {imageFile ? <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">{imageFile.name}</div> : null}
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">Mô tả</label>
          <textarea
            value={form.description ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, description: e.target.value }))}
            className="mt-2 min-h-24 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">FeatureIds (comma-separated)</label>
          <input
            value={form.featureIds ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, featureIds: e.target.value }))}
            placeholder="VD: 1,2,3"
            className="mt-2 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>

        <div className="md:col-span-2">
          <label className="text-sm font-medium text-slate-700 dark:text-zinc-200">
            Specifications (format cũ)
          </label>
          <textarea
            value={form.specifications ?? ''}
            onChange={(e) => setForm((p) => ({ ...p, specifications: e.target.value }))}
            placeholder="VD: Động cơ|Mã lực|300 HP;Kích thước|Chiều dài|4940 mm"
            className="mt-2 min-h-20 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500"
          />
        </div>
      </div>

      <div className="mt-6 flex items-center gap-3">
        <button
          type="button"
          disabled={Boolean(submitting) || !canSubmit}
          onClick={() => onSubmit({ ...form, imageFile })}
          className="rounded-md bg-slate-900 px-3 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-60 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
        >
          {submitting ? 'Đang lưu...' : submitLabel}
        </button>
        {onCancel ? (
          <button
            type="button"
            disabled={Boolean(submitting)}
            onClick={onCancel}
            className="rounded-md border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 hover:bg-slate-100 disabled:opacity-60 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
          >
            Huỷ
          </button>
        ) : null}
      </div>
    </div>
  )
}

