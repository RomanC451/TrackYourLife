import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import type { GuitarTabSearchResponse } from "../../api/guitarTabsApi";

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    search,
  }: {
    children: React.ReactNode;
    to: string;
    search?: { url?: string };
  }) => (
    <a href={`${to}?url=${encodeURIComponent(search?.url ?? "")}`}>{children}</a>
  ),
}));

import GuitarTabSearchResults from "../GuitarTabSearchResults";

const response: GuitarTabSearchResponse = {
  query: "viu",
  resultsCount: 15,
  page: 1,
  totalPages: 2,
  results: [
    {
      id: 5059138,
      songName: "Viu",
      artistName: "Bosquito",
      type: "Chords",
      votes: 5,
      rating: 4.55,
      tonality: "C",
      version: 2,
      part: "Intro",
      tabUrl: "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138",
      artistUrl: "https://www.ultimate-guitar.com/artist/bosquito_123239",
    },
  ],
};

describe("GuitarTabSearchResults", () => {
  it("lists artist, song, and type from the search page", () => {
    render(
      <GuitarTabSearchResults
        response={response}
        isError={false}
        isLoading={false}
        onPageChange={vi.fn()}
      />,
    );

    expect(screen.getByText("15 chords & tabs found")).toBeInTheDocument();
    expect(screen.getByText("Bosquito")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Viu (Intro, ver 2)" })).toHaveAttribute(
      "href",
      "/guitar-tabs/tab?url=https%3A%2F%2Ftabs.ultimate-guitar.com%2Ftab%2Fbosquito%2Fviu-chords-5059138",
    );
    expect(screen.getByText("Chords")).toBeInTheDocument();
    expect(screen.getByText("5")).toBeInTheDocument();
  });

  it("moves to the next page", () => {
    const onPageChange = vi.fn();

    render(
      <GuitarTabSearchResults
        response={response}
        isError={false}
        isLoading={false}
        onPageChange={onPageChange}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Next" }));

    expect(onPageChange).toHaveBeenCalledWith(2);
  });
});
