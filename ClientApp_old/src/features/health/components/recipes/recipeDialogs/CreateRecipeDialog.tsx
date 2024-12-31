import { LinearProgress } from "@mui/material";
import { PropsWithChildren, useState } from "react";
import { useToggle } from "usehooks-ts";
import { Button } from "~/chadcn/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "~/chadcn/ui/dialog";
import { Input } from "~/chadcn/ui/input";
import { InputError } from "~/chadcn/ui/input-error";
import { ApiError } from "~/data/apiSettings";
import { useRecipesTableContext } from "~/features/health/contexts/RecipesTableContext";
import useCreateRecipeMutation from "~/features/health/mutations/recipes/useCreateRecipeMutation";
import { useIsMobile } from "~/hooks/use-mobile";

type CreateRecipeDialogProps = {} & PropsWithChildren;

function CreateRecipeDialog({
  children,
}: CreateRecipeDialogProps): JSX.Element {
  const isMobile = useIsMobile();

  const [createRecipeDialogState, toggleCreateRecipeDialogState] = useToggle();

  const { openEditRecipeModal: openModal } = useRecipesTableContext();

  const [inputValue, setInputValue] = useState("");

  const { createRecipeMutation, isPending } = useCreateRecipeMutation();

  const onSubmit = (name: string) => {
    createRecipeMutation.mutateAsync(
      { name },
      {
        onSuccess: (resp) => {
          toggleCreateRecipeDialogState();
          openModal(resp.id);
        },
      },
    );
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      onSubmit(inputValue);
    }
  };

  return (
    <>
      <Dialog
        open={createRecipeDialogState}
        onOpenChange={toggleCreateRecipeDialogState}
      >
        <DialogTrigger asChild>{children}</DialogTrigger>
        <DialogContent className="space-y-4">
          <DialogTitle>Recipe Name</DialogTitle>
          <DialogDescription hidden>Create a new recipe.</DialogDescription>
          <Input
            value={inputValue}
            autoFocus={false}
            onChange={(e) => setInputValue(e.target.value)}
            onKeyDown={handleKeyDown}
          />

          <InputError
            isError={createRecipeMutation.error !== undefined}
            error={createRecipeMutation.error as ApiError}
          />
          <div className="flex w-full justify-end">
            <Button
              onClick={() => onSubmit(inputValue)}
              disabled={!isPending.isLoaded}
            >
              Create
            </Button>
          </div>
          {isPending.isLoading ? <LinearProgress color="inherit" /> : null}
        </DialogContent>
      </Dialog>
    </>
  );
}

export default CreateRecipeDialog;
