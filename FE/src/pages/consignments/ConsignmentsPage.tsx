import { useState } from 'react'
import toast from 'react-hot-toast'
import {
  Car, CheckCircle2, ChevronRight, Clock,
  FileText, Phone, Shield, TrendingUp,
} from 'lucide-react'
import { createConsignment, type ConsignmentCreatePayload } from '../../services/consignments/consignments'

const inputCls =
  'h-12 w-full rounded-lg border border-slate-200 bg-white px-4 text-sm text-slate-900 ' +
  'placeholder:text-slate-400 outline-none transition focus:border-rose-400 focus:ring-2 focus:ring-rose-100'

const textareaCls =
  'w-full rounded-lg border border-slate-200 bg-white px-4 py-3 text-sm text-slate-900 ' +
  'placeholder:text-slate-400 outline-none transition focus:border-rose-400 focus:ring-2 focus:ring-rose-100 resize-none'

const BENEFITS = [
  { icon: TrendingUp, title: 'Giá tốt nhất thị trường', desc: 'Chúng tôi cam kết định giá minh bạch, sát giá thị trường thực tế.' },
  { icon: Shield,     title: 'An toàn & tin cậy',        desc: 'Hợp đồng ký gửi rõ ràng, xe được bảo quản trong showroom có bảo hiểm.' },
  { icon: Clock,      title: 'Bán nhanh — nhận tiền sớm', desc: 'Hệ thống khách hàng rộng lớn giúp xe của bạn tìm được chủ mới nhanh nhất.' },
  { icon: FileText,   title: 'Thủ tục đơn giản',          desc: 'Chỉ cần điền form — nhân viên sẽ liên hệ và hướng dẫn toàn bộ quy trình.' },
]

const STEPS = [
  { num: '01', title: 'Điền thông tin', desc: 'Mô tả xe và giá kỳ vọng của bạn qua form dưới đây.' },
  { num: '02', title: 'Thẩm định xe',   desc: 'Chuyên viên liên hệ để kiểm tra thực tế và định giá chính xác.' },
  { num: '03', title: 'Ký hợp đồng',    desc: 'Ký hợp đồng ký gửi minh bạch, rõ ràng về hoa hồng và điều khoản.' },
  { num: '04', title: 'Nhận tiền',      desc: 'Xe bán xong, tiền về tay bạn ngay — không chậm trễ.' },
]

const DOCUMENTS = [
  'Đăng ký xe (bản gốc)',
  'CMND/CCCD của chủ xe',
  'Bảo hiểm xe còn hiệu lực',
  'Sổ bảo dưỡng (nếu có)',
]

type FormState = {
  guestName: string
  guestPhone: string
  guestEmail: string
  brand: string
  model: string
  year: string
  mileage: string
  conditionDescription: string
  expectedPrice: string
}

const EMPTY: FormState = {
  guestName: '', guestPhone: '', guestEmail: '',
  brand: '', model: '', year: '', mileage: '',
  conditionDescription: '', expectedPrice: '',
}

function formatBillion(value: string): string {
  const n = Number(value)
  if (!value || isNaN(n) || n <= 0) return ''
  if (n >= 1_000_000_000) return `≈ ${(n / 1_000_000_000).toFixed(3).replace(/\.?0+$/, '')} tỷ đồng`
  return `≈ ${(n / 1_000_000).toFixed(0)} triệu đồng`
}

function validate(form: FormState): string | null {
  if (!form.guestName.trim())                 return 'Vui lòng nhập họ tên.'
  if (!form.guestPhone.trim())                return 'Vui lòng nhập số điện thoại.'
  if (!form.brand.trim())                     return 'Vui lòng nhập hãng xe.'
  if (!form.model.trim())                     return 'Vui lòng nhập dòng xe.'
  if (!form.year || isNaN(Number(form.year))) return 'Vui lòng nhập năm sản xuất hợp lệ.'
  if (Number(form.year) < 1900 || Number(form.year) > new Date().getFullYear())
    return `Năm sản xuất phải từ 1900 đến ${new Date().getFullYear()}.`
  if (!form.expectedPrice || isNaN(Number(form.expectedPrice))) return 'Vui lòng nhập giá kỳ vọng.'
  if (Number(form.expectedPrice) <= 0) return 'Giá kỳ vọng phải lớn hơn 0.'
  return null
}

