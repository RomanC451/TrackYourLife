import { render } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { WorkoutHistoryCardSkeleton } from "../WorkoutHistoryCardSkeleton";

describe("WorkoutHistoryCardSkeleton", () => {
  it("renders loading placeholders", () => {
    const { container } = render(<WorkoutHistoryCardSkeleton />);
    expect(container.querySelectorAll(".animate-pulse").length).toBeGreaterThan(0);
  });
});
