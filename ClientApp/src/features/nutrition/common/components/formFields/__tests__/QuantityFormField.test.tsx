import { zodResolver } from "@hookform/resolvers/zod";
import { fireEvent, render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import QuantityFormField from "../QuantityFormField";

const schema = z.object({ quantity: z.number().min(0.1) });

function Harness({ defaultQuantity = 2 }: { defaultQuantity?: number }) {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { quantity: defaultQuantity },
  });

  return (
    <Form {...form}>
      <QuantityFormField name="quantity" label="Quantity" />
    </Form>
  );
}

describe("QuantityFormField", () => {
  it("renders the quantity input with increment controls", () => {
    render(<Harness />);
    expect(screen.getByDisplayValue("2")).toBeInTheDocument();
  });

  it("increments and decrements the quantity", () => {
    render(<Harness />);
    const buttons = screen.getAllByRole("button");
    fireEvent.click(buttons[1]);
    expect(screen.getByDisplayValue("3")).toBeInTheDocument();
    fireEvent.click(buttons[0]);
    expect(screen.getByDisplayValue("2")).toBeInTheDocument();
  });

  it("clears invalid characters and empty input", () => {
    render(<Harness />);
    const input = screen.getByDisplayValue("2");

    fireEvent.change(input, { target: { value: "" } });
    expect(input).toHaveDisplayValue("");

    fireEvent.change(input, { target: { value: "2.5" } });
    expect(input).toHaveDisplayValue("2.5");

    fireEvent.change(input, { target: { value: "2.5.1" } });
    expect(input).toHaveDisplayValue("2.5");
  });

  it("does not decrement below zero", () => {
    render(<Harness defaultQuantity={0} />);
    fireEvent.click(screen.getAllByRole("button")[0]);
    expect(screen.getByDisplayValue("0")).toBeInTheDocument();
  });

  it("treats a lone decimal point as zero", () => {
    render(<Harness />);
    const input = screen.getByDisplayValue("2");

    fireEvent.change(input, { target: { value: "." } });
    expect(input).toHaveDisplayValue("");
  });

  it("preserves a trailing decimal while typing", () => {
    render(<Harness />);
    const input = screen.getByDisplayValue("2");

    fireEvent.change(input, { target: { value: "2." } });
    expect(input).toHaveDisplayValue("2.");
  });
});
