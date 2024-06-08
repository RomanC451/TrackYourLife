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
import { FoodElement } from "~/features/health/requests/getFoodListRequest";

import { TFoodDiaryFormSchema } from "../../../../hooks/useAddFoodDiaryForm";

type MealTypeMenuProps = {
  form: UseFormReturn<TFoodDiaryFormSchema>;
  food: FoodElement;
};

const ServingSizeFormField: React.FC<MealTypeMenuProps> = ({ form, food }) => {
  return (
    <FormField
      control={form.control}
      name="servingSizeIndex"
      render={({ field }) => (
        <FormItem className="grow">
          <FormLabel>Serving size</FormLabel>
          <Select
            onValueChange={(val) => {
              field.onChange(parseInt(val));
            }}
            defaultValue={"" + field.value}
          >
            <FormControl>
              <SelectTrigger>
                <SelectValue placeholder="Select a servingsize" />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              {food.servingSizes.map((servingSize, index) => (
                <SelectItem
                  key={index}
                  value={`${index}`}
                >{`${servingSize.value} ${servingSize.unit}`}</SelectItem>
              ))}
            </SelectContent>
          </Select>
          <FormMessage />
        </FormItem>
      )}
    />
  );
};

export default ServingSizeFormField;
