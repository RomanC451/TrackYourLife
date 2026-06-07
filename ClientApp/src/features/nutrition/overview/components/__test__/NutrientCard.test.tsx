import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { ThemeProvider } from "@/components/theme-provider";
import { colors } from "@/constants/tailwindColors";
import { OverviewType } from "@/services/openapi";

import { NutrientCard } from "../NutrientCard";

vi.mock("framer-motion", () => ({
  motion: {
    div: ({
      className,
      children,
    }: {
      className?: string;
      children: React.ReactNode;
    }) => <div className={className}>{children}</div>,
  },
  useAnimation: () => ({
    start: vi.fn(),
  }),
}));

describe("NutrientCard", () => {
  const defaultProps = {
    title: "Calories",
    currentValue: 1500,
    targetValue: 2000,
    unit: "kcal",
    color: colors.blue,
    overviewType: OverviewType.Daily as OverviewType,
    isLoading: false,
  };

  const renderCard = (props = defaultProps) =>
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...props} />
      </ThemeProvider>,
    );

  it("renders the component with correct title", () => {
    renderCard();
    expect(screen.getByText("Calories")).toBeInTheDocument();
  });

  it("displays current and target values correctly", () => {
    renderCard();
    expect(screen.getByText("1500 / 2000 kcal")).toBeInTheDocument();
  });

  it("calculates percentage correctly", () => {
    renderCard();
    expect(screen.getByText("75% of daily target")).toBeInTheDocument();
  });

  it("displays weekly target text when overviewType is Weekly", () => {
    renderCard({ ...defaultProps, overviewType: OverviewType.Weekly });
    expect(screen.getByText("75% of weekly target")).toBeInTheDocument();
  });

  it("displays monthly target text when overviewType is Monthly", () => {
    renderCard({ ...defaultProps, overviewType: OverviewType.Monthly });
    expect(screen.getByText("75% of monthly target")).toBeInTheDocument();
  });

  it("handles zero target value safely", () => {
    renderCard({ ...defaultProps, targetValue: 0 });
    expect(screen.getByText("0% of daily target")).toBeInTheDocument();
  });

  it("shows loading state without hiding the title", () => {
    renderCard({ ...defaultProps, isLoading: true });
    expect(screen.getByText("Calories")).toBeInTheDocument();
  });
});
