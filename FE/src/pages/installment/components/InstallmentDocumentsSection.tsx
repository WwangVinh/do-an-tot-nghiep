import { Building2, UserRound } from 'lucide-react'

export function InstallmentDocumentsSection() {
  return (
    <section className="w-full bg-white py-14 sm:py-18">
      <div className="mx-auto w-full max-w-screen-2xl px-6">
        <div className="mx-auto mb-10 max-w-2xl text-center sm:mb-12">
          <h2 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
            <span className="block text-xs font-extrabold uppercase tracking-[0.35em] text-[#9B2335]">
              Thủ tục
            </span>
            <span className="mt-2 block text-slate-900">Hồ sơ cần chuẩn bị</span>
          </h2>
          <p className="mt-3 text-sm leading-relaxed text-slate-600 sm:text-base">
            Danh mục giấy tờ tham khảo — ngân hàng có thể yêu cầu bổ sung tùy hồ sơ cụ thể.
          </p>
          <div className="mx-auto mt-5 h-1 w-16 rounded-full bg-gradient-to-r from-[#9B2335] via-rose-500 to-[#1D3D63]" />
        </div>

        <div className="grid gap-6 lg:grid-cols-2 lg:gap-8">
          <article className="relative overflow-hidden rounded-2xl border border-slate-200/90 bg-slate-50/40 p-6 shadow-sm ring-1 ring-slate-900/[0.04] sm:p-8">
            <div
              className="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-rose-600 via-[#C41E3A] to-rose-700"
              aria-hidden
            />
            <div className="flex flex-col items-center gap-3 sm:flex-row sm:justify-center sm:gap-4">
              <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-white shadow-sm ring-1 ring-slate-200/80">
                <UserRound className="h-5 w-5 text-[#9B2335]" strokeWidth={2} aria-hidden />
              </span>
              <h3 className="text-center text-sm font-bold uppercase tracking-[0.08em] text-[#9B2335] sm:text-base">
                I. Đối với cá nhân
              </h3>
            </div>

            <ol className="mt-6 list-decimal space-y-4 pl-5 text-sm leading-relaxed text-slate-800 marker:font-bold marker:text-[#9B2335]">
              <li>
                <span className="font-semibold text-slate-900">
                  Chứng Minh Nhân Dân và Hộ khẩu.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Giấy chứng nhận độc thân hoặc Giấy chứng nhận đăng ký kết hôn.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Những giấy tờ chứng minh thu nhập:
                </span>
                <ul className="mt-2 list-disc space-y-2 pl-5 font-normal text-slate-700">
                  <li>
                    Giấy xác nhận mức lương và hợp đồng lao động, sổ tiết kiệm, tài khoản
                    cá nhân. Hợp đồng cho thuê nhà, cho thuê xe, cho thuê xưởng, giấy góp
                    vốn, cổ phần, cổ phiếu, trái phiếu.
                  </li>
                  <li>
                    Giấy tờ xác nhận sở hữu tài sản có giá trị: bất động sản, các loại xe ô
                    tô, máy móc, dây chuyền nhà máy, nhà xưởng...
                  </li>
                  <li>
                    Nếu cá nhân có công ty riêng mà thu nhập chủ yếu từ công ty thì cần
                    thêm: giấy phép kinh doanh, báo cáo thuế, báo cáo tài chính, bảng lương,
                    bảng chia lợi nhuận từ công ty.
                  </li>
                </ul>
              </li>
            </ol>
            <div className="mt-6 rounded-xl border border-amber-200/80 bg-amber-50/90 px-4 py-3 text-sm italic leading-relaxed text-amber-950/90">
              (Trong trường hợp cá nhân không đủ điều kiện vay Ngân hàng, có thể nhờ người
              thân có khả năng và thu nhập tốt làm giấy bảo lãnh cho Ngân hàng thẩm định).
            </div>
          </article>

          <article className="relative overflow-hidden rounded-2xl border border-slate-200/90 bg-slate-50/40 p-6 shadow-sm ring-1 ring-slate-900/[0.04] sm:p-8">
            <div
              className="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-slate-600 via-[#1D3D63] to-slate-700"
              aria-hidden
            />
            <div className="flex flex-col items-center gap-3 sm:flex-row sm:justify-center sm:gap-4">
              <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-white shadow-sm ring-1 ring-slate-200/80">
                <Building2 className="h-5 w-5 text-[#1D3D63]" strokeWidth={2} aria-hidden />
              </span>
              <h3 className="text-center text-sm font-bold uppercase tracking-[0.08em] text-[#1D3D63] sm:text-base">
                II. Đối với doanh nghiệp
              </h3>
            </div>

            <ol className="mt-6 list-decimal space-y-3 pl-5 text-sm leading-relaxed text-slate-800 marker:font-bold marker:text-[#1D3D63]">
              <li>
                <span className="font-semibold text-slate-900">Giấy phép kinh doanh.</span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">Mã số thuế.</span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Giấy bổ nhiệm Giám đốc, bổ nhiệm Kế toán trưởng.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">Giấy đăng ký sử dụng mẫu dấu.</span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Báo cáo thuế một năm (hoặc 6 tháng) gần nhất.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Báo cáo hóa đơn VAT một năm (hoặc 6 tháng) gần nhất.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Báo cáo tài chính một năm (hoặc 6 tháng) gần nhất.
                </span>
              </li>
              <li>
                <span className="font-semibold text-slate-900">
                  Hợp đồng kinh tế đầu ra, đầu vào.
                </span>
              </li>
            </ol>
          </article>
        </div>
      </div>
    </section>
  )
}
