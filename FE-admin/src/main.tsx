import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { AppProviders } from './app/providers/AppProviders'
import App from './App'
import { initThemeMode, startSystemThemeWatcher } from './app/theme/themeStore'
import { initAuth } from './app/auth/authStore'

// Apply theme before React mounts (avoid flash)
initThemeMode()
startSystemThemeWatcher()
initAuth()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AppProviders>
      <App />
    </AppProviders>
  </StrictMode>,
)
