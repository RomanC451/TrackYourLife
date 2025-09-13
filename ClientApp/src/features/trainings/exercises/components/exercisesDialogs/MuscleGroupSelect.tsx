import { Tag } from "emblor";
import { useFormContext } from "react-hook-form";

import InputInnerTags from "@/components/input-inner-tags";
import {
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";

const muscleGroups: Tag[] = [
  {
    id: "1",
    text: "Chest",
  },
  {
    id: "2",
    text: "Back",
  },
  {
    id: "3",
    text: "Legs",
  },
  {
    id: "4",
    text: "Shoulders",
  },
  {
    id: "5",
    text: "Arms",
  },
  {
    id: "6",
    text: "Core",
  },
  {
    id: "7",
    text: "Full Body",
  },
];

export function MuscleGroupSelect() {
  const form = useFormContext();

  return (
    <FormField
      control={form.control}
      name="muscleGroups"
      render={({ field }) => (
        <FormItem className="flex flex-col">
          <FormLabel>Muscle Groups</FormLabel>
          <InputInnerTags
            setTags={(newTags) => {
              field.onChange(newTags.map((tag) => tag.text));
            }}
            initialTags={field.value.map((tag: string, index: number) => ({
              id: index.toString(),
              text: tag,
            }))}
            autocompleteOptions={muscleGroups}
            placeholder="Select or add a muscle group"
          />
          <FormMessage />
        </FormItem>
      )}
    />
  );
}
