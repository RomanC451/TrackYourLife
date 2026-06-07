import { fireEvent, render, screen } from "@testing-library/react";
import { FormProvider, useForm, useWatch } from "react-hook-form";
import { beforeAll, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  vi.stubGlobal("ResizeObserver", ResizeObserverMock);
});

import { ExerciseFormSchema } from "../../../../data/exercisesSchemas";
import SetTypeDropdownMenu from "../SetTypeDropdownMenu";

vi.mock("uuid", () => ({ v4: () => "generated-set-id" }));

vi.mock("@/components/ui/combobox", () => ({
  default: ({
    data,
    value,
    onValueChange,
  }: {
    data: { label: string; value: string }[];
    value?: string;
    onValueChange?: (value: string) => void;
  }) => (
    <select
      aria-label="Set type"
      value={value}
      onChange={(e) => onValueChange?.(e.target.value)}
    >
      {data.map((item) => (
        <option key={item.value} value={item.value}>
          {item.label}
        </option>
      ))}
    </select>
  ),
}));

function SetsDisplay() {
  const sets = useWatch({ name: "exerciseSets" });
  return <pre data-testid="sets">{JSON.stringify(sets)}</pre>;
}

function Harness() {
  const form = useForm<ExerciseFormSchema>({
    defaultValues: {
      id: "",
      name: "Bench press",
      muscleGroups: ["chest"],
      difficulty: "Easy",
      description: "",
      equipment: "",
      videoUrl: "",
      pictureUrl: "",
      exerciseSets: [
        {
          id: "set-1",
          name: "Set 1",
          orderIndex: 0,
          count1: 60,
          unit1: "kg",
          count2: 10,
          unit2: "reps",
        },
      ],
    },
  });

  return (
    <FormProvider {...form}>
      <SetTypeDropdownMenu />
      <SetsDisplay />
    </FormProvider>
  );
}

describe("SetTypeDropdownMenu", () => {
  it("updates exercise sets when set type changes", () => {
    render(<Harness />);

    fireEvent.change(screen.getByLabelText("Set type"), {
      target: { value: "Bodyweight" },
    });

    const sets = JSON.parse(screen.getByTestId("sets").textContent!);
    expect(sets).toHaveLength(1);
    expect(sets[0]).toMatchObject({
      id: "generated-set-id",
      unit1: "kg",
    });
    expect(sets[0].count2).toBeUndefined();
    expect(sets[0].unit2).toBeUndefined();
  });

  it("sets time-based defaults", () => {
    render(<Harness />);

    fireEvent.change(screen.getByLabelText("Set type"), {
      target: { value: "Time" },
    });

    const sets = JSON.parse(screen.getByTestId("sets").textContent!);
    expect(sets[0].unit1).toBe("min");
    expect(sets[0].count2).toBeUndefined();
    expect(sets[0].unit2).toBeUndefined();
  });

  it("sets distance-based defaults", () => {
    render(<Harness />);

    fireEvent.change(screen.getByLabelText("Set type"), {
      target: { value: "Distance" },
    });

    const sets = JSON.parse(screen.getByTestId("sets").textContent!);
    expect(sets[0].unit1).toBe("km");
    expect(sets[0].count2).toBeUndefined();
    expect(sets[0].unit2).toBeUndefined();
  });

  it("ignores selecting the same set type twice", () => {
    render(<Harness />);

    fireEvent.change(screen.getByLabelText("Set type"), {
      target: { value: "Weight" },
    });

    const sets = JSON.parse(screen.getByTestId("sets").textContent!);
    expect(sets[0].id).toBe("set-1");
  });
});
