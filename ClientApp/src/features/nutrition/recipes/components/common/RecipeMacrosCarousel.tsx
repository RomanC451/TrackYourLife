import { Badge } from "@/components/ui/badge";
import {
  Carousel,
  CarouselContent,
  CarouselDots,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { multiplyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { cn } from "@/lib/utils";
import { RecipeDto } from "@/services/openapi";

function RecipeMacrosCarousel({
  recipe,
  className,
}: {
  recipe: RecipeDto;
  className?: string;
}) {
  return (
    <Carousel className={cn("relative -ml-6 w-[110%] space-y-2", className)}>
      <div className="absolute right-6 z-10 space-x-2 rounded-2xl backdrop-blur-xl">
        <CarouselPrevious className="static transform-none" />
        <CarouselNext className="static transform-none" />
      </div>
      <CarouselContent className="ml-2">
        <CarouselItem>
          <div className="w-[calc(90%+1.5rem)] space-y-3">
            <Badge className="">Per portion:</Badge>
            <MacrosDialogHeader
              nutritionalContents={multiplyNutritionalContent(
                recipe.nutritionalContents,
                1 / recipe.portions,
              )}
            />
          </div>
        </CarouselItem>
        <CarouselItem>
          <div className="w-[calc(90%+1.5rem)] space-y-3">
            <Badge className="">Total:</Badge>
            <MacrosDialogHeader
              nutritionalContents={recipe.nutritionalContents}
            />
          </div>
        </CarouselItem>
      </CarouselContent>
      <CarouselDots className="mb-0" />
    </Carousel>
  );
}

export default RecipeMacrosCarousel;
