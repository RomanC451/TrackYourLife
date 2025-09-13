import { createContext, useContext, useMemo, useState } from "react";
import { ToOptions } from "@tanstack/react-router";

import Assert from "@/lib/assert";
import { FoodDto } from "@/services/openapi";

type FoodSearchContextType = {
  AddFoodButtonComponent: React.ComponentType<{
    food: FoodDto;
    className?: string;
  }>;
  setAddFoodButtonComponent: React.Dispatch<
    React.SetStateAction<
      React.ComponentType<{
        food: FoodDto;
        className?: string;
      }>
    >
  >;
  onSelectedFoodToOptions: ToOptions;
  setOnSelectedFoodToOptions: (onSelectedFoodToOptions: ToOptions) => void;
};

const FoodSearchContext = createContext<FoodSearchContextType>({
  AddFoodButtonComponent: () => <></>,
  setAddFoodButtonComponent: () => {},
  onSelectedFoodToOptions: {},
  setOnSelectedFoodToOptions: () => {},
});

export function FoodSearchContextProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [AddFoodButtonComponent, setAddFoodButtonComponent] = useState<
    React.ComponentType<{ food: FoodDto; className?: string }>
  >(() => null);

  const [onSelectedFoodToOptions, setOnSelectedFoodToOptions] =
    useState<ToOptions>({});

  const value = useMemo(
    () => ({
      AddFoodButtonComponent,
      setAddFoodButtonComponent,
      onSelectedFoodToOptions,
      setOnSelectedFoodToOptions,
    }),
    [AddFoodButtonComponent, onSelectedFoodToOptions],
  );

  return (
    <FoodSearchContext.Provider value={value}>
      {children}
    </FoodSearchContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useFoodSearchContext() {
  const context = useContext(FoodSearchContext);

  Assert.contextIsDefined(context, "FoodSearchContext");

  return context;
}
