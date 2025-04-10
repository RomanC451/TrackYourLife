import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { ThemeProvider } from "@/components/theme-provider";
import { colors } from "@/constants/tailwindColors";

import { NutrientCard } from "../NutrientCard";
import { OverviewType } from "../NutrientsCharts";

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
    current: 1500,
    target: 2000,
    unit: "kcal",
    color: colors.blue,
    overviewType: "day" as OverviewType,
    isLoading: false,
  };

  it("renders the component with correct title", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} />
      </ThemeProvider>,
    );

    expect(screen.getByText("Calories")).toBeInTheDocument();
  });

  it("displays current and target values correctly", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} />
      </ThemeProvider>,
    );

    expect(screen.getByText("1500 / 2000 kcal")).toBeInTheDocument();
  });

  it("calculates percentage correctly", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} />
      </ThemeProvider>,
    );

    // 1500/2000 = 75%
    expect(screen.getByText("75% of daily target")).toBeInTheDocument();
  });

  it("displays weekly target text when overviewType is week", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} overviewType="week" />
      </ThemeProvider>,
    );

    expect(screen.getByText("75% of weekly target")).toBeInTheDocument();
  });

  it("displays monthly target text when overviewType is month", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} overviewType="month" />
      </ThemeProvider>,
    );

    expect(screen.getByText("75% of monthly target")).toBeInTheDocument();
  });

  it("handles zero target value safely", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} target={0} />
      </ThemeProvider>,
    );

    expect(screen.getByText("0% of daily target")).toBeInTheDocument();
  });

  it("shows loading state", () => {
    render(
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <NutrientCard {...defaultProps} isLoading={true} />
      </ThemeProvider>,
    );

    expect(screen.getByText("Calories")).toBeInTheDocument();
  });
});
