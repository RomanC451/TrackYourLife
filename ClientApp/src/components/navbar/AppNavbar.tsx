import { MenuIcon } from "lucide-react";

import { Button } from "@/components/ui/button";

import { useSidebar } from "../ui/sidebar";
import RootNavbar from "./RootNavbar";

function AppNavbar() {
  const { toggleSidebar } = useSidebar();

  return (
    <RootNavbar themeToggle>
      <li>
        <Button variant="ghost" size="icon" onClick={toggleSidebar}>
          <MenuIcon size={24} />
        </Button>
      </li>
    </RootNavbar>
  );
}

export default AppNavbar;
