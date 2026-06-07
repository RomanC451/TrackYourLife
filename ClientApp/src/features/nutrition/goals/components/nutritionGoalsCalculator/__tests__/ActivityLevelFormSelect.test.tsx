import { zodResolver } from "@hookform/resolvers/zod";
import { render, screen } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { describe, expect, it } from "vitest";
import { z } from "zod";

import { Form } from "@/components/ui/form";
import { ActivityLevel } from "@/services/openapi";
import ActivityLevelFormSelect from "../ActivityLevelFormSelect";

const schema = z.object({ activityLevel: z.nativeEnum(ActivityLevel) });

function Harness() {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { activityLevel: ActivityLevel.Sedentary },
  });

  return (
    <Form {...form}>
      <ActivityLevelFormSelect />
    </Form>
  );
}

describe("ActivityLevelFormSelect", () => {
  it("renders the activity level select", () => {
    render(<Harness />);
    expect(screen.getByText("Activity Level")).toBeInTheDocument();
  });
});
