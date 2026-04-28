export type ThemeMode = 'light' | 'dark' | 'system'

export const THEME_STORAGE_KEY = 'admin_theme'

export function resolveTheme(mode: ThemeMode): 'light' | 'dark' {
  if (mode === 'system') {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
  }
  return mode
}

export function applyThemeClass(resolved: 'light' | 'dark') {
  const root = document.documentElement
  if (resolved === 'dark') root.classList.add('dark')
  else root.classList.remove('dark')
}

