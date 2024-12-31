import { Card } from "~/chadcn/ui/card";
import { cn } from "~/utils";

type NutritionTabCardProps = React.HTMLAttributes<"div">;

function NutritionTabCard({
  className,
  children,
}: NutritionTabCardProps): JSX.Element {
  return (
    <Card
      className={cn(
        "flex flex-grow flex-col gap-4 overflow-auto border-0 sm:border-2 sm:p-8",
        className,
      )}
    >
      {children}
    </Card>
  );
}

export default NutritionTabCard;
