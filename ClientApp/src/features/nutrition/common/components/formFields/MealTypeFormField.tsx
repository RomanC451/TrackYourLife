import { Path, useFormContext } from "react-hook-form";
import { useLocalStorage } from "usehooks-ts";

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

type MealTypeMenuProps = {
  className?: string;
};

const MealTypeFormField = <FormSchema extends { mealType: MealTypes }>({
  className,
}: MealTypeMenuProps) => {
  const [, setMealType] = useLocalStorage("lastMealType", "");

  const form = useFormContext<FormSchema>();
  return (
    <div className={cn("", className)}>
      <FormField
        control={form.control}
        name={"mealType" as Path<FormSchema>}
        render={({ field }) => (
          <FormItem className="">
            <FormLabel>Meal</FormLabel>
            <Select
              onValueChange={(e) => {
                setMealType(e);
                field.onChange(e);
              }}
              defaultValue={field.value}
            >
              <FormControl>
                <SelectTrigger
                  className={cn(field.value == "" ? "text-red-500" : "")}
                >
                  <SelectValue placeholder="Select a meal" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {Object.values(MealTypes).map((meal) => (
                  <SelectItem key={meal} value={meal}>
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
