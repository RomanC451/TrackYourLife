import { render, screen } from "@testing-library/react";
import { FormProvider, useForm } from "react-hook-form";
import { describe, expect, it, vi } from "vitest";

const { mockUseQuery } = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@/components/input-inner-tags", () => ({
  default: ({
    initialTags,
    placeholder,
  }: {
    initialTags: { text: string }[];
    placeholder: string;
  }) => (
    <div data-testid="inner-tags">
      {initialTags.map((tag) => tag.text).join(",")}
      <span>{placeholder}</span>
    </div>
  ),
}));

import { MuscleGroupSelect } from "../MuscleGroupSelect";

function Wrapper({ muscleGroups = ["chest"] }: { muscleGroups?: string[] }) {
  const form = useForm({ defaultValues: { muscleGroups } });
  return (
    <FormProvider {...form}>
      <MuscleGroupSelect />
    </FormProvider>
  );
}

describe("MuscleGroupSelect", () => {
  it("renders muscle group tags from form values", () => {
    mockUseQuery.mockReturnValue({
      data: [{ id: "1", name: "Chest", subgroups: [] }],
    });

    render(<Wrapper muscleGroups={["chest", "triceps"]} />);

    expect(screen.getByText("Muscle Groups")).toBeInTheDocument();
    expect(screen.getByTestId("inner-tags")).toHaveTextContent("chest,triceps");
    expect(
      screen.getByText("Select a muscle group"),
    ).toBeInTheDocument();
  });
});
