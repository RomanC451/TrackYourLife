import { useCallback } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Pencil, Trash2 } from "lucide-react";

import { router } from "@/App";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Skeleton } from "@/components/ui/skeleton";
import { getCalories } from "@/features/nutrition/common/utils/ingredients";
import { cn } from "@/lib/utils";
import { IngredientDto, RecipeDto } from "@/services/openapi";

import useRemoveIngredientsMutation from "../../mutations/useRemoveIngredientsMutation";

type IngredientListElementProps = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  selected: boolean;
  onSelect: (ingredientId: string) => void;
};

function IngredientListElement({
  recipe,
  ingredient,
  selected,
  onSelect,
}: IngredientListElementProps) {
  const removeIngredientsMutation = useRemoveIngredientsMutation();

  const calories = getCalories(ingredient);

  const handleDelete = useCallback(() => {
    removeIngredientsMutation.mutate({
      ingredients: [ingredient],
      recipe,
    });
  }, [ingredient, recipe, removeIngredientsMutation]);

  return (
    <Card
      key={ingredient.id}
      className={cn(
        "mb-4",
        (ingredient.isLoading || ingredient.isDeleting) && "opacity-50",
      )}
    >
      <CardContent className="p-4">
        <IngredientListElement.Header
          ingredient={ingredient}
          selected={selected}
          onSelect={onSelect}
        />
        <IngredientListElement.NutritionalInfo
          ingredient={ingredient}
          calories={calories}
        />
        <IngredientListElement.Actions
          recipe={recipe}
          ingredient={ingredient}
          onDelete={handleDelete}
        />
      </CardContent>
    </Card>
  );
}

type IngredientListElementHeaderProps = {
  ingredient: IngredientDto;
  selected: boolean;
  onSelect: (ingredientId: string) => void;
};

IngredientListElement.Header = function ({
  ingredient,
  selected,
  onSelect,
}: IngredientListElementHeaderProps) {
  return (
    <div className="mb-2 flex items-center justify-between">
      <div className="flex items-center space-x-2">
        <Checkbox
          checked={selected}
          onCheckedChange={() => onSelect(ingredient.id)}
          disabled={ingredient.isLoading || ingredient.isDeleting}
        />
        <h3 className="font-semibold">{ingredient.food.name}</h3>
      </div>
      <span className="text-sm text-muted-foreground">
        {parseFloat(
          (ingredient.quantity * ingredient.servingSize.value).toFixed(2),
        ).toString()}{" "}
        {ingredient.servingSize.unit}
      </span>
    </div>
  );
};

type IngredientListElementNutritionalInfoProps = {
  ingredient: IngredientDto;
  calories: number;
};

IngredientListElement.NutritionalInfo = function ({
  ingredient,
  calories,
}: IngredientListElementNutritionalInfoProps) {
  return (
    <div className="mb-2 flex items-start justify-between">
      <span className="text-sm font-medium">
        Calories: {calories.toFixed(2)}
      </span>
      <Accordion type="single" collapsible className="w-[275px]">
        <AccordionItem value="item-1">
          <AccordionTrigger className="py-0">
            <span className="text-sm">Nutritional Info</span>
          </AccordionTrigger>
          <AccordionContent className="mt-2">
            <div className="grid grid-cols-3 gap-2 text-sm">
              <div>Protein: {ingredient.food.nutritionalContents.protein}g</div>
              <div>
                Carbs: {ingredient.food.nutritionalContents.carbohydrates}g
              </div>
              <div>Fat: {ingredient.food.nutritionalContents.fat}g</div>
            </div>
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </div>
  );
};

type IngredientListElementActionsProps = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  onDelete: () => void;
};

IngredientListElement.Actions = function IngredientListElementActions({
  recipe,
  ingredient,
  onDelete,
}: IngredientListElementActionsProps) {
  const navigate = useNavigate();

  return (
    <div className="mt-4 flex justify-end space-x-2">
      {/* <EditIngredientDialog
        recipe={recipe}
        ingredient={ingredient}
        disabled={ingredient.isLoading || ingredient.isDeleting}
        dialogButtonContent={<Pencil />}
      /> */}
      <Button
        variant="outline"
        disabled={ingredient.isLoading || ingredient.isDeleting}
        className="size-9"
        onClick={() => {
          navigate({
            to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
            params: { recipeId: recipe.id, ingredientId: ingredient.id },
          });
        }}
        onMouseEnter={() => {
          router.preloadRoute({
            to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
            params: { recipeId: recipe.id, ingredientId: ingredient.id },
          });
        }}
        onTouchStart={() => {
          router.preloadRoute({
            to: "/nutrition/recipes/$recipeId/ingredients/edit/$ingredientId",
            params: { recipeId: recipe.id, ingredientId: ingredient.id },
          });
        }}
      >
        <Pencil />
      </Button>
      <Button
        variant="destructive"
        disabled={ingredient.isDeleting || ingredient.isLoading}
        className="size-9"
        onClick={onDelete}
      >
        <Trash2 />
      </Button>
    </div>
  );
};

IngredientListElement.Loading = function () {
  return (
    <Card className="mb-4">
      <CardContent className="p-4">
        <div className="mb-2 flex items-center justify-between">
          <div className="flex items-center space-x-2">
            <Skeleton className="h-8 w-8"></Skeleton>
            <Skeleton className="h-4 w-24"></Skeleton>
          </div>
          <Skeleton className="h-4 w-16"></Skeleton>
        </div>
        <div className="mb-2 flex items-center justify-between">
          <Skeleton className="h-4 w-16"></Skeleton>
          <Skeleton className="h-4 w-24"></Skeleton>
        </div>
        <div className="mt-4 flex justify-end space-x-2">
          <Skeleton className="h-4 w-16"></Skeleton>
          <Skeleton className="h-4 w-16"></Skeleton>
        </div>
      </CardContent>
    </Card>
  );
};

export default IngredientListElement;
