import { Control, FieldValues, Path } from "react-hook-form";

import {
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

type ServingsFormFieldProps<T extends FieldValues> = {
  control: Control<T>;
};

const ServingsFormField = <T extends { nrOfServings: number }>({
  control,
}: ServingsFormFieldProps<T>) => {
  return (
    <FormField
      control={control}
      name={"nrOfServings" as Path<T>}
      render={({ field }) => {
        return (
          <FormItem className="" autoFocus={false}>
            <FormLabel>Number of servings</FormLabel>
            <Input
              className="w-full"
              type="text"
              autoComplete="off"
              inputMode="numeric"
              name={field.name}
              onBlur={field.onBlur}
              ref={field.ref}
              defaultValue={field.value}
              onChange={(e) => {
                e.target.value = e.target.value
                  .replace(/[^\d.]/g, "")
                  .replace(/(\..*)\./g, "$1");
                field.onChange(parseFloat(e.target.value || "0"));
              }}
            />
            <FormMessage />
          </FormItem>
        );
      }}
    />
  );
};

export default ServingsFormField;
