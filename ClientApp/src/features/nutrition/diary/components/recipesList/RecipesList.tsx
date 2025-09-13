import React, { useLayoutEffect, useRef } from "react";

import { Card } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { PendingState } from "@/hooks/useCustomQuery";
import { RecipeDto } from "@/services/openapi";

import RecipesListElement from "./RecipesListElement";

type RecipesListProps = {
  recipesList: RecipeDto[] | undefined;
  searchValue: string;
  pendingState: PendingState;
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  onRecipeSelected: (recipe: RecipeDto) => void;
  cardRef: React.RefObject<HTMLDivElement>;
  onHoverRecipe: (recipe: RecipeDto) => void;
  onTouchRecipe: (recipe: RecipeDto) => void;
};

function RecipesList({
  recipesList,
  searchValue,
  pendingState,
  AddRecipeButton,
  onRecipeSelected,
  cardRef,
  onHoverRecipe,
  onTouchRecipe,
}: RecipesListProps) {
  const listRef = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTo({ top: 0 });
    }
  }, [searchValue]);

  if (pendingState.isPending && !pendingState.isDelayedPending) return null;

  if (pendingState.isDelayedPending || !recipesList)
    return <RecipesList.Loading cardRef={cardRef} />;

  const filteredResults = recipesList.filter((recipe) =>
    recipe.name.toLowerCase().startsWith(searchValue.toLowerCase()),
  );

  return (
    <ScrollArea className="h-64 w-full rounded-md border p-4" ref={listRef}>
      {filteredResults?.map((recipe, index) => (
        <React.Fragment key={recipe.id}>
          <RecipesListElement
            AddRecipeButton={AddRecipeButton}
            onRecipeSelected={onRecipeSelected}
            recipe={recipe}
            onHoverRecipe={onHoverRecipe}
            onTouchRecipe={onTouchRecipe}
          />
          {index !== filteredResults.length - 1 ? (
            <Separator className="my-2" />
          ) : null}
        </React.Fragment>
      ))}
    </ScrollArea>
  );
}

RecipesList.Loading = function RecipesListLoading({
  cardRef,
}: {
  cardRef: React.RefObject<HTMLDivElement>;
}) {
  return (
    <Card
      ref={cardRef}
      className="absolute top-[60px] z-10 h-auto w-[90%] backdrop-blur-2xl lg:w-[80%]"
      onMouseDown={(e) => {
        e.stopPropagation();
      }}
    >
      <ScrollArea className="h-64 w-full rounded-md border p-4">
        {Array(4)
          .fill("")
          .map((_, index) => (
            <React.Fragment key={index}>
              <RecipesListElement.Loading />
              <Separator className="my-2" />
            </React.Fragment>
          ))}
      </ScrollArea>
    </Card>
  );
};

export default RecipesList;
