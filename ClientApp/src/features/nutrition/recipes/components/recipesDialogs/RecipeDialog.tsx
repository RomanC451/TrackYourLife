import { useEffect, useRef, useState } from "react";
import { Pencil } from "lucide-react";
import { useToggle } from "usehooks-ts";

import { Button } from "@/components/ui/button";
import { DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { InputError } from "@/components/ui/input-error";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { RecipeDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import IngredientsList from "../ingredientsList/IngredientsList";

import "./file.css";

import { useLoadingContext } from "@/contexts/LoadingContext";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { MUTATION_KEYS } from "@/features/nutrition/common/data/mutationKeys";

import useUpdateRecipeNameMutation from "../../mutations/useUpdateRecipeNameMutation";

type RecipeDialogProps = {
  recipe: RecipeDto;
  isPending: LoadingState;
  isFetching: boolean;
};

function RecipeDialog({ recipe, isFetching }: RecipeDialogProps): JSX.Element {
  const [nameInputActive, toggleNameInputActive] = useToggle(false);

  const inputValue = useRef(recipe.name);
  const inputRef = useRef<HTMLInputElement>(null);

  const [inputError, setInputError] = useState<string | undefined>(undefined);

  const { updateRecipeNameMutation } = useUpdateRecipeNameMutation();
  const { updateLoadingState } = useLoadingContext(MUTATION_KEYS.recipes);

  useEffect(() => {
    if (!isFetching) {
      updateLoadingState(false);
    }
  }, [isFetching]);

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
      <div className="inline-flex w-full items-center gap-1">
        <Button
          size="sm"
          variant="ghost"
          className="mt-[6px]"
          disabled={updateRecipeNameMutation.isPending}
          onClick={() => {
            toggleNameInputActive();
          }}
        >
          <Pencil className="h-4 w-4" />
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
            className="absolute top-[10px] w-full overflow-x-clip whitespace-pre text-nowrap pl-[13px] text-2xl leading-5 tracking-normal"
          >
            {inputValue.current}
          </DialogTitle>
        </div>
      </div>
      <Separator className="mt-1" />

      <MacrosDialogHeader nutritionalContents={recipe.nutritionalContents} />
      <Separator />
      <IngredientsList recipe={recipe} />
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
