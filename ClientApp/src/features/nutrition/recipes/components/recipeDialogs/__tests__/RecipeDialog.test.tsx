import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import RecipeDialog from "../RecipeDialog";

const mockOnClose = vi.fn();
const mockNavigate = vi.hoisted(() => vi.fn());
const mockUseSearch = vi.hoisted(() => vi.fn(() => ({ tab: "details" as const })));
const mockUseRecipeDialog = vi.hoisted(() =>
  vi.fn(() => ({
    form: { resetSessionStorage: vi.fn() },
    handleCustomSubmit: vi.fn(),
    pendingState: { isPending: false, isDelayedPending: false },
    recipe: { name: "Soup", portions: 2, weight: 400, ingredients: [] },
    queryIsLoading: false,
  })),
);

vi.mock("@/components/ui/dialog", () => ({
  Dialog: ({
    children,
    onOpenChange,
  }: {
    children: React.ReactNode;
    onOpenChange?: (open: boolean) => void;
  }) => (
    <div>
      <button type="button" onClick={() => onOpenChange?.(false)}>
        Close dialog
      </button>
      {children}
    </div>
  ),
  DialogContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogHeader: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DialogTitle: ({ children }: { children: React.ReactNode }) => (
    <h1>{children}</h1>
  ),
  DialogDescription: ({ children }: { children: React.ReactNode }) => (
    <p>{children}</p>
  ),
}));

vi.mock("@/components/ui/tabs", () => ({
  Tabs: ({
    children,
    value,
    onValueChange,
  }: {
    children: React.ReactNode;
    value: string;
    onValueChange?: (value: string) => void;
  }) => (
    <div data-testid="tabs" data-value={value}>
      <button type="button" onClick={() => onValueChange?.("ingredients")}>
        Switch tab
      </button>
      {children}
    </div>
  ),
  TabsList: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  TabsTrigger: ({
    children,
    disabled,
  }: {
    children: React.ReactNode;
    disabled?: boolean;
  }) => (
    <button type="button" role="tab" disabled={disabled}>
      {children}
    </button>
  ),
  TabsContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  useSearch: () => mockUseSearch(),
}));

vi.mock("@/App", () => ({
  router: { history: { location: { pathname: "/nutrition/recipes/create" } } },
}));

vi.mock("../useRecipeDialog", () => ({
  default: () => mockUseRecipeDialog(),
}));

vi.mock("../RecipeForm", () => ({
  default: Object.assign(
    ({
      submitButtonText,
      onCancel,
    }: {
      submitButtonText: string;
      onCancel?: () => void;
    }) => (
      <div data-testid="recipe-form">
        {submitButtonText}
        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </div>
    ),
    {
      Loading: () => <div data-testid="recipe-form-loading" />,
    },
  ),
}));

vi.mock(
  "@/features/nutrition/recipes/components/common/RecipeMacrosCarousel",
  () => ({
    default: () => <div data-testid="macros-carousel" />,
  }),
);

vi.mock(
  "@/features/nutrition/ingredients/components/ingredientsList/IngredientsList",
  () => ({
    default: () => <div data-testid="ingredients-list" />,
  }),
);

describe("RecipeDialog", () => {
  it("renders create dialog with details tab", () => {
    render(<RecipeDialog dialogType="create" onClose={mockOnClose} />);

    expect(
      screen.getByRole("heading", { name: "Create a new recipe" }),
    ).toBeInTheDocument();
    expect(screen.getByTestId("recipe-form")).toHaveTextContent("Create");
    expect(screen.getByRole("tab", { name: "Details" })).toBeInTheDocument();
  });

  it("renders edit dialog with update button text", () => {
    render(
      <RecipeDialog
        dialogType="edit"
        recipeId="recipe-1"
        onClose={mockOnClose}
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Edit recipe" }),
    ).toBeInTheDocument();
    expect(screen.getByTestId("recipe-form")).toHaveTextContent(
      "Update and Next",
    );
  });

  it("shows ingredients tab content and navigates between tabs", () => {
    mockUseSearch.mockReturnValue({ tab: "ingredients" } as unknown as ReturnType<typeof mockUseSearch>);

    render(
      <RecipeDialog
        dialogType="edit"
        recipeId="recipe-1"
        onClose={mockOnClose}
      />,
    );

    expect(screen.getByTestId("macros-carousel")).toBeInTheDocument();
    expect(screen.getByTestId("ingredients-list")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Switch tab" }));
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/recipes/create",
      search: { tab: "ingredients" },
      replace: true,
    });
  });

  it("shows loading form while recipe query is loading", () => {
    mockUseRecipeDialog.mockReturnValue({
      form: { resetSessionStorage: vi.fn() },
      handleCustomSubmit: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
      recipe: { name: "Soup", portions: 2, weight: 400, ingredients: [] },
      queryIsLoading: true,
    });

    render(
      <RecipeDialog
        dialogType="edit"
        recipeId="recipe-1"
        onClose={mockOnClose}
      />,
    );

    expect(screen.getByTestId("recipe-form-loading")).toBeInTheDocument();
  });

  it("calls onClose when the dialog closes", () => {
    render(<RecipeDialog dialogType="create" onClose={mockOnClose} />);

    fireEvent.click(screen.getByRole("button", { name: "Close dialog" }));
    expect(mockOnClose).toHaveBeenCalled();
  });

  it("renders custom dialog title and description", () => {
    render(
      <RecipeDialog
        dialogType="create"
        dialogTitle="Custom title"
        dialogDescription="Custom description"
      />,
    );

    expect(screen.getByRole("heading", { name: "Custom title" })).toBeInTheDocument();
    expect(screen.getByText("Custom description")).toBeInTheDocument();
  });

  it("disables ingredients tab while creating a recipe", () => {
    render(<RecipeDialog dialogType="create" />);

    expect(screen.getByRole("tab", { name: "Ingredients" })).toBeDisabled();
  });

  it("clears session storage and closes when the form is cancelled", () => {
    const resetSessionStorage = vi.fn();
    mockUseRecipeDialog.mockReturnValue({
      form: { resetSessionStorage },
      handleCustomSubmit: vi.fn(),
      pendingState: { isPending: false, isDelayedPending: false },
      recipe: { name: "Soup", portions: 2, weight: 400, ingredients: [] },
      queryIsLoading: false,
    });

    render(<RecipeDialog dialogType="create" onClose={mockOnClose} />);

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));

    expect(resetSessionStorage).toHaveBeenCalled();
    expect(mockOnClose).toHaveBeenCalled();
  });
});
