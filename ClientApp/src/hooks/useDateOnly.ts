import { useState } from "react";

import { DateOnly, getDateOnly } from "@/lib/date";

export function useDateOnlyState(initialDate?: Date) {
  const [date, setDate] = useState<DateOnly>(
    initialDate ? getDateOnly(initialDate) : getDateOnly(new Date()),
  );

  function setDateOnly(date: Date) {
    setDate(getDateOnly(date));
  }

  return [date, setDateOnly] as const;
}
