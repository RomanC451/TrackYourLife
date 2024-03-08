export type DateOnly = `${number}-${number}-${number}`;

export function getDateOnly(date?: Date): DateOnly {
  if (!date) {
    const dateOnly = new Date().toISOString().split("T")[0];
    return dateOnly as DateOnly;
  }
  const dateOnly = date.toISOString().split("T")[0];
  return dateOnly as DateOnly;
}
