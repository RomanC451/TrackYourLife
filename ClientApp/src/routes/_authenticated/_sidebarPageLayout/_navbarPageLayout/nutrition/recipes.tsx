import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes',
)({
  component: RouteComponent,
})

function RouteComponent() {
  return 'Hello /_sidebarPageLayout/_navbarPageLayout/nutrition/recipes!'
}
