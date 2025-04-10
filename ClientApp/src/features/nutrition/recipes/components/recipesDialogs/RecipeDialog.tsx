import { DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { RecipeDto } from "@/services/openapi";

import IngredientsList from "../ingredientsList/IngredientsList";

import "./file.css";

import {
  Carousel,
  CarouselContent,
  CarouselDots,
  CarouselItem,
} from "@/components/ui/carousel";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { multiplyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import UpdateRecipeDialog from "./UpdateRecipeDialog";

type RecipeDialogProps = {
  recipe: RecipeDto;
  isPending: LoadingState;
};

function RecipeDialog({ recipe }: RecipeDialogProps): JSX.Element {
  return (
    <>
      <div className="inline-flex w-full items-center gap-1">
        <UpdateRecipeDialog recipe={recipe} />

        <div>
          <p className="text-xl font-bold">{recipe.name}</p>
          <p className="text-sm">{recipe.portions} portions</p>
        </div>
      </div>
      <Separator className="mt-1" />

      <Carousel className="pl-0">
        <CarouselContent>
          <CarouselItem className="space-y-2 pl-0">
            <p className="ml-2">Per portion:</p>
            <MacrosDialogHeader
              nutritionalContents={multiplyNutritionalContent(
                recipe.nutritionalContents,
                1 / recipe.portions,
              )}
            />
          </CarouselItem>
          <CarouselItem className="space-y-2 pl-0">
            <p className="ml-2">Total:</p>

            <MacrosDialogHeader
              nutritionalContents={recipe.nutritionalContents}
            />
          </CarouselItem>
        </CarouselContent>
        <CarouselDots className="mt-4" />
      </Carousel>

      <Separator />
      <IngredientsList recipe={recipe} />
    </>
  );
}

RecipeDialog.Loading = function () {
  return (
    <>
      <DialogTitle hidden>Loading</DialogTitle>
      <DialogDescription hidden> Edit recipe</DialogDescription>
      <Skeleton className="h-8 w-28" />
      <Separator />
      <MacrosDialogHeader.Loading />
      <Separator />
      <IngredientsList.Loading />
    </>
  );
};

export default RecipeDialog;
