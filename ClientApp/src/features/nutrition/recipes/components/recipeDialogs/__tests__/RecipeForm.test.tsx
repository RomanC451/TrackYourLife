import { zodResolver } from "@hookform/resolvers/zod";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import type { FormEvent } from "react";
import { useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";

import {
  recipeDetailsSchema,
  type RecipeDetailsSchema,
} from "../../../data/recipesSchemas";
import RecipeForm from "../RecipeForm";

function RecipeFormHarness({
  submitButtonText = "Create",
  pendingState = { isPending: false, isDelayedPending: false },
  handleCustomSubmit,
  onCancel = vi.fn(),
  defaultValues = { name: "Soup", portions: 2, weight: 400 },
  onSubmit = vi.fn(),
}: {
  submitButtonText?: string;
  pendingState?: { isPending: boolean; isDelayedPending: boolean };
  handleCustomSubmit?: (event: FormEvent<HTMLFormElement>) => void;
  onCancel?: () => void;
  defaultValues?: RecipeDetailsSchema;
  onSubmit?: ReturnType<typeof vi.fn>;
}) {
  const form = useForm<RecipeDetailsSchema>({
    resolver: zodResolver(recipeDetailsSchema),
    defaultValues,
  });

  const submitHandler =
    handleCustomSubmit ??
    ((event: FormEvent<HTMLFormElement>) => {
      form.handleSubmit(onSubmit)(event);
    });

  return (
    <RecipeForm
      form={form}
      handleCustomSubmit={submitHandler}
      submitButtonText={submitButtonText}
      pendingState={pendingState}
      onCancel={onCancel}
    />
  );
}

describe("RecipeForm", () => {
  it("renders recipe detail inputs and actions", () => {
    render(<RecipeFormHarness />);

    expect(screen.getByDisplayValue("Soup")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Portions")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Weight in grams")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Cancel" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Create" })).toBeInTheDocument();
  });

  it("increments portions with the plus button", () => {
    render(<RecipeFormHarness />);

    const portionsControls =
      screen.getByPlaceholderText("Portions").parentElement!;
    const [, plusButton] = portionsControls.querySelectorAll("button");

    fireEvent.click(plusButton);

    expect(screen.getByPlaceholderText("Portions")).toHaveValue(3);
  });

  it("renders a loading skeleton state", () => {
    const { container } = render(<RecipeForm.Loading />);

    expect(container.querySelectorAll('[class*="animate-pulse"]').length).toBeGreaterThan(0);
  });

  it("submits the form through handleCustomSubmit", () => {
    const handleCustomSubmit = vi.fn((event) => event.preventDefault());

    render(<RecipeFormHarness handleCustomSubmit={handleCustomSubmit} />);

    fireEvent.click(screen.getByRole("button", { name: "Create" }));
    expect(handleCustomSubmit).toHaveBeenCalledTimes(1);
  });

  it("calls onCancel when cancel is clicked", () => {
    const onCancel = vi.fn();

    render(<RecipeFormHarness onCancel={onCancel} />);

    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));
    expect(onCancel).toHaveBeenCalledTimes(1);
  });

  it("decrements portions and updates weight", () => {
    render(<RecipeFormHarness />);

    const portionsControls =
      screen.getByPlaceholderText("Portions").parentElement!;
    const [minusButton] = portionsControls.querySelectorAll("button");

    fireEvent.click(minusButton);
    expect(screen.getByPlaceholderText("Portions")).toHaveValue(1);

    fireEvent.change(screen.getByPlaceholderText("Weight in grams"), {
      target: { value: "500" },
    });
    expect(screen.getByPlaceholderText("Weight in grams")).toHaveValue(500);
  });

  it("disables submit while a mutation is pending", () => {
    render(
      <RecipeFormHarness
        pendingState={{ isPending: true, isDelayedPending: false }}
      />,
    );

    expect(screen.getByRole("button", { name: "Create" })).toBeDisabled();
  });

  it("shows a validation error when the name is empty", async () => {
    const onSubmit = vi.fn();

    render(
      <RecipeFormHarness
        defaultValues={{ name: "", portions: 2, weight: 400 }}
        onSubmit={onSubmit}
      />,
    );

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: "Create" }));
    });

    expect(await screen.findByText("Name is required")).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("shows a validation error when portions are below 1", async () => {
    const onSubmit = vi.fn();

    render(
      <RecipeFormHarness
        defaultValues={{ name: "Soup", portions: 0, weight: 400 }}
        onSubmit={onSubmit}
      />,
    );

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: "Create" }));
    });

    expect(
      await screen.findByText("Portions must be at least 1"),
    ).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("shows a validation error when weight is below 1", async () => {
    const onSubmit = vi.fn();

    render(
      <RecipeFormHarness
        defaultValues={{ name: "Soup", portions: 2, weight: 0 }}
        onSubmit={onSubmit}
      />,
    );

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: "Create" }));
    });

    expect(
      await screen.findByText("Weight must be at least 1"),
    ).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("updates the recipe name when the input changes", () => {
    render(<RecipeFormHarness />);

    fireEvent.change(screen.getByPlaceholderText("Recipe name"), {
      target: { value: "Stew" },
    });

    expect(screen.getByDisplayValue("Stew")).toBeInTheDocument();
  });

  it("treats an empty portions input as zero", () => {
    render(<RecipeFormHarness />);

    fireEvent.change(screen.getByPlaceholderText("Portions"), {
      target: { value: "" },
    });

    expect(screen.getByPlaceholderText("Portions")).toHaveValue(null);
  });

  it("submits valid data through the resolver", async () => {
    const onSubmit = vi.fn();

    render(<RecipeFormHarness onSubmit={onSubmit} />);

    await act(async () => {
      fireEvent.click(screen.getByRole("button", { name: "Create" }));
    });

    await waitFor(() => {
      expect(onSubmit).toHaveBeenCalledWith(
        { name: "Soup", portions: 2, weight: 400 },
        expect.anything(),
      );
    });
  });
});
