---
name: tab
description: Small skill describing how to find, create, or extend accessible Tabs UI in this repo
---

When invoked with the focus `tab`, follow these steps:

1. Search the repository for `tab`, `Tabs`, `TabList`, `TabPanel` occurrences.
2. If an existing Tabs implementation is found, prefer extending or reusing it. Document where it is and any public props.
3. If none exists, scaffold `src/components/Tabs/` with:
   - `Tabs.tsx` — root component and context provider
   - `TabList.tsx` — renders the tab buttons with `role="tablist"`
   - `TabPanel.tsx` — renders panels with `role="tabpanel"`
   - `useTabs.ts` — small hook for active index, keyboard navigation, and ARIA attributes
4. Ensure accessibility:
   - Use WAI-ARIA roles `tablist`, `tab`, `tabpanel`.
   - Implement keyboard navigation (Left/Right/Up/Down, Home, End) and focus management.
5. Styling: use Tailwind utility classes consistent with existing components.
6. Add a demo usage to a relevant page under `src/pages/` for manual verification.

Commands to verify work locally:

- Start dev server: `npm run dev`
- Build: `npm run build`

If user asks to implement the component, create the scaffold files and a small example route.
