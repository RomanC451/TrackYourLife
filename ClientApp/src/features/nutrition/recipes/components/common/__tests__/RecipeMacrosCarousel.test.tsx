import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import RecipeMacrosCarousel from "../RecipeMacrosCarousel";

vi.mock("@/components/ui/carousel", () => ({
  Carousel: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="carousel">{children}</div>
  ),
  CarouselContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  CarouselItem: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  CarouselPrevious: () => <button type="button">Previous</button>,
  CarouselNext: () => <button type="button">Next</button>,
  CarouselDots: () => <div data-testid="carousel-dots" />,
}));

vi.mock(
  "@/features/nutrition/common/components/macros/MacrosDialogHeader",
  () => ({
    default: ({
      nutritionalContents,
    }: {
      nutritionalContents: { protein: number; energy: { value: number } };
    }) => (
      <div data-testid="macros-header">
        {nutritionalContents.protein}g / {nutritionalContents.energy.value} kcal
      </div>
    ),
  }),
);

describe("RecipeMacrosCarousel", () => {
  it("renders per-portion and total macro views", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.protein = 20;
    nutrition.energy = { unit: "calories", value: 400 };

    render(
      <RecipeMacrosCarousel
        recipe={recipe("recipe-1", "Oats bowl", {
          portions: 2,
          nutritionalContents: nutrition,
        })}
      />,
    );

    expect(screen.getByTestId("carousel")).toBeInTheDocument();
    expect(screen.getByText("Per portion:")).toBeInTheDocument();
    expect(screen.getByText("Total:")).toBeInTheDocument();
    expect(screen.getByText("10g / 200 kcal")).toBeInTheDocument();
    expect(screen.getByText("20g / 400 kcal")).toBeInTheDocument();
  });
});
