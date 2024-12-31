import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/test',
)({
  component: RouteComponent,
})

function RouteComponent() {
  return <div className="h-[120vh]">Hello /_sideAndNavBarsPageLayout/test!</div>
}
