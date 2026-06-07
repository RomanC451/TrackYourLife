import { fireEvent, render, screen } from "@testing-library/react";
import type { ReactNode } from "react";
import { describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";

import RecipesTableRowActionsMenu from "../RecipesTableRowActionsMenu";

const { mockMutate, mockPreloadRoute, mockOnOpenChange } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockPreloadRoute: vi.fn(),
  mockOnOpenChange: vi.fn(),
}));

vi.mock("../../../mutations/useDeleteRecipeMutation", () => ({
  default: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
    params,
  }: {
    children: ReactNode;
    to: string;
    params: { recipeId: string };
  }) => (
    <a href={`${to}/${params.recipeId}`}>{children}</a>
  ),
}));

vi.mock("@/App", () => ({
  router: { preloadRoute: mockPreloadRoute },
}));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({
    children,
    onOpenChange,
  }: {
    children: ReactNode;
    onOpenChange?: (open: boolean) => void;
  }) => {
    mockOnOpenChange.mockImplementation(onOpenChange ?? (() => {}));
    return <div>{children}</div>;
  },
  DropdownMenuTrigger: ({
    children,
    asChild,
  }: {
    children: ReactNode;
    asChild?: boolean;
  }) => (asChild ? children : <div>{children}</div>),
  DropdownMenuContent: ({ children }: { children: ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onClick,
    asChild,
  }: {
    children: ReactNode;
    onClick?: () => void;
    asChild?: boolean;
  }) =>
    asChild ? (
      children
    ) : (
      <button type="button" onClick={onClick}>
        {children}
      </button>
    ),
  DropdownMenuSeparator: () => <hr />,
}));

describe("RecipesTableRowActionsMenu", () => {
  const soup = recipe("recipe-1", "Soup");

  it("renders the actions trigger button", () => {
    render(<RecipesTableRowActionsMenu recipe={soup} />);

    expect(screen.getByRole("button", { name: "Open menu" })).toBeEnabled();
  });

  it("preloads edit route when the menu opens and removes the recipe", () => {
    render(<RecipesTableRowActionsMenu recipe={soup} />);

    mockOnOpenChange(true);
    expect(mockPreloadRoute).toHaveBeenCalledWith({
      to: "/nutrition/recipes/edit/$recipeId",
      params: { recipeId: "recipe-1" },
    });

    fireEvent.click(screen.getByRole("button", { name: "Remove" }));
    expect(mockMutate).toHaveBeenCalledWith({ recipe: soup });

    expect(screen.getByRole("link", { name: "Edit" })).toHaveAttribute(
      "href",
      "/nutrition/recipes/edit/$recipeId/recipe-1",
    );
  });
});
