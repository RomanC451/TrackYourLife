import { useState } from "react";

export type DateOnly = `${number}-${number}-${number}`;

export function getDateOnly(date: Date): DateOnly {
  const dateOnly = date.toISOString().split("T")[0];
  return dateOnly as DateOnly;
}

export function parseDateOnly(date: DateOnly): Date {
  return new Date(date);
}

export function useDateOnlyState(initialDate?: Date) {
  const [date, setDate] = useState<DateOnly>(
    initialDate ? getDateOnly(initialDate) : getDateOnly(new Date()),
  );

  function setDateOnly(date: Date) {
    setDate(getDateOnly(date));
  }

  return [date, setDateOnly] as const;
}
