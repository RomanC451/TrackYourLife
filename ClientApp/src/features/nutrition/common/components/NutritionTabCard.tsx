import { Card } from "@/components/ui/card";
import { cn } from "@/lib/utils";

type NutritionTabCardProps = React.HTMLAttributes<"div">;

function NutritionTabCard({
  className,
  children,
}: NutritionTabCardProps): JSX.Element {
  return (
    <Card
      className={cn(
        "sm: m-4 flex flex-grow flex-col gap-4 overflow-auto border-0 shadow-none sm:border-2 sm:p-8 sm:shadow",
        className,
      )}
    >
      {children}
    </Card>
  );
}

export default NutritionTabCard;
