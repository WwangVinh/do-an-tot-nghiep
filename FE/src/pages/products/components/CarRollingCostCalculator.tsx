import { useState, useMemo } from 'react'

// ── Dữ liệu phí theo tỉnh/thành ──
const PROVINCES: { label: string; trước_bạ: number; phí_biển: number }[] = [
  { label: 'Hà Nội',              trước_bạ: 0.12, phí_biển: 1_000_000 },
  { label: 'TP. Hồ Chí Minh',    trước_bạ: 0.10, phí_biển: 1_000_000 },
  { label: 'Đà Nẵng',            trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Hải Phòng',          trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Cần Thơ',            trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Bình Dương',         trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Đồng Nai',           trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Khánh Hòa',          trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Thừa Thiên Huế',     trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Nghệ An',            trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Thanh Hóa',          trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Bắc Ninh',           trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Hưng Yên',           trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Vĩnh Phúc',          trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Quảng Ninh',         trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Hà Tĩnh',            trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Bình Định',          trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Lâm Đồng',           trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Tiền Giang',         trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Long An',            trước_bạ: 0.10, phí_biển:   500_000 },
  { label: 'Các tỉnh còn lại',   trước_bạ: 0.10, phí_biển:   500_000 },
]

// Phí cố định (ước tính)
const PHI_DUONG_BO = 1_560_000    // 1 năm
const PHI_KIEM_DINH = 560_000     // 2 năm đầu
const BAO_HIEM_TNDS = 479_000     // 1 năm
const PHI_CONG_CHUNG = 200_000    // ước tính

function fmt(n: number) {
  return new Intl.NumberFormat('vi-VN').format(Math.round(n))
}

interface Props {
  basePrice: number // giá xe (VND)
  carName?: string
}

export function CarRollingCostCalculator({ basePrice, carName }: Props) {
  const [province, setProvince] = useState(PROVINCES[0])
  const [customPrice, setCustomPrice] = useState<string>('')

  const price = customPrice
    ? Number(customPrice.replace(/\D/g, ''))
    : basePrice

  const calc = useMemo(() => {
    const truocBa    = price * province.trước_bạ
    const phiBien    = province.phí_biển
    const duongBo    = PHI_DUONG_BO
    const kiemDinh   = PHI_KIEM_DINH
    const baoHiem    = BAO_HIEM_TNDS
    const congChung  = PHI_CONG_CHUNG
    const total      = price + truocBa + phiBien + duongBo + kiemDinh + baoHiem + congChung
    return { truocBa, phiBien, duongBo, kiemDinh, baoHiem, congChung, total }
  }, [price, province])

  const rows = [
    { label: 'Giá xe',               value: price,            highlight: false },
    { label: `Lệ phí trước bạ (${(province.trước_bạ * 100).toFixed(0)}%)`, value: calc.truocBa, highlight: false },
    { label: 'Phí đăng ký + biển số', value: calc.phiBien,    highlight: false },
    { label: 'Phí đường bộ (1 năm)', value: calc.duongBo,     highlight: false },
    { label: 'Phí kiểm định',         value: calc.kiemDinh,   highlight: false },
    { label: 'Bảo hiểm TNDS (1 năm)', value: calc.baoHiem,    highlight: false },
    { label: 'Công chứng giấy tờ',    value: calc.congChung,  highlight: false },
  ]

  return (
    <section className="mx-auto w-full max-w-screen-2xl px-6 py-12">
      <div className="max-w-2xl">
        <h2 className="text-2xl font-black text-[#0A2540] mb-1">Tính giá lăn bánh</h2>
        <p className="text-sm text-slate-500 mb-8">
          Ước tính chi phí thực tế khi đưa xe ra đường — bao gồm thuế, phí, bảo hiểm.
          <span className="ml-1 text-amber-600 font-medium">Số liệu mang tính tham khảo.</span>
        </p>

        {/* Inputs */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
          <div>
            <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1.5">
              Tỉnh / Thành phố đăng ký
            </label>
            <select
              value={province.label}
              onChange={(e) => setProvince(PROVINCES.find(p => p.label === e.target.value) ?? PROVINCES[0])}
              className="w-full border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
            >
              {PROVINCES.map(p => (
                <option key={p.label} value={p.label}>{p.label}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1.5">
              Giá xe (để trống = dùng giá niêm yết)
            </label>
            <input
              type="text"
              placeholder={`${fmt(basePrice)} đ`}
              value={customPrice}
              onChange={(e) => {
                const raw = e.target.value.replace(/\D/g, '')
                setCustomPrice(raw ? new Intl.NumberFormat('vi-VN').format(Number(raw)) : '')
              }}
              className="w-full border border-slate-200 rounded-xl px-4 py-3 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
            />
          </div>
        </div>

        {/* Bảng chi phí */}
        <div className="border border-slate-200 rounded-2xl overflow-hidden">
          <div className="bg-[#0A2540] px-5 py-3">
            <p className="text-white font-bold text-sm">{carName ?? 'Chi phí lăn bánh'}</p>
            <p className="text-slate-400 text-xs mt-0.5">{province.label}</p>
          </div>
          <div className="divide-y divide-slate-100">
            {rows.map((row) => (
              <div key={row.label} className="flex items-center justify-between px-5 py-3.5 hover:bg-slate-50 transition">
                <span className="text-sm text-slate-600">{row.label}</span>
                <span className="text-sm font-semibold text-slate-800">{fmt(row.value)} đ</span>
              </div>
            ))}
          </div>
          {/* Total */}
          <div className="bg-amber-50 border-t-2 border-amber-200 px-5 py-4 flex items-center justify-between">
            <div>
              <p className="text-xs text-amber-600 font-semibold uppercase tracking-wider mb-0.5">Tổng chi phí lăn bánh</p>
              <p className="text-xs text-slate-400">Bao gồm tất cả phí và thuế ước tính</p>
            </div>
            <div className="text-right">
              <p className="text-2xl font-black text-[#0A2540]">{fmt(calc.total)}</p>
              <p className="text-xs text-slate-400">đồng</p>
            </div>
          </div>
        </div>

        <p className="text-xs text-slate-400 mt-4 leading-relaxed">
          * Phí trước bạ tại Hà Nội là 12%, các tỉnh thành khác 10% (áp dụng cho xe dưới 9 chỗ giá &lt; 1.5 tỷ, theo Nghị định hiện hành).
          Các mức phí có thể thay đổi theo quy định của từng địa phương và thời điểm đăng ký.
        </p>
      </div>
    </section>
  )
}