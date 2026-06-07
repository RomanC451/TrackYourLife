import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import { Gender } from "@/services/openapi";
import GenderFormSelect from "../GenderFormSelect";

const schema = z.object({ gender: z.nativeEnum(Gender) });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { gender: Gender.Male },
  });

  return (
    <Form {...form}>
      <GenderFormSelect />
    </Form>
  );
}

describe("GenderFormSelect", () => {
  it("renders the gender select", () => {
    render(<Harness />);
    expect(screen.getByText("Gender")).toBeInTheDocument();
  });
});
