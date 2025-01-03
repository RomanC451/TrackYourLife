import { Control, Path } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { cn } from "@/lib/utils";
import { MealTypes } from "@/services/openapi";

type MealTypeMenuProps<T extends { mealType: MealTypes }> = {
  control: Control<T>;
  className?: string;
};

const MealTypeFormField = <T extends { mealType: MealTypes }>({
  control,
  className,
}: MealTypeMenuProps<T>) => {
  return (
    <div className={cn("", className)}>
      <FormField
        control={control}
        name={"mealType" as Path<T>}
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
                {Object.values(MealTypes).map((meal, index) => (
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
