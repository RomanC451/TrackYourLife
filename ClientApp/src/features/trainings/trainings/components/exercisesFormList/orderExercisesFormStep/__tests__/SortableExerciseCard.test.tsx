import { DndContext } from "@dnd-kit/core";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";

import { SortableExerciseCard } from "../SortableExerciseCard";

function renderCard(props: React.ComponentProps<typeof SortableExerciseCard>) {
  return render(
    <DndContext>
      <SortableContext
        items={[props.exercise.id]}
        strategy={verticalListSortingStrategy}
      >
        <SortableExerciseCard {...props} />
      </SortableContext>
    </DndContext>,
  );
}

describe("SortableExerciseCard", () => {
  it("renders exercise details and order index", () => {
    renderCard({
      exercise: exercise("ex-1", {
        name: "Bench press",
        equipment: "Barbell",
      }),
      index: 2,
      onRemove: vi.fn(),
    });

    expect(screen.getByText("Bench press")).toBeInTheDocument();
    expect(screen.getByText("Barbell")).toBeInTheDocument();
    expect(screen.getByText("3")).toBeInTheDocument();
  });

  it("calls onRemove when remove button is clicked", () => {
    const onRemove = vi.fn();

    renderCard({
      exercise: exercise("ex-1", { name: "Squat" }),
      index: 0,
      onRemove,
    });

    fireEvent.click(document.querySelector(".lucide-x")!.closest("button")!);
    expect(onRemove).toHaveBeenCalledOnce();
  });

  it("omits equipment badge when not provided", () => {
    renderCard({
      exercise: exercise("ex-1", { name: "Push-up", equipment: undefined }),
      index: 0,
      onRemove: vi.fn(),
    });

    expect(screen.getByText("Push-up")).toBeInTheDocument();
    expect(screen.queryByText("No equipment")).not.toBeInTheDocument();
  });
});
