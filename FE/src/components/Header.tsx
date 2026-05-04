import { useState } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import { Menu, X } from 'lucide-react'

const navItems = [
  { to: '/', label: 'Trang chủ' },
  { to: '/san-pham', label: 'Sản phẩm' },
  { to: '/bang-gia-xe', label: 'Bảng giá xe' },
  { to: '/tra-gop', label: 'Trả góp' },
  { to: '/tin-tuc-uu-dai', label: 'Tin tức - Ưu đãi' },
  { to: '/ky-gui', label: 'Ký gửi xe' },
  { to: '/lien-he', label: 'Liên hệ' },
] as const

const STATUS_LABEL: Record<string, { text: string; color: string }> = {
  Pending:   { text: 'Chờ xác nhận', color: 'bg-amber-100 text-amber-700' },
  Confirmed: { text: 'Đã xác nhận',  color: 'bg-blue-100 text-blue-700' },
  Completed: { text: 'Hoàn thành',   color: 'bg-green-100 text-green-700' },
  Cancelled: { text: 'Đã huỷ',       color: 'bg-red-100 text-red-700' },
}
const PAY_LABEL: Record<string, { text: string; color: string }> = {
  Unpaid:    { text: 'Chưa thanh toán', color: 'text-red-500' },
  Deposited: { text: 'Đã đặt cọc',      color: 'text-amber-600' },
  Paid:      { text: 'Đã thanh toán',   color: 'text-green-600' },
}

function Logo() {
  return (
    <NavLink to="/" className="inline-flex items-center gap-3 text-slate-900" aria-label="Trang chủ">
      <svg width="42" height="28" viewBox="0 0 42 28" fill="none" xmlns="http://www.w3.org/2000/svg" className="shrink-0" aria-hidden="true">
        <path d="M6 4C10 8 12.5 12.5 21 24C29.5 12.5 32 8 36 4" stroke="#111827" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round" />
        <path d="M12 4C14.5 8 16.5 12.5 21 20C25.5 12.5 27.5 8 30 4" stroke="#111827" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round" opacity="0.45" />
      </svg>
      <div className="leading-tight">
        <div className="text-xl font-bold tracking-widest text-slate-900">CMC AUTOMOTIVE</div>
      </div>
    </NavLink>
  )
}

