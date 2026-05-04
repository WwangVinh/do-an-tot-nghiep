function CarReviews({ carId }: { carId: number }) {
  const [reviews, setReviews] = useState<Review[]>([])

  // Bước 1: nhập SĐT
  const [phone, setPhone] = useState('')
  const [checking, setChecking] = useState(false)
  const [checkError, setCheckError] = useState('')

  // Sau khi xác minh xong
  const [verifiedName, setVerifiedName] = useState<string | null>(null) // null = chưa xác minh

  // Bước 2: viết review
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [submitMsg, setSubmitMsg] = useState({ type: '', text: '' })

  const fetchReviews = async () => {
    try {
      const res = await axios.get(`${env.VITE_API_BASE_URL}/api/Reviews/car/${carId}`)
      setReviews(res.data?.data ?? res.data ?? [])
    } catch {
      console.error('Lỗi tải đánh giá')
    }
  }

  useEffect(() => { fetchReviews() }, [carId])

  // Kiểm tra SĐT
  const handleVerifyPhone = async () => {
    if (!phone.trim()) return
    setChecking(true)
    setCheckError('')
    setVerifiedName(null)
    try {
      // API trả về { fullName: string } nếu hợp lệ
      const res = await axios.get(
        `${env.VITE_API_BASE_URL}/api/Reviews/verify-phone?phone=${phone.trim()}&carId=${carId}`
      )
      const name = res.data?.fullName ?? res.data?.data?.fullName
      if (name) {
        setVerifiedName(name)
      } else {
        setCheckError('Số điện thoại chưa có lịch sử giao dịch được xác nhận với showroom.')
      }
    } catch (err: any) {
      setCheckError(
        err.response?.data?.message ||
        'Số điện thoại chưa có lịch sử giao dịch được xác nhận với showroom.'
      )
    } finally {
      setChecking(false)
    }
  }

  const handleSubmitReview = async () => {
    if (!verifiedName || !comment.trim()) return
    setSubmitting(true)
    setSubmitMsg({ type: '', text: '' })
    try {
      const res = await axios.post(`${env.VITE_API_BASE_URL}/api/Reviews/submit`, {
        carId,
        fullName: verifiedName,
        phone: phone.trim(),
        rating,
        comment,
      })
      setSubmitMsg({ type: 'success', text: res.data?.message || 'Đăng đánh giá thành công!' })
      setComment('')
      setRating(5)
      setVerifiedName(null)
      setPhone('')
      fetchReviews()
    } catch (err: any) {
      setSubmitMsg({
        type: 'error',
        text: err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.',
      })
    } finally {
      setSubmitting(false)
    }
  }

  const avgRating = reviews.length
    ? (reviews.reduce((s, r) => s + r.rating, 0) / reviews.length).toFixed(1)
    : null

  return (
    <section className="mx-auto w-full max-w-screen-2xl px-6 py-12">
      {avgRating && (
        <div className="flex items-center gap-4 mb-8 p-5 bg-slate-50 rounded-2xl border border-slate-100 w-fit">
          <div className="text-5xl font-black text-[#0A2540]">{avgRating}</div>
          <div>
            <div className="flex text-amber-400 text-lg mb-0.5">
              {Array.from({ length: Math.round(Number(avgRating)) }).map((_, i) => (
                <span key={i}>★</span>
              ))}
            </div>
            <p className="text-xs text-slate-500">{reviews.length} đánh giá</p>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-10">
        {/* ── FORM GỬI ĐÁNH GIÁ ── */}
        <div className="bg-slate-50 p-6 rounded-2xl border border-slate-100 h-fit">
          <h3 className="text-base font-bold text-slate-900 mb-4">Gửi đánh giá của bạn</h3>

          {/* Thông báo submit */}
          {submitMsg.text && (
            <div
              className={`mb-4 px-4 py-2 text-sm rounded-xl border ${
                submitMsg.type === 'success'
                  ? 'bg-green-50 text-green-700 border-green-200'
                  : 'bg-rose-50 text-rose-700 border-rose-200'
              }`}
            >
              {submitMsg.text}
            </div>
          )}

          {/* ─ BƯỚC 1: Nhập SĐT xác minh ─ */}
          {!verifiedName ? (
            <div className="space-y-3">
              <p className="text-xs text-slate-500 leading-relaxed bg-blue-50 border border-blue-100 rounded-lg px-3 py-2">
                💡 Chỉ khách hàng đã booking hoặc đặt cọc xe và được showroom xác nhận mới có thể đánh giá.
              </p>
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Số điện thoại <span className="text-red-500">*</span>
                </label>
                <div className="flex gap-2">
                  <input
                    type="tel"
                    placeholder="0987 654 321"
                    className="flex-1 bg-white border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:border-slate-400"
                    value={phone}
                    onChange={(e) => { setPhone(e.target.value); setCheckError('') }}
                    onKeyDown={(e) => { if (e.key === 'Enter') handleVerifyPhone() }}
                  />
                  <button
                    onClick={handleVerifyPhone}
                    disabled={!phone.trim() || checking}
                    className="bg-[#0A2540] disabled:opacity-40 hover:bg-[#0d345c] text-white px-4 rounded-xl text-xs font-bold transition flex-shrink-0"
                  >
                    {checking ? '...' : 'XÁC MINH'}
                  </button>
                </div>
                {checkError && (
                  <p className="text-xs mt-1.5 text-red-500 font-medium">✗ {checkError}</p>
                )}
              </div>
            </div>
          ) : (
            /* ─ BƯỚC 2: Đã xác minh, viết review ─ */
            <div className="space-y-3">
              {/* Hiển thị tên đã xác minh */}
              <div className="flex items-center gap-3 bg-green-50 border border-green-200 rounded-xl px-4 py-3">
                <div className="w-8 h-8 rounded-full bg-green-500 flex items-center justify-center flex-shrink-0">
                  <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div className="min-w-0">
                  <p className="text-xs text-green-600 font-semibold">Đã xác minh</p>
                  <p className="text-sm font-bold text-slate-800 truncate">{verifiedName}</p>
                </div>
                <button
                  onClick={() => { setVerifiedName(null); setPhone(''); setCheckError('') }}
                  className="ml-auto text-slate-400 hover:text-slate-600 transition text-xs underline flex-shrink-0"
                >
                  Đổi
                </button>
              </div>

              {/* Đánh giá sao */}
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Đánh giá sao
                </label>
                <div className="flex gap-1">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <button
                      key={star}
                      onClick={() => setRating(star)}
                      className={`text-2xl transition-transform hover:scale-110 ${
                        star <= rating ? 'text-amber-400' : 'text-slate-200'
                      }`}
                    >
                      ★
                    </button>
                  ))}
                </div>
              </div>

              {/* Nhận xét */}
              <div>
                <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-1">
                  Nhận xét <span className="text-red-500">*</span>
                </label>
                <textarea
                  rows={3}
                  placeholder="Chia sẻ trải nghiệm của bạn..."
                  className="w-full bg-white border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:border-slate-400 resize-none"
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                />
              </div>

              <button
                onClick={handleSubmitReview}
                disabled={!comment.trim() || submitting}
                className="w-full bg-slate-900 disabled:opacity-40 text-white text-sm font-bold py-3 rounded-xl hover:bg-slate-800 transition"
              >
                {submitting ? 'Đang gửi...' : 'GỬI ĐÁNH GIÁ'}
              </button>
            </div>
          )}
        </div>

        {/* ── DANH SÁCH REVIEW ── */}
        <div className="lg:col-span-2">
          <h3 className="text-lg font-bold text-slate-900 mb-5">
            Đánh giá từ khách hàng
            <span className="ml-2 text-sm font-normal text-slate-400">({reviews.length})</span>
          </h3>
          {reviews.length === 0 ? (
            <div className="text-sm text-slate-500 bg-slate-50 border border-slate-100 rounded-2xl p-8 text-center">
              <div className="text-3xl mb-2">💬</div>
              Xe này chưa có đánh giá nào. Hãy là người đầu tiên chia sẻ trải nghiệm!
            </div>
          ) : (
            <div className="space-y-4">
              {reviews.map((r) => (
                <div key={r.reviewId} className="bg-white border border-slate-100 p-5 rounded-2xl shadow-sm">
                  <div className="flex items-center justify-between mb-2">
                    <span className="font-bold text-slate-900 text-sm">{r.fullName}</span>
                    <span className="text-xs text-slate-400">
                      {new Date(r.createdAt).toLocaleDateString('vi-VN')}
                    </span>
                  </div>
                  <div className="flex text-sm mb-2">
                    {Array.from({ length: 5 }).map((_, i) => (
                      <span key={i} className={i < r.rating ? 'text-amber-400' : 'text-slate-200'}>★</span>
                    ))}
                  </div>
                  <p className="text-slate-600 text-sm leading-relaxed">{r.comment}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  )
}