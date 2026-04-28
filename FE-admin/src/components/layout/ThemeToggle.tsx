import { Laptop, Moon, Sun } from 'lucide-react'

import { useTheme } from '../../app/theme/useTheme'

export function ThemeToggle() {
  const { mode, setLight, setDark, setSystem } = useTheme()

  const base =
    'inline-flex items-center gap-1 rounded-md border px-2 py-1 text-xs font-medium transition'
  const on = 'border-slate-200 bg-white text-slate-900 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-100'
  const off =
    'border-transparent bg-transparent text-slate-600 hover:bg-slate-100 dark:text-zinc-300 dark:hover:bg-zinc-900'

  return (
    <div className="flex items-center rounded-md border border-slate-200 bg-white p-1 dark:border-zinc-800 dark:bg-zinc-950">
      <button type="button" className={[base, mode === 'light' ? on : off].join(' ')} onClick={setLight} title="Light">
        <Sun size={14} />
        Light
      </button>
      <button type="button" className={[base, mode === 'dark' ? on : off].join(' ')} onClick={setDark} title="Dark">
        <Moon size={14} />
        Dark
      </button>
      <button type="button" className={[base, mode === 'system' ? on : off].join(' ')} onClick={setSystem} title="System">
        <Laptop size={14} />
        Auto
      </button>
    </div>
  )
}