export function KyGuiPage() {
  const [form, setForm]             = useState<FormState>(EMPTY)
  const [submitting, setSubmitting] = useState(false)
  const [success, setSuccess]       = useState(false)

  function set(field: keyof FormState) {
    return (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
      setForm(f => ({ ...f, [field]: e.target.value }))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    const error = validate(form)
    if (error) { toast.error(error); return }

    const payload: ConsignmentCreatePayload = {
      guestName:            form.guestName.trim(),
      guestPhone:           form.guestPhone.trim(),
      guestEmail:           form.guestEmail.trim() || null,
      brand:                form.brand.trim(),
      model:                form.model.trim(),
      year:                 Number(form.year),
      mileage:              Number(form.mileage) || 0,
      conditionDescription: form.conditionDescription.trim() || null,
      expectedPrice:        Number(form.expectedPrice),
    }

    try {
      setSubmitting(true)
      const res = await createConsignment(payload)
      if (!res.success) { toast.error(res.message); return }
      setSuccess(true)
      setForm(EMPTY)
      window.scrollTo({ top: 0, behavior: 'smooth' })
    } catch {
      toast.error('Có lỗi xảy ra, vui lòng thử lại hoặc liên hệ hotline.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main className="w-full bg-white">

      <section
        className="relative border-b border-slate-100"
        style={{
          backgroundImage: 'url(/src/assets/images/banner3.jpg)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        <div className="absolute inset-0 bg-black/60" />
        <div className="relative mx-auto max-w-screen-2xl px-5 py-14 sm:px-6 lg:px-8 lg:py-20">
          <div className="max-w-2xl">
            <div className="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1 text-xs font-semibold uppercase tracking-widest text-white/80">
              <Car size={12} /> Ký gửi xe
            </div>
            <h1 className="mt-4 text-3xl font-extrabold tracking-tight text-white sm:text-4xl lg:text-5xl">
              Bán xe nhanh,<br />
              <span className="text-rose-400">không lo thủ tục</span>
            </h1>
            <p className="mt-5 text-base leading-relaxed text-white/80 sm:text-lg">
              Gửi xe cho chúng tôi bán hộ — bạn không cần lo về giấy tờ, giao dịch hay thương lượng.
              Chỉ cần điền form, nhân viên sẽ liên hệ trong vòng{' '}
              <strong className="text-white">24 giờ</strong>.
            </p>
            <a
              href="#form-ky-gui"
              className="mt-7 inline-flex items-center gap-2 rounded-xl bg-rose-600 px-6 py-3.5 text-sm font-bold text-white transition hover:bg-rose-700"
            >
              Đăng ký ngay <ChevronRight size={16} />
            </a>
          </div>
        </div>
      </section>

      <section className="border-b border-slate-100">
        <div className="mx-auto max-w-screen-2xl px-5 py-12 sm:px-6 lg:px-8 lg:py-16">
          <h2 className="mb-8 text-center text-xl font-bold text-slate-900 sm:text-2xl">
            Tại sao chọn ký gửi tại CMC Automotive?
          </h2>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
            {BENEFITS.map((b) => (
              <div key={b.title} className="rounded-2xl border border-slate-100 bg-slate-50 p-6">
                <div className="mb-4 inline-flex h-11 w-11 items-center justify-center rounded-xl bg-rose-50 text-rose-600">
                  <b.icon size={20} />
                </div>
                <h3 className="mb-2 text-sm font-bold text-slate-900">{b.title}</h3>
                <p className="text-sm leading-relaxed text-slate-500">{b.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="border-b border-slate-100 bg-slate-50">
        <div className="mx-auto max-w-screen-2xl px-5 py-12 sm:px-6 lg:px-8 lg:py-16">
          <h2 className="mb-10 text-center text-xl font-bold text-slate-900 sm:text-2xl">
            Quy trình ký gửi xe
          </h2>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
            {STEPS.map((s, i) => (
              <div key={s.num} className="relative">
                {i < STEPS.length - 1 && (
                  <div className="absolute top-6 hidden h-px bg-slate-200 lg:block"
                    style={{ left: '3rem', right: '-1rem' }} />
                )}
                <div className="flex items-start gap-4">
                  <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-rose-600 text-sm font-black text-white">
                    {s.num}
                  </div>
                  <div>
                    <h3 className="text-sm font-bold text-slate-900">{s.title}</h3>
                    <p className="mt-1 text-sm leading-relaxed text-slate-500">{s.desc}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section id="form-ky-gui" className="border-b border-slate-100">
        <div className="mx-auto max-w-screen-2xl px-5 py-12 sm:px-6 lg:px-8 lg:py-16">
          <h2 className="mb-8 text-xl font-bold text-slate-900 sm:text-2xl">
            Đăng ký ký gửi xe
          </h2>

          <div className="grid grid-cols-1 gap-8 lg:grid-cols-3">
            <div className="lg:col-span-2">
              {success ? (
                <div className="flex flex-col items-center gap-4 rounded-2xl border border-emerald-200 bg-emerald-50 px-6 py-12 text-center">
                  <CheckCircle2 size={48} className="text-emerald-500" />
                  <h3 className="text-lg font-bold text-emerald-800">Gửi yêu cầu thành công!</h3>
                  <p className="max-w-sm text-sm text-emerald-700">
                    Chúng tôi đã nhận được thông tin xe của bạn. Nhân viên sẽ liên hệ trong vòng 24 giờ để thẩm định và tư vấn.
                  </p>
                  <button
                    type="button"
                    onClick={() => setSuccess(false)}
                    className="mt-2 rounded-xl bg-emerald-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700"
                  >
                    Gửi xe khác
                  </button>
                </div>
              ) : (
                <form onSubmit={handleSubmit} noValidate className="space-y-6">
                  <fieldset>
                    <p className="mb-3 text-xs font-bold uppercase tracking-widest text-slate-400">
                      Thông tin liên hệ
                    </p>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Họ và tên <span className="text-rose-500">*</span>
                        </label>
                        <input value={form.guestName} onChange={set('guestName')} placeholder="Nguyễn Văn A" className={inputCls} />
                      </div>
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Số điện thoại <span className="text-rose-500">*</span>
                        </label>
                        <input value={form.guestPhone} onChange={set('guestPhone')} placeholder="0901 234 567" inputMode="tel" className={inputCls} />
                      </div>
                    </div>
                    <div className="mt-4">
                      <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                        Email <span className="font-normal text-slate-400">(tuỳ chọn)</span>
                      </label>
                      <input value={form.guestEmail} onChange={set('guestEmail')} placeholder="email@example.com" type="email" className={inputCls} />
                    </div>
                  </fieldset>

                  <fieldset>
                    <p className="mb-3 text-xs font-bold uppercase tracking-widest text-slate-400">
                      Thông tin xe
                    </p>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Hãng xe <span className="text-rose-500">*</span>
                        </label>
                        <input value={form.brand} onChange={set('brand')} placeholder="Toyota, VinFast, Honda..." className={inputCls} />
                      </div>
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Dòng xe <span className="text-rose-500">*</span>
                        </label>
                        <input value={form.model} onChange={set('model')} placeholder="Camry, VF8, Civic..." className={inputCls} />
                      </div>
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Năm sản xuất <span className="text-rose-500">*</span>
                        </label>
                        <input value={form.year} onChange={set('year')} placeholder="2020" type="number" min={1990} max={new Date().getFullYear()} className={inputCls} />
                      </div>
                      <div>
                        <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                          Số km đã đi <span className="font-normal text-slate-400">(km)</span>
                        </label>
                        <input value={form.mileage} onChange={set('mileage')} placeholder="50000" type="number" min={0} className={inputCls} />
                      </div>
                    </div>

                    <div className="mt-4">
                      <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                        Giá kỳ vọng (VNĐ) <span className="text-rose-500">*</span>
                      </label>
                      <input value={form.expectedPrice} onChange={set('expectedPrice')} placeholder="500000000" type="number" min={0} className={inputCls} />
                      {formatBillion(form.expectedPrice) && (
                        <p className="mt-1 text-xs text-slate-400">{formatBillion(form.expectedPrice)}</p>
                      )}
                    </div>

                    <div className="mt-4">
                      <label className="mb-1.5 block text-xs font-semibold text-slate-600">
                        Mô tả tình trạng xe <span className="font-normal text-slate-400">(tuỳ chọn)</span>
                      </label>
                      <textarea value={form.conditionDescription} onChange={set('conditionDescription')} placeholder="Xe còn đẹp, không đâm đụng, đầy đủ giấy tờ, đã thay dầu máy định kỳ..." rows={4} className={textareaCls} />
                    </div>
                  </fieldset>

                  <button
                    type="submit"
                    disabled={submitting}
                    className="inline-flex h-12 w-full items-center justify-center gap-2 rounded-xl bg-rose-600 text-sm font-bold text-white transition hover:bg-rose-700 disabled:opacity-60 sm:w-auto sm:px-10"
                  >
                    {submitting ? 'Đang gửi...' : 'Gửi yêu cầu ký gửi'}
                    {!submitting && <ChevronRight size={16} />}
                  </button>
                </form>
              )}
            </div>

            <div className="space-y-5 lg:sticky lg:top-24">
              <div className="rounded-2xl border border-slate-200 bg-slate-50 p-6">
                <h3 className="mb-4 text-sm font-bold text-slate-900">Cần tư vấn trực tiếp?</h3>
                <a href="tel:0333436743" className="flex items-center gap-3 rounded-xl bg-rose-600 px-4 py-3.5 text-white transition hover:bg-rose-700">
                  <Phone size={18} />
                  <div>
                    <div className="text-sm font-bold">Hotline: 0333 436 743</div>
                    <div className="text-xs text-white/80">Thứ 2 – Chủ nhật, 8:00 – 20:00</div>
                  </div>
                </a>
              </div>

              <div className="rounded-2xl border border-slate-200 bg-white p-6">
                <h3 className="mb-4 text-sm font-bold text-slate-900">Giấy tờ cần chuẩn bị</h3>
                <ul className="space-y-2.5">
                  {DOCUMENTS.map((item) => (
                    <li key={item} className="flex items-start gap-2 text-sm text-slate-600">
                      <CheckCircle2 size={15} className="mt-0.5 shrink-0 text-emerald-500" />
                      {item}
                    </li>
                  ))}
                </ul>
                <p className="mt-4 text-xs text-slate-400">Nhân viên sẽ hướng dẫn chi tiết sau khi liên hệ.</p>
              </div>
            </div>
          </div>
        </div>
      </section>

    </main>
  )
}