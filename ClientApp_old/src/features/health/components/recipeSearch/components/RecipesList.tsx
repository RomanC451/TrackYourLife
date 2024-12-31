import React, { useLayoutEffect, useRef } from "react";
import { Card } from "~/chadcn/ui/card";
import { ScrollArea } from "~/chadcn/ui/scroll-area";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { RecipeDto } from "~/services/openapi";
import RecipesListElement from "./RecipesListElement";
import { Separator } from "~/chadcn/ui/separator";

type RecipesListProps = {
  recipesList: RecipeDto[] | undefined;
  searchValue: string;
  isPending: LoadingState;
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  AddRecipeDialog: React.ComponentType<{ recipe: RecipeDto }>;
  cardRef: React.RefObject<HTMLDivElement>;
};

function RecipesList({
  recipesList,
  searchValue,
  isPending,
  AddRecipeButton,
  AddRecipeDialog,
  cardRef,
}: RecipesListProps) {
  const listRef = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTo({ top: 0 });
    }
  }, [searchValue]);

  if (isPending.isStarting) return null;

  if (isPending.isLoading || !recipesList)
    return <RecipesList.Loading cardRef={cardRef} />;

  const filteredResults = recipesList.filter((recipe) =>
    recipe.name.toLowerCase().startsWith(searchValue.toLowerCase()),
  );

  return (
    <Card
      ref={cardRef}
      className="absolute top-[60px] z-10 h-auto w-[90%]  backdrop-blur-2xl lg:w-[80%]"
      onMouseDown={(e) => {
        e.stopPropagation();
      }}
    >
      <ScrollArea className="h-64 w-full rounded-md border p-4" ref={listRef}>
        {filteredResults?.map((recipe) => (
          <React.Fragment key={recipe.id}>
            <RecipesListElement
              AddRecipeButton={AddRecipeButton}
              AddRecipeDialog={AddRecipeDialog}
              recipe={recipe}
            />
            <Separator className="my-2" />
          </React.Fragment>
        ))}
      </ScrollArea>
    </Card>
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
      className="absolute top-[60px] z-10 h-auto w-[90%] bg-red-400 backdrop-blur-2xl lg:w-[80%]"
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
