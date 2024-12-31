import { createFileRoute } from '@tanstack/react-router'

import NavbarPageLayout from '@/layouts/pageLayouts/NavbarPageLayout'

export const Route = createFileRoute(
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout',
)({
  component: NavbarPageLayout,
})
