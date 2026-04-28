import { Outlet, useLocation, useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'

import { AdminHeader } from '../components/layout/AdminHeader'
import { AdminSidebar } from '../components/layout/AdminSidebar'
import { useAuth } from '../app/auth/useAuth'

export function AdminLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
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

  if (!auth.isAuthenticated) return null

  return (
    <div className="min-h-screen">
      <AdminHeader onToggleSidebar={() => setSidebarOpen((v) => !v)} />

      <div className="w-full px-4 py-6">
        <div className="grid grid-cols-1 items-start gap-6 md:grid-cols-[280px_1fr]">
          <aside className="md:sticky md:top-[6.5rem] md:self-start">
            {/* Desktop sidebar */}
            <div className="hidden md:block">
              <div className="h-[calc(100vh-6.5rem)] overflow-hidden rounded-2xl border border-slate-200 bg-white p-2 shadow-md ring-1 ring-slate-200/50 dark:border-zinc-800 dark:bg-zinc-950 dark:ring-zinc-800/60">
                <div className="h-full overflow-y-auto rounded-xl bg-slate-50/60 dark:bg-zinc-950/60">
                  <AdminSidebar />
                </div>
              </div>
            </div>

            {/* Mobile drawer */}
            {sidebarOpen ? (
              <div className="fixed inset-0 z-40 md:hidden">
                <button
                  type="button"
                  className="absolute inset-0 bg-black/30"
                  aria-label="Close sidebar overlay"
                  onClick={() => setSidebarOpen(false)}
                />
                <div className="absolute left-0 top-0 h-full w-[280px] bg-white p-2 shadow-xl dark:bg-zinc-950">
                  <div className="h-full rounded-2xl border border-slate-200 bg-slate-50/60 dark:border-zinc-800 dark:bg-zinc-950/60">
                    <AdminSidebar onNavigate={() => setSidebarOpen(false)} />
                  </div>
                </div>
              </div>
            ) : null}
          </aside>

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

