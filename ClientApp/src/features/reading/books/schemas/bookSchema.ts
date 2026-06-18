import { z } from "zod";

import { BookStatus } from "@/services/openapi";

function optionalDateString() {
  return z.preprocess(
    (value) =>
      value === "" || value === null || value === undefined ? undefined : value,
    z.string().optional(),
  );
}

function hasDate(value?: string) {
  return value !== undefined && value.trim() !== "";
}

export const bookFormSchema = z
  .object({
    title: z.string().min(1, "Title is required"),
    author: z.string().min(1, "Author is required"),
    totalPages: z.coerce.number().int().min(1, "Total pages must be at least 1"),
    currentPage: z.coerce.number().int().min(0),
    status: z.enum([
      BookStatus.NotStarted,
      BookStatus.Ongoing,
      BookStatus.Finished,
    ]),
    startingDate: optionalDateString(),
    finishDate: optionalDateString(),
    rating: z.preprocess(
      (value) => {
        if (value === "" || value === null || value === undefined) {
          return undefined;
        }

        const parsed = Number(value);
        return Number.isNaN(parsed) ? undefined : parsed;
      },
      z.number().int().min(1).max(5).optional(),
    ),
  })
  .superRefine((data, ctx) => {
    if (data.currentPage > data.totalPages) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Current page cannot exceed total pages",
        path: ["currentPage"],
      });
    }

    if (
      (data.status === BookStatus.Ongoing || data.status === BookStatus.Finished) &&
      !hasDate(data.startingDate)
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Starting date is required",
        path: ["startingDate"],
      });
    }

    if (data.status === BookStatus.Finished) {
      if (!hasDate(data.finishDate)) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Finish date is required",
          path: ["finishDate"],
        });
      }
      if (!data.rating) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Rating is required",
          path: ["rating"],
        });
      }
    }
  });

export type BookFormValues = z.infer<typeof bookFormSchema>;
