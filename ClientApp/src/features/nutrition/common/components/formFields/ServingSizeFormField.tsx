import { FieldValues, Path, useFormContext } from "react-hook-form";

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
import { ServingSizeDto } from "@/services/openapi";

const ServingSizeFormField = <FormSchema extends FieldValues>({
  servingSizes,
  name,
  className,
}: {
  servingSizes: ServingSizeDto[];
  name: Path<FormSchema>;
  className?: string;
}) => {
  const form = useFormContext<FormSchema>();

  return (
    <FormField
      control={form.control}
      name={name}
      render={({ field }) => (
        <FormItem className={cn("grow", className)}>
          <FormLabel>Serving size</FormLabel>
          <Select
            onValueChange={(val) => {
              field.onChange(val);
            }}
            defaultValue={field.value}
          >
            <FormControl>
              <SelectTrigger
                onClick={(e) => {
                  e.currentTarget.blur();
                }}
                className="min-w-48"
              >
                <SelectValue placeholder="Select a serving size" />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              {servingSizes.map((servingSize) => (
                <SelectItem
                  key={servingSize.id}
                  value={servingSize.id}
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
