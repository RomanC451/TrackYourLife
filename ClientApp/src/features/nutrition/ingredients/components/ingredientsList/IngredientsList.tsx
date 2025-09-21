import { CheckedState } from "@radix-ui/react-checkbox";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import FoodSearch from "@/features/nutrition/common/components/foodSearch/FoodSearch";
import { FoodSearchContextProvider } from "@/features/nutrition/common/components/foodSearch/useFoodSearchContext";
import { withProps } from "@/lib/with";
import { RecipeDto } from "@/services/openapi";

import { useIngredientsSelection } from "../../hooks/useIngredientsSelection";
import useRemoveIngredientsMutation from "../../mutations/useRemoveIngredientsMutation";
import AddIngredientButton from "../AddIngredientButton";
import IngredientListElement from "./IngredientListElement";

type IngredientsListProps = {
  recipe: RecipeDto;
};

function IngredientsList({ recipe }: IngredientsListProps): React.JSX.Element {
  const {
    selectedIds,
    toggle,
    selectedIngredients,
    isAllSelected,
    handleSelectAll,
    clearSelection,
  } = useIngredientsSelection(recipe.ingredients);

  const removeIngredientsMutation = useRemoveIngredientsMutation();

  async function deleteSelected() {
    if (selectedIngredients.length === 0) return;

    await removeIngredientsMutation.mutateAsync(
      {
        recipe,
        ingredients: selectedIngredients,
      },
      {
        onSuccess: () => {
          clearSelection();
        },
      },
    );
  }

  return (
    <div className="space-y-3">
      <FoodSearchContextProvider>
        <FoodSearch
          addFoodButtonComponent={withProps(AddIngredientButton, {
            recipe: recipe,
          })}
          onSelectedFoodToOptions={{
            to: "/nutrition/recipes/$recipeId/ingredients/create",
            params: { recipeId: recipe.id },
          }}
          placeHolder="Search for ingredients..."
        />
      </FoodSearchContextProvider>
      <IngredientsList.Header
        isDelayedPending={removeIngredientsMutation.isDelayedPending}
        isPending={removeIngredientsMutation.isPending}
        onDelete={deleteSelected}
        selectedCount={selectedIds.length}
      />
      <Separator />

      <ScrollArea className="h-[38vh] max-h-[calc(100dvh-520px)] sm:h-[40vh] lg:h-[50vh]">
        <IngredientsList.Content
          recipe={recipe}
          selectIngredient={toggle}
          selectedIngredients={selectedIds}
        />
      </ScrollArea>
      <Separator />

      <IngredientsList.Footer
        hasIngredients={recipe.ingredients.length > 0}
        isAllSelected={isAllSelected}
        isPending={removeIngredientsMutation.isPending}
        onSelectAll={(checked) => handleSelectAll(Boolean(checked))}
      />
    </div>
  );
}

type IngredientsListHeaderProps = {
  onDelete: () => void;
  selectedCount: number;
  isPending: boolean;
  isDelayedPending: boolean;
};

IngredientsList.Header = function ({
  onDelete,
  selectedCount,
  isPending,
  isDelayedPending,
}: IngredientsListHeaderProps) {
  return (
    <div className="flex items-center justify-between">
      <CardTitle className="text-base">Ingredients list</CardTitle>
      <div>
        <ButtonWithLoading
          variant="destructive"
          onClick={onDelete}
          disabled={isDelayedPending || selectedCount === 0 || isPending}
          isLoading={isDelayedPending}
        >
          Delete selected : {selectedCount}
        </ButtonWithLoading>
      </div>
    </div>
  );
};

type IngredientsListFooterProps = {
  isAllSelected: boolean;
  onSelectAll: (checked: CheckedState) => void;
  isPending: boolean;
  hasIngredients: boolean;
};

IngredientsList.Footer = function ({
  isAllSelected,
  onSelectAll,
  isPending,
  hasIngredients,
}: IngredientsListFooterProps) {
  return (
    <div className="inline-flex items-center gap-2">
      <Checkbox
        checked={isAllSelected}
        onCheckedChange={onSelectAll}
        disabled={isPending || !hasIngredients}
      />
      <h3 className="font-semibold">Select all</h3>
    </div>
  );
};

type IngredientsListContentProps = {
  recipe: RecipeDto;
  selectedIngredients: string[];
  selectIngredient: (id: string) => void;
};

IngredientsList.Content = function ({
  recipe,
  selectedIngredients,
  selectIngredient,
}: IngredientsListContentProps) {
  return recipe.ingredients.map((ingredient) => (
    <IngredientListElement
      key={ingredient.id}
      recipe={recipe}
      ingredient={ingredient}
      selected={selectedIngredients.includes(ingredient.id)}
      onSelect={selectIngredient}
    />
  ));
};

IngredientsList.Loading = function () {
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
};

export default IngredientsList;
