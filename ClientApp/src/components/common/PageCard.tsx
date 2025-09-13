import { cn } from "@/lib/utils";

type PageCardProps = React.HTMLAttributes<"div">;

function PageCard({ className, children }: PageCardProps): JSX.Element {
  return (
    <div className="flex w-full justify-center">
      <div
        className={cn(
          "m-4 mt-0 flex max-w-[1500px] flex-grow flex-col gap-4 overflow-hidden rounded-none border-0 shadow-none @container/page-card sm:px-8",
          className,
        )}
      >
        {children}
      </div>
    </div>
  );
}

export default PageCard;
