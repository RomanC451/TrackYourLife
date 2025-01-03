import { Control, FieldValues, Path } from "react-hook-form";

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
import { FoodDto } from "@/services/openapi";

type ServingSizeFormFieldProps<T extends FieldValues> = {
  control: Control<T>;
  food: FoodDto;
};

const ServingSizeFormField = <T extends { servingSizeIndex: number }>({
  control,
  food,
}: ServingSizeFormFieldProps<T>) => {
  return (
    <FormField
      control={control}
      name={"servingSizeIndex" as Path<T>}
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
                <SelectValue placeholder="Select a serving size" />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              {Object.values(food.servingSizes).map((servingSize, index) => (
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
