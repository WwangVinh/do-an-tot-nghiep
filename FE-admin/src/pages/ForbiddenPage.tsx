import { Link } from 'react-router-dom'

export function ForbiddenPage() {
  return (
    <div className="mx-auto max-w-2xl py-10">
      <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-950">
        <div className="text-sm font-semibold text-rose-600 dark:text-rose-300">403</div>
        <h1 className="mt-2 text-2xl font-semibold text-slate-900 dark:text-zinc-50">Bạn không có quyền truy cập</h1>
        <p className="mt-2 text-sm text-slate-600 dark:text-zinc-300">
          Tài khoản hiện tại không được cấp quyền cho trang này. Nếu cần, vui lòng liên hệ Admin để được cấp quyền.
        </p>

        <div className="mt-5 flex flex-wrap gap-2">
          <Link
            to="/"
            className="inline-flex h-10 items-center justify-center rounded-xl bg-slate-900 px-4 text-sm font-semibold text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
          >
            Về Dashboard
          </Link>
          <Link
            to="/login"
            className="inline-flex h-10 items-center justify-center rounded-xl border border-slate-200 bg-white px-4 text-sm font-medium text-slate-700 hover:bg-slate-50 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
          >
            Đăng nhập lại
          </Link>
        </div>
      </div>
    </div>
  )
}

