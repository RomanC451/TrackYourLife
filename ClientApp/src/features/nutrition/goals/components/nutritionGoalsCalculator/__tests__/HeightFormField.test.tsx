import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import HeightFormField from "../HeightFormField";

const schema = z.object({ height: z.number() });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { height: 180 },
  });

  return (
    <Form {...form}>
      <HeightFormField />
    </Form>
  );
}

describe("HeightFormField", () => {
  it("renders the height input", () => {
    render(<Harness />);
    expect(screen.getByText("Height (cm)")).toBeInTheDocument();
    expect(screen.getByDisplayValue("180")).toBeInTheDocument();
  });
});
