import { cn } from "@/lib/utils";

type PageCardProps = React.HTMLAttributes<HTMLDivElement>;

function PageCard({ className, children }: PageCardProps): React.JSX.Element {
  return (
    <div className="flex w-full justify-center">
      <div
        className={cn(
          "m-4 mt-0 flex max-w-[1500px] grow flex-col gap-4 overflow-visible rounded-none border-0 shadow-none @container/page-card sm:px-8",
          className,
        )}
      >
        {children}
      </div>
    </div>
  );
}

export default PageCard;
