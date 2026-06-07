import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import { FitnessGoal } from "@/services/openapi";
import FitnessGoalFormSelect from "../FitnessGoalFormSelect";

const schema = z.object({ fitnessGoal: z.nativeEnum(FitnessGoal) });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { fitnessGoal: FitnessGoal.Maintain },
  });

  return (
    <Form {...form}>
      <FitnessGoalFormSelect />
    </Form>
  );
}

describe("FitnessGoalFormSelect", () => {
  it("renders the fitness goal select", () => {
    render(<Harness />);
    expect(screen.getByText("Goal")).toBeInTheDocument();
  });
});
