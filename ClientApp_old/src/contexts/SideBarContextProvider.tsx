import {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState,
} from "react";
import { useLocalStorage } from "usehooks-ts";
import { screensEnum } from "~/constants/tailwindSizes";
import { Assert } from "~/utils";
import { useAppGeneralStateContext } from "./AppGeneralContextProvider";

interface ContextInterface {
  sideBarOpened: boolean;
  setSidebarOpened: (value: boolean) => void;
  sideBarPosition: "static" | "absolute";
  deferredSideBarPosition: "static" | "absolute";
  toggleSideBarOpened: () => void;
  isAnimating: boolean;
  setIsAnimating: (value: boolean) => void;
}

const SideBarContext = createContext<ContextInterface>({} as ContextInterface);

export type SideBarPositions = "static" | "absolute";

export const SideBarContextProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const { screenSize } = useAppGeneralStateContext();

  const [sideBarOpened, setSidebarOpened] = useLocalStorage(
    "sideBarOpened",
    true,
  );

  const [sideBarPosition, setSideBarPosition] = useState<SideBarPositions>(
    screenSize.width <= screensEnum.lg ? "absolute" : "static",
  );

  const [deferredSideBarPosition, setDeferredSideBarPosition] =
    useState<SideBarPositions>(
      screenSize.width <= screensEnum.lg ? "absolute" : "static",
    );

  const [isAnimating, setIsAnimating] = useState(false);

  useEffect(() => {
    if (screenSize.width <= screensEnum.lg) {
      setSideBarPosition("absolute");
      setSidebarOpened(false);
    } else {
      setSideBarPosition("static");
    }
  }, [screenSize.width]);

  useEffect(() => {
    if (!isAnimating && sideBarPosition === "absolute") {
      setDeferredSideBarPosition(sideBarPosition);
    } else if (isAnimating && sideBarPosition === "static") {
      setDeferredSideBarPosition(sideBarPosition);
    }
  }, [isAnimating]);

  const toggleSideBarOpened = () => setSidebarOpened(!sideBarOpened);

  return (
    <SideBarContext.Provider
      value={{
        sideBarOpened,
        setSidebarOpened,
        sideBarPosition,
        deferredSideBarPosition,
        toggleSideBarOpened,
        isAnimating,
        setIsAnimating,
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
