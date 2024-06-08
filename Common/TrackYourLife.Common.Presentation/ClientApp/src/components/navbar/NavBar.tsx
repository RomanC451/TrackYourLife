import { MenuIcon } from "lucide-react";
import React from "react";
import { ModeToggle } from "~/chadcn/mode-toggle";
import { Button } from "~/chadcn/ui/button";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import { useSideBarContext } from "~/contexts/SideBarContextProvider";

const NavBar: React.FC = (): React.JSX.Element => {
  const { screenSize } = useAppGeneralStateContext();

  const { toggleSideBarOpened: togleSideBarOpened } = useSideBarContext();

  return (
    <section className=" flex w-full justify-center">
      <nav className="mx-8 w-[100%] py-3">
        <ul className="flex justify-between gap-10">
          <li>
            <Button
              variant="ghost"
              size="icon"
              // style={{
              //   visibility:
              //     screenSize.width > screensEnum.lg ? "hidden" : "visible",
              // }}
              onClick={togleSideBarOpened}
            >
              <MenuIcon size={24} />
            </Button>
          </li>
          <li>
            <ModeToggle />
          </li>
        </ul>
      </nav>
    </section>
  );
};

export default NavBar;
