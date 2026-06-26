import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockUseQuery } = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    params,
  }: {
    children: React.ReactNode;
    params?: { bookId: string };
  }) => <a href={`/reading/books/${params?.bookId ?? ""}`}>{children}</a>,
}));

import ReadingNoteCard from "../ReadingNoteCard";

describe("ReadingNoteCard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows loading skeletons while the note query is pending", () => {
    mockUseQuery.mockReturnValue({ data: undefined, isLoading: true });

    const { container } = render(<ReadingNoteCard scope="dashboard" />);

    expect(container.querySelectorAll(".animate-pulse")).toHaveLength(4);
  });

  it("shows empty state copy when no note is available", () => {
    mockUseQuery.mockReturnValue({ data: null, isLoading: false });

    render(<ReadingNoteCard scope="home" />);

    expect(screen.getByText("Reading note")).toBeInTheDocument();
    expect(
      screen.getByText(/No notes yet. Add chapter notes while reading/),
    ).toBeInTheDocument();
  });

  it("renders chapter title, content, and book link when a note exists", () => {
    mockUseQuery.mockReturnValue({
      data: {
        noteId: "note-1",
        bookId: "book-1",
        bookTitle: "Flow Book",
        chapterTitle: "Cap. 1 — Start",
        content: "A memorable quote",
      },
      isLoading: false,
    });

    render(<ReadingNoteCard scope="dashboard" />);

    expect(screen.getByText("Cap. 1 — Start")).toBeInTheDocument();
    expect(screen.getByText("A memorable quote")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Flow Book" })).toHaveAttribute(
      "href",
      "/reading/books/book-1",
    );
    expect(screen.queryByText("Reading note")).not.toBeInTheDocument();
  });
});
