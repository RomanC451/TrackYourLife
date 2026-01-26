import { useMemo, useState } from "react";
import { DateRange } from "react-day-picker";
import { subDays } from "date-fns";

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

  const startDate = useMemo(() => {
    return selectedRange?.from
      ? getDateOnly(selectedRange.from)
      : getDateOnly(defaultStart);
  }, [selectedRange, defaultStart]);

  const endDate = useMemo(() => {
    return selectedRange?.to
      ? getDateOnly(selectedRange.to)
      : getDateOnly(defaultEnd);
  }, [selectedRange, defaultEnd]);

  return {
    selectedRange,
    setSelectedRange,
    startDate,
    endDate,
  };
}
