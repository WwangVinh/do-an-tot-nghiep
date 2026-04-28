<!-- AGENTS.md: Guidance for AI coding agents working on this repository -->

# AGENTS for car-web

Purpose: Help AI coding agents be immediately productive in this Vite + React + TypeScript frontend.

Quick start (commands):

- `npm run dev` — start dev server (Vite)
- `npm run build` — TypeScript build then Vite build
- `npm run preview` — preview production build
- `npm run lint` — run ESLint

Project overview:

- Tech: Vite, React 19, TypeScript, Tailwind CSS
- Source: `src/`
- Key directories:
  - `src/components/` — reusable UI components
  - `src/pages/` — route pages and page-level components
  - `src/services/` — http clients and query client
  - `src/lib/` — small runtime helpers and env

Conventions for agents:

- Prefer adding new UI components under `src/components/<Feature>/` as a small folder with an index `tsx` export.
- Use TypeScript `.tsx` files and Tailwind utility classes for styling.
- Follow existing patterns (hooks-based data fetching with `@tanstack/react-query`, form handling with `react-hook-form`).
- When changing routes, update `src/router/routes.tsx` and `src/router/AppRouter.tsx` as needed.

Development notes / gotchas:

- Build/test commands are in `package.json` (dev, build, preview, lint).
- The repo uses absolute and relative imports consistent with `tsconfig.json`; prefer local `src/` paths.

Focus: handling requests about "tab"

When the user asks about `tab` (the provided focus argument):

1. Search the codebase for occurrences of "tab" (component names, class names, aria attributes). Look in `src/components/` and `src/pages/`.
2. If a `Tabs` or `Tab` component already exists, reuse or extend it; prefer composing smaller tab-panel and tab-list subcomponents.
3. If not present, implement a new accessible Tabs component under `src/components/Tabs/`:
   - Export a `Tabs.tsx` (root) and smaller `TabList.tsx`, `TabPanel.tsx` files as needed.
   - Use WAI-ARIA roles: `tablist`, `tab`, `tabpanel`. Manage focus and keyboard navigation (Arrow keys, Home/End).
   - Use Tailwind classes and TypeScript types for props. Keep behavior logic in a small hook (e.g., `useTabs`).
4. Add examples in a relevant page (e.g., a demo story or a temporary route under `src/pages/` for manual QA).

Links:

- Repository README: [README.md](README.md)

If you'd like, I can now:

- create the `src/components/Tabs/` scaffold for you
- or add a small skill that automates the common `tab` implementation steps
