import { useState } from "react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { getDateOnly } from "@/lib/date";

import useUpdateNutritionGoalsMutation from "../../mutations/useUpdateNutritionGoalsMutation";
import useNutritionGoalsQuery from "../../queries/useNutritionGoalQueries";

function CalculateNutritionGoalsFormResults({
  onEdit,
}: {
  onEdit: () => void;
}) {
  const [isEditing, setIsEditing] = useState(false);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setGoals((prev) => {
      if (!prev) return prev;
      return {
        ...prev,
        [name as keyof typeof prev]: {
          ...prev[name as keyof typeof prev],
          value: Number(value),
        },
      };
    });
  };

  const { goals: activeGoals } = useNutritionGoalsQuery(
    getDateOnly(new Date()),
  );
  const { updateNutritionGoalsMutation, isPending } =
    useUpdateNutritionGoalsMutation();

  const [goals, setGoals] = useState(activeGoals);

  const toggleEdit = () => {
    setIsEditing(!isEditing);
  };

  const saveChanges = () => {
    if (goals) {
      updateNutritionGoalsMutation.mutate(
        {
          calories: goals.calories.value,
          protein: goals.proteins.value,
          carbohydrates: goals.carbs.value,
          fats: goals.fat.value,
          force: true,
        },
        { onSuccess: () => setIsEditing(false) },
      );
    }
  };

  if (!goals) return null;

  return (
    <>
      <div className="grid gap-4 py-4">
        {Object.entries(goals).map(([key, goal]) => (
          <div key={key} className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor={key} className="text-right capitalize">
              {key}
            </Label>
            <Input
              id={key}
              name={key}
              type="number"
              value={goal.value}
              onChange={handleInputChange}
              className="col-span-3"
              disabled={!isEditing}
            />
          </div>
        ))}
      </div>
      <div className="space-y-2">
        <div className="flex justify-end space-x-2">
          {isEditing ? (
            <>
              <ButtonWithLoading
                isLoading={isPending.isLoading}
                disabled={!isPending.isLoaded}
                onClick={saveChanges}
                variant="default"
              >
                Save Changes
              </ButtonWithLoading>
              <Button
                onClick={toggleEdit}
                disabled={!isPending.isLoaded}
                variant="outline"
              >
                Cancel
              </Button>
            </>
          ) : (
            <Button onClick={toggleEdit} variant="default">
              Edit
            </Button>
          )}
        </div>
        <Button
          onClick={onEdit}
          disabled={!isPending.isLoaded}
          variant="outline"
          className="w-full"
        >
          Return to Calculator
        </Button>
      </div>
    </>
  );
}

export default CalculateNutritionGoalsFormResults;
