import { useState } from "react";
import { useFormContext } from "react-hook-form";

import Combobox from "@/components/ui/combobox";
import { ExerciseSetType } from "@/services/openapi/api";

import { ExerciseFormSchema } from "../../../data/exercisesSchemas";
import { exerciseSetTypes } from "./ExerciseSetRow";

function SetTypeDropdownMenu() {
  const { setValue, watch } = useFormContext<ExerciseFormSchema>();

  const [currentSetType, setCurrentSetType] = useState<
    ExerciseSetType | undefined
  >(watch("exerciseSets")?.[0]?.type ?? ExerciseSetType.Weight);

  function handleSetTypeChange(type: ExerciseSetType) {
    setValue("exerciseSets.0.type", type);
    setCurrentSetType(type);

    const sets = watch("exerciseSets");
    for (const [index] of sets.entries()) {
      setValue(`exerciseSets.${index}.type`, type);
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
