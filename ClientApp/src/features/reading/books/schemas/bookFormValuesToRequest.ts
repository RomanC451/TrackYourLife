import { getDateOnly } from "@/lib/date";
import {
  BookStatus,
  type CreateBookRequest,
  type UpdateBookRequest,
} from "@/services/openapi";

import type { BookFormValues } from "./bookSchema";

function toOptionalDate(value?: string) {
  if (!value || value.trim() === "") {
    return undefined;
  }

  return value.slice(0, 10);
}

export function bookFormValuesToRequest(
  values: BookFormValues,
): CreateBookRequest & UpdateBookRequest {
  const base = {
    title: values.title.trim(),
    author: values.author.trim(),
    totalPages: values.totalPages,
    currentPage: values.currentPage,
    status: values.status,
  };

  if (values.status === BookStatus.NotStarted) {
    return base;
  }

  const startingDate =
    toOptionalDate(values.startingDate) ?? getDateOnly(new Date());

  if (values.status === BookStatus.Ongoing) {
    return {
      ...base,
      startingDate,
    };
  }

  const finishDate =
    toOptionalDate(values.finishDate) ?? getDateOnly(new Date());

  return {
    ...base,
    startingDate,
    finishDate,
    rating: values.rating,
  };
}
