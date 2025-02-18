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
        "m-4 flex flex-grow flex-col gap-4 overflow-hidden rounded-none border-0 shadow-none sm:rounded-xl sm:border-2 sm:p-8 sm:shadow",
        className,
      )}
    >
      {children}
    </Card>
  );
}

export default NutritionTabCard;
