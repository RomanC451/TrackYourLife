import { zodResolver } from "@hookform/resolvers/zod";
import { fireEvent, render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import { MealTypes } from "@/services/openapi";
import MealTypeFormField from "../MealTypeFormField";

const mockOnValueChange = vi.hoisted(() => vi.fn());

vi.mock("@/components/ui/select", () => ({
  Select: ({
    children,
    onValueChange,
  }: {
    children: React.ReactNode;
    onValueChange?: (value: string) => void;
  }) => {
    mockOnValueChange.mockImplementation(onValueChange ?? (() => {}));
    return <div>{children}</div>;
  },
  SelectTrigger: ({ children }: { children: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
  SelectValue: ({ placeholder }: { placeholder?: string }) => (
    <span>{placeholder}</span>
  ),
  SelectContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  SelectItem: ({
    children,
    value,
  }: {
    children: React.ReactNode;
    value: string;
  }) => (
    <button type="button" onClick={() => mockOnValueChange(value)}>
      {children}
    </button>
  ),
}));

vi.mock("usehooks-ts", () => ({
  useLocalStorage: () => [null, vi.fn()],
}));

const schema = z.object({ mealType: z.nativeEnum(MealTypes) });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { mealType: MealTypes.Breakfast },
  });

  return (
    <Form {...form}>
      <MealTypeFormField />
    </Form>
  );
}

describe("MealTypeFormField", () => {
  it("renders the meal type selector", () => {
    render(<Harness />);
    expect(screen.getByText("Meal")).toBeInTheDocument();
  });

  it("updates the meal type when a new value is selected", () => {
    render(<Harness />);

    fireEvent.click(screen.getByRole("button", { name: MealTypes.Lunch }));

    expect(mockOnValueChange).toHaveBeenCalledWith(MealTypes.Lunch);
  });
});
