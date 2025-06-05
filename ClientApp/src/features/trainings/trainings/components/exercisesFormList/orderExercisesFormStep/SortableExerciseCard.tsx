import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { GripVertical, X } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";

interface SortableExerciseCardProps {
  exercise: {
    id: string;
    name: string;
    description?: string;
    equipment?: string;
  };
  index: number;
  onRemove: () => void;
}

export function SortableExerciseCard({
  exercise,
  index,
  onRemove,
}: SortableExerciseCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: exercise.id,
    animateLayoutChanges: () => false,
  });

  const style = {
    transform: transform
      ? CSS.Transform.toString({
          ...transform,
          x: 0, // Lock x movement
          scaleX: 1,
          scaleY: 1,
        })
      : undefined,
    transition,
  };

  return (
    <Card
      ref={setNodeRef}
      style={style}
      className={cn(
        "relative mb-2 border transition-colors",
        isDragging ? "z-50 opacity-50" : "",
      )}
    >
      <CardContent className="flex items-center p-4">
        {/* Dedicated drag handle area */}
        <div
          {...attributes}
          {...listeners}
          className="flex cursor-grab items-center active:cursor-grabbing"
          style={{ touchAction: "none" }}
        >
          <GripVertical className="mr-2 size-5 cursor-grab text-muted-foreground" />
          <div className="mr-4 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10 text-sm font-medium">
            {index + 1}
          </div>
        </div>

        {/* Main content area - not draggable */}
        <div className="flex-1">
          <div className="flex items-center justify-between">
            <h3 className="font-semibold">{exercise.name}</h3>
            <div className="inline-flex">
              <div className="flex items-center gap-2">
                {exercise.equipment && (
                  <Badge variant="secondary">{exercise.equipment}</Badge>
                )}
              </div>
              <div
                className=""
                style={{ touchAction: "none" }}
                onPointerDown={(e) => e.stopPropagation()}
              >
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onRemove();
                  }}
                  className="-mr-2 cursor-pointer p-2 text-muted-foreground hover:text-foreground"
                  type="button"
                >
                  <X className="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
