import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const {
  mockUseQuery,
  mockNavigate,
  mockMutateAsync,
  mockCancelMutateAsync,
  mockPause,
  mockStart,
  timerState,
  sessionState,
  chapterGroupsState,
} = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
  mockNavigate: vi.fn(),
  mockMutateAsync: vi.fn(),
  mockCancelMutateAsync: vi.fn(),
  mockPause: vi.fn(),
  mockStart: vi.fn(),
  timerState: {
    elapsedSeconds: 12,
    isRunning: true,
    start: () => {},
    pause: () => {},
    reset: vi.fn(),
  },
  sessionState: {
    current: {
      id: "session-1",
      bookId: "book-1",
      bookTitle: "Attention with intention",
      bookAuthor: "Dragos Patraru",
      startPage: 10,
    } as {
      id: string;
      bookId: string;
      bookTitle: string;
      bookAuthor: string;
      startPage: number;
    } | null,
  },
  chapterGroupsState: {
    current: [
      {
        chapterTitle: "Chapter 4 - Focus",
        notes: [
          {
            noteId: "note-1",
            sessionId: "session-1",
            date: "2026-06-22",
            content: "Stay with the breath",
            createdOnUtc: "2026-06-22T10:00:00Z",
          },
        ],
      },
    ],
  },
}));

timerState.start = mockStart;
timerState.pause = mockPause;

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  Link: ({
    children,
    to,
    ...props
  }: {
    children: React.ReactNode;
    to: string;
  } & React.AnchorHTMLAttributes<HTMLAnchorElement>) => (
    <a href={to} {...props}>
      {children}
    </a>
  ),
}));

vi.mock("sonner", () => ({
  toast: { success: vi.fn(), message: vi.fn() },
}));

vi.mock("../../context/ReadingTimerContext", () => ({
  useReadingTimer: () => timerState,
}));

vi.mock("../../mutations/readingSessionMutations", () => ({
  useAddReadingSessionNoteMutation: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
  useCancelReadingSessionMutation: () => ({
    mutateAsync: mockCancelMutateAsync,
    isPending: false,
  }),
}));

vi.mock("../BookPreviousNotesPanel", () => ({
  default: () => <div>Previous notes</div>,
}));

vi.mock("../FinishReadingSessionDialog", () => ({
  default: ({ open }: { open: boolean }) =>
    open ? <div role="dialog">Finish session dialog</div> : null,
}));

import OngoingReadingSessionPage from "../OngoingReadingSessionPage";

describe("OngoingReadingSessionPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    timerState.elapsedSeconds = 12;
    timerState.isRunning = true;
    sessionState.current = {
      id: "session-1",
      bookId: "book-1",
      bookTitle: "Attention with intention",
      bookAuthor: "Dragos Patraru",
      startPage: 10,
    };
    chapterGroupsState.current = [
      {
        chapterTitle: "Chapter 4 - Focus",
        notes: [
          {
            noteId: "note-1",
            sessionId: "session-1",
            date: "2026-06-22",
            content: "Stay with the breath",
            createdOnUtc: "2026-06-22T10:00:00Z",
          },
        ],
      },
    ];
    mockMutateAsync.mockResolvedValue("note-2");
    mockCancelMutateAsync.mockResolvedValue(undefined);
    mockUseQuery.mockImplementation((options?: { queryKey?: unknown[] }) => {
      const key = options?.queryKey ?? [];
      if (key[1] === "active") {
        return { data: sessionState.current };
      }
      if (key[1] === "bookChapterNotes") {
        return { data: chapterGroupsState.current };
      }
      return { data: undefined };
    });
  });

  it("shows the empty state when there is no active session", () => {
    sessionState.current = null;

    render(<OngoingReadingSessionPage />);

    expect(screen.getByText(/No active session/)).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Pick a book" })).toHaveAttribute(
      "href",
      "/reading/books",
    );
  });

  it("renders session details and a title-only note form", () => {
    render(<OngoingReadingSessionPage />);

    expect(screen.getByText("Reading session")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: "Attention with intention" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Dragos Patraru · Started at page 10")).toBeInTheDocument();
    expect(screen.getByText("00:12")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Pause" })).toBeInTheDocument();
    expect(screen.getByLabelText("Chapter title")).toHaveValue("Focus");
    expect(screen.queryByLabelText("Chapter number")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Save note" })).toBeDisabled();
    expect(screen.getByRole("button", { name: "Finish session" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Cancel" })).toBeInTheDocument();
  });

  it("saves a note with the stored chapter title when the display title matches", async () => {
    render(<OngoingReadingSessionPage />);

    fireEvent.change(screen.getByLabelText("Note"), {
      target: { value: "A new thought" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save note" }));

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith({
        sessionId: "session-1",
        bookId: "book-1",
        chapterTitle: "Chapter 4 - Focus",
        content: "A new thought",
      });
    });
    expect(screen.getByLabelText("Note")).toHaveValue("");
    expect(screen.getByLabelText("Chapter title")).toHaveValue("Focus");
  });

  it("saves a new chapter title as-is", async () => {
    render(<OngoingReadingSessionPage />);

    fireEvent.change(screen.getByLabelText("Chapter title"), {
      target: { value: "Momentum" },
    });
    fireEvent.change(screen.getByLabelText("Note"), {
      target: { value: "Keep going" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save note" }));

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith({
        sessionId: "session-1",
        bookId: "book-1",
        chapterTitle: "Momentum",
        content: "Keep going",
      });
    });
  });

  it("pauses the running timer", () => {
    render(<OngoingReadingSessionPage />);

    fireEvent.click(screen.getByRole("button", { name: "Pause" }));

    expect(mockPause).toHaveBeenCalled();
  });

  it("resumes a paused timer", () => {
    timerState.isRunning = false;

    render(<OngoingReadingSessionPage />);

    fireEvent.click(screen.getByRole("button", { name: "Resume" }));

    expect(mockStart).toHaveBeenCalled();
  });

  it("cancels the session and returns to books", async () => {
    render(<OngoingReadingSessionPage />);

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));

    await waitFor(() => {
      expect(mockCancelMutateAsync).toHaveBeenCalledWith("session-1");
    });
    expect(mockNavigate).toHaveBeenCalledWith({ to: "/reading/books" });
  });

  it("opens the finish session dialog", () => {
    render(<OngoingReadingSessionPage />);

    fireEvent.click(screen.getByRole("button", { name: "Finish session" }));

    expect(screen.getByRole("dialog")).toHaveTextContent("Finish session dialog");
  });
});
