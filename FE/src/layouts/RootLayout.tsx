import { Outlet } from 'react-router-dom'

import { AiChatPanel } from '../components/AiChatPanel'
import { FloatingContactDock } from '../components/FloatingContactDock'
import { Footer } from '../components/Footer'
import { Header } from '../components/Header'
import { Topbar } from '../components/Topbar'

export function RootLayout() {
  return (
    <div className="min-h-dvh">
      <Topbar />
      <Header />
      <Outlet />
      <Footer />
      <FloatingContactDock />
      <AiChatPanel />
    </div>
  )
}

