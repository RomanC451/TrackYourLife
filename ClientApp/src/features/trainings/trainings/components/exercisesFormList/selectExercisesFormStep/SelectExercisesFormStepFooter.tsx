import { Button } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { ExerciseDto } from "@/services/openapi";

import { ExercisesFormStep } from "../ExercisesFormList";

function SelectExercisesFormStepFooter({
  selectedExercises,
  onCancel,
  setStep,
}: {
  selectedExercises: ExerciseDto[];
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
}) {
  return (
    <div className="flex items-center justify-between">
      <p className="text-sm text-muted-foreground">
        {selectedExercises.length} exercises selected
      </p>
      <div className="flex gap-2">
        <Button variant="outline" type="button" onClick={onCancel}>
          Cancel
        </Button>

        {selectedExercises.length === 0 ? (
          <Tooltip>
            <TooltipTrigger type="button" asChild>
              <Button type="button" className="opacity-50 hover:bg-primary">
                Next: Order Exercises
              </Button>
            </TooltipTrigger>
            <TooltipContent className="bg-card-secondary" side="bottom">
              You must select at least one exercise
            </TooltipContent>
          </Tooltip>
        ) : (
          <Button onClick={() => setStep("order")} type="button">
            Next: Order Exercises
          </Button>
        )}
      </div>
    </div>
  );
}

export default SelectExercisesFormStepFooter;
