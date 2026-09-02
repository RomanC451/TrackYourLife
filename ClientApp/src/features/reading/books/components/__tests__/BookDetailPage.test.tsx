import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockUseSuspenseQuery, mockMutate, mockNavigate } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
  mockMutate: vi.fn(),
  mockNavigate: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  Link: ({
    children,
    to,
    params,
    ...props
  }: {
    children: React.ReactNode;
    to: string;
    params?: { bookId: string };
  } & React.AnchorHTMLAttributes<HTMLAnchorElement>) => {
    const href = params?.bookId
      ? to.replace("$bookId", params.bookId)
      : to;
    return (
      <a href={href} {...props}>
        {children}
      </a>
    );
  },
}));

vi.mock("../../../ongoing-session/mutations/readingSessionMutations", () => ({
  useStartReadingSessionMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import BookDetailPage from "../BookDetailPage";

const book = {
  id: "book-1",
  title: "Attention with intention",
  author: "Dragos Patraru",
  currentPage: 210,
  totalPages: 400,
  status: "Ongoing",
  suggestMarkAsFinished: false,
};

const chapterGroups = [
  {
    chapterTitle: "Chapter 9 - Flux",
    notes: [
      {
        noteId: "note-1",
        sessionId: "session-1",
        date: "2026-06-22",
        content: "Stay with the breath",
      },
    ],
  },
  {
    chapterTitle: "Chapter 8 - Drift",
    notes: [
      {
        noteId: "note-2",
        sessionId: "session-1",
        date: "2026-06-17",
        content: "Earlier thought",
      },
    ],
  },
];

function mockBookQueries(
  bookData = book,
  notes: typeof chapterGroups | [] = chapterGroups,
) {
  mockUseSuspenseQuery
    .mockReturnValueOnce({ data: bookData })
    .mockReturnValueOnce({ data: notes });
}

describe("BookDetailPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders hero details, full-width progress, and notes", () => {
    mockBookQueries();

    render(<BookDetailPage bookId="book-1" />);

    expect(screen.getByRole("link", { name: "Books" })).toHaveAttribute(
      "href",
      "/reading/books",
    );
    expect(
      screen.getByRole("heading", { name: "Attention with intention" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Dragos Patraru")).toBeInTheDocument();
    expect(screen.getByText("Page 210 of 400")).toBeInTheDocument();
    expect(screen.getByText("53% complete")).toBeInTheDocument();
    expect(screen.getByText("190 pages left")).toBeInTheDocument();
    expect(screen.getByRole("progressbar")).toHaveAttribute(
      "aria-valuenow",
      "53",
    );
    expect(screen.getByRole("progressbar")).toHaveClass("w-full");
    expect(screen.getByRole("link", { name: /Edit/ })).toHaveAttribute(
      "href",
      "/reading/books/edit/book-1",
    );
    expect(
      screen.getByRole("button", { name: "Start reading" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Your journey")).toBeInTheDocument();
    expect(screen.getByText("Chapter 9 - Flux")).toBeInTheDocument();
    expect(screen.getByText("Stay with the breath")).toBeInTheDocument();
  });

  it("shows an empty notes state", () => {
    mockBookQueries(book, []);

    render(<BookDetailPage bookId="book-1" />);

    expect(
      screen.getByText("No notes from reading sessions yet."),
    ).toBeInTheDocument();
  });
});
