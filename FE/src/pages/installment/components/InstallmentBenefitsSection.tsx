import { Star } from 'lucide-react'

const cards = [
  {
    title: 'Lợi ích khi mua xe trả góp',
    items: [
      'Số tiền vay lên đến 85% giá trị xe.',
      'Thời gian vay lên đến 8 năm.',
      'Tài trợ vay mua xe ô tô mới và xe ô tô đã qua sử dụng.',
      'Thủ tục vay đơn giản, thời gian xử lý hồ sơ nhanh chóng.',
      'Phương thức trả nợ linh hoạt: lãi trả hàng tháng/hàng quý, vốn trả theo phương thức vốn góp đều hoặc vốn góp bậc thang giảm dần.',
    ],
  },
  {
    title: 'Điều kiện mua xe trả góp',
    items: [
      'Cá nhân từ 18 tuổi trở lên.',
      'Doanh nghiệp thành lập trên 6 tháng.',
      'Có hợp đồng mua bán xe và các giấy tờ có liên quan.',
      'Có thu nhập đảm bảo cho việc trả nợ cho Ngân hàng.',
      'Có tài sản đảm bảo: là chính chiếc xe ô tô mà Quý khách mua, bất động sản hoặc tài sản khác.',
    ],
  },
  {
    title: 'Đối tượng thích hợp nhất',
    items: [
      'Doanh nhân, doanh nghiệp: những người có khả năng sử dụng tiền để kinh doanh sinh lời nhiều hơn là tỷ lệ lãi suất tiền cho vay của ngân hàng.',
      'Những người rất cần sử dụng xe, sẽ có đủ tiền mua xe trong một tương lai gần nhưng hiện tại chưa tập trung đủ tiền để mua xe trả thẳng.',
    ],
  },
] as const

export function InstallmentBenefitsSection() {
  return (
    <section className="w-full border-t border-slate-200/80 bg-gradient-to-b from-slate-50 to-slate-100/50 py-14 sm:py-18">
      <div className="mx-auto w-full max-w-screen-2xl px-6">
        <div className="mx-auto mb-10 max-w-2xl text-center sm:mb-12">
          <h2 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
            <span className="block text-xs font-extrabold uppercase tracking-[0.35em] text-[#1D3D63]/80">
              Chương trình
            </span>
            <span className="mt-2 block bg-gradient-to-r from-[#1D3D63] via-slate-800 to-[#1D3D63] bg-clip-text text-transparent">
              Tổng quan & điều kiện
            </span>
          </h2>
          <p className="mt-3 text-sm leading-relaxed text-slate-600 sm:text-base">
            Nắm rõ lợi ích, điều kiện và đối tượng phù hợp trước khi đăng ký vay.
          </p>
          <div className="mx-auto mt-5 h-1 w-16 rounded-full bg-gradient-to-r from-sky-500 via-[#b91c3c] to-[#1D3D63]" />
        </div>

        <div className="grid gap-6 md:grid-cols-3 md:gap-7">
          {cards.map((card) => (
            <article
              key={card.title}
              className="group flex flex-col rounded-2xl border border-slate-200/90 bg-white p-6 shadow-sm ring-1 ring-slate-900/[0.03] transition duration-300 hover:-translate-y-0.5 hover:shadow-xl hover:shadow-slate-200/90 sm:p-7"
            >
              <div className="flex flex-col items-center text-center">
                <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-gradient-to-br from-sky-500 to-[#1D3D63] shadow-lg shadow-sky-500/25 transition group-hover:scale-105">
                  <Star
                    className="h-7 w-7 fill-white text-white"
                    strokeWidth={1.5}
                    aria-hidden
                  />
                </div>
                <h3 className="mt-4 text-base font-bold leading-snug text-slate-900">
                  {card.title}
                </h3>
              </div>
              <ol className="mt-5 flex-1 list-decimal space-y-2.5 pl-5 text-left text-sm leading-relaxed text-slate-700 marker:font-semibold marker:text-[#1D3D63]">
                {card.items.map((line) => (
                  <li key={line} className="pl-1">
                    {line}
                  </li>
                ))}
              </ol>
            </article>
          ))}
        </div>
      </div>
    </section>
  )
}
