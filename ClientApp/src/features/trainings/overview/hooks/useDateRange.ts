import { useMemo, useState } from "react";
import { subDays } from "date-fns";
import { DateRange } from "react-day-picker";

import type { DateOnly } from "@/lib/date";
import { getDateOnly } from "@/lib/date";

interface UseDateRangeOptions {
  defaultStartDate?: Date;
  defaultEndDate?: Date;
}

export function useDateRange(options?: UseDateRangeOptions) {
  const defaultStart = useMemo(
    () => options?.defaultStartDate ?? subDays(new Date(), 31),
    [options?.defaultStartDate],
  );
  const defaultEnd = useMemo(
    () => options?.defaultEndDate ?? new Date(),
    [options?.defaultEndDate],
  );

  const [selectedRange, setSelectedRange] = useState<DateRange | undefined>({
    from: defaultStart,
    to: defaultEnd,
  });

  const startDate = useMemo((): DateOnly | null => {
    if (!selectedRange?.from) return null;
    return getDateOnly(selectedRange.from);
  }, [selectedRange]);

  const endDate = useMemo((): DateOnly | null => {
    if (!selectedRange?.to) return null;
    return getDateOnly(selectedRange.to);
  }, [selectedRange]);

  return {
    selectedRange,
    setSelectedRange,
    startDate,
    endDate,
  };
}
