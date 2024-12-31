import { TextField } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { RecipeDto } from "~/services/openapi";
import { withOnSuccess } from "~/utils/with";
import useRecipesQuery from "../../queries/recipes/useRecipesQuery";
import RecipesList from "./components/RecipesList";

type RecipeSearchProps = {
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  AddRecipeDialog: React.ComponentType<{
    recipe: RecipeDto;
    onSuccess: () => void;
  }>;
  placeHolder?: string;
};

function RecipeSearch({
  AddRecipeButton,
  AddRecipeDialog,
  placeHolder,
}: RecipeSearchProps): JSX.Element {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const [searchValue, setSearchValue] = useState("");
  const [error, setError] = useState("");

  const { recipesQuery, isPending } = useRecipesQuery();

  const textFieldRef = useRef<HTMLDivElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

  const localAddRecipeDialog = withOnSuccess(AddRecipeDialog, () => {
    setResultsTableOpened(false);
  });

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const targetNode = event.target as Node;
      if (
        targetNode.nodeName !== "HTML" &&
        textFieldRef.current &&
        !textFieldRef.current.contains(targetNode) &&
        cardRef.current &&
        !cardRef.current.contains(targetNode)
      ) {
        setResultsTableOpened(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  return (
    <div className="h-max-[calc(90%-195px)] relative mt-[20px] flex w-full flex-col items-center gap-2">
      <TextField
        ref={textFieldRef}
        className="text-black"
        autoComplete={"false"}
        fullWidth
        label={placeHolder ?? "Search recipe..."}
        error={!!error}
        helperText={error}
        onChange={(e) => {
          setError("");

          setSearchValue(e.target.value);
        }}
        onClick={(e) => {
          e.stopPropagation();
        }}
        onFocus={() => {
          setResultsTableOpened(true);
        }}
      />
      {resultsTableOpened && !recipesQuery.isError ? (
        <RecipesList
          recipesList={recipesQuery.data}
          isPending={isPending}
          AddRecipeButton={AddRecipeButton}
          AddRecipeDialog={localAddRecipeDialog}
          searchValue={searchValue}
          cardRef={cardRef}
        />
      ) : null}
    </div>
  );
}

export default RecipeSearch;
