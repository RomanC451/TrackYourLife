import { fireEvent, render, screen } from "@testing-library/react";
import { beforeAll, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  vi.stubGlobal("ResizeObserver", ResizeObserverMock);
});

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

vi.mock("@/components/ui/shadcn-io/combobox", () => ({
  Combobox: ({
    data,
    value,
    onValueChange,
    children,
  }: {
    data: { label: string; value: string }[];
    value: string;
    onValueChange: (value: string) => void;
    children: React.ReactNode;
  }) => (
    <div>
      <select
        aria-label="Muscle Group"
        value={value}
        onChange={(e) => onValueChange(e.target.value)}
      >
        {data.map((item) => (
          <option key={item.value} value={item.value}>
            {item.label}
          </option>
        ))}
      </select>
      {children}
    </div>
  ),
  ComboboxTrigger: () => null,
  ComboboxContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  ComboboxInput: () => null,
  ComboboxList: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  ComboboxItem: () => null,
}));

import MuscleGroupsFilter from "../MuscleGroupsFilter";

describe("MuscleGroupsFilter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseQuery.mockReturnValue({
      data: [
        {
          id: "chest",
          name: "Chest",
          children: [{ id: "upper-chest", name: "Upper chest" }],
        },
        { id: "legs", name: "Legs", children: [] },
      ],
    });
  });

  it("renders muscle group options including All", () => {
    const setSelectedMuscleGroup = vi.fn();

    render(
      <MuscleGroupsFilter
        selectedMuscleGroup=""
        setSelectedMuscleGroup={setSelectedMuscleGroup}
      />,
    );

    const select = screen.getByLabelText("Muscle Group");
    expect(select).toBeInTheDocument();
    expect(screen.getByRole("option", { name: "All" })).toBeInTheDocument();
    expect(screen.getByRole("option", { name: "Chest" })).toBeInTheDocument();
    expect(
      screen.getByRole("option", {
        name: (name) => name.includes("Upper chest"),
      }),
    ).toBeInTheDocument();
    expect(screen.getByRole("option", { name: "Legs" })).toBeInTheDocument();
  });

  it("calls setSelectedMuscleGroup when a value is chosen", () => {
    const setSelectedMuscleGroup = vi.fn();

    render(
      <MuscleGroupsFilter
        selectedMuscleGroup=""
        setSelectedMuscleGroup={setSelectedMuscleGroup}
      />,
    );

    fireEvent.change(screen.getByLabelText("Muscle Group"), {
      target: { value: "Legs" },
    });
    expect(setSelectedMuscleGroup).toHaveBeenCalledWith("Legs");
  });
});
