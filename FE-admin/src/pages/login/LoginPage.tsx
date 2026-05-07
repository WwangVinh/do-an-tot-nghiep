import { useEffect, useMemo, useState } from 'react'
import toast from 'react-hot-toast'
import { useLocation, useNavigate } from 'react-router-dom'
import { Eye, EyeOff, Lock, User } from 'lucide-react'

import { http } from '../../services/http/http'
import { setAuth } from '../../app/auth/authStore'
import { useAuth } from '../../app/auth/useAuth'

type LoginResponse = {
  success?: boolean
  message?: string
  token?: string
  userId?: number | null
  username?: string | null
  role?: string | null
  showroomId?: number | null
}

function getErrorMessage(err: unknown): string {
  if (typeof err === 'string') return err
  if (err && typeof err === 'object') {
    const anyErr = err as { message?: unknown; response?: { data?: unknown } }
    const data = anyErr.response?.data
    if (data && typeof data === 'object' && 'message' in data) {
      const msg = (data as { message?: unknown }).message
      if (typeof msg === 'string' && msg.trim()) return msg
    }
    if (typeof anyErr.message === 'string' && anyErr.message.trim()) return anyErr.message
  }
  return 'Đăng nhập thất bại'
}

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const auth = useAuth()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const redirectTo = useMemo(() => {
    const st = location.state as unknown
    if (st && typeof st === 'object' && 'from' in st) {
      const from = (st as { from?: unknown }).from
      if (typeof from === 'string' && from.startsWith('/')) return from
    }
    return '/'
  }, [location.state])

  useEffect(() => {
    // Nếu đã có token thì đi thẳng vào dashboard
    if (auth.isAuthenticated) navigate(redirectTo, { replace: true })
  }, [auth.isAuthenticated, navigate, redirectTo])

  async function onSubmit() {
    if (submitting) return
    const u = username.trim()
    const p = password
    if (!u || !p) {
      toast.error('Vui lòng nhập tên đăng nhập và mật khẩu')
      return
    }

    setSubmitting(true)
    try {
      const res = await http.post<LoginResponse>('/api/Auth/login', {
        Username: u,
        Password: p,
      })

      const data = res.data
      const ok = data?.success ?? Boolean(data?.token)
      if (!ok || !data?.token) {
        toast.error(data?.message ?? 'Đăng nhập thất bại')
        return
      }

      setAuth({
        token: data.token,
        user: {
          userId: data.userId ?? null,
          username: data.username ?? u,
          role: data.role ?? null,
          showroomId: data.showroomId ?? null,
        },
      })

      toast.success(data?.message ?? 'Đăng nhập thành công')
      navigate(redirectTo, { replace: true })
    } catch (e: unknown) {
      toast.error(getErrorMessage(e))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="relative min-h-screen overflow-hidden bg-zinc-950 text-zinc-50">
      <div className="pointer-events-none absolute inset-0">
        <div className="absolute -left-24 -top-24 h-72 w-72 rounded-full bg-indigo-600/20 blur-3xl" />
        <div className="absolute -bottom-24 -right-24 h-72 w-72 rounded-full bg-purple-600/20 blur-3xl" />
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,rgba(255,255,255,0.06),transparent_45%)]" />
      </div>

      <div className="relative mx-auto flex min-h-screen w-full max-w-6xl items-center justify-center px-4 py-10">
        <div className="grid w-full max-w-4xl grid-cols-1 overflow-hidden rounded-2xl border border-zinc-800 bg-zinc-950/70 shadow-2xl ring-1 ring-white/5 md:grid-cols-2">
          <div className="hidden border-r border-zinc-800 bg-gradient-to-br from-indigo-600/10 via-transparent to-purple-600/10 p-8 md:block">
            <div className="flex items-center gap-3">
              <div className="h-11 w-11 rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-500" />
              <div className="leading-tight">
                <div className="text-lg font-semibold">CMC AUTOMOTIVE</div>
                <div className="text-sm text-zinc-300">Admin Panel</div>
              </div>
            </div>

            <div className="mt-10 space-y-3">
              <div className="text-2xl font-semibold leading-snug">
                Quản trị hệ thống
                <div className="text-zinc-300">đăng nhập để tiếp tục</div>
              </div>
              <p className="text-sm text-zinc-400">
                Vui lòng dùng tài khoản được cấp quyền (Admin/Manager/Staff). Nếu đăng nhập thất bại, kiểm tra lại tên đăng nhập và mật khẩu.
              </p>
            </div>
          </div>

          <div className="p-6 md:p-8">
            <div className="flex items-center justify-between gap-4">
              <div>
                <h1 className="text-xl font-semibold md:text-2xl">Đăng nhập</h1>
                <p className="mt-1 text-sm text-zinc-300">Nhập thông tin để vào trang quản trị.</p>
              </div>
              <div className="hidden md:block">
                <div className="rounded-full border border-zinc-800 bg-zinc-900/40 px-3 py-1 text-xs text-zinc-300">
                  {redirectTo === '/' ? 'Dashboard' : `→ ${redirectTo}`}
                </div>
              </div>
            </div>

            <div className="mt-6 grid gap-4">
              <div>
                <label className="text-sm font-medium text-zinc-200">Tên đăng nhập</label>
                <div className="relative mt-2">
                  <User size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500" />
                  <input
                    autoFocus
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    placeholder="Nhập tên đăng nhập"
                    className="h-11 w-full rounded-lg border border-zinc-800 bg-zinc-900/60 pl-9 pr-3 text-sm text-zinc-100 placeholder:text-zinc-500 outline-none focus:bg-zinc-900 focus:ring-2 focus:ring-indigo-500/40"
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') onSubmit()
                    }}
                  />
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-zinc-200">Mật khẩu</label>
                <div className="relative mt-2">
                  <Lock size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500" />
                  <input
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="Nhập mật khẩu"
                    type={showPassword ? 'text' : 'password'}
                    className="h-11 w-full rounded-lg border border-zinc-800 bg-zinc-900/60 pl-9 pr-10 text-sm text-zinc-100 placeholder:text-zinc-500 outline-none focus:bg-zinc-900 focus:ring-2 focus:ring-indigo-500/40"
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') onSubmit()
                    }}
                  />
                  <button
                    type="button"
                    className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md p-2 text-zinc-400 hover:bg-zinc-800/60 hover:text-zinc-200"
                    aria-label={showPassword ? 'Hide password' : 'Show password'}
                    onClick={() => setShowPassword((v) => !v)}
                  >
                    {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                  </button>
                </div>
              </div>

              <button
                type="button"
                disabled={submitting}
                className="mt-2 inline-flex h-11 w-full items-center justify-center gap-2 rounded-lg bg-white text-sm font-semibold text-zinc-900 hover:bg-zinc-100 disabled:opacity-60"
                onClick={onSubmit}
              >
                {submitting ? 'Đang đăng nhập...' : 'Đăng nhập'}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

