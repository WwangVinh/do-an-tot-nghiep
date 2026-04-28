import { applyThemeClass, resolveTheme, THEME_STORAGE_KEY, type ThemeMode } from './theme'

type Listener = () => void

let mode: ThemeMode = 'light'
const listeners = new Set<Listener>()

function notify() {
  for (const l of listeners) l()
}

export function getThemeMode(): ThemeMode {
  return mode
}

export function getResolvedTheme(): 'light' | 'dark' {
  return resolveTheme(mode)
}

export function setThemeMode(next: ThemeMode) {
  mode = next
  try {
    localStorage.setItem(THEME_STORAGE_KEY, next)
  } catch {
    // ignore
  }
  applyThemeClass(getResolvedTheme())
  notify()
}

export function initThemeMode() {
  let initial: ThemeMode = 'light'
  try {
    const raw = localStorage.getItem(THEME_STORAGE_KEY)
    if (raw === 'light' || raw === 'dark' || raw === 'system') initial = raw
  } catch {
    // ignore
  }
  mode = initial
  applyThemeClass(getResolvedTheme())
  notify()
}

export function subscribeTheme(listener: Listener) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

export function startSystemThemeWatcher() {
  if (!window.matchMedia) return () => {}
  const mq = window.matchMedia('(prefers-color-scheme: dark)')
  const handler = () => {
    if (mode !== 'system') return
    applyThemeClass(resolveTheme('system'))
    notify()
  }

  if ('addEventListener' in mq) mq.addEventListener('change', handler)
  else (mq as unknown as { addListener: (fn: () => void) => void }).addListener(handler)

  return () => {
    if ('removeEventListener' in mq) mq.removeEventListener('change', handler)
    else (mq as unknown as { removeListener: (fn: () => void) => void }).removeListener(handler)
  }
}

