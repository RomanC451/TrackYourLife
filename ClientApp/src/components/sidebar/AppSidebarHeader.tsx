import Logo from "@/assets/logo.svg?react";
import { cn } from "@/lib/utils";

import { SidebarHeader, useSidebar } from "../ui/sidebar";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { motion } from "framer-motion";

function AppSidebarHeader() {
  const { isMobile, open, isHovered } = useSidebar();
  const { isPro } = useAuthenticationContext();

  return (
    <>
      <SidebarHeader className="mt-4 flex flex-row gap-0 overflow-hidden">
        <div className="relative flex">
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
          {isPro &&
            (
              <span className={cn("absolute rounded bg-[#861bc0] px-1 text-[9px] font-semibold leading-tight text-white -top-0.5 -right-[20px] duration-500 transition-opacity",
                isMobile || open || isHovered ? "opacity-100" : "opacity-0"
              )
              }
              >
                Pro
              </span>
            )}


        </div>

      </SidebarHeader>
      {isPro && (
        <motion.p
          className="overflow-hidden rounded bg-[#861bc0] px-1 text-[9px] font-semibold leading-tight text-white ml-3 text-center align-middle inline-block"
          initial={{
            height: 0,
            marginBottom: 0,
            opacity: 0,
            width: 23,
          }}
          animate={{
            height: !isMobile && !open && !isHovered ? 11 : 0,
            marginBottom: !isMobile && !open && !isHovered ? 12 : 0,
            opacity: !isMobile && !open && !isHovered ? 1 : 0,
          }}
          transition={{ duration: 0.25, ease: "easeInOut" }}
        >
          Pro
        </motion.p>
      )}
    </>
  );
}

export default AppSidebarHeader;
