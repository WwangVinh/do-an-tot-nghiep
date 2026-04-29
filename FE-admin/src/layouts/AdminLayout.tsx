import { Outlet, useLocation, useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { ChevronLeft, ChevronRight } from 'lucide-react'

import { AdminHeader } from '../components/layout/AdminHeader'
import { AdminSidebar } from '../components/layout/AdminSidebar'
import { useAuth } from '../app/auth/useAuth'

const SIDEBAR_COLLAPSED_KEY = 'admin_sidebar_collapsed'

export function AdminLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [collapsed, setCollapsed] = useState(() => {
    try {
      return localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === 'true'
    } catch {
      return false
    }
  })

  const auth = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  useEffect(() => {
    if (!auth.isAuthenticated) {
      navigate('/login', {
        replace: true,
        state: { from: location.pathname + location.search },
      })
    }
  }, [auth.isAuthenticated, location.pathname, location.search, navigate])

  function toggleCollapsed() {
    setCollapsed((v) => {
      const next = !v
      try { localStorage.setItem(SIDEBAR_COLLAPSED_KEY, String(next)) } catch {}
      return next
    })
  }

  if (!auth.isAuthenticated) return null

  return (
    <div className="min-h-screen">
      <AdminHeader onToggleSidebar={() => setSidebarOpen((v) => !v)} />

      <div className="w-full px-4 py-6">
        <div className={[
          'grid items-start gap-6 transition-all duration-300',
          collapsed ? 'md:grid-cols-[64px_1fr]' : 'md:grid-cols-[280px_1fr]',
        ].join(' ')}>

          {/* Sidebar wrapper — nút toggle nằm ngoài overflow-hidden */}
          <aside className="relative hidden md:block md:sticky md:top-[6.5rem] md:self-start">
            {/* Nút toggle — nằm ngoài container sidebar, không bị cắt */}
            <button
              type="button"
              onClick={toggleCollapsed}
              title={collapsed ? 'Mở rộng menu' : 'Thu gọn menu'}
              className={[
                'absolute top-5 -right-3 z-20',
                'flex h-6 w-6 items-center justify-center rounded-full',
                'border border-slate-200 bg-white shadow-sm',
                'text-slate-400 transition-colors hover:bg-indigo-50 hover:text-indigo-600 hover:border-indigo-200',
                'dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-400 dark:hover:bg-zinc-800 dark:hover:text-indigo-300',
              ].join(' ')}
            >
              {collapsed ? <ChevronRight size={12} /> : <ChevronLeft size={12} />}
            </button>

            {/* Sidebar container */}
            <div className={[
              'h-[calc(100vh-6.5rem)] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-md ring-1 ring-slate-200/50 transition-all duration-300',
              'dark:border-zinc-800 dark:bg-zinc-950 dark:ring-zinc-800/60',
              collapsed ? 'p-1' : 'p-2',
            ].join(' ')}>
              <div className="h-full overflow-y-auto rounded-xl bg-slate-50/60 dark:bg-zinc-950/60">
                <AdminSidebar collapsed={collapsed} />
              </div>
            </div>
          </aside>

          {/* Mobile drawer */}
          {sidebarOpen && (
            <div className="fixed inset-0 z-40 md:hidden">
              <button
                type="button"
                className="absolute inset-0 bg-black/30"
                aria-label="Close sidebar overlay"
                onClick={() => setSidebarOpen(false)}
              />
              <div className="absolute left-0 top-0 h-full w-[280px] bg-white p-2 shadow-xl dark:bg-zinc-950">
                <div className="h-full rounded-2xl border border-slate-200 bg-slate-50/60 dark:border-zinc-800 dark:bg-zinc-950/60">
                  <AdminSidebar onNavigate={() => setSidebarOpen(false)} collapsed={false} />
                </div>
              </div>
            </div>
          )}

          <main>
            <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm dark:border-zinc-800 dark:bg-zinc-950 md:p-6">
              <Outlet />
            </div>
          </main>
        </div>
      </div>
    </div>
  )
}