import { zodResolver } from "@hookform/resolvers/zod";
import { fireEvent, render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";

import { food, servingSize } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import { ingredientSchema, type IngredientSchema } from "../../../data/ingredientsSchemas";
import IngredientForm from "../IngredientForm";

vi.mock(
  "@/features/nutrition/common/components/formFields/QuantityFormField",
  () => ({ default: () => <div data-testid="quantity-field" /> }),
);

vi.mock(
  "@/features/nutrition/common/components/formFields/ServingSizeFormField",
  () => ({ default: () => <div data-testid="serving-size-field" /> }),
);

vi.mock(
  "@/features/nutrition/common/components/macros/MacrosDialogHeader",
  () => ({
    default: ({ nutritionMultiplier }: { nutritionMultiplier: number }) => (
      <div data-testid="macros-header">{nutritionMultiplier}</div>
    ),
  }),
);

vi.mock("@/features/nutrition/common/components/NutritionalInfoAccordion", () => ({
  default: () => <div data-testid="nutritional-info" />,
}));

function IngredientFormHarness({
  handleCustomSubmit = vi.fn((event) => event.preventDefault()),
  pendingState = { isPending: false, isDelayedPending: false },
  submitButtonText = "Add ingredient",
}: {
  handleCustomSubmit?: (event: React.FormEvent<HTMLFormElement>) => void;
  pendingState?: { isPending: boolean; isDelayedPending: boolean };
  submitButtonText?: string;
}) {
  const oats = food("food-1", "Oats", "Brand");
  oats.servingSizes = [servingSize("ss-1", 1.5)];
  oats.nutritionalContents = createEmptyNutritionalContent();

  const form = useForm<IngredientSchema>({
    resolver: zodResolver(ingredientSchema),
    defaultValues: {
      foodId: "food-1",
      servingSizeId: "ss-1",
      quantity: 2,
    },
  });

  return (
    <IngredientForm
      form={form}
      food={oats}
      handleCustomSubmit={handleCustomSubmit}
      pendingState={pendingState}
      submitButtonText={submitButtonText}
    />
  );
}

describe("IngredientForm", () => {
  it("renders food badge, macros, and submit button", () => {
    render(<IngredientFormHarness />);

    expect(screen.getByText("Oats - Brand")).toBeInTheDocument();
    expect(screen.getByTestId("macros-header")).toHaveTextContent("3");
    expect(screen.getByTestId("nutritional-info")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add ingredient" })).toBeEnabled();
  });

  it("submits through handleCustomSubmit", () => {
    const handleCustomSubmit = vi.fn((event) => event.preventDefault());

    render(<IngredientFormHarness handleCustomSubmit={handleCustomSubmit} />);

    fireEvent.click(screen.getByRole("button", { name: "Add ingredient" }));
    expect(handleCustomSubmit).toHaveBeenCalledTimes(1);
  });

  it("disables submit while pending", () => {
    render(
      <IngredientFormHarness
        pendingState={{ isPending: true, isDelayedPending: false }}
      />,
    );

    expect(screen.getByRole("button", { name: /add ingredient/i })).toBeDisabled();
  });

  it("renders quantity and serving size fields", () => {
    render(<IngredientFormHarness />);

    expect(screen.getByTestId("quantity-field")).toBeInTheDocument();
    expect(screen.getByTestId("serving-size-field")).toBeInTheDocument();
  });
});
