import { createContext, ReactNode, useContext, useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import { sideBarWidths } from "~/components/sidebar/SideBar";
import { Assert } from "~/utils";

interface ContextInterface {
  sideBarOpened: boolean;
  setSidebarOpened: (value: boolean) => void;
  sideBarMode: "normal" | "absolute";
  setSideBarMode: React.Dispatch<React.SetStateAction<"normal" | "absolute">>;
  sideBarWidth: number;
  setSideBarWidth: React.Dispatch<React.SetStateAction<number>>;
}

const SideBarContext = createContext<ContextInterface>({} as ContextInterface);

export const SideBarContextProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const [sideBarMode, setSideBarMode] = useState<"normal" | "absolute">(
    "normal",
  );
  const [sideBarOpened, setSidebarOpened] = useLocalStorage(
    "sideBarOpened",
    true,
  );

  const [sideBarWidth, setSideBarWidth] = useState<number>(
    sideBarWidths[sideBarOpened ? "opened" : "closed"],
  );

  return (
    <SideBarContext.Provider
      value={{
        sideBarOpened,
        setSidebarOpened,
        sideBarMode,
        setSideBarMode,
        sideBarWidth,
        setSideBarWidth,
      }}
    >
      {children}
    </SideBarContext.Provider>
  );
};

export const useSideBarContext = () => {
  const context = useContext(SideBarContext);
  Assert.isNotUndefined(
    context,
    "useSideBarContext must be used within a SideBarContextProvider!",
  );
  Assert.isNotEmptyObject(
    context,
    "useSideBarContext must be used within a SideBarContextProvider!",
  );
  return context;
};
