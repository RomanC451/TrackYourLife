import { memo, useCallback, useState } from "react";
import { useToggle } from "usehooks-ts";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { useLoadingContext } from "@/contexts/LoadingContext";
import FoodSearch from "@/features/nutrition/common/components/FoodSearch";
import { MUTATION_KEYS } from "@/features/nutrition/common/data/mutationKeys";
import { withProp, withProps } from "@/lib/with";
import { RecipeDto } from "@/services/openapi";

import useRemoveIngredientsMutation from "../../mutations/useRemoveIngredientsMutation";
import AddIngredientButton from "../AddIngredientButton";
import AddIngredientDialog from "../ingredientsDialogs/AddIngredientDialog";
import IngredientListElement from "./IngredientListElement";

type IngredientsListProps = {
  recipe: RecipeDto;
};

function IngredientsList({ recipe }: IngredientsListProps): JSX.Element {
  const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]);

  const { removeIngredientsMutation, isPending } =
    useRemoveIngredientsMutation();

  const [allSelected, toggleAllSelected] = useToggle(false);

  const { loadingState, updateLoadingState } = useLoadingContext(
    MUTATION_KEYS.recipes,
  );

  function handleAllSelected() {
    toggleAllSelected();
    if (allSelected) {
      setSelectedIngredients([]);
      return;
    }
    setSelectedIngredients(recipe.ingredients.map((i) => i.id));
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
  async function deleteSelected() {
    const ingredientsToRemove = recipe.ingredients.filter((i) =>
      selectedIngredients.includes(i.id),
    );

    if (!ingredientsToRemove) return;

    updateLoadingState(true);

    await removeIngredientsMutation.mutateAsync(
      {
        recipe,
        ingredients: ingredientsToRemove,
      },
      {
        onSuccess: () => {
          setSelectedIngredients([]);
        },
      },
    );

    if (allSelected) toggleAllSelected();
  }

  return (
    <div className="space-y-3">
      <FoodSearch
        AddFoodButton={withProps(AddIngredientButton, {
          recipe: recipe,
        })}
        AddFoodDialog={withProp(AddIngredientDialog, "recipe", recipe)}
        placeHolder="Search for ingredients..."
      />
      <div className="flex items-center justify-between">
        <CardTitle className="text-base">Ingredients list</CardTitle>
        <div>
          <ButtonWithLoading
            variant="destructive"
            onClick={deleteSelected}
            disabled={
              isPending.isLoading ||
              selectedIngredients.length === 0 ||
              loadingState
            }
            isLoading={isPending.isLoading}
          >
            Delete selected : {selectedIngredients.length}
          </ButtonWithLoading>
        </div>
      </div>
      <Separator />
      <ScrollArea className="h-[38vh] sm:h-[40vh] lg:h-[50vh]">
        <IngredientsList.Content
          recipe={recipe}
          selectIngredient={selectIngredient}
          selectedIngredients={selectedIngredients}
        />
      </ScrollArea>

      <Separator />
      <div className="inline-flex items-center gap-2">
        <Checkbox
          checked={allSelected}
          onCheckedChange={handleAllSelected}
          disabled={isPending.isLoading || recipe.ingredients.length === 0}
        />
        <h3 className="font-semibold">Select all</h3>
      </div>
    </div>
  );
}

IngredientsList.Content = function ({
  recipe,
  selectedIngredients,
  selectIngredient,
}: {
  recipe: RecipeDto;
  selectedIngredients: string[];
  selectIngredient: (id: string) => void;
}) {
  return recipe.ingredients.map((ingredient) => (
    <IngredientListElement
      key={ingredient.id}
      recipe={recipe}
      ingredient={ingredient}
      selected={selectedIngredients.includes(ingredient.id)}
      handleSelect={selectIngredient}
    />
  ));
};

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
