import { createFileRoute } from '@tanstack/react-router'

import SideBarPageLayout from '@/layouts/pageLayouts/SidebarPageLayout'

export const Route = createFileRoute('/_authenticated/_sidebarPageLayout')({
  component: SideBarPageLayout,
})
