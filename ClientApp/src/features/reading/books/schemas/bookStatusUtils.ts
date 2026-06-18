import { BookStatus } from "@/services/openapi";

export const bookStatusOptions = [
  BookStatus.NotStarted,
  BookStatus.Ongoing,
  BookStatus.Finished,
] as const;

export function normalizeBookStatus(value: unknown): BookStatus {
  if (value === BookStatus.NotStarted) {
    return BookStatus.NotStarted;
  }

  if (value === BookStatus.Ongoing) {
    return BookStatus.Ongoing;
  }

  if (value === BookStatus.Finished) {
    return BookStatus.Finished;
  }

  if (value === 0 || value === "0" || value === "NotStarted") {
    return BookStatus.NotStarted;
  }

  if (value === 1 || value === "1" || value === "Ongoing") {
    return BookStatus.Ongoing;
  }

  if (value === 2 || value === "2" || value === "Finished") {
    return BookStatus.Finished;
  }

  return BookStatus.NotStarted;
}

export function showsStartingDate(status: BookStatus) {
  return status === BookStatus.Ongoing || status === BookStatus.Finished;
}

export function showsFinishedFields(status: BookStatus) {
  return status === BookStatus.Finished;
}
