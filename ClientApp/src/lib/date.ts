export type DateOnly = `${number}-${number}-${number}`;

export function getDateOnly(date: Date): DateOnly {
  const dateOnly = date.toISOString().split("T")[0];
  return dateOnly as DateOnly;
}

export function parseDateOnly(date: DateOnly): Date {
  return new Date(date);
}
