import { act, fireEvent, render, screen } from "@testing-library/react";

import type { UseFormReturn } from "react-hook-form";

import { describe, expect, it, vi } from "vitest";



import { food, ingredient, recipe } from "@/features/nutrition/__tests__/fixtures";

import type { IngredientSchema } from "../../../data/ingredientsSchemas";



import IngredientDialog from "../IngredientDialog";



const mockOnSuccess = vi.fn();

const mockOnClose = vi.fn();

const mockMutate = vi.fn();



let capturedForm: UseFormReturn<IngredientSchema> | undefined;

let addIngredientOptions:

  | {

      setError: (

        name: keyof IngredientSchema,

        error: { message?: string },

        options?: { shouldFocus: boolean },

      ) => void;

    }

  | undefined;

let updateIngredientOptions: typeof addIngredientOptions;



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



vi.mock("../IngredientForm", () => ({

  default: ({

    submitButtonText,

    form,

  }: {

    submitButtonText: string;

    form: UseFormReturn<IngredientSchema>;

  }) => {

    capturedForm = form;

    return <button type="submit">{submitButtonText}</button>;

  },

}));



vi.mock("../../../mutations/useAddIngredientMutation", () => ({

  default: (options: typeof addIngredientOptions) => {

    addIngredientOptions = options;

    return { mutate: mockMutate, pendingState: { isPending: false } };

  },

}));



vi.mock("../../../mutations/useUpdateIngredientMutation", () => ({

  default: (options: typeof updateIngredientOptions) => {

    updateIngredientOptions = options;

    return { mutate: mockMutate, pendingState: { isPending: false } };

  },

}));



describe("IngredientDialog", () => {

  const oats = food("food-1", "Oats");

  const soup = recipe("recipe-1", "Soup");



  it("renders create ingredient dialog", () => {

    render(

      <IngredientDialog

        dialogType="create"

        food={oats}

        recipe={soup}

        servingSizes={oats.servingSizes}

      />,

    );



    expect(

      screen.getByRole("heading", { name: "Create a new ingredient" }),

    ).toBeInTheDocument();

    expect(screen.getByRole("button", { name: "Create" })).toBeInTheDocument();

  });



  it("renders edit ingredient dialog", () => {

    render(

      <IngredientDialog

        dialogType="edit"

        food={oats}

        recipe={soup}

        servingSizes={oats.servingSizes}

        ingredient={ingredient("ing-1", oats)}

      />,

    );



    expect(

      screen.getByRole("heading", { name: "Edit an ingredient" }),

    ).toBeInTheDocument();

    expect(screen.getByRole("button", { name: "Update" })).toBeInTheDocument();

  });



  it("resets the form and calls onClose when the dialog closes", () => {

    render(

      <IngredientDialog

        dialogType="create"

        food={oats}

        recipe={soup}

        servingSizes={oats.servingSizes}

        onClose={mockOnClose}

      />,

    );



    const resetSpy = vi.spyOn(capturedForm!, "reset");

    fireEvent.click(screen.getByRole("button", { name: "Close dialog" }));

    expect(resetSpy).toHaveBeenCalled();

    expect(mockOnClose).toHaveBeenCalled();

  });



  it("wires add-ingredient mutation setError to the form", async () => {

    render(

      <IngredientDialog

        dialogType="create"

        food={oats}

        recipe={soup}

        servingSizes={oats.servingSizes}

        onSuccess={mockOnSuccess}

      />,

    );



    await act(async () => {

      addIngredientOptions?.setError(

        "servingSizeId",

        { message: "Choose a different serving size" },

        { shouldFocus: true },

      );

    });



    expect(capturedForm?.getFieldState("servingSizeId").error?.message).toBe(

      "Choose a different serving size",

    );

  });



  it("wires update-ingredient mutation setError to the form", async () => {

    render(

      <IngredientDialog

        dialogType="edit"

        food={oats}

        recipe={soup}

        servingSizes={oats.servingSizes}

        ingredient={ingredient("ing-1", oats)}

      />,

    );



    await act(async () => {

      updateIngredientOptions?.setError(

        "quantity",

        { message: "Quantity is invalid" },

        { shouldFocus: true },

      );

    });



    expect(capturedForm?.getFieldState("quantity").error?.message).toBe(

      "Quantity is invalid",

    );

  });

});


