import { fireEvent, render, screen } from "@testing-library/react";
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

import BookPreviousNotesPanel from "../BookPreviousNotesPanel";

function chapter(
  chapterTitle: string,
  notes: { noteId: string; content: string; date?: string }[],
) {
  return {
    chapterTitle,
    notes: notes.map((note) => ({
      sessionId: "session-1",
      date: note.date ?? "2026-06-22",
      ...note,
    })),
  };
}

describe("BookPreviousNotesPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows an empty notes section without wrapping the list in a card heading only", () => {
    mockUseQuery.mockReturnValue({ data: [] });

    render(<BookPreviousNotesPanel bookId="book-1" />);

    expect(screen.getByText("Your notes")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: "Recent thoughts" }),
    ).toBeInTheDocument();
    expect(screen.getByText("0 notes")).toBeInTheDocument();
    expect(
      screen.getByText("Notes you save during this session will show up here."),
    ).toBeInTheDocument();
  });

  it("shows chapter titles without chapter numbers", () => {
    mockUseQuery.mockReturnValue({
      data: [
        chapter("Chapter 9 - Flux si concentrare", [
          { noteId: "note-1", content: "Stay with the breath" },
        ]),
      ],
    });

    render(<BookPreviousNotesPanel bookId="book-1" />);

    expect(screen.getByText("1 note")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: "Flux si concentrare" }),
    ).toBeInTheDocument();
    expect(screen.queryByText(/Chapter 9/i)).not.toBeInTheDocument();
    expect(screen.getByText("Stay with the breath")).toBeInTheDocument();
  });

  it("keeps the collapse control after expanding older notes", () => {
    mockUseQuery.mockReturnValue({
      data: [
        chapter("Chapter 1 - First", [
          { noteId: "note-1", content: "one" },
          { noteId: "note-2", content: "two" },
        ]),
        chapter("Chapter 2 - Second", [
          { noteId: "note-3", content: "three", date: "2026-06-21" },
          { noteId: "note-4", content: "four", date: "2026-06-21" },
        ]),
      ],
    });

    render(<BookPreviousNotesPanel bookId="book-1" />);

    expect(screen.getByText("4 notes")).toBeInTheDocument();
    expect(screen.getByText("one")).toBeInTheDocument();
    expect(screen.queryByText("three")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Show 2 older notes" }));

    expect(screen.getByText("three")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Show fewer notes" })).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Show fewer notes" }));

    expect(screen.queryByText("three")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Show 2 older notes" })).toBeInTheDocument();
  });
});