export function Header() {
  const navigate = useNavigate()
  const [mobileOpen, setMobileOpen] = useState(false)
  const [orderOpen, setOrderOpen] = useState(false)

  // Step 1: nhập SĐT
  const [phone, setPhone] = useState('')
  const [loadingOrders, setLoadingOrders] = useState(false)
  const [phoneError, setPhoneError] = useState('')

  // Step 2: danh sách đơn
  const [orders, setOrders] = useState<any[]>([])

  // Step 3: chi tiết 1 đơn
  const [selected, setSelected] = useState<any>(null)

  const handleClose = () => {
    setOrderOpen(false)
    setPhone('')
    setOrders([])
    setSelected(null)
    setPhoneError('')
  }

  const handleSearchByPhone = async () => {
    if (!phone.trim()) return
    setLoadingOrders(true)
    setPhoneError('')
    setOrders([])
    setSelected(null)
    try {
      const { env } = await import('../lib/env')
      // Gọi API lấy danh sách đơn theo SĐT
      const res = await fetch(
        `${env.VITE_API_BASE_URL}/api/public/orders/by-phone?phone=${encodeURIComponent(phone.trim())}`,
        { headers: { 'ngrok-skip-browser-warning': 'true' } }
      )
      const data = await res.json()
      if (!res.ok) throw new Error(data?.message || 'Không tìm thấy đơn hàng')
      const list = Array.isArray(data) ? data : Array.isArray(data?.data) ? data.data : data ? [data] : []
      if (!list.length) throw new Error('Số điện thoại này chưa có đơn hàng nào.')

      // ✅ Tự gọi confirm cho các đơn Unpaid để sync với PayOS
      const unpaidOrders = list.filter((o: any) => o.paymentStatus === 'Unpaid' && (o.orderId || o.id))
      if (unpaidOrders.length > 0) {
        const { env } = await import('../lib/env')
        await Promise.allSettled(
          unpaidOrders.map((o: any) =>
            fetch(`${env.VITE_API_BASE_URL}/api/Checkout/${o.orderId ?? o.id}/confirm`, {
              method: 'POST',
              headers: { 'ngrok-skip-browser-warning': 'true' }
            })
          )
        )
        // Reload lại danh sách sau khi confirm
        const res2 = await fetch(
          `${env.VITE_API_BASE_URL}/api/public/orders/by-phone?phone=${encodeURIComponent(phone.trim())}`,
          { headers: { 'ngrok-skip-browser-warning': 'true' } }
        )
        const data2 = await res2.json()
        const list2 = Array.isArray(data2) ? data2 : Array.isArray(data2?.data) ? data2.data : [data2]
        setOrders(list2)
      } else {
        setOrders(list)
      }
    } catch (e: any) {
      setPhoneError(e.message || 'Không tìm thấy đơn hàng')
    } finally {
      setLoadingOrders(false)
    }
  }

  return (
    <>
      <header className="sticky top-0 z-50 bg-white text-slate-900 shadow-sm shadow-slate-900/5 ring-1 ring-slate-900/5">
        <div className="mx-auto flex w-full max-w-screen-2xl items-center justify-between px-6 py-4">
          <Logo />

          {/* Desktop nav */}
          <nav className="hidden items-center gap-6 text-sm font-semibold lg:flex">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  ['transition-colors hover:text-slate-900', isActive ? 'font-bold text-slate-900' : 'text-slate-600'].join(' ')
                }
                end={item.to === '/'}
              >
                {item.label}
              </NavLink>
            ))}
            <button
              type="button"
              onClick={() => setOrderOpen(true)}
              className="flex items-center gap-1.5 rounded-lg border border-slate-200 px-3.5 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50 hover:text-slate-900"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
              Tra cứu đơn
            </button>
          </nav>

          {/* Mobile buttons */}
          <div className="flex items-center gap-2 lg:hidden">
            <button type="button" onClick={() => setOrderOpen(true)}
              className="inline-flex items-center justify-center rounded-lg p-2 text-slate-600 hover:bg-slate-100"
              aria-label="Tra cứu đơn hàng">
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </button>
            <button type="button"
              className="inline-flex items-center justify-center rounded-lg p-2 text-slate-600 hover:bg-slate-100"
              onClick={() => setMobileOpen((v) => !v)}
              aria-label={mobileOpen ? 'Đóng menu' : 'Mở menu'}>
              {mobileOpen ? <X size={22} /> : <Menu size={22} />}
            </button>
          </div>
        </div>

        {/* Mobile menu */}
        {mobileOpen && (
          <nav className="border-t border-slate-100 bg-white px-6 pb-4 pt-2 lg:hidden">
            <ul className="flex flex-col gap-1">
              {navItems.map((item) => (
                <li key={item.to}>
                  <NavLink to={item.to} end={item.to === '/'} onClick={() => setMobileOpen(false)}
                    className={({ isActive }) =>
                      ['block rounded-lg px-3 py-2.5 text-sm font-semibold transition-colors',
                        isActive ? 'bg-slate-100 text-slate-900' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'].join(' ')
                    }>
                    {item.label}
                  </NavLink>
                </li>
              ))}
            </ul>
          </nav>
        )}
      </header>

      {/* ── Modal tra cứu đơn hàng ── */}
      {orderOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4"
          onClick={(e) => { if (e.target === e.currentTarget) handleClose() }}>
          <div className="w-full max-w-md bg-white rounded-2xl shadow-2xl overflow-hidden">

            {/* Header */}
            <div className="bg-[#0A2540] px-6 py-4 flex items-center justify-between">
              <div className="flex items-center gap-2">
                {selected && (
                  <button onClick={() => setSelected(null)}
                    className="text-slate-400 hover:text-white transition w-7 h-7 flex items-center justify-center rounded-full hover:bg-white/10 mr-1">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                    </svg>
                  </button>
                )}
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
                <h2 className="text-white font-bold text-base tracking-wide">
                  {selected ? 'CHI TIẾT ĐƠN HÀNG' : 'TRA CỨU ĐƠN HÀNG'}
                </h2>
              </div>
              <button onClick={handleClose}
                className="text-slate-400 hover:text-white transition w-8 h-8 flex items-center justify-center rounded-full hover:bg-white/10">
                <X size={18} />
              </button>
            </div>

            <div className="px-6 py-6 space-y-4 max-h-[75vh] overflow-y-auto">

              {/* ── Step 1: Nhập SĐT ── */}
              {!orders.length && !selected && (
                <>
                  <p className="text-sm text-slate-500">Nhập số điện thoại đặt hàng để xem tất cả đơn của bạn.</p>
                  <div>
                    <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1.5">
                      Số điện thoại <span className="text-red-500">*</span>
                    </label>
                    <div className="flex gap-2">
                      <input type="tel" placeholder="0987 654 321" value={phone}
                        onChange={(e) => { setPhone(e.target.value); setPhoneError('') }}
                        onKeyDown={(e) => e.key === 'Enter' && handleSearchByPhone()}
                        className="flex-1 border border-slate-200 rounded-lg px-3.5 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:border-[#0A2540]"
                      />
                      <button onClick={handleSearchByPhone} disabled={!phone.trim() || loadingOrders}
                        className="bg-[#0A2540] disabled:opacity-40 hover:bg-[#0d345c] text-white font-bold px-4 rounded-lg text-sm transition flex items-center gap-1.5">
                        {loadingOrders ? (
                          <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"/>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                          </svg>
                        ) : 'Tìm'}
                      </button>
                    </div>
                    {phoneError && <p className="text-xs mt-1.5 text-red-500 font-medium">✗ {phoneError}</p>}
                  </div>
                </>
              )}

              {/* ── Step 2: Danh sách đơn ── */}
              {orders.length > 0 && !selected && (
                <>
                  <div className="flex items-center justify-between">
                    <p className="text-sm font-semibold text-slate-700">
                      Tìm thấy <span className="text-[#0A2540]">{orders.length}</span> đơn hàng
                    </p>
                    <button onClick={() => { setOrders([]); setPhone('') }}
                      className="text-xs text-slate-400 hover:text-slate-600 underline">Đổi SĐT</button>
                  </div>
                  <div className="space-y-2">
                    {orders.map((o: any) => (
                      <button key={o.orderCode} onClick={() => setSelected(o)}
                        className="w-full text-left border border-slate-200 hover:border-[#0A2540] hover:bg-blue-50 rounded-xl px-4 py-3 transition-all">
                        <div className="flex items-center justify-between mb-1">
                          <span className="font-black text-[#0A2540] text-sm tracking-wider">{o.orderCode}</span>
                          <span className={`text-xs font-bold px-2 py-0.5 rounded-full ${STATUS_LABEL[o.status]?.color ?? 'bg-slate-100 text-slate-600'}`}>
                            {STATUS_LABEL[o.status]?.text ?? o.status}
                          </span>
                        </div>
                        <p className="text-sm text-slate-700 font-medium truncate">{o.carName}</p>
                        <div className="flex items-center justify-between mt-1">
                          <span className="text-xs text-slate-400">{new Date(o.orderDate).toLocaleDateString('vi-VN')}</span>
                          <span className={`text-xs font-semibold ${PAY_LABEL[o.paymentStatus]?.color ?? 'text-slate-500'}`}>
                            {PAY_LABEL[o.paymentStatus]?.text ?? o.paymentStatus}
                          </span>
                        </div>
                      </button>
                    ))}
                  </div>
                </>
              )}

              {/* ── Step 3: Chi tiết đơn ── */}
              {selected && (
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <span className="font-black text-[#0A2540] text-lg tracking-wider">{selected.orderCode}</span>
                    <span className={`text-xs font-bold px-2.5 py-1 rounded-full ${STATUS_LABEL[selected.status]?.color ?? 'bg-slate-100 text-slate-600'}`}>
                      {STATUS_LABEL[selected.status]?.text ?? selected.status}
                    </span>
                  </div>

                  <div className="border border-slate-200 rounded-xl overflow-hidden text-sm">
                    <div className="bg-slate-50 px-4 py-2.5 text-xs font-bold text-slate-500 uppercase tracking-wider">Chi tiết</div>
                    <div className="px-4 py-3 space-y-2.5">
                      <div className="flex justify-between">
                        <span className="text-slate-500">Xe:</span>
                        <span className="font-semibold text-slate-800 text-right max-w-[200px]">{selected.carName}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-slate-500">Ngày đặt:</span>
                        <span className="font-medium text-slate-700">{new Date(selected.orderDate).toLocaleDateString('vi-VN')}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-slate-500">Tổng tiền:</span>
                        <span className="font-bold text-[#0A2540]">{new Intl.NumberFormat('vi-VN').format(selected.finalAmount)} đ</span>
                      </div>
                      <div className="flex justify-between items-center pt-2 border-t border-slate-100">
                        <span className="text-slate-500">Thanh toán:</span>
                        <span className={`font-bold ${PAY_LABEL[selected.paymentStatus]?.color ?? 'text-slate-600'}`}>
                          {PAY_LABEL[selected.paymentStatus]?.text ?? selected.paymentStatus}
                        </span>
                      </div>
                      {selected.accessories?.length > 0 && (
                        <div className="pt-2 border-t border-slate-100">
                          <p className="text-slate-500 mb-1.5">Phụ kiện:</p>
                          <ul className="space-y-0.5">
                            {selected.accessories.map((a: string, i: number) => (
                              <li key={i} className="text-slate-700 text-xs">• {a}</li>
                            ))}
                          </ul>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Nút tiếp tục thanh toán nếu chưa thanh toán */}
                  {selected.paymentStatus === 'Unpaid' && (
                    <button
                      onClick={() => {
                        handleClose()
                        navigate(`/san-pham/xe/${selected.carId}`, {
                          state: {
                            openOrder: true,
                            prefill: {
                              fullName: selected.fullName ?? '',
                              phone: selected.phone ?? '',
                              orderCode: selected.orderCode,
                              orderId: selected.orderId ?? selected.OrderId ?? selected.id,
                            }
                          }
                        })
                      }}
                      className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 rounded-xl transition text-sm flex items-center justify-center gap-2"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 9V7a5 5 0 00-10 0v2M5 9h14l1 11H4L5 9z" />
                      </svg>
                      Tiếp tục thanh toán đặt cọc →
                    </button>
                  )}

                  {/* Nút hủy đơn — chỉ khi Pending + Unpaid */}
                  {selected.status === 'Pending' && selected.paymentStatus === 'Unpaid' && (
                    <button
                      onClick={async () => {
                        if (!confirm(`Bạn có chắc muốn hủy đơn ${selected.orderCode}?`)) return
                        try {
                          const { env } = await import('../lib/env')
                          const res = await fetch(
                            `${env.VITE_API_BASE_URL}/api/public/orders/${selected.orderId ?? selected.OrderId}/cancel`,
                            { method: 'POST', headers: { 'ngrok-skip-browser-warning': 'true' } }
                          )
                          if (res.ok) {
                            alert('Đã hủy đơn hàng thành công.')
                            setSelected(null)
                            setOrders([])
                            setPhone('')
                          } else {
                            const data = await res.json()
                            alert(data?.message || 'Không thể hủy đơn, vui lòng liên hệ showroom.')
                          }
                        } catch {
                          alert('Lỗi kết nối, vui lòng thử lại.')
                        }
                      }}
                      className="w-full border border-red-200 text-red-600 hover:bg-red-50 font-semibold py-2.5 rounded-xl transition text-sm flex items-center justify-center gap-2"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                      </svg>
                      Hủy đơn hàng này
                    </button>
                  )}

                  {orders.length > 1 && (
                    <button onClick={() => setSelected(null)}
                      className="w-full border border-slate-200 text-slate-600 font-semibold py-2.5 rounded-xl hover:bg-slate-50 transition text-sm">
                      ← Xem đơn khác ({orders.length - 1} đơn còn lại)
                    </button>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </>
  )
}