import { formatDate } from "date-fns";

export type DateOnly = `${number}-${number}-${number}`;

export function getDateOnly(date: Date): DateOnly {
  // const dateOnly = date.toISOString().split("T")[0];

  const dateOnly = formatDate(date, "yyyy-MM-dd") as DateOnly;

  return dateOnly as DateOnly;
}

export function parseDateOnly(date: DateOnly): Date {
  return new Date(date);
}
