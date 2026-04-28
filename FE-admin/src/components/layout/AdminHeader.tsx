import { Bell, Languages, Menu, Search, Settings } from 'lucide-react'

import { IconButton } from './IconButton'
import { ThemeToggle } from './ThemeToggle'

export function AdminHeader({
  onToggleSidebar,
}: {
  onToggleSidebar: () => void
}) {
  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/80 backdrop-blur dark:border-zinc-800 dark:bg-zinc-950/80">
      <div className="w-full px-4 py-3">
        <div className="flex items-center gap-3 md:grid md:grid-cols-[280px_1fr] md:gap-6">
          <div className="flex items-center gap-3">
            <IconButton className="md:hidden" onClick={onToggleSidebar} aria-label="Toggle sidebar">
              <Menu size={18} />
            </IconButton>

            <div className="flex items-center gap-2">
              <div className="h-9 w-9 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-500" />
              <div className="leading-tight">
                <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Tên công ty</div>
                <div className="text-xs text-slate-500 dark:text-zinc-400">Admin Panel</div>
              </div>
            </div>
          </div>

          <div className="flex flex-1 items-center gap-3">
            <div className="relative w-full max-w-2xl">
              <Search size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                placeholder="Search"
                className="h-10 w-full rounded-lg border border-slate-200 bg-slate-50 pl-9 pr-3 text-sm text-slate-900 placeholder:text-slate-400 outline-none focus:bg-white focus:ring-2 focus:ring-slate-200 dark:border-zinc-800 dark:bg-zinc-900/40 dark:text-zinc-100 dark:placeholder:text-zinc-500 dark:focus:bg-zinc-950 dark:focus:ring-zinc-800"
              />
            </div>

            <div className="ml-auto flex items-center gap-2">
              <div className="hidden md:block">
                <ThemeToggle />
              </div>

              <IconButton aria-label="Language">
                <Languages size={18} />
              </IconButton>
              <IconButton aria-label="Notifications">
                <Bell size={18} />
              </IconButton>
              <IconButton aria-label="Settings">
                <Settings size={18} />
              </IconButton>

              <button
                type="button"
                className="ml-1 inline-flex items-center gap-2 rounded-full border border-slate-200 bg-white px-2 py-1.5 text-sm text-slate-700 hover:bg-slate-100 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900"
              >
                <span className="inline-flex h-7 w-7 items-center justify-center rounded-full bg-gradient-to-br from-indigo-500 to-purple-500 text-xs font-semibold text-white">
                  A
                </span>
                <span className="hidden pr-1 md:block">Admin</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="px-4 pb-3 md:hidden">
        <ThemeToggle />
      </div>
    </header>
  )
}

