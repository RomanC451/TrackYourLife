import { PropsWithChildren, useState } from "react";
import { LinearProgress } from "@mui/material";
import { useToggle } from "usehooks-ts";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { InputError } from "@/components/ui/input-error";
import { ApiError } from "@/services/openapi/apiSettings";

import { useRecipesTableContext } from "../../contexts/RecipesTableContext";
import useCreateRecipeMutation from "../../mutations/useCreateRecipeMutation";

type CreateRecipeDialogProps = {} & PropsWithChildren;

function CreateRecipeDialog({ children }: CreateRecipeDialogProps) {
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
