import { createContext, useContext, useMemo, type ReactNode } from "react";

import { useDateRange } from "../hooks/useDateRange";

type OverviewDateRangeValue = ReturnType<typeof useDateRange>;

const OverviewDateRangeContext = createContext<OverviewDateRangeValue | null>(
  null,
);

export function OverviewDateRangeProvider({ children }: { children: ReactNode }) {
  const dateRange = useDateRange();

  const value = useMemo(
    () => dateRange,
    [dateRange],
  );

  return (
    <OverviewDateRangeContext.Provider value={value}>
      {children}
    </OverviewDateRangeContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useOverviewDateRange(): OverviewDateRangeValue {
  const context = useContext(OverviewDateRangeContext);
  if (!context) {
    throw new Error(
      "useOverviewDateRange must be used within OverviewDateRangeProvider",
    );
  }
  return context;
}
