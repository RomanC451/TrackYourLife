import { fireEvent, render, screen } from "@testing-library/react";
import { FormProvider, useForm } from "react-hook-form";
import { beforeAll, describe, expect, it, vi } from "vitest";

class ResizeObserverMock {
  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  vi.stubGlobal("ResizeObserver", ResizeObserverMock);
});

import { TooltipProvider } from "@/components/ui/tooltip";
import { exercise } from "@/features/trainings/__tests__/fixtures";
import { Difficulty } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { TrainingFormSchema } from "../../../../data/trainingsSchemas";

const { mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
  useNavigate: () => vi.fn(),
}));

vi.mock("../SelectExercisesList", () => {
  const List = ({
    exercises,
    selectedExercises,
    onSelect,
  }: {
    exercises: { id: string; name: string }[];
    selectedExercises: { id: string }[];
    onSelect: (exercise: { id: string; name: string }) => void;
  }) => (
    <div data-testid="exercise-list">
      {exercises.map((exercise) => (
        <button
          key={exercise.id}
          type="button"
          onClick={() => onSelect(exercise)}
        >
          {exercise.name}
        </button>
      ))}
      <span data-testid="selected-count">{selectedExercises.length}</span>
    </div>
  );
  List.Loading = () => <div data-testid="list-loading" />;
  List.Empty = () => <div data-testid="list-empty" />;
  return { default: List };
});

vi.mock(
  "@/features/trainings/exercises/components/common/MuscleGroupsFilter",
  () => ({
    default: ({
      selectedMuscleGroup,
      setSelectedMuscleGroup,
    }: {
      selectedMuscleGroup: string;
      setSelectedMuscleGroup: (value: string) => void;
    }) => (
      <select
        aria-label="Muscle group filter"
        value={selectedMuscleGroup}
        onChange={(e) => setSelectedMuscleGroup(e.target.value)}
      >
        <option value="">All</option>
        <option value="chest">chest</option>
        <option value="legs">legs</option>
      </select>
    ),
  }),
);

import SelectExercisesFormStep from "../SelectExercisesFormStep";

const allExercises = [
  exercise("ex-1", { name: "Bench press", muscleGroups: ["chest"] }),
  exercise("ex-2", { name: "Squat", muscleGroups: ["legs"] }),
  exercise("ex-3", { name: "Chest fly", muscleGroups: ["chest"] }),
];

function Harness({
  defaultExercises = [] as TrainingFormSchema["exercises"],
}: {
  defaultExercises?: TrainingFormSchema["exercises"];
}) {
  const form = useForm<TrainingFormSchema>({
    defaultValues: {
      name: "Workout",
      muscleGroups: ["chest"],
      difficulty: Difficulty.Easy,
      duration: 60,
      restSeconds: 60,
      exercises: defaultExercises,
    },
  });

  return (
    <TooltipProvider>
      <FormProvider {...form}>
        <SelectExercisesFormStep onCancel={vi.fn()} setStep={vi.fn()} />
      </FormProvider>
    </TooltipProvider>
  );
}

describe("SelectExercisesFormStep", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({ data: allExercises });
  });

  it("renders exercises and filters by search and muscle group", () => {
    render(<Harness />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByText("Step 1: Select Exercises")).toBeInTheDocument();
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("Squat")).toBeInTheDocument();

    fireEvent.change(screen.getByPlaceholderText("Search exercises..."), {
      target: { value: "bench" },
    });
    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.queryByText("Squat")).not.toBeInTheDocument();

    fireEvent.change(screen.getByPlaceholderText("Search exercises..."), {
      target: { value: "" },
    });
    fireEvent.change(screen.getByLabelText("Muscle group filter"), {
      target: { value: "legs" },
    });
    expect(screen.getByText("Squat")).toBeInTheDocument();
    expect(screen.queryByText("Bench press")).not.toBeInTheDocument();
  });

  it("toggles exercise selection in the form", () => {
    render(<Harness />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByText("Bench press"));
    expect(screen.getByText("1 exercises selected")).toBeInTheDocument();
    expect(screen.getByTestId("selected-count")).toHaveTextContent("1");

    fireEvent.click(screen.getByText("Bench press"));
    expect(screen.getByText("0 exercises selected")).toBeInTheDocument();
  });

  it("ignores selection for loading or deleting exercises", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: [
        exercise("ex-1", { name: "Loading", isLoading: true }),
        exercise("ex-2", { name: "Deleting", isDeleting: true }),
      ],
    });

    render(<Harness />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByText("Loading"));
    fireEvent.click(screen.getByText("Deleting"));
    expect(screen.getByText("0 exercises selected")).toBeInTheDocument();
  });

  it("prunes exercises that no longer exist in the query data", () => {
    render(
      <Harness
        defaultExercises={[
          exercise("ex-1"),
          exercise("removed", { name: "Removed exercise" }),
        ]}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText("1 exercises selected")).toBeInTheDocument();
    expect(screen.queryByText("Removed exercise")).not.toBeInTheDocument();
  });
});

describe("SelectExercisesFormStep.Loading", () => {
  it("renders loading and empty variants", () => {
    const setSearchQuery = vi.fn();

    const { rerender } = render(
      <TooltipProvider>
        <SelectExercisesFormStep.Loading
          searchQuery=""
          setSearchQuery={setSearchQuery}
        />
      </TooltipProvider>,
    );

    expect(screen.getByText("Step 1: Select Exercises")).toBeInTheDocument();
    fireEvent.change(screen.getByPlaceholderText("Search exercises..."), {
      target: { value: "squat" },
    });
    expect(setSearchQuery).toHaveBeenCalledWith("squat");

    rerender(
      <TooltipProvider>
        <SelectExercisesFormStep.Loading
          searchQuery=""
          setSearchQuery={setSearchQuery}
          empty
        />
      </TooltipProvider>,
    );
    expect(screen.getByText("0 exercises selected")).toBeInTheDocument();
  });
});
