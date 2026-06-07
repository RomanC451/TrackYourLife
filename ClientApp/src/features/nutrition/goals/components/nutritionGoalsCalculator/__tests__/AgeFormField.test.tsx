import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import AgeFormField from "../AgeFormField";

const schema = z.object({ age: z.number() });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { age: 30 },
  });

  return (
    <Form {...form}>
      <AgeFormField />
    </Form>
  );
}

describe("AgeFormField", () => {
  it("renders the age input", () => {
    render(<Harness />);
    expect(screen.getByText("Age")).toBeInTheDocument();
    expect(screen.getByDisplayValue("30")).toBeInTheDocument();
  });
});
