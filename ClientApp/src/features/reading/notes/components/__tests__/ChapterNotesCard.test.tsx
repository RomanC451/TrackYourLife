import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import ChapterNotesCard from "../ChapterNotesCard";

const notes = [
  {
    noteId: "note-1",
    sessionId: "session-1",
    date: "2026-06-22",
    content: "Stay with the breath",
    createdOnUtc: "2026-06-22T10:00:00Z",
    isLoading: false,
    isDeleting: false,
  },
];

describe("ChapterNotesCard", () => {
  it("shows a shared date by default", () => {
    render(<ChapterNotesCard notes={notes} />);

    expect(screen.getByText("2026-06-22")).toBeInTheDocument();
    expect(screen.getByText("Stay with the breath")).toBeInTheDocument();
  });

  it("hides dates when showDate is false", () => {
    render(<ChapterNotesCard notes={notes} showDate={false} />);

    expect(screen.queryByText("2026-06-22")).not.toBeInTheDocument();
    expect(screen.getByText("Stay with the breath")).toBeInTheDocument();
  });
});
