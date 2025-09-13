import { Minus, Plus } from "lucide-react";
import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form, FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { MutationPendingState } from "@/hooks/useCustomMutation";

import { RecipeDetailsSchema } from "../../data/recipesSchemas";

function RecipeForm({
  form,
  handleCustomSubmit,
  submitButtonText,
  pendingState,
  onCancel,
}: {
  form: UseFormReturn<RecipeDetailsSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  pendingState: MutationPendingState;
  onCancel: () => void;
}) {
  return (
    <Form {...form}>
      <form onSubmit={handleCustomSubmit} className="space-y-4 pt-2">
        <FormField
          control={form.control}
          name="name"
          render={({ field: { onBlur, onChange, value, ...field } }) => (
            <FormItem>
              <Label htmlFor="name" className="text-right">
                Name
              </Label>
              <Input
                {...field}
                value={value || ""}
                onChange={(e) => {
                  onChange(e.target.value);
                }}
                onBlur={onBlur}
                id="create-recipe-name"
                placeholder="Recipe name"
                autoComplete="off"
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="flex flex-col justify-between gap-2 @sm/dialog:flex-row">
          <FormField
            control={form.control}
            name="portions"
            render={({ field: { onChange, value, ...field } }) => (
              <FormItem className="grow @sm/dialog:grow-0">
                <Label htmlFor="portions" className="text-right">
                  Portions
                </Label>
                <div className="flex w-full items-center gap-2">
                  <Input
                    {...field}
                    type="number"
                    value={value === 0 ? "" : value}
                    onChange={(e) => {
                      const newValue = e.target.value;
                      onChange(newValue === "" ? 0 : Number(newValue));
                    }}
                    id="create-recipe-portions"
                    placeholder="Portions"
                    className="w-full"
                  />
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    className="h-8 w-8"
                    onClick={() => {
                      const newValue = Math.max(0, (value || 0) - 1);
                      onChange(newValue);
                    }}
                  >
                    <Minus className="h-4 w-4" />
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    className="h-8 w-8"
                    onClick={() => {
                      const newValue = (value || 0) + 1;
                      onChange(newValue);
                    }}
                  >
                    <Plus className="h-4 w-4" />
                  </Button>
                </div>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="weight"
            render={({ field: { onChange, value, ...field } }) => (
              <FormItem className="relative min-w-32 grow">
                <Label htmlFor="weight" className="text-right">
                  Total weight
                </Label>
                <Input
                  {...field}
                  type="number"
                  value={value === 0 ? "" : value}
                  onChange={(e) => {
                    const newValue = e.target.value;
                    onChange(newValue === "" ? 0 : Number(newValue));
                  }}
                  id="create-recipe-weight"
                  placeholder="Weight in grams"
                />
                <p className="absolute bottom-2 right-3 text-sm">grams</p>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <div className="flex justify-end gap-2">
          <Button variant="outline" onClick={onCancel} type="button">
            Cancel
          </Button>
          <ButtonWithLoading
            type="submit"
            isLoading={pendingState.isDelayedPending}
            disabled={pendingState.isPending}
            className=""
          >
            {submitButtonText}
          </ButtonWithLoading>
        </div>
      </form>
    </Form>
  );
}

RecipeForm.Loading = function Loading() {
  return (
    <div className="space-y-4 pt-2">
      <div className="space-y-2">
        <Skeleton className="h-4 w-12" /> {/* Label */}
        <Skeleton className="h-10 w-full" /> {/* Input */}
      </div>
      <div className="space-y-2">
        <Skeleton className="h-4 w-16" /> {/* Label */}
        <div className="flex items-center gap-2">
          <Skeleton className="h-10 w-32" /> {/* Input */}
          <Skeleton className="h-8 w-8" /> {/* Minus button */}
          <Skeleton className="h-8 w-8" /> {/* Plus button */}
        </div>
      </div>
      <div className="flex justify-end gap-2">
        <Skeleton className="h-10 w-20 bg-muted" />{" "}
        {/* Cancel button - outline variant */}
        <Skeleton className="h-10 w-32 bg-primary/50" />{" "}
        {/* Submit button - primary variant */}
      </div>
    </div>
  );
};

export default RecipeForm;
