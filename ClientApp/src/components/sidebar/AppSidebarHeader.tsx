import Logo from "@/assets/logo.svg?react";
import { cn } from "@/lib/utils";

import { SidebarHeader, useSidebar } from "../ui/sidebar";

function AppSidebarHeader() {
  const { isMobile, open, isHovered } = useSidebar();
  return (
    <SidebarHeader className="mt-4 flex flex-row gap-0 overflow-hidden">
      <div>
        <Logo className="size-8" />
      </div>

      <span
        className={cn(
          "-ml-2 mt-2 text-nowrap text-[#861bc0] transition-opacity duration-500",
          isMobile || open || isHovered ? "opacity-100" : "opacity-0",
        )}
      >
        rack your life
      </span>
    </SidebarHeader>
  );
}

export default AppSidebarHeader;
