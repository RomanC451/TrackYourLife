import { useState } from "react";
import { useFormContext } from "react-hook-form";
import { v4 as uuidv4 } from "uuid";

import Combobox from "@/components/ui/combobox";

import {
  ExerciseFormSchema,
  exerciseSetType,
  ExerciseSetType,
  exerciseSetTypes,
} from "../../../data/exercisesSchemas";

function SetTypeDropdownMenu() {
  const [currentSetType, setCurrentSetType] = useState<
    ExerciseSetType | undefined
  >(exerciseSetType.Weight);

  const { setValue } = useFormContext<ExerciseFormSchema>();

  function handleSetTypeChange(type: ExerciseSetType) {
    if (type == currentSetType) return;

    setCurrentSetType(type);

    const set = {
      id: uuidv4(),
      name: "Set 1",
      orderIndex: 0,
      count1: -1,
      unit1: "kg",
      count2: -1,
      unit2: "reps",
    };

    switch (type) {
      case "Weight":
        setValue("exerciseSets", [set]);
        break;
      case "Bodyweight":
        setValue("exerciseSets", [
          { ...set, count2: undefined, unit2: undefined },
        ]);
        break;
      case "Time":
        setValue("exerciseSets", [
          {
            ...set,
            count1: -1,
            unit1: "min",
            count2: undefined,
            unit2: undefined,
          },
        ]);
        break;
      case "Distance":
        setValue("exerciseSets", [
          {
            ...set,
            count1: -1,
            unit1: "km",
            count2: undefined,
            unit2: undefined,
          },
        ]);
        break;
    }
  }

  return (
    <Combobox
      data={exerciseSetTypes}
      value={currentSetType}
      onValueChange={handleSetTypeChange}
      placeholder="Select type..."
    />
  );
}

export default SetTypeDropdownMenu;
