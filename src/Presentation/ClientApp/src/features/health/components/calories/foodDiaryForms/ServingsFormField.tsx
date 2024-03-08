import React from "react";
import { UseFormReturn } from "react-hook-form";
import { FormField, FormItem, FormLabel, FormMessage } from "~/chadcn/ui/form";
import { Input } from "~/chadcn/ui/input";
import { TFoodDiaryFormSchema } from "~/features/health/hooks/useAddFoodDiaryForm";

type ServingsFormFieldProps = {
  form: UseFormReturn<TFoodDiaryFormSchema>;
};

const ServingsFormField: React.FC<ServingsFormFieldProps> = ({ form }) => {
  return (
    <FormField
      control={form.control}
      name="nrOfServings"
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
