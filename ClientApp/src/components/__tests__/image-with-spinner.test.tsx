import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { ImageWithSpinner } from "../image-with-spinner";

vi.mock("../optimized-image", () => ({
  default: ({ src, alt }: { src: string; alt: string }) => (
    <img src={src} alt={alt} />
  ),
}));

describe("ImageWithSpinner", () => {
  it("renders a fallback when src is missing", () => {
    render(<ImageWithSpinner alt="Missing" />);

    expect(screen.getByText("No image")).toBeInTheDocument();
  });

  it("renders the optimized image when src is provided", () => {
    render(
      <ImageWithSpinner src="https://example.com/photo.png" alt="Photo" />,
    );

    expect(screen.getByRole("img", { name: "Photo" })).toHaveAttribute(
      "src",
      "https://example.com/photo.png",
    );
  });
});
