import { QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from 'react-hot-toast'

import type { PropsWithChildren } from 'react'
import { queryClient } from '../../services/http/queryClient'

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <Toaster position="top-right" />
    </QueryClientProvider>
  )
}

