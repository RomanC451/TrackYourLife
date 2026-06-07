import { zodResolver } from "@hookform/resolvers/zod";
import { fireEvent, render, screen } from "@testing-library/react";
import { useForm, useWatch } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";
import { z } from "zod";

import { servingSize } from "@/features/nutrition/__tests__/fixtures";
import { Form } from "@/components/ui/form";
import ServingSizeFormField from "../ServingSizeFormField";

const mockOnValueChange = vi.hoisted(() => vi.fn());
const mockBlur = vi.hoisted(() => vi.fn());

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
  SelectTrigger: ({
    children,
    onClick,
  }: {
    children: React.ReactNode;
    onClick?: (event: { currentTarget: { blur: () => void } }) => void;
  }) => (
    <button
      type="button"
      onClick={() =>
        onClick?.({ currentTarget: { blur: mockBlur } } as never)
      }
    >
      {children}
    </button>
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

const schema = z.object({ servingSizeId: z.string() });

function Harness({
  servingSizes = [
    servingSize("ss-1", 1, 100, "g"),
    servingSize("ss-2", 2, 200, "g"),
  ],
}: {
  servingSizes?: ReturnType<typeof servingSize>[];
}) {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { servingSizeId: "ss-1" },
  });

  function ValueDisplay() {
    const value = useWatch({ control: form.control, name: "servingSizeId" });
    return <span data-testid="value">{value}</span>;
  }

  return (
    <Form {...form}>
      <ServingSizeFormField name="servingSizeId" servingSizes={servingSizes} />
      <ValueDisplay />
    </Form>
  );
}

describe("ServingSizeFormField", () => {
  it("renders serving size select", () => {
    render(<Harness />);
    expect(screen.getByText("Serving size")).toBeInTheDocument();
  });

  it("updates the form value when a serving size is selected", () => {
    render(<Harness />);

    fireEvent.click(screen.getByRole("button", { name: "200 g" }));

    expect(screen.getByTestId("value")).toHaveTextContent("ss-2");
  });

  it("blurs the trigger when it is clicked", () => {
    render(<Harness />);

    fireEvent.click(screen.getByRole("button", { name: "Select a serving size" }));

    expect(mockBlur).toHaveBeenCalled();
  });
});
