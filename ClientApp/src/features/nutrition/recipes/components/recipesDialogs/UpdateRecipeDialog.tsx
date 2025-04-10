import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { Pencil } from "lucide-react";
import { useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Form, FormField, FormItem, FormLabel } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { RecipeDto } from "@/services/openapi";

import {
  updateRecipeFormSchema,
  UpdateRecipeFormSchema,
} from "../../data/schemas";
import useUpdateRecipeMutation from "../../mutations/useUpdateRecipeMutation";

type UpdateRecipeDialogProps = {
  recipe: RecipeDto;
};

function UpdateRecipeDialog({ recipe }: UpdateRecipeDialogProps) {
  const { updateRecipeMutation, isPending } = useUpdateRecipeMutation();

  const [open, setOpen] = useState(false);

  const form = useForm<UpdateRecipeFormSchema>({
    resolver: zodResolver(updateRecipeFormSchema),
    defaultValues: {
      name: recipe.name,
      portions: recipe.portions,
    },
  });

  function onSubmit(formData: UpdateRecipeFormSchema) {
    updateRecipeMutation.mutate(
      { recipe, ...formData },
      { onSuccess: () => setOpen(false) },
    );
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button size="sm" variant="ghost" className="size-10">
          <Pencil className="" />
        </Button>
      </DialogTrigger>
      <DialogContent className="space-y-4">
        <DialogTitle>Update Recipe</DialogTitle>
        <DialogDescription hidden>Update the recipe name.</DialogDescription>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name</FormLabel>
                  <Input placeholder="Update recipe name" {...field} />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="portions"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Number of portions</FormLabel>
                  <Input
                    placeholder="Update number of portions"
                    // type="number"
                    {...field}
                    onChange={(e) => {
                      e.target.value = e.target.value
                        .replace(/[^\d.]/g, "")
                        .replace(/(\..*)\./g, "$1");
                      if (e.target.value === "") return field.onChange("");
                      field.onChange(parseFloat(e.target.value));
                    }}
                  />
                </FormItem>
              )}
            />
            <div className="flex justify-end gap-4">
              <Button size="sm" variant="ghost" onClick={() => setOpen(false)}>
                Cancel
              </Button>
              <ButtonWithLoading
                type="submit"
                size="sm"
                isLoading={isPending.isLoading}
                disabled={!isPending.isLoaded}
              >
                Update
              </ButtonWithLoading>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

export default UpdateRecipeDialog;
