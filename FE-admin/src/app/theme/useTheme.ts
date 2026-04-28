import { useSyncExternalStore } from 'react'

import { resolveTheme, type ThemeMode } from './theme'
import { getResolvedTheme, getThemeMode, setThemeMode, subscribeTheme } from './themeStore'

export function useTheme() {
  const mode = useSyncExternalStore(subscribeTheme, getThemeMode, getThemeMode)
  const resolved = useSyncExternalStore(subscribeTheme, getResolvedTheme, getResolvedTheme)

  return {
    mode,
    resolved,
    setMode: (m: ThemeMode) => setThemeMode(m),
    setLight: () => setThemeMode('light'),
    setDark: () => setThemeMode('dark'),
    setSystem: () => setThemeMode('system'),
    toggle: () => setThemeMode(resolveTheme(mode) === 'dark' ? 'light' : 'dark'),
  }
}

