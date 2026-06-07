import { render, screen } from "@testing-library/react";
import type { CellContext } from "@tanstack/react-table";
import { describe, expect, it, vi } from "vitest";

import { servingSize } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import {
  DiaryType,
  MealTypes,
  type NutritionDiaryDto,
} from "@/services/openapi";

import { foodDiaryTableColumns } from "../foodDiaryTableColumns";

vi.mock("../FoodDiaryTableRowActionsMenu", () => ({
  default: () => <div data-testid="row-actions" />,
}));

vi.mock("@/components/ui/hybrid-tooltip", () => ({
  HybridTooltip: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  HybridTooltipTrigger: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  HybridTooltipContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
}));

function diary(overrides: Partial<NutritionDiaryDto> = {}): NutritionDiaryDto {
  const nutrition = createEmptyNutritionalContent();
  nutrition.energy = { unit: "calories", value: 250.4 };
  nutrition.carbohydrates = 30.2;
  nutrition.protein = 12.7;
  nutrition.fat = 8.1;

  return {
    id: "diary-1",
    name: "Oats",
    mealType: MealTypes.Breakfast,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: nutrition,
    nutritionMultiplier: 1,
    quantity: 2,
    servingSize: servingSize("ss-1", 1, 50, "g"),
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

function renderCell(accessorKey: string, original: NutritionDiaryDto) {
  const column = foodDiaryTableColumns.find(
    (entry) => "accessorKey" in entry && entry.accessorKey === accessorKey,
  );
  const Cell = column?.cell as (
    context: CellContext<NutritionDiaryDto, unknown>,
  ) => React.ReactNode;

  render(
    <Cell
      row={{ original } as CellContext<NutritionDiaryDto, unknown>["row"]}
      {...({} as Omit<CellContext<NutritionDiaryDto, unknown>, "row">)}
    />,
  );
}

describe("foodDiaryTableColumns", () => {
  it("includes the expected diary columns", () => {
    expect(
      foodDiaryTableColumns.map((column) =>
        "accessorKey" in column ? column.accessorKey : column.id,
      ),
    ).toEqual([
      "actions",
      "name",
      "quantity",
      "calories",
      "carbs",
      "protein",
      "fat",
    ]);
  });

  it("renders food and recipe prefixes in the name cell", () => {
    renderCell("name", diary());
    expect(screen.getByText("(F)")).toBeInTheDocument();
    expect(screen.getByText("Oats")).toBeInTheDocument();
    expect(screen.getByText("Food entry")).toBeInTheDocument();

    renderCell(
      "name",
      diary({ diaryType: DiaryType.RecipeDiary, name: "Soup" }),
    );
    expect(screen.getByText("(R)")).toBeInTheDocument();
    expect(screen.getByText("Recipe entry")).toBeInTheDocument();
  });

  it("formats quantity using serving size units", () => {
    renderCell("quantity", diary());
    expect(screen.getByText("100 g")).toBeInTheDocument();
  });

  it("formats macro and calorie values", () => {
    renderCell("calories", diary());
    renderCell("carbs", diary());
    renderCell("protein", diary());
    renderCell("fat", diary());

    expect(screen.getByText("250.4")).toBeInTheDocument();
    expect(screen.getByText("30.2")).toBeInTheDocument();
    expect(screen.getByText("12.7")).toBeInTheDocument();
    expect(screen.getByText("8.1")).toBeInTheDocument();
  });
});
