import { useState } from "react";
import { MinusIcon, PlusIcon } from "lucide-react";
import { FieldValues, Path, useFormContext } from "react-hook-form";

import { Button } from "@/components/ui/button";
import {
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";

const QuantityFormField = <FormSchema extends FieldValues>({
  name,
  label,
  className,
}: {
  name: Path<FormSchema>;
  label: string;
  className?: string;
}) => {
  const form = useFormContext<FormSchema>();

  const [inputValue, setInputValue] = useState<string>(form.getValues(name));

  return (
    <FormField
      control={form.control}
      name={name}
      render={({ field }) => {
        return (
          <FormItem
            className={cn("flex grow items-end gap-2", className)}
            autoFocus={false}
          >
            <div className="grow space-y-2">
              <FormLabel>{label}</FormLabel>
              <div className="flex gap-2">
                <Input
                  {...field}
                  type="numeric"
                  value={inputValue}
                  onChange={(e) => {
                    let newValue;
                    const regex = /^\d*(\.\d{0,2})?$/;
                    if (!regex.test(e.target.value)) {
                      e.target.value = e.target.value.slice(0, -1);
                      return;
                    }

                    if (e.target.value === "") {
                      setInputValue("");
                      field.onChange(0);
                      return;
                    }

                    if (e.target.value === ".") {
                      e.target.value = "";
                      setInputValue("");
                      field.onChange(0);
                      return;
                    }

                    newValue = String(Number(e.target.value));
                    if (e.target.value.endsWith(".")) {
                      newValue = newValue + ".";
                    }

                    e.target.value = newValue;
                    setInputValue(newValue);
                    field.onChange(Number(newValue));
                  }}
                />
                <Button
                  variant="outline"
                  type="button"
                  size="icon"
                  onClick={() => {
                    field.onChange(Math.max(field.value - 1, 0));
                    setInputValue(String(Math.max(field.value - 1, 0)));
                  }}
                >
                  <MinusIcon className="h-4 w-4" />
                </Button>
                <Button
                  variant="outline"
                  type="button"
                  size="icon"
                  onClick={() => {
                    field.onChange(field.value + 1);
                    setInputValue(String(field.value + 1));
                  }}
                >
                  <PlusIcon className="h-4 w-4" />
                </Button>
              </div>
              <FormMessage />
            </div>
          </FormItem>
        );
      }}
    />
  );
};

export default QuantityFormField;
