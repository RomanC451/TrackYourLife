import { Pencil } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { useToggle } from "usehooks-ts";
import { Button } from "~/chadcn/ui/button";
import { DialogDescription, DialogTitle } from "~/chadcn/ui/dialog";
import { Input } from "~/chadcn/ui/input";
import { Separator } from "~/chadcn/ui/separator";
import { Skeleton } from "~/chadcn/ui/skeleton";
import { RecipeDto } from "~/services/openapi";
import MacrosDialogHeader from "../../common/MacrosDialogHeader";
import IngredientsList from "./IngredientsList";

import { LinearProgress } from "@mui/material";
import { InputError } from "~/chadcn/ui/input-error";
import { useLoadingState } from "~/contexts/LoadingContext";
import { ApiError } from "~/data/apiSettings";
import useUpdateRecipeNameMutation from "~/features/health/mutations/recipes/useUpdateRecipeNameMutation";
import { LoadingState } from "~/hooks/useDelayedLoading";
import "./file.css";

type RecipeDialogProps = {
  recipe: RecipeDto;
  isPending: LoadingState;
};

function RecipeDialog({ recipe }: RecipeDialogProps): JSX.Element {
  const [nameInputActive, toggleNameInputActive] = useToggle(false);

  const inputValue = useRef(recipe.name);
  const inputRef = useRef<HTMLInputElement>(null);

  const [inputError, setInputError] = useState<string | undefined>(undefined);

  const { updateRecipeNameMutation, isPending } = useUpdateRecipeNameMutation();

  const isLoading = useLoadingState();

  useEffect(() => {
    if (nameInputActive && inputRef.current) {
      inputRef.current.focus();
    } else if (!nameInputActive && inputRef.current) {
      inputRef.current.blur();
    }
  }, [nameInputActive]);

  function submit() {
    if (inputValue.current === recipe.name) {
      toggleNameInputActive();
      return;
    }
    if (inputValue.current === "") {
      setInputError("Name cannot be empty");
      return;
    }
    setInputError(undefined);

    updateRecipeNameMutation.mutate(
      {
        recipe: recipe,
        name: inputValue.current,
      },
      {
        onSuccess: () => {
          toggleNameInputActive();
        },
        onError: (error) => {
          setInputError((error as ApiError).response?.data.detail);
        },
      },
    );
  }

  return (
    <>
      <div className="inline-flex items-center gap-1">
        <Button
          size="sm"
          variant="ghost"
          className="mt-[6px]"
          disabled={!isPending.isLoaded}
          onClick={() => {
            toggleNameInputActive();
          }}
        >
          <Pencil className=" h-4 w-4 " />
        </Button>
        <div className="relative h-10 w-[calc(100%-80px)]">
          <Input
            ref={inputRef}
            defaultValue={inputValue.current}
            onChange={(e) => {
              setInputError(undefined);
              inputValue.current = e.target.value;
            }}
            enterKeyHint="done"
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                inputRef.current!.blur();
                submit();
              }
            }}
            type={!nameInputActive ? "hidden" : "text"}
            className="text-2xl font-semibold"
          />
          <InputError
            className="mt-[2px]"
            isError={inputError !== undefined}
            message={inputError ?? ""}
          />
          <DialogTitle
            hidden={nameInputActive}
            className="absolute top-[10px] w-full overflow-hidden whitespace-pre text-nowrap pl-[13px] text-2xl leading-5 tracking-normal"
          >
            {inputValue.current}
          </DialogTitle>
        </div>
      </div>
      <Separator className="mt-1" />

      <MacrosDialogHeader nutritionalContents={recipe.nutritionalContents} />
      <Separator />
      <IngredientsList ingredients={recipe.ingredients} recipe={recipe} />
      <div className="2-full h-1">
        <LinearProgress
          color="inherit"
          sx={{ display: isLoadingContext ? "block" : "none" }}
        />
      </div>
    </>
  );
}

RecipeDialog.Loading = function () {
  return (
    <>
      <DialogTitle hidden>Loading</DialogTitle>
      <DialogDescription hidden> Edit recipe</DialogDescription>
      <Skeleton className="h-8 w-28" />
      <Separator />
      <MacrosDialogHeader.Loading />
      <Separator />
      <IngredientsList.Loading />
    </>
  );
};

export default RecipeDialog;
