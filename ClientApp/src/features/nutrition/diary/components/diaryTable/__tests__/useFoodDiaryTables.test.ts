import { renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { DiaryType, MealTypes } from "@/services/openapi";

import useFoodDiaryTables from "../useFoodDiaryTables";

const mockUseCustomQuery = vi.fn();

vi.mock("../FoodDiaryTableRowActionsMenu", () => ({
  default: () => null,
}));

vi.mock("@/components/ui/hybrid-tooltip", () => ({
  HybridTooltip: ({ children }: { children: React.ReactNode }) => children,
  HybridTooltipTrigger: ({ children }: { children: React.ReactNode }) =>
    children,
  HybridTooltipContent: ({ children }: { children: React.ReactNode }) =>
    children,
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

function diaryEntry(id: string, mealType: MealTypes) {
  return {
    id,
    name: "Oats",
    mealType,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: createEmptyNutritionalContent(),
    nutritionMultiplier: 1,
    quantity: 1,
    isLoading: false,
    isDeleting: false,
  };
}

describe("useFoodDiaryTables", () => {
  it("creates one table per meal type from query data", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [diaryEntry("b-1", MealTypes.Breakfast)],
            [MealTypes.Lunch]: [diaryEntry("l-1", MealTypes.Lunch)],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [diaryEntry("s-1", MealTypes.Snacks)],
          },
        },
      },
      pendingState: { isPending: false },
    });

    const { result } = renderHook(() => useFoodDiaryTables("2026-06-05"));

    expect(
      result.current.tables.breakfastTable.getRowModel().rows,
    ).toHaveLength(1);
    expect(result.current.tables.lunchTable.getRowModel().rows).toHaveLength(1);
    expect(result.current.tables.dinnerTable.getRowModel().rows).toHaveLength(0);
    expect(result.current.tables.snacksTable.getRowModel().rows).toHaveLength(1);
    expect(result.current.pendingState.isPending).toBe(false);
  });

  it("uses empty arrays when query data is missing", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: undefined },
      pendingState: { isPending: true },
    });

    const { result } = renderHook(() => useFoodDiaryTables("2026-06-05"));

    expect(
      result.current.tables.breakfastTable.getRowModel().rows,
    ).toHaveLength(0);
    expect(result.current.pendingState.isPending).toBe(true);
  });
});
