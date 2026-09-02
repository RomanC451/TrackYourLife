import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    params,
    ...props
  }: {
    children: React.ReactNode;
    to: string;
    params?: { bookId: string };
  } & React.AnchorHTMLAttributes<HTMLAnchorElement>) => (
    <a href={params?.bookId ? `/reading/books/${params.bookId}` : to} {...props}>
      {children}
    </a>
  ),
}));

vi.mock("../ReadingPagesChart", () => ({
  default: () => <div data-testid="pages-chart">Pages read</div>,
}));

vi.mock("@/features/reading/notes/components/ReadingNoteCard", () => ({
  default: () => <div>Current chapter</div>,
}));

import ReadingDashboardPage from "../ReadingDashboardPage";

const dashboardData = {
  dailyProgress: {
    hasTarget: true,
    pagesReadToday: 3,
    targetPages: 10,
    remaining: 7,
    targetMet: false,
  },
  streak: {
    currentStreak: 2,
    longestStreak: 5,
    todayTargetMet: false,
  },
  activeSession: undefined,
  recentBooks: [
    {
      id: "book-1",
      title: "Narrow Corridor",
      currentPage: 409,
      totalPages: 555,
    },
  ],
};

describe("ReadingDashboardPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({ data: dashboardData });
  });

  it("renders the dashboard layout with progress, streak, and books", () => {
    render(<ReadingDashboardPage />);

    expect(screen.getByText("Your reading space")).toBeInTheDocument();
    expect(screen.getByText("Reading dashboard")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Daily goal/i })).toHaveAttribute(
      "href",
      "/reading/daily-goal",
    );
    expect(screen.getByText("10 pages")).toBeInTheDocument();
    expect(screen.getByText("Today's progress")).toBeInTheDocument();
    expect(screen.getByRole("progressbar")).toHaveAttribute("aria-valuenow", "30");
    expect(screen.getByText("7 pages remaining")).toBeInTheDocument();
    expect(screen.getByText("Streak")).toBeInTheDocument();
    expect(screen.getByText("Keep going")).toBeInTheDocument();
    expect(screen.getByText("Narrow Corridor")).toBeInTheDocument();
    expect(screen.getByText("(409/555)")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "View all books" })).toHaveAttribute(
      "href",
      "/reading/books",
    );
  });

  it("shows an empty goal state and active session when present", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: {
        ...dashboardData,
        dailyProgress: {
          hasTarget: false,
          pagesReadToday: 0,
          remaining: 0,
          targetMet: false,
        },
        activeSession: { bookTitle: "Deep Work" },
        recentBooks: [],
      },
    });

    render(<ReadingDashboardPage />);

    expect(
      screen.getByText("Set a daily reading goal to track progress."),
    ).toBeInTheDocument();
    expect(screen.getByText("No ongoing books.")).toBeInTheDocument();
    expect(screen.getByText("Deep Work")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Resume" })).toHaveAttribute(
      "href",
      "/reading/ongoing-session",
    );
  });
});
