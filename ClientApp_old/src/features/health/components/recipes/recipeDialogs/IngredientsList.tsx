import { memo, useCallback, useState } from "react";
import { useToggle } from "usehooks-ts";
import { Button } from "~/chadcn/ui/button";
import { CardTitle } from "~/chadcn/ui/card";
import { Checkbox } from "~/chadcn/ui/checkbox";
import { ScrollArea } from "~/chadcn/ui/scroll-area";
import { Separator } from "~/chadcn/ui/separator";
import { Skeleton } from "~/chadcn/ui/skeleton";
import useAddIngredientMutation from "~/features/health/mutations/recipes/useAddIngredientMutation";
import useRemoveIngredientMutation from "~/features/health/mutations/recipes/useRemoveIngredientMutation";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { IngredientDto, RecipeDto } from "~/services/openapi";
import { withProp, withProps } from "~/utils/with";
import FoodSearch from "../../foodSearch/FoodSearch";
import AddIngredientButton from "../AddIngredientButton";
import AddIngredientDialog from "./AddIngredientDialog";
import IngredientListElement from "./IngredientListElement";

type IngredientsListProps = {
  recipe: RecipeDto;
  ingredients: IngredientDto[];
};

function IngredientsList({
  recipe,
  ingredients,
}: IngredientsListProps): JSX.Element {
  const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]);

  const { addIngredientMutation } = useAddIngredientMutation();

  const { removeIngredientMutation, isPending } = useRemoveIngredientMutation();

  const [allSelected, toggleAllSelected] = useToggle(false);

  function handleAllSelected() {
    toggleAllSelected();
    if (allSelected) {
      setSelectedIngredients([]);
      return;
    }
    setSelectedIngredients(ingredients.map((i) => i.id));
  }

  const selectIngredient = useCallback(
    function (id: string) {
      if (selectedIngredients.includes(id)) {
        setSelectedIngredients(selectedIngredients.filter((i) => i !== id));
        return;
      }
      setSelectedIngredients([...selectedIngredients, id]);
    },
    [selectedIngredients],
  );

  const handleSingleDelete = useCallback(function (ingredient: IngredientDto) {
    removeIngredientMutation.mutate({
      ingredient,
      recipe,
    });
  }, []);

  async function deleteSelected() {
    for (const ingredientId of selectedIngredients) {
      const ingredient = ingredients.find((i) => i.id === ingredientId);

      if (!ingredient) return;

      await removeIngredientMutation.mutateAsync(
        {
          recipe,
          ingredient,
        },
        {
          onSuccess: () => {
            setSelectedIngredients((prev) =>
              prev.filter((i) => i !== ingredientId),
            );
          },
        },
      );
    }

    if (allSelected) toggleAllSelected();
  }

  return (
    <div className="space-y-3">
      <FoodSearch
        AddFoodButton={withProps(AddIngredientButton, {
          recipe: recipe,
          mutation: addIngredientMutation,
        })}
        AddFoodDialog={withProp(AddIngredientDialog, "recipe", recipe)}
        placeHolder="Search for ingredients..."
      />
      <div className="flex items-center justify-between">
        <CardTitle className="text-base">Ingredients list</CardTitle>
        <div>
          <Button
            variant="destructive"
            onClick={deleteSelected}
            // disabled={isPending.isLoading || selectedIngredients.length === 0}
          >
            Delete selected : {selectedIngredients.length}
          </Button>
        </div>
      </div>
      <Separator />
      <ScrollArea className="h-[38vh] sm:h-[40vh] lg:h-[50vh]">
        <IngredientsList.Content
          recipe={recipe}
          ingredients={ingredients}
          handleSingleDelete={handleSingleDelete}
          selectIngredient={selectIngredient}
          selectedIngredients={selectedIngredients}
          isPending={isPending}
        />
        {/* {ingredients.map((ingredient, index) => (
          <IngredientListElement
            key={ingredient.food.name + index}
            recipe={recipe}
            ingredient={ingredient}
            selected={selectedIngredients.includes(ingredient.id)}
            handleSelect={selectIngredient}
            handleDelete={handleSingleDelete}
            isPending={isPending}
          />
        ))} */}
      </ScrollArea>
      {ingredients.length === 0 ? null : (
        <>
          <Separator />
          <div className="inline-flex items-center gap-2">
            <Checkbox
              checked={allSelected}
              onCheckedChange={handleAllSelected}
              disabled={isPending.isLoading}
            />
            <h3 className="font-semibold">Select all</h3>
          </div>
        </>
      )}
    </div>
  );
}

IngredientsList.Content = memo(function ({
  recipe,
  ingredients,
  selectedIngredients,
  selectIngredient,
  handleSingleDelete,
  isPending,
}: {
  recipe: RecipeDto;
  ingredients: IngredientDto[];
  selectedIngredients: string[];
  selectIngredient: (id: string) => void;
  handleSingleDelete: (ingredient: IngredientDto) => void;
  isPending: LoadingState;
}) {
  return ingredients.map((ingredient) => (
    <IngredientListElement
      key={ingredient.id}
      recipe={recipe}
      ingredient={ingredient}
      selected={selectedIngredients.includes(ingredient.id)}
      handleSelect={selectIngredient}
      handleDelete={handleSingleDelete}
      isPending={isPending}
    />
  ));
});

IngredientsList.Loading = memo(function () {
  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <CardTitle className="text-base">Ingredients list</CardTitle>
        <div>
          <Skeleton className="h-10 w-36 bg-red-400" />
        </div>
      </div>
      <FoodSearch.Loading />
      <ScrollArea className="h-[50vh]">
        <IngredientListElement.Loading />
        <IngredientListElement.Loading />
        <IngredientListElement.Loading />
      </ScrollArea>
    </div>
  );
});

export default IngredientsList;
