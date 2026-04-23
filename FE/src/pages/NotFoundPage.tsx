import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <main className="mx-auto w-full max-w-5xl px-6 py-10">
      <h1 className="text-2xl font-semibold tracking-tight">404</h1>
      <p className="mt-2 text-sm text-zinc-300">Page not found.</p>
      <Link className="mt-6 inline-block text-sm underline" to="/">
        Go home
      </Link>
    </main>
  )
}

