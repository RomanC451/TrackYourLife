import { ComponentProps } from "react";

import { Collapsible } from "../ui/collapsible";

function AppSidebarCollapsible(props: ComponentProps<typeof Collapsible>) {
  return <Collapsible {...props} />;
}

export default AppSidebarCollapsible;
