import { useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Link } from 'react-router-dom'

export function PaymentSuccessPage() {
  const navigate = useNavigate()
  const [params] = useSearchParams()
  const orderCode = params.get('orderCode')

  useEffect(() => {
    const timer = setTimeout(() => navigate('/'), 5000)
    return () => clearTimeout(timer)
  }, [])

  return (
    <main className="min-h-screen bg-white flex items-center justify-center px-4">
      <div className="max-w-md w-full text-center space-y-6">
        {/* Icon */}
        <div className="w-24 h-24 rounded-full bg-green-100 flex items-center justify-center mx-auto">
          <svg className="w-12 h-12 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
          </svg>
        </div>

        <div>
          <h1 className="text-2xl font-black text-slate-900 mb-2">Thanh toán thành công! 🎉</h1>
          <p className="text-slate-500 text-sm">Đặt cọc của bạn đã được ghi nhận.</p>
          {orderCode && (
            <div className="mt-3 inline-block bg-slate-100 rounded-xl px-4 py-2">
              <span className="text-xs text-slate-500 font-semibold uppercase tracking-wider">Mã đơn hàng</span>
              <p className="font-black text-[#0A2540] text-lg tracking-widest">{orderCode}</p>
            </div>
          )}
        </div>

        <div className="bg-blue-50 border border-blue-100 rounded-xl p-4 text-sm text-blue-700">
          Nhân viên sẽ liên hệ xác nhận đơn hàng trong vòng <strong>24 giờ</strong>.
        </div>

        <p className="text-xs text-slate-400">Tự động về trang chủ sau 5 giây...</p>

        <Link
          to="/"
          className="inline-flex items-center gap-2 bg-[#0A2540] hover:bg-[#0d345c] text-white font-bold px-8 py-3 rounded-xl transition text-sm"
        >
          Về trang chủ ngay
        </Link>
      </div>
    </main>
  )
}