import { Link, useRouterState } from "@tanstack/react-router";
import { MenuIcon } from "lucide-react";
import React from "react";
import { ModeToggle } from "~/chadcn/mode-toggle";
import { Button } from "~/chadcn/ui/button";
import { Tabs, TabsList, TabsTrigger } from "~/chadcn/ui/tabs";
import { useSideBarContext } from "~/contexts/SideBarContextProvider";

const NavBar: React.FC = (): React.JSX.Element => {
  const { toggleSideBarOpened } = useSideBarContext();

  const router = useRouterState();

  const fullPathName = router.location.pathname
    .split("/")
    .filter((p) => p !== "");

  const pathName = fullPathName[fullPathName.length - 1];

  return (
    <section className=" flex w-full justify-center">
      <nav className="mx-8 w-[100%] py-3">
        <ul className="flex justify-between gap-10">
          <div className="flex  gap-10">
            <li>
              <Button variant="ghost" size="icon" onClick={toggleSideBarOpened}>
                <MenuIcon size={24} />
              </Button>
            </li>
            <li>
              <Tabs defaultValue={pathName} className="flex flex-grow flex-col">
                <div>
                  <TabsList className="ite flex-grow-0">
                    <Link to="/nutrition">
                      <TabsTrigger value="health">Main</TabsTrigger>
                    </Link>
                    <Link to="/nutrition/foodDiary" preload={"intent"}>
                      <TabsTrigger value="foodDiary">Diary</TabsTrigger>
                    </Link>
                    <Link to="/nutrition/recipes" preload={"intent"}>
                      <TabsTrigger value="recipes">Recipes</TabsTrigger>
                    </Link>
                  </TabsList>
                </div>
              </Tabs>
            </li>
          </div>
          <li>
            <ModeToggle />
          </li>
        </ul>
      </nav>
    </section>
  );
};

export default NavBar;
