import React from "react";
import { UseFormReturn } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "~/chadcn/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "~/chadcn/ui/select";

import { cn } from "../../../../../utils/utils";
import { mealTypes } from "../../../data/enums";
import { TFoodDiaryFormSchema } from "../../../hooks/useAddFoodDiaryForm";

type MealTypeMenuProps = {
  form: UseFormReturn<TFoodDiaryFormSchema>;
  className?: string;
};

const MealTypeFormField: React.FC<MealTypeMenuProps> = ({
  form,
  className,
}) => {
  return (
    <div className={cn("", className)}>
      <FormField
        control={form.control}
        name="mealType"
        render={({ field }) => (
          <FormItem className="">
            <FormLabel>Meal</FormLabel>
            <Select onValueChange={field.onChange} defaultValue={field.value}>
              <FormControl>
                <SelectTrigger
                  className={cn(field.value == "" ? "text-red-500" : "")}
                >
                  <SelectValue placeholder="Select a meal" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {Object.values(mealTypes).map((meal, index) => (
                  <SelectItem key={index} value={meal}>
                    {meal}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            <FormMessage />
          </FormItem>
        )}
      />
    </div>
  );
};

export default MealTypeFormField;
