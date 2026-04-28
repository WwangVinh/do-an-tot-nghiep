import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <div className="py-10">
      <h1 className="text-xl font-semibold">Không tìm thấy trang</h1>
      <p className="mt-1 text-sm text-zinc-300">Đường dẫn không hợp lệ.</p>
      <Link to="/" className="mt-4 inline-block rounded-md bg-white px-3 py-2 text-sm font-medium text-zinc-900">
        Về Dashboard
      </Link>
    </div>
  )
}

