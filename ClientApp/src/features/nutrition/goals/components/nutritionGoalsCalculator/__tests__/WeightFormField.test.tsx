import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import WeightFormField from "../WeightFormField";

const schema = z.object({ weight: z.number() });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { weight: 75 },
  });

  return (
    <Form {...form}>
      <WeightFormField />
    </Form>
  );
}

describe("WeightFormField", () => {
  it("renders the weight input", () => {
    render(<Harness />);
    expect(screen.getByText("Weight (kg)")).toBeInTheDocument();
    expect(screen.getByDisplayValue("75")).toBeInTheDocument();
  });
});
