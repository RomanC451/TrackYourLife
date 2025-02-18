import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import useUpdateNutritionGoalsMutation from "../../mutations/useUpdateNutritionGoalsMutation";
import useActiveNutritionGoalsQuery from "../../queries/useCaloriesGoalQuery";

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

  const { goals: activeGoals } = useActiveNutritionGoalsQuery();
  const { updateNutritionGoalsMutation } = useUpdateNutritionGoalsMutation();

  const [goals, setGoals] = useState(activeGoals);

  const toggleEdit = () => {
    setIsEditing(!isEditing);
  };

  const saveChanges = () => {
    if (goals) {
      updateNutritionGoalsMutation.mutate({
        calories: goals.calories.value,
        protein: goals.proteins.value,
        carbohydrates: goals.carbs.value,
        fats: goals.fat.value,
        force: true,
      });
    }
    setIsEditing(false);
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
              <Button onClick={saveChanges} variant="default">
                Save Changes
              </Button>
              <Button onClick={toggleEdit} variant="outline">
                Cancel
              </Button>
            </>
          ) : (
            <Button onClick={toggleEdit} variant="default">
              Edit
            </Button>
          )}
        </div>
        <Button onClick={onEdit} variant="outline" className="w-full">
          Return to Calculator
        </Button>
      </div>
    </>
  );
}

export default CalculateNutritionGoalsFormResults;
