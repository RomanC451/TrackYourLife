import { Card } from "@/components/ui/card";
import { cn } from "@/lib/utils";

type PageCardProps = React.HTMLAttributes<"div">;

function PageCard({ className, children }: PageCardProps): JSX.Element {
  return (
    <Card
      className={cn(
        "m-4 flex flex-grow flex-col gap-4 overflow-hidden rounded-none border-0 shadow-none @container/page-card sm:rounded-xl sm:border-2 sm:p-8 sm:shadow",
        className,
      )}
    >
      {children}
    </Card>
  );
}

export default PageCard;
