import { Link } from 'react-router-dom'

export function PaymentFailedPage() {
  return (
    <main className="min-h-screen bg-white flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center space-y-6">
        <div className="w-24 h-24 rounded-full bg-red-100 flex items-center justify-center mx-auto">
          <svg className="w-12 h-12 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </div>

        <div>
          <h1 className="text-2xl font-black text-slate-900 mb-2">Thanh toán thất bại</h1>
          <p className="text-slate-500 text-sm">Giao dịch bị hủy hoặc có lỗi xảy ra.</p>
        </div>

        <div className="bg-amber-50 border border-amber-100 rounded-xl p-4 text-sm text-amber-700">
          Đơn hàng vẫn được lưu. Bạn có thể thử lại qua <strong>Tra cứu đơn</strong> trên thanh điều hướng.
        </div>

        <Link
          to="/"
          className="inline-flex items-center gap-2 bg-[#0A2540] hover:bg-[#0d345c] text-white font-bold px-8 py-3 rounded-xl transition text-sm"
        >
          Về trang chủ
        </Link>
      </div>
    </main>
  )
}