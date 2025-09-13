import { useEffect, useRef, useState } from "react";

import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { InputError } from "@/components/ui/input-error";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { RecipeDto } from "@/services/openapi";

import RecipesList from "../recipesList/RecipesList";

type RecipeSearchProps = {
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  onRecipeSelected: (recipe: RecipeDto) => void;
  onHoverRecipe: (recipe: RecipeDto) => void;
  onTouchRecipe: (recipe: RecipeDto) => void;
  placeHolder?: string;
};

function RecipeSearch({
  AddRecipeButton,
  onRecipeSelected,
  onHoverRecipe,
  onTouchRecipe,
  placeHolder,
}: RecipeSearchProps): JSX.Element {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const [searchValue, setSearchValue] = useState("");
  const [error, setError] = useState("");

  const { query: recipesQuery, pendingState } = useCustomQuery(
    recipesQueryOptions.all,
  );

  const textFieldRef = useRef<HTMLInputElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

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
      <Input
        ref={textFieldRef}
        autoComplete={"false"}
        placeholder={placeHolder ?? "Search recipe..."}
        className="h-12"
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
      <InputError isError={!!error} message={error} />
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
            // ! TODO: add a loading state if needed or remove it
            pendingState={pendingState}
            AddRecipeButton={AddRecipeButton}
            onRecipeSelected={onRecipeSelected}
            searchValue={searchValue}
            cardRef={cardRef}
            onHoverRecipe={onHoverRecipe}
            onTouchRecipe={onTouchRecipe}
          />
        </Card>
      ) : null}
    </div>
  );
}

export default RecipeSearch;
