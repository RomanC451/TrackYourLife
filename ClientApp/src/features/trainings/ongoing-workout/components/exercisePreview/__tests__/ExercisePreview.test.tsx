import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise, ongoingTraining } from "@/features/trainings/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

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

vi.mock("@/features/trainings/common/components/workoutTimer/WorkoutTimer", () => ({
  default: () => <div data-testid="workout-timer" />,
}));

vi.mock("../CurrentSet", () => ({
  default: ({ equipment }: { equipment?: string }) => (
    <div data-testid="current-set">{equipment}</div>
  ),
}));

vi.mock("../ExerciseDescriptionCollapsible", () => ({
  default: () => <div data-testid="description" />,
}));

vi.mock("../ExercisePreviewFooter", () => ({
  default: () => <div data-testid="preview-footer" />,
}));

vi.mock("@/components/image-with-spinner", () => ({
  ImageWithSpinner: ({ alt }: { alt: string }) => <img alt={alt} />,
}));

vi.mock("@/components/video-player-with-loading", () => ({
  default: () => <div data-testid="video-player" />,
}));

import ExercisePreview from "../ExercisePreview";

describe("ExercisePreview", () => {
  beforeEach(() => {
    mockUseSuspenseQuery.mockReturnValue({
      data: ongoingTraining({
        training: {
          ...ongoingTraining().training,
          exercises: [
            exercise("ex-1", {
              name: "Bench press",
              description: "Keep shoulders pinned",
              pictureUrl: "https://cdn.test/bench.png",
              videoUrl: "https://cdn.test/bench.mp4",
              equipment: "Barbell",
            }),
          ],
        },
      }),
    });
  });

  it("renders timer, current set, description, media tabs, and footer", () => {
    render(<ExercisePreview />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByTestId("workout-timer")).toBeInTheDocument();
    expect(screen.getByTestId("current-set")).toHaveTextContent("Barbell");
    expect(screen.getByTestId("description")).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Image" })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Video" })).toBeInTheDocument();
    expect(screen.getByAltText("Bench press")).toBeInTheDocument();
    expect(screen.getByTestId("preview-footer")).toBeInTheDocument();
  });
});
