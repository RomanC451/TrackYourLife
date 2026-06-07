import { render } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import LinearProgress from "../linear-progress";

describe("LinearProgress", () => {
  it("renders with the requested width", () => {
    const { container } = render(<LinearProgress value={75} />);

    const bar = container.querySelector("[style*='width']") as HTMLDivElement;

    expect(bar.style.width).toBe("75%");
  });
});
