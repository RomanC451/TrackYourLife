import { useEffect, useRef, useState } from "react";

import { Card } from "@/components/ui/card";
import GoogleInput from "@/components/ui/google-input";
import { withOnSuccess } from "@/lib/with";
import { RecipeDto } from "@/services/openapi";

import useRecipesQuery from "../../queries/useRecipesQuery";
import RecipesList from "../recipesList/RecipesList";

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

  const textFieldRef = useRef<HTMLInputElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

  const localAddRecipeDialog = withOnSuccess(AddRecipeDialog, () => {
    setResultsTableOpened(false);
  });

  useEffect(() => {
    if (!recipesQuery.data) return;

    if (
      recipesQuery.data.some((recipe) =>
        recipe.name.toLowerCase().startsWith(searchValue.toLowerCase()),
      )
    ) {
      setError("");
    } else {
      setError("No recipes found");
    }
  }, [searchValue, recipesQuery.data]);

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
    <div className="h-max-[calc(90%-195px)] relative flex w-full flex-col items-center gap-2">
      <GoogleInput
        ref={textFieldRef}
        autoComplete={"false"}
        label={placeHolder ?? "Search recipe..."}
        error={!!error}
        helperText={error}
        onChange={(e) => {
          setSearchValue(e.target.value);
        }}
        onClick={(e) => {
          e.stopPropagation();
        }}
        onFocus={() => {
          setResultsTableOpened(true);
          window.scrollTo({
            top: document.documentElement.scrollHeight,
            behavior: "smooth",
          });
        }}
      />
      {resultsTableOpened && !recipesQuery.isError ? (
        <Card
          ref={cardRef}
          className="absolute top-[60px] z-10 h-auto w-[90%] backdrop-blur-2xl"
          onMouseDown={(e) => {
            e.stopPropagation();
          }}
        >
          <RecipesList
            recipesList={recipesQuery.data}
            isPending={isPending}
            AddRecipeButton={AddRecipeButton}
            AddRecipeDialog={localAddRecipeDialog}
            searchValue={searchValue}
            cardRef={cardRef}
          />
        </Card>
      ) : null}
    </div>
  );
}

export default RecipeSearch;
