import { Pencil, Trash2 } from "lucide-react";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "~/chadcn/ui/accordion";
import { Button } from "~/chadcn/ui/button";
import { Card, CardContent } from "~/chadcn/ui/card";
import { Checkbox } from "~/chadcn/ui/checkbox";
import { Skeleton } from "~/chadcn/ui/skeleton";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { IngredientDto, RecipeDto } from "~/services/openapi";
import EditIngredientDialog from "./EditIngredientDialog";

type IngredientListElementProps = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  selected: boolean;
  handleSelect: (ingredientId: string) => void;
  handleDelete: (ingredient: IngredientDto) => void;
  isPending: LoadingState;
};

function IngredientListElement({
  recipe,
  ingredient,
  selected,
  handleSelect,
  handleDelete,
  isPending,
}: IngredientListElementProps): JSX.Element {
  const calories =
    ingredient.servingSize.nutritionMultiplier *
    ingredient.quantity *
    ingredient.food.nutritionalContents.energy.value;

  return (
    <>
      <Card key={ingredient.id} className="mb-4">
        <CardContent className="p-4">
          <div className="mb-2 flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <Checkbox
                checked={selected}
                onCheckedChange={() => handleSelect(ingredient.id)}
              />
              <h3 className="font-semibold">{ingredient.food.name}</h3>
            </div>
            <span className="text-sm text-muted-foreground">
              {ingredient.quantity * ingredient.servingSize.value}{" "}
              {ingredient.servingSize.unit}
            </span>
          </div>
          <div className="mb-2 flex items-start justify-between">
            <span className="text-sm font-medium">Calories: {calories}</span>
            <Accordion type="single" collapsible className="w-[275px]">
              <AccordionItem value="item-1">
                <AccordionTrigger className="py-0">
                  <span className="text-sm">Nutritional Info</span>
                </AccordionTrigger>
                <AccordionContent className="mt-2">
                  <div className="grid grid-cols-3 gap-2 text-sm">
                    <div>
                      Protein: {ingredient.food.nutritionalContents.protein}g
                    </div>
                    <div>
                      Carbs: {ingredient.food.nutritionalContents.carbohydrates}
                      g
                    </div>
                    <div>Fat: {ingredient.food.nutritionalContents.fat}g</div>
                  </div>
                </AccordionContent>
              </AccordionItem>
            </Accordion>
          </div>
          <div className="mt-4 flex justify-end space-x-2">
            <EditIngredientDialog recipe={recipe} ingredient={ingredient}>
              <Button size="sm" variant="outline">
                <Pencil className="mr-1 h-4 w-4" />
                Edit
              </Button>
            </EditIngredientDialog>
            <Button
              size="sm"
              variant="secondary"
              disabled={!isPending.isLoaded}
              onClick={() => handleDelete(ingredient)}
            >
              <Trash2 className="mr-1 h-4 w-4" />
              Delete
            </Button>
          </div>
        </CardContent>
      </Card>
    </>
  );
}

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
