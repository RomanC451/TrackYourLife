import { render, screen } from "@testing-library/react";
import type { ColumnFiltersState, SortingState } from "@tanstack/react-table";
import { useState } from "react";
import { describe, expect, it, vi } from "vitest";

import useTable from "@/features/nutrition/common/hooks/useTable";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { DiaryType, MealTypes, type NutritionDiaryDto } from "@/services/openapi";

import { foodDiaryTableColumns } from "../foodDiaryTableColumns";
import FoodDiaryTable from "../FoodDiaryTable";

const mockUseCustomQuery = vi.fn();

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../FoodDiaryTableRowActionsMenu", () => ({
  default: () => <div data-testid="row-actions" />,
}));

vi.mock("@/components/ui/hybrid-tooltip", () => ({
  HybridTooltip: ({ children }: { children: React.ReactNode }) => children,
  HybridTooltipTrigger: ({ children }: { children: React.ReactNode }) =>
    children,
  HybridTooltipContent: ({ children }: { children: React.ReactNode }) =>
    children,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

function diaryEntry(id: string, mealType: MealTypes, name: string): NutritionDiaryDto {
  const nutrition = createEmptyNutritionalContent();
  nutrition.energy = { unit: "calories", value: 250 };
  nutrition.carbohydrates = 30;
  nutrition.protein = 12;
  nutrition.fat = 8;

  return {
    id,
    name,
    mealType,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: nutrition,
    nutritionMultiplier: 1,
    quantity: 1,
    isLoading: false,
    isDeleting: false,
  };
}

describe("FoodDiaryTable", () => {
  it("renders meal sections and totals from diary data", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [
              diaryEntry("b-1", MealTypes.Breakfast, "Oats"),
            ],
            [MealTypes.Lunch]: [],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [],
          },
        },
      },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodDiaryTable
        date="2026-06-05"
        setDate={vi.fn()}
        disabled={false}
      />,
    );

    expect(screen.getByText("Food history")).toBeInTheDocument();
    expect(screen.getByText("Breakfast")).toBeInTheDocument();
    expect(screen.getByText("Lunch")).toBeInTheDocument();
    expect(screen.getByText("Oats")).toBeInTheDocument();
    expect(screen.getByText("Total")).toBeInTheDocument();
  });

  it("renders a loading state", () => {
    render(<FoodDiaryTable.Loading />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("shows delayed pending skeleton rows", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [
              diaryEntry("b-1", MealTypes.Breakfast, "Oats"),
            ],
            [MealTypes.Lunch]: [],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [],
          },
        },
      },
      pendingState: { isPending: true, isDelayedPending: true },
    });

    const { container } = render(
      <FoodDiaryTable date="2026-06-05" setDate={vi.fn()} disabled={false} />,
    );

    expect(container.querySelector(".h-\\[16px\\]")).toBeInTheDocument();
  });

  it("aggregates totals across multiple meals", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [
              diaryEntry("b-1", MealTypes.Breakfast, "Oats"),
            ],
            [MealTypes.Lunch]: [
              diaryEntry("l-1", MealTypes.Lunch, "Chicken"),
            ],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [],
          },
        },
      },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    render(
      <FoodDiaryTable date="2026-06-05" setDate={vi.fn()} disabled={false} />,
    );

    expect(screen.getByText("Chicken")).toBeInTheDocument();
    expect(screen.getAllByText("250").length).toBeGreaterThan(1);
  });

  it("renders header controls", () => {
    const setDate = vi.fn();

    render(
      <FoodDiaryTable.Header
        date="2026-06-05"
        setDate={setDate}
        table={
          {
            getAllColumns: () => [],
          } as never
        }
        disabled={false}
      />,
    );

    expect(screen.getByText("Food history")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /view/i })).toBeInTheDocument();
  });

  it("shows immediate pending placeholder rows", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [],
            [MealTypes.Lunch]: [],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [],
          },
        },
      },
      pendingState: { isPending: true, isDelayedPending: false },
    });

    const { container } = render(
      <FoodDiaryTable date="2026-06-05" setDate={vi.fn()} disabled={false} />,
    );

    expect(container.querySelectorAll("tbody tr").length).toBeGreaterThan(0);
    expect(screen.queryByText("Oats")).not.toBeInTheDocument();
  });

  it("dims rows that are being deleted", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          diaries: {
            [MealTypes.Breakfast]: [
              {
                ...diaryEntry("b-1", MealTypes.Breakfast, "Oats"),
                isDeleting: true,
              },
            ],
            [MealTypes.Lunch]: [],
            [MealTypes.Dinner]: [],
            [MealTypes.Snacks]: [],
          },
        },
      },
      pendingState: { isPending: false, isDelayedPending: false },
    });

    const { container } = render(
      <FoodDiaryTable date="2026-06-05" setDate={vi.fn()} disabled={false} />,
    );

    expect(container.querySelector(".opacity-50")).toBeInTheDocument();
  });
});

function TableBodyHarness({
  entries,
  pendingState,
}: {
  entries: ReturnType<typeof diaryEntry>[];
  pendingState: { isPending: boolean; isDelayedPending: boolean };
}) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] = useState({});
  const [rowSelection, setRowSelection] = useState({});

  const table = useTable({
    data: entries,
    columns: foodDiaryTableColumns,
    setSorting,
    setColumnFilters,
    sorting,
    columnFilters,
    columnVisibility,
    setColumnVisibility,
    rowSelection,
    onRowSelectionChange: setRowSelection,
  });

  return (
    <table>
      <tbody>
        <FoodDiaryTable.Body
          table={table as never}
          name="Breakfast"
          pendingState={pendingState}
        />
      </tbody>
    </table>
  );
}

describe("FoodDiaryTable.Body", () => {
  it("renders meal subtotals and diary rows", () => {
    render(
      <TableBodyHarness
        entries={[diaryEntry("b-1", MealTypes.Breakfast, "Oats")]}
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(screen.getByText("Breakfast")).toBeInTheDocument();
    expect(screen.getByText("Oats")).toBeInTheDocument();
    expect(screen.getAllByText("250").length).toBeGreaterThan(0);
  });

  it("renders an empty row when a meal has no entries", () => {
    const { container } = render(
      <TableBodyHarness
        entries={[]}
        pendingState={{ isPending: false, isDelayedPending: false }}
      />,
    );

    expect(screen.getByText("Breakfast")).toBeInTheDocument();
    expect(
      container.querySelector(`td[colspan="${foodDiaryTableColumns.length}"]`),
    ).toBeInTheDocument();
  });

  it("renders skeleton cells while delayed pending", () => {
    const { container } = render(
      <TableBodyHarness
        entries={[diaryEntry("b-1", MealTypes.Breakfast, "Oats")]}
        pendingState={{ isPending: true, isDelayedPending: true }}
      />,
    );

    expect(container.querySelector(".h-\\[16px\\]")).toBeInTheDocument();
    expect(screen.queryByText("Oats")).not.toBeInTheDocument();
  });
});
