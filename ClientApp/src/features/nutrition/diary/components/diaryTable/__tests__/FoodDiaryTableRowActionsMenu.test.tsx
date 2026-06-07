import { fireEvent, render, screen } from "@testing-library/react";
import type { ReactNode } from "react";
import { describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { DiaryType, MealTypes } from "@/services/openapi";

import FoodDiaryTableRowActionsMenu from "../FoodDiaryTableRowActionsMenu";

const { mockMutate, mockNavigate, mockPreloadRoute } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockNavigate: vi.fn(),
  mockPreloadRoute: vi.fn(),
}));

const mockDeleteMutation = vi.hoisted(() => ({
  mutate: mockMutate,
  isPending: false,
  isDelayedPending: false,
}));

vi.mock("../../../mutations/useDeleteNutritionDiaryMutation", () => ({
  default: () => mockDeleteMutation,
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({
    children,
    asChild,
  }: {
    children: ReactNode;
    asChild?: boolean;
  }) => (asChild ? children : <div>{children}</div>),
  DropdownMenuContent: ({ children }: { children: ReactNode }) => (
    <div data-testid="menu-content">{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onClick,
    onMouseEnter,
    onTouchStart,
  }: {
    children: ReactNode;
    onClick?: () => void;
    onMouseEnter?: () => void;
    onTouchStart?: () => void;
  }) => (
    <button
      type="button"
      onClick={onClick}
      onMouseEnter={onMouseEnter}
      onTouchStart={onTouchStart}
    >
      {children}
    </button>
  ),
  DropdownMenuSeparator: () => <hr />,
}));

vi.mock("@/components/ui/spinner", () => ({
  default: () => <div data-testid="spinner" />,
}));

describe("FoodDiaryTableRowActionsMenu", () => {
  const diary = {
    id: "diary-1",
    name: "Oats",
    mealType: MealTypes.Breakfast,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: createEmptyNutritionalContent(),
    nutritionMultiplier: 1,
    quantity: 1,
    isLoading: false,
    isDeleting: false,
  };

  it("renders the actions trigger button", () => {
    render(<FoodDiaryTableRowActionsMenu diary={diary} />);

    expect(screen.getByRole("button", { name: "Open menu" })).toBeEnabled();
  });

  it("disables the trigger while the diary entry is deleting", () => {
    render(
      <FoodDiaryTableRowActionsMenu
        diary={{ ...diary, isDeleting: true }}
      />,
    );

    expect(screen.getByRole("button", { name: "Open menu" })).toBeDisabled();
  });

  it("shows a spinner while delete is delayed pending", () => {
    mockDeleteMutation.isDelayedPending = true;

    render(<FoodDiaryTableRowActionsMenu diary={diary} />);

    expect(screen.getByTestId("spinner")).toBeInTheDocument();
    mockDeleteMutation.isDelayedPending = false;
  });

  it("navigates to edit and removes the diary entry", () => {
    render(<FoodDiaryTableRowActionsMenu diary={diary} />);

    fireEvent.mouseEnter(screen.getByRole("button", { name: "Edit" }));
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/diary/foodDiary/edit/$diaryId",
      params: { diaryId: "diary-1" },
    });

    fireEvent.click(screen.getByRole("button", { name: "Edit" }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/diary/foodDiary/edit/$diaryId",
      params: { diaryId: "diary-1" },
    });

    fireEvent.click(screen.getByRole("button", { name: "Remove" }));
    expect(mockMutate).toHaveBeenCalledWith(diary);
  });

  it("preloads the edit route on touch", () => {
    render(<FoodDiaryTableRowActionsMenu diary={diary} />);

    fireEvent.touchStart(screen.getByRole("button", { name: "Edit" }));
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/diary/foodDiary/edit/$diaryId",
      params: { diaryId: "diary-1" },
    });
  });
});
