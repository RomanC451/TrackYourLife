import { describe, expect, it } from "vitest";

import { BookStatus } from "@/services/openapi";

import { bookFormSchema } from "../bookSchema";
import { bookFormValuesToRequest } from "../bookFormValuesToRequest";

describe("bookFormValuesToRequest", () => {
  it("omits finishDate and rating for Ongoing books", () => {
    const request = bookFormValuesToRequest({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 50,
      status: BookStatus.Ongoing,
      startingDate: "2026-06-01",
      finishDate: "2026-06-10",
      rating: 5,
    });

    expect(request).toEqual({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 50,
      status: BookStatus.Ongoing,
      startingDate: "2026-06-01",
    });
    expect(request).not.toHaveProperty("finishDate");
    expect(request).not.toHaveProperty("rating");
  });

  it("defaults startingDate for Ongoing when missing", () => {
    const request = bookFormValuesToRequest({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 50,
      status: BookStatus.Ongoing,
    });

    expect(request.startingDate).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });

  it("includes finishDate and rating for Finished books", () => {
    const request = bookFormValuesToRequest({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 200,
      status: BookStatus.Finished,
      startingDate: "2026-01-01",
      finishDate: "2026-06-01",
      rating: 4,
    });

    expect(request).toEqual({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 200,
      status: BookStatus.Finished,
      startingDate: "2026-01-01",
      finishDate: "2026-06-01",
      rating: 4,
    });
  });

  it("defaults finishDate for Finished when missing", () => {
    const request = bookFormValuesToRequest({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 200,
      status: BookStatus.Finished,
      startingDate: "2026-01-01",
      rating: 5,
    });

    expect(request.finishDate).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });
});

describe("bookFormSchema", () => {
  it("accepts Ongoing books with a starting date", () => {
    const result = bookFormSchema.safeParse({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 50,
      status: BookStatus.Ongoing,
      startingDate: "2026-06-01",
    });

    expect(result.success).toBe(true);
  });

  it("rejects Ongoing books without a starting date", () => {
    const result = bookFormSchema.safeParse({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 50,
      status: BookStatus.Ongoing,
      startingDate: "",
    });

    expect(result.success).toBe(false);
  });

  it("accepts Finished books with required fields", () => {
    const result = bookFormSchema.safeParse({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 200,
      status: BookStatus.Finished,
      startingDate: "2026-01-01",
      finishDate: "2026-06-01",
      rating: 5,
    });

    expect(result.success).toBe(true);
  });

  it("rejects Finished books without a rating", () => {
    const result = bookFormSchema.safeParse({
      title: "Test",
      author: "Author",
      totalPages: 200,
      currentPage: 200,
      status: BookStatus.Finished,
      startingDate: "2026-01-01",
      finishDate: "2026-06-01",
    });

    expect(result.success).toBe(false);
  });
});
